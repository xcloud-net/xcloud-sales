using System.Threading.Tasks;
using XCloud.Application.Service;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Application.Member.Service.User;

public interface IWxUnionService : IXCloudApplicationService
{
    Task SaveOpenIdUnionIdMappingAsync(string platform, string appId, string openId, string unionIdOrEmpty);
}

public class WxUnionService : XCloudApplicationService, IWxUnionService
{
    private readonly IMemberRepository<SysWxUnion> _repository;

    public WxUnionService(IMemberRepository<SysWxUnion> repository)
    {
        _repository = repository;
    }

    public async Task SaveOpenIdUnionIdMappingAsync(string platform, string appId, string openId, string unionIdOrEmpty)
    {
        await this._repository.DeleteAsync(x => x.Platform == platform && x.AppId == appId && x.OpenId == openId);

        if (!string.IsNullOrWhiteSpace(unionIdOrEmpty))
        {
            var entity = new SysWxUnion()
            {
                Platform = platform,
                AppId = appId,
                OpenId = openId,
                UnionId = unionIdOrEmpty,
                Id = this.GuidGenerator.CreateGuidString(),
                CreationTime = this.Clock.Now
            };
            await this._repository.InsertAsync(entity);
        }
    }
}