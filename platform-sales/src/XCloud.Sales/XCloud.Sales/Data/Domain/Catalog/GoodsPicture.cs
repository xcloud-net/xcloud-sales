using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class GoodsPicture : SalesBaseEntity
{
    public int GoodsId { get; set; }

    public int CombinationId { get; set; }

    public int PictureId { get; set; }

    public int DisplayOrder { get; set; }
}