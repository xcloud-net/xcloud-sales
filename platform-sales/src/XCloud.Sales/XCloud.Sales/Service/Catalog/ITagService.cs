using XCloud.Application.Extension;
using XCloud.Application.Mapper;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Service.Catalog;

public interface ITagService : ISalesPagingStringAppService<Tag, TagDto, QueryTagPagingInput>
{
    Task<TagDto[]> QueryAllAsync();

    Task UpdateStatusAsync(UpdateTagStatusInput dto);
}

public class TagService : SalesPagingStringAppService<Tag, TagDto, QueryTagPagingInput>, ITagService
{
    private readonly ISalesRepository<Tag> _tagRepository;

    public TagService(ISalesRepository<Tag> tagRepository) : base(tagRepository)
    {
        this._tagRepository = tagRepository;
    }

    protected override async Task<IQueryable<Tag>> GetPagingFilteredQueryableAsync(IQueryable<Tag> query, QueryTagPagingInput dto)
    {
        await Task.CompletedTask;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            query = query.Where(x => x.Name.Contains(dto.Name));
        
        return query;
    }

    public async Task<TagDto[]> QueryAllAsync()
    {
        var query = await _tagRepository.GetQueryableAsync();

        var data = await query.TakeUpTo5000().ToArrayAsync();

        var res = this.ObjectMapper.MapArray<Tag, TagDto>(data);

        return res;
    }

    protected override async Task InitBeforeInsertAsync(Tag entity)
    {
        await base.InitBeforeInsertAsync(entity);
        entity.IsDeleted = false;
    }

    protected override async Task CheckBeforeInsertAsync(TagDto dto)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new UserFriendlyException("tag name is required");
    }

    protected override async Task CheckBeforeUpdateAsync(TagDto dto)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new UserFriendlyException("tag name is required");
    }

    protected override async Task ModifyFieldsForUpdateAsync(Tag entity, TagDto dto)
    {
        await Task.CompletedTask;
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Alert = dto.Alert;
        entity.Link = dto.Link;
    }

    public async Task UpdateStatusAsync(UpdateTagStatusInput dto)
    {
        var db = await _tagRepository.GetDbContextAsync();

        var tag = await db.Set<Tag>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (tag == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.IsDeleted != null)
            tag.IsDeleted = dto.IsDeleted.Value;

        await db.TrySaveChangesAsync();
    }
}