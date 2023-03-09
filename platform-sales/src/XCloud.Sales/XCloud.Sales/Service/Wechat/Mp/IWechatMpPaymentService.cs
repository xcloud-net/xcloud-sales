using Microsoft.Extensions.Options;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using XCloud.Core;
using XCloud.Core.Helper;
using XCloud.Platform.Connection.WeChat;
using XCloud.Platform.Shared.Settings;
using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Finance;
using XCloud.Sales.Service.Finance;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Service.Wechat.Mp;

public interface IWechatMpPaymentService : ISalesAppService
{
    //
}

public class WechatMpPaymentService : SalesAppService, IWechatMpPaymentService
{
    private readonly IOptions<WechatMpOption> _wechatMpOption;
    private readonly IOptions<PlatformServiceAddressOption> _platformServiceAddressOption;
    private readonly IOrderService _orderService;
    private readonly OrderUtils _orderUtils;
    private readonly IOrderBillService _orderBillService;
    private readonly IUserWechatMpConnectionService _wechatMpConnectionService;

    private WeChatPayOption WechatPaymentOption => this._wechatMpOption.Value.Payment;

    public WechatMpPaymentService(IOptions<WechatMpOption> wechatMpOption, IOrderService orderService,
        OrderUtils orderUtils,
        IOrderBillService orderBillService,
        IOptions<PlatformServiceAddressOption> platformServiceAddressOption,
        IUserWechatMpConnectionService wechatMpConnectionService)
    {
        _wechatMpOption = wechatMpOption;
        _orderService = orderService;
        _orderUtils = orderUtils;
        _orderBillService = orderBillService;
        _platformServiceAddressOption = platformServiceAddressOption;
        _wechatMpConnectionService = wechatMpConnectionService;
    }

    private WechatTenpayClient GetRequiredPaymentClient()
    {
        if (this.WechatPaymentOption == null)
            throw new ConfigException(nameof(this.WechatPaymentOption));

        var option = new WechatTenpayClientOptions()
        {
            MerchantId = this.WechatPaymentOption.MchId,
            MerchantV3Secret = this.WechatPaymentOption.APIv3Key,
            MerchantCertificatePrivateKey = this.WechatPaymentOption.APIPrivateKey
        };
        return new WechatTenpayClient(option);
    }

    private async Task<OrderBill> GetRequiredOrderBillAsync(string billId)
    {
        var bill = await this._orderBillService.QueryByIdAsync(billId);
        if (bill == null)
            throw new EntityNotFoundException(nameof(GetRequiredOrderBillAsync));

        if (bill.Paid)
            throw new UserFriendlyException("bill is paid already");

        return bill;
    }

    private async Task<string> GetRequiredOpenIdAsync(int userId)
    {
        var connection = await this._wechatMpConnectionService.QueryUserWechatMpConnectionAsync(userId);

        if (connection == null || string.IsNullOrWhiteSpace(connection.OpenId))
            throw new UserFriendlyException("pls bind wechat first");

        return connection.OpenId;
    }

    public async Task CreatePaymentOrderAsync(string billId)
    {
        var bill = await this.GetRequiredOrderBillAsync(billId);

        var order = await this._orderService.QueryByIdAsync(bill.OrderId);
        if (order == null)
            throw new EntityNotFoundException(nameof(order));

        var openId = await this.GetRequiredOpenIdAsync(order.UserId);

        var wechatPaymentClient = this.GetRequiredPaymentClient();

        var request = new CreatePayTransactionJsapiRequest()
        {
            OutTradeNumber = bill.Id,
            Description = string.Empty,
            NotifyUrl = Com.ConcatUrl(this._platformServiceAddressOption.Value.PublicGateway,
                this.WechatPaymentOption.PaymentNotifyUrl),
            Detail = new CreatePayPartnerTransactionJsapiRequest.Types.Detail()
            {
                GoodsList = new List<CreatePayTransactionAppRequest.Types.Detail.Types.GoodsDetail>()
                {
                    new CreatePayTransactionAppRequest.Types.Detail.Types.GoodsDetail()
                    {
                        GoodsName = "bill payment"
                    }
                }
            },
            Amount = new CreatePayPartnerTransactionJsapiRequest.Types.Amount()
            {
                Total = this._orderUtils.CastMoneyToCent(bill.Price)
            },
            Payer = new CreatePayTransactionJsapiRequest.Types.Payer()
            {
                OpenId = openId
            }
        };

        var response = await wechatPaymentClient.ExecuteCreatePayTransactionJsapiAsync(request);

        if (!response.IsSuccessful())
            throw new UserFriendlyException("failed to create payment order").WithData("wechat-response",
                this.JsonDataSerializer.SerializeToString(response));

        throw new NotImplementedException();
    }
}