using System;
using SKIT.FlurlHttpClient.Wechat.Api;
using Volo.Abp;

namespace XCloud.Platform.Connection.WeChat;

public class WechatException : BusinessException
{
    public WechatApiResponse WechatApiResponse { get; set; }

    public WechatException() { }
    public WechatException(string message) : base(message) { }
    public WechatException(string message, Exception e) : base(message: message, innerException: e) { }
    public WechatException(string message, WechatApiResponse wechatApiResponse) : base(message)
    {
        this.WechatApiResponse = wechatApiResponse;
    }
}

public class WechatPaymentException : WechatException
{
    public WechatPaymentException() { }
    public WechatPaymentException(string message) : base(message) { }
    public WechatPaymentException(string message, Exception e) : base(message, e) { }
}