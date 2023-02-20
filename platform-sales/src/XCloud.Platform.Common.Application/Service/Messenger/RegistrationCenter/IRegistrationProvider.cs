using System.Threading.Tasks;

namespace XCloud.Platform.Common.Application.Service.Messenger.RegistrationCenter;

public interface IRegistrationProvider
{
    Task RegisterUserInfoAsync(UserRegistrationInfo info);
    
    Task<string[]> GetUserServerInstancesAsync(string userId);
    
    Task RegisterGroupInfoAsync(GroupRegistrationInfo info);
    
    Task RemoveRegisterInfoAsync(string userId, string device);
}