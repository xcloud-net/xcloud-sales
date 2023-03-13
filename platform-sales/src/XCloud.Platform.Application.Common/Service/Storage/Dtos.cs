using System.IO;
using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Domain.Storage;

namespace XCloud.Platform.Application.Common.Service.Storage;

public class StorageResourceMetaDto : StorageResourceMeta
{
    //
}

public class QueryStoragePagingInput : PagedRequest,IEntityDto
{
    public string ContentType { get; set; }
    public string FileExtension { get; set; }
    public string StorageProvider { get; set; }
    public double? MinSize { get; set; }
    public double? MaxSize { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class FileUploadArgs : IEntityDto
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
}

public class FileUploadBytesArgs : FileUploadArgs
{
    public FileUploadBytesArgs()
    {
        //
    }

    public byte[] Bytes { get; set; }
}

public class FileUploadStreamArgs : FileUploadArgs
{
    public FileUploadStreamArgs(Stream stream)
    {
        this.Stream = stream;
    }
    public Stream Stream { get; set; }
}

public class FileUploadFromUrl : IEntityDto
{
    public FileUploadFromUrl() { }
    public FileUploadFromUrl(string url, string fileName)
    {
        this.Url = url;
        this.FileName = fileName;
    }

    public string Url { get; set; }
    public string FileName { get; set; }
}