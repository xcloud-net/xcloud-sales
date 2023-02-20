using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Users;

public class UserGradeMapping : SalesBaseEntity<string>, IHasCreationTime
{
    public int UserId { get; set; }
    public string GradeId { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public DateTime CreationTime { get; set; }
}