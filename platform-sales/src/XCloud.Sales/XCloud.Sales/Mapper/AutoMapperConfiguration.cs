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
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Data.Domain.WarehouseStock;
using XCloud.Sales.Service.AfterSale;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Common;
using XCloud.Sales.Service.Configuration;
using XCloud.Sales.Service.Coupons;
using XCloud.Sales.Service.Finance;
using XCloud.Sales.Service.Logging;
using XCloud.Sales.Service.Media;
using XCloud.Sales.Service.Orders;
using XCloud.Sales.Service.Promotion;
using XCloud.Sales.Service.Shipping;
using XCloud.Sales.Service.ShoppingCart;
using XCloud.Sales.Service.Stores;
using XCloud.Sales.Service.Users;
using XCloud.Sales.Service.WarehouseStock;

namespace XCloud.Sales.Mapper;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        this.CreateMap<Store,StoreDto>().ReverseMap();
        this.CreateMap<StoreGoodsMapping, StoreGoodsMappingDto>().ReverseMap();
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

        this.CreateMap<Shipment, ShipmentDto>().ReverseMap();
        this.CreateMap<ShipmentOrderItem, ShipmentOrderItemDto>().ReverseMap();

        this.CreateMap<AfterSales, AfterSalesDto>().ReverseMap();
        this.CreateMap<AftersalesItem, AfterSalesItemDto>().ReverseMap();
        this.CreateMap<AfterSalesComment, AfterSalesCommentDto>().ReverseMap();

        this.CreateMap<PrepaidCard, PrepaidCardDto>().ReverseMap();

        this.CreateMap<Stock, StockDto>().ReverseMap();
        this.CreateMap<StockItem, StockItemDto>().ReverseMap();
        this.CreateMap<StockUsageHistory, StockUsageHistoryDto>().ReverseMap();

        this.CreateMap<Coupon, CouponDto>().ReverseMap();
        this.CreateMap<CouponUserMapping, CouponUserMappingDto>().ReverseMap();
        this.CreateMap<StorePromotion, StorePromotionDto>().ReverseMap();

        this.CreateMap<Supplier, SupplierDto>().ReverseMap();
        this.CreateMap<Warehouse, WarehouseDto>().ReverseMap();
    }
}