using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using XCloud.Core.Dto;
using XCloud.Platform.Common.Application.Service.Region;
using XCloud.Platform.Core.Domain.Region;
using XCloud.Platform.Framework.Controller;

namespace XCloud.Platform.Api.Controller;

[Route("/api/platform/region")]
public class RegionController : PlatformBaseController
{
    private readonly IRegionService _regionService;
    public RegionController(IRegionService regionService)
    {
        this._regionService = regionService;
    }

    [HttpPost("by-parent")]
    public async Task<ApiResponse<SysRegion[]>> QueryByParentIdAsync([FromBody] IdDto dto)
    {
        var data = await this._regionService.QueryByParentIdAsync(dto.Id);
        return new ApiResponse<SysRegion[]>().SetData(data);
    }
}