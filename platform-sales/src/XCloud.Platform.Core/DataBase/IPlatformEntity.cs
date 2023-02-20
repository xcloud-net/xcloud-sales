using Volo.Abp.Domain.Entities;
using XCloud.Core.Helper;

namespace XCloud.Platform.Core.Database;

public interface IPlatformEntity : IEntity, IDbTableFinder { }