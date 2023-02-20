using System;
using XCloud.Core;

namespace XCloud.Elasticsearch;

public class ResponseException : BaseException
{
    public ResponseException(string msg, Exception inner = null) : base(msg, inner) { }
}

public class BulkException : Exception
{
    public BulkException(string msg) : base(msg) { }

    public string[] ErrorList { get; set; }
}