using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Core.Application;

namespace XCloud.Database.EntityFrameworkCore.Mapping.PreConfiguration.Builtins;

public class PreConfigurationBaseEntity<T> : IEntityTypePreConfiguration<T> where T : class, IEntityBase
{
    public void PreConfigureEntity(EntityTypeBuilder<T> builder)
    {
        builder.ConfigBaseEntity();
    }
}