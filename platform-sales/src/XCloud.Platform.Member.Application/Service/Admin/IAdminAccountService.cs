using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Data.Repository.Admin;

namespace XCloud.Platform.Member.Application.Service.Admin;

public interface IAdminAccountService : IXCloudApplicationService
{
    Task<AdminAuthResponse> GetAdminAuthResultByUserIdAsync(string userId);

    Task UpdateAdminStatusAsync(UpdateAdminStatusDto dto);

    Task<ApiResponse<SysAdmin>> CreateAdminAccountAsync(CreateAdminDto dto);

    Task<SysAdminDto> GetAdminByUserIdAsync(string userId);

    Task<SysAdminDto> GetAdminByUserIdAsync(string userId, CachePolicy cachePolicyOption);
}

public class AdminAccountService : PlatformApplicationService, IAdminAccountService, IAuditingEnabled
{
    private readonly IAdminAccountRepository _adminAccountRepository;

    public AdminAccountService(IAdminAccountRepository userRepo)
    {
        _adminAccountRepository = userRepo;
    }

    public async Task<AdminAuthResponse> GetAdminAuthResultByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var res = new AdminAuthResponse();

        var authData = await this.GetAdminByUserIdAsync(
            userId,
            new CachePolicy() { Cache = true });

        if (authData == null)
            return res.SetError("admin identity is not exist");

        if (!authData.IsActive)
        {
            res.IsNotActive = true;
            return res.SetError("admin identity is disabled");
        }

        res.SetData(authData);
        return res;
    }

    public async Task<SysAdminDto> GetAdminByUserIdAsync(string userId)
    {
        var db = await this._adminAccountRepository.GetDbContextAsync();

        var query = db.Set<SysAdmin>().IgnoreQueryFilters().AsNoTracking();

        query = query.Where(x => x.UserId == userId);

        var authData = await query.OrderBy(x => x.CreationTime).FirstOrDefaultAsync();

        if (authData == null)
            return null;

        var dto = this.ObjectMapper.Map<SysAdmin, SysAdminDto>(authData);
        return dto;
    }

    public async Task<SysAdminDto> GetAdminByUserIdAsync(string userId, CachePolicy cachePolicyOption)
    {
        var option = new CacheOption<SysAdminDto>($"user.{userId}.admin.auth.data");

        var data = await this.CacheProvider.ExecuteWithPolicyAsync(
            () => GetAdminByUserIdAsync(userId),
            option,
            cachePolicyOption);

        return data;
    }

    public async Task<ApiResponse<SysAdmin>> CreateAdminAccountAsync(CreateAdminDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.UserId))
            throw new ArgumentNullException(nameof(dto.UserId));

        var res = new ApiResponse<SysAdmin>();

        var errors = await this.ObjectValidator.GetErrorsAsync(dto);
        if (errors.Any())
        {
            return res.SetError(errors.First().ErrorMessage);
        }

        var model = new SysAdmin()
        {
            UserId = dto.UserId,
            IsActive = true
        };

        model.Id = this.GuidGenerator.CreateGuidString();
        model.CreationTime = this.Clock.Now;

        if (!string.IsNullOrWhiteSpace(model.UserId))
        {
            var adminCreatedByUser = await this.GetAdminByUserIdAsync(new IdDto(model.UserId));
            if (adminCreatedByUser != null)
            {
                return res.SetError("当前用户已经有管理员身份");
            }
        }

        await _adminAccountRepository.InsertNowAsync(model);

        return res.SetData(model);
    }

    public async Task UpdateAdminStatusAsync(UpdateAdminStatusDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var db = await this._adminAccountRepository.GetDbContextAsync();

        var entity = await db.Set<SysAdmin>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateAdminStatusAsync));

        if (dto.IsActive != null)
            entity.IsActive = dto.IsActive.Value;

        if (dto.IsSuperAdmin != null)
            entity.IsSuperAdmin = dto.IsSuperAdmin.Value;

        await _adminAccountRepository.UpdateNowAsync(entity);
    }
}