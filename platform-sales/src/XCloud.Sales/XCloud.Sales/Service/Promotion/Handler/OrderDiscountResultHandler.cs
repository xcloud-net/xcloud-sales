using Volo.Abp.Application.Dtos;
using XCloud.Core.Json;
using XCloud.Sales.Core;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Service.Promotion.Handler;

public class OrderDiscountResultHandlerParameter : IEntityDto
{
    public OrderDiscountResultHandlerParameter()
    {
        //
    }

    //public double? DiscountPercent { get; set; }
    public decimal? DiscountAmount { get; set; }
}

[ExposeServices(typeof(IPromotionResultHandler))]
public class OrderDiscountResultHandler : IPromotionResultHandler, ITransientDependency
{
    private readonly IJsonDataSerializer _jsonDataSerializer;

    public OrderDiscountResultHandler(IJsonDataSerializer jsonDataSerializer)
    {
        this._jsonDataSerializer = jsonDataSerializer;
    }

    public int Order => int.MaxValue;

    public string ResultHandlerCode => PromotionResultTypes.OrderDiscount;

    public Task<bool> IsResultTypeSupportedAsync(PromotionResult promotionResult)
    {
        return Task.FromResult(promotionResult.ResultType == this.ResultHandlerCode);
    }

    public string ResolveDescriptor(PromotionResult promotionResult)
    {
        if (string.IsNullOrWhiteSpace(promotionResult.ResultJson))
            return "rule not exist";

        var param =
            this._jsonDataSerializer.DeserializeFromString<OrderDiscountResultHandlerParameter>(
                promotionResult.ResultJson);

        return $"减免{param.DiscountAmount ?? 0}元";
    }

    public async Task<Order> ApplyToOrderAsync(Order order, PromotionResult promotionResult)
    {
        if (!decimal.Equals(order.PromotionDiscount, default))
            throw new SalesException("promotion discount already exist");

        if (string.IsNullOrWhiteSpace(promotionResult.ResultJson))
            throw new UserFriendlyException("promotion result config error");

        var param =
            this._jsonDataSerializer.DeserializeFromString<OrderDiscountResultHandlerParameter>(
                promotionResult.ResultJson);

        if (param.DiscountAmount != null)
        {
            //set promotion result
            order.PromotionDiscount = Math.Abs(param.DiscountAmount.Value);
        }

        await Task.CompletedTask;

        return order;
    }
}