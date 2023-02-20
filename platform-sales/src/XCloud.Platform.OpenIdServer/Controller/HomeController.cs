using Microsoft.AspNetCore.Mvc;

namespace XCloud.Platform.OpenIdServer.Controller;

[Route("home")]
public class HomeController : ControllerBase
{
    [HttpPost("index")]
    public string index() => string.Empty;
}