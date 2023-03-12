using XCloud.Application.Mapper;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Aftersale;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Configuration;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Service.AfterSale;

public interface IAfterSaleProcessingService : ISalesAppService
{
    Task DangerouslyUpdateStatusAsync(DangerouslyUpdateAfterSalesStatusInput dto);

    Task ApproveAsync(ApproveAftersaleInput dto);

    Task RejectAsync(RejectAftersaleInput dto);

    Task CompleteAsync(CompleteAftersaleInput dto);

    Task CancelAsync(CancelAftersaleInput dto);

    Task<ApiResponse<AfterSalesDto>> CreateAsync(AfterSalesDto dto);
}

public class AfterSaleProcessingService : SalesAppService, IAfterSaleProcessingService
{
    private readonly ISalesRepository<AfterSales> _afterSalesRepository;
    private readonly AfterSaleUtils _afterSaleUtils;
    private readonly OrderUtils _orderUtils;
    private readonly IMallSettingService _mallSettingService;

    public AfterSaleProcessingService(
        AfterSaleUtils afterSaleUtils,
        IMallSettingService mallSettingService,
        ISalesRepository<AfterSales> afterSalesRepository,
        OrderUtils orderUtils)
    {
        this._afterSaleUtils = afterSaleUtils;
        _mallSettingService = mallSettingService;
        _afterSalesRepository = afterSalesRepository;
        _orderUtils = orderUtils;
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

    private async Task<Order> GetRequiredOrderForAfterSalesAsync(DbContext db, string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentNullException(nameof(orderId));

        var order = await db.Set<Order>().FirstOrDefaultAsync(x => x.Id == orderId);

        if (order == null)
            throw new EntityNotFoundException("order not found");

        if (order.IsAftersales || !string.IsNullOrWhiteSpace(order.AfterSalesId))
            throw new UserFriendlyException("order is already in aftersale");

        if (order.GetOrderStatus() != OrderStatus.Delivering)
            throw new UserFriendlyException("you can't request aftersale now");

        return order;
    }

    private async Task CheckBeforeAfterSalesAsync(DbContext db, AfterSalesDto dto)
    {
        if (await db.Set<AfterSales>().AnyAsync(x => x.OrderId == dto.OrderId))
            throw new UserFriendlyException("a after sale is started before,pls don't resubmit");

        if (await db.Set<AftersalesItem>().AnyAsync(x => x.OrderId == dto.OrderId))
            throw new UserFriendlyException("a after sale is started before,pls don't resubmit");
    }

    public async Task<ApiResponse<AfterSalesDto>> CreateAsync(AfterSalesDto dto)
    {
        this.CheckCreateAfterSalesInput(dto);

        var mallSettings = await this._mallSettingService.GetCachedMallSettingsAsync();
        if (mallSettings.AftersaleDisabled)
            throw new UserFriendlyException("after sale is disabled by admin");

        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(AfterSaleService)}.{nameof(CreateAsync)}.{dto.OrderId}",
            expiryTime: TimeSpan.FromSeconds(5));

        if (dlock.IsAcquired)
        {
            var db = await this._afterSalesRepository.GetDbContextAsync();

            //check
            await this.CheckBeforeAfterSalesAsync(db, dto);

            //query relative order
            var order = await this.GetRequiredOrderForAfterSalesAsync(db, dto.OrderId);
            var orderItems = await db.Set<OrderItem>().Where(x => x.OrderId == order.Id).ToArrayAsync();

            //mapping
            var entity = this.ObjectMapper.Map<AfterSalesDto, AfterSales>(dto);
            var items = this.ObjectMapper.MapArray<AfterSalesItemDto, AftersalesItem>(dto.Items);

            //set entity fields
            entity.SetAfterSalesStatus(AfterSalesStatus.Processing);
            entity.Id = this.GuidGenerator.CreateGuidString();
            entity.CreationTime = this.Clock.Now;

            //set items fields
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

            //save changes
            await db.SaveChangesAsync();

            var response = this.ObjectMapper.Map<AfterSales, AfterSalesDto>(entity);

            return new ApiResponse<AfterSalesDto>(response);
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

        var db = await this._afterSalesRepository.GetDbContextAsync();

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

        var db = await this._afterSalesRepository.GetDbContextAsync();
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

        var db = await this._afterSalesRepository.GetDbContextAsync();
        var aftersales = await db.Set<AfterSales>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (aftersales == null)
            throw new EntityNotFoundException();

        if (aftersales.GetAfterSalesStatus() != AfterSalesStatus.Processing)
            throw new UserFriendlyException("aftersales is not in processing");

        aftersales.SetAfterSalesStatus(AfterSalesStatus.Rejected);
        aftersales.LastModificationTime = this.Clock.Now;

        await db.SaveChangesAsync();
    }

    private async Task TryMarkOrderFinishedWithAfterSaleAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentNullException(nameof(orderId));

        var db = await this._afterSalesRepository.GetDbContextAsync();

        var order = await db.Set<Order>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
            return;

        //manual set status
        order.SetOrderStatus(OrderStatus.FinishWithAfterSale);

        //state flow
        await this._orderUtils.GetOrderStateMachine(order).FireAsync(OrderProcessingAction.FinishWithAfterSale);

        order.IsAftersales = false;
        order.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public async Task CompleteAsync(CompleteAftersaleInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(CompleteAsync));

        var db = await this._afterSalesRepository.GetDbContextAsync();
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

        var db = await this._afterSalesRepository.GetDbContextAsync();
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

        var db = await this._afterSalesRepository.GetDbContextAsync();

        var order = await db.Set<Order>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == orderId);

        if (order == null)
            return;

        order.IsAftersales = false;
        order.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }
}