using System.Threading.Tasks;

namespace XCloud.Core.Development;

public interface IDevelopmentInformationProvider
{
    string ProviderName { get; }
    Task<object> Information();
}