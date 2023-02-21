using Microsoft.AspNetCore.Mvc;
using XCloud.Platform.Shared.Localization;
using XCloud.Sales.Localization;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/test")]
public class TestController : ShopBaseController
{
    public TestController()
    {
        this.LocalizationResource = typeof(SalesResource);
    }

    [HttpGet("lang-test")]
    public string LangTest()
    {
        return $"lang:{L["ManageFeatures"]}-{L["manage-admin"]}";
    }
}