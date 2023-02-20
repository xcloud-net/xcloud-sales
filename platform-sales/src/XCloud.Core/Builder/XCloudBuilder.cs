using System.Reflection;
using Microsoft.Extensions.Configuration;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;

namespace XCloud.Core.Builder;

public interface IXCloudBuilder : IHasExtraProperties
{
    /// <summary>
    /// 入口module类型
    /// </summary>
    Type EntryModuleType { get; }

    /// <summary>
    /// 入口程序集
    /// </summary>
    Assembly EntryAssembly { get; }

    /// <summary>
    /// 所有程序集
    /// </summary>
    IReadOnlyCollection<Assembly> AllModuleAssemblies { get; }

    IConfiguration Configuration { get; }

    IServiceCollection Services { get; }
}

public sealed class XCloudBuilder<EntryModule> : ExtensibleObject, IXCloudBuilder
{
    public IConfiguration Configuration { get; }
    public IServiceCollection Services { get; }

    public IReadOnlyCollection<Assembly> AllModuleAssemblies { get; }

    public Assembly EntryAssembly { get; }

    public Type EntryModuleType { get; }

    public XCloudBuilder(IServiceCollection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));
        
        this.Services = collection;
        this.Configuration = this.Services.GetConfiguration();

        //入口程序集
        this.EntryModuleType = typeof(EntryModule);

        this.EntryAssembly = this.EntryModuleType.Assembly;
        this.AllModuleAssemblies = this.FindDependedAssembliesDeepFirst(this.EntryModuleType);
        
        if (this.EntryAssembly == null)
            throw new AbpException(nameof(this.EntryAssembly));
    }

    private Assembly[] FindDependedAssembliesDeepFirst(Type entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        var findedModuleType = new List<Type>();

        var findedAssembly = new List<Assembly>();

        void FindAssembly(Type t)
        {
            //检查有没有历遍过
            if (findedModuleType.Contains(t))
            {
                return;
            }
            findedModuleType.Add(t);

            //深度优先
            var dependOn = t.GetCustomAttributes<DependsOnAttribute>().ToArray();
            if (dependOn.Any())
            {
                dependOn.SelectMany(x => x.DependedTypes).ToList().ForEach(FindAssembly);
            }

            //保证被依赖的排在前面
            if (!findedAssembly.Contains(t.Assembly))
            {
                findedAssembly.Add(t.Assembly);
            }
        }

        FindAssembly(entry);

        var res = findedAssembly.WhereNotNull().ToArray();
        return res;
    }
}