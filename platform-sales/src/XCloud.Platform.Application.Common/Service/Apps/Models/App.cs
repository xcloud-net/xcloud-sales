using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Application.Common.Service.Apps.Models;

public class App : IEntityDto<string>
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public AppFunctionGroup FunctionGroups { get; set; }
}