using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.AsyncJob;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Application.Common.Service.AsyncJob;

/// <summary>
/// 一个job应该对应一个或者多个记录
/// </summary>
public interface IJobRecordService : IPlatformPagingCrudService<JobRecord, JobRecordDto, QueryJobPagingInput>
{
    //
}

public class JobRecordService : PlatformPagingCrudService<JobRecord, JobRecordDto, QueryJobPagingInput>,
    IJobRecordService
{
    private readonly IPlatformRepository<JobRecord> _repository;

    public JobRecordService(IPlatformRepository<JobRecord> repo) : base(repo)
    {
        this._repository = repo;
    }
}