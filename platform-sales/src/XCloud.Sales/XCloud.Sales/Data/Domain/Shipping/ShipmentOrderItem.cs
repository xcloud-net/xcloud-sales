using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Shipping;

public class ShipmentOrderItem : SalesBaseEntity<string>
{
    public string ShipmentId { get; set; }
    /// <summary>
    /// ��������
    /// </summary>
    public int OrderItemId { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public int Quantity { get; set; }
}