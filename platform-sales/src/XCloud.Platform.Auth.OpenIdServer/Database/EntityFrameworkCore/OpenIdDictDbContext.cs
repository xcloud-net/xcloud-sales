using Microsoft.EntityFrameworkCore;

namespace XCloud.Platform.Auth.OpenIdServer.Database.EntityFrameworkCore;

public class OpenIdDictDbContext : DbContext
{
    public OpenIdDictDbContext(DbContextOptions<OpenIdDictDbContext> options) : base(options)
    {
        //
    }
}