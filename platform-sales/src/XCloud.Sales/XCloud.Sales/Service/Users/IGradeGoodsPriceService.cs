using XCloud.Sales.Application;
using XCloud.Core.Application.Entity;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Service.Users;

public interface IGradeGoodsPriceService : ISalesAppService
{
    Task<GoodsSpecCombinationDto[]> AttachGradePriceAsync(
        GoodsSpecCombinationDto[] combinations, string gradeId);

    Task<GoodsGradePriceDto[]> AttachDataAsync(GoodsGradePriceDto[] data,
        GoodsGradePriceAttachDataInput dto);

    Task DeleteGradePriceAsync(string gradePriceId);

    Task SetGradePriceAsync(GoodsGradePriceDto dto);

    Task SaveGradePriceAsync(int combinationId, GoodsGradePriceDto[] gradePrices);
}

public class GradeGoodsPriceService : SalesAppService, IGradeGoodsPriceService
{
    private readonly IUserGradeService _userGradeService;
    private readonly ISalesRepository<GoodsGradePrice> _gradePriceRepository;

    public GradeGoodsPriceService(ISalesRepository<GoodsGradePrice> gradePriceRepository,
        IUserGradeService userGradeService)
    {
        this._gradePriceRepository = gradePriceRepository;
        _userGradeService = userGradeService;
    }

    public async Task<GoodsGradePriceDto[]> AttachDataAsync(GoodsGradePriceDto[] data,
        GoodsGradePriceAttachDataInput dto)
    {
        if (data == null || dto == null)
            throw new ArgumentNullException(nameof(AttachDataAsync));
        if (!data.Any())
            return data;

        var db = await this._gradePriceRepository.GetDbContextAsync();

        if (dto.GoodsInfo)
        {
            var combinationIds = data.Select(x => x.GoodsCombinationId).Distinct().ToArray();
            var query = from combination in db.Set<GoodsSpecCombination>().AsNoTracking()
                join g in db.Set<Goods>().AsNoTracking()
                    on combination.GoodsId equals g.Id into goodsGrouping
                from goods in goodsGrouping.DefaultIfEmpty()
                select new { combination, goods };

            query = query.Where(x => combinationIds.Contains(x.combination.Id));
            var goodsInfo = await query.ToArrayAsync();

            foreach (var m in data)
            {
                var gradePriceGoods = goodsInfo.FirstOrDefault(x => x.combination.Id == m.GoodsCombinationId);
                if (gradePriceGoods == null)
                    continue;

                m.GoodsSpecCombination =
                    this.ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(gradePriceGoods.combination);
                if (gradePriceGoods.goods != null)
                    m.Goods = this.ObjectMapper.Map<Goods, GoodsDto>(gradePriceGoods.goods);
            }
        }

        return data;
    }

    public async Task SaveGradePriceAsync(int combinationId, GoodsGradePriceDto[] gradePrices)
    {
        if (combinationId <= 0 || gradePrices == null)
            throw new ArgumentNullException(nameof(SaveGradePriceAsync));

        var db = await this._gradePriceRepository.GetDbContextAsync();
        var combination = await db.Set<GoodsSpecCombination>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == combinationId);
        if (combination == null)
            throw new EntityNotFoundException(nameof(combination));

        foreach (var m in gradePrices)
        {
            m.GoodsId = combination.GoodsId;
            m.GoodsCombinationId = combination.Id;
            if (m.Price < 0)
                throw new UserFriendlyException("价格不能小于0");
            if (string.IsNullOrWhiteSpace(m.GradeId))
                throw new ArgumentNullException(nameof(m.GradeId));
        }

        var set = db.Set<GoodsGradePrice>();

        var allGradePrices = await set.Where(x => x.GoodsCombinationId == combination.Id).ToArrayAsync();
        var toSaveGradePrices = gradePrices
            .Select(x => this.ObjectMapper.Map<GoodsGradePriceDto, GoodsGradePrice>(x)).ToArray();

        string FingerPrint(GoodsGradePrice x) => $"{x.GoodsCombinationId}-{x.GradeId}-{x.Price}";

        var toAdd = toSaveGradePrices.NotInBy(allGradePrices, x => FingerPrint(x)).ToArray();
        var toDelete = allGradePrices.NotInBy(toSaveGradePrices, x => FingerPrint(x)).ToArray();

        if (toAdd.Any())
        {
            foreach (var m in toAdd)
            {
                m.Id = this.GuidGenerator.CreateGuidString();
                m.CreationTime = this.Clock.Now;
            }

            set.AddRange(toAdd);
        }

        if (toDelete.Any())
            set.RemoveRange(toDelete);

        await db.TrySaveChangesAsync();
    }

    private async Task<GoodsGradePriceDto[]> QueryGradePriceOffsetsAsync(int[] goodsIds, string gradeId)
    {
        var db = await _gradePriceRepository.GetDbContextAsync();

        var query = from gradePriceOffset in db.Set<GoodsGradePrice>().AsNoTracking()
            join grade in db.Set<UserGrade>().AsNoTracking()
                on gradePriceOffset.GradeId equals grade.Id
            select new { gradePriceOffset, grade };

        query = query.Where(x =>
            goodsIds.Contains(x.gradePriceOffset.GoodsId) && x.gradePriceOffset.GradeId == gradeId);

        var data = await query.ToArrayAsync();

        var list = new List<GoodsGradePriceDto>();

        foreach (var m in data)
        {
            var dto = ObjectMapper.Map<GoodsGradePrice, GoodsGradePriceDto>(m.gradePriceOffset);
            dto.Grade = m.grade;
            list.Add(dto);
        }

        return list.ToArray();
    }

    public async Task<IDictionary<int, GoodsGradePriceDto>> CalculateUserCombinationGradePriceAsync(
        int[] combinationIds, string gradeId)
    {
        if (string.IsNullOrWhiteSpace(gradeId))
            throw new ArgumentNullException(nameof(gradeId));

        if (ValidateHelper.IsEmptyCollection(combinationIds))
            return new Dictionary<int, GoodsGradePriceDto>();

        var db = await _gradePriceRepository.GetDbContextAsync();

        var query = from combination in db.Set<GoodsSpecCombination>().AsNoTracking()
            join goods in db.Set<Goods>().AsNoTracking()
                on combination.GoodsId equals goods.Id
            select combination;

        query = query.Where(x => combinationIds.Contains(x.Id));

        var data = await query.ToArrayAsync();

        var goodsIds = data.Select(x => x.GoodsId).Distinct().ToArray();

        var allGoodsGradePriceOffsets = await QueryGradePriceOffsetsAsync(goodsIds, gradeId);

        var dict = new Dictionary<int, GoodsGradePriceDto>();

        foreach (var m in data)
        {
            var goodsGradePriceOffsets = allGoodsGradePriceOffsets.Where(x => x.GoodsId == m.GoodsId).ToArray();

            var selectedGradePriceOffset = goodsGradePriceOffsets
                .Where(x => x.GoodsCombinationId == m.Id)
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefault(x => x.GradeId == gradeId);

            dict[m.Id] = selectedGradePriceOffset;
        }

        return dict;
    }

    public async Task<GoodsSpecCombinationDto[]> AttachGradePriceAsync(
        GoodsSpecCombinationDto[] combinations, string gradeId)
    {
        if (string.IsNullOrWhiteSpace(gradeId))
            throw new ArgumentNullException(nameof(gradeId));

        if (!combinations.Any())
            return combinations;

        var combinationIds = combinations.Ids().ToArray();
        var combinationGradePrice = await CalculateUserCombinationGradePriceAsync(
            combinationIds, gradeId);

        foreach (var m in combinations)
        {
            var selectedGradePrice = combinationGradePrice[m.Id];
            m.SelectedGradePrice = selectedGradePrice;
        }

        return combinations;
    }

    public async Task DeleteGradePriceAsync(string gradePriceId)
    {
        if (string.IsNullOrWhiteSpace(gradePriceId))
            throw new ArgumentNullException(nameof(gradePriceId));

        var offset = await _gradePriceRepository.QueryOneAsync(x => x.Id == gradePriceId);
        if (offset != null)
            await _gradePriceRepository.DeleteNowAsync(offset);
    }

    public async Task SetGradePriceAsync(GoodsGradePriceDto dto)
    {
        var db = await _gradePriceRepository.GetDbContextAsync();

        var combination = await db.Set<GoodsSpecCombination>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.GoodsCombinationId && x.GoodsId == dto.GoodsId);

        if (combination == null)
            throw new UserFriendlyException("combination not exist");

        var set = db.Set<GoodsGradePrice>();

        var offsets = await set.Where(x => x.GoodsCombinationId == combination.Id && x.GradeId == dto.GradeId)
            .ToArrayAsync();

        if (offsets.Any())
            set.RemoveRange(offsets);

        var entity = ObjectMapper.Map<GoodsGradePriceDto, GoodsGradePrice>(dto);

        entity.Id = GuidGenerator.CreateGuidString();
        entity.CreationTime = Clock.Now;
        if (entity.Price < decimal.Zero)
            throw new UserFriendlyException("一口价不能小于0");

        set.Add(entity);

        await db.TrySaveChangesAsync();
    }
}