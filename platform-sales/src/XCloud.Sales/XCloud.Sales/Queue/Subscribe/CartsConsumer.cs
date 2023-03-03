using DotNetCore.CAP;
using XCloud.Sales.Application;
using XCloud.Sales.Service.ShoppingCart;

namespace XCloud.Sales.Queue.Subscribe;

[UnitOfWork]
public class CartsConsumer : SalesAppService, ICapSubscribe
{
    private readonly IShoppingCartService _shoppingCartService;
    public CartsConsumer(IShoppingCartService shoppingCartService)
    {
        this._shoppingCartService = shoppingCartService;
    }

    [CapSubscribe(SalesMessageTopics.RemoveCartsAfterPlaceOrder)]
    public virtual async Task RemoveCartsAfterPlaceOrder(RemoveCartBySpecs dto)
    {
        await this._shoppingCartService.DeleteByGoodsSpecCombinationIdsAsync(dto);
    }
}