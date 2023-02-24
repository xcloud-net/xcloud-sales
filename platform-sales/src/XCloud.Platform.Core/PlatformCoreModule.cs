global using System;
global using System.Linq;
global using XCloud.Core.IdGenerator;
using Volo.Abp.AutoMapper;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.FluentValidation;
using Volo.Abp.Modularity;
using XCloud.Application;
using XCloud.Job;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Core.Job;
using XCloud.Platform.Shared;

namespace XCloud.Platform.Core;

[DependsOn(
    typeof(BaseApplicationServiceModule),
    typeof(AbpBlobStoringFileSystemModule),
    typeof(AbpFluentValidationModule),
    typeof(AbpAutoMapperModule),
    typeof(PlatformSharedModule)
)]
public class PlatformCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        this.Configure<PlatformJobOption>(option =>
        {
            option.AutoStartJob = true;
        });
        this.Configure<PlatformDatabaseOption>(option => option.AutoCreateDatabase = false);
        this.Configure<AbpAutoMapperOptions>(option => option.AddMaps<PlatformCoreModule>(false));
    }
}