using System;
using System.ComponentModel;

namespace XCloud.Platform.Shared.Constants;

public static class Roles
{
    /// <summary>
    /// 管理员
    /// </summary>
    public static int ManagerRole => (int)(MemberRoleEnum.管理员);

    /// <summary>
    /// 管理员或者普通成员
    /// </summary>
    public static int MemberRole => (int)(MemberRoleEnum.管理员 | MemberRoleEnum.普通成员);

    /// <summary>
    /// 所有
    /// </summary>
    public static int AnyRole => (int)(MemberRoleEnum.管理员 | MemberRoleEnum.普通成员 | MemberRoleEnum.观察者);
}

[Flags]
public enum MemberRoleEnum : int
{
    管理员 = 1 << 0,

    普通成员 = 1 << 3,

    [Description("观察者-只能看")] 观察者 = 1 << 4
}

public static class PlatformDbConnectionNames
{
    public static string Platform => "Platform";
}

public static class SubjectTypes
{
    public static string Role => "role";

    /// <summary>
    /// 系统管理员
    /// </summary>
    public static string Admin => "admin";

    /// <summary>
    /// 平台用户
    /// </summary>
    public static string User => "user";

    /// <summary>
    /// 租户成员
    /// </summary>
    public static string Member => "member";
}

public static class IdentityConsts
{
    public static class GrantType
    {
        /// <summary>
        /// 微信外部登陆grant type
        /// </summary>
        public static string UserWechat => "user_wechat";

        public static string AdminPassword => "admin_password";

        public static string InternalGrantType => "internal_grant_type";
    }

    public static class Scheme
    {
        public static string IdentityServerWebCookieScheme => "ids_web_cookie_scheme";

        /// <summary>
        /// 外部登陆域
        /// </summary>
        public static string ExternalLoginScheme => "external_login_scheme";

        /// <summary>
        /// 用户登陆
        /// </summary>
        public static string BearerTokenScheme => "bearer_token_scheme";

        public static string JwtTokenScheme => "jwt_token_scheme";
    }

    public static class Account
    {
        public static string DefaultUserName => "user";
        public static string DefaultAdminUserName => "admin";
        public static string DefaultSingleTenantId => "single-tenant-id";
    }
}