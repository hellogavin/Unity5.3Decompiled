namespace UnityEditorInternal
{
    using System;

    public enum MemoryInfoGCReason
    {
        AssetMarkedDirtyInEditor = 3,
        AssetReferenced = 9,
        AssetReferencedByNativeCodeOnly = 8,
        BuiltinResource = 1,
        MarkedDontSave = 2,
        NotApplicable = 10,
        SceneAssetReferenced = 6,
        SceneAssetReferencedByNativeCodeOnly = 5,
        SceneObject = 0
    }
}

