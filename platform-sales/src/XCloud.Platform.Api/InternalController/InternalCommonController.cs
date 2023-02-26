using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Platform.Common.Application.Service.Address;
using XCloud.Platform.Common.Application.Service.IdGenerator;
using XCloud.Platform.Framework.Controller;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Api.InternalController;

[Route("/internal-api/platform/common")]
public class InternalCommonController : PlatformBaseController
{
    private readonly IUserAddressService _userAddressService;
    private readonly ISequenceGeneratorService _sequenceGeneratorService;

    public InternalCommonController(IUserAddressService userAddressService,
        ISequenceGeneratorService sequenceGeneratorService)
    {
        this._userAddressService = userAddressService;
        this._sequenceGeneratorService = sequenceGeneratorService;
    }

    [HttpPost("create-no")]
    public async Task<ApiResponse<int>> CreateNo([FromBody] CreateNoByCategoryDto dto)
    {
        var res = await this._sequenceGeneratorService.GenerateNoWithDistributedLockAsync(dto);

        return new ApiResponse<int>(res);
    }
}