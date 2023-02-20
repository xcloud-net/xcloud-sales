using Volo.Abp.Authorization;

namespace XCloud.Platform.Shared.Exceptions;

/// <summary>
/// 没有登录
/// </summary>
public class NoLoginException : AbpAuthorizationException
{
    public NoLoginException(string msg = null) : base(msg ?? "没有登录")
    {
        //
    }
}

/// <summary>
/// 没有权限
/// </summary>
public class NoPermissionException : AbpAuthorizationException
{
    public NoPermissionException(string msg = null) : base(msg ?? "没有权限")
    {
        //
    }
}

public class NotActiveException : AbpAuthorizationException
{
    public NotActiveException() { }
    public NotActiveException(string message) : base(message) { }
}