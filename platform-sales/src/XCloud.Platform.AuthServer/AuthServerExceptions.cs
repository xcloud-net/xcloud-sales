using XCloud.Core;

namespace XCloud.Platform.AuthServer;

public class AuthServerException : BaseException
{
    public AuthServerException(string msg) : base(msg) { }
}