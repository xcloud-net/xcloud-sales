using System.Collections.Generic;
using Volo.Abp.Auditing;
using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Menu;

/// <summary>
/// 导航/边栏/轮播都属于菜单
/// </summary>
public class SysMenu : TreeEntityBase, IPlatformEntity, IHasModificationTime, IHasAppFields
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Url { get; set; }

    public string ImageUrl { get; set; }

    public string IconCls { get; set; }

    public string IconUrl { get; set; }

    public int Sort { get; set; }

    public string PermissionJson { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public IEnumerable<string> RequiredPermissions { get; set; }
    public string AppKey { get; set; }
}