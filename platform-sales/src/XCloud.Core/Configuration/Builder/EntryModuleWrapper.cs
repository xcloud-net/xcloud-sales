using System.Reflection;
using FluentAssertions;
using Volo.Abp.Modularity;

namespace XCloud.Core.Configuration.Builder;

public class EntryModuleWrapper
{
    public Type EntryModule { get; }
    public Assembly EntryAssembly => this.EntryModule?.Assembly;

    public EntryModuleWrapper(Type entryModuleType)
    {
        entryModuleType.Should().NotBeNull();
        entryModuleType.IsAssignableTo_<AbpModule>().Should().BeTrue();

        this.EntryModule = entryModuleType;
    }
}