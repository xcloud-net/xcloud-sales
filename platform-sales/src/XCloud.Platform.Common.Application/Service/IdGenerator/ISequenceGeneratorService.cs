using System.Data;
using System.Threading.Tasks;
using Polly;
using Volo.Abp.Data;
using Volo.Abp.Uow;
using XCloud.Application.Service;
using XCloud.Core.Application;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.IdGenerator;
using XCloud.Platform.Data.Database;
using XCloud.Platform.Shared.Dto;
using XCloud.Redis;

namespace XCloud.Platform.Common.Application.Service.IdGenerator;

/// <summary>
/// 使用分布式锁只能锁住当前代码块，不能防止其他方式访问并修改数据库。
/// 所以这里配合数据库乐观锁来使用。
/// </summary>
public interface ISequenceGeneratorService : IXCloudApplicationService
{
    Task<int> GenerateNoWithOptimisticLockAndRetryAsync(CreateNoByCategoryDto dto);
    
    Task<int> GenerateNoWithDistributedLockAsync(CreateNoByCategoryDto dto);
}

public class SequenceGeneratorService : PlatformApplicationService, ISequenceGeneratorService
{
    private readonly IPlatformRepository<SysSequence> _sequenceEntities;
    public SequenceGeneratorService(IPlatformRepository<SysSequence> sequenceEntities)
    {
        this._sequenceEntities = sequenceEntities;
    }

    private void CheckCreateNoByCategoryDto(CreateNoByCategoryDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Category))
            throw new ArgumentNullException(nameof(dto.Category));
    }

    private void TrySetMinId(SysSequence entity, CreateNoByCategoryDto dto)
    {
        if (dto.MinId != null)
        {
            if (entity.NextId < dto.MinId.Value)
            {
                entity.NextId = dto.MinId.Value;
            }
        }
    }

    private async Task<int> GenerateNoWithOptimisticLockAsync(CreateNoByCategoryDto dto)
    {
        using var uow = this.UnitOfWorkManager.Begin(requiresNew: true);
        try
        {
            var db = await this._sequenceEntities.GetDbContextAsync();
            var set = db.Set<SysSequence>();

            var entity = await set.AsTracking().FirstOrDefaultAsync(x => x.Category == dto.Category);
            if (entity == null)
            {
                //create new record
                entity = new SysSequence();
                entity.Id = this.GuidGenerator.CreateGuidString();
                entity.CreationTime = this.Clock.Now;
                entity.Category = dto.Category;
                entity.Description = dto.Description;
                entity.NextId = 1;

                this.TrySetMinId(entity, dto);

                set.Add(entity);
            }
            else
            {
                //modify the record
                entity.NextId += 1;
                entity.LastModificationTime = this.Clock.Now;

                this.TrySetMinId(entity, dto);
            }

            await db.SaveChangesAsync();

            await uow.CompleteAsync();

            return entity.NextId;
        }
        catch (Exception e) when (this.IsDbConcurrencyException(e))
        {
            await uow.RollbackAsync();
            throw;
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }

    private bool IsDbConcurrencyException(Exception e) =>
        e is DBConcurrencyException ||
        e is DbUpdateConcurrencyException ||
        e is AbpDbConcurrencyException;

    public async Task<int> GenerateNoWithOptimisticLockAndRetryAsync(CreateNoByCategoryDto dto)
    {
        var retryPolicy = Policy.Handle<Exception>(exceptionPredicate: this.IsDbConcurrencyException)
            .WaitAndRetryAsync(retryCount: 3, i => TimeSpan.FromMilliseconds(100 * i));

        var nextId = await retryPolicy.ExecuteAsync(() => this.GenerateNoWithOptimisticLockAsync(dto));

        return nextId;
    }

    public async Task<int> GenerateNoWithDistributedLockAsync(CreateNoByCategoryDto dto)
    {
        this.CheckCreateNoByCategoryDto(dto);

        var resourceKey = $"{nameof(SequenceGeneratorService)}.{nameof(GenerateNoWithDistributedLockAsync)}.category={dto.Category}";

        using var distributedLock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: resourceKey,
            expiryTime: TimeSpan.FromSeconds(30),
            waitTime: TimeSpan.FromSeconds(5),
            retryTime: TimeSpan.FromSeconds(1));

        if (distributedLock.IsAcquired)
        {
            return await this.GenerateNoWithOptimisticLockAndRetryAsync(dto);
        }
        else
        {
            throw new FailToGetRedLockException("无法生成id，未能获取分布式锁");
        }
    }
}