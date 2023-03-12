using XCloud.Sales.Application;
using XCloud.Sales.Service.Coupons;
using XCloud.Sales.Service.Orders.Validator;
using XCloud.Sales.Service.Promotion;

namespace XCloud.Sales.Service.Orders;

/// <summary>
/// estimate order price for shopping cart view
/// </summary>
public interface IOrderPriceEstimateService : ISalesAppService
{
    //
}

public class OrderPriceEstimateService : SalesAppService, IOrderPriceEstimateService
{
    private readonly IPlaceOrderService _placeOrderService;
    private readonly ICouponService _couponService;
    private readonly OrderUtils _orderUtils;
    private readonly IPromotionService _promotionService;
    private readonly PromotionUtils _promotionUtils;
    private readonly OrderConditionUtils _orderConditionUtils;

    public OrderPriceEstimateService(IPlaceOrderService placeOrderService, ICouponService couponService,
        IPromotionService promotionService, PromotionUtils promotionUtils, OrderConditionUtils orderConditionUtils,
        OrderUtils orderUtils)
    {
        _placeOrderService = placeOrderService;
        _couponService = couponService;
        _promotionService = promotionService;
        _promotionUtils = promotionUtils;
        _orderConditionUtils = orderConditionUtils;
        _orderUtils = orderUtils;
    }

    public async Task EstimateOrderMinPriceAsync(PlaceOrderRequestDto dto)
    {
        var orderInfo = await this._placeOrderService.BuildOrderEntitiesAsync(dto);

        await this._couponService.QueryUserCouponPagingAsync(new QueryUserCouponPagingInput()
        {
            UserId = dto.UserId,
            Page = 1,
            PageSize = 1000,
            SkipCalculateTotalCount = true,
        });

        throw new NotImplementedException();
    }
}