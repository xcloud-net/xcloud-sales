using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Core.Application;

namespace XCloud.Database.EntityFrameworkCore.Mapping.PreConfiguration.Builtins;

public class PreConfigurationTreeEntity<T> : IEntityTypePreConfiguration<T> where T : class, ITreeEntityBase
{
    public void PreConfigureEntity(EntityTypeBuilder<T> builder)
    {
        builder.ConfigTreeEntity();
    }
}