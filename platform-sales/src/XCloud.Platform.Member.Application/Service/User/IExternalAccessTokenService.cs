using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Application.Service;
using XCloud.Core.Application;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Member.Application.Service.User;

public enum AccessTokenTypeEnum : int
{
    Client = 1,
    User = 2
}

public interface IExternalAccessTokenService : IXCloudApplicationService
{
    Task CleanExpiredTokenAsync();

    Task InsertAccessTokenAsync(SysUserExternalAccessTokenDto dto);

    Task<SysExternalAccessToken> GetValidClientAccessTokenAsync(GetValidClientAccessTokenInput dto);

    Task<SysExternalAccessToken> GetValidUserAccessTokenAsync(GetValidUserAccessTokenInput dto);
}

public class ExternalAccessTokenService : PlatformApplicationService, IExternalAccessTokenService
{
    private readonly IMemberRepository<SysExternalAccessToken> _accessTokenRepository;

    public ExternalAccessTokenService(IMemberRepository<SysExternalAccessToken> accessTokenRepository)
    {
        this._accessTokenRepository = accessTokenRepository;
    }

    public async Task InsertAccessTokenAsync(SysUserExternalAccessTokenDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var entity = this.ObjectMapper.Map<SysUserExternalAccessTokenDto, SysExternalAccessToken>(dto);

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;

        await this._accessTokenRepository.InsertNowAsync(entity);
    }

    public async Task<SysExternalAccessToken> GetValidUserAccessTokenAsync(GetValidUserAccessTokenInput dto)
    {
        var db = await this._accessTokenRepository.GetDbContextAsync();

        var query = db.Set<SysExternalAccessToken>().AsNoTracking();

        var tokenType = (int)AccessTokenTypeEnum.User;
        query = query.Where(x => x.AccessTokenType == tokenType);
        query = query.Where(x => x.Platform == dto.Platform && x.AppId == dto.AppId && x.UserId == dto.UserId);

        var now = this.Clock.Now;

        query = query.Where(x => x.ExpiredAt > now);

        var token = await query.OrderByDescending(x => x.CreationTime).FirstOrDefaultAsync();

        return token;
    }

    public async Task<SysExternalAccessToken> GetValidClientAccessTokenAsync(GetValidClientAccessTokenInput dto)
    {
        var db = await this._accessTokenRepository.GetDbContextAsync();

        var query = db.Set<SysExternalAccessToken>().AsNoTracking();

        var tokenType = (int)AccessTokenTypeEnum.Client;
        query = query.Where(x => x.AccessTokenType == tokenType);
        query = query.Where(x => x.Platform == dto.Platform && x.AppId == dto.AppId);

        var now = this.Clock.Now;

        query = query.Where(x => x.ExpiredAt > now);

        var token = await query.OrderByDescending(x => x.CreationTime).FirstOrDefaultAsync();

        return token;
    }

    public async Task CleanExpiredTokenAsync()
    {
        while (true)
        {
            var uow = this.UnitOfWorkManager.Begin(requiresNew: true);
            try
            {
                var db = await this._accessTokenRepository.GetDbContextAsync();

                var set = db.Set<SysExternalAccessToken>();

                var now = this.Clock.Now;

                var list = await set.Where(x => x.ExpiredAt <= now).OrderBy(x => x.CreationTime).Take(100)
                    .ToArrayAsync();
                if (!list.Any())
                    break;

                set.RemoveRange(list);

                await db.SaveChangesAsync();
                await uow.CompleteAsync();
            }
            catch (Exception e)
            {
                await uow.RollbackAsync();
                this.Logger.LogError(message: e.Message, exception: e);
                break;
            }
        }
    }
}