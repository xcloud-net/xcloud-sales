namespace XCloud.Sales.Data.Database;

public class SalesEfCoreOption : ISingletonDependency
{
    public SalesEfCoreOption()
    {
        this.AutoCreateDatabase = true;
    }

    public bool AutoCreateDatabase { get; set; } = true;
}