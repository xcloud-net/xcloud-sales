using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Core.Application;

namespace XCloud.Database.EntityFrameworkCore.Mapping.PreConfiguration.Builtins;

public class PreConfigurationIdentityNameEntity<T> : IEntityTypePreConfiguration<T> where T : class, IHasIdentityNameFields
{
    public void PreConfigureEntity(EntityTypeBuilder<T> builder)
    {
        builder.ConfigIdentityNameEntity();
    }
}