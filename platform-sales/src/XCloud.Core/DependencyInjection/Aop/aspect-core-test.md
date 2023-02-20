# aspect core test

```csharp
using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace XCloud.Test
{
    public class SampleInterceptor : AspectCore.DynamicProxy.AbstractInterceptorAttribute
    {
        private readonly Action<string> action;
        public SampleInterceptor(Action<string> action)
        {
            this.action = action;
        }

        public override Task Invoke(
            AspectCore.DynamicProxy.AspectContext context,
            AspectCore.DynamicProxy.AspectDelegate next)
        {
            this.action?.Invoke(context.ImplementationMethod?.Name);
            return context.Invoke(next);
        }
    }

    [TestClass]
    public class aspectcore_dynamic_proxy_test
    {
        public interface ISimple
        {
            string Name { get; set; }
        }

        public class SimpleSamepleEntity
        {
            public virtual string Name { get; set; }

            //public void set_Name(string n) { }

            public virtual int Age { get; set; }
        }

        [TestMethod]
        public void aspectcore_proxy_with_target()
        {
            var change = 0;

            var proxyGeneratorBuilder = new ProxyGeneratorBuilder().Configure(option =>
            {
                option.Interceptors.AddTyped<SampleInterceptor>(new object[] { new Action<string>(x => ++change) });
            });
            var proxyGenerator = proxyGeneratorBuilder.Build();
            var entity = proxyGenerator.CreateClassProxy<SimpleSamepleEntity>();
            entity.Name = "Richie";

            change.Should().Be(1);
        }

        [TestMethod]
        public void aspectcore_proxy_without_target()
        {
            var change = 0;

            var proxyGeneratorBuilder = new ProxyGeneratorBuilder().Configure(option =>
            {
                option.Interceptors.AddTyped<SampleInterceptor>(new object[] { new Action<string>(x => ++change) });
            });
            var proxyGenerator = proxyGeneratorBuilder.Build();
            var entity = proxyGenerator.CreateInterfaceProxy<ISimple>();
            entity.Name = "Richie";

            change.Should().Be(1);
        }

        interface IFoo
        {
            Task<string> xxdd();
        }

        class FooProxy : System.Reflection.DispatchProxy
        {
            public IServiceProvider Provider { get; set; }

            async Task<string> xx(string f)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                return f;
            }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                return this.xx(targetMethod.Name);
            }
        }

        [TestMethod]
        public async Task dispatch_proxy()
        {
            var xx = FooProxy.Create<IFoo, FooProxy>();
            ((FooProxy)xx).Provider = null;

            var res = await xx.xxdd();
            res.Should().Be(nameof(xx.xxdd));
        }

    }
}
```
