using Volo.Abp.Application.Dtos;
using XCloud.Sales.Data.Domain.Configuration;

namespace XCloud.Sales.Service.Configuration;

public class HomeBlocksDto : IEntityDto
{
    public bool Published { get; set; }
    public string Blocks { get; set; }
}

public class MallSettingsDto : IEntityDto
{
    public string HomePageNotice { get; set; }
    public string HomeSliderImages { get; set; }

    public string PlaceOrderNotice { get; set; }
    public string LoginNotice { get; set; }
    public string RegisterNotice { get; set; }
    public string GoodsDetailNotice { get; set; }
    public string HomePageCategorySeoNames { get; set; } = string.Empty;

    public bool PlaceOrderDisabled { get; set; } = false;
    public bool AftersaleDisabled { get; set; } = false;

    public bool DisplayPriceForGuest { get; set; } = false;

    public bool HidePriceForGuest => !this.DisplayPriceForGuest;
}

public class SettingDto : Setting, IEntityDto
{
    //
}