using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace XCloud.Core.Helper;

public class INIFile
{
    private readonly string path;

    public INIFile(string INIPath)
    {
        path = INIPath;
        if (!File.Exists(path ?? throw new Exception("????????????????")))
        {
            throw new Exception($"????{path}??????");
        }
    }

    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section, string key, string defVal, byte[] retVal, int size, string filePath);


    /// <summary>
    /// ??INI????
    /// </summary>
    public void IniWriteValue(string Section, string Key, string Value)
    {
        WritePrivateProfileString(Section, Key, Value, path);
    }

    /// <summary>
    /// ????INI????
    /// </summary>
    public string IniReadValue(string Section, string Key)
    {
        var temp = new StringBuilder(255);
        int i = GetPrivateProfileString(Section, Key, "", temp, 255, path);
        return temp.ToString();
    }

    public byte[] IniReadValues(string section, string key)
    {
        var temp = new byte[255];
        int i = GetPrivateProfileString(section, key, "", temp, 255, path);
        return temp;

    }


    /// <summary>
    /// ????ini??????????????
    /// </summary>
    public void ClearAllSection()
    {
        IniWriteValue(null, null, null);
    }

    /// <summary>
    /// ????ini??????personal??????????????
    /// </summary>
    public void ClearSection(string Section)
    {
        IniWriteValue(Section, null, null);
    }

}