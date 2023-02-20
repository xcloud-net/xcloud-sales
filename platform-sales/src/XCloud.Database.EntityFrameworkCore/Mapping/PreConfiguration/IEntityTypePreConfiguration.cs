using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XCloud.Database.EntityFrameworkCore.Mapping.PreConfiguration;

public interface IEntityTypePreConfiguration<T> where T : class
{
    void PreConfigureEntity(EntityTypeBuilder<T> builder);
}