using XCloud.Core.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace XCloud.Test;

[TestClass]
public class DynamicCallTest
{
    class Mapper<T> where T : class, IDisposable { }

    [TestMethod]
    public void test_generic_parameter_constraint()
    {
        var constranit = typeof(Mapper<>).GetGenericArguments().First().GetGenericParameterConstraints();
    }

    class TagAttribute : Attribute { }

    public class User { }

    [Tag]
    public void Xx(string a) { }

    [Tag]
    public async Task Dd(User u) => await Task.CompletedTask;

    [Tag]
    public async Task<int> Yy(User u) => await Task.FromResult(4);

    [TestMethod]
    public async Task Run()
    {
        var ms = typeof(DynamicCallTest).GetMethods().ToList();
        ms = ms.Where(x => x.GetCustomAttributes_<TagAttribute>().Any()).ToList();

        foreach (var m in ms)
        {
            var res = m.Invoke(this, new object[] { null });
            if (res?.GetType().IsAssignableTo_<Task>() ?? false)
            {
                await (Task)res;
            }
        }
        //ActivatorUtilities.GetServiceOrCreateInstance(null, this.GetType());
    }
}