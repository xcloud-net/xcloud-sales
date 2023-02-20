using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using FluentAssertions;
using XCloud.Core.Builder;
using XCloud.Core.DependencyInjection;
using XCloud.Core.Dto;

namespace XCloud.Core.Development.Builtin;

public class ModuleDependencyDevInfoProvider : IDevelopmentInformationProvider, IAutoRegistered
{
    private readonly int maxDeep = 5;

    private readonly IXCloudBuilder builder;
    public ModuleDependencyDevInfoProvider(IXCloudBuilder builder)
    {
        this.builder = builder;
    }

    public string ProviderName => $"模块依赖关系,一共显示{this.maxDeep}层";

    IEnumerable<TreeNode> FindDependedAssemblies(Type entry, int maxDeep)
    {
        entry.Should().NotBeNull();

        IEnumerable<TreeNode> __find__(Type t, int parentDeep)
        {
            //检查有没有历遍过
            var currentDeep = parentDeep + 1;
            if (currentDeep <= maxDeep)
            {
                var node = new TreeNode()
                {
                    Name = t.Assembly.GetName()?.Name,
                };

                var depend_on = t.GetCustomAttributes<DependsOnAttribute>();
                if (depend_on.Any())
                {
                    var children = depend_on
                        .SelectMany(x => x.DependedTypes)
                        .SelectMany(x => __find__(x, currentDeep))
                        .ToArray();

                    node.Children = children;
                }

                yield return node;
            }
        }

        return __find__(entry, 0);
    }

    public async Task<object> Information()
    {
        await Task.CompletedTask;
        return FindDependedAssemblies(this.builder.EntryModuleType, maxDeep: this.maxDeep);
    }
}