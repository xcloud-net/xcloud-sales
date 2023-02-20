namespace XCloud.Core.Cache;

public class CacheException : BaseException
{
    public CacheException(string msg, Exception e = null) : base(msg, e) { }
}

public class CacheSourceException : BaseException
{
    public CacheSourceException(Exception e) : base(e.Message, e) { }
}