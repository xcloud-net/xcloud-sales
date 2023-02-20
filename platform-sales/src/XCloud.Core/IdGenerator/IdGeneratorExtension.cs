using Volo.Abp.Guids;

namespace XCloud.Core.IdGenerator;

public static class IdGeneratorExtension
{
    public static string CreateGuidString(this IGuidGenerator idGenerator)
    {
        var res = idGenerator.Create().ToString().Replace("-", string.Empty);
        return res;
    }
}