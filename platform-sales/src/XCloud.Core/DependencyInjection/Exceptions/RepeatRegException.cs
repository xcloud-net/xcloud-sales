namespace XCloud.Core.DependencyInjection.Exceptions;

public class RepeatRegException : Exception
{
    public RepeatRegException(string msg) : base(msg) { }

    public object RepeatItems { get; set; }
}