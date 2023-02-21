using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Coupons;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Coupons;

public class AttachCouponDataInput : IEntityDto
{
    public int? IssuedToUserId { get; set; }
}

public class CouponDto : Coupon, IEntityDto
{
    public bool? CanBeIssued { get; set; }
}

public class CouponUserMappingDto : CouponUserMapping, IEntityDto
{
    public CouponDto Coupon { get; set; }
    public StoreUserDto User { get; set; }
}

public class IssueCouponInput : IEntityDto
{
    public int CouponId { get; set; }
    public int UserId { get; set; }
}

public class UpdateCouponStatusInput : IEntityDto<int>
{
    public int Id { get; set; }
    public bool? IsDeleted { get; set; }
}

public class AttachUserCouponDataInput : IEntityDto
{
    public bool User { get; set; } = false;
}

public class QueryUserCouponPagingInput : PagedRequest
{
    public int? UserId { get; set; }

    public bool? Used { get; set; }

    public DateTime? AvailableFor { get; set; }

    public bool? IsDeleted { get; set; }
}

public class QueryCouponPagingInput : PagedRequest
{
    public DateTime? AvailableFor { get; set; }

    public bool? IsDeleted { get; set; }
    public bool? SortForAdmin { get; set; }
}