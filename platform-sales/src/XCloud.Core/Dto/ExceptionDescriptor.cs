using System.Collections;

namespace XCloud.Core.Dto;

public class ExceptionDescriptor
{
    public string Message { get; set; }
    public string Code { get; set; }
    public string Detail { get; set; }
    public IDictionary Data { get; set; }

    public IEnumerable<ExceptionDescriptor> InnerExceptions { get; set; }
}