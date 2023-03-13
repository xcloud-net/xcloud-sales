using XCloud.Application.Extension;
using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Shared.Extension;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Media;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Media;
using XCloud.Sales.Service.Stores;

namespace XCloud.Sales.Service.Catalog;

public interface ISpecCombinationService : ISalesAppService
{
    Task<GoodsSpecCombinationDto[]> QueryGoodsCombinationForSelectionAsync(
        QueryCombinationForSelectionInput dto);

    Task FormatSpecCombinationAsync(FormatGoodsSpecCombinationInput dto);

    Task<GoodsSpecCombination> InsertAsync(GoodsSpecCombinationDto dto);

    Task<GoodsSpecCombination> UpdateAsync(GoodsSpecCombinationDto m);

    Task SetSpecCombinationAsync(GoodsSpecCombinationDto dto);

    Task<GoodsSpecCombinationDto[]> AttachDataAsync(
        GoodsSpecCombinationDto[] data,
        GoodsCombinationAttachDataInput dto);

    Task UpdateGoodsCombinationInfoAsync(int goodsId);

    Task UpdateStatusAsync(UpdateGoodsSpecCombinationStatusInput dto);

    Task<GoodsSpecCombination[]> QueryByGoodsIdAsync(int goodsId);

    Task<GoodsSpecCombination> QueryByIdAsync(int combinationId);

    Task<GoodsSpecCombinationDto> QueryBySkuAsync(string sku);
}

public class SpecCombinationService : SalesAppService, ISpecCombinationService
{
    private readonly ISalesRepository<Goods> _goodsRepository;
    private readonly ISpecCombinationParser _goodsSpecParser;
    private readonly ISpecService _goodsSpecService;
    private readonly IPictureService _pictureService;

    public SpecCombinationService(ISalesRepository<Goods> goodsRepository,
        ISpecCombinationParser goodsSpecParser,
        ISpecService goodsSpecService,
        IPictureService pictureService)
    {
        _goodsRepository = goodsRepository;
        _goodsSpecParser = goodsSpecParser;
        _goodsSpecService = goodsSpecService;
        _pictureService = pictureService;
    }

    public async Task<GoodsSpecCombinationDto[]> QueryGoodsCombinationForSelectionAsync(
        QueryCombinationForSelectionInput dto)
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var query = from combination in db.Set<GoodsSpecCombination>().AsNoTracking()
            join goods in db.Set<Goods>().AsNoTracking()
                on combination.GoodsId equals goods.Id
            select new { combination, goods };

        if (ValidateHelper.IsNotEmptyCollection(dto.ExcludedCombinationIds))
            query = query.Where(x => !dto.ExcludedCombinationIds.Contains(x.combination.Id));

        if (!string.IsNullOrWhiteSpace(dto.Keywords))
        {
            query = query.Where(x =>
                x.goods.Name.Contains(dto.Keywords) ||
                x.goods.ShortDescription.Contains(dto.Keywords) ||
                x.combination.Name.Contains(dto.Keywords));
        }

        query = query.OrderByDescending(x => x.goods.CreationTime);

        var data = await query.Take(dto.Take).ToArrayAsync();

        var list = new List<GoodsSpecCombinationDto>();

        foreach (var m in data)
        {
            var combination = ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(m.combination);

            combination.Goods = ObjectMapper.Map<Goods, GoodsDto>(m.goods);

            list.Add(combination);
        }

        return list.ToArray();
    }

    public async Task FormatSpecCombinationAsync(FormatGoodsSpecCombinationInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (dto.Id <= 0)
            throw new ArgumentNullException(nameof(dto.Id));

        var db = await this._goodsRepository.GetDbContextAsync();

        var specAndValues = await this._goodsSpecService.GetSpecAndValueByGoodsIdAsync(dto.Id);
        var allSpecCombinations = specAndValues
            .Select(x => new SpecCombinationItemDto()
            {
                SpecId = x.Key.Id,
                SpecValueId = x.Value.Id
            })
            .ToArray();

        var combinations = await db.Set<GoodsSpecCombination>().Where(x => x.GoodsId == dto.Id).ToArrayAsync();

        var now = this.Clock.Now;
        foreach (var m in combinations)
        {
            var specCombination = this._goodsSpecParser.DeserializeSpecCombination(m.SpecificationsJson);
            var validSpecCombination =
                specCombination.InBy(allSpecCombinations, this._goodsSpecParser.FingerPrint).ToArray();
            m.SpecificationsJson = this._goodsSpecParser.SerializeSpecCombination(validSpecCombination);
            m.LastModificationTime = now;
        }

        await db.TrySaveChangesAsync();
    }

    public async Task<GoodsSpecCombinationDto[]> AttachDataAsync(GoodsSpecCombinationDto[] data,
        GoodsCombinationAttachDataInput dto)
    {
        if (data == null || dto == null)
            throw new ArgumentNullException(nameof(AttachDataAsync));

        if (!data.Any())
            return data;

        var ids = data.Ids().Distinct().ToArray();

        var db = await this._goodsRepository.GetDbContextAsync();

        if (dto.DeserializeSpecCombinationJson)
        {
            foreach (var m in data)
            {
                m.ParsedSpecificationsJson ??= this._goodsSpecParser.DeserializeSpecCombination(m.SpecificationsJson);
            }
        }

        //load goods combination/spec and spec value/spec description
        if (dto.SpecCombinationDetail)
        {
            var specCombinations = data.Where(x => x.ParsedSpecificationsJson != null)
                .SelectMany(x => x.ParsedSpecificationsJson).ToArray();
            if (specCombinations.Any())
            {
                var specValueIds = specCombinations.Select(x => x.SpecValueId).Distinct().ToArray();

                var query = from value in db.Set<SpecValue>().AsNoTracking()
                    join spec in db.Set<Spec>().AsNoTracking()
                        on value.GoodsSpecId equals spec.Id
                    select new { value, spec };
                var specAndValues = await query.Where(x => specValueIds.Contains(x.value.Id)).ToArrayAsync();

                foreach (var m in specCombinations)
                {
                    var specValuePair = specAndValues.FirstOrDefault(x =>
                        x.value.Id == m.SpecValueId && x.spec.Id == m.SpecId);
                    if (specValuePair == null)
                        continue;
                    m.Spec = this.ObjectMapper.Map<Spec, SpecDto>(specValuePair.spec);
                    m.SpecValue = this.ObjectMapper.Map<SpecValue, SpecValueDto>(specValuePair.value);
                }
            }
        }

        if (dto.CalculateSpecCombinationErrors)
        {
            var goodsIds = data.Select(x => x.GoodsId).Distinct().ToArray();
            var query = from value in db.Set<SpecValue>().AsNoTracking()
                join spec in db.Set<Spec>().AsNoTracking()
                    on value.GoodsSpecId equals spec.Id
                select new { value, spec };
            var specAndValues = await query.Where(x => goodsIds.Contains(x.spec.GoodsId)).ToArrayAsync();

            var combinationsGroupbyGoods = data
                .GroupBy(x => x.GoodsId)
                .Select(x => new { x.Key, Items = x.ToArray() }).ToArray();

            foreach (var g in combinationsGroupbyGoods)
            {
                var goodsId = g.Key;
                var scopedSpecAndValues = specAndValues
                    .Where(x => x.spec.GoodsId == goodsId)
                    .Select(x => new KeyValueDto<Spec, SpecValue>(x.spec, x.value))
                    .ToArray();

                foreach (var m in g.Items)
                {
                    if (m.ParsedSpecificationsJson == null)
                        continue;
                    this.CalculateSpecCombinationErrors(scopedSpecAndValues, m.ParsedSpecificationsJson,
                        out var errors);
                    m.SpecCombinationErrors = errors;
                }
            }
        }

        if (dto.GradePrices)
        {
            var query = from gradePrice in db.Set<GoodsGradePrice>().AsNoTracking()
                join grade in db.Set<UserGrade>().AsNoTracking()
                    on gradePrice.GradeId equals grade.Id
                select new { gradePrice, grade };

            query = query.Where(x => ids.Contains(x.gradePrice.GoodsCombinationId));

            var gradePrices = await query.ToArrayAsync();

            foreach (var m in data)
            {
                var res = new List<GoodsGradePriceDto>();

                var combinationGradePrices =
                    gradePrices.Where(x => x.gradePrice.GoodsCombinationId == m.Id).ToArray();

                foreach (var price in combinationGradePrices)
                {
                    var priceDto = ObjectMapper.Map<GoodsGradePrice, GoodsGradePriceDto>(price.gradePrice);
                    priceDto.Grade = price.grade;

                    res.Add(priceDto);
                }

                m.AllGradePrices = res.ToArray();
            }
        }

        if (dto.Stores)
        {
            var query = from storeMapping in db.Set<StoreGoodsMapping>().AsNoTracking()
                join store in db.Set<Store>().AsNoTracking()
                    on storeMapping.StoreId equals store.Id
                select new
                {
                    storeMapping,
                    store
                };

            query = query.Where(x => ids.Contains(x.storeMapping.GoodsCombinationId));

            var stores = await query.TakeUpTo5000().ToArrayAsync();

            foreach (var m in data)
            {
                var storesList = stores
                    .Where(x => x.storeMapping.GoodsCombinationId == m.Id)
                    .Select(x => x.store).ToArray();
                m.Stores = storesList.Select(x => this.ObjectMapper.Map<Store, StoreDto>(x)).ToArray();
            }
        }

        if (dto.Goods)
        {
            var goodsIds = data.Select(x => x.GoodsId).Distinct().ToArray();
            var goodsList = await db.Set<Goods>().WhereIdIn(goodsIds).ToArrayAsync();

            foreach (var m in data)
            {
                var g = goodsList.FirstOrDefault(x => x.Id == m.GoodsId);
                if(g==null)
                    continue;
                m.Goods = this.ObjectMapper.Map<Goods, GoodsDto>(g);
            }
        }

        if (dto.Images)
        {
            var query = from goodsPicture in db.Set<GoodsPicture>().AsNoTracking()
                join picture in db.Set<Picture>().AsNoTracking()
                    on goodsPicture.PictureId equals picture.Id
                select new
                {
                    goodsPicture,
                    picture
                };

            query = query.Where(x => ids.Contains(x.goodsPicture.CombinationId));

            var pics = await query
                .OrderBy(x => x.goodsPicture.DisplayOrder)
                .TakeUpTo5000().ToArrayAsync();

            foreach (var m in data)
            {
                var xdata = pics
                    .Where(x => x.goodsPicture.GoodsId == m.Id)
                    .OrderBy(x => x.goodsPicture.DisplayOrder)
                    .ToArray();

                var pictureList = new List<MallStorageMetaDto>();
                foreach (var x in xdata)
                {
                    var meta = await this._pictureService.DeserializePictureMetaAsync(x.picture);
                    meta.PictureId = x.picture.Id;
                    meta.GoodsId = x.goodsPicture.GoodsId;
                    meta.CombinationId = x.goodsPicture.CombinationId;
                    meta.DisplayOrder = x.goodsPicture.DisplayOrder;
                    meta.Simplify();
                    pictureList.Add(meta);
                }

                m.XPictures = pictureList.ToArray();
            }
        }

        return data;
    }

    public async Task UpdateStatusAsync(UpdateGoodsSpecCombinationStatusInput dto)
    {
        if (dto == null || dto.Id <= 0)
            throw new ArgumentNullException(nameof(dto));

        var db = await _goodsRepository.GetDbContextAsync();
        var set = db.Set<GoodsSpecCombination>();

        var combination = await set.IgnoreQueryFilters().Where(x => x.Id == dto.Id).FirstOrDefaultAsync();

        if (combination == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.IsDeleted != null)
        {
            combination.IsDeleted = dto.IsDeleted.Value;
            combination.DeletionTime = combination.IsDeleted ? this.Clock.Now : null;
        }

        if (dto.IsActive != null)
            combination.IsActive = dto.IsActive.Value;

        combination.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public async Task UpdateGoodsCombinationInfoAsync(int goodsId)
    {
        var db = await _goodsRepository.GetDbContextAsync();

        var goods = await db.Set<Goods>().FirstOrDefaultAsync(x => x.Id == goodsId);
        if (goods == null)
            return;

        var combinations = await db.Set<GoodsSpecCombination>().AsNoTracking()
            .Where(x => x.IsActive)
            .Where(x => x.GoodsId == goods.Id)
            .ToArrayAsync();

        goods.MinPrice = 0;
        goods.MaxPrice = 0;
        goods.StockQuantity = 0;

        if (combinations.Any())
        {
            goods.MinPrice = combinations.Min(x => x.Price);
            goods.MaxPrice = combinations.Max(x => x.Price);
            goods.StockQuantity = combinations.Sum(x => x.StockQuantity);
        }

        await db.TrySaveChangesAsync();
    }

    public virtual async Task<GoodsSpecCombination[]> QueryByGoodsIdAsync(int goodsId)
    {
        if (goodsId <= 0)
            return Array.Empty<GoodsSpecCombination>();

        var db = await _goodsRepository.GetDbContextAsync();

        var query = db.Set<GoodsSpecCombination>().AsNoTracking().Where(x => x.GoodsId == goodsId);

        var combinations = await query.TakeUpTo5000().ToArrayAsync();
        return combinations;
    }

    private string NormalizeSku(string sku) => sku.Trim().RemoveWhitespace();

    private string NormalizeName(string name) => name?.Trim().RemoveWhitespace();

    private async Task<bool> CheckIsSkuExistAsync(DbContext db, string sku, int? exceptId = null)
    {
        var query = db.Set<GoodsSpecCombination>().IgnoreQueryFilters();
        query = query.Where(x => x.Sku == sku);
        if (exceptId != null)
            query = query.Where(x => x.Id != exceptId.Value);

        return await query.AnyAsync();
    }

    private void CheckCombinationInput(GoodsSpecCombinationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new UserFriendlyException("combination name is empty");
        }
    }

    public async Task<GoodsSpecCombination> QueryByIdAsync(int combinationId)
    {
        if (combinationId <= 0)
            throw new ArgumentNullException(nameof(combinationId));

        var db = await this._goodsRepository.GetDbContextAsync();

        var entity = await db.Set<GoodsSpecCombination>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == combinationId);

        return entity;
    }

    public async Task<GoodsSpecCombinationDto> QueryBySkuAsync(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentNullException(nameof(sku));

        var db = await _goodsRepository.GetDbContextAsync();
        var query = from combination in db.Set<GoodsSpecCombination>().AsNoTracking()
            join goods in db.Set<Goods>().AsNoTracking()
                on combination.GoodsId equals goods.Id
            select new { combination, goods };

        query = query.Where(x => x.combination.Sku == sku);

        var data = await query.OrderByDescending(x => x.combination.Id).FirstOrDefaultAsync();

        if (data == null)
            return null;

        var dto = ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(data.combination);
        dto.Goods = ObjectMapper.Map<Goods, GoodsDto>(data.goods);

        return dto;
    }

    private void CalculateSpecCombinationErrors(
        KeyValueDto<Spec, SpecValue>[] specAndValues,
        SpecCombinationItemDto[] combinations,
        out string[] errors)
    {
        if (specAndValues == null)
            throw new ArgumentNullException(nameof(specAndValues));

        if (combinations == null)
            throw new ArgumentNullException(nameof(combinations));

        var errorList = new List<string>();

        //所有配置项都是存在的
        foreach (var m in combinations)
        {
            var existInAllCombinations = specAndValues.Any(x => x.Key.Id == m.SpecId && x.Value.Id == m.SpecValueId);
            if (!existInAllCombinations)
                errorList.Add("invalid parameter exist");
        }

        //每个规格分组有且只有一个配置项
        var grouped = specAndValues
            .GroupBy(x => x.Key.Id)
            .Select(x => new
            {
                SpecId = x.Key,
                Items = x.ToArray()
            })
            .ToArray();

        foreach (var g in grouped)
        {
            var specGroup = g.Items.First().Key;

            var groupedOptions = g.Items
                .Select(x => new SpecCombinationItemDto(x.Key.Id, x.Value.Id))
                .ToArray();

            var scopedSettings = groupedOptions
                .InBy(combinations, this._goodsSpecParser.FingerPrint).ToArray();

            if (!scopedSettings.Any())
            {
                errorList.Add($"config for {specGroup.Name} is required");
                continue;
            }

            if (scopedSettings.Length > 1)
            {
                errorList.Add($"config for {specGroup.Name} is configured for multiple times");
                continue;
            }

            var configValueId = scopedSettings[0].SpecValueId;
            var specValueIds = groupedOptions.Select(d => d.SpecValueId).ToArray();
            if (!specValueIds.Contains(configValueId))
            {
                errorList.Add($"config for {specGroup.Name} is error");
                continue;
            }
        }

        errors = errorList.Distinct().WhereNotEmpty().ToArray();
    }

    private async Task CheckSpecCombinationAsyncV2(int goodsId, GoodsSpecCombinationDto dto)
    {
        var specAndValues = await this._goodsSpecService.GetSpecAndValueByGoodsIdAsync(goodsId);

        this.CalculateSpecCombinationErrors(specAndValues, dto.ParsedSpecificationsJson, out var errors);

        if (errors.Any())
            throw new UserFriendlyException(errors.First());
    }

    public async Task SetSpecCombinationAsync(GoodsSpecCombinationDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Id <= 0)
            throw new ArgumentNullException(nameof(dto.Id));

        if (dto.ParsedSpecificationsJson == null)
            throw new ArgumentNullException(nameof(dto.ParsedSpecificationsJson));

        var db = await this._goodsRepository.GetDbContextAsync();

        var entity = await db.Set<GoodsSpecCombination>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        var specCombinationJson = this._goodsSpecParser.SerializeSpecCombination(dto.ParsedSpecificationsJson);

        //validate when specs combination is not empty
        if (dto.ParsedSpecificationsJson.Any())
        {
            await this.CheckSpecCombinationAsyncV2(entity.GoodsId, dto);

            var allCombinations = await db.Set<GoodsSpecCombination>().AsNoTracking()
                .Where(x => x.GoodsId == entity.GoodsId)
                .ToArrayAsync();

            if (allCombinations.Any(x =>
                    x.Id != entity.Id &&
                    this._goodsSpecParser.AreSpecCombinationEqual(x.SpecificationsJson, specCombinationJson)))
            {
                throw new UserFriendlyException("duplicate spec combinations");
            }
        }

        entity.SpecificationsJson = specCombinationJson;
        entity.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public async Task<GoodsSpecCombination> InsertAsync(GoodsSpecCombinationDto dto)
    {
        this.CheckCombinationInput(dto);

        var entity = this.ObjectMapper.Map<GoodsSpecCombinationDto, GoodsSpecCombination>(dto);

        if (!string.IsNullOrWhiteSpace(entity.Sku))
            entity.Sku = NormalizeSku(entity.Sku);

        entity.Id = default;
        entity.Name = NormalizeName(entity.Name);
        entity.IsDeleted = false;
        entity.DeletionTime = null;
        entity.CreationTime = this.Clock.Now;

        var db = await _goodsRepository.GetDbContextAsync();

        if (!string.IsNullOrWhiteSpace(entity.Sku))
            if (await CheckIsSkuExistAsync(db, entity.Sku))
                throw new UserFriendlyException("sku is already taken");

        var set = db.Set<GoodsSpecCombination>();

        set.Add(entity);

        await db.SaveChangesAsync();

        return entity;
    }

    public async Task<GoodsSpecCombination> UpdateAsync(GoodsSpecCombinationDto m)
    {
        this.CheckCombinationInput(m);

        if (m.Id <= 0)
            throw new ArgumentException(nameof(UpdateAsync));

        if (!string.IsNullOrWhiteSpace(m.Sku))
            m.Sku = NormalizeSku(m.Sku);

        m.Name = NormalizeName(m.Name);

        var db = await _goodsRepository.GetDbContextAsync();

        var set = db.Set<GoodsSpecCombination>();

        var entity = await set.Where(x => x.Id == m.Id).FirstOrDefaultAsync();
        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateAsync));

        entity.Sku = m.Sku;
        entity.Name = m.Name;
        entity.Description = m.Description;
        entity.Weight = m.Weight;
        entity.PictureId = m.PictureId;
        entity.Color = m.Color;
        entity.LastModificationTime = this.Clock.Now;

        entity.CostPrice = m.CostPrice;
        entity.StockQuantity = m.StockQuantity;

        if (!string.IsNullOrWhiteSpace(entity.Sku))
            if (await CheckIsSkuExistAsync(db, entity.Sku, entity.Id))
                throw new UserFriendlyException("sku is already taken");

        await db.SaveChangesAsync();

        return entity;
    }
}