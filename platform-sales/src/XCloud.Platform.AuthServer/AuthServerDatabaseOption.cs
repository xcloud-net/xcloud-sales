using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.AuthServer;

public class AuthServerDatabaseOption : IEntityDto, ISingletonDependency
{
    public AuthServerDatabaseOption()
    {
        this.AutoCreateDatabase = false;
    }

    public bool AutoCreateDatabase { get; set; } = false;
}