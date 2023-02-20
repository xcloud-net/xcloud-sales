using System.Reflection;
using Volo.Abp.Modularity;
using FluentAssertions;

namespace XCloud.Core.Builder;

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