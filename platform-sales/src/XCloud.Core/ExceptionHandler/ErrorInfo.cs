using System.Net;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Http;

namespace XCloud.Core.ExceptionHandler;

public class ErrorInfo : RemoteServiceErrorInfo, IEntityDto
{
    public ErrorInfo()
    {
        //
    }

    public ErrorInfo(RemoteServiceErrorInfo e) : this(string.Empty)
    {
        this.Code = e.Code;
        this.Message = e.Message;
        this.Data = e.Data;
        this.Details = e.Details;
        this.ValidationErrors = e.ValidationErrors;
    }

    public ErrorInfo(string msg)
    {
        this.Message = msg;
    }

    public HttpStatusCode? HttpStatusCode { get; set; }
    public bool? FriendlyError { get; set; }
}