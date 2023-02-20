using Castle.DynamicProxy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace XCloud.Test;

public class SimpleLogInterceptor : IInterceptor
{
    public SimpleLogInterceptor() { }


    private readonly Action<string> action;
    public SimpleLogInterceptor(Action<string> action)
    {
        this.action = action;
    }

    public void Intercept(Castle.DynamicProxy.IInvocation invocation)
    {
        var name = invocation.Method.Name;
        /*
        if (!name.StartsWith("set", StringComparison.CurrentCultureIgnoreCase))
            return;
            */

        //对接口的proxy这里是未实现
        //invocation.Proceed();

        this.action?.Invoke(name);
    }
}

public class XxInterceptor : IInterceptor
{
    public XxInterceptor() { }

    public void Intercept(Castle.DynamicProxy.IInvocation invocation)
    {
        invocation.Proceed();
    }
}

[TestClass]
public class CastleDynamicProxyTest
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

        public void Throw() => throw new NotImplementedException();
    }

    [TestMethod]
    public void castle_proxy_with_target()
    {
        int change = 0;

        var obj = new SimpleSamepleEntity() { };

        var generator = new Castle.DynamicProxy.ProxyGenerator();
        var entity = (SimpleSamepleEntity)generator.CreateClassProxyWithTarget(
            classToProxy: typeof(SimpleSamepleEntity),
            target: obj,
            interceptors: new SimpleLogInterceptor(column => ++change));

        entity.Name = "Richie";
        entity.Age = 50;

        change.Should().Be(2);
    }

    [TestMethod]
    public void castle_proxy_without_target()
    {
        int change = 0;

        var generator = new Castle.DynamicProxy.ProxyGenerator();
        var entity = generator.CreateInterfaceProxyWithoutTarget<ISimple>(
            interceptors: new SimpleLogInterceptor(column => ++change));

        entity.Name = "Richie";

        change.Should().Be(1);
    }

    [TestMethod]
    public void castle_proxy_throw()
    {
        var obj = new SimpleSamepleEntity() { };

        var generator = new Castle.DynamicProxy.ProxyGenerator();
        var entity = (SimpleSamepleEntity)generator.CreateClassProxyWithTarget(
            classToProxy: typeof(SimpleSamepleEntity),
            target: obj,
            interceptors: new XxInterceptor());

        new Action(() => entity.Throw()).Should().Throw<NotImplementedException>();

    }
}