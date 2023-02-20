using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XCloud.AspNetMvc.RequestCache;
using XCloud.Core.Dto;

namespace XCloud.Test;

[TestClass]
public class ThreadTest
{
    void __wait__() => Thread.Sleep(TimeSpan.FromSeconds(1));

    [TestMethod]
    public void Parallel_test1()
    {
        var data = XCloud.Core.Helper.Com.Range(5);

        var res = Parallel.ForEach(data, x =>
        {
            this.__wait__();
        });
    }

    [TestMethod]
    public void Parallel_test2()
    {
        var data = XCloud.Core.Helper.Com.Range(5);

        data.AsParallel().ForAll(x =>
        {
            this.__wait__();
        });
    }

    [TestMethod]
    public void semaphore_test()
    {
        Action releaseWithoutWait = () =>
        {
            using (var sem = new SemaphoreSlim(1, 1))
                sem.Release();
        };

        releaseWithoutWait.Should().Throw<SemaphoreFullException>();

        Action releaseWithWait = () =>
        {
            using (var sem = new SemaphoreSlim(1, 1))
            {
                sem.Wait();
                sem.Release();
            }
        };

        releaseWithWait.Should().NotThrow();
    }

    static readonly ThreadLocal<int> ThreadLocal = new ThreadLocal<int>();
    static readonly AsyncLocal<int> AsyncLocal = new AsyncLocal<int>();

    [TestMethod]
    public async Task test_async_local()
    {
        var data = 9;

        AsyncLocal.Value = data;

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        await this.test_async_local_children(data, 3);

        (AsyncLocal.Value == data).Should().BeTrue();
    }

    async Task test_async_local_children(int expect, int newData)
    {
        (AsyncLocal.Value == expect).Should().BeTrue();

        AsyncLocal.Value = newData;

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        await this.test_async_local_children_2(newData, 6);

        (AsyncLocal.Value == newData).Should().BeTrue();
    }

    async Task test_async_local_children_2(int expect, int newData)
    {
        (AsyncLocal.Value == expect).Should().BeTrue();

        AsyncLocal.Value = newData;

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        (AsyncLocal.Value == newData).Should().BeTrue();
    }

    [TestMethod]
    public async Task test_thread_local()
    {
        ThreadLocal.Value = 9;

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        (ThreadLocal.Value == 9).Should().BeFalse();
    }

    [TestMethod]
    public async Task test_async_local_cache()
    {
        var provider = new AsyncLocalRequestCache();

        provider.SetObject("d", new List<string>() { });

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        provider.GetObject<IEnumerable<string>>("d").IsSuccess().Should().BeTrue();
    }
}