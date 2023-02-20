using COSXML;
using COSXML.Auth;

namespace XCloud.Platform.Common.Application.Service.Storage.QCloud;

public class CosServerWrapObject
{
    public CosXmlServer CosXmlServer { get; protected set; }

    public CosServerWrapObject(TencentCloudBlobProviderConfiguration configuration)
    {
        var cosConfig = new CosXmlConfig.Builder()
            .SetConnectionLimit(TimeSpan.FromSeconds(configuration.ConnectionTimeout).TotalMilliseconds.To<int>())
            .SetReadWriteTimeoutMs(TimeSpan.FromSeconds(configuration.ReadWriteTimeout).TotalMilliseconds.To<int>())
            .IsHttps(true)
            .SetAppid(configuration.AppId)
            .SetRegion(configuration.Region)
            .SetDebugLog(false)
            .Build();

        var credentialProvider = new DefaultQCloudCredentialProvider(
            configuration.SecretId,
            configuration.SecretKey,
            configuration.KeyDurationSecond);

        CosXmlServer = new CosXmlServer(cosConfig, credentialProvider);
    }
}