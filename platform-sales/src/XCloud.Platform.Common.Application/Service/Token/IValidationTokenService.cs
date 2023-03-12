using System.Threading.Tasks;
using FluentAssertions;
using Volo.Abp.Domain.Repositories;
using XCloud.Application.Service;
using XCloud.Core.Application;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Token;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Common.Application.Service.Token;

public interface IValidationTokenService : IXCloudApplicationService
{
    Task AddValidationCodeAsync(string group, string key, string code, TimeSpan? timeout = null);

    Task<ValidationCode> GetValidationCodeAsync(string group, string key);

    Task<ApiResponse<object>> AddTokenWhenNotExistAsync(TokenDto dto);
}

public class ValidationTokenService : PlatformApplicationService, IValidationTokenService
{
    private readonly IPlatformRepository<ValidationToken> _validationTokenRepository;
    private readonly ICacheProvider _cacheProvider;

    public ValidationTokenService(
        ICacheProvider cacheProvider,
        IPlatformRepository<ValidationToken> validationTokenRepository)
    {
        this._cacheProvider = cacheProvider;
        this._validationTokenRepository = validationTokenRepository;
    }

    string ValidationCodeKey(string group, string key) => $"validation_code:{group}:{key}";

    public async Task AddValidationCodeAsync(string group, string key, string code, TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(group) || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(AddValidationCodeAsync));

        var model = new ValidationCode()
        {
            Code = code,
            CreateTime = DateTime.UtcNow
        };

        var finalKey = this.ValidationCodeKey(group, key);
        var finalTimeout = timeout ?? TimeSpan.FromMinutes(5);

        await this._cacheProvider.SetAsync(finalKey, model, finalTimeout);
    }

    public async Task<ValidationCode> GetValidationCodeAsync(string group, string key)
    {
        if (string.IsNullOrWhiteSpace(group) || string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(AddValidationCodeAsync));

        var finalKey = this.ValidationCodeKey(group, key);

        var res = await this._cacheProvider.GetAsync<ValidationCode>(finalKey);

        return res.Data;
    }

    public async Task<ApiResponse<object>> AddTokenWhenNotExistAsync(TokenDto dto)
    {
        dto.Should().NotBeNull();
        dto.Token.Should().NotBeNullOrEmpty();

        if (await _validationTokenRepository.AnyAsync(x => x.Token == dto.Token))
        {
            return new ApiResponse<object>().SetError("token已经存在");
        }

        var entity = new ValidationToken() { Token = dto.Token };

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;

        await _validationTokenRepository.InsertNowAsync(entity);

        return new ApiResponse<object>();
    }
}