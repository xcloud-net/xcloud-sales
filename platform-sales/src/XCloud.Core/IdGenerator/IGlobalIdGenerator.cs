using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace XCloud.Core.IdGenerator;

public interface IGlobalIdGenerator
{
    Task<string> NewIdAsync(string category = null);
}

[ExposeServices(typeof(IGlobalIdGenerator))]
public class AbpGuidGenerator : IGlobalIdGenerator, ITransientDependency
{
    private readonly IGuidGenerator guidGenerator;
    public AbpGuidGenerator(IGuidGenerator guidGenerator)
    {
        this.guidGenerator = guidGenerator;
    }

    public Task<string> NewIdAsync(string category)
    {
        var guid = this.guidGenerator.CreateGuidString();
        return Task.FromResult(guid);
    }
}