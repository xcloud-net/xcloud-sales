using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Users;

public class BalanceHistory : SalesBaseEntity<string>, IHasCreationTime
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the points redeemed/added
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Gets or sets the points balance
    /// </summary>
    public decimal LatestBalance { get; set; }

    public int ActionType { get; set; }

    /// <summary>
    /// Gets or sets the message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreationTime { get; set; }

}