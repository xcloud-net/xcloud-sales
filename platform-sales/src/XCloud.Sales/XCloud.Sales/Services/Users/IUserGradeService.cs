using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Services.Users;

public interface IUserGradeService : ISalesAppService
{
    Task UpdateUserGradeAsync(UserGrade dto);

    Task<UserGrade> GetGradeByUserIdAsync(int userId);

    Task SetUserGradeAsync(UserGradeMappingDto dto);

    Task<ApiResponse<UserGrade>> AddUserGradeAsync(UserGrade userGrade);

    Task UpdateUserGradeStatusAsync(UpdateUserGradeStatusInput input);

    Task UpdateGradeUserCountAsync(string gradeId);

    Task<UserGrade[]> QueryAllGradesAsync();
}

public class UserGradeService : SalesAppService, IUserGradeService
{
    private readonly ISalesRepository<UserGrade> _userGradeRepository;
        
    public UserGradeService(ISalesRepository<UserGrade> userGradeRepository)
    {
        this._userGradeRepository = userGradeRepository;
    }

    public async Task<UserGrade> GetGradeByUserIdAsync(int userId)
    {
        var db = await this._userGradeRepository.GetDbContextAsync();

        var query = from mapping in db.Set<UserGradeMapping>().AsNoTracking()
            join grade in db.Set<UserGrade>().AsNoTracking()
                on mapping.GradeId equals grade.Id
            select new { mapping, grade };

        query = query.Where(x => x.mapping.UserId == userId);

        var now = this.Clock.Now;
        query = query.Where(x => x.mapping.StartTime == null || x.mapping.StartTime <= now);
        query = query.Where(x => x.mapping.EndTime == null || x.mapping.EndTime >= now);

        query = query.OrderByDescending(x => x.mapping.CreationTime);

        var data = await query.FirstOrDefaultAsync();

        if (data == null)
            return null;

        return data.grade;
    }

    public async Task<UserGrade[]> QueryAllGradesAsync()
    {
        var db = await this._userGradeRepository.GetDbContextAsync();

        var query = db.Set<UserGrade>().AsNoTracking();

        var data = await query.OrderByDescending(x => x.Sort).ToArrayAsync();

        return data;
    }

    public async Task UpdateGradeUserCountAsync(string gradeId)
    {
        var db = await this._userGradeRepository.GetDbContextAsync();

        var userGrade = await db.Set<UserGrade>().FirstOrDefaultAsync(x => x.Id == gradeId);

        if (userGrade == null)
            return;

        var query = from mapping in db.Set<UserGradeMapping>().AsNoTracking()
            join grade in db.Set<UserGrade>().AsNoTracking()
                on mapping.GradeId equals grade.Id
            select new { mapping, grade };
        var now = this.Clock.Now;
        query = query.Where(x => x.mapping.StartTime == null || x.mapping.StartTime <= now);
        query = query.Where(x => x.mapping.EndTime == null || x.mapping.EndTime >= now);

        userGrade.UserCount = await query.Select(x => x.grade.Id).CountAsync();

        await db.TrySaveChangesAsync();
    }

    public async Task<ApiResponse<UserGrade>> AddUserGradeAsync(UserGrade userGrade)
    {
        if (string.IsNullOrWhiteSpace(userGrade.Name))
            return new ApiResponse<UserGrade>().SetError("pls input grade name");

        userGrade.Id = this.GuidGenerator.CreateGuidString();
        await this._userGradeRepository.InsertNowAsync(userGrade);
        return new ApiResponse<UserGrade>(userGrade);
    }

    public async Task UpdateUserGradeAsync(UserGrade dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(UpdateUserGradeAsync));
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentNullException("pls input grade name");

        var grade = await this._userGradeRepository.QueryOneAsync(x => x.Id == dto.Id);
        if (grade == null)
            throw new EntityNotFoundException(nameof(UpdateUserGradeAsync));

        grade.Name = dto.Name;
        grade.Description = dto.Description;

        await this._userGradeRepository.UpdateAsync(grade);
    }

    public async Task UpdateUserGradeStatusAsync(UpdateUserGradeStatusInput input)
    {
        var db = await this._userGradeRepository.GetDbContextAsync();

        var grade = await db.Set<UserGrade>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == input.Id);

        if (grade == null)
            throw new EntityNotFoundException(nameof(UpdateUserGradeStatusAsync));

        if (input.IsDeleted != null)
            grade.IsDeleted = input.IsDeleted.Value;

        await db.TrySaveChangesAsync();
    }

    public async Task SetUserGradeAsync(UserGradeMappingDto dto)
    {
        var db = await this._userGradeRepository.GetDbContextAsync();

        var set = db.Set<UserGradeMapping>();

        var mappings = await set.Where(x => x.UserId == dto.UserId).ToArrayAsync();

        if (mappings.Any())
            set.RemoveRange(mappings);

        var entity = this.ObjectMapper.Map<UserGradeMappingDto, UserGradeMapping>(dto);

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;

        set.Add(entity);

        await db.TrySaveChangesAsync();
    }
}