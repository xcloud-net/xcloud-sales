# aspect core +.net core 3.1

## install dependency

> nuget install AspectCore.Extensions.DependencyInjection

## start up

```csharp
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebApplication4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            // for aspcectcore
            .UseServiceProviderFactory(new AspectCoreServiceProviderFactory());

    }
}
```

## the interceptor

```csharp
using AspectCore.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace WebApplication4.Aop
{

    public class CustomInterceptorAttribute : AbstractInterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before service call");
                await next(context);
            }
            catch (Exception)
            {
                Console.WriteLine("Service threw an exception!");
                throw;
            }
            finally
            {
                Console.WriteLine("After service call");
            }
        }
    }

}
```

## usage

```csharp
using System.Collections.Generic;
using WebApplication4.Aop;

namespace WebApplication4.App
{
    public class StudentRepository : IStudentRepository
    {

        [CustomInterceptor]
        public List<Student> GetAllStudents()
        {
            var students = new List<Student>();
            students.Add(new Student { Name = "Panxixi", Age = 11 });
            students.Add(new Student { Name = "Liuchuhui", Age = 12 });
            return students;

        }
    }
}
```

## extra (autofac+aspec core)

> nuget install AspectCore.Extensions.Autofac

``` csharp
        public override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterDynamicProxy(option =>
            {
                option.Interceptors.AddServiced<MethodCallMetricAttribute>(x => x.NameSpace.StartWith("xx.xx"));
                option.ThrowAspectException = false;
            });
        }
```
