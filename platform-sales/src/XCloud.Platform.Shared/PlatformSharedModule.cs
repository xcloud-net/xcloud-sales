global using System.Linq;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using XCloud.Core;
using XCloud.Platform.Shared.Localization;

namespace XCloud.Platform.Shared;

[DependsOn(
    typeof(CoreModule)
)]
public class PlatformSharedModule : AbpModule
{
    void ConfigLanguage()
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PlatformSharedModule>(baseNamespace: "XCloud.Platform.Shared");
        });
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));

            options.Resources
                .Add<PlatformResource>(defaultCultureName: "zh-Hans")
                .AddVirtualJson("/Localization/Platform");
            //.AddBaseTypes(typeof(AbpValidationResource));
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //language
        ConfigLanguage();
    }
}