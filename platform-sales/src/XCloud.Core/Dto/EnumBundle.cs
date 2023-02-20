using System.ComponentModel;

namespace XCloud.Core.Dto;

/// <summary>
/// 是否
/// </summary>
public enum YesOrNoEnum : int
{
    No = 0,
    YES = 1
}

/// <summary>
/// 男女未知
/// </summary>
public enum GenderEnum : int
{
    Male = 1,
    FeMale = 0,
    Unknow = -1
}

/// <summary>
/// 增删改查
/// </summary>
public enum CrudEnum : int
{
    Add = 1 << 0,
    Delete = 1 << 1,
    Update = 1 << 2,
    Query = 1 << 3
}

/// <summary>
/// 平台
/// </summary>
public enum PlatformEnum : int
{
    [Description("未知")]
    Unknow = 0,
    IOS = 1,
    Android = 2,
    H5 = 3,
    Web = 4,
    MiniProgram = 5
}