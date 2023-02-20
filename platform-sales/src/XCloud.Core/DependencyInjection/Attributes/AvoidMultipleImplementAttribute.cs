namespace XCloud.Core.DependencyInjection.Attributes;

/// <summary>
/// 检查重复注册实例，标签放在接口或者抽象类上
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class AvoidMultipleImplementAttribute : Attribute { }