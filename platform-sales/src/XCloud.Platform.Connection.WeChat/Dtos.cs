
using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Connection.WeChat;

public class WxMpConfig : IEntityDto
{
    public string AppID { get; set; }

    public string AppSecret { get; set; }
}

public class WxOpenConfig : IEntityDto
{
    public string AppID { get; set; }

    public string AppSecret { get; set; }

    public string MchID { get; set; }

    public string Key { get; set; }

    public string NotifyUrl { get; set; }

    public string ServerIp { get; set; } = "0.0.0.0";

    public string SSLCertPath { get; set; }

    public string SSLCertPassword { get; set; }

    public string SignType { get; set; } = "MD5";
}