using XCloud.Core.Dto;

namespace XCloud.Sales.Service.Orders.Validator.Condition;

[ExposeServices(typeof(IOrderConditionValidator))]
public class MatchAllValidator : IOrderConditionValidator, IScopedDependency
{
    public int Order => int.MaxValue;
    
    public string ConditionCode => OrderConditionValidatorTypes.All;

    public string ResolveDescriptor(OrderCondition promotionCondition)
    {
        return $"所有条件均满足";
    }

    public Task<bool> IsConditionTypeSupportedAsync(OrderCondition condition)
    {
        return Task.FromResult(condition.ConditionType == this.ConditionCode);
    }

    public Task<OrderConditionValidationResponse> IsConditionMatchedAsync(OrderConditionCheckInput promotionCheckInput,
        OrderCondition condition)
    {
        var response = new OrderConditionValidationResponse(this.ConditionCode);
        response.ResetError();
        return Task.FromResult(response);
    }
}