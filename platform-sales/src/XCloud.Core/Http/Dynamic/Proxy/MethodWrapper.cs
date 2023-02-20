using System.Reflection;
using FluentAssertions;

namespace XCloud.Core.Http.Dynamic.Proxy;

public class MethodWrapper
{
    public MethodInfo Method { get; }
    public MethodWrapper(MethodInfo m)
    {
        m.Should().NotBeNull();
        this.Method = m;
    }
}