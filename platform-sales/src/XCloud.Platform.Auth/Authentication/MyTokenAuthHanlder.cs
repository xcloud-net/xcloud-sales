using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace XCloud.Platform.Auth.Authentication;

public class MyTokenAuthOption : AuthenticationSchemeOptions
{
    //
}

/// <summary>
/// signin-hanlder->signout-handler->auth-handler
/// </summary>
public class MyTokenAuthHanlder<LoginUserEntity> : SignInAuthenticationHandler<MyTokenAuthOption>,
    IAuthenticationHandler,
    IAuthenticationSignInHandler,
    IAuthenticationSignOutHandler
    where LoginUserEntity : class
{
    public MyTokenAuthHanlder(IOptionsMonitor<MyTokenAuthOption> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        //
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        throw new NotImplementedException();
    }

    protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }

    protected override Task HandleSignOutAsync(AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }
}