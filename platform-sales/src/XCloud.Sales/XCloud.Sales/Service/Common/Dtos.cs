using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Shared.Dto;
using XCloud.Sales.Data.Domain.Common;

namespace XCloud.Sales.Service.Common;

public class AttachPageDataInput : IEntityDto
{
    public bool CoverImage { get; set; }
}

public class QueryPagesInput : PagedRequest, IEntityDto
{
    public string Keyword { get; set; }
    public bool? IsPublished { get; set; }
    public bool? IsDeleted { get; set; } = false;
    public bool? SortForAdmin { get; set; }
}

public class SetPageContentInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string Content { get; set; }
}

public class PagesDto : Pages, IEntityDto<string>
{
    public StorageMetaDto Cover { get; set; }
}

public class UpdatePagesStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? IsPublished { get; set; }
    public bool? IsDeleted { get; set; }
}