using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Data.Mapping.Orders;

public class OrderNoteMap : SalesEntityTypeConfiguration<OrderNote>
{
    public override void Configure(EntityTypeBuilder<OrderNote> builder)
    {
        builder.ToTable(nameof(OrderNote));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Note).HasMaxLength(1000);

        builder.Property(x => x.DisplayToUser);
        builder.Property(x => x.CreationTime);

        base.Configure(builder);
    }
}