using System.Threading.Tasks;
using FluentAssertions;
using XCloud.Core.Application;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Region;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Common.Application.Service.Region;

public interface IRegionService : IXCloudApplicationService
{
    Task<SysRegion[]> QueryByParentIdAsync(string parentId);

    [Obsolete("may cause memory overflow exception")]
    Task<SysRegion[]> QueryAllAsync();
}

public class RegionService : PlatformApplicationService, IRegionService
{
    private readonly IPlatformRepository<SysRegion> _regionRepository;
    public RegionService(IPlatformRepository<SysRegion> regionRepository)
    {
        this._regionRepository = regionRepository;
    }

    public async Task<SysRegion[]> QueryAllAsync()
    {
        var maxTake = 20000;

        var query = await this._regionRepository.GetQueryableAsync();

        query = query.AsNoTracking();

        var data = await query.OrderByDescending(x => x.Sort).Take(maxTake).ToArrayAsync();

        (data.Length < maxTake).Should().BeTrue();

        return data;
    }

    public async Task<SysRegion[]> QueryByParentIdAsync(string parentId)
    {
        var query = await this._regionRepository.GetQueryableAsync();

        query = query.AsNoTracking();

        if (string.IsNullOrWhiteSpace(parentId))
        {
            query = query.Where(x => x.ParentId == string.Empty || x.ParentId == null);
        }
        else
        {
            query = query.Where(x => x.ParentId == parentId);
        }

        var data = await query.OrderByDescending(x => x.Sort).TakeUpTo5000().ToArrayAsync();

        return data;
    }
}