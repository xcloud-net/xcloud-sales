using Microsoft.AspNetCore.Mvc.Routing;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DynamicProxy;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Queue;
using XCloud.Sales.Services.Logging;

namespace XCloud.Sales.Framework;

/// <summary>
/// make sure that action@controller should be virtual method
/// otherwise the audit log interceptor won't work
/// </summary>
public class StoreAuditLogAttribute : Attribute { }

public class AuditLogInterceptor : IAbpInterceptor, ITransientDependency
{
    private readonly ILogger _logger;
    private readonly IEventBusService _eventBusService;
    private readonly IJsonDataSerializer _jsonDataSerializer;
    private readonly ISalesWorkContext _salesWorkContext;
    private readonly IActivityLogService _activityLogService;

    public AuditLogInterceptor(IEventBusService eventBusService,
        IJsonDataSerializer jsonDataSerializer,
        ISalesWorkContext salesWorkContext,
        ILogger<AuditLogInterceptor> logger,
        IActivityLogService activityLogService)
    {
        this._salesWorkContext = salesWorkContext;
        this._eventBusService = eventBusService;
        this._jsonDataSerializer = jsonDataSerializer;
        this._logger = logger;
        this._activityLogService = activityLogService;
    }

    private bool DropArgs(Type arg) => !(arg.IsAssignableTo_<IEntityDto>() || arg.IsAssignableTo_<IEntity>());

    private IDictionary<string, object> GetArgs(IAbpMethodInvocation invocation)
    {
        var dict = new Dictionary<string, object>();

        foreach (var m in invocation.ArgumentsDictionary)
        {
            var arg = m.Value;
            if (arg == null || this.DropArgs(arg.GetType()))
                continue;

            dict[m.Key] = m.Value;
        }

        return dict;
    }

    private async Task LogAuditLogAsync(IAbpMethodInvocation invocation)
    {
        try
        {
            var classAttributes = invocation.Method.DeclaringType.GetCustomAttributes_<StoreAuditLogAttribute>();
            var methodAttributes = invocation.Method.GetCustomAttributes_<StoreAuditLogAttribute>();
            var actionMethodAttributes = invocation.Method.GetCustomAttributes_<HttpMethodAttribute>();

            var intercept =
                invocation.Method.IsPublic &&
                actionMethodAttributes.Any() &&
                (classAttributes.Any() || methodAttributes.Any());

            if (!intercept)
            {
                this._logger.LogInformation($"ignore audit log for {invocation.Method.Name}");
                return;
            }

            var dict = this.GetArgs(invocation);
            var dataJson = this._jsonDataSerializer.SerializeToString(dict);

            var log = new ActivityLog()
            {
                UserId = this._salesWorkContext.AuthedStoreUser?.Data?.Id ?? 0,
                AdministratorId = this._salesWorkContext.AuthedStoreAdministrator?.Data?.AdministratorId,
                Comment = nameof(AuditLogInterceptor),
                ActivityLogTypeId = (int)ActivityLogType.AuditLog,
                Data = dataJson,
            };

            log = this._activityLogService.AttachHttpContextInfo(log);

            await this._eventBusService.NotifyInsertActivityLog(log);
        }
        catch (Exception e)
        {
            this._logger.LogError(exception: e, message: e.Message);
        }
    }

    public async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        try
        {
            await invocation.ProceedAsync();
        }
        finally
        {
            await this.LogAuditLogAsync(invocation);
        }
    }
}