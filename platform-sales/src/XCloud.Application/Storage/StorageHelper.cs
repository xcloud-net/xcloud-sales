using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http;

namespace XCloud.Application.Storage;

public class StorageHelper : IScopedDependency
{
    private readonly ILogger logger;

    public StorageHelper(ILogger<StorageHelper> logger)
    {
        this.logger = logger;
    }

    public string ResolveContentTypeOrNull(string resourceKey)
    {
        if (string.IsNullOrWhiteSpace(resourceKey))
            return null;
        var lowerResourceKey = resourceKey.ToLower();
        var ext = this.GetNormalizedFileExtension(lowerResourceKey).TrimStart('.');

        switch (ext)
        {
            case "jpg":
            case "jpeg":
                return MimeTypes.Image.Jpeg;
            case "png":
                return MimeTypes.Image.Png;
            case "gif":
                return MimeTypes.Image.Gif;
            case "bmp":
                return MimeTypes.Image.Bmp;
            case "webp":
                return MimeTypes.Image.Webp;
            case "svg":
                return MimeTypes.Image.SvgXml;
            case "tiff":
                return MimeTypes.Image.Tiff;
            default:
                return null;
        }
    }

    public long CalculateStreamLength(Stream stream)
    {
        try
        {
            return stream.Length;
        }
        catch (Exception e)
        {
            this.logger.LogWarning(message: e.Message, exception: e);

            if (stream.Position != 0)
                stream.Seek(0, SeekOrigin.Begin);

            var length = default(long);
            var buffer = new byte[1000];
            while (true)
            {
                var len = stream.Read(buffer, 0, buffer.Length);
                if (len <= 0)
                    break;
                length += len;
            }
            return length;
        }
    }

    public bool IsImage(string fileName)
    {
        var extension = this.GetNormalizedFileExtension(fileName);

        var imageExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };

        foreach (var m in imageExtensions)
        {
            if (m.ToLower() == extension)
                return true;
        }

        return false;
    }

    public string GetOriginFileExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        if (Path.HasExtension(fileName))
        {
            var res = Path.GetExtension(fileName);
            return $".{res.TrimStart('.')}";
        }
        else
        {
            var special = new[] { ".tar.gz" };

            foreach (var m in special)
            {
                if (fileName.EndsWith(m, StringComparison.OrdinalIgnoreCase))
                    return m;
            }

            //get last part
            fileName = fileName.Split('/', '\\').Last();

            var index = fileName.LastIndexOf('.');
            if (index >= 0 && index < fileName.Length - 1)
            {
                //xxx.png
                //3,4
                var ext = fileName.Substring(index, length: fileName.Length - 1 - index);
                return ext;
            }

            return string.Empty;
        }
    }

    public string GetNormalizedFileExtension(string file_name)
    {
        var extension = this.GetOriginFileExtension(file_name);
        if (string.IsNullOrWhiteSpace(extension))
            return extension;

        extension = extension.ToLower();
        if (extension == ".jpeg")
            extension = ".jpg";

        return extension;
    }

}