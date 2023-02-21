using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using XCloud.Platform.Shared.Localization;
using XCloud.Sales.Localization;

namespace XCloud.Sales.Configuration;

public static class LocalizationExtension
{
    public static void ConfigSalesLocalization(this ServiceConfigurationContext context)
    {
        context.Services.Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SalesModule>(baseNamespace: typeof(SalesModule).Namespace);
        });

        context.Services.Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Clear();
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));

            options.Resources
                .Add<SalesResource>(defaultCultureName: "zh-Hans")
                .AddVirtualJson("/Localization/Resources/Sales")
                .AddBaseTypes(typeof(PlatformResource), typeof(AbpValidationResource));

            options.DefaultResourceType = typeof(SalesResource);
        });
    }
}