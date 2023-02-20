using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Configuration;

namespace XCloud.Sales.Services.Configuration;

[Obsolete]
public interface ISettingService : ISalesAppService
{
    //
}

[Obsolete]
public class SettingService : SalesAppService, ISettingService
{
    private readonly ISalesRepository<Setting> _settingRepository;

    public SettingService(ISalesRepository<Setting> settingRepository)
    {
        this._settingRepository = settingRepository;
    }
}