# 为 identity server 提供实现接口

1. 集成 membership
2. 实现 password grant type
3. 扩展微信授权登录的 grant type
4. 自定义 consent 界面
5. membership 角色权限管理
6. 菜单管理


# wechat grant type

```csharp
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using XCloud.Core;
using XCloud.Core.Ddd.Dtos;
using XCloud.Core.Extension;
using XCloud.Platform.Member.Application;
using XCloud.Platform.Member.Application.User;
using XCloud.Platform.Member.Application.User.Entity;
using XCloud.Platform.Member.Auth.Authentication;
using XCloud.Platform.Member.Auth.User;
using XCloud.Platform.Shared;
using XCloud.Platform.Shared.Dtos;
using XCloud.WeChat;

namespace XCloud.Identity.Providers.GrantProvider
{
    /// <summary>
    /// 使用微信小程序授权登录
    /// </summary>
    public class UserWechatLoginValidator : IExtensionGrantValidator
    {
        private readonly IWorkContext workContext;
        private readonly IUserAccountService userAccountService;
        private readonly IUserProfileService userProfileService;
        private readonly IUserExternalAccountService userExternalAccountService;
        private readonly IMemberShipMessageBus memberShipMessageBus;

        private readonly IWxLoginService userWxLoginService;

        public UserWechatLoginValidator(IWorkContext<UserWechatLoginValidator> xCloudContext,
            IMemberShipMessageBus memberShipMessageBus,
            IWxLoginService userWxLoginService,
            IUserProfileService userProfileService,
            IUserAccountService login,
            IUserExternalAccountService userExternalAccountService)
        {
            workContext = xCloudContext;
            this.userProfileService = userProfileService;
            this.userWxLoginService = userWxLoginService;
            this.memberShipMessageBus = memberShipMessageBus;

            userAccountService = login;
            this.userExternalAccountService = userExternalAccountService;
        }

        public string GrantType => IdentityConsts.GrantType.UserWechat;

        /// <summary>
        /// code换token
        /// token换openid
        /// openid查找关联用户
        /// 如果没有用户就创建并关联
        /// 用此用户的userid作为subject颁发token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var list = new List<string>();
            try
            {
                list.Add(workContext.JsonSerializer.SerializeToString(context.Request.Raw.ToDict()));

                var code = context.Request.Raw["code"];
                var nick_name = context.Request.Raw["nick_name"];
                var avatar_url = context.Request.Raw["avatar_url"];

                var openid_response = await userWxLoginService.GetAccessToken(code);

                var user = await __get_user_or_create__(openid_response.openid, nick_name, avatar_url);

                list.Add(workContext.JsonSerializer.SerializeToString(user));

                var subject = user.Id;

                var identity = new ClaimsIdentity()
                    .SetSubjectID(user.Id)
                    .AsUser()
                    .SetCreationTime(DateTime.UtcNow);

                //这个返回还没用到
                var response = new Dictionary<string, object>()
                {
                    ["user_name"] = user.IdentityName
                };

                context.Result = new GrantValidationResult(
                 subject: subject,
                 claims: identity.Claims,
                 authenticationMethod: "custom",
                 customResponse: response);
            }
            catch (BusinessException e)
            {
                list.Add(e.Message);

                var msg = e.Message ?? nameof(UserWechatLoginValidator);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, msg);
            }
            catch (Exception e)
            {
                list.Add(workContext.JsonSerializer.SerializeToString(e.ExtractExceptionDescriptor()));

                workContext.Logger.AddErrorLog($"{e.Message}", e);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "服务器发生错误");
            }
            finally
            {
                var log = string.Join("=>", list);
                workContext.Logger.LogInformation(message: log);
            }
        }

        async Task<UserEntity> __get_user_or_create__(string openid, string nick_name, string avatar_url)
        {
            var map = await userExternalAccountService.FindExternalLoginByOpenID(userWxLoginService.LoginProvider, openid);

            if (map == null)
            {
                var user = new UserEntity()
                {
                    IdentityName = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    NickName = nick_name,
                    Avatar = avatar_url
                };
                var res = await userAccountService.AddAccount(new IdentityNameDto(user.IdentityName));
                res.ThrowIfNotSuccess();
                await userProfileService.UpdateNickName(res.Data.Id, user.NickName);
                await userProfileService.UpdateUserAvatar(res.Data.Id, user.Avatar);

                await this.memberShipMessageBus.CopyAvatar(new CopyAvatarMessage() { UserId = res.Data.Id, AvatarUrl = avatar_url });

                await userExternalAccountService.SaveExternalProviderMapping(userWxLoginService.LoginProvider, openid, res.Data.Id);

                return user;
            }
            else
            {
                var user = await userAccountService.GetUserDtoById(new IdDto(map.UserId));
                if (user != null)
                {
                    await userExternalAccountService.RemoveExternalLogin(map.UserId, new[] { userWxLoginService.LoginProvider });
                    throw new UserFriendlyException("此微信绑定的用户被禁用，现已解除绑定");
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

```