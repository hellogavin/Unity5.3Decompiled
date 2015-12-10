namespace UnityEngine
{
    using System;

    [Flags]
    public enum HideFlags
    {
        DontSave = 0x34,
        DontSaveInBuild = 0x10,
        DontSaveInEditor = 4,
        DontUnloadUnusedAsset = 0x20,
        HideAndDontSave = 0x3d,
        HideInHierarchy = 1,
        HideInInspector = 2,
        None = 0,
        NotEditable = 8
    }
}

