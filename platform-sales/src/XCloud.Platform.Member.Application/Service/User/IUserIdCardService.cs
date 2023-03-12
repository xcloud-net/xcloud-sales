using System.Threading.Tasks;
using Masuit.Tools;
using Microsoft.EntityFrameworkCore;
using XCloud.Application.Service;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Member.Application.Service.User;

/// <summary>
/// 用户身份证
/// </summary>
public interface IUserIdCardService : IXCloudApplicationService
{
    Task<ApiResponse<object>> SetUserIdCardAsync(SetIdCardInput dto);

    Task<bool> IsUserIdCardExistAsync(string userId);

    Task<SysUserRealName> QueryUserIdCardAsync(string userId);
}

public class UserIdCardService : PlatformApplicationService, IUserIdCardService
{
    private readonly IMemberRepository<SysUserRealName> _userIdCardRepository;

    public UserIdCardService(IMemberRepository<SysUserRealName> userIdCardRepository)
    {
        _userIdCardRepository = userIdCardRepository;
    }

    private void CheckIdCard(string idCard)
    {
        if (string.IsNullOrWhiteSpace(idCard))
            throw new ArgumentNullException(nameof(idCard));
        if (!idCard.MatchIdentifyCard())
        {
            throw new UserFriendlyException("身份证格式错误");
        }
    }

    public async Task<SysUserRealName> QueryUserIdCardAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var data = await _userIdCardRepository.QueryManyAsync(x => x.UserId == userId, 500);

        var res = data.FirstOrDefault();

        return res;
    }

    public async Task<bool> IsUserIdCardExistAsync(string userId)
    {
        var idCard = await this.QueryUserIdCardAsync(userId);
        return idCard != null;
    }

    public async Task<ApiResponse<object>> SetUserIdCardAsync(SetIdCardInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        CheckIdCard(dto.IdCard);

        var db = await _userIdCardRepository.GetDbContextAsync();
        var set = db.Set<SysUserRealName>();

        var entity = await set.OrderByDescending(x => x.CreationTime).FirstOrDefaultAsync(x => x.UserId == dto.UserId);

        if (entity == null)
        {
            entity = new SysUserRealName
            {
                UserId = dto.UserId,
                IdCard = dto.IdCard,
                IdCardRealName = dto.RealName,
                IdCardConfirmed = false,
                Id = this.GuidGenerator.CreateGuidString(),
                CreationTime = this.Clock.Now
            };

            await db.Set<SysUserRealName>().AddAsync(entity);

            await db.SaveChangesAsync();
        }
        else
        {
            entity.IdCard = dto.IdCard;
            entity.IdCardRealName = dto.RealName;

            await db.SaveChangesAsync();
        }

        return new ApiResponse<object>();
    }
}