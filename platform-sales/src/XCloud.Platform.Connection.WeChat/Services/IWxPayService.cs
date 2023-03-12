using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
using Volo.Abp.Application.Services;
using XCloud.Platform.Connection.WeChat.Configuration;

namespace XCloud.Platform.Connection.WeChat.Services;

public interface IWxPayService : IApplicationService
{
    //
}

public class WxPayService : ApplicationService, IWxPayService
{
    private readonly IConfiguration _configuration;

    public WxPayService(IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        this._configuration = configuration;
        httpClientFactory.CreateClient(nameof(WxMpService));
    }

    public async Task<CreatePayTransactionJsapiResponse> CreateJsPayAsync()
    {
        var config = _configuration.GetWxMpConfig();

        var wechatOption = new WechatTenpayClientOptions()
        {
            //
        };

        var client = new WechatTenpayClient(wechatOption);

        var request = new CreatePayTransactionJsapiRequest()
        {
            OutTradeNumber = String.Empty,
            AppId = String.Empty,
            Description = String.Empty,
            NotifyUrl = String.Empty,
            Amount = new CreatePayTransactionJsapiRequest.Types.Amount() { Total = 99 },
            Payer = new CreatePayTransactionJsapiRequest.Types.Payer() { OpenId = String.Empty }
        };

        var response = await client.ExecuteCreatePayTransactionJsapiAsync(request);

        if (!response.IsSuccessful())
            throw new WechatPaymentException(nameof(CreateJsPayAsync)) { };

        return response;
    }

    public async Task RefundAsync()
    {
        var config = _configuration.GetWxMpConfig();

        var wechatOption = new WechatTenpayClientOptions()
        {
            MerchantId = default,
            MerchantV3Secret = default,
            MerchantCertificateSerialNumber = default,
            MerchantCertificatePrivateKey = default,
            PlatformCertificateManager = new InMemoryCertificateManager() { },
        };

        var client = new WechatTenpayClient(wechatOption);

        var response = await client.ExecuteCreateRefundDomesticRefundAsync(new CreateRefundDomesticRefundRequest()
        {
            SubMerchantId = default,
            OutTradeNumber = default,
            TransactionId = default,
            OutRefundNumber = default,
            Reason = default,
            Amount = new CreateRefundDomesticRefundRequest.Types.Amount()
            {
                Total = default,
                Refund = default,
                Currency = default
            },
            NotifyUrl = default,
            FundsAccount = default,
        });

        if (!response.IsSuccessful())
            throw new WechatPaymentException(nameof(CreateJsPayAsync)) { };
    }
}