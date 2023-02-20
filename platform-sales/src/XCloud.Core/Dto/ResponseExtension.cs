using Volo.Abp;
using Volo.Abp.Http;

namespace XCloud.Core.Dto;

public static class ResponseExtension
{
    public static bool IsSuccess(this RemoteServiceErrorResponse response) => response.Error == null;

    public static TResponse ThrowIfErrorOccured<TResponse>(this TResponse response) where TResponse : RemoteServiceErrorResponse
    {
        var error = response.Error;
        if (error != null)
        {
            throw new BusinessException(message: error.Message, code: error.Code).WithData("error", error);
        }

        return response;
    }

    public static ApiResponse<T> SetData<T>(this ApiResponse<T> response, T data)
    {
        response.Data = data;

        return response;
    }

    public static TResponse ResetError<TResponse>(this TResponse response) where TResponse : RemoteServiceErrorResponse
    {
        response.Error = null;

        return response;
    }

    public static TResponse SetError<TResponse>(this TResponse response, string msg, string code = null, string details = null)
        where TResponse : RemoteServiceErrorResponse
    {
        response.Error = new RemoteServiceErrorInfo(message: msg, code: code, details: details);

        return response;
    }

}