using DotNetCore.CAP;
using XCloud.Core.Dto;
using XCloud.Sales.Application;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Search;
using XCloud.Sales.ViewService;

namespace XCloud.Sales.Queue.Subscribe;

[UnitOfWork]
public class GoodsConsumer : SalesAppService, ICapSubscribe
{
    private readonly ISpecService _goodsSpecService;
    private readonly IGoodsSearchService _goodsSearchService;
    private readonly ISpecCombinationService _specCombinationService;
    private readonly IGoodsViewService _goodsViewService;

    public GoodsConsumer(ISpecService goodsSpecService,
        IGoodsSearchService goodsSearchService,
        IGoodsViewService goodsViewService,
        ISpecCombinationService specCombinationService)
    {
        this._goodsViewService = goodsViewService;
        this._goodsSpecService = goodsSpecService;
        this._goodsSearchService = goodsSearchService;
        this._specCombinationService = specCombinationService;
    }

    [CapSubscribe(SalesMessageTopics.FormatCombinationSpecs)]
    public virtual async Task FormatGoodsSpecCombinationAsync(FormatGoodsSpecCombinationInput dto)
    {
        await this._specCombinationService.FormatSpecCombinationAsync(dto);
    }

    [CapSubscribe(SalesMessageTopics.RefreshGoodsInfo)]
    public virtual async Task RefreshGoodsInfo(IdDto<int> dto)
    {
        if (dto == null || dto.Id <= 0)
            return;

        await this._specCombinationService.UpdateGoodsCombinationInfoAsync(dto.Id);
        await this._goodsSearchService.UpdateGoodsKeywordsAsync(dto.Id);
        await this._goodsViewService.QueryGoodsDetailByIdAsync(dto.Id, new CachePolicy() { Refresh = true });
    }
}