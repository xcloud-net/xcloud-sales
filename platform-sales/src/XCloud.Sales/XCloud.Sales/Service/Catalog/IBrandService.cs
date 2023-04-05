using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Shared.Extension;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Media;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Service.Media;

namespace XCloud.Sales.Service.Catalog;

public interface IBrandService : ISalesPagingIntAppService<Brand, BrandDto, QueryBrandDto>
{
    Task UpdateStatusAsync(UpdateBrandStatusInput dto);

    Task<BrandDto[]> AttachDataAsync(BrandDto[] data, AttachBrandDataInput dto);

    Task SetPictureIdAsync(int brandId, int pictureId);
}

public class BrandService : SalesPagingIntAppService<Brand, BrandDto, QueryBrandDto>, IBrandService
{
    private readonly ISalesRepository<Brand> _brandRepository;
    private readonly IPictureService _pictureService;

    public BrandService(
        ISalesRepository<Brand> brandRepository, IPictureService pictureService) : base(brandRepository)
    {
        _brandRepository = brandRepository;
        _pictureService = pictureService;
    }

    private string NormalizeName(string name)
    {
        return name?.Trim();
    }

    private string NormalizeKeywords(string keywords)
    {
        if (string.IsNullOrWhiteSpace(keywords))
            return string.Empty;

        keywords = keywords.Trim().ToLower().RemoveWhitespace();
        return keywords;
    }

    public async Task SetPictureIdAsync(int brandId, int pictureId)
    {
        if (brandId <= 0)
            throw new ArgumentNullException(nameof(brandId));

        var brand = await this._brandRepository.QueryOneAsync(x => x.Id == brandId);
        
        if (brand == null)
            throw new EntityNotFoundException(nameof(brand));

        brand.PictureId = pictureId;
        brand.LastModificationTime = this.Clock.Now;

        await this._brandRepository.UpdateAsync(brand);
    }

    public async Task<BrandDto[]> AttachDataAsync(BrandDto[] data, AttachBrandDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._brandRepository.GetDbContextAsync();
        if (dto.Picture)
        {
            var pictureIds = data
                .Where(x => x.PictureId > 0)
                .Select(x => x.PictureId).Distinct().ToArray();
            if (pictureIds.Any())
            {
                var pictures = await db.Set<Picture>().WhereIdIn(pictureIds).ToArrayAsync();
                foreach (var m in data)
                {
                    var pic = pictures.FirstOrDefault(x => x.Id == m.PictureId);
                    if (pic == null)
                        continue;
                    m.Picture = await this._pictureService.DeserializePictureMetaAsync(pic);
                    m.Picture.Simplify();
                }
            }
        }

        return data;
    }

    protected override async Task<IQueryable<Brand>> GetPagingQueryableAsync(DbContext db, QueryBrandDto dto)
    {
        await Task.CompletedTask;
        return db.Set<Brand>().AsNoTracking().IgnoreQueryFilters();
    }

    private async Task<IQueryable<Brand>> BuildQuery(QueryBrandDto dto)
    {
        var db = await _brandRepository.GetDbContextAsync();

        var query = db.Set<Brand>().AsNoTracking().IgnoreQueryFilters();

        if (dto.IsDeleted != null)
            query = query.Where(x => x.IsDeleted == dto.IsDeleted.Value);

        if (dto.ShowOnHomePage)
            query = query.Where(x => x.ShowOnPublicPage);

        if (!string.IsNullOrWhiteSpace(dto.Name))
            query = query.Where(x => x.Name.Contains(dto.Name) || x.MetaKeywords.Contains(dto.Name));

        if (!string.IsNullOrWhiteSpace(dto.StoreId))
        {
            var joinQuery = from brand in query
                join g in db.Set<Goods>().AsNoTracking()
                    on brand.Id equals g.BrandId into goodsGrouping
                from goods in goodsGrouping.DefaultIfEmpty()
                join m in db.Set<StoreGoodsMapping>().AsNoTracking()
                    on goods.Id equals m.GoodsCombinationId into mappingGrouping
                from mapping in mappingGrouping.DefaultIfEmpty()
                select new
                {
                    brand,
                    goods,
                    mapping
                };
            joinQuery = joinQuery.Where(x => x.mapping.StoreId == dto.StoreId);
            query = joinQuery.Select(x => x.brand);
        }

        if (dto.Published)
        {
            var joinQuery = from brand in query
                join g in db.Set<Goods>().AsNoTracking()
                    on brand.Id equals g.BrandId into goodsGrouping
                from goods in goodsGrouping.DefaultIfEmpty()
                select new
                {
                    brand,
                    goods,
                };
            joinQuery = joinQuery.Where(x => x.goods.Published);
            query = joinQuery.Select(x => x.brand);
        }

        return query;
    }

    protected override async Task<IQueryable<Brand>> GetPagingFilteredQueryableAsync(IQueryable<Brand> query,
        QueryBrandDto dto)
    {
        var q = await BuildQuery(dto);

        var brandIdsQuery = q.Select(x => x.Id).Distinct();

        var brandQuery = query.Where(x => brandIdsQuery.Contains(x.Id));

        return brandQuery;
    }

    protected override async Task<IOrderedQueryable<Brand>> GetPagingOrderedQueryableAsync(IQueryable<Brand> query,
        QueryBrandDto dto)
    {
        await Task.CompletedTask;
        return query.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.CreationTime);
    }

    public async Task UpdateStatusAsync(UpdateBrandStatusInput dto)
    {
        var db = await _brandRepository.GetDbContextAsync();

        var brand = await db.Set<Brand>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.BrandId);
        if (brand == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.Published != null)
            brand.Published = dto.Published.Value;

        if (dto.IsDeleted != null)
            brand.IsDeleted = dto.IsDeleted.Value;

        if (dto.ShowOnPublicPage != null)
            brand.ShowOnPublicPage = dto.ShowOnPublicPage.Value;

        await db.TrySaveChangesAsync();
    }

    protected override async Task CheckBeforeInsertAsync(BrandDto dto)
    {
        await Task.CompletedTask;
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new UserFriendlyException("brand name is required");
    }

    protected override async Task CheckBeforeUpdateAsync(BrandDto dto)
    {
        await Task.CompletedTask;
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new UserFriendlyException("brand name is required");
    }

    protected override async Task InitBeforeInsertAsync(Brand entity)
    {
        await base.InitBeforeInsertAsync(entity);

        entity.Name = this.NormalizeName(entity.Name);
        entity.Description = entity.Description?.Trim();
        entity.MetaKeywords = this.NormalizeKeywords(entity.MetaKeywords);
        entity.CreationTime = this.Clock.Now;

        if (await _brandRepository.AnyAsync(x => x.Name == entity.Name))
            throw new UserFriendlyException("brand is already exist");
    }

    protected override async Task ModifyFieldsForUpdateAsync(Brand brand, BrandDto dto)
    {
        await Task.CompletedTask;

        brand.Name = NormalizeName(dto.Name);
        brand.MetaKeywords = NormalizeKeywords(dto.MetaKeywords);
        brand.Description = dto.Description;
        brand.ShowOnPublicPage = dto.ShowOnPublicPage;
        brand.Published = dto.Published;

        brand.LastModificationTime = Clock.Now;

        if (await _brandRepository.AnyAsync(x => x.Id != brand.Id && x.Name == brand.Name))
            throw new UserFriendlyException("brand is already exist");
    }
}