using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Data.EntityFrameworkCore;

public class PlatformEfCoreOption : ISingletonDependency
{
    public PlatformEfCoreOption()
    {
        this.AutoCreateDatabase = true;
    }

    public bool AutoCreateDatabase { get; set; } = true;
}