namespace UnityEditor
{
    using System;

    public enum SelectionMode
    {
        Assets = 0x10,
        Deep = 2,
        DeepAssets = 0x20,
        Editable = 8,
        ExcludePrefab = 4,
        OnlyUserModifiable = 8,
        TopLevel = 1,
        Unfiltered = 0
    }
}

