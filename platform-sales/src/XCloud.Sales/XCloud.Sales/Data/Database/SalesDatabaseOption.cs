namespace XCloud.Sales.Data.Database;

public class SalesDatabaseOption : ISingletonDependency
{
    public SalesDatabaseOption()
    {
        this.AutoCreateDatabase = false;
    }

    public bool AutoCreateDatabase { get; set; } = false;
}