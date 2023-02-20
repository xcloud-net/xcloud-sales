using System.Net.Http;
using System.Text;
using Volo.Abp.Http;

namespace XCloud.Core.Http.Content;

public class JsonContent : StringContent
{
    private const string MediaType = MimeTypes.Application.Json;

    public JsonContent(string json, Encoding encoding = null) :
        base(
            content: json ?? throw new ArgumentNullException(nameof(json)),
            encoding: encoding ?? Encoding.UTF8,
            mediaType: MediaType)
    {
        //
    }
}