using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Aftersale;

namespace XCloud.Sales.Service.AfterSale;

public interface IAfterSaleCommentService : ISalesAppService
{
    Task InsertAsync(AfterSalesCommentDto dto);

    Task<PagedResponse<AfterSalesCommentDto>> QueryPagingAsync(QueryAfterSalesCommentPagingInput dto);
}

public class AfterSaleCommentService : SalesAppService, IAfterSaleCommentService
{
    private readonly ISalesRepository<AfterSalesComment> _repository;
    private readonly IAfterSaleService _afterSaleService;
    private readonly AfterSaleUtils _afterSaleUtils;

    public AfterSaleCommentService(ISalesRepository<AfterSalesComment> repository, IAfterSaleService afterSaleService, AfterSaleUtils afterSaleUtils)
    {
        _repository = repository;
        _afterSaleService = afterSaleService;
        _afterSaleUtils = afterSaleUtils;
    }

    private async Task InsertCheckAsync(AfterSalesCommentDto dto)
    {
        var entity = await this._afterSaleService.QueryByIdAsync(dto.AfterSaleId);
        if (entity == null)
            throw new EntityNotFoundException(nameof(InsertCheckAsync));

        if (!this._afterSaleUtils.PendingStatus().Contains(entity.AfterSalesStatusId))
            throw new UserFriendlyException("");
        
        var db = await this._repository.GetDbContextAsync();

        var start = this.Clock.Now.Date;
        var end = start.AddDays(1);

        var query = db.Set<AfterSalesComment>()
            .Where(x => x.AfterSaleId == dto.AfterSaleId)
            .Where(x => !x.IsAdmin)
            .Where(x => x.CreationTime >= start && x.CreationTime < end);

        var count = await query.CountAsync();
        if (count > 5)
            throw new UserFriendlyException("try comment tomorrow");
    }

    public async Task InsertAsync(AfterSalesCommentDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.AfterSaleId))
            throw new ArgumentNullException(nameof(dto.AfterSaleId));

        await this.InsertCheckAsync(dto);

        var entity = this.ObjectMapper.Map<AfterSalesCommentDto, AfterSalesComment>(dto);

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;
        
        await this._repository.InsertAsync(entity);
    }

    public async Task<PagedResponse<AfterSalesCommentDto>> QueryPagingAsync(QueryAfterSalesCommentPagingInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.AfterSalesId))
            throw new ArgumentNullException(nameof(dto.AfterSalesId));

        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<AfterSalesComment>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(dto.AfterSalesId))
            query = query.Where(x => x.AfterSaleId == dto.AfterSalesId);

        var count = await query.CountOrDefaultAsync(dto);

        var list = await query
            .OrderByDescending(x => x.CreationTime)
            .PageBy(dto.AsAbpPagedRequestDto())
            .ToArrayAsync();

        var items = this.ObjectMapper.MapArray<AfterSalesComment, AfterSalesCommentDto>(list);

        return new PagedResponse<AfterSalesCommentDto>(items, dto, count);
    }
}