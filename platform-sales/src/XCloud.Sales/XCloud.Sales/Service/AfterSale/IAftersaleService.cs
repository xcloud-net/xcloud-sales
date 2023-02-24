using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Aftersale;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Configuration;
using XCloud.Sales.Service.Orders;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.AfterSale;

public interface IAfterSaleService : ISalesAppService
{
    Task DangerouslyUpdateStatusAsync(DangerouslyUpdateAfterSalesStatusInput dto);

    Task ApproveAsync(ApproveAftersaleInput dto);

    Task RejectAsync(RejectAftersaleInput dto);

    Task CompleteAsync(CompleteAftersaleInput dto);

    Task CancelAsync(CancelAftersaleInput dto);

    Task UpdateStatusAsync(UpdateAfterSaleStatusInput dto);

    Task<ApiResponse<AfterSalesDto>> InsertAsync(AfterSalesDto dto);

    Task<AfterSalesDto> QueryByIdAsync(string afterSaleId);

    Task<AfterSalesDto[]> AttachDataAsync(AfterSalesDto[] data, AttachDataInput dto);

    Task<AfterSalesItemDto[]> AttachAfterSalesItemsDataAsync(AfterSalesItemDto[] data,
        AttachAftersalesItemsDataInput dto);

    Task<int> QueryPendingCountAsync(QueryAftersalePendingCountInput dto);

    Task<PagedResponse<AfterSalesDto>> QueryPagingAsync(QueryAfterSaleInput dto);
}

public class AfterSaleService : SalesAppService, IAfterSaleService
{
    private readonly ISalesRepository<AfterSales> _returnRequestRepository;
    private readonly AfterSaleUtils _afterSaleUtils;
    private readonly IMallSettingService _mallSettingService;

    public AfterSaleService(
        AfterSaleUtils afterSaleUtils,
        ISalesRepository<AfterSales> returnRequestRepository,
        IMallSettingService mallSettingService)
    {
        this._afterSaleUtils = afterSaleUtils;
        _returnRequestRepository = returnRequestRepository;
        _mallSettingService = mallSettingService;
    }

    private async Task TryMarkOrderFinishedWithAfterSaleAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentNullException(nameof(orderId));

        var db = await this._returnRequestRepository.GetDbContextAsync();

        var order = await db.Set<Order>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
            return;

        order.SetOrderStatus(OrderStatus.AfterSale);
        order.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public async Task UpdateStatusAsync(UpdateAfterSaleStatusInput dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var db = await this._returnRequestRepository.GetDbContextAsync();

        var entity = await db.Set<AfterSales>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (dto.IsDeleted != null)
            entity.IsDeleted = dto.IsDeleted.Value;

        if (dto.HideForAdmin != null)
            entity.HideForAdmin = dto.HideForAdmin.Value;

        entity.LastModificationTime = this.Clock.Now;

        await this._returnRequestRepository.UpdateAsync(entity);
    }

    private void CheckCreateAfterSalesInput(AfterSalesDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.UserId <= 0)
            throw new ArgumentException("user id");

        if (string.IsNullOrWhiteSpace(dto.OrderId))
            throw new ArgumentException("order is required");

        if (ValidateHelper.IsEmptyCollection(dto.Items))
            throw new ArgumentException("items is required");

        if (string.IsNullOrWhiteSpace(dto.ReasonForReturn) || string.IsNullOrWhiteSpace(dto.RequestedAction))
            throw new UserFriendlyException("reason and request is required");
    }

    public async Task<ApiResponse<AfterSalesDto>> InsertAsync(AfterSalesDto dto)
    {
        this.CheckCreateAfterSalesInput(dto);

        var mallSettings = await this._mallSettingService.GetCachedMallSettingsAsync();
        if (mallSettings.AftersaleDisabled)
            throw new UserFriendlyException("after sale is disabled by admin");

        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(AfterSaleService)}.{nameof(InsertAsync)}.{dto.OrderId}",
            expiryTime: TimeSpan.FromSeconds(5));

        if (dlock.IsAcquired)
        {
            var db = await this._returnRequestRepository.GetDbContextAsync();
            var order = await db.Set<Order>().FirstOrDefaultAsync(x => x.Id == dto.OrderId);

            if (order == null)
                throw new EntityNotFoundException("order not found");

            if (order.IsAftersales || !string.IsNullOrWhiteSpace(order.AfterSalesId))
                throw new UserFriendlyException("order is already in aftersale");

            if (order.GetOrderStatus() != OrderStatus.Delivering)
                throw new UserFriendlyException("you can't request aftersale now");

            if (await db.Set<AfterSales>().AnyAsync(x => x.OrderId == dto.OrderId))
                throw new UserFriendlyException("a aftersale is started before,pls don't resubmit");

            if (await db.Set<AftersalesItem>().AnyAsync(x => x.OrderId == dto.OrderId))
                throw new UserFriendlyException("a aftersale is started before,pls don't resubmit");

            var entity = this.ObjectMapper.Map<AfterSalesDto, AfterSales>(dto);

            entity.SetAfterSalesStatus(AfterSalesStatus.Processing);
            entity.Id = this.GuidGenerator.CreateGuidString();
            entity.CreationTime = this.Clock.Now;

            var orderItems = await db.Set<OrderItem>().Where(x => x.OrderId == order.Id).ToArrayAsync();
            var items = dto.Items.Select(x => this.ObjectMapper.Map<AfterSalesItemDto, AftersalesItem>(x)).ToArray();
            foreach (var m in items)
            {
                var orderItem = orderItems.FirstOrDefault(x => x.Id == m.OrderItemId);
                if (orderItem == null)
                    throw new AbpException("order item is not exist");

                if (orderItem.Quantity < m.Quantity)
                    throw new AbpException("goods quantity error");

                m.Id = this.GuidGenerator.CreateGuidString();
                m.AftersalesId = entity.Id;
                m.OrderId = order.Id;
            }

            //modify entity
            order.IsAftersales = true;
            order.AfterSalesId = entity.Id;
            db.Set<AfterSales>().Add(entity);
            db.Set<AftersalesItem>().AddRange(items);

            await db.SaveChangesAsync();

            var aftersaleDto = this.ObjectMapper.Map<AfterSales, AfterSalesDto>(entity);

            return new ApiResponse<AfterSalesDto>(aftersaleDto);
        }
        else
        {
            throw new FailToGetRedLockException("can't get dlock");
        }
    }

    public async Task DangerouslyUpdateStatusAsync(DangerouslyUpdateAfterSalesStatusInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(ApproveAsync));

        var db = await this._returnRequestRepository.GetDbContextAsync();

        var afterSales = await db.Set<AfterSales>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (afterSales == null)
            throw new EntityNotFoundException();

        if (dto.Status != null)
            afterSales.AfterSalesStatusId = dto.Status.Value;

        afterSales.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public async Task ApproveAsync(ApproveAftersaleInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(ApproveAsync));

        var db = await this._returnRequestRepository.GetDbContextAsync();
        var aftersales = await db.Set<AfterSales>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (aftersales == null)
            throw new EntityNotFoundException();

        if (aftersales.GetAfterSalesStatus() != AfterSalesStatus.Processing)
            throw new UserFriendlyException("aftersales is not in processing");

        aftersales.SetAfterSalesStatus(AfterSalesStatus.Approved);
        aftersales.LastModificationTime = this.Clock.Now;

        await db.SaveChangesAsync();
    }

    public async Task RejectAsync(RejectAftersaleInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(RejectAsync));

        var db = await this._returnRequestRepository.GetDbContextAsync();
        var aftersales = await db.Set<AfterSales>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (aftersales == null)
            throw new EntityNotFoundException();

        if (aftersales.GetAfterSalesStatus() != AfterSalesStatus.Processing)
            throw new UserFriendlyException("aftersales is not in processing");

        aftersales.SetAfterSalesStatus(AfterSalesStatus.Rejected);
        aftersales.LastModificationTime = this.Clock.Now;

        await db.SaveChangesAsync();
    }

    public async Task CompleteAsync(CompleteAftersaleInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(CompleteAsync));

        var db = await this._returnRequestRepository.GetDbContextAsync();
        var afterSales = await db.Set<AfterSales>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (afterSales == null)
            throw new EntityNotFoundException();

        if (afterSales.GetAfterSalesStatus() != AfterSalesStatus.Approved)
            throw new UserFriendlyException("after sales is not approved");

        afterSales.SetAfterSalesStatus(AfterSalesStatus.Complete);
        afterSales.LastModificationTime = this.Clock.Now;

        await db.SaveChangesAsync();

        await this.TryMarkOrderFinishedWithAfterSaleAsync(afterSales.OrderId);
    }

    public async Task CancelAsync(CancelAftersaleInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(CancelAsync));

        var db = await this._returnRequestRepository.GetDbContextAsync();
        var aftersales = await db.Set<AfterSales>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (aftersales == null)
            throw new EntityNotFoundException();

        if (aftersales.GetAfterSalesStatus() == AfterSalesStatus.Cancelled)
            throw new UserFriendlyException("aftersales is already cancelled");

        aftersales.SetAfterSalesStatus(AfterSalesStatus.Cancelled);
        aftersales.LastModificationTime = this.Clock.Now;

        await db.SaveChangesAsync();

        //update order status
        await this.TryRevertOrderAfterSaleAsync(aftersales.OrderId);
    }

    private async Task TryRevertOrderAfterSaleAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentNullException(nameof(orderId));

        var db = await this._returnRequestRepository.GetDbContextAsync();

        var order = await db.Set<Order>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == orderId);

        if (order == null)
            return;

        order.IsAftersales = false;
        order.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public async Task<AfterSalesDto[]> AttachDataAsync(AfterSalesDto[] data, AttachDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._returnRequestRepository.GetDbContextAsync();

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

        var db = await this._returnRequestRepository.GetDbContextAsync();

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

        var db = await _returnRequestRepository.GetDbContextAsync();

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

        var db = await _returnRequestRepository.GetDbContextAsync();

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

        var db = await _returnRequestRepository.GetDbContextAsync();

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

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        if (dto.SortForAdmin ?? false)
        {
            query = query.OrderBy(x => x.HideForAdmin).ThenByDescending(x => x.CreationTime);
        }
        else
        {
            query = query.OrderBy(x => x.IsDeleted).ThenByDescending(x => x.CreationTime);
        }

        var items = await query.PageBy(dto.AsAbpPagedRequestDto())
            .ToArrayAsync();

        var aftersaleDtos = items.Select(x => ObjectMapper.Map<AfterSales, AfterSalesDto>(x)).ToArray();

        return new PagedResponse<AfterSalesDto>(aftersaleDtos, dto, count);
    }
}