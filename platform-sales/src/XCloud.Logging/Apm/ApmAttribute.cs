﻿using System;

namespace XCloud.Logging.Apm;

/// <summary>
/// 标记开启apm
/// </summary>
public class ApmAttribute : Attribute
{
    public bool Disabled { get; set; } = false;
}