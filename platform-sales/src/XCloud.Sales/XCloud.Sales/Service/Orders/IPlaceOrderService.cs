using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Platform.Common.Application.Service.Address;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Core;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Shipping;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Common;
using XCloud.Sales.Service.Configuration;
using XCloud.Sales.Service.Coupons;
using XCloud.Sales.Service.Orders.Validator;
using XCloud.Sales.Service.Promotion;
using XCloud.Sales.Service.ShoppingCart;
using XCloud.Sales.Service.Stores;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Orders;

public interface IPlaceOrderService : ISalesAppService
{
    Task<KeyValuePair<Order, OrderItem[]>> BuildOrderEntitiesAsync(PlaceOrderRequestDto dto);

    Task<PlaceOrderResult> PlaceOrderAsync(PlaceOrderRequestDto processRequest);
}

/// <summary>
/// todo 单独一个方法用来计算商品价格
/// </summary>
public class PlaceOrderService : SalesAppService, IPlaceOrderService
{
    private readonly OrderUtils _orderUtils;
    private readonly CouponUtils _couponUtils;
    private readonly IOrderService _orderService;
    private readonly IWebHelper _webHelper;
    private readonly IUserService _userService;
    private readonly IAddressService _addressService;
    private readonly ISalesRepository<Order> _orderRepository;
    private readonly IMallSettingService _mallSettingService;
    private readonly ISpecCombinationService _specCombinationService;
    private readonly IGradeGoodsPriceService _gradeGoodsPriceService;
    private readonly IStoreGoodsPriceService _storeGoodsPriceService;
    private readonly IUserGradeService _userGradeService;
    private readonly IGoodsStockService _goodsStockService;
    private readonly PromotionUtils _promotionUtils;
    private readonly IPromotionService _promotionService;
    private readonly ICouponService _couponService;
    private readonly OrderConditionUtils _orderConditionUtils;

    public PlaceOrderService(PromotionUtils promotionUtils,
        OrderConditionUtils orderConditionUtils,
        IPromotionService promotionService,
        IGoodsStockService goodsStockService,
        IUserGradeService userGradeService,
        IMallSettingService mallSettingService,
        OrderUtils orderUtils,
        IOrderService orderService,
        IWebHelper webHelper,
        IUserService userService,
        IAddressService addressService,
        ICouponService couponService,
        ISalesRepository<Order> orderRepository,
        IGradeGoodsPriceService gradeGoodsPriceService,
        IStoreGoodsPriceService storeGoodsPriceService,
        ISpecCombinationService specCombinationService, CouponUtils couponUtils)
    {
        this._orderConditionUtils = orderConditionUtils;
        this._promotionService = promotionService;
        this._promotionUtils = promotionUtils;
        this._goodsStockService = goodsStockService;
        this._userGradeService = userGradeService;
        this._orderUtils = orderUtils;
        this._mallSettingService = mallSettingService;
        this._orderRepository = orderRepository;
        _gradeGoodsPriceService = gradeGoodsPriceService;
        _storeGoodsPriceService = storeGoodsPriceService;
        _specCombinationService = specCombinationService;
        _couponUtils = couponUtils;
        this._orderService = orderService;
        this._webHelper = webHelper;
        this._userService = userService;
        this._addressService = addressService;
        this._couponService = couponService;
    }


    [UnitOfWork(isTransactional: true)]
    public virtual async Task<PlaceOrderResult> PlaceOrderAsync(PlaceOrderRequestDto processRequest)
    {
        var settings = await this._mallSettingService.GetCachedMallSettingsAsync();
        if (settings.PlaceOrderDisabled)
            throw new UserFriendlyException("admin disabled placing order");

        var key = processRequest.FingerPrint();

        using var @lock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: key,
            expiryTime: TimeSpan.FromSeconds(5));

        if (@lock.IsAcquired)
        {
            return await this.PlaceOrderImplAsync(processRequest);
        }
        else
        {
            throw new FailToGetRedLockException("pls try later");
        }
    }

    private bool CheckFormOfPlaceOrderRequest(PlaceOrderRequestDto dto, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (dto.UserId <= 0)
        {
            errorMessage = "user not found";
            return false;
        }

        return true;
    }

    private bool CheckGoodsInfoOfPlaceOrderRequest(PlaceOrderRequestDto dto, out string errorMessage)
    {
        if (ValidateHelper.IsEmptyCollection(dto.Items))
        {
            errorMessage = "empty specs";
            return false;
        }

        if (dto.Items.Any(x => x.GoodsSpecCombinationId <= 0))
        {
            errorMessage = "spec combination is empty";
            return false;
        }

        if (dto.Items.Any(x => x.Quantity < 1))
        {
            errorMessage = "the min quantity is 1";
            return false;
        }

        if (dto.Items.GroupBy(x => x.FingerPrint()).Any(x => x.Count() > 1))
        {
            errorMessage = "duplicate input";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    private async Task<UserAddressDto> GetUserAddressOrNullAsync(PlaceOrderRequestDto dto, StoreUserDto storeUser)
    {
        if (string.IsNullOrWhiteSpace(dto.AddressId))
            return null;

        var allAddress = await this._addressService.QueryByGlobalUserId(storeUser.GlobalUserId);

        var shippingAddress = allAddress.FirstOrDefault(x => x.Id == dto.AddressId);

        if (shippingAddress == null)
            throw new UserFriendlyException("the shipping address not found");

        return shippingAddress;
    }

    private IEnumerable<PlaceOrderCheckInput> BuildPlaceOrderCheckInputs(PlaceOrderRequestDto dto)
    {
        foreach (var m in dto.Items)
        {
            yield return new PlaceOrderCheckInput()
            {
                Quantity = m.Quantity,
                GoodsSpecCombinationId = m.GoodsSpecCombinationId,
                StoreId = dto.StoreId
            };
        }
    }

    private async Task<GoodsSpecCombinationDto[]> QuerySpecCombinationInformationAsync(
        DbContext db, PlaceOrderRequestDto dto, StoreUserDto storeUser)
    {
        var gradeId = storeUser.GradeId;
        var combinationIds = dto.Items.Select(x => x.GoodsSpecCombinationId).Distinct().ToArray();

        var datalist = await db.Set<GoodsSpecCombination>().AsNoTracking().WhereIdIn(combinationIds).ToArrayAsync();

        var combinations = this.ObjectMapper.MapArray<GoodsSpecCombination, GoodsSpecCombinationDto>(datalist);

        await this._specCombinationService.AttachDataAsync(combinations, new GoodsCombinationAttachDataInput()
        {
            Goods = true,
        });

        if (!string.IsNullOrWhiteSpace(gradeId))
        {
            await this._gradeGoodsPriceService.AttachGradePriceAsync(combinations, gradeId);
        }

        if (!string.IsNullOrWhiteSpace(dto.StoreId))
        {
            await this._storeGoodsPriceService.AttachStorePriceAsync(combinations, dto.StoreId);
        }

        return combinations;
    }

    private async Task<decimal> SetOrderItemFinalUnitPriceAsync(OrderItem orderItem,
        GoodsSpecCombinationDto combination,
        PlaceOrderRequestDto dto)
    {
        await Task.CompletedTask;

        var storePrice = combination.StoreGoodsMapping.FirstOrDefault(x => x.StoreId == dto.StoreId);
        if (storePrice != null && storePrice.Price != null)
        {
            orderItem.UnitPrice = storePrice.Price.Value;
        }
        else if (combination.SelectedGradePrice != null)
        {
            orderItem.UnitPrice = combination.SelectedGradePrice.Price;
        }
        else
        {
            orderItem.UnitPrice = combination.Price;
        }

        return orderItem.UnitPrice;
    }

    private async Task<OrderItem[]> BuildOrderItemsAsync(
        PlaceOrderRequestDto dto,
        Order order,
        GoodsSpecCombinationDto[] combinationList)
    {
        await Task.CompletedTask;

        var orderItems = new List<OrderItem>();

        foreach (var sc in dto.Items)
        {
            var combination = combinationList.FirstOrDefault(x => x.Id == sc.GoodsSpecCombinationId);
            if (combination == null)
                throw new UserFriendlyException("goods not available");

            var goods = combination.Goods;
            if (goods == null)
                throw new UserFriendlyException("goods not available");

            if (!combination.IsActive)
                throw new UserFriendlyException("商品当前不可销售");

            if (goods.MaxAmountInOnePurchase > 0 && sc.Quantity > goods.MaxAmountInOnePurchase)
                throw new UserFriendlyException(
                    $"{goods.Name}-{combination.Name} you can only purchase {goods.MaxAmountInOnePurchase} in one time");

            var orderItem = new OrderItem()
            {
                Id = default,
                OrderId = order.Id,
                GoodsId = combination.GoodsId,
                GoodsName = goods.Name,
                GoodsSpecCombinationId = combination.Id,
                Quantity = sc.Quantity,
                ItemWeight = default,
                GradePriceOffset = decimal.Zero,
                Price = decimal.Zero,
                UnitPrice = decimal.Zero,
            };

            if (orderItem.Quantity > combination.StockQuantity)
                throw new UserFriendlyException($"库存不足");

            var finalUnitPrice = await this.SetOrderItemFinalUnitPriceAsync(orderItem, combination, dto);

            if (finalUnitPrice < 0)
                throw new UserFriendlyException("结算单价不能小于0");

            orderItem.GradePriceOffset = finalUnitPrice - orderItem.UnitPrice;
            orderItem.Price = finalUnitPrice * orderItem.Quantity;

            orderItems.Add(orderItem);
        }

        return orderItems.ToArray();
    }

    private async Task TryApplyPromotionAsync(Order order, OrderItem[] items)
    {
        if (string.IsNullOrWhiteSpace(order.PromotionId))
            return;

        var promotion = await this._promotionService.QueryByIdAsync(order.PromotionId);
        if (promotion == null)
            throw new UserFriendlyException("promotion not available");

        if (promotion.IsExclusive)
        {
            if (order.CouponId != null && order.CouponId.Value > 0)
                throw new UserFriendlyException("promotion can't be joined while you are using a coupon");
        }

        var input = this._orderConditionUtils.BuildPromotionCheckInput(order, items);
        var conditions = this._orderConditionUtils.DeserializeConditions(promotion.Condition, throwIfException: true);
        var checkResponse = this._orderConditionUtils.ValidateOrderConditionsAsync(input, conditions);

        await foreach (var res in checkResponse)
        {
            if (!res.IsMatch)
                throw new UserFriendlyException(res.Message);
        }

        var results = this._promotionUtils.DeserializeResults(promotion.Result, throwIfException: true);
        await this._promotionUtils.ApplyPromotionResultAsync(order, results);
    }

    private async Task TryUseCouponAsync(Order order, OrderItem[] items)
    {
        if (order.CouponId == null || order.CouponId.Value <= 0)
            return;

        //before check
        if (!decimal.Equals(order.CouponDiscount, default))
            throw new SalesException("coupon discount already exist");

        //user coupon
        var userCoupon = await this._couponService.GetUserCouponByIdAsync(order.CouponId.Value);
        if (userCoupon == null)
            throw new UserFriendlyException("coupon not exist");

        //check coupon condition
        if (userCoupon.MinimumConsumption > order.OrderTotal)
            throw new UserFriendlyException("coupon can't be use for this order");

        var coupon = await this._couponService.QueryByIdAsync(userCoupon.CouponId);
        if (coupon == null)
            throw new BusinessException(message: "coupon not valid");

        //set coupon discount
        await this._couponUtils.ApplyToOrderAsync(order, userCoupon);

        //mark coupon as used
        await this._couponService.UseUserCouponAsync(userCoupon.Id);
    }

    private async Task FinalCheckOrderInformationAsync(Order order, OrderItem[] items)
    {
        //todo
        if (order.OrderTotal < decimal.Zero)
            throw new UserFriendlyException("订单金额不能小于0");

        if (!items.Any())
            throw new AbpException("order item is required");

        if (items.Any(x => x.Price < decimal.Zero))
            throw new UserFriendlyException("order item price should be greater than zero");

        await Task.CompletedTask;
    }

    private async Task<Order> BuildEmptyOrderEntityAsync(PlaceOrderRequestDto dto, StoreUserDto storeUser)
    {
        var order = new Order()
        {
            Id = this.GuidGenerator.CreateGuidString(),
            OrderSn = default,

            //associated
            StoreId = dto.StoreId,
            UserId = storeUser.Id,
            GradeId = storeUser.GradeId,
            OrderIp = _webHelper.GetCurrentIpAddress(),

            //fee
            OrderSubtotal = decimal.Zero,
            OrderShippingFee = decimal.Zero,
            OrderTotal = decimal.Zero,
            ExchangePointsAmount = decimal.Zero,

            //payment
            PaidTime = null,
            RefundedAmount = decimal.Zero,

            //coupon
            CouponId = dto.CouponId,
            CouponDiscount = decimal.Zero,

            //promotion
            PromotionId = dto.PromotionId,
            PromotionDiscount = decimal.Zero,

            //sipping
            ShippingRequired = true,
            ShippingAddressId = dto.AddressId,
            ShippingAddressProvice = dto.AddressProvince,
            ShippingAddressCity = dto.AddressCity,
            ShippingAddressArea = dto.AddressArea,
            ShippingAddressDetail = dto.AddressDetail,
            ShippingAddressContactName = dto.AddressContact,
            ShippingAddressContact = dto.AddressPhone,
            ShippingTime = null,

            //other
            Remark = dto.Remark,
            CreationTime = this.Clock.Now,
            LastModificationTime = default
        };

        //default status
        order.SetOrderStatus(OrderStatus.None);
        order.SetPaymentStatus(PaymentStatus.Pending);
        order.SetShippingStatus(ShippingStatus.NotYetShipped);

        //mark as order placed
        await this._orderUtils.GetOrderStateMachine(order).FireAsync(OrderProcessingAction.PlaceOrder);

        //try fill shipping address id
        var shippingAddress = await this.GetUserAddressOrNullAsync(dto, storeUser);
        if (shippingAddress != null)
        {
            order.ShippingAddressId = shippingAddress.Id;
            order.ShippingAddressProvice = shippingAddress.Province;
            order.ShippingAddressCity = shippingAddress.City;
            order.ShippingAddressArea = shippingAddress.Area;
            order.ShippingAddressDetail = shippingAddress.AddressDetail;
            order.ShippingAddressContact = shippingAddress.Tel;
            order.ShippingAddressContactName = shippingAddress.Name;
        }

        return order;
    }

    private async Task AfterOrderCreatedAsync(PlaceOrderRequestDto processRequest, Order order)
    {
        await this.EventBusService.NotifyInsertOrderNote(new OrderNote()
        {
            Note = "order created",
            OrderId = order.Id,
            DisplayToUser = false,
            CreationTime = this.Clock.Now
        });

        foreach (var m in processRequest.Items)
        {
            //扣减库存
            await this._goodsStockService.AdjustCombinationStockAsync(m.GoodsSpecCombinationId, -m.Quantity);
        }

        await this.EventBusService.NotifyRemoveCartsBySpecs(new RemoveCartBySpecs()
        {
            UserId = order.UserId,
            GoodsSpecCombinationId = processRequest.Items.Select(x => x.GoodsSpecCombinationId).ToArray()
        });
    }

    private async Task CheckGoodsStockStatusAsync(PlaceOrderRequestDto dto)
    {
        var placeOrderCheckInputs = this.BuildPlaceOrderCheckInputs(dto).ToArray();
        var checkResults = await this._orderService.CheckGoodsStockStatusAsync(placeOrderCheckInputs);

        foreach (var m in checkResults)
        {
            if (m.HasErrors())
                throw new UserFriendlyException(m.FirstErrorOrDefault());
        }
    }

    private async Task<string> GetUserGradeIdOrNullAsync(PlaceOrderRequestDto dto)
    {
        var userGrade = await this._userGradeService.GetGradeByUserIdAsync(dto.UserId);
        return userGrade?.Id;
    }

    public async Task<KeyValuePair<Order, OrderItem[]>> BuildOrderEntitiesAsync(PlaceOrderRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        //check form data
        if (!this.CheckFormOfPlaceOrderRequest(dto, out var formErrorMessage))
            throw new UserFriendlyException(formErrorMessage);

        //check goods info
        if (!this.CheckGoodsInfoOfPlaceOrderRequest(dto, out var goodsErrorMessage))
            throw new UserFriendlyException(goodsErrorMessage);

        //check goods info and stock quantity
        await this.CheckGoodsStockStatusAsync(dto);

        //prepare and check customer info
        var storeUser = await this.GetRequiredCustomerInfoAsync(dto);

        var db = await this._orderRepository.GetDbContextAsync();

        //load relative goods information into memory to optimise further query
        var allGoods = await this.QuerySpecCombinationInformationAsync(db, dto, storeUser);

        //build order entity
        var order = await this.BuildEmptyOrderEntityAsync(dto, storeUser);

        //build order items
        var orderItems = await this.BuildOrderItemsAsync(dto, order, allGoods);

        //coupon
        await this.TryUseCouponAsync(order, orderItems);

        //promotion
        await this.TryApplyPromotionAsync(order, orderItems);

        //calculate total
        await this._orderUtils.CalculateOrderFinalPriceAsync(order, orderItems);

        //generate sn
        order.OrderSn = await this._orderUtils.GenerateOrderSnAsync();

        //final check
        await this.FinalCheckOrderInformationAsync(order, orderItems);

        return new KeyValuePair<Order, OrderItem[]>(order, orderItems);
    }

    private async Task<StoreUserDto> GetRequiredCustomerInfoAsync(PlaceOrderRequestDto dto)
    {
        var user = await _userService.GetUserByIdAsync(dto.UserId);
        if (user == null || user.IsDeleted || !user.Active)
            throw new UserFriendlyException("user is not allow to place order");

        var storeUserDto = this.ObjectMapper.Map<User, StoreUserDto>(user);

        //get grade id
        storeUserDto.GradeId = await this.GetUserGradeIdOrNullAsync(dto);

        return storeUserDto;
    }

    private async Task<PlaceOrderResult> PlaceOrderImplAsync(PlaceOrderRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var result = new PlaceOrderResult();

        try
        {
            var db = await this._orderRepository.GetDbContextAsync();

            var orderEntities = await this.BuildOrderEntitiesAsync(dto);

            var order = orderEntities.Key;
            var orderItems = orderEntities.Value;

            //save to database
            db.Set<Order>().Add(order);
            db.Set<OrderItem>().AddRange(orderItems);

            //flush to db
            await db.SaveChangesAsync();

            //trigger changes
            await this.AfterOrderCreatedAsync(dto, order);

            result.SetData(order);
            return result;
        }
        catch (UserFriendlyException e)
        {
            return result.SetError(e.Message);
        }
    }
}