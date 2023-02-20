using Volo.Abp;

namespace XCloud.Core;

[Serializable]
public class BaseException : AbpException
{
    public BaseException(string msg) : base(msg)
    {
        //
    }

    public BaseException(string msg, Exception inner) : base(msg, inner)
    {
        //
    }
}

public class ConfigException : AbpException
{
    public ConfigException()
    {
        //
    }

    public ConfigException(string message) : base(message)
    {
        //
    }
}

/// <summary>
/// 传参错误
/// </summary>
public class NoParamException : ArgumentNullException
{
    /// <summary>
    /// 接口返回模板
    /// </summary>
    public object ResponseTemplate { get; set; }

    public NoParamException() : base()
    {
        //
    }

    public NoParamException(string paramName) : base(paramName)
    {
        //
    }

    public NoParamException(string msg, Exception inner) : base(msg, inner)
    {
        //
    }
}