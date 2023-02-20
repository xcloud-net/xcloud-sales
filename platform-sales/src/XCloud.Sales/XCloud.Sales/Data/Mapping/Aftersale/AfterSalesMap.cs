using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Aftersale;

namespace XCloud.Sales.Data.Mapping.Aftersale;

public class AfterSalesMap : SalesEntityTypeConfiguration<AfterSales>
{
    public override void Configure(EntityTypeBuilder<AfterSales> builder)
    {
        builder.ToTable(nameof(AfterSales));
        builder.HasKey(rr => rr.Id);

        builder.Property(x => x.OrderId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.UserId);

        builder.Property(rr => rr.ReasonForReturn).IsRequired().HasMaxLength(500);
        builder.Property(rr => rr.RequestedAction).IsRequired().HasMaxLength(500);
        builder.Property(rr => rr.UserComments).HasMaxLength(500);
        builder.Property(rr => rr.StaffNotes).HasMaxLength(500);
        builder.Property(rr => rr.Images).HasMaxLength(1000);

        base.Configure(builder);
    }
}