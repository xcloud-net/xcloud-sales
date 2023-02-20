using Newtonsoft.Json;
using System.Reflection;

namespace XCloud.Sales.Core.Plugins;

/// <summary>
/// Represents a plugin descriptor
/// </summary>
public class PluginDescriptor
{
    /// <summary>
    /// Gets or sets the plugin group
    /// </summary>
    public virtual string Group { get; set; }

    /// <summary>
    /// Gets or sets the plugin friendly name
    /// </summary>
    public virtual string FriendlyName { get; set; }

    /// <summary>
    /// Gets or sets the plugin system name
    /// </summary>
    public virtual string SystemName { get; set; }

    /// <summary>
    /// Gets or sets the version
    /// </summary>
    public virtual string Version { get; set; }

    public virtual IList<string> SupportedVersions { get; set; }

    /// <summary>
    /// Gets or sets the author
    /// </summary>
    public virtual string Author { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public virtual int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the name of the assembly file
    /// </summary>
    [JsonProperty(PropertyName = "FileName")]
    public virtual string AssemblyFileName { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public virtual string Description { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether plugin is installed
    /// </summary>
    [JsonIgnore]
    public virtual bool Installed { get; set; }

    /// <summary>
    /// Gets or sets the plugin type
    /// </summary>
    [JsonIgnore]
    public virtual Type PluginType { get; set; }

    /// <summary>
    /// Gets or sets the original assembly file that a shadow copy was made from it
    /// </summary>
    [JsonIgnore]
    public virtual string OriginalAssemblyFile { get; internal set; }

    /// <summary>
    /// Gets or sets the assembly that has been shadow copied that is active in the application
    /// </summary>
    [JsonIgnore]
    public virtual Assembly ReferencedAssembly { get; internal set; }

        
}