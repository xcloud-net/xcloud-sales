using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using XCloud.AspNetMvc.Extension;
using XCloud.Core.DependencyInjection.Extension;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Core.Helper;

namespace XCloud.AspNetMvc.Filters;

/// <summary>
/// 验证签名
/// </summary>
public class ValidateSignAttribute : ActionFilterAttribute
{
    /// <summary>
    /// 配置文件里的key
    /// </summary>
    public string ConfigKey { get; set; } = "sign_key";
    public string SignKey { get; set; } = "sign";

    /// <summary>
    /// 时间戳误差
    /// </summary>
    public int DeviationSeconds { get; set; } = 10;

    public override async Task OnActionExecutionAsync(ActionExecutingContext _context, ActionExecutionDelegate next)
    {
        var config = _context.HttpContext.RequestServices.ResolveConfiguration();

        var salt = config[ConfigKey];
        salt.Should().NotBeNullOrEmpty($"没有配置签名的约定key({ConfigKey})");

        var context = _context.HttpContext;

        var allparams = context.Request.Query.ToDict().AddDict(context.Request.Form.ToDict());

        #region 验证时间戳
        long.TryParse(allparams.GetValueOrDefault("timestamp") ?? "-1", out var clientTimestamp).Should().BeTrue();
        if (clientTimestamp < 0)
        {
            _context.Result = new JsonResult(new ApiResponse<object>() { }.SetError("缺少时间戳"));
            return;
        }
        var serverTimestamp = DateTimeHelper.GetTimeStamp(DateTime.UtcNow);
        //取绝对值
        if (Math.Abs(serverTimestamp - clientTimestamp) > Math.Abs(DeviationSeconds))
        {
            _context.Result = new JsonResult(new ApiResponse<object>() { }.SetData(new
            {
                client_timestamp = clientTimestamp, server_timestamp = serverTimestamp
            }).SetError("请求时间戳已经过期"));
            return;
        }
        #endregion

        #region 验证签名
        var clientSign = allparams.GetValueOrDefault(SignKey)?.ToUpper();
        if (string.IsNullOrWhiteSpace(clientSign))
        {
            _context.Result = new JsonResult(new ApiResponse<object>() { }.SetError("请求被拦截，获取不到签名"));
            return;
        }

        var reqparams = SignHelper.FilterAndSort(allparams, SignKey, new MyStringComparer());
        var (server_sign, sign_data) = SignHelper.CreateSign(reqparams, salt);

        if (clientSign != server_sign)
        {
            _context.Result = new JsonResult(new ApiResponse<object>() { }.SetData(new
            {
                server_sign,
                client_sign = clientSign,
                sign_data
            }).SetError("签名错误"));
            return;
        }
        #endregion

        await next.Invoke();
    }
}