using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using XCloud.Core.Application;
using XCloud.Core.Cache;
using XCloud.Core.Extension;
using XCloud.Core.Json;
using XCloud.Platform.Core.Domain.Settings;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Common.Application.Service.Settings;

public interface ISysSettingsService : IApplicationService
{
    Task<bool> SettingsExistAsync(string name);

    Task SaveSettingsAsync(string name, string value);
    Task RemoveByNameAsync(string name);

    Task<string> GetSettingValueByNameAsync(string name);
    Task<string> GetSettingValueByNameAsync(string name, CachePolicy option);

    Task SaveObjectAsync<T>(string name, T obj) where T : class;
    Task<T> GetObjectOrDefaultAsync<T>(string name) where T : class;
}

public class SysSettingsService : XCloudApplicationService, ISysSettingsService
{
    private readonly IPlatformRepository<SysSettings> _settingRepository;

    public SysSettingsService(IPlatformRepository<SysSettings> settingRepository)
    {
        this._settingRepository = settingRepository;
    }

    private string NormalizeName(string name)
    {
        return name.Trim().RemoveWhitespace();
    }

    public async Task<string> GetSettingValueByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        name = this.NormalizeName(name);

        var entity = await this._settingRepository.QueryOneAsync(x => x.Name == name);
        if (entity == null)
            return null;

        return entity.Value;
    }

    public async Task<string> GetSettingValueByNameAsync(string name, CachePolicy option)
    {
        var key = $"sys.settings.value.by.name={name}";

        var value = await this.CacheProvider.ExecuteWithPolicyAsync(
            () => this.GetSettingValueByNameAsync(name),
            new CacheOption<string>(key, TimeSpan.FromMinutes(3)),
            option);

        return value;
    }

    public async Task<bool> SettingsExistAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        name = this.NormalizeName(name);

        var exist = await this._settingRepository.AnyAsync(x => x.Name == name);
        return exist;
    }

    public async Task SaveSettingsAsync(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        name = this.NormalizeName(name);

        await this._settingRepository.DeleteNowAsync(x => x.Name == name);

        var entity = new SysSettings
        {
            Name = name,
            Value = value,
            Id = this.GuidGenerator.CreateGuidString(),
            CreationTime = this.Clock.Now
        };

        await this._settingRepository.InsertNowAsync(entity);

        await this.GetSettingValueByNameAsync(entity.Name, new CachePolicy() { Refresh = true });
    }

    public async Task RemoveByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        name = this.NormalizeName(name);

        await this._settingRepository.DeleteAsync(x => x.Name == name, autoSave: true);

        await this.GetSettingValueByNameAsync(name, new CachePolicy() { RemoveCache = true });
    }

    public async Task SaveObjectAsync<T>(string name, T obj) where T : class
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var json = this.JsonDataSerializer.SerializeToString(obj);

        await this.SaveSettingsAsync(name, json);
    }

    public async Task<T> GetObjectOrDefaultAsync<T>(string name) where T : class
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        var json = await this.GetSettingValueByNameAsync(name,
            new CachePolicy() { Cache = true });

        if (!string.IsNullOrWhiteSpace(json))
        {
            var obj = this.JsonDataSerializer.DeserializeFromStringOrDefault<T>(json, this.Logger);
            if (obj != null)
                return obj;
        }

        return default;
    }
}