using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using XCloud.Platform.Shared.Localization;

namespace XCloud.Platform.Shared.Configuration;

public static class LocalizationExtension
{
    public static void ConfigPlatformLocalization(this ServiceConfigurationContext context)
    {
        context.Services.Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PlatformSharedModule>(
                baseNamespace: typeof(PlatformSharedModule).Namespace);
        });
        context.Services.Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));

            options.Resources
                .Add<PlatformResource>(defaultCultureName: "zh-Hans")
                .AddVirtualJson("/Localization/Resources/Platform")
                .AddBaseTypes(typeof(AbpValidationResource));

            options.DefaultResourceType = typeof(PlatformResource);
        });
    }
}