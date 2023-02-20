using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Storage;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Storage;

public class StorageResourceMetaMap : EfEntityTypeConfiguration<StorageResourceMeta>
{
    public override void Configure(EntityTypeBuilder<StorageResourceMeta> builder)
    {
        builder.ToTable("sys_storage_meta");

        builder.Property(x => x.StorageProvider).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ContentType).HasMaxLength(100);
        builder.Property(x => x.FileExtension).HasMaxLength(30);
        builder.Property(x => x.ResourceHash).HasMaxLength(100);
        builder.Property(x => x.HashType).HasMaxLength(20);
        builder.Property(x => x.ResourceKey).HasMaxLength(500);
        builder.Property(x => x.ExtraData);
        builder.Property(x => x.ReferenceCount);
        builder.Property(x => x.UploadTimes);

        //不同catalog里可以重复
        //builder.HasIndex(x => x.FileHash).IsUnique();

        base.Configure(builder);
    }
}