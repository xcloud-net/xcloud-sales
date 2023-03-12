namespace XCloud.Core.Exceptions;

public static class ExceptionHandlerExtension
{
    public static void AddMyExceptionHandler(this IServiceCollection services)
    {
        services.AddScoped(typeof(IExceptionLogger<>), typeof(DefaultExceptionLogger<>));
    }

    public static void AddMyExceptionDetailContributor<T>(this IServiceCollection services) where T : class, IExceptionDetailContributor
    {
        services.Remove(ServiceDescriptor.Transient<IExceptionDetailContributor, T>());
        services.AddTransient<IExceptionDetailContributor, T>();
    }
}