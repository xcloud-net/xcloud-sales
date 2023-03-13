using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Application.Service;
using XCloud.Core.Dto;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Application.Member.Service.Admin;

public interface IAdminService : IXCloudApplicationService
{
    Task<SysAdminDto> GetAdminByIdAsync(IdDto adminIdDto);

    Task<SysAdminDto[]> GetAdminByIdsAsync(string[] ids);

    Task<PagedResponse<SysAdminDto>> QueryAdminPagingAsync(QueryAdminDto filter);

    Task<SysAdminDto> GetAdminProfileByIdAsync(IdDto adminIdDto, CachePolicy cachePolicyOption);
}

public class AdminService : PlatformApplicationService, IAdminService
{
    private readonly IMemberRepository<SysAdmin> _adminRepository;
    private readonly IUserProfileService _userProfileService;

    public AdminService(IMemberRepository<SysAdmin> adminRepository,
        IUserProfileService userProfileService)
    {
        this._userProfileService = userProfileService;
        this._adminRepository = adminRepository;
    }

    public async Task<SysAdminDto> GetAdminProfileByIdAsync(IdDto adminIdDto, CachePolicy cachePolicyOption)
    {
        var key = $"sys-admin-profile-admin:{adminIdDto.Id}";
        var sysAdmin = await this.CacheProvider.ExecuteWithPolicyAsync(() => this.QueryAdminProfileByIdAsync(adminIdDto),
            new CacheOption<SysAdminDto>(key, TimeSpan.FromMinutes(3)),
            cachePolicyOption);
        return sysAdmin;
    }

    public async Task<SysAdminDto> QueryAdminProfileByIdAsync(IdDto adminIdDto)
    {
        var dto = await this.GetAdminByIdAsync(adminIdDto);

        if (!string.IsNullOrWhiteSpace(dto.UserId))
        {
            dto.SysUser = await this._userProfileService.QueryProfileByUserIdAsync(dto.UserId);
        }

        return dto;
    }

    public async Task<SysAdminDto> GetAdminByIdAsync(IdDto adminIdDto)
    {
        if (adminIdDto == null || string.IsNullOrWhiteSpace(adminIdDto.Id))
            throw new ArgumentNullException(nameof(adminIdDto));

        var db = await this._adminRepository.GetDbContextAsync();

        var data = await db.Set<SysAdmin>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == adminIdDto.Id);

        if (data == null)
            return null;

        var dto = this.ObjectMapper.Map<SysAdmin, SysAdminDto>(data);

        return dto;
    }

    public async Task<SysAdminDto[]> GetAdminByIdsAsync(string[] ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));
        if (!ids.Any())
            return Array.Empty<SysAdminDto>();

        var db = await this._adminRepository.GetDbContextAsync();

        var query = from admin in db.Set<SysAdmin>().AsNoTracking()
            join u in db.Set<SysUser>().AsNoTracking()
                on admin.UserId equals u.Id into userGrouping
            from user in userGrouping.DefaultIfEmpty()
            select new { admin, user };

        query = query.Where(x => ids.Contains(x.admin.Id));

        var data = await query.OrderByDescending(x => x.admin.CreationTime).ToArrayAsync();

        var response = data.Select(x => this.BuildAdminDto(x.admin, x.user)).ToArray();
        return response;
    }

    SysAdminDto BuildAdminDto(SysAdmin admin, SysUser user = null)
    {
        var dto = this.ObjectMapper.Map<SysAdmin, SysAdminDto>(admin);
        if (user != null)
        {
            dto.IdentityName = user.IdentityName;
            dto.OriginIdentityName = user.OriginIdentityName;
            dto.NickName = user.NickName;
            dto.Avatar = user.Avatar;
            dto.SysUser = this.ObjectMapper.Map<SysUser, SysUserDto>(user);
        }

        return dto;
    }

    public async Task<PagedResponse<SysAdminDto>> QueryAdminPagingAsync(QueryAdminDto filter)
    {
        var db = await this._adminRepository.GetDbContextAsync();

        var query = from admin in db.Set<SysAdmin>().IgnoreQueryFilters().AsNoTracking()
            join u in db.Set<SysUser>().IgnoreQueryFilters().AsNoTracking()
                on admin.UserId equals u.Id into userGrouping
            from user in userGrouping.DefaultIfEmpty()
            select new { admin, user };

        if (filter.IsActive != null)
        {
            query = query.Where(x => x.admin.IsActive == filter.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            query = query.Where(x =>
                x.user.IdentityName == filter.Keyword || x.user.NickName.Contains(filter.Keyword));
        }

        var count = 0;
        if (!filter.SkipCalculateTotalCount)
            count = await query.CountAsync();
            
        var items = await query
            .OrderByDescending(x => x.admin.IsActive)
            .ThenByDescending(x=>x.admin.CreationTime)
            .PageBy(filter.ToAbpPagedRequest())
            .ToArrayAsync();

        var list = new List<SysAdminDto>();
        foreach (var m in items)
        {
            list.Add(this.BuildAdminDto(m.admin, m.user));
        }

        return new PagedResponse<SysAdminDto>(list, filter, count);
    }
}