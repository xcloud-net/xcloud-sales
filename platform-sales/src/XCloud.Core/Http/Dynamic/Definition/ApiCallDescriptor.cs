using System.Threading;
using FluentAssertions;

namespace XCloud.Core.Http.Dynamic.Definition;

public class ApiCallDescriptor
{
    /// <summary>
    /// 方法定义
    /// </summary>
    public ActionDefinition ActionDefinition { get; }
    /// <summary>
    /// 请求参数
    /// </summary>
    public IReadOnlyDictionary<ParameterDefinition, object> Args { get; }

    public CancellationToken CancellationToken { get; }

    public ApiCallDescriptor(ActionDefinition actionDefinition, IDictionary<ParameterDefinition, object> args, CancellationToken cancellationToken)
    {
        actionDefinition.Should().NotBeNull();
        args.Should().NotBeNull();

        this.ActionDefinition = actionDefinition;
        this.Args = args.ToDict(x => x.Key, x => x.Value);
        this.CancellationToken = cancellationToken;
    }
}