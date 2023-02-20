using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Core.Job;

public class PlatformJobOption : ISingletonDependency
{
    public PlatformJobOption()
    {
        this.AutoStartJob = true;
    }

    public bool AutoStartJob { get; set; } = true;
}