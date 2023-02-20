using Microsoft.Extensions.Configuration;

using Newtonsoft.Json.Linq;

namespace XCloud.Core.DataSerializer;

public static class SerializerExtension
{
    public static T DeserializeFromStringOrDefault<T>(this IJsonDataSerializer jsonDataSerializer, 
        string json,
        ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentNullException(nameof(json));
        
        try
        {
            var obj = jsonDataSerializer.DeserializeFromString<T>(json);
            return obj;
        }
        catch (SerializeException e)
        {
            logger.LogWarning(message: e.Message, exception: e);
        }
        catch (Exception e)
        {
            logger.LogWarning(message: e.Message, exception: e);
        }

        return default;
    }

    public static string GetDateformatOrDefault(this IConfiguration config)
    {
        var format = config["app:config:date_format"];
        if (string.IsNullOrWhiteSpace(format))
        {
            format = "yyyy-MM-dd HH:mm:ss";
        }
        return format;
    }

    /// <summary>
    /// 比较两个json结构是否相同
    /// </summary>
    /// <param name="json_1"></param>
    /// <param name="json_2"></param>
    /// <returns></returns>
    public static bool HasSameStructure(string json_1, string json_2)
    {
        var path_list = new List<string>();
        void FindJsonNode(JToken token)
        {
            if (token == null) { return; }
            if (token.Type == JTokenType.Property)
            {
                path_list.Add(token.Path);
            }
            //find next node
            var children = token.Children();
            foreach (var child in children)
            {
                FindJsonNode(child);
            }
        }

        FindJsonNode(JToken.Parse(json_1));
        FindJsonNode(JToken.Parse(json_2));

        return path_list.Count == path_list.Distinct().Count() * 2;
    }
}