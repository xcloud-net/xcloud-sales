using Volo.Abp.Application.Dtos;

namespace XCloud.Sales.Core.Settings;

public class SalesJobOption : IEntityDto, ISingletonDependency
{
    public SalesJobOption()
    {
        this.AutoStartJob = true;
    }

    public bool AutoStartJob { get; set; } = true;
}