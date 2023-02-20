using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Aftersale;

namespace XCloud.Sales.Services.AfterSale;

public interface IAfterSalesCommentService : ISalesAppService
{
    Task InsertAsync(AfterSalesCommentDto dto);

    Task<PagedResponse<AfterSalesCommentDto>> QueryPagingAsync(QueryAfterSalesCommentPagingInput dto);
}

public class AfterSalesCommentService : SalesAppService, IAfterSalesCommentService
{
    private readonly ISalesRepository<AfterSalesComment> _repository;

    public AfterSalesCommentService(ISalesRepository<AfterSalesComment> repository)
    {
        _repository = repository;
    }

    public async Task InsertAsync(AfterSalesCommentDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.AfterSaleId))
            throw new ArgumentNullException(nameof(dto.AfterSaleId));

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