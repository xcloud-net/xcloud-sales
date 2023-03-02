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

    public PagedResultRequestDto ToAbpPagedRequest()
    {
        this.ThrowIfException();

        return new PagedResultRequestDto()
        {
            SkipCount = PageHelper.GetPagedSkip(this.Page, this.PageSize),
            MaxResultCount = this.PageSize
        };
    }

    public void ThrowIfException(int? maxPageSize = null)
    {
        maxPageSize ??= 1000;

        PageHelper.EnsurePage(this.Page);
        PageHelper.EnsurePageSize(this.PageSize);
        PageHelper.EnsureMaxPageSize(this.PageSize, maxPageSize.Value);
    }

    public static implicit operator PagedResultRequestDto(PagedRequest dto) => dto.ToAbpPagedRequest();
}

/// <summary>
/// Paged list
/// </summary>
/// <typeparam name="T">T</typeparam>
public class PagedResponse<T> : ApiResponse<T>, IPagedResult<T>
{
    public PagedResponse()
    {
        //
    }

    public PagedResponse(IEnumerable<T> source, PagedRequest dto, int totalCount) : this(dto.Page, dto.PageSize)
    {
        this.TotalCount = totalCount;
        this.Items = source.ToArray();
    }

    public PagedResponse(int page, int pageSize) : this()
    {
        PageHelper.EnsurePage(page);
        PageHelper.EnsurePageSize(pageSize);

        this.PageIndex = page;
        this.PageSize = pageSize;
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

            var res = TotalCount % PageSize == 0 ? TotalCount / PageSize : TotalCount / PageSize + 1;

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
        get => this._items ??= Array.Empty<T>();
        set => this._items = value ?? Array.Empty<T>();
    }

    public long TotalCount { get; set; }
}