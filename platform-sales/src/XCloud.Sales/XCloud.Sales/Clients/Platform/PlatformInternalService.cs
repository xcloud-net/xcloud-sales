using JetBrains.Annotations;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Platform.Common.Application.Service.Address;
using XCloud.Platform.Common.Application.Service.IdGenerator;
using XCloud.Platform.Core.Domain.Address;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Member.Application.Service.Admin;
using XCloud.Platform.Member.Application.Service.AdminPermission;
using XCloud.Platform.Member.Application.Service.User;
using XCloud.Platform.Shared.Dto;
using XCloud.Sales.Application;
using XCloud.Sales.Service.Users;
using IUserProfileService = XCloud.Platform.Member.Application.Service.User.IUserProfileService;

namespace XCloud.Sales.Clients.Platform;

[ExposeServices(typeof(PlatformInternalService))]
public class PlatformInternalService : SalesAppService
{
    private readonly IExternalConnectService _externalConnectService;
    private readonly IUserAddressService _userAddressService;
    private readonly ISequenceGeneratorService _sequenceGeneratorService;
    private readonly IAdminAccountService _adminAccountService;
    private readonly IAdminService _adminService;
    private readonly IUserProfileService _userProfileService;
    private readonly IAdminPermissionService _adminPermissionService;

    public PlatformInternalService(IUserProfileService userProfileService,
        IAdminService adminService,
        IExternalConnectService externalConnectService,
        ISequenceGeneratorService sequenceGeneratorService,
        IUserAddressService userAddressService,
        IAdminPermissionService adminPermissionService, IAdminAccountService adminAccountService)
    {
        this._userProfileService = userProfileService;
        this._adminService = adminService;
        this._sequenceGeneratorService = sequenceGeneratorService;
        this._userAddressService = userAddressService;
        _adminPermissionService = adminPermissionService;
        _adminAccountService = adminAccountService;
        this._externalConnectService = externalConnectService;
    }

    public async Task<GrantedPermissionResponse> QueryAdminPermissionsAsync(string adminId)
    {
        var dto = new GetGrantedPermissionInput() { AdminId = adminId };

        var response =
            await this._adminPermissionService.GetGrantedPermissionsAsync(dto, new CachePolicy() { Cache = true });

        return response;
    }

    public async Task<SysAdminDto[]> AttachSysUserAsync(SysAdminDto[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        var userIds = data.Select(x => x.UserId).WhereNotEmpty().Distinct().ToArray();
        if (userIds.Any())
        {
            var sysUsers = await this.QueryUserProfileByIdsAsync(userIds);
            foreach (var m in data)
            {
                m.SysUser = sysUsers.FirstOrDefault(x => x.Id == m.UserId);
            }
        }

        return data;
    }

    public async Task<StoreUserDto[]> AttachSysUserAsync(StoreUserDto[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        var userIds = data.Select(x => x.GlobalUserId).WhereNotEmpty().Distinct().ToArray();
        if (userIds.Any())
        {
            var sysUsers = await this.QueryUserProfileByIdsAsync(userIds);
            foreach (var m in data)
            {
                m.SysUser = sysUsers.FirstOrDefault(x => x.Id == m.GlobalUserId);
            }
        }

        return data;
    }

    public async Task<SysUserDto> QueryProfileByUserIdAsync(string userId)
    {
        var response = await this._userProfileService.QueryProfileByUserIdAsync(new IdDto(userId));
        return response;
    }

    public async Task<SysUserDto[]> QueryUserProfileByIdsAsync(string[] ids)
    {
        if (!ids.Any())
            return Array.Empty<SysUserDto>();
        var response = await this._userProfileService.QueryUserProfileByIdsAsync(ids);
        return response;
    }

    public async Task<SysAdminDto[]> QueryAdminProfileByIdsAsync(string[] ids)
    {
        var response = await this._adminService.GetAdminByIdsAsync(ids);
        return response;
    }

    public async Task<ApiResponse<SysAdminDto>> AdminAuthAsync([NotNull] SysUserDto dto)
    {
        var response = await this._adminAccountService.GetAdminAuthResultByUserIdAsync(dto.UserId);

        return response;
    }

    [Obsolete("xxdd")]
    public async Task<ApiResponse<SysExternalConnect>> QueryUserConnectionAsync(
        QueryUserConnectionRequest dto)
    {
        var data = await this._externalConnectService.FindByUserIdAsync(dto.Platform, dto.AppId,
            dto.UserId);

        if (data == null)
            return new ApiResponse<SysExternalConnect>().SetError("no connection found");

        return new ApiResponse<SysExternalConnect>(data);
    }

    public async Task<ApiResponse<int>> GenerateSerialNoAsync(CreateNoByCategoryDto dto)
    {
        var sn = await this._sequenceGeneratorService.GenerateNoWithDistributedLockAsync(dto);

        return new ApiResponse<int>(sn);
    }

    public async Task<ApiResponse<UserAddressDto[]>> QueryUserAddressByUserIdAsync(IdDto dto)
    {
        try
        {
            var allAddress = await this._userAddressService.QueryByUserIdAsync(dto.Id);

            var addressDtos = this.ObjectMapper.MapArray<UserAddress,UserAddressDto>(allAddress);

            return new ApiResponse<UserAddressDto[]>(addressDtos);
        }
        catch (Exception e)
        {
            throw new PlatformApiException(e.Message, e);
        }
    }
}