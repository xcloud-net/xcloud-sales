using Volo.Abp.Application.Dtos;
using XCloud.Sales.Data.Domain.Shipping;

namespace XCloud.Sales.Services.Shipping;

public class GetShippingOptionResponse: IEntityDto
{
    public GetShippingOptionResponse()
    {
        this.Errors = new List<string>();
        this.ShippingOptions = new List<ShippingOption>();
    }

    public IList<ShippingOption> ShippingOptions { get; set; }

    public IList<string> Errors { get; set; }

    public bool Success { get; set; }

    public void AddError(string error)
    {
        this.Errors.Add(error);
    }
}