using Volo.Abp.Data;
using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Data.DataSeeder;

public class BrandDataSeederContributor : SalesAppService, IDataSeedContributor, ITransientDependency
{
    private readonly IBrandService _brandService;
    private readonly ISalesRepository<Brand> _repository;

    public BrandDataSeederContributor(IBrandService brandService, ISalesRepository<Brand> repository)
    {
        _brandService = brandService;
        _repository = repository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"try-create-preset-brands",
            expiryTime: TimeSpan.FromSeconds(30));

        if (dlock.IsAcquired)
        {
            var db = await this._repository.GetDbContextAsync();
            var query = db.Set<Brand>().IgnoreQueryFilters().AsNoTracking();

            if (await query.AnyAsync())
                return;

            var presetBrands = new[] { "海蓝之谜", "欧莱雅", "莱伯妮" };

            foreach (var m in presetBrands)
            {
                await this._brandService.InsertAsync(new BrandDto()
                {
                    Name = m,
                    IsDeleted = false,
                    Published = true,
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