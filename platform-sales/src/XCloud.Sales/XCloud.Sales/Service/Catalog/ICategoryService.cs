using XCloud.Core.Application;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Shared;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Media;
using XCloud.Sales.Service.Media;

namespace XCloud.Sales.Service.Catalog;

public interface ICategoryService : ISalesAppService
{
    Task SetPictureIdAsync(int brandId, int pictureId);

    Task TryFixTreeAsync();

    Task<CategoryDto> QueryBySeoNameAsync(string seoName);

    Task<int> QueryCountAsync();

    Task AdjustParentIdAsync(int id, int parentId);

    Task<Category[]> QueryAllAsync(string tenantId);

    Task UpdateStatusAsync(UpdateCategoryStatusInput dto);

    Task<CategoryDto[]> AttachDataAsync(CategoryDto[] data, CategoryAttachDataInput dto);

    Task UpdateCategoryNodePathAsync(int catId);

    Task<Category[]> QueryAllChildrenAsync(int catId);

    Task InsertCategoryAsync(Category category);

    Task UpdateCategoryAsync(Category category);

    Task<Category[]> QueryByParentIdAsync(int? parentId);

    Task<Category[]> QueryByParentIdAsync(int? parentId, CachePolicy cachePolicyOption);

    Task<CategoryDto> QueryByIdAsync(int id);

    Task<CategoryDto> QueryByIdAsync(int id, CachePolicy cachePolicyOption);
}

public class CategoryService : SalesAppService, ICategoryService
{
    private readonly ISalesRepository<Category> _categoryRepository;
    private readonly IPictureService _pictureService;

    public CategoryService(
        ISalesRepository<Category> categoryRepository,
        IPictureService pictureService)
    {
        _categoryRepository = categoryRepository;
        this._pictureService = pictureService;
    }

    public async Task SetPictureIdAsync(int brandId, int pictureId)
    {
        if (brandId <= 0)
            throw new ArgumentNullException(nameof(brandId));

        var brand = await this._categoryRepository.QueryOneAsync(x => x.Id == brandId);
        if (brand == null)
            throw new EntityNotFoundException(nameof(brand));

        brand.PictureId = pictureId;
        brand.LastModificationTime = this.Clock.Now;

        await this._categoryRepository.UpdateAsync(brand);
    }

    public async Task<CategoryDto> QueryBySeoNameAsync(string seoName)
    {
        if (string.IsNullOrWhiteSpace(seoName))
            throw new ArgumentNullException(nameof(QueryBySeoNameAsync));
        var normalizedName = this.NormalizeSeoName(seoName);

        var category = await this._categoryRepository.QueryOneAsync(x => x.SeoName == normalizedName);
        if (category == null)
            return null;

        return this.ObjectMapper.Map<Category, CategoryDto>(category);
    }

    private string NormalizeSeoName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalizedName = name.Trim().RemoveWhitespace();
        return normalizedName;
    }

    public async Task<int> QueryCountAsync()
    {
        var db = await _categoryRepository.GetDbContextAsync();

        var query = db.Set<Category>().AsNoTracking();

        var count = await query.CountAsync();

        return count;
    }

    public async Task AdjustParentIdAsync(int id, int parentId)
    {
        if (id <= 0)
            throw new ArgumentNullException(nameof(AdjustParentIdAsync));

        if (id == parentId)
            throw new ArgumentException("id & parent id should not be equal");

        if (parentId < 0)
            parentId = 0;

        var db = await this._categoryRepository.GetDbContextAsync();

        var set = db.Set<Category>();

        var entity = await set.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(AdjustParentIdAsync));

        if (parentId > 0)
        {
            var parent = await set.AsNoTracking().FirstOrDefaultAsync(x => x.Id == parentId);
            if (parent == null)
                throw new EntityNotFoundException("parent not found");
        }

        var originParentId = entity.ParentCategoryId;

        entity.ParentCategoryId = parentId;
        entity.LastModificationTime = this.Clock.Now;

        await this._categoryRepository.UpdateAsync(entity);

        await this.QueryByParentIdAsync(originParentId, new CachePolicy() { Refresh = true });
        await this.QueryByParentIdAsync(parentId, new CachePolicy() { Refresh = true });
        await this.UpdateCategoryNodePathAsync(entity.Id);
        var allChildren = await this.QueryAllChildrenAsync(entity.ParentCategoryId);
        foreach (var m in allChildren)
        {
            await this.UpdateCategoryNodePathAsync(m.Id);
        }
    }

    public async Task<CategoryDto> QueryByIdAsync(int id, CachePolicy cachePolicyOption)
    {
        var key = $"mall.category.{id}.dto";
        var option = new CacheOption<CategoryDto>(key, TimeSpan.FromMinutes(30));

        var data = await this.CacheProvider.ExecuteWithPolicyAsync(() => this.QueryByIdAsync(id), option,
            cachePolicyOption);

        return data;
    }

    public async Task<CategoryDto> QueryByIdAsync(int id)
    {
        var category = await _categoryRepository.QueryOneAsync(x => x.Id == id);

        if (category == null)
            return null;

        var dto = ObjectMapper.Map<Category, CategoryDto>(category);
        return dto;
    }

    public async Task<Category[]> QueryByParentIdAsync(int? parentId, CachePolicy cachePolicyOption)
    {
        var key = $"mall.category.{parentId ?? 0}.children.entity";

        var option = new CacheOption<Category[]>(key, TimeSpan.FromMinutes(30));

        var response = await this.CacheProvider.ExecuteWithPolicyAsync(() => this.QueryByParentIdAsync(parentId),
            option,
            cachePolicyOption);

        response ??= Array.Empty<Category>();

        return response;
    }

    public async Task<Category[]> QueryByParentIdAsync(int? parentId)
    {
        var db = await _categoryRepository.GetDbContextAsync();

        var query = db.Set<Category>().AsNoTracking();

        if (parentId != null && parentId.Value > 0)
        {
            query = query.Where(x => x.ParentCategoryId == parentId.Value);
        }
        else
        {
            query = query.Where(x => x.ParentCategoryId <= 0);
        }

        var data = await query.OrderBy(x => x.CreationTime).ToArrayAsync();

        return data;
    }

    public async Task<CategoryDto[]> AttachDataAsync(CategoryDto[] data, CategoryAttachDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await _categoryRepository.GetDbContextAsync();

        if (dto.Picture)
        {
            var picIds = data.Where(x => x.PictureId > 0).Select(x => x.PictureId).Distinct().ToArray();
            if (picIds.Any())
            {
                var pics = await db.Set<Picture>().AsNoTracking().Where(x => picIds.Contains(x.Id)).ToArrayAsync();
                foreach (var m in data)
                {
                    var pic = pics.FirstOrDefault(x => x.Id == m.PictureId);
                    if (pic == null)
                        continue;

                    m.Picture = await _pictureService.DeserializePictureMetaAsync(pic);
                    m.Picture.Simplify();
                }
            }
        }

        if (dto.ParentNodes)
        {
            foreach (var m in data)
                m.ParentNodesIds = TryDeserializeNodePath(m.NodePath).Where(x => x > 0).ToArray();
            var nodes = data.SelectMany(x => x.ParentNodesIds).Distinct().ToArray();

            var allNodes = await db.Set<Category>().AsNoTracking().Where(x => nodes.Contains(x.Id)).ToArrayAsync();

            IEnumerable<Category> ResolveCategory(int[] ids)
            {
                foreach (var id in ids)
                {
                    var cat = allNodes.FirstOrDefault(x => x.Id == id);
                    if (cat != null)
                        yield return cat;
                }
            }

            foreach (var m in data)
                m.ParentNodes = ResolveCategory(m.ParentNodesIds).ToArray();
        }

        return data;
    }

    private int[] TryDeserializeNodePath(string nodePath)
    {
        try
        {
            return DeserializeNodePath(nodePath);
        }
        catch (Exception e)
        {
            Logger.LogError(message: e.Message, exception: e);
            return Array.Empty<int>();
        }
    }

    private int[] DeserializeNodePath(string nodePath)
    {
        nodePath = nodePath?.Trim();

        if (string.IsNullOrWhiteSpace(nodePath))
            return Array.Empty<int>();

        if (nodePath.StartsWith('[') && nodePath.EndsWith(']'))
        {
            var path = JsonDataSerializer.DeserializeFromString<int[]>(nodePath);
            return path;
        }

        var res = nodePath.Split(',', '|', ' ', ';', '.').Where(x => !string.IsNullOrEmpty(x))
            .Select(x => int.Parse(x)).ToArray();
        return res;
    }

    private string SerializeNodePath(int[] nodes)
    {
        return JsonDataSerializer.SerializeToString(nodes);
    }

    public async Task<Category[]> QueryNodePathAsync(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        var handledIds = new List<int>();
        var nodePath = new List<Category>();

        async Task FindParent(Category cat)
        {
            if (handledIds.Contains(cat.Id))
                return;
            handledIds.Add(cat.Id);

            nodePath.Insert(0, cat);

            var parent = await this.QueryByIdAsync(cat.ParentCategoryId);
            if (parent != null)
                await FindParent(parent);
        }

        await FindParent(category);

        return nodePath.ToArray();
    }

    public async Task<Category[]> QueryAllChildrenAsync(int catId)
    {
        var categories = new List<Category>();
        var handledIds = new List<int>();

        async Task FindChildren(Category cat)
        {
            if (handledIds.Contains(cat.Id))
                return;
            handledIds.Add(cat.Id);

            categories.Add(cat);

            var children = await this.QueryByParentIdAsync(cat.Id);

            foreach (var m in children)
                await FindChildren(m);
        }

        var children = await this.QueryByParentIdAsync(catId);

        foreach (var m in children)
            await FindChildren(m);

        return categories.ToArray();
    }

    public async Task UpdateStatusAsync(UpdateCategoryStatusInput dto)
    {
        var db = await _categoryRepository.GetDbContextAsync();

        var cat = await db.Set<Category>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.CategoryId);
        if (cat == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.Published != null)
            cat.Published = dto.Published.Value;

        if (dto.IsDeleted != null)
            cat.IsDeleted = dto.IsDeleted.Value;

        if (dto.ShowOnHomePage != null)
            cat.ShowOnHomePage = dto.ShowOnHomePage.Value;

        if (dto.Recommend != null)
            cat.Recommend = dto.Recommend.Value;

        cat.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();

        await this.QueryByIdAsync(cat.Id, new CachePolicy() { Refresh = true });
    }

    public async Task<Category[]> QueryAllAsync(string tenantId)
    {
        var db = await _categoryRepository.GetDbContextAsync();

        var data = await db.Set<Category>().AsNoTracking().TakeUpTo5000().ToArrayAsync();

        return data;
    }

    private async Task<bool> CheckSeoNameIsExistAsync(DbContext db, string normalizedName, int? exceptIdOrNull)
    {
        var query = db.Set<Category>().AsNoTracking().IgnoreQueryFilters();

        query = query.Where(x => x.SeoName == normalizedName);

        if (exceptIdOrNull != null)
            query = query.Where(x => x.Id != exceptIdOrNull.Value);

        return await query.AnyAsync();
    }

    private async Task HandleWithDeadNodesAsync()
    {
        var allData = await this.QueryAllAsync(null);
        var deadNodes = new List<int>();
        foreach (var m in allData)
        {
            if (deadNodes.Contains(m.Id))
                continue;

            var nodeIds = new List<int>() { m.Id };
            var parentId = m.ParentCategoryId;
            while (true)
            {
                var parent = allData.FirstOrDefault(x => x.Id == parentId);
                if (parent == null)
                    break;
                //add node
                if (nodeIds.Contains(parent.Id))
                    break;
                nodeIds.Add(parent.Id);
                //next loop
                parentId = parent.ParentCategoryId;
            }

            //the first node is not root
            if (parentId > 0 && nodeIds.Any())
            {
                deadNodes.AddRange(nodeIds);
            }
        }

        //improve update by batch
        if (deadNodes.Any())
        {
            this.Logger.LogWarning("category:find dead nodes");
            var db = await this._categoryRepository.GetDbContextAsync();
            var nodes = await db.Set<Category>().Where(x => deadNodes.Contains(x.Id)).ToArrayAsync();

            var now = this.Clock.Now;
            foreach (var m in nodes)
            {
                m.ParentCategoryId = default;
                m.LastModificationTime = now;
            }

            await db.TrySaveChangesAsync();
        }
    }

    public async Task TryFixTreeAsync()
    {
        using var dlock =
            await this.RedLockClient.RedLockFactory.CreateLockAsync(resource: "mall.category.try.fix.tree",
                expiryTime: TimeSpan.FromSeconds(30));

        if (dlock.IsAcquired)
        {
            //handle with dead nodes
            //await this.HandleWithDeadNodesAsync();

            //refresh root
            await this.QueryByParentIdAsync(null, new CachePolicy() { Refresh = true });
            var allData = await this.QueryAllAsync(null);
            foreach (var m in allData)
            {
                try
                {
                    //node
                    await this.QueryByIdAsync(m.Id, new CachePolicy() { Refresh = true });
                    //children
                    await this.QueryByParentIdAsync(m.Id, new CachePolicy() { Refresh = true });
                    //node path
                    await this.UpdateCategoryNodePathAsync(m.Id);
                }
                catch (Exception e)
                {
                    this.Logger.LogError(message: e.Message, exception: e);
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
            }
        }
        else
        {
            throw new FailToGetRedLockException(nameof(TryFixTreeAsync));
        }

        await Task.CompletedTask;
    }

    public virtual async Task InsertCategoryAsync(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));
        var db = await this._categoryRepository.GetDbContextAsync();

        if (category.ParentCategoryId > 0)
        {
            var parent = await this.QueryByIdAsync(category.ParentCategoryId);
            if (parent == null)
                category.ParentCategoryId = 0;
        }

        category.SeoName = this.NormalizeSeoName(category.SeoName);
        if (!string.IsNullOrWhiteSpace(category.SeoName))
            if (await this.CheckSeoNameIsExistAsync(db, category.SeoName, null))
                throw new UserFriendlyException("seo name exist already");

        category.CreationTime = Clock.Now;
        category.LastModificationTime = category.CreationTime;

        await _categoryRepository.InsertAsync(category, autoSave: true);

        await this.UpdateCategoryNodePathAsync(category.Id);
        await this.QueryByParentIdAsync(category.ParentCategoryId, new CachePolicy() { Refresh = true });
    }

    public async Task UpdateCategoryNodePathAsync(int catId)
    {
        var db = await _categoryRepository.GetDbContextAsync();
        var cat = await db.Set<Category>().FirstOrDefaultAsync(x => x.Id == catId);
        if (cat == null)
            return;

        var nodePath = await QueryNodePathAsync(cat);
        var nodes = nodePath.Select(x => x.Id).ToArray();

        cat.RootId = nodes.FirstOrDefault();
        cat.NodePath = SerializeNodePath(nodes);

        await db.SaveChangesAsync();

        await this.QueryByIdAsync(cat.Id, new CachePolicy() { Refresh = true });
    }

    public virtual async Task UpdateCategoryAsync(Category dto)
    {
        var category = await _categoryRepository.QueryOneAsync(x => x.Id == dto.Id);
        if (category == null)
            throw new EntityNotFoundException(nameof(UpdateCategoryAsync));

        var db = await this._categoryRepository.GetDbContextAsync();

        category.Name = dto.Name;
        category.SeoName = dto.SeoName;
        category.Description = dto.Description;
        category.DisplayOrder = dto.DisplayOrder;
        category.LastModificationTime = Clock.Now;

        category.SeoName = this.NormalizeSeoName(category.SeoName);
        if (!string.IsNullOrWhiteSpace(category.SeoName))
            if (await this.CheckSeoNameIsExistAsync(db, category.SeoName, category.Id))
                throw new UserFriendlyException("seo name exist already");

        await _categoryRepository.UpdateAsync(category, autoSave: true);

        await this.QueryByIdAsync(category.Id, new CachePolicy() { Refresh = true });
    }
}