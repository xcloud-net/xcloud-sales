using AutoMapper;
using XCloud.Platform.Shared.Dto;
using XCloud.Sales.Data.Domain.Aftersale;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Common;
using XCloud.Sales.Data.Domain.Configuration;
using XCloud.Sales.Data.Domain.Coupons;
using XCloud.Sales.Data.Domain.Finance;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Data.Domain.Media;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Promotion;
using XCloud.Sales.Data.Domain.Shipping;
using XCloud.Sales.Data.Domain.Stock;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Services.AfterSale;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Common;
using XCloud.Sales.Services.Configuration;
using XCloud.Sales.Services.Coupons;
using XCloud.Sales.Services.Finance;
using XCloud.Sales.Services.Logging;
using XCloud.Sales.Services.Media;
using XCloud.Sales.Services.Orders;
using XCloud.Sales.Services.Promotion;
using XCloud.Sales.Services.Shipping;
using XCloud.Sales.Services.ShoppingCart;
using XCloud.Sales.Services.Stock;
using XCloud.Sales.Services.Stores;
using XCloud.Sales.Services.Users;

namespace XCloud.Sales.Services;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        this.CreateMap<Store,StoreDto>().ReverseMap();
        this.CreateMap<Picture, PictureDto>().ReverseMap();
        this.CreateMap<StorageMetaDto, MallStorageMetaDto>().ReverseMap();
        
        this.CreateMap<ActivityLog, ActivityLogDto>().ReverseMap();

        this.CreateMap<Brand, BrandDto>().ReverseMap();
        this.CreateMap<Category, CategoryDto>().ReverseMap();
        this.CreateMap<Tag, TagDto>().ReverseMap();
        this.CreateMap<Goods, GoodsDto>().ReverseMap();

        this.CreateMap<GoodsSpecCombination, GoodsSpecCombinationDto>().ReverseMap();

        this.CreateMap<Spec, SpecDto>().ReverseMap();
        this.CreateMap<SpecValue, SpecValueDto>().ReverseMap();

        this.CreateMap<GoodsGradePrice, GoodsGradePriceDto>().ReverseMap();

        this.CreateMap<GoodsCollection, GoodsCollectionDto>().ReverseMap();
        this.CreateMap<GoodsCollectionItem, GoodsCollectionItemDto>().ReverseMap();

        this.CreateMap<Pages, PagesDto>().ReverseMap();

        this.CreateMap<UserGradeMappingDto, UserGradeMapping>().ReverseMap();

        this.CreateMap<Order, OrderDto>().ReverseMap();
        this.CreateMap<OrderItem, OrderItemDto>().ReverseMap();

        this.CreateMap<User, StoreUserDto>().ReverseMap();
        this.CreateMap<PointsHistory, PointsHistoryDto>().ReverseMap();
        this.CreateMap<BalanceHistory, BalanceHistoryDto>().ReverseMap();
        this.CreateMap<StoreManager, StoreManagerDto>().ReverseMap();

        this.CreateMap<ShoppingCartItem, ShoppingCartItemDto>().ReverseMap();

        this.CreateMap<Setting, SettingDto>().ReverseMap();
        
        this.CreateMap<Favorites, FavoritesDto>().ReverseMap();

        this.CreateMap<OrderNote, OrderNoteDto>().ReverseMap();
        this.CreateMap<OrderBill, OrderBillDto>().ReverseMap();
        this.CreateMap<OrderRefundBill, OrderRefundBillDto>().ReverseMap();

        this.CreateMap<Shipment, ShipmentDto>().ReverseMap();
        this.CreateMap<ShipmentOrderItem, ShipmentOrderItemDto>().ReverseMap();

        this.CreateMap<AfterSales, AfterSalesDto>().ReverseMap();
        this.CreateMap<AftersalesItem, AfterSalesItemDto>().ReverseMap();
        this.CreateMap<AfterSalesComment, AfterSalesCommentDto>().ReverseMap();

        this.CreateMap<PrepaidCard, PrepaidCardDto>().ReverseMap();

        this.CreateMap<WarehouseStock, WarehouseStockDto>().ReverseMap();
        this.CreateMap<WarehouseStockItem, WarehouseStockItemDto>().ReverseMap();

        this.CreateMap<Coupon, CouponDto>().ReverseMap();
        this.CreateMap<CouponUserMapping, CouponUserMappingDto>().ReverseMap();
        this.CreateMap<StorePromotion, StorePromotionDto>().ReverseMap();

        this.CreateMap<Supplier, SupplierDto>().ReverseMap();
        this.CreateMap<Warehouse, WarehouseDto>().ReverseMap();
    }
}