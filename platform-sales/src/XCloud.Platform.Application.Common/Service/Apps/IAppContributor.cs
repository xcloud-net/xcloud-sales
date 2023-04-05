using System.Threading.Tasks;

namespace XCloud.Platform.Application.Common.Service.Apps;

public interface IAppContributor
{
    Task<Models.App> GetApp();
}

//创建一个abp的permission provider