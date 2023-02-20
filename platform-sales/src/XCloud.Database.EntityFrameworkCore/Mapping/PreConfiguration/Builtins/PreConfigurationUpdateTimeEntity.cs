using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Volo.Abp.Auditing;

namespace XCloud.Database.EntityFrameworkCore.Mapping.PreConfiguration.Builtins;

public class PreConfigurationUpdateTimeEntity<T> : IEntityTypePreConfiguration<T> where T : class, IHasModificationTime
{
    public void PreConfigureEntity(EntityTypeBuilder<T> builder)
    {
        builder.ConfigModificationTimeEntity();
    }
}