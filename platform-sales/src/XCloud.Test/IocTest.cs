using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XCloud.Core.DependencyInjection.AutofacProvider;
using XCloud.Core.DependencyInjection.Extension;

namespace XCloud.Test;

[TestClass]
public class IocTest
{
    interface IId { }
    class D : IId { public int Xx { get; set; } }
    class Dd : IId { }

    [TestMethod]
    public void autofac_repeat_reg_test()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<D>().AsSelf().AsImplementedInterfaces();
        builder.RegisterType<Dd>().AsSelf().AsImplementedInterfaces();
        builder.RegisterInstance(new Dd() { }).AsSelf().AsImplementedInterfaces();

        using var con = builder.Build();
        con.ResolveAll<IId>().Length.Should().Be(3);
        con.Resolve<IId>().GetType().Should().Be(typeof(Dd));
    }

    [TestMethod]
    public void scope_test()
    {
        var collection = new ServiceCollection();
        collection.AddScoped<D>();
        var provider = collection.BuildServiceProvider();

        using var scope1 = provider.CreateScope();
        var xx = scope1.ServiceProvider.GetRequiredService<D>();
        xx.Xx = 88;
        scope1.ServiceProvider.GetRequiredService<D>().Xx.Should().Be(xx.Xx);

        using var scope2 = scope1.ServiceProvider.CreateScope();
        var dd = scope2.ServiceProvider.GetRequiredService<D>();
        dd.Xx.Should().Be(0);
    }

    [TestMethod]
    public void single_instance()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton(provider => new Service());

        collection.GetSingletonInstanceOrNull<Service>().Should().BeNull();

        collection.RemoveAll<Service>();
        collection.AddSingleton(new Service());

        collection.GetSingletonInstanceOrNull<Service>().Should().NotBeNull();
    }

    //[TestMethod]
    public void autofac_integrate_with_builtin_di_system_test()
    {
        //创建builder
        var builder = new ContainerBuilder();
        //必须把servicecollection中的注册项copy过来，不然无法用iserviceprovider创建scope
        builder.Populate(new ServiceCollection());

        using var context = builder.Build();
        var provider = context.AsServiceProvider();

        try
        {
            using (var scope = provider.CreateScope())
            {
                //scope:Autofac.Extensions.DependencyInjection.AutofacServiceScope
            }
        }
        catch (Exception e)
        {
            /*
         Autofac.Core.Registration.ComponentNotRegisteredException:
         “The requested service 'Microsoft.Extensions.DependencyInjection.IServiceScopeFactory' has not been registered. 
         To avoid this exception, either register a component to provide the service, 
         check for service registration using IsRegistered(), or use the ResolveOptional() method to resolve an optional dependency.”    
         */
            Console.Out.Write(e.Message);
        }
    }

    interface ITransit : IDisposable { Action Cb { get; set; } }
    interface IScoped : IDisposable { Action Cb { get; set; } }
    interface ISingle : IDisposable { Action Cb { get; set; } }

    class Service : ITransit, IScoped, ISingle
    {
        public Action Cb { get; set; }

        public void Dispose()
        {
            this.Cb?.Invoke();
        }
    }

    /// <summary>
    /// 印证我的想法
    /// </summary>
    [TestMethod]
    public void di_test_transient()
    {
        var provider = new ServiceCollection()
            .AddTransient<ITransit, Service>()
            .AddScoped<IScoped, Service>()
            .AddSingleton<ISingle, Service>()
            .BuildServiceProvider().SetAsRootServiceProvider();

        var singleDispose = 0;
        var scopedDispose = 0;
        var transitDispose = 0;

        Action addSingle = () => ++singleDispose;
        Action addScoped = () => ++scopedDispose;
        Action addTransit = () => ++transitDispose;

        var singleObject = provider.GetRequiredService<ISingle>();
        singleObject.Cb = addSingle;

        //作用域1
        using (var s = provider.CreateScope())
        {
            var anotherSingleObject = s.ServiceProvider.GetRequiredService<ISingle>();
            anotherSingleObject.Cb = addSingle;
            singleObject.Should().Be(anotherSingleObject);
        }

        //作用域2
        using (var s = provider.CreateScope())
        {
            var anotherSingleObject = s.ServiceProvider.GetRequiredService<ISingle>();
            anotherSingleObject.Cb = addSingle;
            singleObject.Should().Be(anotherSingleObject);

            var scopedObject = s.ServiceProvider.GetRequiredService<IScoped>();
            scopedObject.Cb = addScoped;
            var anotherScopedObject = s.ServiceProvider.GetRequiredService<IScoped>();
            anotherScopedObject.Cb = addScoped;
            scopedObject.Should().Be(anotherScopedObject);

            var transitObject = s.ServiceProvider.GetRequiredService<ITransit>();
            transitObject.Cb = addTransit;
            var anotherTransitObject = s.ServiceProvider.GetRequiredService<ITransit>();
            anotherTransitObject.Cb = addTransit;
            transitObject.Should().NotBe(anotherTransitObject);
        }
        //出了作用域
        singleDispose.Should().Be(0);
        scopedDispose.Should().Be(1);
        transitDispose.Should().Be(2);
    }
}