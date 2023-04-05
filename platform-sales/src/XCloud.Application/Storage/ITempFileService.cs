using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.DependencyInjection;
using XCloud.Application.Service;
using XCloud.Core.Helper;
using XCloud.Core.IdGenerator;

namespace XCloud.Application.Storage;

public interface ITempFileService : IXCloudApplicationService
{
    Task<ITempFile> CreateTempFileAsync(Stream s);
}

[ExposeServices(typeof(ITempFileService))]
public class TempFileService : XCloudApplicationService, ITempFileService, IScopedDependency
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public TempFileService(IWebHostEnvironment webHostEnvironment)
    {
        this._webHostEnvironment = webHostEnvironment;
    }

    private string EnsureTempDir()
    {
        var tempDir = Path.Combine(this._webHostEnvironment.ContentRootPath, $"temp");
        new DirectoryInfo(tempDir).CreateIfNotExist();

        //yyyy/MM/dd hh:mm:ss
        var time = this.Clock.Now.ToString("yyyyMMddhhmm");
        tempDir = Path.Combine(tempDir, $"temp-{time}");
        new DirectoryInfo(tempDir).CreateIfNotExist();

        return tempDir;
    }

    public async Task<ITempFile> CreateTempFileAsync(Stream s)
    {
        var guid = this.GuidGenerator.CreateGuidString();

        guid = $"{guid}.tmp";

        guid = Path.Combine(this.EnsureTempDir(), guid);

        File.Exists(guid).Should().BeFalse("temp file is already exist,pls try again");

        using var f = new FileStream(guid, FileMode.OpenOrCreate);
        await s.CopyToAsync(f);

        return new LocalDiskTempFile(guid);
    }
}