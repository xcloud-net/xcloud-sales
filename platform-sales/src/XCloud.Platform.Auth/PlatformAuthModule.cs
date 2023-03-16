using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Authorization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

using XCloud.AspNetMvc;
using XCloud.Platform.Auth.Configuration;
using XCloud.Platform.Application.Member;
using XCloud.Platform.Shared.Constants;

namespace XCloud.Platform.Auth;

[DependsOn(
    typeof(AspNetMvcModule),
    typeof(AbpAuthorizationModule),
    typeof(PlatformMemberModule)
)]
public class PlatformAuthModule : AbpModule
{
    private void ConfigureMultiTenancy(ServiceConfigurationContext context)
    {
        this.Configure<AbpMultiTenancyOptions>(option => option.IsEnabled = false);
        this.Configure<AbpTenantResolveOptions>(option =>
        {
            option.TenantResolvers.Clear();
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        this.ConfigureMultiTenancy(context);

        this.Configure<AbpAutoMapperOptions>(option => option.AddMaps<PlatformAuthModule>(false));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        //add user authentication support
        //warning: add after identity server
        //or identity server will change default scheme
        context.Services.AddUserAuthentication();
        context.Services.AddAuthorizationHandlers();

        context.Services.Configure<AuthenticationOptions>(option =>
        {
            //set default auth scheme
            option.DefaultScheme = AuthConstants.Scheme.BearerTokenScheme;
        });
    }
}