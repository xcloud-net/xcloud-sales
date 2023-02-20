using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Logging;

public class ActivityLog : SalesBaseEntity
{
    public ActivityLog() { }

    public ActivityLog(int userId, string comment)
    {
        this.UserId = userId;
        this.Comment = comment;
    }

    public int ActivityLogTypeId { get; set; }

    public int UserId { get; set; }

    public string AdministratorId { get; set; }

    public string Comment { get; set; }

    public string Value { get; set; }

    public string Data { get; set; }

    public string UrlReferrer { get; set; }

    public string BrowserType { get; set; }

    public string Device { get; set; }

    public string UserAgent { get; set; }

    public string IpAddress { get; set; }

    public string GeoCountry { get; set; }

    public string GeoCity { get; set; }

    public double? Lng { get; set; }

    public double? Lat { get; set; }

    public string RequestPath { get; set; }

    public string SubjectType { get; set; }

    public string SubjectId { get; set; }

    public int SubjectIntId { get; set; }

    public DateTime CreationTime { get; set; }

}