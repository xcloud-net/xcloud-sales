using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using XCloud.Core.Extension;

namespace XCloud.AspNetMvc.Extension;

public static class MvcExtension
{
    public static string GetCurrentIpAddress(this HttpContext context)
    {
        var result = string.Empty;
            
            
        //The X-Forwarded-For (XFF) HTTP header field is a de facto standard
        //for identifying the originating IP address of a client
        //connecting to a web server through an HTTP proxy or load balancer.

        //it's used for identifying the originating IP address of a client connecting to a web server
        //through an HTTP proxy or load balancer. 
        var xff = (string)context.Request.Headers.Keys
            .Where(x => "X-FORWARDED-FOR".Equals(x, StringComparison.InvariantCultureIgnoreCase))
            .Select(k => context.Request.Headers[k])
            .FirstOrDefault();

        //if you want to exclude private IP addresses, then see http://stackoverflow.com/questions/2577496/how-can-i-get-the-clients-ip-address-in-asp-net-mvc
        if (!string.IsNullOrWhiteSpace(xff))
        {
            var lastIp = xff.Split(new[] { ',' }).FirstOrDefault();
            result = lastIp;
        }

        //some validation
        if (result == "::1")
        {
            result = "127.0.0.1";
        }
        //remove port
        if (!string.IsNullOrWhiteSpace(result))
        {
            int index = result.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
            if (index > 0)
            {
                result = result.Substring(0, index);
            }
        }
            
        //if this header not exists try get connection remote IP address
        if (string.IsNullOrEmpty(result) && context.Connection.RemoteIpAddress != null)
            result = context.Connection.RemoteIpAddress.ToString();

        //some of the validation
        if (result != null && result.Equals(IPAddress.IPv6Loopback.ToString(),
                StringComparison.InvariantCultureIgnoreCase))
            result = IPAddress.Loopback.ToString();

        //"TryParse" doesn't support IPv4 with port number
        if (IPAddress.TryParse(result ?? string.Empty, out var ip))
            //IP address is valid 
            result = ip.ToString();
        else if (!string.IsNullOrEmpty(result))
            //remove port
            result = result.Split(':').FirstOrDefault();

        return result;
    }
        
    public static IWebHostEnvironment ResolveHostingEnvironment(this IServiceProvider provider)
    {
        var res = provider.GetRequiredService<IWebHostEnvironment>();
        return res;
    }

    /// <summary>
    /// 获取上传文件的字节数组
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<byte[]> GetBytes(this IFormFile file)
    {
        using var ms = new MemoryStream();
        using var s = file.OpenReadStream();
        await s.CopyToAsync(ms);
        var res = ms.ToArray();
        return res;
    }

    public static Dictionary<string, string> ToDict(this IEnumerable<KeyValuePair<string, StringValues>> data)
    {
        var res = data.ToDict(x => x.Key, x => (string)x.Value);
        return res;
    }
}