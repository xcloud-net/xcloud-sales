using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Users;

/// <summary>
/// Represents a reward point history entry
/// </summary>
public class PointsHistory : SalesBaseEntity, IHasCreationTime
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public int UserId { get; set; }

    public string OrderId { get; set; }

    /// <summary>
    /// Gets or sets the points redeemed/added
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// Gets or sets the points balance
    /// </summary>
    public int PointsBalance { get; set; }

    public int ActionType { get; set; }

    [Obsolete]
    public decimal UsedAmount { get; set; }

    /// <summary>
    /// Gets or sets the message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreationTime { get; set; }

}