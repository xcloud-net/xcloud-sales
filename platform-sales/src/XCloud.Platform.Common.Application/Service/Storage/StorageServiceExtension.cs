using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using Volo.Abp.Http;
using Volo.Abp.Validation;
using XCloud.Core.Helper;
using XCloud.Platform.Core.Domain.Storage;

namespace XCloud.Platform.Common.Application.Service.Storage;

public static class StorageServiceExtension
{
    public static async Task<byte[]> GetFileBytesOrNullAsync(this IStorageService storageService, string fileKey)
    {
        var stream = await storageService.GetFileStreamOrNullAsync(fileKey);

        if (stream != null)
        {
            using (stream)
            {
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var bs = ms.ToArray();
                return bs;
            }
        }

        return null;
}
    
    public static async Task<string> GetContentTypeOrDefaultAsync(this IStorageMetaService storageMetaService,
        string key, string defaultContentType = default)
    {
        var contentType = await storageMetaService.GetContentTypeOrNullAsync(key);

        if (string.IsNullOrWhiteSpace(contentType))
        {
            contentType = defaultContentType ?? MimeTypes.Application.OctetStream;
        }

        return contentType;
    }

    public static async Task<StorageResourceMeta> UploadBytesAsync(this IStorageService storageService,
        FileUploadBytesArgs args)
    {
        if (args == null || ValidateHelper.IsEmptyCollection(args.Bytes))
            throw new AbpValidationException(nameof(UploadBytesAsync));

        using var ms = new MemoryStream();
        await ms.WriteAsync(args.Bytes);

        var streamArgs = new FileUploadStreamArgs(ms)
        {
            FileName = args.FileName,
            ContentType = args.ContentType
        };

        var model = await storageService.UploadStreamAsync(streamArgs);

        return model;
    }

    public static async Task<StorageResourceMeta> UploadFromUrlAsync(this IStorageService storageService,
        HttpClient httpClient,
        FileUploadFromUrl dto)
    {
        dto.Url.Should().NotBeNullOrEmpty();
        dto.FileName.Should().NotBeNullOrEmpty();

        using var response = await httpClient.GetAsync(dto.Url);

        using var stream = await response.Content.ReadAsStreamAsync();

        var args = new FileUploadStreamArgs(stream)
        {
            FileName = dto.FileName,
        };

        if (response.Headers.TryGetValues(HeaderNames.ContentType, out var val))
        {
            args.ContentType = val.FirstOrDefault();
        }

        return await storageService.UploadStreamAsync(args);
    }
}