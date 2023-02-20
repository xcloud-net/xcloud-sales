using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Member.Application.Service.User;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Services.Catalog;

namespace XCloud.Sales.Services.Users;

public class FavoritesDto : Favorites, IEntityDto
{
    public GoodsDto Goods { get; set; }
}

public class QueryFavoritesInput : PagedRequest
{
    public int userId { get; set; }
}

public class QueryPrepaidCardPagingInput : PagedRequest
{
    public int? UserId { get; set; }
    public bool? Used { get; set; }
}

public class UpdatePrepaidCardStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}

public class PrepaidCardDto : PrepaidCard, IEntityDto
{
    public bool? Expired { get; set; }
}

public class UsePrepaidCardInput : IEntityDto
{
    public string CardId { get; set; }
    public int UserId { get; set; }
}

public class QueryBalanceInput : PagedRequest
{
    public int UserId { get; set; }
    public int? ActionType { get; set; }
}

public class QueryPointsInput : PagedRequest
{
    public int UserId { get; set; }
    public int? ActionType { get; set; }
    public string OrderId { get; set; }
}

public class PointsHistoryDto : PointsHistory, IEntityDto
{
    //
}

public class BalanceHistoryDto : BalanceHistory, IEntityDto
{
    //
}

public class UserBalanceAndPoints : IEntityDto<int>
{
    public UserBalanceAndPoints() { }

    public UserBalanceAndPoints(User user) : this()
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        this.Id = user.Id;
        this.Balance = user.Balance;
        this.Points = user.Points;
    }

    public int Id { get; set; }
    public decimal Balance { get; set; }
    public int Points { get; set; }
}

public class StoreUserDto : User, IEntityDto
{
    public string GradeId { get; set; }

    public string GradeName { get; set; }

    public string GradeDescription { get; set; }

    public UserGrade Grade { get; set; }

    public SysUserDto SysUser { get; set; }
}

public class UserGradeMappingDto : UserGradeMapping, IEntityDto
{
    //
}

public class UpdateUserGradeStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? IsDeleted { get; set; }
}
    
public class UpdateUserStatusInput : IEntityDto<int>
{
    public int Id { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsActive { get; set; }
}

public class QueryUserInput : PagedRequest, IEntityDto
{
    public string Keywords { get; set; }
    public string[] GlobalUserIds { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool? Active { get; set; }
    public string GradeId { get; set; }
    public bool OnlyWithShoppingCart { get; set; }
    public bool? IsDeleted { get; set; }
    public string AccountMobile { get; set; }
}