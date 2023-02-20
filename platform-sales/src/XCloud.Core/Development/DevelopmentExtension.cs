using System.Threading.Tasks;

namespace XCloud.Core.Development;

public static class DevelopmentExtension
{
    public static async Task<IEnumerable<object>> GetDevelopmentInformation(this IServiceProvider serviceProvider)
    {
        async Task<object> Info(IDevelopmentInformationProvider p)
        {
            return new
            {
                p.ProviderName,
                Information = await p.Information()
            };
        }

        var providers = serviceProvider.GetServices<IDevelopmentInformationProvider>().ToArray();

        var res = await Task.WhenAll(providers.Select(Info));

        return res;
    }
}