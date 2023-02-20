import { SxProps } from '@mui/material';

export interface IPageLink {
  goods?: number;
  coupon?: string;
  activity?: string;
  keywords?: string;
  path?: {
    pathname: string;
    query?: {};
  };
}

export const PageItemTypes = {
  video: 'video',
  slider: 'slider',
  title: 'title',
  search: 'search',
  picture: 'picture',
  alert: 'alert',
  content: 'content',
  goodsCollection: 'goods-collection',
};

export const AllPageItemTypes = [
  PageItemTypes.video,
  PageItemTypes.slider,
  PageItemTypes.title,
  PageItemTypes.search,
  PageItemTypes.picture,
  PageItemTypes.alert,
  PageItemTypes.content,
  PageItemTypes.goodsCollection,
];

export interface IPageItem {
  id?: number;
  type?:
    | string
    | 'video'
    | 'slider'
    | 'title'
    | 'search'
    | 'picture'
    | 'alert'
    | 'goods-collection'
    | 'content';
  sx?: SxProps;
}

export interface PageSlider {
  img?: string;
  text?: string;
  link?: IPageLink;
}

export interface IPageSliderItem extends IPageItem {
  sliders?: Array<PageSlider>;
}

export interface IPageSearchItem extends IPageItem {
  keywords?: Array<string>;
}

export interface IPagePictureItem extends IPageItem {
  picture?: StorageMetaDto;
  link?: IPageLink;
}

export interface IPageAlertItem extends IPageItem {
  alertType?: string | 'info' | 'error' | 'success' | 'warning';
  message?: string;
  description?: string;
  link?: IPageLink;
}

export interface IPageVideoItem extends IPageItem {
  url?: string;
}

export interface IPageGoodsCollectionItem extends IPageItem {
  displayType?: string;
  goodsIds?: Array<number>;
}

export interface IPageContentItem extends IPageItem {
  content?: string;
}

export interface IPageQuotaItem extends IPageItem {
  cite?: string;
  content?: string;
}

export interface IPageTitleItem extends IPageItem {
}

export interface IPageData {
  title?: string;
  desc?: string;
  items?: Array<
    | IPageItem
    | IPageSearchItem
    | IPageSliderItem
    | IPagePictureItem
    | IPageAlertItem
    | IPageVideoItem
    | IPageGoodsCollectionItem
    | IPageContentItem
    | IPageQuotaItem
    | IPageTitleItem
  >;
}

//-----------------------------------
export interface MallSettingsDto {
  HomePageNotice?: string;
  HomePageCategorySeoNames?: string;
  PlaceOrderNotice?: string;
  LoginNotice?: string;
  RegisterNotice?: string;
  GoodsDetailNotice?: string;
  PlaceOrderDisabled?: boolean;
  AftersaleDisabled?: boolean;
}

export interface CategoryDto {
  Id?: number;
  RootId?: number;
  SeoName?: string;
  Name?: string;
  Description?: string;
  ParentNodesIds?: any[];
  IsDeleted?: boolean;
  Published?: boolean;
  Recommend?: boolean;
  CreationTime?: string;
  LastModificationTime?: string;
}

export interface BrandDto {
  Id?: number;
  Name?: string;
  Description?: string;
  IsDeleted?: boolean;
  Published?: boolean;
  CreationTime?: string;
  LastModificationTime?: string;
  Picture?: StorageMetaDto;
}

export interface TagDto {
  Id?: string;
  Name?: string;
  Description?: string;
  IsDeleted?: boolean;
  Alert?: string;
  Link?: string;
}

/*
public string UserId { get; set; }

    public double? Lat { get; set; }
    public double? Lng { get; set; }

    public string NationCode { get; set; }
    public string Nation { get; set; }

    public string ProvinceCode { get; set; }
    public string Province { get; set; }

    public string CityCode { get; set; }
    public string City { get; set; }

    public string AreaCode { get; set; }
    public string Area { get; set; }

* */
export interface AddressDto {
  Id?: string;
  UserId?: string;
  Tel?: string;
  Name?: string;
  Lat?: number;
  Lng?: number;
  NationCode?: string;
  Nation?: string;
  ProvinceCode?: string;
  Province?: string;
  CityCode?: string;
  City?: string;
  AreaCode?: string;
  Area?: string;
  AddressDescription?: string;
  AddressDetail?: string;
  PostalCode?: string;
  IsDefault?: boolean;
  CreationTime?: string;
}

export interface StorageMetaDto {
  Id?: string;
  Url?: string;
  ContentType?: string;
  ResourceSize?: number;
  ResourceKey?: string;
  ResourceHash?: string;
  HashType?: string;
  ExtraData?: string;
  StorageProvider?: string;
}

export interface MallStorageMetaDto extends StorageMetaDto {
  //
  PictureId?: number;
  GoodsId?: number;
  CombinationId?: number;
  DisplayOrder?: number;
}

export interface PictureDto {
  Id?: number;
  StorageMeta?: StorageMetaDto;
  ResourceData?: string;
  ResourceId?: string;
  //
  GoodsId?: number;
  CombinationId?: number;
  DisplayOrder?: number;
}

export interface SearchGoodsInputDto {
  Initial?: boolean;
  Page?: number;
  StockQuantityLessThanOrEqualTo?: number;
  StockQuantityGreaterThanOrEqualTo?: number;
  IsNew?: boolean;
  IsHot?: boolean;
  WithoutBrand?: boolean;
  WithoutCategory?: boolean;
  TagId?: string;
  CategoryId?: number;
  BrandId?: number;
  PriceMin?: number;
  PriceMax?: number;
  Keywords?: string;
  OrderBy?: number;
  IsPublished?: boolean;
  Sku?: string;
  IsDeleted?: boolean;
}

export interface GoodsDto {
  Id?: number;
  Name?: string;
  FullDescription?: string;
  Brand?: BrandDto;
  Category?: CategoryDto;
  Tags?: TagDto[];
  GoodsSpecCombinations?: GoodsCombinationDto[];
  IsFavorite?: boolean;
  PriceIsHidden?: boolean;
  Published?: boolean;
  GoodsAttributes?: any[];
  XPictures?: MallStorageMetaDto[];
  AdminComment?: string;
  StickyTop?: boolean;
  IsNew?: boolean;
  IsHot?: boolean;
  AttributeType?: number;
  Price?: number;
  CostPrice?: number;
  StockQuantity?: number;
}

export interface GoodsSpecDto {
  Id?: number;
  Name?: string;
  Values?: GoodsSpecValueDto[];
}

export interface GoodsSpecValueDto {
  Id?: number;
  GoodsSpecId?: number;
  Name?: string;
}

export interface GoodsSpecCombinationItemDto {
  SpecId?: number;
  SpecValueId?: number;
  Spec?: GoodsSpecDto;
  SpecValue?: GoodsSpecValueDto;
}

export interface GoodsCombinationDto {
  Id?: number;
  GoodsId?: number;
  Name?: string;
  Sku?: string;
  Price?: number;
  CostPrice?: number;
  StockQuantity?: number;
  GoodsName?: string;
  GoodsShortDescription?: string;
  Goods?: GoodsDto;
  GradeId?: string;
  GradeName?: string;
  GradePrice?: number;
  AllGradePrices?: GoodsGradePriceDto[];
  IsActive?: boolean;
  IsDeleted?: boolean;
  PriceIsHidden?: boolean;
  FinalPrice?: number;
  Description?: string;
  SpecificationsJson?: string;
  ParsedSpecificationsJson?: GoodsSpecCombinationItemDto[];
  SpecCombinationErrors?: string[];
  Stores?: StoreDto[];
  XPictures?: MallStorageMetaDto[];
}

export interface GoodsGradePriceDto {
  Id?: string;
  GoodsId?: number;
  GoodsCombinationId?: number;
  GradeId?: string;
  Price?: number;
  CreationTime?: string;
  Goods?: GoodsDto;
  GoodsSpecCombination?: GoodsCombinationDto;
  GradeName?: string;
  GradeDescription?: string;
}

export interface UserGradeDto {
  Id?: string;
  Name?: string;
}

export interface IRoute {
  name?: string;
  icon?: any;
  path?: string;
  component?: string;
  redirect?: string;
  hideInMenu?: boolean;
  hideChildrenInMenu?: boolean;
  routes?: IRoute[];
}

export interface SysUserDto {
  Id?: string;
  UserId?: string;
  IdentityName?: string;
  OriginIdentityName?: string;
  NickName?: string;
  Avatar?: string;
  AccountMobile?: string;
  AdminIdentity?: SysAdminDto;
}

export interface SysAdminDto {
  Id?: string;
  UserId?: string;
  /**
   * @deprecated
   */
  AdminId?: string;
  /**
   * @deprecated
   */
  NickName?: string;
  /**
   * @deprecated
   */
  Avatar?: string;
  SysUser?: SysUserDto;
  IsActive?: boolean;
}

export interface MallUserDto {
  Id?: number;
  GlobalUserId?: string;
  NickName?: string;
  Avatar?: string;
  AccountMobile?: string;
  Balance?: number;
  Points?: number;
  HistoryPoints?: number;
  Active?: boolean;
  LastActivityTime?: string;
  IsDeleted?: boolean;

  GradeId?: string;
  GradeName?: string;
  Grade?: UserGradeDto;
  SysUser?: SysUserDto;
}

export interface ActivityLogDto {
  Id?: number;
  ActivityLogTypeId?: number;
  UserId?: number;
  AdministratorId?: string;
  Comment?: string;
  Value?: string;
  Data?: string;
  UrlReferrer?: string;
  BrowserType?: string;
  Device?: string;
  UserAgent?: string;
  IpAddress?: string;
  RequestPath?: string;
  SubjectType?: string;
  SubjectId?: string;
  SubjectIntId?: number;
  CreationTime?: string;
  Admin?: SysAdminDto;
  User?: MallUserDto;
  GeoCountry?: string;
  GeoCity?: string;
  Lat?: number;
  Lng?: number;
}

export interface OrderItemDto {
  Goods?: GoodsDto;
  GoodsSpecCombination?: GoodsCombinationDto;
  OrderId?: string;
  Id?: number;
  Quantity?: number;
  UnitPrice?: number;
  Price?: number;
  GradePriceOffset?: number;
}

export interface StoreDto {
  Id?: string;
  StoreName?: string;
}

export interface OrderBillRefundDto {
}

/*

    public string OrderId { get; set; }
    public decimal Price { get; set; }
*/

export interface OrderBillDto {
  Id?: string;

  OrderId?: string;
  Price?: number;

  PaymentMethod?: number;

  Paid?: boolean;
  PayTime?: string;
  PaymentTransactionId?: string;
  NotifyData?: string;

  Refunded?: boolean;
  RefundTime?: string;
  RefundTransactionId?: string;
  RefundNotifyData?: string;
  IsDeleted?: boolean;
  CreationTime?: string;

  Order?: OrderDto;
  OrderRefundBill?: OrderBillRefundDto;
}

export interface OrderDto {
  Id?: string;
  StoreId?: string;
  OrderSn?: string;
  OrderStatusId?: number;
  ShippingStatusId?: number;
  PaymentStatusId?: number;
  IsAftersales?: boolean;
  AfterSalesId?: string;
  OrderTotal?: number;
  OrderSubtotal?: number;
  Remark?: string;
  HideForAdmin?: boolean;
  IsDeleted?: boolean;
  CreationTime?: string;
  LastModificationTime?: string;
  UserId?: number;
  User?: MallUserDto;
  Items?: OrderItemDto[];
  /**
   * @deprecated
   */
  ShippingRequired?: boolean;
  AfterSales?: AfterSaleDto;
  ShippingTime?: string;
  PaidTime?: string;
  ShippingAddressContactName?: string;
  ShippingAddressContact?: string;
  ShippingAddressDetail?: string;
}

export interface AfterSalesItemDto {
  OrderItem?: OrderItemDto;
  AftersalesId?: string;
  OrderId?: string;
  OrderItemId?: number;
  Quantity?: number;
}

export interface AfterSaleDto {
  Id?: string;
  UserId?: number;
  OrderId?: string;
  User?: MallUserDto;
  Items?: AfterSalesItemDto[];
  Order?: OrderDto;
  AfterSalesStatusId?: number;
  ReasonForReturn?: string;
  RequestedAction?: string;
  CreationTime?: string;
}

export interface CouponDto {
  Id?: number;
  Title?: string;
  Value?: number;
  Amount?: number;
  MinimumConsumption?: number;
  ExpiredDaysFromIssue?: number;
  StartTime?: string;
  EndTime?: string;
  IsAmountLimit?: boolean;
  AccountIssuedLimitCount?: number;
  IssuedAmount?: number;
  UsedAmount?: number;
  CreationTime?: string;
  IsDeleted?: boolean;
  CanBeIssued?: boolean;
}

export interface UserCouponDto {
  Id?: string;
  UserId?: number;
  CouponId?: number;
  IsUsed?: boolean;
  UsedTime?: string;
  Value?: number;
  MinimumConsumption?: number;
  ExpiredAt?: string;
  CreationTime?: string;

  Coupon?: CouponDto;
  User?: MallUserDto;
}

export interface PromotionDto {
  Id?: string;
  Name?: string;
  Description?: string;
  IsExclusive?: boolean;
  Condition?: string;
  Result?: string;
  Order?: string;
  StartTime?: string;
  EndTime?: string;
  IsActive?: boolean;
  IsDeleted?: boolean;
  CreationTime?: string;

  PromotionConditions?: any[];
  PromotionResults?: any[];
}

export interface PromotionConditionDto {
  ConditionType?: string;
  ConditionJson?: string;
}

export interface PromotionResultDto {
  ResultType?: string;
  ResultJson?: string;
}

export interface Permission {
  name: string;
  key: string;
}

export interface PermissionGroup {
  name: string;
  permissions: Permission[];
}

export interface NotificationDto {
  Id?: string;
  Title?: string;
  Content?: string;
  App?: string;
  UserId?: string;
  SenderId?: string;
  SenderType?: string;
  ActionType?: string;
  Data?: string;
  Read?: boolean;
  ReadTime?: string;
  CreationTime?: string;
}

export interface ApiResponse<T> {
  Error?: {
    Message?: string;
    Code?: string;
  };
  Data?: T;
}

export interface PagedResponse<T> extends ApiResponse<T> {
  Items?: T[];
  TotalPages?: number;
  PageSize?: number;
  PageIndex?: number;
  TotalCount?: number;
}

export interface StockDto {
  Id?: string;
  No?: string;
  SupplierId?: string;
  WarehouseId?: string;
  Remark?: string;
  Approved?: string;
  ApprovedTime?: string;
  ExpirationTime?: string;
  CreationTime?: string;
  Items?: StockItemDto[];
  Supplier?: SupplierDto;
  Warehouse?: WarehouseDto;
}

export interface StockItemDto {
  Id?: string;
  WarehouseStockId?: string;
  GoodsId?: number;
  CombinationId?: number;
  Quantity?: number;
  Price?: number;
  DeductQuantity?: number;
  RuningOut?: boolean;
  LastModificationTime?: string;
  WarehouseStock?: StockDto;
  Goods?: GoodsDto;
  Combination?: GoodsCombinationDto;
}

export interface SupplierDto {
  Id?: string;
  Name?: string;
}

export interface WarehouseDto {
  Id?: string;
  Name?: string;
  Address?: string;
}

export interface RoleDto {
  Id?: string;
  Name?: string;
  Description?: string;
  PermissionKeys?: string[];
}
