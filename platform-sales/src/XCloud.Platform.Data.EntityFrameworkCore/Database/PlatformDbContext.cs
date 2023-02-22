using XCloud.Database.EntityFrameworkCore.DatabaseContext;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Database.EntityFrameworkCore.MySQL.Mapping;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Core.Domain.Address;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Core.Domain.App;
using XCloud.Platform.Core.Domain.AsyncJob;
using XCloud.Platform.Core.Domain.Department;
using XCloud.Platform.Core.Domain.IdGenerator;
using XCloud.Platform.Core.Domain.Logging;
using XCloud.Platform.Core.Domain.Menu;
using XCloud.Platform.Core.Domain.Notification;
using XCloud.Platform.Core.Domain.Region;
using XCloud.Platform.Core.Domain.Security;
using XCloud.Platform.Core.Domain.Settings;
using XCloud.Platform.Core.Domain.Storage;
using XCloud.Platform.Core.Domain.Token;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Platform.Data.EntityFrameworkCore.Database;

/// <summary>
/// 账号体系数据库
/// </summary>
public class PlatformDbContext : AbpDbContextBase<PlatformDbContext>
{
    public PlatformDbContext(IServiceProvider serviceProvider, DbContextOptions<PlatformDbContext> option)
        : base(serviceProvider, option)
    {
        //
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyUtf8Mb4ForAll();
        modelBuilder.ConfigEntityMapperFromAssemblies<IPlatformEntity>(typeof(PlatformDataEntityFrameworkCoreModule)
            .Assembly);
    }

    //管理员
    public DbSet<SysAdmin> AdminEntity { get; set; }

    //用户
    public DbSet<SysUser> UserEntity { get; set; }
    public DbSet<SysUserIdentity> UserPhoneEntity { get; set; }
    public DbSet<SysUserRealName> RealNameEntity { get; set; }

    //部门
    public DbSet<SysDepartment> DepartmentEntity { get; set; }
    public DbSet<SysDepartmentAssign> DepartmentAssignmentEntity { get; set; }


    //角色
    public DbSet<SysRole> RoleEntity { get; set; }
    public DbSet<SysAdminRole> UserRoleEntity { get; set; }

    //权限
    public DbSet<SysResourceAcl> ResourceAclEntity { get; set; }
    public DbSet<SysRolePermission> PermissionAssignmentEntity { get; set; }

    //common service

    public DbSet<SysApp> AppEntity { get; set; }
    public DbSet<SysAppScope> AppScopeEntity { get; set; }

    public DbSet<UserAddress> AddressEntities { get; set; }
    public DbSet<SysNotification> NotificationEntities { get; set; }
    public DbSet<SysSequence> SequenceEntity { get; set; }
    public DbSet<JobRecord> QueueJobEntity { get; set; }

    public DbSet<StorageResourceMeta> FileUploadEntity { get; set; }

    public DbSet<SysMenu> MenuEntity { get; set; }

    public DbSet<SysSettings> KvStoreEntity { get; set; }

    public DbSet<ValidationToken> ValidationTokenEntity { get; set; }

    public DbSet<ActivityLog> OperationLogEntities { get; set; }

    public DbSet<SysRegion> RegionEntities { get; set; }
}