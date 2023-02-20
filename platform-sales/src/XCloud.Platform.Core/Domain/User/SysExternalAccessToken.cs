using XCloud.Core.Application;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.User;

public class SysExternalAccessToken : EntityBase, IMemberEntity
{
    public string Platform { get; set; }
    public string AppId { get; set; }
    public string UserId { get; set; }
    public string Scope { get; set; }
    [Obsolete]
    public string GrantType { get; set; }
    public int AccessTokenType { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiredAt { get; set; }
}