using XCloud.Core.Dto;
using XCloud.Platform.Common.Application.Service.Address;
using XCloud.Sales.Clients.Platform;

namespace XCloud.Sales.Services.Common;

public interface IAddressService : ISalesAppService
{
    Task<UserAddressDto[]> QueryByGlobalUserId(string globalUserId);
}

public class AddressService : SalesAppService, IAddressService
{
    private readonly PlatformInternalService _internalCommonClientService;

    public AddressService(PlatformInternalService internalCommonClientService)
    {
        this._internalCommonClientService = internalCommonClientService;
    }

    public async Task<UserAddressDto[]> QueryByGlobalUserId(string globalUserId)
    {
        var res = await this._internalCommonClientService.QueryUserAddressByUserIdAsync(new IdDto(globalUserId));
        if (!res.IsSuccess())
        {
            this.Logger.LogError(message: res.Error?.Message);
        }

        return res.Data ?? Array.Empty<UserAddressDto>();
    }
}