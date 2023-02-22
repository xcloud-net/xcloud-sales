using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.WarehouseStock;

namespace XCloud.Sales.Service.WarehouseStock;

public interface IStockUsageHistoryService : ISalesPagingStringAppService<StockUsageHistory, StockUsageHistoryDto,
    QueryStockUsageHistoryPagingInput>
{
    //
}

public class StockUsageHistoryService : SalesPagingStringAppService<StockUsageHistory, StockUsageHistoryDto,
        QueryStockUsageHistoryPagingInput>,
    IStockUsageHistoryService
{
    public StockUsageHistoryService(ISalesRepository<StockUsageHistory> repository) : base(repository)
    {
        //
    }

    public override Task DeleteByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public override Task DeleteByIdsAsync(string[] ids)
    {
        throw new NotImplementedException();
    }

    public override Task<StockUsageHistory> UpdateAsync(StockUsageHistoryDto dto)
    {
        throw new NotImplementedException();
    }

    protected override async Task CheckBeforeInsertAsync(StockUsageHistoryDto dto)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(dto.OrderItemId))
            throw new ArgumentNullException(nameof(dto.OrderItemId));

        if (string.IsNullOrWhiteSpace(dto.WarehouseStockItemId))
            throw new ArgumentNullException(nameof(dto.WarehouseStockItemId));

        if (dto.Quantity <= 0)
            throw new ArgumentNullException(nameof(dto.Quantity));
    }

    protected override async Task InitBeforeInsertAsync(StockUsageHistory entity)
    {
        await base.InitBeforeInsertAsync(entity);

        entity.Revert = false;
        entity.RevertTime = null;
    }
}