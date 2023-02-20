namespace XCloud.Sales.Core;

public class SalesJobOption : ISingletonDependency
{
    public SalesJobOption()
    {
        this.AutoStartJob = true;
    }

    public bool AutoStartJob { get; set; } = true;
}