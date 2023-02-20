using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Logging;
using XCloud.Core.Http.Dynamic.Definition;

namespace XCloud.AspNetMvc.DynamicApi;

/// <summary>
/// 用于找到action controller，
/// 这里没有用来查找controller，只是删除了不需要的controller
/// </summary>
public class DynamicApiApplicationModelProvider : IApplicationModelProvider
{
    private readonly ILogger logger;
    public DynamicApiApplicationModelProvider(ILogger<DynamicApiApplicationModelProvider> logger)
    {
        this.logger = logger;
    }

    public int Order => default;

    public void OnProvidersExecuted(ApplicationModelProviderContext context)
    {
        var deletedController = new List<ControllerModel>();
        foreach (var controller in context.Result.Controllers)
        {
            if (!DynamicApiControllerHelper.IsDynamicApiController(controller.ControllerType))
            {
                continue;
            }

            var contract = ServiceDefinitionHelper.GetServiceContractInterfaceOrNull(controller.ControllerType);
            if (contract == null)
            {
                deletedController.Add(controller);
                continue;
            }

            var definition = new ServiceDefinition(contract);

            var deletedAction = new List<ActionModel>();
            foreach (var action in controller.Actions)
            {
                var isAction = definition.ActionDefinitions.Any(x => ServiceDefinitionHelper.IsImplementRelation(action.ActionMethod, x.ActionMethod));
                if (!isAction)
                {
                    deletedAction.Add(action);
                }
            }
            deletedAction.ForEach(x => controller.Actions.Remove(x));
            if (!controller.Actions.Any())
            {
                deletedController.Add(controller);
            }
        }
        deletedController.ForEach(x => context.Result.Controllers.Remove(x));
    }

    public void OnProvidersExecuting(ApplicationModelProviderContext context)
    {
        //
    }
}