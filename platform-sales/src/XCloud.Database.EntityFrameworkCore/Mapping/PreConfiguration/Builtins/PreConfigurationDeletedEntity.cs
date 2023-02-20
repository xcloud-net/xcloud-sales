using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.Auditing;

namespace XCloud.Database.EntityFrameworkCore.Mapping.PreConfiguration.Builtins;

public class PreConfigurationDeletedEntity<T> : IEntityTypePreConfiguration<T> where T : class, IHasDeletionTime
{
    public void PreConfigureEntity(EntityTypeBuilder<T> builder)
    {
        builder.ConfigAbpSoftDeletionTimeEntity();
    }
}