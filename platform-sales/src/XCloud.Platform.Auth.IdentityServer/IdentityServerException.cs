using XCloud.Core;

namespace XCloud.Platform.Auth.IdentityServer;

public class IdentityServerException : BaseException
{
    public IdentityServerException(string msg) : base(msg) { }
}