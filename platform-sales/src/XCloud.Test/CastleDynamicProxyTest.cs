using Castle.DynamicProxy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace XCloud.Test;

public interface ISimple
{
    string Name { get; set; }
}

public class SimpleEntity
{
    public virtual string Name { get; set; }

    //public void set_Name(string n) { }

    public void Throw() => throw new NotImplementedException();
}

public class WatchFieldsChangeInterceptor : IInterceptor
{
    public WatchFieldsChangeInterceptor(bool invoke)
    {
        this._invoke = invoke;
    }


    private readonly Action<string> _actionOrNull;
    private readonly bool _invoke;

    public WatchFieldsChangeInterceptor(Action<string> action, bool invoke) : this(invoke)
    {
        this._actionOrNull = action;
    }

    public void Intercept(Castle.DynamicProxy.IInvocation invocation)
    {
        var name = invocation.Method.Name;
        /*
        if (!name.StartsWith("set", StringComparison.CurrentCultureIgnoreCase))
            return;
            */

        this._actionOrNull?.Invoke(name);
        
        if (this._invoke)
        {
            invocation.Proceed();
        }
    }
}

[TestClass]
public class CastleDynamicProxyTest
{
    [TestMethod]
    public void castle_proxy_with_target()
    {
        int change = 0;

        var obj = new SimpleEntity() { };

        var generator = new Castle.DynamicProxy.ProxyGenerator();
        var entity = (SimpleEntity)generator.CreateClassProxyWithTarget(
            classToProxy: typeof(SimpleEntity),
            target: obj,
            interceptors: new WatchFieldsChangeInterceptor(column => ++change, true));

        entity.Name = "Richie";
        Console.Write(entity.Name);

        //get and set
        change.Should().Be(2);
    }

    [TestMethod]
    public void castle_proxy_without_target()
    {
        int change = 0;

        var generator = new Castle.DynamicProxy.ProxyGenerator();
        var entity = generator.CreateInterfaceProxyWithoutTarget<ISimple>(
            interceptors: new WatchFieldsChangeInterceptor(column => ++change, false));

        entity.Name = "Richie";

        change.Should().Be(1);
    }

    [TestMethod]
    public void castle_proxy_throw()
    {
        var obj = new SimpleEntity() { };

        var generator = new Castle.DynamicProxy.ProxyGenerator();
        var entity = (SimpleEntity)generator.CreateClassProxyWithTarget(
            classToProxy: typeof(SimpleEntity),
            target: obj,
            interceptors: new WatchFieldsChangeInterceptor(null, true));

        entity.Name = "test-name";

        new Action(() => entity.Throw()).Should().Throw<NotImplementedException>();
    }
}