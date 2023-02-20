using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XCloud.Core.Dto;
using XCloud.Platform.Auth.Application.Admin;
using XCloud.Platform.Common.Application.Service.Storage;
using XCloud.Platform.Framework.Controller;

namespace XCloud.Platform.Api.Controller.Admin;

[Route("/api/sys/fs")]
public class SysFsController : PlatformBaseController, IAdminController
{
    private readonly IStorageMetaService _storageMetaService;

    public SysFsController(IStorageMetaService storageMetaService)
    {
        _storageMetaService = storageMetaService;
        //
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<StorageResourceMetaDto>> QueryPagingAsync([FromBody] QueryStoragePagingInput dto)
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        var response = await this._storageMetaService.QueryPagingAsync(dto);

        return response;
    }
}