using Volo.Abp.Application.Dtos;
using XCloud.Platform.Common.Application.Service.Address;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Services.Shipping;

public class GetShippingOptionRequest: IEntityDto
{
    public GetShippingOptionRequest()
    {
        this.Items = new List<ShoppingCartItem>();
    }
    public virtual IList<ShoppingCartItem> Items { get; set; }

    public virtual UserAddressDto ShippingAddress { get; set; }

    public virtual string ZipPostalCodeFrom { get; set; }
}