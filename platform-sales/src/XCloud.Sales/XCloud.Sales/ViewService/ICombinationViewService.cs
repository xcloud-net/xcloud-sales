using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Services.Catalog;
using XCloud.Core.Dto;

namespace XCloud.Sales.ViewService;

public interface ICombinationViewService : ISalesAppService
{
    Task<SpecGroupSelectResponse> QuerySpecGroupMenuAsync(QueryCombinationInput dto);

    Task<GoodsSpecCombinationDto[]> QueryAvailableCombinationsAsync(QueryCombinationInput dto);
}

public class CombinationViewService : SalesAppService, ICombinationViewService
{
    private readonly ISpecService _goodsSpecService;
    private readonly ISpecCombinationParser _goodsSpecParser;
    private readonly IGoodsSpecCombinationService _goodsSpecCombinationService;

    public CombinationViewService(ISpecService goodsSpecService,
        ISpecCombinationParser goodsSpecParser,
        IGoodsSpecCombinationService goodsSpecCombinationService)
    {
        _goodsSpecService = goodsSpecService;
        _goodsSpecParser = goodsSpecParser;
        _goodsSpecCombinationService = goodsSpecCombinationService;
        //
    }

    private void CheckQueryCombinationInput(QueryCombinationInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (dto.GoodsId <= 0)
            throw new ArgumentNullException(nameof(dto.GoodsId));
        
        var selection = dto.Specs;
        if (selection == null)
            throw new ArgumentNullException(nameof(selection));

        if (selection.GroupBy(x => x.SpecId).Any(x => x.Count() > 1))
            throw new BusinessException("selection error");
    }

    private bool IsCombinationMatched(GoodsSpecCombinationDto combination,
        SpecCombinationItemDto[] selection)
    {
        if (!selection.Any())
            return true;

        if (!combination.ParsedSpecificationsJson.Any())
            return false;

        var combinationFingerPrints =
            combination.ParsedSpecificationsJson.Select(x => _goodsSpecParser.FingerPrint(x)).ToArray();
        var selectedFingerPrints = selection.Select(x => _goodsSpecParser.FingerPrint(x)).ToArray();

        var containAll = selectedFingerPrints.All(x => combinationFingerPrints.Contains(x));

        return containAll;
    }

    private GoodsSpecCombinationDto[] GetMatchedCombinations(GoodsSpecCombinationDto[] combinations,
        SpecCombinationItemDto[] conditions)
    {
        if (!conditions.Any())
            return combinations;

        var matchedCombinations = new List<GoodsSpecCombinationDto>();

        foreach (var m in combinations)
        {
            var allMatched = IsCombinationMatched(m, conditions);
            if (!allMatched)
                continue;

            matchedCombinations.Add(m);
        }

        return matchedCombinations.ToArray();
    }

    private async Task<GoodsSpecCombinationDto[]> GetGoodsActiveCombinationsAsync(int goodsId)
    {
        var allCombinations = await this._goodsSpecCombinationService.QueryByGoodsIdAsync(goodsId);

        var combinations = allCombinations
            .Where(x => x.IsActive)
            .Select(x => this.ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(x))
            .ToArray();

        foreach (var m in combinations)
        {
            m.ParsedSpecificationsJson = this._goodsSpecParser.DeserializeSpecCombination(m.SpecificationsJson);
        }

        return combinations;
    }

    private IEnumerable<SpecGroup> BuildSpecGroups(KeyValueDto<Spec, SpecValue>[] specAndValueList)
    {
        var groupedSpecList = specAndValueList
            .GroupBy(x => x.Key.Id)
            .Select(x => new { SpecId = x.Key, Items = x.ToArray() })
            .ToArray();

        foreach (var g in groupedSpecList)
        {
            var group = new SpecGroup(g.Items.First().Key);

            var items = new List<SpecGroupItem>();

            //var groupValueIds = g.Items.Select(x => x.Value.Id).ToArray();
            foreach (var item in g.Items)
            {
                var valueItem = new SpecGroupItem(item.Value)
                {
                    IsActive = default,
                    IsSelected = default
                };

                items.Add(valueItem);
            }

            group.Items = items.ToArray();

            yield return group;
        }
    }

    private SpecGroup[] AttachSpecGroupStatus(SpecGroup[] groups,
        QueryCombinationInput dto,
        GoodsSpecCombinationDto[] combinations)
    {
        foreach (var group in groups)
        {
            foreach (var item in group.Items)
            {
                item.IsActive = default;
                item.IsSelected = dto.Specs.Any(x => x.SpecId == group.Id && x.SpecValueId == item.Id);

                if (!item.IsSelected)
                {
                    var candidateSelection = dto.Specs
                        .Append(new SpecCombinationItemDto() { SpecId = group.Id, SpecValueId = item.Id })
                        .ToArray();

                    item.IsActive = combinations.Any(x => IsCombinationMatched(x, candidateSelection));
                }
            }
        }

        return groups;
    }

    public async Task<SpecGroupSelectResponse> QuerySpecGroupMenuAsync(QueryCombinationInput dto)
    {
        this.CheckQueryCombinationInput(dto);

        var combinations = await this.GetGoodsActiveCombinationsAsync(dto.GoodsId);

        var specAndValueList = await this._goodsSpecService.GetSpecAndValueByGoodsIdAsync(dto.GoodsId);

        var groups = this.BuildSpecGroups(specAndValueList).ToArray();

        groups = this.AttachSpecGroupStatus(groups, dto, combinations);

        var matchedCombinations = this.GetMatchedCombinations(combinations, dto.Specs);

        var response = new SpecGroupSelectResponse()
        {
            SpecGroup = groups,
            FilteredCombinations = matchedCombinations
        };

        return response;
    }

    public async Task<GoodsSpecCombinationDto[]> QueryAvailableCombinationsAsync(QueryCombinationInput dto)
    {
        this.CheckQueryCombinationInput(dto);

        var combinations = await this.GetGoodsActiveCombinationsAsync(dto.GoodsId);

        var matchedCombinations = this.GetMatchedCombinations(combinations, dto.Specs);

        return matchedCombinations;
    }
}