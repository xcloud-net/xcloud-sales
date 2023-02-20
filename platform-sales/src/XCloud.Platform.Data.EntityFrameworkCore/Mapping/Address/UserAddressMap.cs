using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Address;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Address;

public class UserAddressMap : EfEntityTypeConfiguration<UserAddress>
{
    public override void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("sys_user_address");

        builder.Property(x => x.UserId).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(100);

        builder.Property(x => x.Lat);
        builder.Property(x => x.Lng);

        builder.Property(x => x.NationCode).HasMaxLength(100);
        builder.Property(x => x.ProvinceCode).HasMaxLength(100);
        builder.Property(x => x.CityCode).HasMaxLength(100);
        builder.Property(x => x.AreaCode).HasMaxLength(100);

        builder.Property(x => x.Nation).HasMaxLength(100);
        builder.Property(x => x.Province).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Area).HasMaxLength(100);
        builder.Property(x => x.AddressDetail).HasMaxLength(500);

        builder.Property(x => x.AddressDescription).HasMaxLength(1000);

        builder.Property(x => x.PostalCode).HasMaxLength(100);
        builder.Property(x => x.Tel).HasMaxLength(100);

        builder.Property(x => x.DeletionTime);

        base.Configure(builder);
    }
}