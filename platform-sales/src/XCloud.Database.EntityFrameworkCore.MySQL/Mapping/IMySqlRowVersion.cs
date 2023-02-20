using System;

namespace XCloud.Database.EntityFrameworkCore.MySQL.Mapping;

public interface IMySqlRowVersion
{
    /// <summary>
    /// 不同数据库可能会有不同，具体看驱动程序，sqlserver用byte[]，mysql的驱动使用datetime
    /// https://docs.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=fluent-api
    /// https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/351
    /// </summary>
    public DateTime RowVersion { get; set; }
}