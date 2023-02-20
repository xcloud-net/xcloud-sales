using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace XCloud.Application.Compress;

/// <summary>压缩解压缩</summary>
public class ZipHelper
{
    private void ZipDirectory(ZipOutputStream outputStream, string baseDirectory, string directoryPath, IEnumerable<string> ignoreFilesOrDirectories)
    {
        foreach (var file in Directory.GetFiles(directoryPath ?? baseDirectory))
        {
            ZipFile(outputStream, baseDirectory, file, ignoreFilesOrDirectories);
        }

        foreach (var dir in Directory.GetDirectories(directoryPath ?? baseDirectory))
        {
            if (ignoreFilesOrDirectories == null || !ignoreFilesOrDirectories.Any(fd => string.Compare(fd, dir, true) == 0))
                ZipDirectory(outputStream, baseDirectory, dir, ignoreFilesOrDirectories);
        }
    }

    private void ZipFile(ZipOutputStream outputStream, string baseDirectory, string fileFullPath, IEnumerable<string> ignoreFilesOrDirectories)
    {
        if (ignoreFilesOrDirectories == null || !ignoreFilesOrDirectories.Any(fd => string.Compare(fd, fileFullPath, true) == 0))
            using (var input = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read))
            {
                var entry = new ZipEntry(fileFullPath.Substring(baseDirectory.Length).TrimStart('\\'));

                var fileInfo = new FileInfo(fileFullPath);
                entry.DateTime = fileInfo.LastWriteTime;

                outputStream.PutNextEntry(entry);

                int readLength;
                byte[] buffer = new byte[1024 * 1024];
                while ((readLength = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, readLength);

                    outputStream.Flush();
                }
            }
    }

    /// <summary>解压缩到档案</summary>
    /// <param name="inputStream">输入流</param>
    /// <param name="outputDirectory">输出目录</param>
    public IList<string> Dezip(Stream inputStream, string outputDirectory)
    {
        ZipEntry entry;
        using (var zipStream = new ZipInputStream(inputStream))
        {
            var files = new List<string>();
            while ((entry = zipStream.GetNextEntry()) != null)
            {
                var path = Path.Combine(outputDirectory, entry.Name.Replace('/', '\\'));

                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (entry.IsFile)
                {
                    files.Add(entry.Name.Replace('/', '\\'));

                    using (var output = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        var readLength = 0;
                        byte[] data = new byte[1024 * 1024];
                        while ((readLength = zipStream.Read(data, 0, data.Length)) > 0)
                        {
                            output.Write(data, 0, readLength);

                            output.Flush();
                        }
                    }
                }
            }

            return files;
        }
    }

    /// <summary>获得文档中的所有文件名称</summary>
    /// <param name="inputFile">输入流</param>
    public IList<string> GetFileList(Stream input)
    {
        ZipEntry entry;
        using (var inputStream = new ZipInputStream(input))
        {
            var files = new List<string>();
            while ((entry = inputStream.GetNextEntry()) != null)
            {
                if (entry.IsFile)
                    files.Add(entry.Name.Replace('/', '\\'));
            }
            return files;
        }
    }
}