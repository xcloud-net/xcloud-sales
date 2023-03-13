using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;
using XCloud.Core.Application.Entity;

namespace XCloud.Platform.Application.Common.Service.App.Models;

public class App : ExtensibleObject, IHasIdentityNameFields, IEntityDto
{
    public string DisplayName { get; set; }
    public string Description { get; set; }

    public string IdentityName { get; set; }
    public string OriginIdentityName { get; set; }

    public AppFunctionGroup FunctionGroups { get; set; }
}