using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using XCloud.Core.Http.Dynamic.Definition;

namespace XCloud.AspNetMvc.DynamicApi;

/// <summary>
/// 用于修改动态api application model的配置参数
/// 比如http method，路由绑定，参数绑定
/// </summary>
public class DynamicApiApplicationModelConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            if (DynamicApiControllerHelper.IsDynamicApiController(controller.ControllerType))
            {
                ConfigureApplicationService(controller);
            }
        }
    }

    void ConfigureApplicationService(ControllerModel controller)
    {
        var contract = ServiceDefinitionHelper.GetServiceContractInterfaceOrNull(controller.ControllerType);
        if (contract == null)
        {
            controller.ApiExplorer.IsVisible = false;
            return;
        }
        var definition = new ServiceDefinition(contract);

        controller.ApiExplorer.IsVisible ??= true;
        controller.Selectors.Clear();

        var controllerSelector = new SelectorModel()
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(definition.RouteOrEmpty()))
        };
        controller.Selectors.Add(controllerSelector);

        foreach (var action in controller.Actions)
        {
            this.ConfigureApplicationAction(action, definition);
        }
    }

    void ConfigureApplicationAction(ActionModel action, ServiceDefinition definition)
    {
        var actionContract = definition.ActionDefinitions.FirstOrDefault(x => ServiceDefinitionHelper.IsImplementRelation(action.ActionMethod, x.ActionMethod));
        if (actionContract == null)
        {
            action.ApiExplorer.IsVisible = false;
            return;
        }
        action.ApiExplorer.IsVisible ??= true;

        action.Selectors.Clear();
        var actionSelector = new SelectorModel()
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(actionContract.RouteOrEmpty()))
        };
        var httpMethod = actionContract.ActionHttpMethod();
        httpMethod.Should().NotBeNullOrEmpty();
        actionSelector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { httpMethod }));
        action.Selectors.Add(actionSelector);

        foreach (var parameter in action.Parameters)
        {
            this.ConfigureParameters(parameter, action, definition, actionContract);
        }
    }

    void ConfigureParameters(ParameterModel parameter,
        ActionModel action,
        ServiceDefinition definition,
        ActionDefinition serviceAction)
    {
        if (parameter.BindingInfo != null)
        {
            return;
        }

        var parameterContract = serviceAction.ParameterDefinitions.FirstOrDefault(x => ServiceDefinitionHelper.IsParameterSame(x.ParameterInfo, parameter.ParameterInfo));
        if (parameterContract != null)
        {
            parameter.BindingInfo = new BindingInfo()
            {
                BindingSource = parameterContract.BindingSource,
                BinderType = parameterContract.BinderTypeProviderMetadata?.BinderType
            };
        }
        else
        {
            parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
        }
    }
}