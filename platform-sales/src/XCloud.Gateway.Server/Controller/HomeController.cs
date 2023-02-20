using Microsoft.AspNetCore.Mvc;
using XCloud.AspNetMvc.Controller;

namespace XCloud.Gateway.Server.Controller;

public class HomeController : XCloudBaseController
{
    public IActionResult Index() => this.Redirect("/swagger");
}