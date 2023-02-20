using Volo.Abp.ObjectMapping;

namespace XCloud.Core.Application;

public static class AbpMapperExtension
{
    public static Target[] MapArray<T,Target>(this IObjectMapper mapper, IEnumerable<T> source)
    {
        if (mapper == null)
            throw new ArgumentNullException(nameof(mapper));
        
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        
        var data = source.Select(mapper.Map<T, Target>).ToArray();
        
        return data;
    }
}