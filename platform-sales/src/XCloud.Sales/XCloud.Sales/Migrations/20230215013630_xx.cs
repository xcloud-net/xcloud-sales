using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XCloud.Sales.Migrations
{
    public partial class xx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityLogTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdministratorId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Value = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    UrlReferrer = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    BrowserType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Device = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    GeoCountry = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    GeoCity = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Lng = table.Column<double>(type: "REAL", nullable: true),
                    Lat = table.Column<double>(type: "REAL", nullable: true),
                    RequestPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SubjectType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SubjectId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SubjectIntId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AfterSales",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReasonForReturn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    RequestedAction = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    UserComments = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    StaffNotes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Images = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AfterSalesStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁"),
                    HideForAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AfterSales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AfterSalesComment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AfterSaleId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    PictureJson = table.Column<string>(type: "TEXT", nullable: true),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AfterSalesComment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AftersalesItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AftersalesId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AftersalesItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BalanceHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Balance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    LatestBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ActionType = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ShowOnPublicPage = table.Column<bool>(type: "INTEGER", nullable: false),
                    MetaKeywords = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    MetaDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    PictureId = table.Column<int>(type: "INTEGER", nullable: false),
                    PageSize = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceRanges = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "删除时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SeoName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    RootId = table.Column<int>(type: "INTEGER", nullable: false),
                    NodePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ParentCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    PictureId = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceRanges = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    ShowOnHomePage = table.Column<bool>(type: "INTEGER", nullable: false),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "删除时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Recommend = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MinimumConsumption = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ExpiredDaysFromIssue = table.Column<int>(type: "INTEGER", nullable: true),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAmountLimit = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccountIssuedLimitCount = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuedAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    UsedAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CouponUserMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CouponId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsUsed = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Value = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MinimumConsumption = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponUserMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    StickyTop = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsNew = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHot = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeoName = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    ShortDescription = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Keywords = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FullDescription = table.Column<string>(type: "TEXT", nullable: true),
                    AdminComment = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AttributeType = table.Column<int>(type: "INTEGER", nullable: false),
                    StockQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    SaleVolume = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAmountInOnePurchase = table.Column<int>(type: "INTEGER", nullable: false),
                    CostPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MinPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MaxPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    BrandId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApprovedReviewsRates = table.Column<double>(type: "REAL", nullable: false),
                    ApprovedTotalReviews = table.Column<int>(type: "INTEGER", nullable: false),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsAttribute",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsAttribute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsCollection",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Keywords = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ApplyedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁"),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "删除时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsCollection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsCollectionItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CollectionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoodsSpecCombinationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsCollectionItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsGradePrice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoodsCombinationId = table.Column<int>(type: "INTEGER", nullable: false),
                    GradeId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsGradePrice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsPicture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    CombinationId = table.Column<int>(type: "INTEGER", nullable: false),
                    PictureId = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsPicture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsPriceHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoodsSpecCombinationId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsPriceHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ReviewText = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsSpecCombination",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Sku = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CostPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: false),
                    StockQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PictureId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpecificationsJson = table.Column<string>(type: "TEXT", nullable: true, comment: "规格配置参数"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "删除时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsSpecCombination", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocaleStringResource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocaleStringResource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    StoreId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OrderSn = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", maxLength: 100, nullable: false),
                    ShippingRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShippingTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ShippingAddressId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ShippingAddressProvice = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ShippingAddressCity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ShippingAddressArea = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ShippingAddressDetail = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ShippingAddressContactName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ShippingAddressContact = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OrderStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShippingStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAftersales = table.Column<bool>(type: "INTEGER", nullable: false),
                    AfterSalesId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    GradeId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    GradePriceOffsetTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    OrderTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    OrderSubtotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    OrderShippingFee = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CouponId = table.Column<int>(type: "INTEGER", nullable: true),
                    CouponDiscount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PromotionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PromotionDiscount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ExchangePointsAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    RefundedAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RewardPointsHistoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    Remark = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AffiliateId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderIp = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PaidTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OverPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HideForAdmin = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderBill",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "INTEGER", nullable: false),
                    Paid = table.Column<bool>(type: "INTEGER", nullable: false),
                    PayTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaymentTransactionId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NotifyData = table.Column<string>(type: "TEXT", nullable: true),
                    Refunded = table.Column<bool>(type: "INTEGER", nullable: true),
                    RefundTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RefundTransactionId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    RefundNotifyData = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBill", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoodsSpecCombinationId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoodsName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    GradePriceOffset = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ItemWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderNote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DisplayToUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderNote", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderRefundBill",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BillId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Refunded = table.Column<bool>(type: "INTEGER", nullable: true),
                    RefundTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RefundTransactionId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    RefundNotifyData = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRefundBill", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SeoName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CoverImageResourceData = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    BodyContent = table.Column<string>(type: "TEXT", nullable: true),
                    ReadCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    PublishedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PagesReader",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagesReader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    SeoFilename = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ResourceId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ResourceData = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointsHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    PointsBalance = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionType = table.Column<int>(type: "INTEGER", nullable: false),
                    UsedAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrepaidCard",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Used = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrepaidCard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shipment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ShippingMethod = table.Column<string>(type: "TEXT", nullable: true),
                    ExpressName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TrackingNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TotalWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    ShippedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeliveryTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentOrderItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ShipmentId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentOrderItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingMethod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoodsSpecCombinationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spec",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spec", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoodsSpecId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    PriceOffset = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecValue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    StoreName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StoreUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    StoreLogo = table.Column<int>(type: "INTEGER", maxLength: 1000, nullable: false),
                    CopyrightInfo = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ICPRecord = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    StoreServiceTime = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ServiceTelePhone = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    StoreClosed = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreGoodsMapping",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    StoreId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GoodsCombinationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreGoodsMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreManager",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    StoreId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GlobalUserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NickName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreManager", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorePromotion",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsExclusive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Condition = table.Column<string>(type: "TEXT", nullable: true),
                    Result = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorePromotion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContactName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Telephone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Alert = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Link = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagGoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TagId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagGoods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GlobalUserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NickName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AccountMobile = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Balance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    HistoryPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "删除时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastActivityTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGrade",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    UserCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGrade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGradeMapping",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    GradeId = table.Column<string>(type: "TEXT", nullable: true),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGradeMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Lat = table.Column<double>(type: "REAL", nullable: true),
                    Lng = table.Column<double>(type: "REAL", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseStock",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    No = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SupplierId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    WarehouseId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Remark = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Approved = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ApprovedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseStock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseStockItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    WarehouseStockId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GoodsId = table.Column<int>(type: "INTEGER", nullable: false),
                    CombinationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DeductQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    RuningOut = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseStockItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AfterSales_CreationTime",
                table: "AfterSales",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AfterSales_IsDeleted",
                table: "AfterSales",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AfterSales_LastModificationTime",
                table: "AfterSales",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AfterSalesComment_CreationTime",
                table: "AfterSalesComment",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceHistory_CreationTime",
                table: "BalanceHistory",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Brand_IsDeleted",
                table: "Brand",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Category_IsDeleted",
                table: "Category",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_CreationTime",
                table: "Coupon",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_IsDeleted",
                table: "Coupon",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUserMapping_CreationTime",
                table: "CouponUserMapping",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_IsDeleted",
                table: "Goods",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsAttribute_CreationTime",
                table: "GoodsAttribute",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsCollection_CreationTime",
                table: "GoodsCollection",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsCollection_IsDeleted",
                table: "GoodsCollection",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsCollection_LastModificationTime",
                table: "GoodsCollection",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsCollectionItem_CreationTime",
                table: "GoodsCollectionItem",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsGradePrice_CreationTime",
                table: "GoodsGradePrice",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsPriceHistory_CreationTime",
                table: "GoodsPriceHistory",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsSpecCombination_CreationTime",
                table: "GoodsSpecCombination",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsSpecCombination_IsDeleted",
                table: "GoodsSpecCombination",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsSpecCombination_LastModificationTime",
                table: "GoodsSpecCombination",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IsDeleted",
                table: "Order",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Order_LastModificationTime",
                table: "Order",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBill_CreationTime",
                table: "OrderBill",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBill_IsDeleted",
                table: "OrderBill",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderNote_CreationTime",
                table: "OrderNote",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_CreationTime",
                table: "Pages",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_IsDeleted",
                table: "Pages",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PagesReader_CreationTime",
                table: "PagesReader",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_PointsHistory_CreationTime",
                table: "PointsHistory",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_PrepaidCard_CreationTime",
                table: "PrepaidCard",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_PrepaidCard_IsDeleted",
                table: "PrepaidCard",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Spec_IsDeleted",
                table: "Spec",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SpecValue_IsDeleted",
                table: "SpecValue",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_StorePromotion_CreationTime",
                table: "StorePromotion",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_StorePromotion_IsDeleted",
                table: "StorePromotion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_CreationTime",
                table: "Supplier",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_IsDeleted",
                table: "Supplier",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_IsDeleted",
                table: "Tag",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_User_IsDeleted",
                table: "User",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_User_LastModificationTime",
                table: "User",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_UserGrade_IsDeleted",
                table: "UserGrade",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserGradeMapping_CreationTime",
                table: "UserGradeMapping",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_CreationTime",
                table: "Warehouse",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_IsDeleted",
                table: "Warehouse",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_CreationTime",
                table: "WarehouseStock",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStockItem_LastModificationTime",
                table: "WarehouseStockItem",
                column: "LastModificationTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLog");

            migrationBuilder.DropTable(
                name: "AfterSales");

            migrationBuilder.DropTable(
                name: "AfterSalesComment");

            migrationBuilder.DropTable(
                name: "AftersalesItem");

            migrationBuilder.DropTable(
                name: "BalanceHistory");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "CouponUserMapping");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "GoodsAttribute");

            migrationBuilder.DropTable(
                name: "GoodsCollection");

            migrationBuilder.DropTable(
                name: "GoodsCollectionItem");

            migrationBuilder.DropTable(
                name: "GoodsGradePrice");

            migrationBuilder.DropTable(
                name: "GoodsPicture");

            migrationBuilder.DropTable(
                name: "GoodsPriceHistory");

            migrationBuilder.DropTable(
                name: "GoodsReview");

            migrationBuilder.DropTable(
                name: "GoodsSpecCombination");

            migrationBuilder.DropTable(
                name: "LocaleStringResource");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "OrderBill");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "OrderNote");

            migrationBuilder.DropTable(
                name: "OrderRefundBill");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "PagesReader");

            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropTable(
                name: "PointsHistory");

            migrationBuilder.DropTable(
                name: "PrepaidCard");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "Shipment");

            migrationBuilder.DropTable(
                name: "ShipmentOrderItem");

            migrationBuilder.DropTable(
                name: "ShippingMethod");

            migrationBuilder.DropTable(
                name: "ShoppingCartItem");

            migrationBuilder.DropTable(
                name: "Spec");

            migrationBuilder.DropTable(
                name: "SpecValue");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "StoreGoodsMapping");

            migrationBuilder.DropTable(
                name: "StoreManager");

            migrationBuilder.DropTable(
                name: "StorePromotion");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "TagGoods");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserGrade");

            migrationBuilder.DropTable(
                name: "UserGradeMapping");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "WarehouseStock");

            migrationBuilder.DropTable(
                name: "WarehouseStockItem");
        }
    }
}
