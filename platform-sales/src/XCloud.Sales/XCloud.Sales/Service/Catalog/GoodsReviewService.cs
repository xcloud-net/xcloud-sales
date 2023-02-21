using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Service.Catalog;

public interface IGoodsReviewService : ISalesAppService
{
    Task UpdateGoodsReviewTotalsAsync(int goodsId);

    Task<PagedResponse<GoodsReview>> QueryPagingAsync(QueryGoodsReviewInput dto);
}

public class GoodsReviewService : SalesAppService, IGoodsReviewService
{
    private readonly ISalesRepository<Goods> _goodsRepository;
    private readonly ISalesRepository<GoodsReview> _goodsReviewRepository;

    public GoodsReviewService(
        ISalesRepository<Goods> goodsRepository,
        ISalesRepository<GoodsReview> goodsReviewRepository)
    {
        _goodsRepository = goodsRepository;
        _goodsReviewRepository = goodsReviewRepository;
    }

    public virtual async Task UpdateGoodsReviewTotalsAsync(int goodsId)
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var goods = await db.Set<Goods>().FirstOrDefaultAsync(x => x.Id == goodsId);

        if (goods == null)
            throw new EntityNotFoundException(nameof(UpdateGoodsReviewTotalsAsync));

        goods.ApprovedTotalReviews = await db.Set<GoodsReview>().AsNoTracking().Where(x => x.GoodsId == goods.Id).CountAsync();

        await db.TrySaveChangesAsync();
    }

    public async Task<PagedResponse<GoodsReview>> QueryPagingAsync(QueryGoodsReviewInput dto)
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var joinQuery = from review in db.Set<GoodsReview>().AsNoTracking()

            join g in db.Set<Goods>().AsNoTracking().IgnoreQueryFilters()
                on review.GoodsId equals g.Id into goodsGrouping
            from goods in goodsGrouping.DefaultIfEmpty()

            join o in db.Set<Order>().AsNoTracking().IgnoreQueryFilters()
                on review.OrderId equals o.Id into orderGrouping
            from order in orderGrouping.DefaultIfEmpty()

            select new
            {
                review,
                goods,
                order
            };

        if (!string.IsNullOrWhiteSpace(dto.StoreId))
            joinQuery = joinQuery.Where(x => x.order.StoreId == dto.StoreId);

        var query = joinQuery.Select(x => x.review);

        if (dto.GoodsId.HasValue && dto.GoodsId.Value > 0)
            query = query.Where(o => o.GoodsId == dto.GoodsId.Value);
        if (dto.UserId.HasValue)
            query = query.Where(o => o.UserId == dto.UserId.Value);
        if (!string.IsNullOrWhiteSpace(dto.OrderId))
            query = query.Where(o => o.OrderId == dto.OrderId);
        if (dto.StartTime.HasValue)
            query = query.Where(o => dto.StartTime.Value <= o.CreationTime);
        if (dto.EndTime.HasValue)
            query = query.Where(o => dto.EndTime.Value >= o.CreationTime);

        var total = 0;
        if (!dto.SkipCalculateTotalCount)
            total = await query.CountAsync();
            
        query = query.OrderByDescending(c => c.CreationTime);
        var items = await query.PageBy(dto.AsAbpPagedRequestDto()).ToArrayAsync();

        var reviews = new PagedResponse<GoodsReview>(items, dto, total);

        return reviews;
    }

}