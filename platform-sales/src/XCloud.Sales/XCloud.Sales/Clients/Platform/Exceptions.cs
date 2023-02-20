namespace XCloud.Sales.Clients.Platform;

public class PlatformApiException : BusinessException
{
    public PlatformApiException()
    {
        //
    }

    public PlatformApiException(string message) : base(message: message)
    {
        //
    }

    public PlatformApiException(string message, Exception inner) : base(message, innerException: inner)
    {
        //
    }
}