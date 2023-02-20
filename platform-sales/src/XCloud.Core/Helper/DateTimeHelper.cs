namespace XCloud.Core.Helper;

public static class DateTimeHelper
{
    public static readonly DateTime UTC1970 = new DateTime(
        year: 1970, month: 1, day: 1,
        hour: 0, minute: 0, second: 0,
        kind: DateTimeKind.Utc);

    public const string FormatHourMin = "HH:mm";
    public const string FormatHourMinSecond = "HH:mm:ss";
    public const string FormatHourMinSecondMillisecond = "HH:mm:ss.fff";
    public const string FormatDate = "yyyy-MM-dd";
    public const string FormatDateTime = "yyyy-MM-dd HH:mm";
    public const string FormatDateTimeWithSecond = "yyyy-MM-dd HH:mm:ss";
    public const string FormatDateChinese = "yyyy年MM月dd日";

    /// <summary>
    /// 时间戳生成规则
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStamp(DateTime utc_time)
    {
        var ts = utc_time - UTC1970;
        return (long)ts.TotalSeconds;
    }
}