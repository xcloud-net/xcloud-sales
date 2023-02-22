using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controller.User;

[Route("api/mall/prepaid-card")]
public class PrepaidCardController : ShopBaseController
{
    private readonly IPrepaidCardService prepaidCardService;

    public PrepaidCardController(IPrepaidCardService prepaidCardService)
    {
        this.prepaidCardService = prepaidCardService;
    }

    [HttpPost("by-id")]
    public async Task<ApiResponse<PrepaidCardDto>> QueryByIdAsync([FromBody] IdDto dto)
    {
        var storeUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var cardDto = await this.prepaidCardService.QueryByIdAsync(dto.Id);

        if (cardDto == null)
            throw new EntityNotFoundException(nameof(QueryByIdAsync));

        if (cardDto.EndTime != null && cardDto.EndTime.Value < this.Clock.Now)
            cardDto.Expired = true;

        return new ApiResponse<PrepaidCardDto>(cardDto);
    }

    [HttpPost("use-card")]
    public async Task<ApiResponse<object>> UseCardAsync([FromBody] UsePrepaidCardInput dto)
    {
        var storeUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        dto.UserId = storeUser.Id;

        await this.prepaidCardService.UsePrepaidCardAsync(dto);

        return new ApiResponse<object>();
    }
}