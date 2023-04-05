using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Common;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Common;
using XCloud.Sales.Service.Logging;

namespace XCloud.Sales.Mall.Api.Controller.User;

[Route("api/mall/pages")]
public class PagesController : ShopBaseController
{
    private readonly IPagesService _pagesService;

    public PagesController(IPagesService pagesService)
    {
        this._pagesService = pagesService;
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<PagesDto>> PagingAsync([FromBody] QueryPagesInput dto)
    {
        dto.SkipCalculateTotalCount = true;
        dto.IsPublished = true;

        var res = await this._pagesService.QueryPagingAsync(dto);

        await this._pagesService.AttachDataAsync(res.Items.ToArray(),
            new AttachPageDataInput() { CoverImage = true });

        return res;
    }

    [HttpPost("by-id")]
    public async Task<ApiResponse<PagesDto>> QueryByIdAsync([FromBody] IdDto dto)
    {
        var data = await this._pagesService.QueryByIdAsync(dto.Id);

        if (data == null)
            throw new EntityNotFoundException(nameof(QueryByIdAsync));

        var page = this.ObjectMapper.Map<Pages, PagesDto>(data);
        
        await this._pagesService.AttachDataAsync(new[] { page }, new AttachPageDataInput() { CoverImage = true });

        var loginUser = await this.StoreAuthService.GetStoreUserOrNullAsync();
        if (loginUser != null)
            await this.SalesEventBusService.NotifyInsertActivityLog(new ActivityLog()
            {
                ActivityLogTypeId = (int)ActivityLogType.VisitPage,
                UserId = loginUser.Id,
                SubjectId = page.Id,
                SubjectType = ActivityLogSubjectType.Page,
                Comment = "添加收藏"
            });

        return new ApiResponse<PagesDto>(page);
    }

    [HttpPost("by-name")]
    public async Task<ApiResponse<PagesDto>> QueryByNameAsync([FromBody] NameDto dto)
    {
        var page = await this._pagesService.QueryPagesBySeoNameAsync(dto.Name);

        if (page == null)
            throw new EntityNotFoundException(nameof(QueryByNameAsync));

        return await this.QueryByIdAsync(new IdDto(page.Id));
    }
}