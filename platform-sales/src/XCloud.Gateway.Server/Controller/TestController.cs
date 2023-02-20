using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using XCloud.AspNetMvc.Controller;
using XCloud.AspNetMvc.ModelBinder.CancellationTokenBinder;
using XCloud.AspNetMvc.ModelBinder.JsonModel;
using XCloud.Core.Http.Dynamic;

namespace XCloud.Gateway.Server.Controller;

public class Post : IEntityDto<string>
{
    public string Id { get; set; }
    public string Title { get; set; }
    public int ReadCount { get; set; }
    public DateTime CreateTimeUtc { get; set; }
}

[DynamicHttpClient("baidu", "api/test")]
public interface IPostService : IApplicationService
{
    [HttpGet("query/{id}/name")]
    Task<string> Query([FromQuery] string name, [FromRoute] string id);

    [HttpPost("put")]
    Task<string> Put([FromBody] [JsonData] Post p);

    [HttpPost("delete")]
    Task<string> Delete([FromForm] string id);

    [HttpPost("void-call")]
    Task Vcall([JsonData] Post p, [InjectCancellationToken] CancellationToken token);

    [HttpPost("post-data")]
    Task<Post> Postdata([JsonData] Post p);
}

[Route("api/gateway/test")]
public class TestController : XCloudBaseController
{
    [HttpPost("body-post")]
    public async Task<Post> post_post([FromServices] ILogger<TestController> logger,
        [FromServices] IPostService postService,
        [InjectCancellationToken] CancellationToken token,
        [FromBody] [JsonData] Post data)
    {
        string response = null;
        response = await postService.Query("xx", "dd");
        logger.LogInformation(response);
        response = await postService.Put(new Post() { });
        logger.LogInformation(response);
        response = await postService.Delete("ddss");
        logger.LogInformation(response);

        await postService.Vcall(new Post() { }, token);
        await postService.Postdata(new Post() { });

        logger.LogInformation(DateTime.Now.ToString());

        return data;
    }
}