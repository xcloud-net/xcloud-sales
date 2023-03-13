namespace XCloud.Sales.Queue;

public static class SalesMessageTopics
{
    public const string InsertActivityLog = "insert-activity-log";

    public const string ClearAllActivityLogs = "clear-all-activity-logs";

    public const string RemoveCartsAfterPlaceOrder = "remove-carts-after-place-order";

    public const string InsertOrderNote = "insert-order-note";

    public const string OrderCreated = "order-created";

    public const string OrderShipped = "order-shipped";

    public const string OrderCanceled = "order-canceled";
        
    public const string RefreshGoodsInfo = "refresh-goods-info";

    public const string FormatCombinationSpecs = "format-combination-specs";

    public const string UpdateUserLastActivityTime = "update-user-last-activity-time";

    public const string RefreshUserInformation = "refresh-user-information";

    public const string SyncUserInfoFromPlatform = "sync-mall-user-info-from-platform";
}