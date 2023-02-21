using Volo.Abp.ObjectMapping;
using XCloud.Core.Helper;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Promotion;

namespace XCloud.Sales.Service.Orders.Validator;

public interface IOrderConditionValidator
{
    int Order { get; }

    string ConditionCode { get; }

    string ResolveDescriptor(OrderCondition promotionCondition);

    Task<bool> IsConditionTypeSupportedAsync(OrderCondition condition);

    Task<OrderConditionValidationResponse> IsConditionMatchedAsync(OrderConditionCheckInput promotionCheckInput,
        OrderCondition condition);
}

[ExposeServices(typeof(OrderConditionUtils))]
public class OrderConditionUtils : IScopedDependency
{
    public IReadOnlyCollection<IOrderConditionValidator> ConditionValidators { get; }

    private readonly IJsonDataSerializer _jsonDataSerializer;
    private readonly IObjectMapper _objectMapper;
    private readonly ILogger _logger;

    public OrderConditionUtils(IServiceProvider serviceProvider,
        IJsonDataSerializer jsonDataSerializer,
        IObjectMapper objectMapper,
        ILogger<PromotionUtils> logger)
    {
        this._logger = logger;
        this._objectMapper = objectMapper;
        this._jsonDataSerializer = jsonDataSerializer;
        //resolve implements
        this.ConditionValidators = this.ResolveConditionMatchers(serviceProvider);
    }

    private IOrderConditionValidator[] ResolveConditionMatchers(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetServices<IOrderConditionValidator>()
            .OrderByDescending(x => x.Order).ToArray();
    }

    public OrderCondition[] DeserializeConditions(string conditionJson, bool throwIfException = false)
    {
        if (string.IsNullOrWhiteSpace(conditionJson))
            return Array.Empty<OrderCondition>();

        try
        {
            var conditions = this._jsonDataSerializer.DeserializeFromString<OrderCondition[]>(conditionJson);
            return conditions;
        }
        catch (Exception e)
        {
            if (throwIfException)
                throw;
            else
                this._logger.LogError(message: e.Message, exception: e);
        }

        return Array.Empty<OrderCondition>();
    }

    public async Task<string> TryResolveDescriptor(IOrderConditionValidator validator, OrderCondition condition)
    {
        try
        {
            if (!await validator.IsConditionTypeSupportedAsync(condition))
                return null;

            return validator.ResolveDescriptor(condition);
        }
        catch (Exception e)
        {
            this._logger.LogWarning(exception: e, message: e.Message);
            return "parse condition descriptor error";
        }
    }

    public async Task CheckOrderConditionCheckInputAsync(OrderConditionCheckInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Order == null)
            throw new ArgumentNullException(nameof(dto.Order));

        if (ValidateHelper.IsEmptyCollection(dto.Order.Items))
            throw new ArgumentNullException(nameof(dto.Order.Items));

        await Task.CompletedTask;
    }

    public OrderConditionCheckInput BuildPromotionCheckInput(Order order, OrderItem[] items)
    {
        var dto = this._objectMapper.Map<Order, OrderDto>(order);

        dto.Items = items.Select(x => this._objectMapper.Map<OrderItem, OrderItemDto>(x)).ToArray();

        return new OrderConditionCheckInput(dto);
    }

    public async IAsyncEnumerable<OrderConditionValidationResponse> ValidateOrderConditionsAsync(
        OrderConditionCheckInput dto,
        OrderCondition[] conditions)
    {
        await this.CheckOrderConditionCheckInputAsync(dto);

        foreach (var condition in conditions)
        {
            foreach (var m in this.ConditionValidators)
            {
                if (await m.IsConditionTypeSupportedAsync(condition))
                {
                    var result = await m.IsConditionMatchedAsync(dto, condition);
                    yield return result;
                }
            }
        }
    }

    public async Task<bool> ValidateOrderConditionsAsync(OrderConditionCheckInput dto, string conditionJson,
        bool throwIfException = true)
    {
        var conditions = this.DeserializeConditions(conditionJson, throwIfException: throwIfException);

        await foreach (var m in this.ValidateOrderConditionsAsync(dto, conditions))
        {
            if (!m.IsMatch)
                return false;
        }

        return true;
    }
}