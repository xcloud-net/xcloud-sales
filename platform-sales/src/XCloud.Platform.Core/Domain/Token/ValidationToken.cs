using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Token;

public class ValidationToken : EntityBase, IPlatformEntity
{
    public string Token { get; set; }
}