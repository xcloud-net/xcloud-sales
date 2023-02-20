using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Shipping;

/// <summary>
/// ���ͷ���
/// </summary>
public class ShippingMethod : SalesBaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public int DisplayOrder { get; set; }
}