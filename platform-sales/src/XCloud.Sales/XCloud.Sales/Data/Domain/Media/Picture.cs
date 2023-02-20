using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Media;

/// <summary>
/// Represents a picture
/// </summary>
public class Picture : SalesBaseEntity
{
    /// <summary>
    /// Gets or sets the picture mime type
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// Gets or sets the SEO friednly filename of the picture
    /// </summary>
    public string SeoFilename { get; set; }

    public string ResourceId { get; set; }

    public string ResourceData { get; set; }

}