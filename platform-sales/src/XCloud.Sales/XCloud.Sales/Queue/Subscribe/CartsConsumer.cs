using DotNetCore.CAP;
using XCloud.Sales.Services.ShoppingCart;

namespace XCloud.Sales.Queue.Subscribe;

[UnitOfWork]
public class CartsConsumer : SalesAppService, ICapSubscribe
{
    private readonly IShoppingCartService shoppingCartService;
    public CartsConsumer(IShoppingCartService shoppingCartService)
    {
        this.shoppingCartService = shoppingCartService;
    }

    [CapSubscribe(SalesMessageTopics.RemoveCartsAfterPlaceOrder)]
    public virtual async Task RemoveCartsAfterPlaceOrder(RemoveCartBySpecs dto)
    {
        await this.shoppingCartService.DeleteByGoodsSpecCombinationIdsAsync(dto);
    }
}