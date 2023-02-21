using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Promotion;
using XCloud.Sales.Service.Orders.Validator;

namespace XCloud.Sales.Service.Promotion;

public interface IPromotionService : ISalesAppService
{
    Task<int> ActivePromotionCountAsync();

    Task<StorePromotionDto[]> AttachDataAsync(StorePromotionDto[] data, AttachPromotionDataInput dto);

    Task UpdateRulesAsync(StorePromotionDto dto);

    Task<StorePromotionDto> QueryByIdAsync(string promotionId);

    Task UpdateStatusAsync(UpdatePromotionStatusInput dto);

    Task InsertAsync(StorePromotionDto dto);

    Task UpdateAsync(StorePromotionDto dto);

    Task<PagedResponse<StorePromotionDto>> QueryPagingAsync(QueryPromotionPagingInput dto);
}

public class PromotionService : SalesAppService, IPromotionService
{
    private readonly ISalesRepository<StorePromotion> _repository;
    private readonly PromotionUtils _promotionUtils;
    private readonly OrderConditionUtils _orderConditionUtils;

    public PromotionService(ISalesRepository<StorePromotion> repository,
        PromotionUtils promotionUtils,
        OrderConditionUtils orderConditionUtils)
    {
        this._orderConditionUtils = orderConditionUtils;
        this._promotionUtils = promotionUtils;
        this._repository = repository;
    }

    public async Task<int> ActivePromotionCountAsync()
    {
        var db = await this._repository.GetDbContextAsync();
        var query = db.Set<StorePromotion>().AsNoTracking();

        var now = this.Clock.Now;
        query = query.Where(x => x.IsActive);

        query = query.Where(x => x.StartTime == null || x.StartTime.Value < now);
        query = query.Where(x => x.EndTime == null || x.EndTime.Value > now);

        var count = await query.CountAsync();
        return count;
    }

    public async Task<StorePromotionDto[]> AttachDataAsync(StorePromotionDto[] data, AttachPromotionDataInput dto)
    {
        if (!data.Any())
            return data;

        await Task.CompletedTask;

        if (dto.ParseCondition)
        {
            foreach (var m in data)
            {
                m.PromotionConditions =
                    this._orderConditionUtils.DeserializeConditions(m.Condition, throwIfException: false);
                m.PromotionConditions ??= Array.Empty<OrderCondition>();
                var descriptors = new List<string>();

                foreach (var condition in m.PromotionConditions)
                {
                    foreach (var validator in this._orderConditionUtils.ConditionValidators)
                    {
                        var desc = await this._orderConditionUtils.TryResolveDescriptor(validator, condition);
                        if (!string.IsNullOrWhiteSpace(desc))
                            descriptors.Add(desc);
                    }
                }

                m.PromotionConditionDescriptors = descriptors.ToArray();
            }
        }

        if (dto.ParseResults)
        {
            foreach (var m in data)
            {
                m.PromotionResults = this._promotionUtils.DeserializeResults(m.Result, throwIfException: false);
                m.PromotionResults ??= Array.Empty<PromotionResult>();
                var descriptors = new List<string>();

                foreach (var result in m.PromotionResults)
                {
                    foreach (var handler in this._promotionUtils.ResultHandlers)
                    {
                        var desc = await this._promotionUtils.TryResolveDescriptor(handler, result);
                        if (!string.IsNullOrWhiteSpace(desc))
                            descriptors.Add(desc);
                    }
                }

                m.PromotionConditionDescriptors = descriptors.ToArray();
            }
        }

        return data;
    }

    private StorePromotionDto FromEntity(StorePromotion entity)
    {
        var dto = this.ObjectMapper.Map<StorePromotion, StorePromotionDto>(entity);

        return dto;
    }

    public async Task<StorePromotionDto> QueryByIdAsync(string promotionId)
    {
        if (string.IsNullOrWhiteSpace(promotionId))
            return null;

        var entity = await this._repository.QueryOneAsync(x => x.Id == promotionId);
        if (entity == null)
            return null;

        return this.FromEntity(entity);
    }

    public async Task UpdateStatusAsync(UpdatePromotionStatusInput dto)
    {
        var db = await this._repository.GetDbContextAsync();

        var entity = await db.Set<StorePromotion>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (dto.IsDeleted != null)
            entity.IsDeleted = dto.IsDeleted.Value;

        if (dto.IsActive != null)
            entity.IsActive = dto.IsActive.Value;

        if (dto.IsExclusive != null)
            entity.IsExclusive = dto.IsExclusive.Value;

        await db.TrySaveChangesAsync();
    }

    private async Task CheckInsertPromotionInputAsync(StorePromotionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new UserFriendlyException(nameof(dto.Name));

        await Task.CompletedTask;
    }

    public async Task UpdateRulesAsync(StorePromotionDto dto)
    {
        dto = this._promotionUtils.SerializeConfigFields(dto);

        var db = await this._repository.GetDbContextAsync();

        var entity = await db.Set<StorePromotion>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateRulesAsync));

        entity.Condition = dto.Condition;
        entity.Result = dto.Result;

        await this._repository.UpdateAsync(entity);
    }

    public async Task InsertAsync(StorePromotionDto dto)
    {
        await this.CheckInsertPromotionInputAsync(dto);

        var db = await this._repository.GetDbContextAsync();

        var entity = this.ObjectMapper.Map<StorePromotionDto, StorePromotion>(dto);

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.IsActive = false;
        entity.IsDeleted = false;
        entity.CreationTime = this.Clock.Now;

        await this._repository.InsertAsync(entity);
    }

    public async Task UpdateAsync(StorePromotionDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var entity = await this._repository.QueryOneAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Order = dto.Order;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;

        await this._repository.UpdateAsync(entity);
    }

    public async Task<PagedResponse<StorePromotionDto>> QueryPagingAsync(QueryPromotionPagingInput dto)
    {
        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<StorePromotion>().IgnoreQueryFilters().AsNoTracking();

        if (dto.IsDeleted != null)
            query = query.Where(x => x.IsDeleted == dto.IsDeleted.Value);

        if (dto.IsActive != null)
            query = query.Where(x => x.IsActive == dto.IsActive.Value);

        if (dto.AvailableFor != null)
        {
            query = query.Where(x => x.StartTime == null || x.StartTime.Value < dto.AvailableFor.Value);
            query = query.Where(x => x.EndTime == null || x.EndTime.Value > dto.AvailableFor.Value);
        }

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        if (dto.SortForAdmin ?? false)
        {
            query = query.OrderBy(x => x.IsDeleted).ThenByDescending(x => x.CreationTime);
        }
        else
        {
            query = query.OrderBy(x => x.IsDeleted).ThenByDescending(x => x.CreationTime);
        }

        var data = await query.PageBy(dto.AsAbpPagedRequestDto()).ToArrayAsync();

        var response = data.Select(this.FromEntity).ToArray();

        return new PagedResponse<StorePromotionDto>(response, dto, count);
    }
}