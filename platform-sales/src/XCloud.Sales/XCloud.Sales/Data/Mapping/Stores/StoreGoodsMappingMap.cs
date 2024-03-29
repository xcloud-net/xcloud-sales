﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Stores;

namespace XCloud.Sales.Data.Mapping.Stores;

public class StoreGoodsMappingMap : SalesEntityTypeConfiguration<StoreGoodsMapping>
{
    public override void Configure(EntityTypeBuilder<StoreGoodsMapping> builder)
    {
        builder.ToTable(nameof(StoreGoodsMapping));

        builder.HasKey(m => m.Id);
        builder.Property(m => m.StoreId).IsRequired().HasMaxLength(100);
        builder.Property(m => m.GoodsCombinationId);
        builder.Property(m => m.Price).HasPrecision(18, 2);
        builder.Property(m => m.StockQuantity);
        builder.Property(m => m.CreationTime);
        builder.Property(m => m.LastModificationTime);
    }
}