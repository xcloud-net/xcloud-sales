using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Logging;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controller.User;

/// <summary>
/// 收藏夹
/// </summary>
[Route("api/mall/favorites")]
public class FavoritesController : ShopBaseController
{
    private readonly IFavoritesService _favoritesService;
    private readonly IGoodsService _goodsService;
    private readonly ISpecCombinationService _specCombinationService;

    /// <summary>
    /// 构造器
    /// </summary>
    public FavoritesController(IFavoritesService favoritesService,
        IGoodsService goodsService,
        ISpecCombinationService specCombinationService)
    {
        this._favoritesService = favoritesService;
        this._goodsService = goodsService;
        this._specCombinationService = specCombinationService;
    }

    [HttpPost("pagingv1")]
    public async Task<PagedResponse<FavoritesDto>> PagingAsync([FromBody] QueryFavoritesInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        dto.UserId = loginUser.Id;
        dto.SkipCalculateTotalCount = true;

        var res = await this._favoritesService.QueryPagingAsync(dto);

        var goods = res.Items.Select(x => x.Goods).WhereNotNull().ToArray();

        await this._goodsService.AttachDataAsync(goods, new AttachGoodsDataInput()
        {
            Images = true,
            Combination = true
        });

        return res;
    }

    [HttpPost("like")]
    public async Task<ApiResponse<object>> LikeAsync([FromBody] IdDto<int> dto)
    {
        if (dto.Id <= 0)
            return new ApiResponse<object>().SetError("invalid goods id");

        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        await this._favoritesService.AddFavoritesAsync(loginUser.Id, dto.Id);

        await this._favoritesService.CheckIsFavoritesAsync(loginUser.Id, dto.Id,
            new CachePolicy() { Refresh = true });
            
        await this.EventBusService.NotifyInsertActivityLog(new ActivityLog()
        {
            ActivityLogTypeId = (int)ActivityLogType.AddFavorite,
            UserId = loginUser.Id,
            SubjectIntId = dto.Id,
            SubjectType = ActivityLogSubjectType.Goods,
            Comment = "添加收藏"
        });

        return new ApiResponse<object>();
    }

    [HttpPost("unlike")]
    public async Task<ApiResponse<object>> UnLikeAsync([FromBody] IdDto<int> dto)
    {
        if (dto.Id <= 0)
            return new ApiResponse<object>().SetError("invalid goods id");

        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        await this._favoritesService.DeleteFavoritesAsync(loginUser.Id, dto.Id);

        await this._favoritesService.CheckIsFavoritesAsync(loginUser.Id, dto.Id,
            new CachePolicy() { Refresh = true });

        await this.EventBusService.NotifyInsertActivityLog(new ActivityLog()
        {
            ActivityLogTypeId = (int)ActivityLogType.DeleteFavorite,
            UserId = loginUser.Id,
            SubjectIntId = dto.Id,
            SubjectType = ActivityLogSubjectType.Goods,
            Comment = "取消收藏"
        });

        return new ApiResponse<object>();
    }
}