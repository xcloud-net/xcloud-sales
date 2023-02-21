using XCloud.Sales.Core.Plugins;

namespace XCloud.Sales.Service.Shipping;

public interface IShippingRateMethod : IPlugin
{
    GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest);

    decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest);
}