using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Volo.Abp.EntityFrameworkCore.Modeling;
using XCloud.Core.Helper;

namespace XCloud.Database.EntityFrameworkCore.Mapping;

/// <summary>
/// fluent map base class
/// </summary>
public abstract class EfEntityTypeConfiguration<T> : IMappingConfiguration, IEntityTypeConfiguration<T> where T : class, IDbTableFinder
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        AbpEntityTypeBuilderExtensions.ConfigureByConvention(builder);
        builder.TryConfig();
    }

    public void ApplyConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(this);
    }
}