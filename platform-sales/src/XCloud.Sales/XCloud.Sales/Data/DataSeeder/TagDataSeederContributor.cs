using Volo.Abp.Data;
using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Data.DataSeeder;

public class TagDataSeederContributor : SalesAppService, IDataSeedContributor, ITransientDependency
{
    private readonly ITagService _tagService;
    private readonly ISalesRepository<Tag> _repository;

    public TagDataSeederContributor(ITagService tagService, ISalesRepository<Tag> repository)
    {
        _tagService = tagService;
        _repository = repository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"try-create-preset-tags",
            expiryTime: TimeSpan.FromSeconds(30));

        if (dlock.IsAcquired)
        {
            var db = await this._repository.GetDbContextAsync();
            var query = db.Set<Tag>().IgnoreQueryFilters().AsNoTracking();

            if (await query.AnyAsync())
                return;

            var presetTags = new[] { "活动", "推荐", "新品" };

            foreach (var m in presetTags)
            {
                await this._tagService.InsertAsync(new TagDto()
                {
                    Name = m,
                    IsDeleted = false,
                });
            }

            await Task.CompletedTask;
        }
        else
        {
            this.Logger.LogWarning("failed to get distributed lock");
        }
    }
}