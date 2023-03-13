using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Application.Common.Service.Messenger.UserContext;

public interface IUserGroups
{
    Task<string[]> GetUserGroupsAsync(string userId);
    
    Task<string[]> GetUsersGroupsAsync(string[] userIds);
}

[ExposeServices(typeof(IUserGroups))]
public class TestUserGroups : IUserGroups, ISingletonDependency
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