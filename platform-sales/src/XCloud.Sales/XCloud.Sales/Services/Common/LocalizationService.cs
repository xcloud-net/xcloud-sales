using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Localization;

namespace XCloud.Sales.Services.Common;

public  interface ILocalizationService : ISalesAppService
{
    string GetResource(string resourceKey);

    string GetResource(string resourceKey,
        bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false);

}

public class LocalizationService : SalesAppService, ILocalizationService
{
    private readonly ISalesRepository<LocaleStringResource> _lsrRepository;

    public LocalizationService(
        ISalesRepository<LocaleStringResource> lsrRepository)
    {
        this._lsrRepository = lsrRepository;
    }

    public virtual string GetResource(string resourceKey)
    {
        return GetResource(resourceKey, true, "", false);
    }

    public virtual string GetResource(string resourceKey,
        bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
    {
        throw new NotImplementedException();
    }


}