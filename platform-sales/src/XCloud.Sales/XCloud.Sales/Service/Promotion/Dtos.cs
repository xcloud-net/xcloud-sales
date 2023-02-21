using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Promotion;
using XCloud.Sales.Service.Orders.Validator;

namespace XCloud.Sales.Service.Promotion;

public static class PromotionResultTypes
{
    public static string OrderDiscount => "order-discount";
}

public class PromotionResult : IEntityDto
{
    public string ResultType { get; set; }

    public string ResultJson { get; set; }
}

public class AttachPromotionDataInput : IEntityDto
{
    public bool ParseCondition { get; set; } = false;
    public bool ParseResults { get; set; } = false;
}

public class StorePromotionDto : StorePromotion, IEntityDto
{
    public OrderCondition[] PromotionConditions { get; set; }

    public string[] PromotionConditionDescriptors { get; set; }

    public PromotionResult[] PromotionResults { get; set; }

    public string[] PromotionResultsDescriptors { get; set; }
}

public class UpdatePromotionStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsExclusive { get; set; }
}

public class QueryPromotionPagingInput : PagedRequest
{
    public DateTime? AvailableFor { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? SortForAdmin { get; set; }
}