using XCloud.Core.Attributes;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Shared.Dto;
using XCloud.Platform.Shared.Storage;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Media;

namespace XCloud.Sales.Services.Media;

public interface IPictureService : ISalesAppService
{
    Task<PictureDto[]> SavePictureAsync(PictureDto[] picture);

    Task<MallStorageMetaDto> DeserializePictureMetaAsync(Picture picture);
}

public class PictureService : SalesAppService, IPictureService
{
    private readonly ISalesRepository<Picture> _pictureRepository;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public PictureService(ISalesRepository<Picture> pictureRepository,
        IStorageUrlResolver storageUrlResolver)
    {
        this._pictureRepository = pictureRepository;
        this._storageUrlResolver = storageUrlResolver;
    }

    private async Task<Picture[]> PreHandlePictureAsync(PictureDto[] data)
    {
        foreach (var picture in data)
        {
            if (picture.StorageMeta == null)
                throw new UserFriendlyException(nameof(picture.StorageMeta));

            picture.ResourceId = picture.StorageMeta.Id;
            picture.MimeType = picture.StorageMeta.ContentType;
            picture.ResourceData = this.JsonDataSerializer.SerializeToString(picture.StorageMeta);
        }

        var dataEntity = data.Select(x => this.ObjectMapper.Map<PictureDto, Picture>(x)).ToArray();

        await Task.CompletedTask;
        return dataEntity;
    }

    public async Task<PictureDto[]> SavePictureAsync(PictureDto[] data)
    {
        if (!data.Any())
            return data;

        var db = await this._pictureRepository.GetDbContextAsync();
        var set = db.Set<Picture>();
        
        var dataEntity = await this.PreHandlePictureAsync(data);

        var resIds = dataEntity.Select(x => x.ResourceId).Distinct().ToArray();

        var previousSaved = await set.Where(x => resIds.Contains(x.ResourceId)).ToArrayAsync();

        var toAdd = dataEntity.NotInBy(previousSaved, x => x.ResourceId).ToArray();
        var toModified = previousSaved.InBy(dataEntity, x => x.ResourceId).ToArray();

        if (toAdd.Any())
        {
            set.AddRange(toAdd);
        }

        foreach (var m in toModified)
        {
            var intersect = dataEntity.FirstOrDefault(x => x.ResourceId == m.ResourceId);
            if (intersect == null)
                continue;

            m.MimeType = intersect.MimeType;
            m.SeoFilename = intersect.SeoFilename;
            m.ResourceData = intersect.ResourceData;
        }

        await db.TrySaveChangesAsync();

        var pictureList = toAdd.AppendManyItems(previousSaved)
            .Select(x => this.ObjectMapper.Map<Picture, PictureDto>(x))
            .ToArray();

        foreach (var m in pictureList)
        {
            m.StorageMeta = await this.DeserializePictureMetaAsync(m);
        }

        return pictureList;
    }

    [Apm(Disabled = true)]
    public virtual async Task<MallStorageMetaDto> DeserializePictureMetaAsync(Picture picture)
    {
        await Task.CompletedTask;

        var meta = this._storageUrlResolver.DeserializeStorageDto(picture.ResourceData);

        var dto = this.ObjectMapper.Map<StorageMetaDto, MallStorageMetaDto>(meta);

        return dto;
    }
}