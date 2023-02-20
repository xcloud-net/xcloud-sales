using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Common.Application.Service.Address;
using XCloud.Platform.Core.Domain.Address;
using XCloud.Platform.Framework.Controller;

namespace XCloud.Platform.Api.Controller.User;

[Route("/api/platform/user/address")]
public class UserAddressController : PlatformBaseController, IUserController
{
    private readonly IUserAddressService _userAddressService;
    public UserAddressController(IUserAddressService userAddressService)
    {
        this._userAddressService = userAddressService;
    }

    [HttpPost("list")]
    public async Task<ApiResponse<UserAddress[]>> ListAsync()
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();

        var allAddress = await this._userAddressService.QueryByUserIdAsync(loginUser.UserId);

        allAddress = allAddress
            .OrderByDescending(x => x.IsDefault.ToBoolInt())
            .ThenByDescending(x => x.CreationTime).ToArray();

        return new ApiResponse<UserAddress[]>().SetData(allAddress);
    }

    [HttpPost("by-id")]
    public async Task<ApiResponse<UserAddress>> ByIdAsync([FromBody] IdDto dto)
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();

        var address = await this._userAddressService.QueryByIdAsync(dto.Id);

        if (address == null)
            return new ApiResponse<UserAddress>().SetError("not exist");

        (address.UserId == loginUser.UserId).Should().BeTrue();

        return new ApiResponse<UserAddress>().SetData(address);
    }

    [HttpPost("save")]
    public async Task<ApiResponse<object>> SaveAsync([FromBody] UserAddress userAddress)
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();

        if (string.IsNullOrWhiteSpace(userAddress.Id))
        {
            userAddress.UserId = loginUser.UserId;
            userAddress = await this._userAddressService.InsertAsync(userAddress);

            await this._userAddressService.UpdateDescriptionAsync(userAddress.Id);

            if (userAddress.IsDefault)
                await this._userAddressService.SetAsDefaultAsync(loginUser.UserId, userAddress.Id);
        }
        else
        {
            var expiredId = userAddress.Id;

            var address = await this._userAddressService.QueryByIdAsync(expiredId);
            if (address == null || address.UserId != loginUser.UserId)
                throw new EntityNotFoundException(nameof(address));

            var addressIsDefault = address.IsDefault;

            //hide previous address
            await this._userAddressService.DeleteByIdAsync(expiredId);

            //insert new one
            userAddress = await this._userAddressService.InsertAsync(userAddress);

            await this._userAddressService.UpdateDescriptionAsync(userAddress.Id);
            //set default if previous address is the default 
            if (addressIsDefault)
                await this._userAddressService.SetAsDefaultAsync(loginUser.UserId, userAddress.Id);

        }

        return new ApiResponse<object>();
    }

    [HttpPost("set-default")]
    public async Task<ApiResponse<object>> SetDefaultAsync([FromBody] IdDto dto)
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();

        await this._userAddressService.SetAsDefaultAsync(loginUser.UserId, dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("delete")]
    public async Task<ApiResponse<object>> DeleteAsync([FromBody] IdDto dto)
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();

        var address = await this._userAddressService.QueryByIdAsync(dto.Id);

        if (address == null)
            return new ApiResponse<object>();

        (address.UserId == loginUser.UserId).Should().BeTrue();

        await this._userAddressService.DeleteByIdAsync(dto.Id);

        return new ApiResponse<object>();
    }

}