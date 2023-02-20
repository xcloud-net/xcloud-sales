using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Shipping;
using XCloud.Sales.Services.Orders;

namespace XCloud.Sales.Services.Shipping;

public interface IShipmentService : ISalesAppService
{
    Task<ApiResponse<ShipmentDto>> CreateShippingAsync(ShipmentDto dto);

    Task<ShipmentDto[]> QueryByOrderIdAsync(string orderId);

    Task<PagedResponse<ShipmentDto>> QueryPagingAsync(QueryShipmentInput dto);

    Task<ShipmentDto[]> AttachDataAsync(ShipmentDto[] data, AttachDataInput dto);

    Task<ShipmentOrderItemDto[]> AttachItemDataAsync(ShipmentOrderItemDto[] data, AttachItemDataInput dto);
}

public class ShipmentService : SalesAppService, IShipmentService
{
    private readonly ISalesRepository<Shipment> _shipmentRepository;
    private readonly ISalesRepository<ShipmentOrderItem> _sopvRepository;

    public ShipmentService(ISalesRepository<Shipment> shipmentRepository,
        ISalesRepository<ShipmentOrderItem> sopvRepository)
    {
        this._shipmentRepository = shipmentRepository;
        this._sopvRepository = sopvRepository;
    }

    void CheckShippingRequestInput(ShipmentDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(CheckShippingRequestInput));

        if (string.IsNullOrWhiteSpace(dto.OrderId))
            throw new ArgumentNullException(nameof(dto.OrderId));

        if (string.IsNullOrWhiteSpace(dto.ShippingMethod))
            throw new ArgumentNullException(nameof(dto.ShippingMethod));

        if (ValidateHelper.IsEmptyCollection(dto.Items))
            throw new ArgumentNullException(nameof(dto.Items));

        if (dto.Items.Any(x => x.OrderItemId <= 0 || x.Quantity <= 0))
            throw new ArgumentNullException("order items and quantity");
    }

    async Task CheckOrderShippingAsync(DbContext db, ShipmentDto dto)
    {
        var query = db.Set<Shipment>().AsNoTracking();
        if (await query.AnyAsync(x => x.OrderId == dto.OrderId))
            throw new UserFriendlyException("shipment already exist");
    }

    public async Task<ApiResponse<ShipmentDto>> CreateShippingAsync(ShipmentDto dto)
    {
        this.CheckShippingRequestInput(dto);

        var db = await this._shipmentRepository.GetDbContextAsync();

        await this.CheckOrderShippingAsync(db, dto);

        var entity = this.ObjectMapper.Map<ShipmentDto, Shipment>(dto);
        var items = dto.Items.Select(x => this.ObjectMapper.Map<ShipmentOrderItemDto, ShipmentOrderItem>(x)).ToArray();

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;
        entity.ShippedTime = entity.CreationTime;
        entity.DeliveryTime = entity.CreationTime;

        foreach (var m in items)
        {
            m.Id = this.GuidGenerator.CreateGuidString();
            m.ShipmentId = entity.Id;
        }

        db.Set<Shipment>().Add(entity);
        db.Set<ShipmentOrderItem>().AddRange(items);

        await db.SaveChangesAsync();

        var response = this.ObjectMapper.Map<Shipment, ShipmentDto>(entity);
        response.Items = items.Select(x => this.ObjectMapper.Map<ShipmentOrderItem, ShipmentOrderItemDto>(x)).ToArray();

        return new ApiResponse<ShipmentDto>(response);
    }

    public async Task<ShipmentDto[]> QueryByOrderIdAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentNullException(nameof(orderId));

        var db = await this._shipmentRepository.GetDbContextAsync();

        var list = await db.Set<Shipment>().AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.CreationTime)
            .TakeUpTo5000().ToArrayAsync();

        var response = list.Select(x => this.ObjectMapper.Map<Shipment, ShipmentDto>(x)).ToArray();

        return response;
    }

    public async Task<PagedResponse<ShipmentDto>> QueryPagingAsync(QueryShipmentInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(QueryPagingAsync));

        var db = await this._shipmentRepository.GetDbContextAsync();

        var query = db.Set<Shipment>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(dto.OrderId))
            query = query.Where(x => x.OrderId == dto.OrderId);

        var count = await query.CountAsync();
        var list = await query.OrderByDescending(x => x.CreationTime).PageBy(dto.AsAbpPagedRequestDto()).ToArrayAsync();

        var response = list.Select(x => this.ObjectMapper.Map<Shipment, ShipmentDto>(x)).ToArray();

        return new PagedResponse<ShipmentDto>(response, dto, count);
    }

    public async Task<ShipmentDto[]> AttachDataAsync(ShipmentDto[] data, AttachDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._shipmentRepository.GetDbContextAsync();

        if (dto.Items)
        {
            var ids = data.Ids().ToArray();
            var query = db.Set<ShipmentOrderItem>().AsNoTracking();
            var list = await query.Where(x => ids.Contains(x.ShipmentId)).ToArrayAsync();

            foreach (var m in data)
            {
                m.Items = list.Where(x => x.ShipmentId == m.Id)
                    .Select(x => this.ObjectMapper.Map<ShipmentOrderItem, ShipmentOrderItemDto>(x))
                    .ToArray();
            }
        }

        return data;
    }

    public async Task<ShipmentOrderItemDto[]> AttachItemDataAsync(ShipmentOrderItemDto[] data, AttachItemDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._shipmentRepository.GetDbContextAsync();

        if (dto.OrderItem)
        {
            var ids = data.Select(x => x.OrderItemId).ToArray();
            var query = db.Set<OrderItem>().AsNoTracking();
            var list = await query.Where(x => ids.Contains(x.Id)).ToArrayAsync();

            foreach (var m in data)
            {
                m.OrderItem = list.Where(x => x.Id == m.OrderItemId)
                    .Select(x => this.ObjectMapper.Map<OrderItem, OrderItemDto>(x))
                    .FirstOrDefault();
            }
        }

        return data;
    }
}