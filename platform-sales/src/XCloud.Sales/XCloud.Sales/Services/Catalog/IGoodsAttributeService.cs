using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Services.Catalog;

public interface IGoodsAttributeService : ISalesAppService
{
    Task UpdateAsync(GoodsAttribute dto);
    
    Task InsertAsync(GoodsAttribute dto);
    
    Task<GoodsAttribute> QueryByIdAsync(string attrId);
    
    Task DeleteByIdAsync(string attrId);
    
    Task SetGoodsAttributesAsync(int goodsId, GoodsAttribute[] goodsAttributes);
    
    Task<GoodsAttribute[]> QueryGoodsAttributesAsync(int goodsId);
}

public class GoodsAttributeService : SalesAppService, IGoodsAttributeService
{
    private readonly ISalesRepository<GoodsAttribute> _salesRepository;

    public GoodsAttributeService(ISalesRepository<GoodsAttribute> salesRepository)
    {
        this._salesRepository = salesRepository;
    }

    public async Task<GoodsAttribute> QueryByIdAsync(string attrId)
    {
        if (string.IsNullOrWhiteSpace(attrId))
            throw new ArgumentNullException(nameof(attrId));

        var attr = await this._salesRepository.QueryOneAsync(x => x.Id == attrId);

        return attr;
    }

    public async Task DeleteByIdAsync(string attrId)
    {
        if (string.IsNullOrWhiteSpace(attrId))
            throw new ArgumentNullException(nameof(attrId));

        await this._salesRepository.DeleteAsync(x => x.Id == attrId);
    }

    private async Task<bool> CheckAttributeNameIsExistAsync(int goodsId, string name, string exceptAttrIdOrNull = null)
    {
        var db = await this._salesRepository.GetDbContextAsync();
        var query = db.Set<GoodsAttribute>().AsNoTracking();

        query = query.Where(x => x.GoodsId == goodsId && x.Name == name);
        if (!string.IsNullOrWhiteSpace(exceptAttrIdOrNull))
            query = query.Where(x => x.Id != exceptAttrIdOrNull);

        return await query.AnyAsync();
    }

    private void CheckGoodsAttributeInput(GoodsAttribute dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Value))
            throw new ArgumentNullException(nameof(InsertAsync));
        if (dto.GoodsId <= 0)
            throw new ArgumentNullException(nameof(dto.GoodsId));
    }

    public async Task InsertAsync(GoodsAttribute dto)
    {
        this.CheckGoodsAttributeInput(dto);

        if (await this.CheckAttributeNameIsExistAsync(dto.GoodsId, dto.Name))
            throw new UserFriendlyException("name exists already");

        dto.Id = this.GuidGenerator.CreateGuidString();
        dto.CreationTime = this.Clock.Now;

        await this._salesRepository.InsertAsync(dto);
    }
        
    public async Task UpdateAsync(GoodsAttribute dto)
    {
        this.CheckGoodsAttributeInput(dto);
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var entity = await this._salesRepository.QueryOneAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));
            
        if (await this.CheckAttributeNameIsExistAsync(dto.GoodsId, dto.Name,entity.Id))
            throw new UserFriendlyException("name exists already");

        entity.Name = dto.Name;
        entity.Value = dto.Value;

        await this._salesRepository.UpdateAsync(entity);
    }

    public async Task SetGoodsAttributesAsync(int goodsId, GoodsAttribute[] goodsAttributes)
    {
        if (goodsId <= 0)
            throw new ArgumentNullException(nameof(goodsId));

        if (goodsAttributes == null)
            throw new ArgumentNullException(nameof(goodsAttributes));

        foreach (var m in goodsAttributes)
        {
            m.Id = null;
            m.GoodsId = goodsId;
            m.Name = m.Name?.Trim();
            m.Value = m.Value?.Trim();
            if (string.IsNullOrWhiteSpace(m.Name) || string.IsNullOrWhiteSpace(m.Value))
                throw new UserFriendlyException("请输入名称和对应的值");
        }

        if (goodsAttributes.GroupBy(x => x.Name).Any(x => x.Count() > 1))
            throw new UserFriendlyException("存储相同的名称");

        var db = await this._salesRepository.GetDbContextAsync();

        var set = db.Set<GoodsAttribute>();

        var data = await set.Where(x => x.GoodsId == goodsId).ToArrayAsync();

        string FinglePrint(GoodsAttribute x) => $"{x.Name}={x.Value}";

        var toAdd = goodsAttributes.NotInBy(data, x => FinglePrint(x)).ToArray();
        var toDelete = data.NotInBy(goodsAttributes, x => FinglePrint(x)).ToArray();

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

    public async Task<GoodsAttribute[]> QueryGoodsAttributesAsync(int goodsId)
    {
        if (goodsId <= 0)
            throw new ArgumentNullException(nameof(goodsId));

        var data = await this._salesRepository.QueryManyAsNoTrackingAsync(x => x.GoodsId == goodsId);
        return data;
    }
}