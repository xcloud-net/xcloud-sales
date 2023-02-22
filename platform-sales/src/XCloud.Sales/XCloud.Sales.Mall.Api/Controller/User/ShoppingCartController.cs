using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Logging;
using XCloud.Sales.Service.ShoppingCart;
using XCloud.Sales.Service.Stores;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controller.User;

/// <summary>
/// 购物车/收藏接口
/// </summary>
[Route("api/mall/shoppingcart")]
public class ShoppingCartController : ShopBaseController
{
    private readonly IGoodsService _goodsService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IGoodsPriceService _goodsPriceService;
    private readonly IUserGradeService _userGradeService;

    /// <summary>
    /// 构造器
    /// </summary>
    public ShoppingCartController(
        IGoodsService goodsService,
        IGoodsPriceService goodsPriceService,
        IShoppingCartService shoppingCartService,
        IUserGradeService userGradeService)
    {
        this._goodsPriceService = goodsPriceService;
        this._goodsService = goodsService;
        this._shoppingCartService = shoppingCartService;
        this._userGradeService = userGradeService;
    }

    [HttpPost("count")]
    public async Task<ApiResponse<int>> CountAsync()
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var data = await this._shoppingCartService.QueryUserShoppingCartAsync(loginUser.Id,
            new CachePolicy() { Cache = true });

        return new ApiResponse<int>(data.Length);
    }

    [HttpPost("list")]
    public async Task<ApiResponse<ShoppingCartItemDto[]>> ListShoppingCarts()
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var storeId = await this.CurrentStoreSelector.GetRequiredStoreIdAsync();

        var cart = await this._shoppingCartService.QueryUserShoppingCartsWithWarningsAsync(loginUser.Id, storeId);

        if (cart.Any())
        {
            var goods = cart.Select(x => x.Goods).WhereNotNull().ToArray();
            await this._goodsService.AttachDataAsync(goods, new AttachGoodsDataInput() { Images = true });

            var grade = await this._userGradeService.GetGradeByUserIdAsync(loginUser.Id);
            if (grade != null)
            {
                var combinations = cart.Select(x => x.GoodsSpecCombination).WhereNotNull().ToArray();

                await this._goodsPriceService.AttachGradePriceAsync(combinations, grade.Id);
            }
        }

        return new ApiResponse<ShoppingCartItemDto[]>(cart);
    }

    [HttpPost("add-v1")]
    public async Task<ApiResponse<ShoppingCartItem>> GoodsToCartV1([FromBody] AddShoppingCartInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        dto.UserId = loginUser.Id;
        var res = await _shoppingCartService.AddShoppingCartAsync(dto);

        await this.EventBusService.NotifyInsertActivityLog(new ActivityLog()
        {
            ActivityLogTypeId = (int)ActivityLogType.AddShoppingCart,
            UserId = loginUser.Id,
            SubjectIntId = dto.GoodsSpecCombinationId,
            SubjectType = ActivityLogSubjectType.GoodsCombination,
            Comment = "添加收藏"
        });

        return res;
    }

    [HttpPost("update")]
    public async Task<ApiResponse<ShoppingCartItem>> UpdateCartItem([FromBody] UpdateCartModel item)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        if (item.Quantity < 1)
            return new ApiResponse<ShoppingCartItem>().SetError("数量不能小于零");

        //get shopping cart item
        var cart = await this._shoppingCartService.QueryUserShoppingCartAsync(loginUser.Id);
        var sci = cart.FirstOrDefault(x => x.Id == item.Id);

        if (sci == null)
            return new ApiResponse<ShoppingCartItem>().SetError("shoppingcart item is deleted");

        var res = await this._shoppingCartService.UpdateShoppingCartItemAsync(sci.Id, item.Quantity);

        return res;
    }

    /// <summary>
    /// 删除购物车子项
    /// </summary>
    [HttpPost("delete")]
    public async Task<ApiResponse<object>> RemoveItem([FromBody] IdDto<int> dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var userCarts = await this._shoppingCartService.QueryUserShoppingCartAsync(loginUser.Id);

        var cart = userCarts.FirstOrDefault(x => x.Id == dto.Id);
        if (cart == null)
            throw new EntityNotFoundException(nameof(cart));

        await this._shoppingCartService.DeleteShoppingCartAsync(new[] { dto.Id });

        await this.EventBusService.NotifyInsertActivityLog(new ActivityLog()
        {
            ActivityLogTypeId = (int)ActivityLogType.DeleteShoppingCart,
            UserId = loginUser.Id,
            SubjectIntId = cart.GoodsSpecCombinationId,
            SubjectType = ActivityLogSubjectType.GoodsCombination,
            Comment = "删除购物车"
        });

        return new ApiResponse<object>();
    }
}