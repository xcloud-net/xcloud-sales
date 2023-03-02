using XCloud.Core;

namespace XCloud.Elasticsearch.Exception;

public class ResponseException : BaseException
{
    public ResponseException(string msg, System.Exception inner = null) : base(msg, inner) { }
}

public class BulkException : System.Exception
{
    public BulkException(string msg) : base(msg) { }

    public string[] ErrorList { get; set; }
}