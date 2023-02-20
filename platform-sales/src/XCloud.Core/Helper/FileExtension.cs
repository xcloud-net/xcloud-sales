using System.IO;

namespace XCloud.Core.Helper;

public static class FileExtension
{
    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="dir"></param>
    public static void CreateIfNotExist(this DirectoryInfo dir)
    {
        if (!dir.Exists)
        {
            dir.Create();
        }
    }
}