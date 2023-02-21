using XCloud.Sales.Application;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.ViewService;

public interface IGoodsViewService : ISalesAppService
{
    Task<GoodsDto> QueryGoodsDetailByIdAsync(int goodsId, CachePolicy cachePolicyOption);
}

public class GoodsViewService : SalesAppService, IGoodsViewService
{
    private readonly IGoodsService _goodsService;

    public GoodsViewService(IGoodsService goodsService)
    {
        this._goodsService = goodsService;
    }

    private async Task<GoodsDto> QueryGoodsDetailByIdAsync(int goodsId)
    {
        var goods = await _goodsService.QueryByIdAsync(goodsId);
        if (goods == null || !goods.Published)
            return null;

        goods = (await this._goodsService.AttachDataAsync(new[] { goods }, new AttachGoodsDataInput()
        {
            Images = true,
            Brand = true,
            Category = true,
            Tags = true,
            GoodsAttributes = true
        })).First();

        return goods;
    }

    public async Task<GoodsDto> QueryGoodsDetailByIdAsync(int goodsId, CachePolicy cachePolicyOption)
    {
        var key = $"mall.goods.detail.id={goodsId}";
        var option = new CacheOption<GoodsDto>(key, TimeSpan.FromMinutes(30));

        var goods = await this.CacheProvider.ExecuteWithPolicyAsync(() => this.QueryGoodsDetailByIdAsync(goodsId),
            option,
            cachePolicyOption);

        return goods;
    }
}