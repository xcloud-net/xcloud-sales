using XCloud.Platform.Common.Application.Service.Address;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Shipping;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Common;

namespace XCloud.Sales.Services.Shipping;

public interface IShippingService : ISalesAppService
{
    IList<IShippingRateMethod> LoadActiveShippingRateComputationMethods();

    IShippingRateMethod LoadShippingRateComputationMethodBySystemName(string systemName);

    IList<IShippingRateMethod> LoadAllShippingRateComputationMethods();

    decimal GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem);

    decimal GetShoppingCartItemTotalWeight(ShoppingCartItem shoppingCartItem);

    GetShippingOptionRequest CreateShippingOptionRequest(IList<ShoppingCartItem> cart, UserAddressDto shippingAddress);

}

public class ShippingService : SalesAppService, IShippingService
{
    private readonly ISalesRepository<ShippingMethod> _shippingMethodRepository;
    private readonly ISpecCombinationParser _goodsSpecParser;
    private readonly ISpecService _goodsSpecService;
    private readonly ILocalizationService _localizationService;

    public ShippingService(
        ISpecService goodsSpecService,
        ISalesRepository<ShippingMethod> shippingMethodRepository,
        ISpecCombinationParser goodsSpecParser,
        ILocalizationService localizationService)
    {
        this._goodsSpecService = goodsSpecService;
        this._shippingMethodRepository = shippingMethodRepository;
        this._goodsSpecParser = goodsSpecParser;
        this._localizationService = localizationService;
    }

    public virtual IList<IShippingRateMethod> LoadActiveShippingRateComputationMethods()
    {
        return LoadAllShippingRateComputationMethods().ToList();
    }

    public virtual IShippingRateMethod LoadShippingRateComputationMethodBySystemName(string systemName)
    {
        return null;
    }

    public virtual IList<IShippingRateMethod> LoadAllShippingRateComputationMethods()
    {
        return new List<IShippingRateMethod>();
    }

    public virtual decimal GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem)
    {
        throw new NotImplementedException();
    }

    public virtual decimal GetShoppingCartItemTotalWeight(ShoppingCartItem shoppingCartItem)
    {
        if (shoppingCartItem == null)
            throw new ArgumentNullException("shoppingCartItem");

        decimal totalWeight = GetShoppingCartItemWeight(shoppingCartItem) * shoppingCartItem.Quantity;
        return totalWeight;
    }

    public virtual decimal GetShoppingCartTotalWeight(IList<ShoppingCartItem> cart)
    {
        decimal totalWeight = decimal.Zero;
        foreach (var shoppingCartItem in cart)
            totalWeight += GetShoppingCartItemTotalWeight(shoppingCartItem);

        return totalWeight;
    }

    public virtual GetShippingOptionRequest CreateShippingOptionRequest(IList<ShoppingCartItem> cart,
        UserAddressDto shippingAddress)
    {
        var request = new GetShippingOptionRequest();
        request.Items = new List<ShoppingCartItem>();
        foreach (var sc in cart)
            request.Items.Add(sc);
        request.ShippingAddress = shippingAddress;
        return request;

    }

    public virtual GetShippingOptionResponse GetShippingOptions(IList<ShoppingCartItem> cart,
        UserAddressDto shippingAddress, string allowedShippingRateComputationMethodSystemName = "")
    {
        if (cart == null)
            throw new ArgumentNullException("cart");

        var result = new GetShippingOptionResponse();

        var getShippingOptionRequest = CreateShippingOptionRequest(cart, shippingAddress);
        var shippingRateComputationMethods = LoadActiveShippingRateComputationMethods()
            .Where(srcm =>
                String.IsNullOrWhiteSpace(allowedShippingRateComputationMethodSystemName) ||
                allowedShippingRateComputationMethodSystemName.Equals(srcm.PluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
        if (shippingRateComputationMethods.Count == 0)
        {
            return result;
        }
        foreach (var srcm in shippingRateComputationMethods)
        {
            var getShippingOptionResponse = srcm.GetShippingOptions(getShippingOptionRequest);
            foreach (var so2 in getShippingOptionResponse.ShippingOptions)
            {
                so2.ShippingRateMethodSystemName = srcm.PluginDescriptor.SystemName;
                so2.Rate = Math.Round(so2.Rate, 2);
                result.ShippingOptions.Add(so2);
            }

            if (!getShippingOptionResponse.Success)
            {
                foreach (string error in getShippingOptionResponse.Errors)
                {
                    result.AddError(error);
                    this.Logger.LogWarning(message: string.Format("Shipping ({0}). {1}", srcm.PluginDescriptor.FriendlyName, error));
                }
            }
        }

        if (result.ShippingOptions.Count > 0 && result.Errors.Count > 0)
            result.Errors.Clear();

        if (result.ShippingOptions.Count == 0 && result.Errors.Count == 0)
            result.Errors.Add("Checkout.ShippingOptionCouldNotBeLoaded");

        result.Success = true;
        return result;
    }




}