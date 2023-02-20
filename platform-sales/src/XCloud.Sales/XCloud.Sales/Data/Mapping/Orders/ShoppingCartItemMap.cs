using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Data.Mapping.Orders;

public class ShoppingCartItemMap : SalesEntityTypeConfiguration<ShoppingCartItem>
{
    public override void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
    {
        builder.ToTable(nameof(ShoppingCartItem));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId);
        builder.Property(x => x.GoodsId);
        builder.Property(x => x.GoodsSpecCombinationId);
        builder.Property(x => x.Quantity);

        builder.Property(x => x.CreationTime);
        builder.Property(x => x.LastModificationTime);

        base.Configure(builder);
    }
}