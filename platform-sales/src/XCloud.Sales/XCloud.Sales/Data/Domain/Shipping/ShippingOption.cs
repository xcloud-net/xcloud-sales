namespace XCloud.Sales.Data.Domain.Shipping;

public class ShippingOption
{
    public string ShippingRateMethodSystemName { get; set; }

    public decimal Rate { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}