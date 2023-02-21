using XCloud.Core.Dto;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Service.Users;

public interface IFavoritesService : ISalesAppService
{
    Task<bool> CheckIsFavoritesAsync(int userId, int goodsId);

    Task<bool> CheckIsFavoritesAsync(int userId, int goodsId, CachePolicy cachePolicyOption);

    Task AddFavoritesAsync(int userId, int goodsId);

    Task DeleteFavoritesAsync(int userId, int goodsId);

    Task<PagedResponse<FavoritesDto>> QueryPagingAsync(QueryFavoritesInput dto);
}

public class FavoritesService : SalesAppService, IFavoritesService
{
    private readonly ISalesRepository<Favorites> _favoritesRepository;

    public FavoritesService(ISalesRepository<Favorites> favoritesRepository)
    {
        _favoritesRepository = favoritesRepository;
    }

    public async Task<bool> CheckIsFavoritesAsync(int userId, int goodsId)
    {
        var data = await _favoritesRepository.AnyAsync(x => x.UserId == userId && x.GoodsId == goodsId);
        return data;
    }

    public async Task<bool> CheckIsFavoritesAsync(int userId, int goodsId, CachePolicy cachePolicyOption)
    {
        var key = $"mall.user.{userId}.favorite.goods.{goodsId}.check";
        var option = new CacheOption<bool>(key, TimeSpan.FromMinutes(10));

        var response = await this.CacheProvider.ExecuteWithPolicyAsync(
            () => this.CheckIsFavoritesAsync(userId, goodsId),
            option, cachePolicyOption);

        return response;
    }

    public async Task<PagedResponse<FavoritesDto>> QueryPagingAsync(QueryFavoritesInput dto)
    {
        var db = await _favoritesRepository.GetDbContextAsync();

        var query = from favorite in db.Set<Favorites>().AsNoTracking()
            join g in db.Set<Goods>().AsNoTracking() on favorite.GoodsId equals g.Id into goodsGrouping
            from goods in goodsGrouping.DefaultIfEmpty()
            select new { favorite, goods };

        if (dto.userId > 0)
            query = query.Where(x => x.favorite.UserId == dto.userId);

        var count = 0;

        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var items = await query.OrderByDescending(x => x.favorite.CreationTime)
            .PageBy(dto.AsAbpPagedRequestDto()).ToArrayAsync();

        var list = new List<FavoritesDto>();

        foreach (var m in items)
        {
            var favorites = ObjectMapper.Map<Favorites, FavoritesDto>(m.favorite);
            if (m.goods != null)
                favorites.Goods = ObjectMapper.Map<Goods, GoodsDto>(m.goods);
            list.Add(favorites);
        }

        return new PagedResponse<FavoritesDto>(list, dto, count);
    }

    public async Task AddFavoritesAsync(int userId, int goodsId)
    {
        if (userId <= 0)
            throw new ArgumentNullException(nameof(userId));
        if (goodsId <= 0)
            throw new ArgumentNullException(nameof(goodsId));

        if (await CheckIsFavoritesAsync(userId, goodsId))
            return;

        var entity = new Favorites()
        {
            UserId = userId,
            GoodsId = goodsId,
            CreationTime = Clock.Now
        };

        await _favoritesRepository.InsertAsync(entity);
    }

    public async Task DeleteFavoritesAsync(int userId, int goodsId)
    {
        if (userId <= 0 || goodsId <= 0)
            throw new ArgumentNullException(nameof(DeleteFavoritesAsync));

        await _favoritesRepository.DeleteAsync(x => x.UserId == userId && x.GoodsId == goodsId);
    }
}