using Volo.Abp.Modularity;
using XCloud.AspNetMvc;

namespace XCloud.Gateway;

[DependsOn(typeof(AspNetMvcModule))]
public class XCloudGatewayModule : AbpModule
{
    public XCloudGatewayModule()
    {
        //
    }
}