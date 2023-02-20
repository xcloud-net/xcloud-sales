using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Stores;

namespace XCloud.Sales.Services.Catalog;

public interface IGoodsSearchService : ISalesAppService
{
    Task UpdateGoodsKeywordsAsync(int goodsId);

    Task<ApiResponse<Tag[]>> QueryRelatedTagsAsync(SearchProductsInput dto);

    Task<ApiResponse<Brand[]>> QueryRelatedBrandsAsync(SearchProductsInput dto);

    Task<ApiResponse<Category[]>> QueryRelatedCategoriesAsync(SearchProductsInput dto);

    Task<PagedResponse<GoodsDto>> SearchGoodsV2Async(SearchProductsInput dto);

    Task<PagedResponse<GoodsSpecCombinationDto>> QueryGoodsCombinationPaginationAsync(
        QueryGoodsCombinationInput dto);
}

public class GoodsSearchService : SalesAppService, IGoodsSearchService
{
    private readonly ICategoryService _categoryService;
    private readonly ISalesRepository<Goods> _goodsRepository;
    private readonly IGoodsService _goodsService;

    public GoodsSearchService(
        ICategoryService categoryService,
        IGoodsService goodsService,
        ISalesRepository<Goods> goodsRepository)
    {
        this._goodsService = goodsService;
        this._categoryService = categoryService;
        _goodsRepository = goodsRepository;
    }

    public async Task<PagedResponse<GoodsSpecCombinationDto>> QueryGoodsCombinationPaginationAsync(
        QueryGoodsCombinationInput dto)
    {
        var db = await this._goodsRepository.GetDbContextAsync();

        var combinationQuery = db.Set<GoodsSpecCombination>().IgnoreQueryFilters().AsNoTracking();

        if (dto.IsDeleted != null)
            combinationQuery = combinationQuery.Where(x => x.IsDeleted == dto.IsDeleted.Value);

        if (dto.StockQuantityGreaterThanOrEqualTo != null)
            combinationQuery =
                combinationQuery.Where(x => x.StockQuantity >= dto.StockQuantityGreaterThanOrEqualTo.Value);

        if (dto.StockQuantityLessThanOrEqualTo != null)
            combinationQuery =
                combinationQuery.Where(x => x.StockQuantity <= dto.StockQuantityLessThanOrEqualTo.Value);

        if (!string.IsNullOrWhiteSpace(dto.Sku))
            combinationQuery = combinationQuery.Where(x => x.Sku == dto.Sku);

        if (dto.PriceMax != null)
            combinationQuery = combinationQuery.Where(x => x.Price <= dto.PriceMax.Value);

        if (dto.PriceMin != null)
            combinationQuery = combinationQuery.Where(x => x.Price >= dto.PriceMin);

        var goodsQuery = db.Set<Goods>().IgnoreQueryFilters().AsNoTracking();

        var joinQuery = from combination in combinationQuery
            join g in goodsQuery
                on combination.GoodsId equals g.Id into goodsGrouping
            from goods in goodsGrouping.DefaultIfEmpty()
            select new { combination, goods };

        if (!string.IsNullOrWhiteSpace(dto.Keywords))
        {
            joinQuery = joinQuery.Where(x =>
                x.goods.Name.Contains(dto.Keywords) ||
                x.goods.ShortDescription.Contains(dto.Keywords) ||
                x.combination.Name.Contains(dto.Keywords) ||
                x.combination.Description.Contains(dto.Keywords));
        }

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await joinQuery.CountAsync();

        var data = await joinQuery
            .OrderBy(x => x.combination.IsDeleted)
            .ThenByDescending(x => x.goods.LastModificationTime)
            .PageBy(dto.AsAbpPagedRequestDto()).ToArrayAsync();

        GoodsSpecCombinationDto BuildResponse(GoodsSpecCombination combination, Goods goodsOrNull)
        {
            var combinationDto = this.ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(combination);
            if (goodsOrNull != null)
                combinationDto.Goods = this.ObjectMapper.Map<Goods, GoodsDto>(goodsOrNull);
            return combinationDto;
        }

        var items = data.Select(x => BuildResponse(x.combination, x.goods)).ToArray();

        return new PagedResponse<GoodsSpecCombinationDto>(items, dto, count);
    }

    public async Task UpdateGoodsKeywordsAsync(int goodsId)
    {
        if (goodsId <= 0)
            throw new ArgumentNullException(nameof(UpdateGoodsKeywordsAsync));

        var db = await _goodsRepository.GetDbContextAsync();

        var goods = await db.Set<Goods>().FirstOrDefaultAsync(x => x.Id == goodsId);
        if (goods == null)
            throw new EntityNotFoundException(nameof(UpdateGoodsKeywordsAsync));

        var goodsDto = ObjectMapper.Map<Goods, GoodsDto>(goods);
        await _goodsService.AttachDataAsync(new[] { goodsDto },
            new AttachGoodsDataInput() { Brand = true, Category = true, Tags = true });

        var keywords = new List<string>();

        if (goodsDto.Brand != null)
            keywords.AddRange(new[] { goodsDto.Brand.Name, goodsDto.Brand.MetaKeywords });

        if (goodsDto.Category != null)
            keywords.AddRange(new[] { goodsDto.Category.Name });

        if (ValidateHelper.IsNotEmptyCollection(goodsDto.Tags))
        {
            keywords.AddRange(goodsDto.Tags.Select(x => x.Name));
        }

        keywords = keywords.Select(x => x?.ToLower().Trim().RemoveWhitespace()).WhereNotEmpty().ToList();
        goods.Keywords = string.Join("-", keywords);
        goods.Keywords = goods.Keywords.Substring(0, length: Math.Min(900, goods.Keywords.Length));

        await db.TrySaveChangesAsync();
    }

    public async Task<ApiResponse<Brand[]>> QueryRelatedBrandsAsync(SearchProductsInput dto)
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var goodsIdQuery = await BuildGoodsIdsQueryV2Async(db, dto);

        var idsQuery = await db.Set<Goods>().AsNoTracking()
            .Where(x => x.BrandId > 0 && goodsIdQuery.Contains(x.Id))
            .Select(x => x.BrandId)
            .Distinct()
            .Take(dto.PageSize).ToArrayAsync();

        var brands = await db.Set<Brand>().Where(x => idsQuery.Contains(x.Id)).Take(dto.PageSize).ToArrayAsync();

        return new ApiResponse<Brand[]>(brands);
    }

    public async Task<ApiResponse<Category[]>> QueryRelatedCategoriesAsync(SearchProductsInput dto)
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var goodsIdQuery = await BuildGoodsIdsQueryV2Async(db, dto);

        var idsQuery = await db.Set<Goods>().AsNoTracking()
            .Where(x => x.CategoryId > 0 && goodsIdQuery.Contains(x.Id))
            .Select(x => x.CategoryId)
            .Distinct()
            .Take(dto.PageSize).ToArrayAsync();

        var datas = await db.Set<Category>().Where(x => idsQuery.Contains(x.Id)).Take(dto.PageSize).ToArrayAsync();

        return new ApiResponse<Category[]>(datas);
    }

    public async Task<ApiResponse<Tag[]>> QueryRelatedTagsAsync(SearchProductsInput dto)
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var goodsIdQuery = await BuildGoodsIdsQueryV2Async(db, dto);

        var query = from goods in db.Set<Goods>().AsNoTracking()
            join tm in db.Set<TagGoods>().AsNoTracking()
                on goods.Id equals tm.GoodsId
            select new { goods, tm };

        query = query.Where(x => goodsIdQuery.Contains(x.goods.Id));

        var idsQuery = await query.Select(x => x.tm.TagId).Distinct()
            .Take(dto.PageSize).ToArrayAsync();

        var datas = await db.Set<Tag>().Where(x => idsQuery.Contains(x.Id)).Take(dto.PageSize).ToArrayAsync();

        return new ApiResponse<Tag[]>(datas);
    }

    public virtual async Task<PagedResponse<GoodsDto>> SearchGoodsV2Async(SearchProductsInput dto)
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var goodsIdQuery = await BuildGoodsIdsQueryV2Async(db, dto);

        var query = db.Set<Goods>().IgnoreQueryFilters().AsNoTracking().Where(x => goodsIdQuery.Contains(x.Id));

        var orderedQuery = query
            .OrderBy(x => x.IsDeleted)
            .ThenByDescending(x => x.StickyTop);

        if (dto.OrderBy == GoodsSortingEnum.PriceAsc)
        {
            orderedQuery = orderedQuery.ThenBy(p => p.MinPrice);
        }
        else if (dto.OrderBy == GoodsSortingEnum.PriceDesc)
        {
            orderedQuery = orderedQuery.ThenByDescending(p => p.MinPrice);
        }
        else if (dto.OrderBy == GoodsSortingEnum.CreationTime)
        {
            orderedQuery = orderedQuery.ThenByDescending(p => p.CreationTime);
        }
        else
        {
            orderedQuery = orderedQuery.ThenByDescending(p => p.LastModificationTime);
        }

        query = orderedQuery;

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
        {
            count = await query.CountAsync();
        }

        var items = await query.PageBy(dto.AsAbpPagedRequestDto()).ToArrayAsync();

        var goodsDtos = items.Select(x => ObjectMapper.Map<Goods, GoodsDto>(x)).ToArray();

        var list = new PagedResponse<GoodsDto>(goodsDtos, dto, count);

        return list;
    }

    async Task<IQueryable<int>> BuildGoodsIdsQueryV2Async(DbContext db, SearchProductsInput dto)
    {
        var joinQuery = from goods in db.Set<Goods>().IgnoreQueryFilters().AsNoTracking()
            select new
            {
                goods,
            };
        //goods query
        if (dto.IsDeleted != null)
            joinQuery = joinQuery.Where(x => x.goods.IsDeleted == dto.IsDeleted.Value);

        if (dto.IsPublished != null)
            joinQuery = joinQuery.Where(x => x.goods.Published == dto.IsPublished.Value);

        if (dto.IsNew != null)
            joinQuery = joinQuery.Where(x => x.goods.IsNew == dto.IsNew.Value);

        if (dto.IsHot != null)
            joinQuery = joinQuery.Where(x => x.goods.IsHot == dto.IsHot.Value);

        if (dto.BrandId != null)
            joinQuery = joinQuery.Where(q => q.goods.BrandId == dto.BrandId.Value);

        if (dto.WithoutBrand ?? false)
            joinQuery = joinQuery.Where(x => x.goods.BrandId <= 0);

        if (dto.WithoutCategory ?? false)
            joinQuery = joinQuery.Where(x => x.goods.CategoryId <= 0);

        if (dto.CategoryId != null && dto.CategoryId.Value > 0)
        {
            var ids = await _categoryService.QueryCategoryAndAllChildrenIdsWithCacheAsync(dto.CategoryId.Value);
            joinQuery = joinQuery.Where(x => ids.Contains(x.goods.CategoryId));
        }

        //tag query
        if (!string.IsNullOrWhiteSpace(dto.TagId))
        {
            var tagGoodsQuery = db.Set<TagGoods>().AsNoTracking().Select(x => new { x.TagId, x.GoodsId });
            var tagJoinQuery = from q in joinQuery
                join t in tagGoodsQuery
                    on q.goods.Id equals t.GoodsId
                select new { q, t };
            joinQuery = tagJoinQuery.Where(x => x.t.TagId == dto.TagId).Select(x => x.q);
        }

        //combination query
        var combinationQuery = db.Set<GoodsSpecCombination>().AsNoTracking().Select(x =>
            new { x.Id, x.Sku, x.GoodsId, x.Name, x.Description, x.Price, x.StockQuantity });
        var combinationJoinQuery = from q in joinQuery
            join combination in combinationQuery
                on q.goods.Id equals combination.GoodsId
            select new { q, combination };
        var originCombinationJoinQuery = combinationJoinQuery;
        if (!string.IsNullOrEmpty(dto.Sku))
            combinationJoinQuery = combinationJoinQuery.Where(p => p.combination.Sku == dto.Sku);

        if (dto.PriceMin != null && dto.PriceMin.Value > 0)
            combinationJoinQuery = combinationJoinQuery.Where(x => x.combination.Price >= dto.PriceMin.Value);

        if (dto.PriceMax != null && dto.PriceMax.Value > 0)
            combinationJoinQuery = combinationJoinQuery.Where(x => x.combination.Price <= dto.PriceMax.Value);

        if (dto.StockQuantityGreaterThanOrEqualTo != null)
            combinationJoinQuery = combinationJoinQuery.Where(x =>
                x.combination.StockQuantity >= dto.StockQuantityGreaterThanOrEqualTo.Value);

        if (dto.StockQuantityLessThanOrEqualTo != null)
            combinationJoinQuery = combinationJoinQuery.Where(x =>
                x.combination.StockQuantity <= dto.StockQuantityLessThanOrEqualTo.Value);

        //on sale store query
        if (!string.IsNullOrWhiteSpace(dto.StoreId))
        {
            var storeGoodsMappingQuery = from q in combinationJoinQuery
                join mapping in db.Set<StoreGoodsMapping>()
                    on q.combination.Id equals mapping.GoodsCombinationId
                select new { q, mapping };
            storeGoodsMappingQuery = storeGoodsMappingQuery.Where(x => x.mapping.StoreId == dto.StoreId);
            combinationJoinQuery = storeGoodsMappingQuery.Select(x => x.q);
        }

        //try set new query
        if (!ReferenceEquals(combinationJoinQuery, originCombinationJoinQuery))
            joinQuery = combinationJoinQuery.Select(x => x.q);

        //keywords query
        if (!string.IsNullOrWhiteSpace(dto.Keywords))
        {
            //tag and goods mapping
            var tagGoodsMappingQuery = from tm in db.Set<TagGoods>().AsNoTracking()
                join tag in db.Set<Tag>().AsNoTracking()
                    on tm.TagId equals tag.Id
                select new { tm.GoodsId, tag };

            //goods and tag join query
            var likeQuery = from q in joinQuery
                join tg in tagGoodsMappingQuery
                    on q.goods.Id equals tg.GoodsId into tagGrouping
                from tagMapping in tagGrouping.DefaultIfEmpty()
                select new { q, tagMapping };

            //query with like
            likeQuery = likeQuery.Where(x =>
                x.q.goods.Name.Contains(dto.Keywords) ||
                x.q.goods.ShortDescription.Contains(dto.Keywords) ||
                x.q.goods.Keywords.Contains(dto.Keywords) ||
                x.tagMapping.tag.Name == dto.Keywords);

            joinQuery = likeQuery.Select(x => x.q);
        }

        //cancel cross join
        var goodsIdQuery = joinQuery.Select(x => x.goods.Id).Distinct();

        return goodsIdQuery;
    }
}