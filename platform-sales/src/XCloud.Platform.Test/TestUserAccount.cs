using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using XCloud.Platform.Test.Service;

namespace XCloud.Platform.Test;

[TestClass]
public class TestUserAccount : TestBase
{
    [TestMethod]
    public async Task TestUserAccountAsync()
    {
        using var s = this.WebApplication.Services.CreateScope();

        await s.ServiceProvider.GetRequiredService<TestUserAccountService>().TestUserAccountAsync();
    }
}