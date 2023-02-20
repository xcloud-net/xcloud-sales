# mysql 驱动

https://mysqlconnector.net/

这个比官方的mysql驱动好

# ef provider

https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql


```csharp
            this.Configure<AbpDbContextOptions>(option =>
            {
                option.Configure<CommonServiceDbContext>(db_option =>
                {
                    var config = db_option.ServiceProvider.ResolveConfiguration();
                    var cstr = config.GetCommonServiceConnectionStringOrThrow();

                    cstr = cstr.MySqlConnectionString();

                    var serverVersion = ServerVersion.AutoDetect(cstr);
                    db_option.DbContextOptions.UseMySql(cstr);
                });
            });
```