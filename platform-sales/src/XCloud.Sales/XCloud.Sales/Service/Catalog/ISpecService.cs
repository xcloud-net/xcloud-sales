using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Service.Catalog;

public interface ISpecService : ISalesAppService
{
    Task<Spec> QuerySpecByIdAsync(int specId);
    
    Task<SpecValue> QuerySpecValueByIdAsync(int specValueId);
    
    Task<SpecDto[]> AttachDataAsync(SpecDto[] data, GoodsSpecAttachDataInput dto);

    Task<KeyValueDto<Spec, SpecValue>[]> GetSpecAndValueByGoodsIdAsync(int goodsId);

    Task<SpecDto[]> QuerySpecByGoodsIdAsync(int goodsId);

    Task UpdateSpecValueStatusAsync(UpdateGoodsSpecValueStatusInput dto);

    Task UpdateSpecValueAsync(SpecValue goodsSpecValue);

    Task UpdateSpecStatusAsync(UpdateGoodsSpecStatusInput dto);

    Task UpdateSpecAsync(Spec goodsSpec);

    Task InsertSpecAsync(Spec goodsSpec);

    Task InsertSpecValueAsync(SpecValue goodsSpecValue);
}

public class SpecService : SalesAppService, ISpecService
{
    private readonly ISalesRepository<Spec> _goodsSpecRepository;
    private readonly ISalesRepository<SpecValue> _goodsSpecValueRepository;

    public SpecService(
        ISalesRepository<Spec> goodsSpecRepository,
        ISalesRepository<SpecValue> goodsSpecValueRepository)
    {
        _goodsSpecRepository = goodsSpecRepository;
        _goodsSpecValueRepository = goodsSpecValueRepository;
    }

    public async Task<Spec> QuerySpecByIdAsync(int specId)
    {
        if (specId <= 0)
            throw new ArgumentNullException(nameof(specId));
        
        var spec = await this._goodsSpecRepository.QueryOneAsync(x => x.Id == specId);
        return spec;
    }
    
    public async Task<SpecValue> QuerySpecValueByIdAsync(int specValueId)
    {
        if (specValueId <= 0)
            throw new ArgumentNullException(nameof(specValueId));
        
        var specValue = await this._goodsSpecValueRepository.QueryOneAsync(x => x.Id == specValueId);
        return specValue;
    }

    public async Task<SpecDto[]> AttachDataAsync(SpecDto[] data, GoodsSpecAttachDataInput dto)
    {
        if (ValidateHelper.IsEmptyCollection(data))
            return data;

        var db = await _goodsSpecRepository.GetDbContextAsync();

        if (dto.Values)
        {
            var ids = data.Select(x => x.Id).Distinct().ToArray();
            var values = await db.Set<SpecValue>().AsNoTracking()
                .Where(x => ids.Contains(x.GoodsSpecId)).ToArrayAsync();
            foreach (var m in data)
            {
                m.Values = values.Where(x => x.GoodsSpecId == m.Id).ToArray();
            }
        }

        return data;
    }

    public virtual async Task InsertSpecAsync(Spec goodsSpec)
    {
        if (goodsSpec == null)
            throw new ArgumentNullException(nameof(goodsSpec));

        if (goodsSpec.GoodsId <= 0)
            throw new ArgumentNullException(nameof(goodsSpec.GoodsId));

        await _goodsSpecRepository.InsertAsync(goodsSpec);
    }

    public virtual async Task UpdateSpecAsync(Spec goodsSpec)
    {
        var db = await _goodsSpecRepository.GetDbContextAsync();
        var set = db.Set<Spec>();

        var spec = await set.FirstOrDefaultAsync(x => x.Id == goodsSpec.Id);

        if (spec == null)
            throw new EntityNotFoundException(nameof(UpdateSpecAsync));

        spec.Name = goodsSpec.Name;

        await db.TrySaveChangesAsync();
    }

    public virtual async Task UpdateSpecStatusAsync(UpdateGoodsSpecStatusInput dto)
    {
        var db = await _goodsSpecRepository.GetDbContextAsync();
        var set = db.Set<Spec>().IgnoreQueryFilters();

        var spec = await set.FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (spec == null)
            throw new EntityNotFoundException(nameof(UpdateSpecStatusAsync));

        if (dto.IsDeleted != null)
            spec.IsDeleted = dto.IsDeleted.Value;

        await db.TrySaveChangesAsync();
    }

    public virtual async Task<KeyValueDto<Spec, SpecValue>[]> GetSpecAndValueByGoodsIdAsync(int goodsId)
    {
        var db = await _goodsSpecRepository.GetDbContextAsync();

        var query = from spec in db.Set<Spec>().AsNoTracking().IgnoreQueryFilters()
            join value in db.Set<SpecValue>().AsNoTracking().IgnoreQueryFilters()
                on spec.Id equals value.GoodsSpecId
            select new
            {
                spec,
                value
            };

        query = query.Where(x => x.spec.GoodsId == goodsId);

        var data = await query.ToArrayAsync();

        var res = data.Select(x => new KeyValueDto<Spec, SpecValue>(x.spec, x.value)).ToArray();

        return res;
    }

    public virtual async Task<SpecDto[]> QuerySpecByGoodsIdAsync(int goodsId)
    {
        var db = await _goodsSpecRepository.GetDbContextAsync();

        var data = await db.Set<Spec>().AsNoTracking().Where(x => x.GoodsId == goodsId).ToArrayAsync();

        var res = data.Select(x => this.ObjectMapper.Map<Spec, SpecDto>(x)).ToArray();

        return res;
    }

    public virtual async Task InsertSpecValueAsync(SpecValue goodsSpecValue)
    {
        if (goodsSpecValue == null)
            throw new ArgumentNullException(nameof(goodsSpecValue));

        if (goodsSpecValue.GoodsSpecId <= 0)
            throw new ArgumentNullException(nameof(goodsSpecValue.GoodsSpecId));

        await _goodsSpecValueRepository.InsertAsync(goodsSpecValue);
    }

    public virtual async Task UpdateSpecValueAsync(SpecValue goodsSpecValue)
    {
        var db = await _goodsSpecRepository.GetDbContextAsync();
        var set = db.Set<SpecValue>();

        var entity = await set.FirstOrDefaultAsync(x => x.Id == goodsSpecValue.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateSpecValueAsync));

        entity.Name = goodsSpecValue.Name;

        await db.TrySaveChangesAsync();
    }

    public virtual async Task UpdateSpecValueStatusAsync(UpdateGoodsSpecValueStatusInput dto)
    {
        var db = await _goodsSpecRepository.GetDbContextAsync();
        var set = db.Set<SpecValue>().IgnoreQueryFilters();

        var entity = await set.FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateSpecValueStatusAsync));

        if (dto.IsDeleted != null)
            entity.IsDeleted = dto.IsDeleted.Value;

        await db.TrySaveChangesAsync();
    }
}