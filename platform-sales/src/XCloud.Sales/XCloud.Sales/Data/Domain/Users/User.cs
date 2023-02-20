using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Users;

public class User : SalesBaseEntity, ISoftDelete, IHasDeletionTime, IHasModificationTime
{
    public User()
    {
        //
    }

    public string GlobalUserId { get; set; }

    public string NickName { get; set; }

    public string Avatar { get; set; }

    public string AccountMobile { get; set; }

    public decimal Balance { get; set; }

    public int Points { get; set; }

    public int HistoryPoints { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is active
    /// </summary>
    public bool Active { get; set; }


    public DateTime? DeletionTime { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime LastActivityTime { get; set; }

    public DateTime? LastModificationTime { get; set; }
}