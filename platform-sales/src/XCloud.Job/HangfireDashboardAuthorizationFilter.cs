using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace XCloud.Job;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}