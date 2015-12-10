using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
internal class AssemblyValidationRule : Attribute
{
    private readonly RuntimePlatform _platform;
    public int Priority;

    public AssemblyValidationRule(RuntimePlatform platform)
    {
        this._platform = platform;
        this.Priority = 0;
    }

    public RuntimePlatform Platform
    {
        get
        {
            return this._platform;
        }
    }
}

