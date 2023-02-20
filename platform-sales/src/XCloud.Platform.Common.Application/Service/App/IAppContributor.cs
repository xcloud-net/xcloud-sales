using System.Threading.Tasks;

namespace XCloud.Platform.Common.Application.Service.App;

public interface IAppContributor
{
    Task<Models.App> GetApp();
}

//创建一个abp的permission provider