using Volo.Abp.BlobStoring;

namespace XCloud.Platform.Common.Application.Service.Storage.QCloud;

public class TencentCloudBlobProviderConfiguration
{
    public static class ConfigurationNames
    {
        public static string ContainerName => "TencentCloud.ContainerName";

        public static string AppId => "TencntCloud.AppId";
        public static string SecretId => "TencentCloud.SecretId";
        public static string SecretKey => "TencentCloud.SecretKey";

        public static string Region => "TencentCloud.Region";
        public static string ConnectionTimeout => "TencentCloud.ConnectionTimeout";
        public static string ReadWriteTimeout => "TencentCloud.ReadWriteTimeout";
        public static string KeyDurationSecond => "TencentCloud.KeyDurationSecond";
    }

    public string AppId
    {
        get => _containerConfiguration.GetConfigurationOrDefault<string>(ConfigurationNames.AppId);
        set => _containerConfiguration.SetConfiguration(ConfigurationNames.AppId, value);
    }

    public string SecretId
    {
        get => _containerConfiguration.GetConfigurationOrDefault<string>(ConfigurationNames.SecretId);
        set => _containerConfiguration.SetConfiguration(ConfigurationNames.SecretId, value);
    }

    public string SecretKey
    {
        get => _containerConfiguration.GetConfigurationOrDefault<string>(ConfigurationNames.SecretKey);
        set => _containerConfiguration.SetConfiguration(ConfigurationNames.SecretKey, value);
    }

    public string Region
    {
        get => _containerConfiguration.GetConfigurationOrDefault<string>(ConfigurationNames.Region);
        set => _containerConfiguration.SetConfiguration(ConfigurationNames.Region, value);
    }


    public int ConnectionTimeout
    {
        get => _containerConfiguration.GetConfigurationOrDefault<int>(ConfigurationNames.ConnectionTimeout, defaultValue: 3);
        set => _containerConfiguration.SetConfiguration(ConfigurationNames.ConnectionTimeout, value);
    }

    public int ReadWriteTimeout
    {
        get => _containerConfiguration.GetConfigurationOrDefault<int>(ConfigurationNames.ReadWriteTimeout, defaultValue: 5);
        set => _containerConfiguration.SetConfiguration(ConfigurationNames.ReadWriteTimeout, value);
    }

    public int KeyDurationSecond
    {
        get => _containerConfiguration.GetConfigurationOrDefault<int>(ConfigurationNames.KeyDurationSecond, defaultValue: 600);
        set => _containerConfiguration.SetConfiguration(ConfigurationNames.KeyDurationSecond, value);
    }

    public string ContainerName
    {
        get => _containerConfiguration.GetConfigurationOrDefault<string>(ConfigurationNames.ContainerName);
        set => _containerConfiguration.SetConfiguration(ConfigurationNames.ContainerName, value);
    }

    private readonly BlobContainerConfiguration _containerConfiguration;

    public TencentCloudBlobProviderConfiguration(BlobContainerConfiguration containerConfiguration)
    {
        _containerConfiguration = containerConfiguration;
    }
}