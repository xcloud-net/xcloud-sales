using System.Threading.Tasks;

namespace XCloud.Platform.Common.Application.Service.IdGenerator;

public class InEfficientDatabaseGlobalIdGenerator : IGlobalIdGenerator
{
    private readonly ISequenceGeneratorService _sequenceGeneratorService;
    public InEfficientDatabaseGlobalIdGenerator(ISequenceGeneratorService sequenceGeneratorService)
    {
        this._sequenceGeneratorService = sequenceGeneratorService;
    }

    public async Task<string> NewIdAsync(string category = null)
    {
        var nextId = await this._sequenceGeneratorService.GenerateNoWithOptimisticLockAndRetryAsync(category);

        return nextId.ToString();
    }
}