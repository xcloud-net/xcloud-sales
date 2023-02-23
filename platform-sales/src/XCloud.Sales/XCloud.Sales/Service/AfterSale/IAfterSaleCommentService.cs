using XCloud.Core.Application;
using XCloud.Core.Dto;
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

    public AfterSaleCommentService(ISalesRepository<AfterSalesComment> repository)
    {
        _repository = repository;
    }

    private async Task InsertCheckAsync(AfterSalesCommentDto dto)
    {
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

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var list = await query
            .OrderBy(x => x.CreationTime)
            .PageBy(dto.AsAbpPagedRequestDto())
            .ToArrayAsync();

        var items = this.ObjectMapper.MapArray<AfterSalesComment, AfterSalesCommentDto>(list);

        return new PagedResponse<AfterSalesCommentDto>(items, dto, count);
    }
}