using Microsoft.AspNetCore.Mvc;
using XCloud.Sales.Localization;

namespace XCloud.Sales.Mall.Api.Controller;

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