using Volo.Abp.Data;
using XCloud.Application.Service;
using XCloud.Core;

namespace XCloud.Sales.Data.DataSeeder;

[ExposeServices(typeof(SalesDataSeederExecutor))]
public class SalesDataSeederExecutor : XCloudApplicationService
{
    private readonly IDataSeeder _dataSeeder;

    public SalesDataSeederExecutor(IDataSeeder dataSeeder)
    {
        _dataSeeder = dataSeeder;
    }

    public async Task Execute()
    {
        this.Logger.LogInformation("开始执行数据库初始化工作");
        if (this.Configuration.InitDatabaseRequired())
        {
            await this._dataSeeder.SeedAsync(new DataSeedContext() { });
            this.Logger.LogInformation("初始化数据库完成！");
        }
        else
        {
            this.Logger.LogInformation("配置关闭了数据库初始化");
        }
    }
}