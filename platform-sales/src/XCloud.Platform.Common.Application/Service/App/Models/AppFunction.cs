using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

namespace XCloud.Platform.Common.Application.Service.App.Models;

public class AppFunctionGroup : ExtensibleObject, IEntityDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public AppFunction[] Functions { get; set; }
}

public class AppFunction : ExtensibleObject, IEntityDto
{
    public string Key { get; set; }
    public string Description { get; set; }
}