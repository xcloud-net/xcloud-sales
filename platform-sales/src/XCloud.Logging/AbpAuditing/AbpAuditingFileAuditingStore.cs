using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using XCloud.Core.DataSerializer;

namespace XCloud.Logging.AbpAuditing;

public class AbpAuditingFileAuditingStore : IAuditingStore
{
    private readonly IServiceProvider serviceProvider;
    public AbpAuditingFileAuditingStore(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task SaveAsync(AuditLogInfo auditInfo)
    {
        if (auditInfo == null)
        {
            return;
        }

        await Task.CompletedTask;

        var json = this.serviceProvider.GetRequiredService<IJsonDataSerializer>().SerializeToString(auditInfo);

        this.serviceProvider.GetRequiredService<AbpAuditingFileLogger>().Logger.LogInformation(json);
    }
}