using System.Threading.Tasks;
using XCloud.Core.Application;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Address;
using XCloud.Platform.Core.Domain.Region;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Common.Application.Service.Address;

public interface IUserAddressService : IXCloudApplicationService
{
    Task<UserAddress> InsertAsync(UserAddress userAddress);

    Task SetAsDefaultAsync(string userId, string addressId);

    Task<UserAddress[]> QueryByUserIdAsync(string userId);

    Task DeleteByIdAsync(string id);

    Task UpdateDescriptionAsync(string userAddressId);

    Task<UserAddress[]> QueryByIdsAsync(string[] ids);

    Task<UserAddress> QueryByIdAsync(string id);
}

public class UserAddressService : PlatformApplicationService, IUserAddressService
{
    private readonly IPlatformRepository<UserAddress> _userAddressRepository;
    public UserAddressService(IPlatformRepository<UserAddress> userAddressRepository)
    {
        this._userAddressRepository = userAddressRepository;
    }

    public async Task UpdateDescriptionAsync(string userAddressId)
    {
        if (string.IsNullOrWhiteSpace(userAddressId))
            throw new ArgumentNullException(nameof(userAddressId));
        
        var db = await this._userAddressRepository.GetDbContextAsync();

        var address = await db.Set<UserAddress>().FirstOrDefaultAsync(x => x.Id == userAddressId);
        if (address == null)
            return;

        var query = from userAddress in db.Set<UserAddress>().AsNoTracking()

            join n in db.Set<SysRegion>().AsNoTracking()
                on userAddress.NationCode equals n.Id into nationGrouping
            from nation in nationGrouping.DefaultIfEmpty()

            join p in db.Set<SysRegion>().AsNoTracking()
                on userAddress.ProvinceCode equals p.Id into provinceGrouping
            from province in provinceGrouping.DefaultIfEmpty()

            join c in db.Set<SysRegion>().AsNoTracking()
                on userAddress.CityCode equals c.Id into cityGrouping
            from city in cityGrouping.DefaultIfEmpty()

            join co in db.Set<SysRegion>().AsNoTracking()
                on userAddress.AreaCode equals co.Id into countryGrouping
            from area in countryGrouping.DefaultIfEmpty()

            select new { userAddress, nation, province, city, area };

        query = query.Where(x => x.userAddress.Id == userAddressId);

        var data = await query.FirstOrDefaultAsync();

        var regions = new[] { data.nation?.Name, data.province?.Name, data.city?.Name, data.area?.Name };
        regions = regions.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        address.AddressDescription = string.Join('-', regions);

        await db.TrySaveChangesAsync();
    }

    public async Task<UserAddress> InsertAsync(UserAddress userAddress)
    {
        if (userAddress == null)
            throw new ArgumentNullException(nameof(userAddress));
        
        if (string.IsNullOrWhiteSpace(userAddress.UserId))
            throw new ArgumentNullException(nameof(userAddress.UserId));

        userAddress.Id = this.GuidGenerator.CreateGuidString();
        userAddress.CreationTime = this.Clock.Now;

        await this._userAddressRepository.InsertNowAsync(userAddress);

        return userAddress;
    }

    public async Task SetAsDefaultAsync(string userId, string addressId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));
        
        if (string.IsNullOrWhiteSpace(addressId))
            throw new ArgumentNullException(nameof(addressId));
        
        var db = await this._userAddressRepository.GetDbContextAsync();
        var set = db.Set<UserAddress>();

        var allAddress = await set.Where(x => x.UserId == userId).ToArrayAsync();

        if (!allAddress.Any())
            return;

        foreach (var m in allAddress)
        {
            var deft = m.Id == addressId;
            if (m.IsDefault != deft)
            {
                m.IsDefault = deft;
            }
        }

        await db.TrySaveChangesAsync();
    }

    public async Task<UserAddress[]> QueryByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));
        
        var allAddress = await this._userAddressRepository.QueryManyAsync(x => x.UserId == userId);
        return allAddress;
    }

    public async Task DeleteByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));

        var address = await this._userAddressRepository.QueryOneAsync(x => x.Id == id);
        if (address == null)
            return;

        address.IsDeleted = true;
        await this._userAddressRepository.UpdateAsync(address);
    }

    public async Task<UserAddress[]> QueryByIdsAsync(string[] ids)
    {
        if (ValidateHelper.IsEmptyCollection(ids))
            return Array.Empty<UserAddress>();

        var allAddress = await this._userAddressRepository.QueryManyAsync(x => ids.Contains(x.Id));
        return allAddress;
    }

    public async Task<UserAddress> QueryByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));

        var address = await this._userAddressRepository.QueryOneAsync(x => x.Id == id);
        return address;
    }
}