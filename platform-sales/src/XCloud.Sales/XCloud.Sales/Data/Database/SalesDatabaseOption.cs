using Volo.Abp.Application.Dtos;

namespace XCloud.Sales.Data.Database;

public class SalesDatabaseOption : IEntityDto, ISingletonDependency
{
    public SalesDatabaseOption()
    {
        this.AutoCreateDatabase = false;
    }

    public bool AutoCreateDatabase { get; set; } = false;
}