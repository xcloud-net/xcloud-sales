using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Dtos;

namespace XCloud.Core.Cache;

public class CachePolicy : IEntityDto
{
    public bool? Cache { get; set; }
    public bool? CacheOnly { get; set; }
    public bool? Refresh { get; set; }
    public bool? Source { get; set; }
    public bool? RemoveCache { get; set; }
}

public class CacheOption<T> : CacheOption
{
    public Func<T, bool> CacheCondition { get; set; }

    public CacheOption(string key, TimeSpan expiration) : base(key, expiration)
    {
        //
    }

    public CacheOption(string key) : base(key)
    {
        //
    }

    public CacheOption()
    {
        //
    }
}

public class CacheOption : IEntityDto
{
    public string Key { get; set; }
    public TimeSpan Expiration { get; set; }

    /// <summary>
    /// 当缓存出现异常，则直接溯源
    /// </summary>
    public bool IgnoreCacheException { get; set; } = false;

    public bool RemoveCacheKeyWhenSerializeException { get; set; } = true;

    public CacheOption()
    {
        //
    }

    public CacheOption(string key) : this(key, TimeSpan.FromMinutes(15))
    {
        //
    }

    public CacheOption(string key, TimeSpan expiration) : this("cache", key, expiration)
    {
        //
    }

    public CacheOption(string prefix, string key, TimeSpan expiration) : this()
    {
        this.Key = $"{prefix}:{key}";
        this.Expiration = expiration;
    }

    public static implicit operator string(CacheOption option)
    {
        return option.Key;
    }

    public static implicit operator TimeSpan(CacheOption option)
    {
        return option.Expiration;
    }
}

public class CacheProviderOption : IEntityDto
{
    public CacheProviderOption(IConfiguration configuration)
    {
        //
    }
}