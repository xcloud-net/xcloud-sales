using Volo.Abp;

namespace XCloud.Redis;

public class FailToGetRedLockException : AbpException
{
    public FailToGetRedLockException() { }
    public FailToGetRedLockException(string message) : base(message) { }
}