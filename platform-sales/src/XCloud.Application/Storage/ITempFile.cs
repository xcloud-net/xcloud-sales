using System;
using System.IO;
using System.Threading.Tasks;

namespace XCloud.Application.Storage;

public interface ITempFile : IAsyncDisposable
{
    Task<int> GetTempFileSize();

    Task<Stream> OpenTempFileStreamAsync();

    Task DeleteTempFileAsync();
}

public class LocalDiskTempFile : ITempFile
{
    public LocalDiskTempFile()
    {
        //
    }

    public LocalDiskTempFile(string guid) : this()
    {
        LocalPath = guid;
    }

    public string LocalPath { get; set; }

    public async Task<int> GetTempFileSize()
    {
        await Task.CompletedTask;

        var f = new FileInfo(this.LocalPath);
        if (!f.Exists)
            throw new FileNotFoundException(f.FullName);

        return (int)f.Length;
    }

    public async Task<Stream> OpenTempFileStreamAsync()
    {
        var f = new FileInfo(this.LocalPath);
        if (!f.Exists)
            throw new FileNotFoundException(f.FullName);

        await Task.CompletedTask;

        var s = f.OpenRead();
        return s;
    }

    public async Task DeleteTempFileAsync()
    {
        await Task.CompletedTask;

        if (File.Exists(this.LocalPath))
            File.Delete(this.LocalPath);
    }

    public async ValueTask DisposeAsync()
    {
        await this.DeleteTempFileAsync();
    }
}