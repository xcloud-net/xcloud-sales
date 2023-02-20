namespace XCloud.Core.Helper;

/// <summary>
/// 地理位置相关帮助类
/// </summary>
public static class GeoHelper
{
    #region 私有

    /// <summary>
    /// 地球半径
    /// </summary>
    private const double EARTH_RADIUS = 6378.137;

    /// <summary>
    /// 弧度转换
    /// </summary>
    private static double Rad(double d) => d * Math.PI / 180.0;

    /// <summary>
    /// 验证经纬度数值正确性
    /// </summary>
    private static bool IsGeoValid(GeoInfo point) =>
        point != null && Math.Abs(point.Lat) <= 90 && Math.Abs(point.Lon) <= 180;

    #endregion

    /// <summary>
    /// 计算距离
    /// </summary>
    public static double GetDistanceInKm(double lat1, double lon1, double lat2, double lon2, double? defaultValue = -1) =>
        GetDistanceInKm(new GeoInfo() { Lat = lat1, Lon = lon1 }, new GeoInfo() { Lat = lat2, Lon = lon2 }, defaultValue);

    /// <summary>
    /// 计算距离，如返回最大值需要检查数据
    /// </summary>
    public static double GetDistanceInKm(GeoInfo startPoint, GeoInfo endPoint, double? defaultValue)
    {
        if (!IsGeoValid(startPoint) || !IsGeoValid(endPoint))
        {
            return defaultValue ?? throw new ArgumentException("无法计算距离，传入坐标错误");
        }

        var startlatrad = Rad(startPoint.Lat);
        var endlatrad = Rad(endPoint.Lat);
        var a = startlatrad - endlatrad;
        var b = Rad(startPoint.Lon - endPoint.Lon);

        return 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
                                                      Math.Cos(startlatrad) *
                                                      Math.Cos(endlatrad) *
                                                      Math.Pow(Math.Sin(b / 2), 2)));
    }

}

public class GeoInfo
{
    /// <summary>
    /// 纬度
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    public double Lon { get; set; }
}

/// <summary>
/// 国际坐标系（wgs84）-百度坐标系（bd09）-高德坐标系（火星坐标系，gcj02）相互转换
/// https://github.com/TopJohn/CoordinateTransform
/// https://github.com/wandergis/coordTransform_py
/// </summary>
public static class GeoCoordinatesTransformHelper
{
    private const double x_PI = 3.14159265358979324 * 3000.0 / 180.0;
    private const double PI = 3.1415926535897932384626;
    private const double a = 6378245.0;
    private const double ee = 0.00669342162296594323;

    /// <summary>
    /// 百度坐标（BD09）转 GCJ02
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static double[] transformBD09ToGCJ02(double lng, double lat)
    {
        var x = lng - 0.0065;
        var y = lat - 0.006;
        var z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_PI);
        var theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_PI);
        var gcj_lng = z * Math.Cos(theta);
        var gcj_lat = z * Math.Sin(theta);
        return new double[] { gcj_lng, gcj_lat };
    }

    /// <summary>
    /// GCJ02 转百度坐标
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static double[] transformGCJ02ToBD09(double lng, double lat)
    {
        var z = Math.Sqrt(lng * lng + lat * lat) + 0.00002 * Math.Sin(lat * x_PI);
        var theta = Math.Atan2(lat, lng) + 0.000003 * Math.Cos(lng * x_PI);
        var bd_lng = z * Math.Cos(theta) + 0.0065;
        var bd_lat = z * Math.Sin(theta) + 0.006;
        return new double[] { bd_lng, bd_lat };
    }

    /// <summary>
    /// GCJ02 转 WGS84
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static double[] transformGCJ02ToWGS84(double lng, double lat)
    {
        if (outOfChina(lng, lat))
        {
            return new double[] { lng, lat };
        }
        else
        {
            var dLat = transformLat(lng - 105.0, lat - 35.0);
            var dLng = transformLng(lng - 105.0, lat - 35.0);
            var radLat = lat / 180.0 * PI;
            var magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            var sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * PI);
            dLng = (dLng * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * PI);
            var mgLat = lat + dLat;
            var mgLng = lng + dLng;
            return new double[] { lng * 2 - mgLng, lat * 2 - mgLat };
        }
    }

    /// <summary>
    /// WGS84 坐标 转 GCJ02
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static double[] transformWGS84ToGCJ02(double lng, double lat)
    {
        if (outOfChina(lng, lat))
        {
            return new double[] { lng, lat };
        }
        else
        {
            var dLat = transformLat(lng - 105.0, lat - 35.0);
            var dLng = transformLng(lng - 105.0, lat - 35.0);
            var redLat = lat / 180.0 * PI;
            var magic = Math.Sin(redLat);
            magic = 1 - ee * magic * magic;
            var sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * PI);
            dLng = (dLng * 180.0) / (a / sqrtMagic * Math.Cos(redLat) * PI);
            var mgLat = lat + dLat;
            var mgLng = lng + dLng;
            return new double[] { mgLng, mgLat };
        }
    }

    /// <summary>
    /// 百度坐标BD09 转 WGS84
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static double[] transformBD09ToWGS84(double lng, double lat)
    {
        var lngLat = transformBD09ToGCJ02(lng, lat);

        return transformGCJ02ToWGS84(lngLat[0], lngLat[1]);
    }

    /// <summary>
    /// WGS84 转 百度坐标BD09
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static double[] transformWGS84ToBD09(double lng, double lat)
    {
        var lngLat = transformWGS84ToGCJ02(lng, lat);

        return transformGCJ02ToBD09(lngLat[0], lngLat[1]);
    }

    private static double transformLat(double lng, double lat)
    {
        var ret = -100.0 + 2.0 * lng + 3.0 * lat + 0.2 * lat * lat + 0.1 * lng * lat + 0.2 * Math.Sqrt(Math.Abs(lng));
        ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(lat * PI) + 40.0 * Math.Sin(lat / 3.0 * PI)) * 2.0 / 3.0;
        ret += (160.0 * Math.Sin(lat / 12.0 * PI) + 320 * Math.Sin(lat * PI / 30.0)) * 2.0 / 3.0;
        return ret;
    }

    private static double transformLng(double lng, double lat)
    {
        var ret = 300.0 + lng + 2.0 * lat + 0.1 * lng * lng + 0.1 * lng * lat + 0.1 * Math.Sqrt(Math.Abs(lng));
        ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(lng * PI) + 40.0 * Math.Sin(lng / 3.0 * PI)) * 2.0 / 3.0;
        ret += (150.0 * Math.Sin(lng / 12.0 * PI) + 300.0 * Math.Sin(lng / 30.0 * PI)) * 2.0 / 3.0;
        return ret;
    }

    /// <summary>
    /// 判断坐标是否不在国内
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static bool outOfChina(double lng, double lat) =>
        (lng < 72.004 || lng > 137.8347) || (lat < 0.8293 || lat > 55.8271);
}