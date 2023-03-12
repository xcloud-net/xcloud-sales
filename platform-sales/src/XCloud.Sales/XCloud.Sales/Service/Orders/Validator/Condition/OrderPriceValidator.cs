using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Core.Json;

namespace XCloud.Sales.Service.Orders.Validator.Condition;

public class OrderPriceConditionParameter : IEntityDto
{
    public decimal? LimitedOrderAmount { get; set; }
}

[ExposeServices(typeof(IOrderConditionValidator))]
public class OrderPriceValidator : IOrderConditionValidator, ITransientDependency
{
    private readonly IJsonDataSerializer _jsonDataSerializer;

    public OrderPriceValidator(IJsonDataSerializer jsonDataSerializer)
    {
        this._jsonDataSerializer = jsonDataSerializer;
    }

    public int Order => default;

    public string ConditionCode => OrderConditionValidatorTypes.OrderPrice;

    public string ResolveDescriptor(OrderCondition promotionCondition)
    {
        if (string.IsNullOrWhiteSpace(promotionCondition.ConditionJson))
            return "rule not exist";

        var param =
            this._jsonDataSerializer.DeserializeFromString<OrderPriceConditionParameter>(
                promotionCondition.ConditionJson);

        return $"订单金额不少于{param.LimitedOrderAmount ?? 0}";
    }

    public async Task<OrderConditionValidationResponse> IsConditionMatchedAsync(
        OrderConditionCheckInput promotionCheckInput,
        OrderCondition condition)
    {
        var response = new OrderConditionValidationResponse(this.ConditionCode);

        if (string.IsNullOrWhiteSpace(condition.ConditionJson))
        {
            response.SetError("promotion config error");
            return response;
        }

        var param =
            this._jsonDataSerializer.DeserializeFromString<OrderPriceConditionParameter>(condition.ConditionJson);

        if (param.LimitedOrderAmount != null &&
            promotionCheckInput.Order.OrderTotal < param.LimitedOrderAmount.Value)
        {
            response.SetError("订单价格不满足促销条件");
            return response;
        }

        await Task.CompletedTask;

        return response;
    }

    public Task<bool> IsConditionTypeSupportedAsync(OrderCondition condition)
    {
        return Task.FromResult(condition.ConditionType == this.ConditionCode);
    }
}