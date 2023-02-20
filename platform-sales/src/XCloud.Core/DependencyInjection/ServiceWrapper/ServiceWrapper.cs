namespace XCloud.Core.DependencyInjection.ServiceWrapper;

public class ServiceWrapper<T>
{
    public T Value { get; }

    public ServiceWrapper()
    {
        //
    }

    public ServiceWrapper(T data) : this()
    {
        this.Value = data;
    }
}