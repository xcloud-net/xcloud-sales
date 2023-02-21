using Volo.Abp.Application.Dtos;
using XCloud.Platform.Shared.Dto;
using XCloud.Sales.Data.Domain.Media;

namespace XCloud.Sales.Service.Media;

public class PictureDto : Picture, IEntityDto
{
    public int? GoodsId { get; set; }
    public int? CombinationId { get; set; }
    public int? BrandId { get; set; }
    public int DisplayOrder { get; set; }
    public MallStorageMetaDto StorageMeta { get; set; }
}

public class MallStorageMetaDto : StorageMetaDto, IEntityDto
{
    public MallStorageMetaDto()
    {
        //
    }

    public MallStorageMetaDto(int pictureId)
    {
        this.PictureId = pictureId;
    }

    public int? PictureId { get; set; }
    public int? GoodsId { get; set; }
    public int? CombinationId { get; set; }
    public int? BrandId { get; set; }
    public int? DisplayOrder { get; set; }
}