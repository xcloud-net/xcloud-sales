using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Core.Application;
using XCloud.Core.Application.Entity;

namespace XCloud.Database.EntityFrameworkCore.Mapping.PreConfiguration.Builtins;

public class PreConfigurationAppEntity<T> : IEntityTypePreConfiguration<T> where T : class, IHasAppFields
{
    public void PreConfigureEntity(EntityTypeBuilder<T> builder)
    {
        builder.ConfigAppEntity();
    }
}