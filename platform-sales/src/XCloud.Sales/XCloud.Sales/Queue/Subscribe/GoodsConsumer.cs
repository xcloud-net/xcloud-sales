using DotNetCore.CAP;
using XCloud.Core.Dto;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.ViewService;

namespace XCloud.Sales.Queue.Subscribe;

[UnitOfWork]
public class GoodsConsumer : SalesAppService, ICapSubscribe
{
    private readonly ISpecService _goodsSpecService;
    private readonly IGoodsSearchService _goodsSearchService;
    private readonly IGoodsSpecCombinationService _goodsSpecCombinationService;
    private readonly IGoodsViewService _goodsViewService;

    public GoodsConsumer(ISpecService goodsSpecService,
        IGoodsSearchService goodsSearchService,
        IGoodsViewService goodsViewService,
        IGoodsSpecCombinationService goodsSpecCombinationService)
    {
        this._goodsViewService = goodsViewService;
        this._goodsSpecService = goodsSpecService;
        this._goodsSearchService = goodsSearchService;
        this._goodsSpecCombinationService = goodsSpecCombinationService;
    }

    [CapSubscribe(SalesMessageTopics.FormatCombinationSpecs)]
    public virtual async Task FormatGoodsSpecCombinationAsync(FormatGoodsSpecCombinationInput dto)
    {
        await this._goodsSpecCombinationService.FormatSpecCombinationAsync(dto);
    }

    [CapSubscribe(SalesMessageTopics.RefreshGoodsInfo)]
    public virtual async Task RefreshGoodsInfo(IdDto<int> dto)
    {
        if (dto == null || dto.Id <= 0)
            return;

        await this._goodsSpecCombinationService.UpdateGoodsCombinationInfoAsync(dto.Id);
        await this._goodsSearchService.UpdateGoodsKeywordsAsync(dto.Id);
        await this._goodsViewService.QueryGoodsDetailByIdAsync(dto.Id, new CachePolicy() { Refresh = true });
    }
}