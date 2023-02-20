using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Shared;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Media;
using XCloud.Sales.Services.Media;

namespace XCloud.Sales.Services.Catalog;

public interface IGoodsService : ISalesAppService
{
    Task<int> CountAsync();

    Task<GoodsDto> QueryByIdAsync(int goodsId);

    Task<GoodsDto[]> QueryByIdsAsync(int[] goodsIds);

    [Obsolete]
    Task SaveGoodsImagesV1Async(SaveGoodsImagesInput dto);

    Task SaveGoodsCombinationImagesV1Async(SaveGoodsImagesInput dto);

    Task<ApiResponse<Goods>> InsertAsync(Goods goods);

    Task<ApiResponse<Goods>> UpdateAsync(Goods dto);

    Task UpdateStatusAsync(UpdateGoodsStatusInput dto);

    Task<GoodsDto[]> AttachDataAsync(GoodsDto[] data, AttachGoodsDataInput dto);

    Task SetTagsAsync(SetGoodsTagsInput dto);

    Task<ApiResponse<Goods>> SetSeoNameAsync(int goodsId, string seoName);

    Task<GoodsDto> QueryBySeoNameAsync(string name);
}

public class GoodsService : SalesAppService, IGoodsService
{
    private readonly ISalesRepository<Goods> _goodsRepository;
    private readonly IPictureService _pictureService;

    public GoodsService(
        IPictureService pictureService,
        ISalesRepository<Goods> goodsRepository)
    {
        this._pictureService = pictureService;
        _goodsRepository = goodsRepository;
    }

    public async Task<int> CountAsync()
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var count = await db.Set<Goods>().AsNoTracking().CountAsync();

        return count;
    }

    public async Task<GoodsDto> QueryBySeoNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(QueryBySeoNameAsync));

        var db = await _goodsRepository.GetDbContextAsync();

        name = NormalizeSeoName(name);

        var goods = await db.Set<Goods>().OrderBy(x => x.CreationTime).FirstOrDefaultAsync(x => x.SeoName == name);

        if (goods == null)
            return null;

        var dto = ObjectMapper.Map<Goods, GoodsDto>(goods);

        return dto;
    }

    public async Task<ApiResponse<Goods>> SetSeoNameAsync(int goodsId, string seoName)
    {
        if (goodsId <= 0)
            throw new ArgumentNullException(nameof(goodsId));

        var db = await _goodsRepository.GetDbContextAsync();

        var goods = await db.Set<Goods>().FirstOrDefaultAsync(x => x.Id == goodsId);
        if (goods == null)
            throw new EntityNotFoundException(nameof(SetSeoNameAsync));

        if (string.IsNullOrWhiteSpace(seoName))
        {
            goods.SeoName = string.Empty;
        }
        else
        {
            goods.SeoName = NormalizeSeoName(seoName);

            if (await CheckSeoNameIsExistAsync(goods.SeoName, goods.Id))
                return new ApiResponse<Goods>().SetError("seo name is already taken");
        }

        await db.TrySaveChangesAsync();

        return new ApiResponse<Goods>(goods);
    }

    public string NormalizeSeoName(string seoName)
    {
        if (string.IsNullOrWhiteSpace(seoName))
            throw new ArgumentNullException(nameof(seoName));

        return seoName.Trim().RemoveWhitespace().ToLower();
    }

    public string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        return name.Trim().RemoveWhitespace();
    }

    private async Task<bool> CheckSeoNameIsExistAsync(string seoName, int? exceptGoodsId = null)
    {
        if (string.IsNullOrWhiteSpace(seoName))
            throw new ArgumentNullException(nameof(seoName));

        var query = await _goodsRepository.GetQueryableAsync();

        var name = NormalizeSeoName(seoName);

        query = query.Where(x => x.SeoName == name);

        if (exceptGoodsId != null)
            query = query.Where(x => x.Id != exceptGoodsId.Value);

        var exist = await query.AnyAsync();

        return exist;
    }

    public async Task SetTagsAsync(SetGoodsTagsInput dto)
    {
        if (dto.Id <= 0 || dto.TagIds == null)
            throw new ArgumentException(nameof(SetTagsAsync));

        var entities = dto.TagIds.Select(x => new TagGoods() { GoodsId = dto.Id, TagId = x }).ToArray();

        var db = await _goodsRepository.GetDbContextAsync();

        var set = db.Set<TagGoods>();

        var tagsMapping = await set.Where(x => x.GoodsId == dto.Id).ToArrayAsync();

        var toDeleted = tagsMapping.NotInBy(entities, x => x.TagId).ToArray();
        var toAdd = entities.NotInBy(tagsMapping, x => x.TagId).ToArray();

        if (toDeleted.Any())
            set.RemoveRange(toDeleted);

        if (toAdd.Any())
            set.AddRange(toAdd);

        await db.TrySaveChangesAsync();
    }

    public virtual async Task<GoodsDto> QueryByIdAsync(int goodsId)
    {
        if (goodsId <= 0)
            throw new ArgumentNullException(nameof(goodsId));

        var goods = await _goodsRepository.QueryOneAsync(x => x.Id == goodsId);

        if (goods == null)
            return null;

        var dto = ObjectMapper.Map<Goods, GoodsDto>(goods);

        return dto;
    }

    public async Task<GoodsDto[]> QueryByIdsAsync(int[] goodsIds)
    {
        if (ValidateHelper.IsEmptyCollection(goodsIds))
            return Array.Empty<GoodsDto>();

        var goods = await _goodsRepository.QueryManyAsNoTrackingAsync(x => goodsIds.Contains(x.Id));

        var response = this.ObjectMapper.MapArray<Goods, GoodsDto>(goods);

        return response;
    }

    public virtual async Task<ApiResponse<Goods>> InsertAsync(Goods goods)
    {
        if (!CheckGoodsForm(goods, out var error))
            throw new UserFriendlyException(error);

        goods.Name = NormalizeName(goods.Name);
        goods.SeoName = string.Empty;
        goods.CreationTime = Clock.Now;
        goods.LastModificationTime = goods.CreationTime;

        await _goodsRepository.InsertNowAsync(goods);

        return new ApiResponse<Goods>(goods);
    }

    private bool CheckGoodsForm(Goods dto, out string error)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        error = string.Empty;
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            error = "name is required";
            return false;
        }

        return true;
    }

    public virtual async Task<ApiResponse<Goods>> UpdateAsync(Goods dto)
    {
        var res = new ApiResponse<Goods>();

        if (!CheckGoodsForm(dto, out var error))
            throw new UserFriendlyException(error);

        dto.Name = NormalizeName(dto.Name);

        var db = await _goodsRepository.GetDbContextAsync();

        var goods = await db.Set<Goods>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (goods == null)
            throw new EntityNotFoundException(nameof(UpdateAsync));

        goods.Name = dto.Name;
        goods.ShortDescription = dto.ShortDescription;
        goods.FullDescription = dto.FullDescription;

        goods.CategoryId = dto.CategoryId;
        goods.BrandId = dto.BrandId;

        goods.IsNew = dto.IsNew;
        goods.IsHot = dto.IsHot;
        goods.StickyTop = dto.StickyTop;

        goods.Price = dto.Price;
        goods.CostPrice = dto.CostPrice;
        goods.StockQuantity = dto.StockQuantity;
        
        goods.AdminComment = dto.AdminComment;
        goods.AttributeType = dto.AttributeType;

        goods.LastModificationTime = Clock.Now;

        await db.TrySaveChangesAsync();

        return res.SetData(goods);
    }

    public async Task SaveGoodsImagesV1Async(SaveGoodsImagesInput dto)
    {
        if (dto == null || dto.PictureIdArray == null)
            throw new ArgumentNullException(nameof(dto.PictureIdArray));

        dto.CombinationId = default;
        await this.SaveGoodsCombinationImagesV1Async(dto);
    }

    public async Task SaveGoodsCombinationImagesV1Async(SaveGoodsImagesInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (dto.PictureIdArray == null)
            throw new ArgumentNullException(nameof(dto.PictureIdArray));

        int NormalizeCombinationId(int id) => Math.Max(id, 0);

        dto.CombinationId = NormalizeCombinationId(dto.CombinationId);

        var inputData = dto.PictureIdArray
            .Where(x => x.Id > 0)
            .Select((x) => new GoodsPicture()
            {
                GoodsId = dto.Id,
                CombinationId = dto.CombinationId,
                PictureId = x.Id,
                DisplayOrder = x.Index,
            }).ToArray();

        var db = await _goodsRepository.GetDbContextAsync();

        var set = db.Set<GoodsPicture>();

        var query = set.Where(x => x.GoodsId == dto.Id);
        if (dto.CombinationId > 0)
            query = query.Where(x => x.CombinationId == dto.CombinationId);
        else
            query = query.Where(x => x.CombinationId <= 0);

        var currentImages = await query.ToArrayAsync();

        string FingerPrint(GoodsPicture d) =>
            $"{d.GoodsId}.{NormalizeCombinationId(d.CombinationId)}.{d.PictureId}";

        var toDelete = currentImages.NotInBy(inputData, FingerPrint).ToArray();
        var toAdd = inputData.NotInBy(currentImages, FingerPrint).ToArray();
        var toModified = currentImages.InBy(inputData, FingerPrint).ToArray();

        //delete
        if (toDelete.Any())
            set.RemoveRange(toDelete);

        //add
        if (toAdd.Any())
            set.AddRange(toAdd);

        //modify
        foreach (var m in toModified)
        {
            var fp = FingerPrint(m);
            var intersect = inputData.FirstOrDefault(x => FingerPrint(x) == fp);
            if (intersect == null)
                continue;
            if (m.DisplayOrder != intersect.DisplayOrder)
                m.DisplayOrder = intersect.DisplayOrder;
        }

        await db.TrySaveChangesAsync();
    }

    private PictureDto BuildPictureDto(Picture picture, GoodsPicture goodsPicture)
    {
        var picDto = ObjectMapper.Map<Picture, PictureDto>(picture);
        picDto.GoodsId = goodsPicture.GoodsId;
        picDto.CombinationId = goodsPicture.CombinationId;
        picDto.DisplayOrder = goodsPicture.DisplayOrder;
        return picDto;
    }

    public async Task<GoodsDto[]> AttachDataAsync(GoodsDto[] data, AttachGoodsDataInput dto)
    {
        if (data == null || dto == null)
            throw new ArgumentNullException(nameof(data));
        if (!data.Any())
            return data;

        var goodsIds = data.Select(x => x.Id).Distinct().ToArray();

        var db = await _goodsRepository.GetDbContextAsync();

        if (dto.GoodsAttributes)
        {
            var attributes = await db.Set<GoodsAttribute>().AsNoTracking().Where(x => goodsIds.Contains(x.GoodsId))
                .ToArrayAsync();
            foreach (var m in data)
            {
                m.GoodsAttributes = attributes.Where(x => x.GoodsId == m.Id).ToArray();
            }
        }

        if (dto.Combination)
        {
            var combinations = await db.Set<GoodsSpecCombination>().AsNoTracking()
                .Where(x => goodsIds.Contains(x.GoodsId)).ToArrayAsync();

            foreach (var m in data)
            {
                var combinationResults = combinations.Where(x => x.GoodsId == m.Id).ToArray();

                m.GoodsSpecCombinations = combinationResults
                    .Select(x => ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(x)).ToArray();
            }
        }

        if (dto.Brand)
        {
            var brandIds = data.Where(x => x.BrandId > 0).Select(x => x.BrandId).Distinct().ToArray();
            if (brandIds.Any())
            {
                var brands = await db.Set<Brand>().AsNoTracking().Where(x => brandIds.Contains(x.Id))
                    .ToArrayAsync();
                foreach (var m in data)
                    m.Brand = brands.FirstOrDefault(x => x.Id == m.BrandId);
            }
        }

        if (dto.Category)
        {
            var categoryIds = data.Where(x => x.CategoryId > 0).Select(x => x.CategoryId).Distinct().ToArray();
            if (categoryIds.Any())
            {
                var categories = await db.Set<Category>().AsNoTracking().Where(x => categoryIds.Contains(x.Id))
                    .ToArrayAsync();
                foreach (var m in data)
                    m.Category = categories.FirstOrDefault(x => x.Id == m.CategoryId);
            }
        }

        if (dto.Tags)
        {
            var query = from tagGoods in db.Set<TagGoods>().AsNoTracking()
                join tag in db.Set<Tag>().AsNoTracking()
                    on tagGoods.TagId equals tag.Id
                select new { tagGoods, tag };
            query = query.Where(x => goodsIds.Contains(x.tagGoods.GoodsId));
            var tags = await query.ToArrayAsync();

            foreach (var m in data)
                m.Tags = tags.Where(x => x.tagGoods.GoodsId == m.Id).Select(x => x.tag).ToArray();
        }

        if (dto.Images)
        {
            var query = from goodsPicture in db.Set<GoodsPicture>().AsNoTracking()
                join picture in db.Set<Picture>().AsNoTracking()
                    on goodsPicture.PictureId equals picture.Id
                join combination in db.Set<GoodsSpecCombination>().AsNoTracking()
                    on goodsPicture.CombinationId equals combination.Id into combinationGrouping
                from combinationOrEmpty in combinationGrouping.DefaultIfEmpty()
                select new
                {
                    goodsPicture,
                    combinationOrEmpty,
                    picture
                };

            query = query.Where(x =>
                x.goodsPicture.CombinationId == 0 ||
                x.combinationOrEmpty.IsActive);
            query = query.Where(x => goodsIds.Contains(x.goodsPicture.GoodsId));

            var pics = await query
                .OrderBy(x => x.goodsPicture.DisplayOrder)
                .TakeUpTo5000().ToArrayAsync();

            foreach (var m in data)
            {
                var xdata = pics
                    .Where(x => x.goodsPicture.GoodsId == m.Id)
                    .OrderBy(x => x.goodsPicture.DisplayOrder)
                    .ToArray();

                var pictureList = new List<MallStorageMetaDto>();
                foreach (var x in xdata)
                {
                    var meta = await this._pictureService.DeserializePictureMetaAsync(x.picture);
                    meta.PictureId = x.picture.Id;
                    meta.GoodsId = x.goodsPicture.GoodsId;
                    meta.CombinationId = x.goodsPicture.CombinationId;
                    meta.DisplayOrder = x.goodsPicture.DisplayOrder;
                    meta.Simplify();
                    pictureList.Add(meta);
                }

                m.XPictures = pictureList.ToArray();

                //------------------------------------------------
                m.Pictures = xdata.Select(x => BuildPictureDto(x.picture, x.goodsPicture)).ToArray();

                foreach (var pic in m.Pictures)
                {
                    pic.StorageMeta = await _pictureService.DeserializePictureMetaAsync(pic);
                    //remove useless data
                    pic.StorageMeta.Simplify();
                    pic.ResourceData = null;
                }
            }
        }

        return data;
    }

    public async Task UpdateStatusAsync(UpdateGoodsStatusInput dto)
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var goods = await db.Set<Goods>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.GoodsId);
        if (goods == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.IsDeleted != null)
            goods.IsDeleted = dto.IsDeleted.Value;

        if (dto.Published != null)
            goods.Published = dto.Published.Value;

        if (dto.StickyTop != null)
            goods.StickyTop = dto.StickyTop.Value;

        if (dto.IsHot != null)
            goods.IsHot = dto.IsHot.Value;

        if (dto.IsNew != null)
            goods.IsNew = dto.IsNew.Value;

        goods.LastModificationTime = Clock.Now;

        await db.TrySaveChangesAsync();
    }
}