using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Service.ShoppingCart;

public interface IShoppingCartService : ISalesAppService
{
    Task DeleteByGoodsSpecCombinationIdsAsync(RemoveCartBySpecs dto);

    Task<ApiResponse<ShoppingCartItem>> AddShoppingCartAsync(AddShoppingCartInput dto);

    Task<ApiResponse<ShoppingCartItem>> UpdateShoppingCartItemAsync(int shoppingCartItemId, int newQuantity);

    Task DeleteShoppingCartAsync(int[] ids);

    Task<ShoppingCartItemDto[]> QueryUserShoppingCartsWithWarningsAsync(int userId, string storeId);

    Task<ShoppingCartItem[]> QueryUserShoppingCartAsync(int userId);

    Task<ShoppingCartItem[]> QueryUserShoppingCartAsync(int userId, CachePolicy cachePolicyOption);
}

public class ShoppingCartService : SalesAppService, IShoppingCartService
{
    private readonly ISalesRepository<ShoppingCartItem> _sciRepository;
    private readonly ISpecCombinationParser _goodsSpecParser;
    private readonly IOrderService _orderService;

    public ShoppingCartService(ISalesRepository<ShoppingCartItem> sciRepository,
        ISpecCombinationParser goodsSpecParser,
        IOrderService orderService)
    {
        _sciRepository = sciRepository;
        _goodsSpecParser = goodsSpecParser;
        this._orderService = orderService;
    }

    public async Task DeleteByGoodsSpecCombinationIdsAsync(RemoveCartBySpecs dto)
    {
        if (dto.UserId <= 0 || ValidateHelper.IsEmptyCollection(dto.GoodsSpecCombinationId))
            return;

        var db = await _sciRepository.GetDbContextAsync();

        var set = db.Set<ShoppingCartItem>();

        var data = await set
            .Where(x => x.UserId == dto.UserId)
            .OrderByDescending(x => x.CreationTime)
            .ToArrayAsync();

        var toRemove = data.Where(x => dto.GoodsSpecCombinationId.Contains(x.GoodsSpecCombinationId)).ToArray();

        if (toRemove.Any())
        {
            set.RemoveRange(toRemove);

            await db.TrySaveChangesAsync();

            await this.QueryUserShoppingCartAsync(dto.UserId, new CachePolicy() { Refresh = true });
        }
    }

    public async Task<ShoppingCartItemDto[]> QueryUserShoppingCartsWithWarningsAsync(int userId, string storeId)
    {
        var db = await _sciRepository.GetDbContextAsync();

        var query = from cart in db.Set<ShoppingCartItem>().AsNoTracking()
            join g in db.Set<Goods>().AsNoTracking()
                on cart.GoodsId equals g.Id into goodsGrouping
            from goods in goodsGrouping.DefaultIfEmpty()
            join c in db.Set<GoodsSpecCombination>().AsNoTracking()
                on cart.GoodsSpecCombinationId equals c.Id into goodsSpecCombinationGrouping
            from goodsSpecCombination in goodsSpecCombinationGrouping.DefaultIfEmpty()
            select new
            {
                cart,
                goods,
                goodsSpecCombination
            };

        query = query.Where(x => x.cart.UserId == userId);

        var data = await query.OrderByDescending(x => x.cart.CreationTime).TakeUpTo5000().ToArrayAsync();

        var res = new List<ShoppingCartItemDto>();

        foreach (var m in data)
        {
            var dto = ObjectMapper.Map<ShoppingCartItem, ShoppingCartItemDto>(m.cart);
            dto.Goods = ObjectMapper.Map<Goods, GoodsDto>(m.goods);
            dto.GoodsSpecCombination =
                ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(m.goodsSpecCombination);

            res.Add(dto);
        }

        var checkInput = BuildPlaceOrderCheckInputs(storeId, res.ToArray()).ToArray();
        var checkResults = await _orderService.CheckGoodsStockStatusAsync(checkInput);

        foreach (var m in res)
        {
            var result = checkResults.FirstOrDefault(x =>
                x.CheckInput.GoodsSpecCombinationId == m.GoodsSpecCombinationId &&
                x.CheckInput.Quantity == m.Quantity);

            if (result == null)
            {
                m.Waring = Array.Empty<string>();
            }
            else
            {
                m.Waring = result.Errors.ToArray();
            }
        }

        return res.ToArray();
    }

    IEnumerable<PlaceOrderCheckInput> BuildPlaceOrderCheckInputs(string storeId, ShoppingCartItem[] items)
    {
        foreach (var m in items)
        {
            yield return new PlaceOrderCheckInput()
            {
                StoreId = storeId,
                GoodsSpecCombinationId = m.GoodsSpecCombinationId,
                Quantity = m.Quantity
            };
        }
    }

    public async Task<ShoppingCartItem[]> QueryUserShoppingCartAsync(int userId,
        CachePolicy cachePolicyOption)
    {
        var key = $"mall.user.{userId}.shopping.cart.count";
        var option = new CacheOption<ShoppingCartItem[]>(key, TimeSpan.FromMinutes(30));

        var data = await this.CacheProvider.ExecuteWithPolicyAsync(() => this.QueryUserShoppingCartAsync(userId), option,
            cachePolicyOption);

        data ??= Array.Empty<ShoppingCartItem>();

        return data;
    }

    public async Task<ShoppingCartItem[]> QueryUserShoppingCartAsync(int userId)
    {
        var db = await _sciRepository.GetDbContextAsync();

        var data = await db.Set<ShoppingCartItem>().AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreationTime)
            .ToArrayAsync();

        return data;
    }

    public async Task DeleteShoppingCartAsync(int[] ids)
    {
        if (ValidateHelper.IsEmptyCollection(ids))
            return;

        var db = await _sciRepository.GetDbContextAsync();

        var set = db.Set<ShoppingCartItem>();

        var data = await set
            .Where(x => ids.Contains(x.Id))
            .OrderByDescending(x => x.CreationTime)
            .ToArrayAsync();

        if (data.Any())
        {
            set.RemoveRange(data);
            await db.SaveChangesAsync();

            //update cache
            var mallUserIds = data.Select(x => x.UserId).Distinct().ToArray();
            foreach (var m in mallUserIds)
            {
                await this.QueryUserShoppingCartAsync(m, new CachePolicy() { Refresh = true });
            }
        }
    }

    public async Task<ApiResponse<ShoppingCartItem>> AddShoppingCartAsync(AddShoppingCartInput dto)
    {
        if (dto.Quantity <= 0)
            throw new ArgumentNullException(nameof(dto.Quantity));

        var db = await _sciRepository.GetDbContextAsync();

        var combination = await db.Set<GoodsSpecCombination>().AsNoTracking()
            .Where(x => x.Id == dto.GoodsSpecCombinationId)
            .FirstOrDefaultAsync();

        if (combination == null)
            throw new EntityNotFoundException("goods combination not found");

        var set = db.Set<ShoppingCartItem>();

        var userCarts = await set.Where(x => x.UserId == dto.UserId).ToArrayAsync();

        var cart = userCarts.FirstOrDefault(x => x.GoodsSpecCombinationId == combination.Id);

        if (cart == null)
        {
            cart = new ShoppingCartItem()
            {
                UserId = dto.UserId,
                GoodsId = combination.GoodsId,
                GoodsSpecCombinationId = combination.Id,
                Quantity = dto.Quantity,
                CreationTime = this.Clock.Now,
            };
            set.Add(cart);
        }
        else
        {
            cart.Quantity += dto.Quantity;
            cart.Quantity = Math.Max(cart.Quantity, 1);
            cart.LastModificationTime = this.Clock.Now;
        }

        if (cart.Quantity > combination.StockQuantity)
            throw new UserFriendlyException("quantity");

        await db.TrySaveChangesAsync();

        await this.QueryUserShoppingCartAsync(dto.UserId, new CachePolicy() { Refresh = true });

        return new ApiResponse<ShoppingCartItem>(cart);
    }

    public virtual async Task<ApiResponse<ShoppingCartItem>> UpdateShoppingCartItemAsync(
        int shoppingCartItemId,
        int newQuantity)
    {
        var db = await _sciRepository.GetDbContextAsync();

        var set = db.Set<ShoppingCartItem>();

        var shoppingCartItem = await set.Where(sci => sci.Id == shoppingCartItemId).FirstOrDefaultAsync();

        if (shoppingCartItem == null)
            return new ApiResponse<ShoppingCartItem>().SetError("shopping cart is not exist");

        var combination = await db.Set<GoodsSpecCombination>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == shoppingCartItem.GoodsSpecCombinationId);

        if (combination == null)
            return new ApiResponse<ShoppingCartItem>().SetError("goods is not exist");

        shoppingCartItem.Quantity = Math.Max(1, newQuantity);
        shoppingCartItem.LastModificationTime = Clock.Now;

        await db.SaveChangesAsync();

        await this.QueryUserShoppingCartAsync(shoppingCartItem.UserId, new CachePolicy() { Refresh = true });

        return new ApiResponse<ShoppingCartItem>(shoppingCartItem);
    }
}