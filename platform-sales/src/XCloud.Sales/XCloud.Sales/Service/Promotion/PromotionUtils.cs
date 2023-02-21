using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Orders.Validator;

namespace XCloud.Sales.Service.Promotion;

public interface IPromotionResultHandler
{
    int Order { get; }
    string ResultHandlerCode { get; }
    string ResolveDescriptor(PromotionResult promotionResult);
    Task<bool> IsResultTypeSupportedAsync(PromotionResult promotionResult);
    Task<Order> ApplyToOrderAsync(Order order, PromotionResult promotionResult);
}

[ExposeServices(typeof(PromotionUtils))]
public class PromotionUtils : IScopedDependency
{
    public IReadOnlyCollection<IPromotionResultHandler> ResultHandlers { get; }

    private readonly IJsonDataSerializer _jsonDataSerializer;
    private readonly ILogger _logger;

    public PromotionUtils(IServiceProvider serviceProvider,
        IJsonDataSerializer jsonDataSerializer,
        ILogger<PromotionUtils> logger)
    {
        this._logger = logger;
        this._jsonDataSerializer = jsonDataSerializer;
        //resolve implements
        this.ResultHandlers = this.ResolveResultHandlers(serviceProvider);
    }

    private IPromotionResultHandler[] ResolveResultHandlers(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetServices<IPromotionResultHandler>()
            .OrderByDescending(x => x.Order).ToArray();
    }

    public async Task<string> TryResolveDescriptor(IPromotionResultHandler handler, PromotionResult result)
    {
        try
        {
            if (!await handler.IsResultTypeSupportedAsync(result))
                return null;

            return handler.ResolveDescriptor(result);
        }
        catch (Exception e)
        {
            this._logger.LogWarning(exception: e, message: e.Message);
            return "parse result descriptor error";
        }
    }

    public PromotionResult[] DeserializeResults(string resultJson, bool throwIfException = false)
    {
        if (string.IsNullOrWhiteSpace(resultJson))
            return Array.Empty<PromotionResult>();

        try
        {
            var results = this._jsonDataSerializer.DeserializeFromString<PromotionResult[]>(resultJson);
            return results;
        }
        catch (Exception e)
        {
            if (throwIfException)
                throw;
            else
                this._logger.LogError(message: e.Message, exception: e);
        }

        return Array.Empty<PromotionResult>();
    }

    public StorePromotionDto SerializeConfigFields(StorePromotionDto dto)
    {
        dto.PromotionConditions ??= Array.Empty<OrderCondition>();
        dto.PromotionResults ??= Array.Empty<PromotionResult>();

        dto.Condition = this._jsonDataSerializer.SerializeToString(dto.PromotionConditions);
        dto.Result = this._jsonDataSerializer.SerializeToString(dto.PromotionResults);
        return dto;
    }

    public async Task<Order> ApplyPromotionResultAsync(Order order, PromotionResult[] results)
    {
        if (results == null)
            throw new ArgumentNullException(nameof(results));

        foreach (var result in results)
        {
            foreach (var handler in this.ResultHandlers)
            {
                if (await handler.IsResultTypeSupportedAsync(result))
                {
                    order = await handler.ApplyToOrderAsync(order, result);
                    //only first result applied
                    break;
                }
            }
        }

        return order;
    }
}