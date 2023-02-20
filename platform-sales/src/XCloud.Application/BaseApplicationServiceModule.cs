using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using XCloud.AspNetMvc;
using XCloud.Database.EntityFrameworkCore;
using Masuit.Tools.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Dapper;
using Volo.Abp.Validation;
using XCloud.Application.Validation;

namespace XCloud.Application;

[DependsOn(
    typeof(EfCoreModule),
    typeof(AspNetMvcModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpValidationModule),
    typeof(AbpDapperModule)
)]
public class BaseApplicationServiceModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        this.Configure<AbpUnitOfWorkDefaultOptions>(option =>
        {
            option.TransactionBehavior = UnitOfWorkTransactionBehavior.Enabled;
        });
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        //context.Services.RemoveAll<IObjectValidator>();
        //context.Services.AddTransient<IObjectValidator, AbpFluentValidationProvider>();
        context.Services.RemoveAll<IMethodInvocationValidator>();
        context.Services.AddTransient<IMethodInvocationValidator, NullMethodInvocationValidator>();

        context.Services.GetConfiguration().AddToMasuitTools();
    }
}