using System;
using FluentAssertions;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Application;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Core.Security.Hash;

namespace XCloud.Platform.Shared.Helper;

/// <summary>
/// 把不同格式的电话号码变成统一格式
/// 方便数据库比对查找
/// </summary>
public class MemberHelper : IScopedDependency
{
    private readonly AppConfig _appConfig;
    public MemberHelper(AppConfig appConfig)
    {
        this._appConfig = appConfig;
    }

    public string EncryptPassword(string raw, string salt = null)
    {
        raw.Should().NotBeNullOrEmpty();
        salt ??= string.Empty;

        var data = MD5.Encrypt($"{raw}.{salt}", _appConfig.Encoding);

        data = data.Replace("-", string.Empty).Trim().ToUpper();

        return data;
    }

    public string NormalizeMobilePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentNullException($"{nameof(NormalizeMobilePhone)},phone:{phone}");

        phone = phone.Replace("-", string.Empty).RemoveWhitespace();
        phone = phone.RemovePreFix("+86");

        return phone;
    }

    public void NormalizeIdentityName(IHasIdentityNameFields entity, string originIdentityName)
    {
        originIdentityName.Should().NotBeNullOrEmpty();
        var normalizedIdentityName = NormalizeIdentityName(originIdentityName);
        entity.IdentityName = normalizedIdentityName;
        entity.OriginIdentityName = originIdentityName;
    }

    public string NormalizeIdentityName(string identityName)
    {
        var normalized = identityName.RemoveWhitespace().ToLower();
        return normalized;
    }

    public bool ValidateIdentityName(string name, out string msg)
    {
        msg = string.Empty;

        name.Should().NotBeNullOrEmpty();

        var chars = Com.Range('a', 'z').Concat(Com.Range('A', 'Z')).Select(x => (char)x).ToList();
        chars.AddRange(Com.Range(0, 10).Select(x => x.ToString()[0]));
        chars.AddRange(new[] { '_', '-', '@' });

        if (!name.All(x => chars.Contains(x)))
        {
            msg = "用户名包含非法字符";
            return false;
        }
        return true;
    }
}