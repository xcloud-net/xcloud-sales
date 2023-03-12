using Volo.Abp.Application.Dtos;

namespace XCloud.Sales.Core.Settings;

public class SalesServiceAddressOption : IEntityDto
{
    public string FrontEnd { get; set; }
}