using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using XCloud.Application.Images;
using XCloud.Application.Storage;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Common.Application.Configuration;
using XCloud.Platform.Common.Application.Extension;
using XCloud.Platform.Common.Application.Service.Storage;
using XCloud.Platform.Framework.Controller;
using XCloud.Platform.Shared;
using XCloud.Platform.Shared.Dto;
using XCloud.Platform.Shared.Storage;

namespace XCloud.Platform.Api.Controller;

[Route("/api/platform/storage")]
public class StorageController : PlatformBaseController, IUserController
{
    private readonly IRemoteServiceConfigurationProvider _remoteServiceConfigurationProvider;
    private readonly IStorageService _fileUploadService;
    private readonly IStorageMetaService _storageMetaService;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IStorageUrlResolver _storageUrlResolver;
    private readonly IThumborService _thumborService;
    private readonly StorageHelper _storageHelper;

    public StorageController(IStorageService fileUploadService,
        IStorageMetaService storageMetaService,
        IStorageUrlResolver storageUrlResolver,
        IImageProcessingService imageProcessingService,
        IThumborService thumborService,
        IRemoteServiceConfigurationProvider remoteServiceConfigurationProvider,
        StorageHelper storageHelper)
    {
        this._remoteServiceConfigurationProvider = remoteServiceConfigurationProvider;
        this._storageUrlResolver = storageUrlResolver;
        this._storageMetaService = storageMetaService;
        this._imageProcessingService = imageProcessingService;
        this._thumborService = thumborService;
        this._storageHelper = storageHelper;

        this._fileUploadService = fileUploadService;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    [HttpPost("upload")]
    public async Task<ApiResponse<StorageMetaDto[]>> UploadAsync()
    {
        if (!this.Request.Form.Files.Any())
            return new ApiResponse<StorageMetaDto[]>().SetError("empty");

        if (this.Request.Form.Files.Count > 3)
            return new ApiResponse<StorageMetaDto[]>().SetError("3 files is the limitation");

        foreach (var m in this.Request.Form.Files)
        {
            if (m.Length > 1024 * 1024 * 1)
                return new ApiResponse<StorageMetaDto[]>().SetError($"{m.FileName} reach the limit size");
        }

        var loginUser = this.GetRequiredAuthedUserAsync();

        var resultList = new List<StorageMetaDto>();

        foreach (var m in this.Request.Form.Files)
        {
            using var stream = m.OpenReadStream();
            var args = new FileUploadStreamArgs(stream)
            {
                FileName = m.FileName,
                ContentType = m.ContentType
            };

            var res = await this._fileUploadService.UploadStreamAsync(args);

            var dto = res.ToDto();

            var storageResponse = await this._storageUrlResolver.ResolveUrlAsync(dto);
            if (storageResponse.IsSuccess())
                dto.Url = storageResponse.Data;

            resultList.Add(dto);
        }

        return new ApiResponse<StorageMetaDto[]>(resultList.ToArray());
    }

    [HttpGet("file/origin/{key}")]
    public async Task<IActionResult> OriginFileAsync([FromRoute] string key)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            var contentType = await this._storageMetaService.GetContentTypeOrDefaultAsync(key);

            var stream = await this._fileUploadService.GetFileStreamOrNullAsync(key);
            if (stream != null)
                return File(stream, contentType);
        }

        return NotFound();
    }

    [NonAction]
    private async Task<IActionResult> GetCompressedResultOrNullAsync(string key, string contentType, int height,
        int width)
    {
        var thumborEnabled = this.Configuration.IsThumborEnabled();
        if (thumborEnabled)
        {
            var gatewayAddress = await this._remoteServiceConfigurationProvider.ResolveGatewayBaseAddressAsync();

            var thumborInputUrl = Com.ConcatUrl(gatewayAddress, "/api/platform/storage/file/origin/", key);

            var stream = await this._thumborService.GetResizedStreamOrNullAsync(thumborInputUrl, height, width);

            if (stream != null)
                return File(stream, contentType);
        }
        else
        {
            var bs = await this._fileUploadService.GetFileBytesOrNullAsync(key);
            if (ValidateHelper.IsNotEmptyCollection(bs))
            {
                var compressedBytes = await this._imageProcessingService.ResizeAsync(bs, width, height);
                return this.File(compressedBytes, contentType);
            }
        }

        return null;
    }

    [HttpGet("file/{w}x{h}/{key}")]
    public async Task<IActionResult> ResizedFileAsync([FromRoute] string key,
        [FromRoute] int? h,
        [FromRoute] int? w)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            var contentType = await this._storageMetaService.GetContentTypeOrDefaultAsync(key);

            h ??= 0;
            w ??= 0;
            
            if ((h.Value > 0 || w.Value > 0) && this._storageHelper.IsImage(key))
            {
                //resize and cache
                var result = await this.GetCompressedResultOrNullAsync(key, contentType, h.Value, w.Value);
                if (result != null)
                    return result;
            }
            else
            {
                var stream = await this._fileUploadService.GetFileStreamOrNullAsync(key);
                if (stream != null)
                    return File(stream, contentType);
            }
        }

        return NotFound();
    }

    [Obsolete("unhandled exception : response stream has started problem")]
    [HttpGet("filev4/{w}x{h}/{key}")]
    public async Task ResizedFileV4Async([FromRoute] string key,
        [FromRoute] int? h,
        [FromRoute] int? w)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            var contentType = await this._storageMetaService.GetContentTypeOrDefaultAsync(key);

            h ??= 0;
            w ??= 0;
            if ((h > 0 || w > 0) && this._storageHelper.IsImage(key))
            {
                var gatewayAddress = await this._remoteServiceConfigurationProvider.ResolveGatewayBaseAddressAsync();

                var thumborInputUrl = Com.ConcatUrl(gatewayAddress, "/api/platform/qcloud-fs/file/origin/", key);

                using var outputStream = this.Response.BodyWriter.AsStream();
                if (await this._thumborService.ResizeAndWriteToStreamAsync(
                        thumborInputUrl, h.Value, w.Value,
                        outputStream))
                {
                    Response.ContentType = contentType;
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    await outputStream.FlushAsync();
                    return;
                }
            }
            else
            {
                var stream = await this._fileUploadService.GetFileStreamOrNullAsync(key);
                if (stream != null)
                {
                    using (stream)
                    {
                        Response.ContentType = contentType;
                        Response.StatusCode = (int)HttpStatusCode.OK;

                        using var outputStream = this.Response.BodyWriter.AsStream();

                        await stream.CopyToAsync(outputStream);
                        await outputStream.FlushAsync();
                        return;
                    }
                }
            }
        }

        Response.StatusCode = (int)HttpStatusCode.NotFound;
    }
}