using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;

namespace XCloud.Sales.Service.Orders.Validator;

public static class OrderConditionValidatorTypes
{
    public static string All => "*";
    
    public static string OrderPrice => "order-price";
}

/// <summary>
/// 促销条件表
/// </summary>
public class OrderCondition : IEntityDto
{
    public string ConditionType { get; set; }

    public string ConditionJson { get; set; }
}

public class OrderConditionCheckInput : IEntityDto
{
    public OrderConditionCheckInput()
    {
        //
    }

    public OrderConditionCheckInput(OrderDto order)
    {
        this.Order = order;
    }

    public OrderDto Order { get; set; }
}

public class OrderConditionValidationResponse : ApiResponse<object>
{
    public OrderConditionValidationResponse()
    {
        //
    }

    public OrderConditionValidationResponse(string conditionType)
    {
        this.ConditionType = conditionType;
    }

    public string ConditionType { get; set; }
    public bool IsMatch => this.Error == null;
    public string Message => this.Error?.Message;
}