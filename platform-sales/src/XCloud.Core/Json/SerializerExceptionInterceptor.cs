using System.Runtime.Serialization;
using System.Threading.Tasks;
using Volo.Abp.DynamicProxy;

namespace XCloud.Core.Json;

public class SerializerExceptionInterceptor : IAbpInterceptor
{
    public SerializerExceptionInterceptor()
    {
        //
    }
    
    public async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        try
        {
            await invocation.ProceedAsync();
        }
        catch (SerializeException)
        {
            throw;
        }
        catch (System.Exception e)
        {
            throw new SerializationException(e.Message, e);
        }
    }
}