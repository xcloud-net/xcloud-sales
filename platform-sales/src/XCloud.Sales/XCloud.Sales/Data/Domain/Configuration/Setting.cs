using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Configuration;

/// <summary>
/// Represents a setting
/// </summary>
public class Setting : SalesBaseEntity
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the value
    /// </summary>
    public string Value { get; set; }
}