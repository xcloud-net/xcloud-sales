using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Application;

namespace XCloud.Platform.Application.Common.Service.Token;

[ExposeServices(typeof(TempCodeService))]
public class TempCodeService : PlatformApplicationService, ITransientDependency
{
    public TempCodeService()
    {
        //
    }

    public async Task<string> CreateTempCode(IdDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var key = $"{GuidGenerator.CreateGuidString()}@{dto.Id}";
        await CacheProvider.SetAsync(key, dto, TimeSpan.FromMinutes(1));
        return key;
    }

    public async Task RemoveTempCode(string key)
    {
        await CacheProvider.RemoveAsync(key);
    }

    public async Task<IdDto> GetTempCode(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        
        var res = await CacheProvider.GetAsync<IdDto>(key);

        return res.Data;
    }
}