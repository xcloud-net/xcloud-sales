using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Aftersale;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Orders;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.AfterSale;

public interface IAfterSaleService : ISalesAppService
{
    Task UpdateStatusAsync(UpdateAfterSaleStatusInput dto);

    Task<AfterSalesDto> QueryByIdAsync(string afterSaleId);

    Task<AfterSalesDto[]> AttachDataAsync(AfterSalesDto[] data, AttachDataInput dto);

    Task<AfterSalesItemDto[]> AttachAfterSalesItemsDataAsync(AfterSalesItemDto[] data,
        AttachAftersalesItemsDataInput dto);

    Task<int> QueryPendingCountAsync(QueryAftersalePendingCountInput dto);

    Task<PagedResponse<AfterSalesDto>> QueryPagingAsync(QueryAfterSaleInput dto);
}

public class AfterSaleService : SalesAppService, IAfterSaleService
{
    private readonly ISalesRepository<AfterSales> _afterSalesRepository;
    private readonly AfterSaleUtils _afterSaleUtils;

    public AfterSaleService(
        AfterSaleUtils afterSaleUtils,
        ISalesRepository<AfterSales> afterSalesRepository)
    {
        this._afterSaleUtils = afterSaleUtils;
        _afterSalesRepository = afterSalesRepository;
    }

    public async Task UpdateStatusAsync(UpdateAfterSaleStatusInput dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var db = await this._afterSalesRepository.GetDbContextAsync();

        var entity = await db.Set<AfterSales>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (dto.IsDeleted != null)
            entity.IsDeleted = dto.IsDeleted.Value;

        if (dto.HideForAdmin != null)
            entity.HideForAdmin = dto.HideForAdmin.Value;

        entity.LastModificationTime = this.Clock.Now;

        await this._afterSalesRepository.UpdateAsync(entity);
    }

    public async Task<AfterSalesDto[]> AttachDataAsync(AfterSalesDto[] data, AttachDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._afterSalesRepository.GetDbContextAsync();

        if (dto.Order)
        {
            var orderIds = data.Select(x => x.OrderId).Distinct().ToArray();
            if (orderIds.Any())
            {
                var orders = await db.Set<Order>().AsNoTracking().WhereIdIn(orderIds).ToArrayAsync();
                foreach (var m in data)
                {
                    var order = orders.FirstOrDefault(x => x.Id == m.OrderId);
                    if (order == null)
                        continue;

                    m.Order = this.ObjectMapper.Map<Order, OrderDto>(order);
                }
            }
        }

        if (dto.Items)
        {
            var ids = data.Ids().ToArray();
            var allitems = await db.Set<AftersalesItem>().AsNoTracking().Where(x => ids.Contains(x.AftersalesId))
                .ToArrayAsync();
            foreach (var m in data)
            {
                var items = allitems.Where(x => x.AftersalesId == m.Id).ToArray();
                m.Items = items.Select(x => this.ObjectMapper.Map<AftersalesItem, AfterSalesItemDto>(x)).ToArray();
            }
        }

        if (dto.User)
        {
            var userids = data.Select(x => x.UserId).Distinct().ToArray();
            var allusers = await db.Set<User>().AsNoTracking().Where(x => userids.Contains(x.Id)).ToArrayAsync();

            foreach (var m in data)
            {
                var u = allusers.FirstOrDefault(x => x.Id == m.UserId);
                if (u == null)
                    continue;

                m.User = this.ObjectMapper.Map<User, StoreUserDto>(u);
            }
        }

        return data;
    }

    public async Task<AfterSalesItemDto[]> AttachAfterSalesItemsDataAsync(AfterSalesItemDto[] data,
        AttachAftersalesItemsDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._afterSalesRepository.GetDbContextAsync();

        if (dto.OrderItems)
        {
            var ids = data.Select(x => x.OrderItemId).ToArray();
            var allitems = await db.Set<OrderItem>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync();
            foreach (var m in data)
            {
                var item = allitems.Where(x => x.Id == m.OrderItemId).FirstOrDefault();
                if (item == null)
                    continue;
                m.OrderItem = this.ObjectMapper.Map<OrderItem, OrderItemDto>(item);
            }
        }

        return data;
    }

    public async Task<int> QueryPendingCountAsync(QueryAftersalePendingCountInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var db = await _afterSalesRepository.GetDbContextAsync();

        var finishedStatus = this._afterSaleUtils.DoneStatus();

        var query = db.Set<AfterSales>().Where(x => !finishedStatus.Contains(x.AfterSalesStatusId));

        if (dto.UserId != null && dto.UserId.Value > 0)
            query = query.Where(x => x.UserId == dto.UserId.Value);

        var count = await query.CountAsync();

        return count;
    }

    public async Task<AfterSalesDto> QueryByIdAsync(string afterSaleId)
    {
        if (string.IsNullOrWhiteSpace(afterSaleId))
            throw new ArgumentNullException(nameof(QueryByIdAsync));

        var db = await _afterSalesRepository.GetDbContextAsync();

        var query = db.Set<AfterSales>().AsNoTracking();

        var entity = await query.FirstOrDefaultAsync(x => x.Id == afterSaleId);

        if (entity == null)
            return null;

        var dto = this.ObjectMapper.Map<AfterSales, AfterSalesDto>(entity);

        return dto;
    }

    public async Task<PagedResponse<AfterSalesDto>> QueryPagingAsync(QueryAfterSaleInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var db = await _afterSalesRepository.GetDbContextAsync();

        var query = db.Set<AfterSales>().IgnoreQueryFilters().AsNoTracking();

        if (dto.IsDeleted != null)
            query = query.Where(x => x.IsDeleted == dto.IsDeleted.Value);

        if (dto.HideForAdmin != null)
            query = query.Where(x => x.HideForAdmin == dto.HideForAdmin.Value);

        if (dto.UserId != null && dto.UserId.Value > 0)
            query = query.Where(x => x.UserId == dto.UserId.Value);

        if (ValidateHelper.IsNotEmptyCollection(dto.Status))
            query = query.Where(x => dto.Status.Contains(x.AfterSalesStatusId));

        if (dto.IsAfterSalesPending ?? false)
        {
            var pendingStatus = this._afterSaleUtils.PendingStatus();
            query = query.Where(x => pendingStatus.Contains(x.AfterSalesStatusId));
        }

        if (dto.StartTime != null)
            query = query.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.CreationTime <= dto.EndTime.Value);

        var count = await query.CountOrDefaultAsync(dto);

        if (dto.SortForAdmin ?? false)
        {
            query = query.OrderBy(x => x.HideForAdmin).ThenByDescending(x => x.CreationTime);
        }
        else
        {
            query = query.OrderBy(x => x.IsDeleted).ThenByDescending(x => x.CreationTime);
        }

        var items = await query.PageBy(dto.ToAbpPagedRequest())
            .ToArrayAsync();

        var aftersaleDtos = items.Select(x => ObjectMapper.Map<AfterSales, AfterSalesDto>(x)).ToArray();

        return new PagedResponse<AfterSalesDto>(aftersaleDtos, dto, count);
    }
}