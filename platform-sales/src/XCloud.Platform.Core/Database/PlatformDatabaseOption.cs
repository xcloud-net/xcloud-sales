using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Core.Database;

public class PlatformDatabaseOption : IEntityDto, ISingletonDependency
{
    public PlatformDatabaseOption()
    {
        this.AutoCreateDatabase = false;
    }

    public bool AutoCreateDatabase { get; set; } = false;
}