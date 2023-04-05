using XCloud.Application.Mapper;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Finance;

namespace XCloud.Sales.Service.Finance;

public interface IOrderRefundBillService : ISalesAppService
{
    Task<OrderRefundBillDto> QueryByIdAsync(string refundBillId);
    
    Task<OrderRefundBillDto[]> AttachDataAsync(OrderRefundBillDto[] data, AttachRefundBillDataInput dto);

    Task<PagedResponse<OrderRefundBillDto>> QueryPagingAsync(QueryRefundBillPagingInput dto);

    Task CreateRefundBillAsync(OrderRefundBillDto dto);

    Task UpdateStatusAsync(UpdateRefundBillStatus dto);

    Task ApproveAsync(ApproveRefundBillInput dto);

    Task MarkRefundBillAsRefundedAsync(MarkRefundBillAsRefundInput dto);
}

public class OrderRefundBillService : SalesAppService, IOrderRefundBillService
{
    private readonly ISalesRepository<OrderRefundBill> _salesRepository;
    private readonly IOrderBillService _orderBillService;

    public OrderRefundBillService(ISalesRepository<OrderRefundBill> salesRepository, IOrderBillService orderBillService)
    {
        _salesRepository = salesRepository;
        _orderBillService = orderBillService;
    }

    public async Task<OrderRefundBillDto> QueryByIdAsync(string refundBillId)
    {
        if (string.IsNullOrWhiteSpace(refundBillId))
            throw new ArgumentNullException(nameof(refundBillId));

        var bill = await this._salesRepository.QueryOneAsync(x => x.Id == refundBillId);

        if (bill == null)
            return null;

        return this.ObjectMapper.Map<OrderRefundBill, OrderRefundBillDto>(bill);
    }

    public async Task<OrderRefundBillDto[]> AttachDataAsync(OrderRefundBillDto[] data, AttachRefundBillDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._salesRepository.GetDbContextAsync();

        if (dto.OrderBill)
        {
            var billIds = data.Select(x => x.BillId).Distinct().ToArray();
            var billList = await db.Set<OrderBill>().AsNoTracking().WhereIdIn(billIds).ToArrayAsync();
            foreach (var m in data)
            {
                var bill = billList.FirstOrDefault(x => x.Id == m.BillId);
                if (bill == null)
                    continue;

                m.OrderBill = this.ObjectMapper.Map<OrderBill, OrderBillDto>(bill);
            }
        }

        return data;
    }

    public async Task<PagedResponse<OrderRefundBillDto>> QueryPagingAsync(QueryRefundBillPagingInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var db = await this._salesRepository.GetDbContextAsync();

        var query = db.Set<OrderRefundBill>().AsNoTracking();

        var count = await query.CountOrDefaultAsync(dto);
        var data = await query.OrderBy(x => x.CreationTime).PageBy(dto.ToAbpPagedRequest()).ToArrayAsync();

        var items = this.ObjectMapper.MapArray<OrderRefundBill, OrderRefundBillDto>(data);

        return new PagedResponse<OrderRefundBillDto>(items, dto, count);
    }

    public async Task CreateRefundBillAsync(OrderRefundBillDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.OrderId))
            throw new ArgumentNullException(nameof(dto.OrderId));

        if (string.IsNullOrWhiteSpace(dto.BillId))
            throw new ArgumentNullException(nameof(dto.BillId));

        var db = await this._salesRepository.GetDbContextAsync();

        var bill = await this._orderBillService.QueryByIdAsync(dto.BillId);
        if (bill == null)
            throw new EntityNotFoundException(nameof(bill));

        var total = bill.Price;

        var refundTotal = await db.Set<OrderRefundBill>().AsNoTracking()
            .Where(x => x.OrderId == dto.OrderId)
            .Where(x => x.BillId == bill.Id)
            .Where(x => x.Approved && x.Refunded)
            .SumAsync(x => x.Price);

        var remain = total - refundTotal;

        if (dto.Price > remain)
            throw new UserFriendlyException($"you can refund {remain} maximum");

        var entity = this.ObjectMapper.Map<OrderRefundBillDto, OrderRefundBill>(dto);

        await this._salesRepository.InsertAsync(entity);
    }

    public async Task MarkRefundBillAsRefundedAsync(MarkRefundBillAsRefundInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var entity = await this._salesRepository.QueryOneAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(MarkRefundBillAsRefundedAsync));

        if (entity.Refunded)
            return;

        entity.Refunded = true;
        entity.RefundTime = this.Clock.Now;
        entity.RefundTransactionId = dto.TransactionId;
        entity.RefundNotifyData = dto.NotifyData;

        entity.LastModificationTime = entity.RefundTime.Value;

        await this._salesRepository.UpdateAsync(entity);
    }

    public async Task ApproveAsync(ApproveRefundBillInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var entity = await this._salesRepository.QueryOneAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (entity.Approved)
            return;

        entity.Approved = true;
        entity.ApprovedTime = this.Clock.Now;

        await this._salesRepository.UpdateAsync(entity);
    }

    public async Task UpdateStatusAsync(UpdateRefundBillStatus dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var db = await this._salesRepository.GetDbContextAsync();

        var entity = await db.Set<OrderRefundBill>().IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        var now = this.Clock.Now;

        if (dto.IsDeleted != null)
        {
            entity.IsDeleted = dto.IsDeleted.Value;
            if (entity.IsDeleted)
                entity.DeletionTime = now;
        }

        entity.LastModificationTime = now;

        await db.TrySaveChangesAsync();
    }
}