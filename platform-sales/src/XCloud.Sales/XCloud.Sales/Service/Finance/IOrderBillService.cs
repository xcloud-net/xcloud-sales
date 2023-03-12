using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Finance;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Service.Finance;

public interface IOrderBillService : ISalesAppService
{
    Task<OrderBillDto> CreateBillAsync(OrderBillDto dto);

    Task MarkBillAsPayedAsync(MarkBillAsPayedInput dto);

    [Obsolete]
    Task MarkBillAsRefundAsync(MarkBillAsRefundInput dto);

    Task<OrderBillDto[]> QueryByOrderIdAsync(ListOrderBillInput dto);

    Task<OrderBillDto> QueryByIdAsync(string billNo);

    Task<PagedResponse<OrderBillDto>> QueryPagingAsync(QueryOrderBillPagingInput dto);

    Task<OrderBillDto[]> AttachDataAsync(OrderBillDto[] data, AttachOrderBillDataInput dto);
}

public class OrderBillService : SalesAppService, IOrderBillService
{
    private readonly ISalesRepository<OrderBill> _billRepository;

    public OrderBillService(ISalesRepository<OrderBill> billRepository)
    {
        this._billRepository = billRepository;
    }

    public async Task<OrderBillDto[]> AttachDataAsync(OrderBillDto[] data, AttachOrderBillDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._billRepository.GetDbContextAsync();

        if (dto.Order)
        {
            var orderIds = data
                .Where(x => !string.IsNullOrWhiteSpace(x.OrderId))
                .Select(x => x.OrderId)
                .Distinct().ToArray();

            if (orderIds.Any())
            {
                var orders = await db.Set<Order>().AsNoTracking().WhereIdIn(orderIds).ToArrayAsync();
                foreach (var m in data)
                {
                    var order = orders.FirstOrDefault(x => x.Id == m.Id);
                    if (order == null)
                        continue;
                    m.Order = this.ObjectMapper.Map<Order, OrderDto>(order);
                }
            }
        }

        return data;
    }

    public async Task<PagedResponse<OrderBillDto>> QueryPagingAsync(QueryOrderBillPagingInput dto)
    {
        var db = await this._billRepository.GetDbContextAsync();

        var query = from bill in db.Set<OrderBill>().AsNoTracking()
            join order in db.Set<Order>().AsNoTracking().IgnoreQueryFilters()
                on bill.OrderId equals order.Id into orderGrouping
            from orderOrEmpty in orderGrouping.DefaultIfEmpty()
            select new { bill, orderOrEmpty };

        if (!string.IsNullOrWhiteSpace(dto.OrderId))
            query = query.Where(x => x.orderOrEmpty.Id == dto.OrderId);

        if (!string.IsNullOrWhiteSpace(dto.OrderNo))
            query = query.Where(x => x.orderOrEmpty.OrderSn == dto.OrderNo);

        if (!string.IsNullOrWhiteSpace(dto.OutTradeNo))
            query = query.Where(x => x.bill.Id == dto.OutTradeNo);

        if (!string.IsNullOrWhiteSpace(dto.PaymentTransactionId))
            query = query.Where(x => x.bill.PaymentTransactionId == dto.PaymentTransactionId);

        if (dto.Paid != null)
            query = query.Where(x => x.bill.Paid == dto.Paid.Value);

        if (dto.PaymentMethod != null)
            query = query.Where(x => x.bill.PaymentMethod == dto.PaymentMethod.Value);

        if (dto.Refunded != null)
            query = query.Where(x => x.bill.Refunded == dto.Refunded.Value);

        if (!string.IsNullOrWhiteSpace(dto.RefundTransactionId))
            query = query.Where(x => x.bill.RefundTransactionId == dto.RefundTransactionId);

        var count = await query.CountOrDefaultAsync(dto);

        var list = await query
            .OrderByDescending(x => x.bill.CreationTime)
            .PageBy(dto.ToAbpPagedRequest())
            .ToArrayAsync();

        var items = new List<OrderBillDto>();

        foreach (var m in list)
        {
            var bill = this.ObjectMapper.Map<OrderBill, OrderBillDto>(m.bill);
            if (m.orderOrEmpty != null)
            {
                bill.Order = this.ObjectMapper.Map<Order, OrderDto>(m.orderOrEmpty);
            }

            items.Add(bill);
        }

        return new PagedResponse<OrderBillDto>(items.ToArray(), dto, count);
    }

    public async Task<OrderBillDto> QueryByIdAsync(string billNo)
    {
        if (string.IsNullOrWhiteSpace(billNo))
            throw new ArgumentNullException(nameof(QueryByIdAsync));

        var db = await _billRepository.GetDbContextAsync();

        var entity = await db.Set<OrderBill>().FirstOrDefaultAsync(x => x.Id == billNo);

        if (entity == null)
            return null;

        var billDto = ObjectMapper.Map<OrderBill, OrderBillDto>(entity);
        return billDto;
    }

    public async Task MarkBillAsRefundAsync(MarkBillAsRefundInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(MarkBillAsRefundAsync));

        var db = await _billRepository.GetDbContextAsync();

        var entity = await db.Set<OrderBill>().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(MarkBillAsPayedAsync));

        if (!entity.Paid)
            throw new UserFriendlyException("bill has not been paid");

        if (entity.Refunded ?? false)
            throw new UserFriendlyException("bill has been refunded");

        entity.Refunded = true;
        entity.RefundTime = this.Clock.Now;
        entity.RefundTransactionId = dto.TransactionId;
        entity.RefundNotifyData = dto.NotifyData;

        await this._billRepository.UpdateAsync(entity);
    }

    public async Task MarkBillAsPayedAsync(MarkBillAsPayedInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(MarkBillAsPayedAsync));

        var db = await _billRepository.GetDbContextAsync();

        var entity = await db.Set<OrderBill>().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(MarkBillAsPayedAsync));

        if (entity.Paid)
            return;

        entity.Paid = true;
        entity.PaymentMethod = dto.PaymentMethod;
        entity.PayTime = Clock.Now;
        entity.PaymentTransactionId = dto.TransactionId;
        entity.NotifyData = dto.NotifyData;

        await db.TrySaveChangesAsync();
    }

    public async Task<OrderBillDto> CreateBillAsync(OrderBillDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.OrderId))
            throw new ArgumentNullException(nameof(CreateBillAsync));

        var entity = ObjectMapper.Map<OrderBillDto, OrderBill>(dto);
        entity.Id = GuidGenerator.CreateGuidString();
        entity.CreationTime = Clock.Now;

        entity.Paid = false;
        entity.PaymentMethod = (int)PaymentMethod.None;
        entity.PayTime = null;
        entity.PaymentTransactionId = string.Empty;
        entity.NotifyData = string.Empty;

        entity.Refunded = null;
        entity.RefundTime = null;
        entity.RefundTransactionId = string.Empty;
        entity.RefundNotifyData = string.Empty;

        await _billRepository.InsertNowAsync(entity);

        var billDto = ObjectMapper.Map<OrderBill, OrderBillDto>(entity);
        return billDto;
    }

    public async Task<OrderBillDto[]> QueryByOrderIdAsync(ListOrderBillInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(QueryByOrderIdAsync));

        var db = await _billRepository.GetDbContextAsync();

        var query = db.Set<OrderBill>().AsNoTracking().Where(x => x.OrderId == dto.Id);

        if (dto.Paid != null)
            query = query.Where(x => x.Paid == dto.Paid.Value);

        if (dto.PaymentMethod != null)
            query = query.Where(x => x.PaymentMethod == dto.PaymentMethod.Value);

        var count = dto.MaxCount ?? 5000;

        var data = await query.OrderBy(x => x.CreationTime).Take(count).ToArrayAsync();

        var billDtos = data.Select(x => ObjectMapper.Map<OrderBill, OrderBillDto>(x)).ToArray();

        return billDtos;
    }
}