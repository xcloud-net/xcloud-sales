using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using XCloud.Database.EntityFrameworkCore.ValueGenerators;

namespace XCloud.Database.EntityFrameworkCore.MySQL.Mapping;

public static class MappingExtension
{
    public static void ApplyUtf8Mb4ForAll(this ModelBuilder builder)
    {
        builder.HasCharSet(CharSet.Utf8Mb4, DelegationModes.ApplyToAll);
    }

    /// <summary>
    /// ef提供的这个特性不是对所有数据库通用，所以剥离出来，单独正对某个provider扩展
    /// </summary>
    public static EntityTypeBuilder<T> ConfigMySqlRowVersion<T>(this EntityTypeBuilder<T> builder) where T : class, IMySqlRowVersion
    {
        builder.Property(x => x.RowVersion)
            .IsRequired()
            .IsRowVersion()
            .HasValueGenerator<UtcTimeGenerator>()
            .ValueGeneratedOnAddOrUpdate()
            .HasComment("控制并发");

        return builder;
    }
}