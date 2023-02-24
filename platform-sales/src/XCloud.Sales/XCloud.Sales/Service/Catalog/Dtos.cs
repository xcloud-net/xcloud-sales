using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Media;
using XCloud.Sales.Service.Stores;

namespace XCloud.Sales.Service.Catalog;

public class QueryTagPagingInput : PagedRequest
{
    //
}

public class FormatGoodsSpecCombinationInput : IEntityDto<int>
{
    public int Id { get; set; }
}

public class IdWithIndex : IdDto<int>
{
    public int Index { get; set; }
}

public class SaveGoodsImagesInput : IEntityDto<int>
{
    public int Id { get; set; }
    public int CombinationId { get; set; }
    public IdWithIndex[] PictureIdArray { get; set; }
}

public class UpdateGoodsStatusInput : IEntityDto
{
    public int GoodsId { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? Published { get; set; }
    public bool? IsNew { get; set; }
    public bool? IsHot { get; set; }
    public bool? StickyTop { get; set; }
}

public class SaveAttributesInput : IEntityDto
{
    public int GoodsId { get; set; }
    public GoodsAttribute[] GoodsAttributes { get; set; }
}

public class AttachGoodsDataInput : IEntityDto
{
    public bool Brand { get; set; }
    public bool Category { get; set; }
    public bool Images { get; set; }
    public bool Combination { get; set; }
    public bool GoodsAttributes { get; set; }
    public bool Tags { get; set; }
}

public class SpecGroupSelectResponse : IEntityDto
{
    public GoodsSpecCombinationDto[] FilteredCombinations { get; set; }
    public SpecGroup[] SpecGroup { get; set; }
}

public class SpecGroup : IEntityDto<int>
{
    public SpecGroup()
    {
        //
    }

    public SpecGroup(Spec goodsSpec) : this()
    {
        this.Id = goodsSpec.Id;
        this.Name = goodsSpec.Name;
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public SpecGroupItem[] Items { get; set; }
}

public class SpecGroupItem : IEntityDto<int>
{
    public SpecGroupItem()
    {
        //
    }

    public SpecGroupItem(SpecValue goodsSpec) : this()
    {
        this.Id = goodsSpec.Id;
        this.Name = goodsSpec.Name;
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public bool IsSelected { get; set; }
    public bool IsActive { get; set; }
}

public class QueryCombinationInput : IEntityDto
{
    public int GoodsId { get; set; }

    public SpecCombinationItemDto[] Specs { get; set; }
}

public class QueryGoodsReviewInput : PagedRequest, IEntityDto
{
    public string StoreId { get; set; }
    public int? GoodsId { get; set; }
    public string OrderId { get; set; }
    public int? UserId { get; set; }
    public bool? IsApproved { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class SetGoodsTagsInput : IEntityDto<int>
{
    public int Id { get; set; }
    public string[] TagIds { get; set; }
}

public class QueryCombinationForSelectionInput : IEntityDto
{
    public int Take { get; set; }
    public int[] ExcludedCombinationIds { get; set; }
    public string Keywords { get; set; }
}

public class SearchOptionsDto : IEntityDto
{
    public BrandDto Brand { get; set; }
    public CategoryDto Category { get; set; }
    public Tag Tag { get; set; }
}

public class QueryGoodsCombinationInput : PagedRequest, IEntityDto
{
    public QueryGoodsCombinationInput()
    {
        this.IsDeleted = false;
    }

    public int? StockQuantityLessThanOrEqualTo { get; set; }
    public int? StockQuantityGreaterThanOrEqualTo { get; set; }

    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public string Sku { get; set; }

    public string Keywords { get; set; }

    public bool? IsDeleted { get; set; } = false;
}

public class SearchProductsInput : PagedRequest, IEntityDto
{
    public SearchProductsInput()
    {
        this.IsDeleted = false;
    }

    public int? StockQuantityLessThanOrEqualTo { get; set; }
    public int? StockQuantityGreaterThanOrEqualTo { get; set; }

    public bool? IsNew { get; set; }
    public bool? IsHot { get; set; }

    public bool? WithoutBrand { get; set; }
    public bool? WithoutCategory { get; set; }
    public string TagId { get; set; }
    public string StoreId { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public string Keywords { get; set; }
    public GoodsSortingEnum OrderBy { get; set; }
    public bool? IsPublished { get; set; }
    public string Sku { get; set; }

    public AttachGoodsDataInput AttachDataOptions { get; set; }

    public bool? IsDeleted { get; set; } = false;
}

public class UpdateBrandStatusInput : IEntityDto
{
    public int BrandId { get; set; }
    public bool? Published { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? ShowOnPublicPage { get; set; }
}

public class AttachBrandDataInput : IEntityDto
{
    public AttachBrandDataInput()
    {
        this.Picture = false;
    }

    public bool Picture { get; set; }
}

public class SetCategoryPictureIdInput : IEntityDto<int>
{
    public int Id { get; set; }
    public int PictureId { get; set; }
}

public class SetBrandPictureIdInput : IEntityDto<int>
{
    public int Id { get; set; }
    public int PictureId { get; set; }
}

public class QueryBrandDto : PagedRequest
{
    public string StoreId { get; set; }
    public string Name { get; set; }
    public bool Published { get; set; }
    public bool ShowOnHomePage { get; set; }
    public bool? IsDeleted { get; set; } = false;
}

public class CategoryAttachDataInput : IEntityDto
{
    public bool Picture { get; set; }
    public bool ParentNodes { get; set; }
}

public class UpdateCategoryStatusInput : IEntityDto
{
    public int CategoryId { get; set; }
    public bool? Published { get; set; }
    public bool? ShowOnHomePage { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? Recommend { get; set; }
}

public class BrandDto : Brand, IEntityDto<int>
{
    public MallStorageMetaDto Picture { get; set; }
}

public class CategoryDto : Category, IEntityDto
{
    public int[] ParentNodesIds { get; set; }

    public Category[] ParentNodes { get; set; }

    public MallStorageMetaDto Picture { get; set; }
}

public class UpdateGoodsSpecCombinationStatusInput : IEntityDto<int>
{
    public int Id { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsActive { get; set; }
}

public class SpecValueDto : SpecValue, IEntityDto
{
    public SpecDto Spec { get; set; }
}

public class SpecDto : Spec, IEntityDto
{
    public SpecValue[] Values { get; set; }
}

public class SetGoodsSeoNameInput : IEntityDto
{
    public int GoodsId { get; set; }
    public string SeoName { get; set; }
}

public static class AttributeTypeEnum
{
    public const int Simple = 0;
    public const int Combination = 1;
}

public class GoodsDto : Goods, IEntityDto
{
    public bool? IsFavorite { get; set; }

    public Brand Brand { get; set; }

    public Category Category { get; set; }

    public GoodsAttribute[] GoodsAttributes { get; set; }

    public MallStorageMetaDto[] XPictures { get; set; }
    
    [Obsolete]
    public PictureDto[] Pictures { get; set; }

    public Tag[] Tags { get; set; }

    public GoodsSpecCombinationDto[] GoodsSpecCombinations { get; set; }

    public bool? PriceIsHidden { get; set; }

    public bool SimpleAttribute => this.AttributeType == AttributeTypeEnum.Simple;

    public GoodsDto HidePrice()
    {
        this.PriceIsHidden = true;

        this.MaxPrice = default;
        this.MinPrice = default;
        this.Price = default;
        this.CostPrice = default;

        if (this.GoodsSpecCombinations != null)
            foreach (var m in this.GoodsSpecCombinations)
                m.HidePrice();

        return this;
    }

    public GoodsDto HideDetail()
    {
        this.FullDescription = null;
        return this;
    }
}

public class UpdateGoodsPriceDto : IEntityDto<int>
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public string Comment { get; set; }
}

public class UpdateGoodsCostPriceDto : IEntityDto<int>
{
    public int Id { get; set; }
    public decimal CostPrice { get; set; }
}

public class GoodsCombinationAttachDataInput : IEntityDto
{
    public bool Stores { get; set; } = false;

    public bool GradePrices { get; set; } = false;

    public bool DeserializeSpecCombinationJson { get; set; } = false;

    public bool SpecCombinationDetail { get; set; } = false;

    public bool CalculateSpecCombinationErrors { get; set; } = false;

    public bool Images { get; set; } = false;

    public bool Goods { get; set; } = false;
}

public class CombinationBySkuInput : IEntityDto
{
    public string Sku { get; set; }
    public int? UserId { get; set; }
}

public class SpecCombinationItemDto : IEntityDto
{
    public SpecCombinationItemDto()
    {
        //
    }

    public SpecCombinationItemDto(int specId, int specValueId)
    {
        this.SpecId = specId;
        this.SpecValueId = specValueId;
    }

    public int SpecId { get; set; }
    public int SpecValueId { get; set; }

    public SpecDto Spec { get; set; }
    public SpecValueDto SpecValue { get; set; }
}

public class GoodsSpecCombinationDto : GoodsSpecCombination, IEntityDto
{
    public StoreDto[] Stores { get; set; }

    public GoodsDto Goods { get; set; }

    public MallStorageMetaDto[] XPictures { get; set; }

    public string GoodsName => this.Goods?.Name;

    public string GoodsShortDescription => this.Goods?.ShortDescription;

    public SpecCombinationItemDto[] ParsedSpecificationsJson { get; set; }

    public string[] SpecCombinationErrors { get; set; }

    public GoodsGradePriceDto SelectedGradePrice { get; set; }

    public GoodsGradePriceDto[] GradePriceToSave { get; set; }

    public GoodsGradePriceDto[] AllGradePrices { get; set; }

    public string GradeId => this.SelectedGradePrice?.GradeId;

    public string GradeName => this.SelectedGradePrice?.GradeName;

    public decimal? GradePrice => this.SelectedGradePrice?.Price;

    public decimal FinalPrice => this.GradePrice ?? this.Price;

    public bool? PriceIsHidden { get; set; }

    public GoodsSpecCombinationDto HidePrice()
    {
        this.PriceIsHidden = true;
        this.Price = default;
        this.CostPrice = default;
        this.SelectedGradePrice = default;
        return this;
    }
}

public class GoodsGradePriceAttachDataInput : IEntityDto
{
    public bool GoodsInfo { get; set; }
}

public class GoodsGradePriceDto : GoodsGradePrice, IEntityDto
{
    public GoodsDto Goods { get; set; }
    public GoodsSpecCombinationDto GoodsSpecCombination { get; set; }
    public UserGrade Grade { get; set; }
    public string GradeName => this.Grade?.Name;
    public string GradeDescription => this.Grade?.Description;
}

public class GoodsSpecAttachDataInput : IEntityDto
{
    public bool Values { get; set; }
}

public class UpdateGoodsSpecStatusInput : IEntityDto<int>
{
    public int Id { get; set; }
    public bool? IsDeleted { get; set; }
}

public class UpdateGoodsSpecValueStatusInput : IEntityDto<int>
{
    public int Id { get; set; }
    public bool? IsDeleted { get; set; }
}

public class TagDto : Tag, IEntityDto<string>
{
    //
}

public class UpdateTagStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? IsDeleted { get; set; }
}

public class AttachCollectionItemDataInput : IEntityDto
{
    public bool Combination { get; set; } = false;
}

public class GoodsCollectionItemDto : GoodsCollectionItem, IEntityDto
{
    public GoodsSpecCombinationDto GoodsSpecCombination { get; set; }
}

public class QueryGoodsForCollectionItemSelectionInput : QueryCombinationForSelectionInput
{
    public string CollectionId { get; set; }
}

public class GoodsCollectionDto : GoodsCollection, IEntityDto
{
    public GoodsCollectionItemDto[] Items { get; set; }
}