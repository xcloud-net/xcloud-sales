using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Core.Database;

public class PlatformDatabaseOption : ISingletonDependency
{
    public PlatformDatabaseOption()
    {
        this.AutoCreateDatabase = false;
    }

    public bool AutoCreateDatabase { get; set; } = false;
}