using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Connection.WeChat.Settings;

public abstract class WechatOptionBase
{
    private WechatPaymentOption _payment;

    [NotNull]
    public WechatPaymentOption Payment
    {
        get => this._payment ??= new WechatPaymentOption();
        set { this._payment = value; }
    }
}

public class WechatMpOption : WechatOptionBase, IEntityDto
{
    public string AppId { get; set; }

    public string AppSecret { get; set; }
}

public class WechatOpenOption : WechatOptionBase, IEntityDto
{
    public string AppId { get; set; }

    public string AppSecret { get; set; }
}

/// <summary>
/// WeChatPay 配置选项
/// </summary>
public class WechatPaymentOption : IEntityDto
{
    /// <summary>
    /// 应用号
    /// </summary>
    /// <remarks>
    /// 公众号、移动应用、小程序AppId、企业微信CorpId。
    /// </remarks>
    public string AppId { get; set; }

    /// <summary>
    /// 应用密钥
    /// </summary>
    /// <remarks>
    /// 企业微信AppSecret，目前仅调用"企业红包API"时使用。
    /// </remarks>
    public string AppSecret { get; set; }

    /// <summary>
    /// 商户号
    /// </summary>
    /// <remarks>
    /// 商户号、服务商户号
    /// </remarks>
    public string MchId { get; set; }

    /// <summary>
    /// 子商户应用号
    /// </summary>
    /// <remarks>
    /// 目前仅调用服务商API时使用，子商户的公众号、移动应用AppId。
    /// </remarks>
    public string SubAppId { get; set; }

    /// <summary>
    /// 子商户号
    /// </summary>
    /// <remarks>
    /// 目前仅调用服务商API时使用，子商户的商户号。
    /// </remarks>
    public string SubMchId { get; set; }

    /// <summary>
    /// 商户API证书
    /// </summary>
    /// <remarks>
    /// 可为 证书文件路径、证书文件的Base64编码。
    /// </remarks>
    public string Certificate { get; set; }

    /// <summary>
    /// 商户API证书密码
    /// </summary>
    /// <remarks>
    /// 默认为商户号
    /// </remarks>
    public string CertificatePassword { get; set; }

    /// <summary>
    /// 商户API私钥
    /// </summary>
    /// <remarks>
    /// 当配置了P12格式证书时，已包含私钥信息，不必再配置API私钥。PEM格式则必须配置。
    /// </remarks>
    public string APIPrivateKey { get; set; }

    /// <summary>
    /// 商户API密钥
    /// </summary>
    public string APIKey { get; set; }

    /// <summary>
    /// 商户APIv3密钥
    /// </summary>
    public string APIv3Key { get; set; }

    /// <summary>
    /// RSA公钥
    /// </summary>
    /// <remarks>
    /// 目前仅调用"企业付款到银行卡API [V2]"时使用，执行"获取RSA加密公钥API [V2]"即可获取。
    /// </remarks>
    public string RsaPublicKey { get; set; }

    public string PaymentNotifyUrl { get; set; }

    public string RefundNotifyUrl { get; set; }
}