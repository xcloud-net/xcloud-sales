namespace XCloud.Sales.Core;

/// <summary>
/// Represents errors that occur during application execution
/// </summary>
[Serializable]
public class SalesException : BusinessException
{
    /// <summary>
    /// Initializes a new instance of the Exception class.
    /// </summary>
    public SalesException()
    {
        //
    }

    /// <summary>
    /// Initializes a new instance of the Exception class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SalesException(string message)
        : base(message)
    {
    }
}