using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Platform.Common.Application.Service.Address;
using XCloud.Platform.Shared.Dto;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Clients.Platform;
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
    private readonly PlatformInternalService _internalCommonClientService;
    private readonly IOrderService _orderService;
    private readonly IWebHelper _webHelper;
    private readonly IUserService _userService;
    private readonly IAddressService _addressService;
    private readonly ISalesRepository<Order> _orderRepository;
    private readonly IMallSettingService _mallSettingService;
    private readonly ISpecCombinationPriceService _specCombinationPriceService;
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
        ISpecCombinationPriceService specCombinationPriceService,
        IMallSettingService mallSettingService,
        OrderUtils orderUtils,
        PlatformInternalService internalCommonClientService,
        IOrderService orderService,
        IWebHelper webHelper,
        IUserService userService,
        IAddressService addressService,
        ICouponService couponService,
        ISalesRepository<Order> orderRepository, IGradeGoodsPriceService gradeGoodsPriceService,
        IStoreGoodsPriceService storeGoodsPriceService)
    {
        this._orderConditionUtils = orderConditionUtils;
        this._promotionService = promotionService;
        this._promotionUtils = promotionUtils;
        this._goodsStockService = goodsStockService;
        this._userGradeService = userGradeService;
        this._specCombinationPriceService = specCombinationPriceService;
        this._orderUtils = orderUtils;
        this._mallSettingService = mallSettingService;
        this._internalCommonClientService = internalCommonClientService;
        this._orderRepository = orderRepository;
        _gradeGoodsPriceService = gradeGoodsPriceService;
        _storeGoodsPriceService = storeGoodsPriceService;
        this._orderService = orderService;
        this._webHelper = webHelper;
        this._userService = userService;
        this._addressService = addressService;
        this._couponService = couponService;
    }


    [UnitOfWork(isTransactional: true)]
    public virtual async Task<PlaceOrderResult> PlaceOrderAsync(
        PlaceOrderRequestDto processRequest)
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
            throw new FailToGetRedLockException("稍后再试");
        }
    }

    private bool CheckFormOfPlaceOrderRequest(PlaceOrderRequestDto dto, out string errorMessage)
    {
        errorMessage = string.Empty;
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
            errorMessage = "dumplicate input";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    private async Task<UserAddressDto> GetUserAddressOrNullAsync(PlaceOrderRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.AddressId))
            return null;

        var user = dto.UserHolder;
        var allAddress = await this._addressService.QueryByGlobalUserId(user.GlobalUserId);
        var shippingAddress = allAddress.FirstOrDefault(x => x.Id == dto.AddressId);
        return shippingAddress;
    }

    private string DateString() => this.Clock.Now.ToString("yyyyMMdd");

    private string MonthString() => this.Clock.Now.ToString("yyyyMM");

    private async Task<string> GenerateOrderSnAsync()
    {
        var month = this.MonthString();

        var noResponse =
            await this._internalCommonClientService.GenerateSerialNoAsync(
                new CreateNoByCategoryDto($"order-sn-{month}"));
        if (!noResponse.IsSuccess())
            throw new AbpException("failed to generate order sn");

        if (noResponse.Data > 1_0000_0000)
            this.Logger.LogWarning("order code is too large,in this month");

        var code = noResponse.Data.ToString().PadLeft(10, '0');

        return $"{month}-{code}";
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

    private async Task<KeyValueDto<GoodsSpecCombinationDto, Goods>[]> QueryGoodsAndSpecCombinationsAsync(
        DbContext db, PlaceOrderRequestDto dto)
    {
        var gradeId = dto.UserHolder.GradeId;
        var combinationIds = dto.Items.Select(x => x.GoodsSpecCombinationId).Distinct().ToArray();

        var query = from combination in db.Set<GoodsSpecCombination>().AsNoTracking()
            join goods in db.Set<Goods>().AsNoTracking()
                on combination.GoodsId equals goods.Id
            select new { combination, goods };

        query = query.Where(x => combinationIds.Contains(x.combination.Id));

        var data = await query.ToArrayAsync();

        KeyValueDto<GoodsSpecCombinationDto, Goods> BuildResponse(GoodsSpecCombination combination, Goods goods)
        {
            var combinationDto = this.ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(combination);
            return new KeyValueDto<GoodsSpecCombinationDto, Goods>(combinationDto, goods);
        }

        var arr = data.Select(x => BuildResponse(x.combination, x.goods)).ToArray();

        if (!string.IsNullOrWhiteSpace(gradeId))
        {
            var combinations = arr.Select(x => x.Key).ToArray();
            await this._gradeGoodsPriceService.AttachGradePriceAsync(combinations, gradeId);
        }

        return arr;
    }

    private async Task<OrderItem[]> BuildOrderItemsAsync(
        PlaceOrderRequestDto dto,
        Order order,
        KeyValueDto<GoodsSpecCombinationDto, Goods>[] allGoods)
    {
        await Task.CompletedTask;

        var orderItems = new List<OrderItem>();

        foreach (var sc in dto.Items)
        {
            var goodsInformation = allGoods.FirstOrDefault(x => x.Key.Id == sc.GoodsSpecCombinationId);
            if (goodsInformation == null)
                throw new BusinessException("goods not exist");

            var combination = goodsInformation.Key;
            var goods = goodsInformation.Value;

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
                UnitPrice = combination.Price,
                Quantity = sc.Quantity,
                ItemWeight = default,
                GradePriceOffset = decimal.Zero,
                Price = decimal.Zero,
            };
            if (orderItem.Quantity > combination.StockQuantity)
                throw new UserFriendlyException($"库存不足");

            var finalUnitPrice = combination.GradePrice ?? orderItem.UnitPrice;
            if (finalUnitPrice < 0)
                throw new UserFriendlyException("结算单价不能小于0");

            orderItem.GradePriceOffset = finalUnitPrice - orderItem.UnitPrice;
            orderItem.Price = finalUnitPrice * orderItem.Quantity;

            orderItems.Add(orderItem);
        }

        return orderItems.ToArray();
    }

    private async Task<Order> TryApplyPromotionAsync(Order order, OrderItem[] items)
    {
        if (string.IsNullOrWhiteSpace(order.PromotionId))
            return order;

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

        return order;
    }

    private async Task<Order> TryUseCouponAsync(Order order, OrderItem[] items)
    {
        if (order.CouponId == null || order.CouponId.Value <= 0)
            return order;

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

        //condition validator
        var input = this._orderConditionUtils.BuildPromotionCheckInput(order, items);
        var conditions = this._orderConditionUtils.DeserializeConditions("[]", throwIfException: true);
        var checkResponse = this._orderConditionUtils.ValidateOrderConditionsAsync(input, conditions);

        await foreach (var res in checkResponse)
        {
            if (!res.IsMatch)
                throw new UserFriendlyException(res.Message);
        }

        //set coupon discount
        order.CouponDiscount = userCoupon.Value;

        //mark coupon as used
        await this._couponService.UseUserCouponAsync(userCoupon.Id);

        return order;
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

    private async Task<Order> BuildEmptyOrderEntityAsync(PlaceOrderRequestDto dto)
    {
        var order = new Order()
        {
            Id = this.GuidGenerator.CreateGuidString(),
            OrderSn = default,

            //associated
            StoreId = dto.StoreId,
            UserId = dto.UserHolder.Id,
            GradeId = dto.UserHolder.GradeId,
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
        var shippingAddress = await this.GetUserAddressOrNullAsync(dto);
        if (shippingAddress != null)
            order.ShippingAddressId = shippingAddress.Id;

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
        // todo build order entities
        var db = await this._orderRepository.GetDbContextAsync();
        throw new NotImplementedException();
    }

    private async Task<Order> CalculateOrderFinalPriceAsync(Order order, OrderItem[] orderItems)
    {
        //todo put into order utils
        order.GradePriceOffsetTotal = orderItems.Sum(x => x.GradePriceOffset);
        order.OrderSubtotal = orderItems.Sum(x => x.Price);
        //total
        order.OrderTotal = order.OrderSubtotal + order.OrderShippingFee;
        order.OrderTotal -= order.CouponDiscount;
        order.OrderTotal -= order.PromotionDiscount;
        order.ExchangePointsAmount = order.OrderTotal;

        await Task.CompletedTask;

        return order;
    }

    private async Task<CombinationFinalPriceDto[]> CalculateFinalPriceAsync(DbContext db, PlaceOrderRequestDto dto)
    {
        //get store prices
        //get grade price
        //get combination price

        //return price or fallback

        var combinationIds = dto.Items.Select(x => x.GoodsSpecCombinationId).Distinct().ToArray();

        var items = await db.Set<GoodsSpecCombination>().WhereIdIn(combinationIds).ToArrayAsync();
        var combinations = this.ObjectMapper.MapArray<GoodsSpecCombination, GoodsSpecCombinationDto>(items);

        var gradeId = dto.UserHolder.GradeId;
        if (!string.IsNullOrWhiteSpace(gradeId))
        {
            await this._gradeGoodsPriceService.AttachGradePriceAsync(combinations, gradeId);
        }

        var storePrices = string.IsNullOrWhiteSpace(dto.StoreId)
            ? Array.Empty<StoreGoodsMappingDto>()
            : await this._storeGoodsPriceService.QueryManyByAsync(combinationIds, dto.StoreId);

        var response = new List<CombinationFinalPriceDto>();

        foreach (var id in combinationIds)
        {
            var storePrice = storePrices.FirstOrDefault(x => x.GoodsCombinationId == id);
        }

        throw new NotImplementedException();
    }

    private async Task<PlaceOrderRequestDto> PrepareCustomerInfoAsync(PlaceOrderRequestDto dto)
    {
        var user = await _userService.GetUserByIdAsync(dto.UserId);
        if (user == null || user.IsDeleted || !user.Active)
            throw new UserFriendlyException("user is not allow to place order");
        dto.UserHolder = this.ObjectMapper.Map<User, StoreUserDto>(user);

        //get grade id
        dto.UserHolder.GradeId = await this.GetUserGradeIdOrNullAsync(dto);

        return dto;
    }

    private async Task<PlaceOrderResult> PlaceOrderImplAsync(PlaceOrderRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var result = new PlaceOrderResult();

        //check form data
        if (!this.CheckFormOfPlaceOrderRequest(dto, out var formErrorMessage))
            return result.SetError(formErrorMessage);

        //check goods info
        if (!this.CheckGoodsInfoOfPlaceOrderRequest(dto, out var goodsErrorMessage))
            return result.SetError(goodsErrorMessage);

        //check goods info and stock quantity
        await this.CheckGoodsStockStatusAsync(dto);

        //todo check coupon address ...

        //prepare and check customer info
        dto = await this.PrepareCustomerInfoAsync(dto);

        //start new uow
        using var uow = this.UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true);

        try
        {
            var db = await this._orderRepository.GetDbContextAsync();

            //load relative goods information into memory to optimise further query
            var allGoods = await this.QueryGoodsAndSpecCombinationsAsync(db, dto);

            //build order entity
            var order = await this.BuildEmptyOrderEntityAsync(dto);
            //build order items
            var orderItems = await this.BuildOrderItemsAsync(dto, order, allGoods);

            //coupon
            await this.TryUseCouponAsync(order, orderItems);

            //promotion
            await this.TryApplyPromotionAsync(order, orderItems);

            //calculate total
            await this.CalculateOrderFinalPriceAsync(order, orderItems);

            //generate sn
            order.OrderSn = await this.GenerateOrderSnAsync();

            //final check
            await this.FinalCheckOrderInformationAsync(order, orderItems);

            //save to database
            db.Set<Order>().Add(order);
            db.Set<OrderItem>().AddRange(orderItems);

            //flush to db
            await db.SaveChangesAsync();
            //commit transaction
            await uow.CompleteAsync();

            //trigger changes
            await this.AfterOrderCreatedAsync(dto, order);

            result.SetData(order);
            return result;
        }
        catch (UserFriendlyException e)
        {
            await uow.RollbackAsync();
            return result.SetError(e.Message);
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }
}