namespace XCloud.Platform.Application.Messenger.Registry;

public interface IRegistrationProvider
{
    Task RegisterUserInfoAsync(UserRegistrationInfo info);
    
    Task<string[]> GetUserServerInstancesAsync(string userId);

    Task RemoveRegisterInfoAsync(string userId, string device);
}