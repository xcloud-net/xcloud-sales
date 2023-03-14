using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Application.Messenger.Service;

public interface IUserService
{
    Task<string[]> GetUserGroupsAsync(string userId);
    
    Task<string[]> GetUsersGroupsAsync(string[] userIds);
}

[ExposeServices(typeof(IUserService))]
public class TestUserService : IUserService, ISingletonDependency
{
    public async Task<string[]> GetUserGroupsAsync(string userUid)
    {
        await Task.CompletedTask;
        return new string[] { "group-a" };
    }

    public async Task<string[]> GetUsersGroupsAsync(string[] userUids)
    {
        await Task.CompletedTask;
        return new string[] { "group-a" };
    }
}