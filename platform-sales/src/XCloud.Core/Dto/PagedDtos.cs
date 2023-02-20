using FluentAssertions;
using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;
using XCloud.Core.Helper;

namespace XCloud.Core.Dto;

public class PagedRequest : IEntityDto
{
    public PagedRequest()
    {
        this.Page = 1;
        this.PageSize = 20;
        this.SkipCalculateTotalCount = false;
    }

    public bool SkipCalculateTotalCount { get; set; } = false;

    public int Page { get; set; }

    public int PageSize { get; set; }

    public void NormalizePageParameters(int maxPageSize)
    {
        this.PageSize = Math.Min(maxPageSize, this.PageSize);
        this.PageSize = Math.Max(1, this.PageSize);

        this.Page = Math.Max(1, this.Page);
    }

    public PagedResultRequestDto AsAbpPagedRequestDto()
    {
        this.ThrowIfException();

        return new PagedResultRequestDto()
        {
            SkipCount = Com.GetPagedSkip(this.Page, this.PageSize),
            MaxResultCount = this.PageSize
        };
    }

    public void ThrowIfException(int? maxPageSize = null)
    {
        maxPageSize ??= 1000;

        (this.Page >= 1).Should().BeTrue();
        (this.PageSize >= 1 && this.PageSize <= maxPageSize.Value).Should().BeTrue($"max page size:{maxPageSize}");
    }

    public static implicit operator PagedResultRequestDto(PagedRequest dto) => dto.AsAbpPagedRequestDto();
}

/// <summary>
/// Paged list
/// </summary>
/// <typeparam name="T">T</typeparam>
public class PagedResponse<T> : ApiResponse<T>, IPagedResult<T>
{
    public PagedResponse() { }

    public PagedResponse(IEnumerable<T> source, PagedRequest dto, int totalCount) : this(dto.Page, dto.PageSize)
    {
        TotalCount = totalCount;
        this.Items = source.ToArray();
    }

    public PagedResponse(int page, int pagesize)
    {
        this.PageIndex = Math.Max(page, 1);
        this.PageSize = Math.Max(pagesize, 1);
    }

    public bool IsNotEmpty => ValidateHelper.IsNotEmptyCollection(this.Items);

    public bool IsEmpty => !this.IsNotEmpty;

    public int TotalPages
    {
        get
        {
            if (PageSize <= 0 || TotalCount <= 0)
            {
                return default;
            }

            var item_count = TotalCount;
            var page_size = PageSize;

            var res = item_count % page_size == 0 ?
                item_count / page_size :
                item_count / page_size + 1;

            return (int)res;
        }
    }

    /// <summary>
    /// 每页显示数量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageIndex { get; set; }

    private IReadOnlyList<T> _items;
        
    /// <summary>
    /// not null
    /// </summary>
    [NotNull]
    public IReadOnlyList<T> Items
    {
        get => this._items ?? (this._items = Array.Empty<T>());
        set
        {
            this._items = value ?? Array.Empty<T>();
        }
    }

    public long TotalCount { get; set; }
}