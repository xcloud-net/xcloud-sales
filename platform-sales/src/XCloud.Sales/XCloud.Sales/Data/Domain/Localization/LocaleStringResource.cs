using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Localization;

public class LocaleStringResource : SalesBaseEntity
{
    public string Name { get; set; }

    public string Value { get; set; }
}