namespace UnityEditor
{
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("UnityEditor.AudioImporterLoadType has been deprecated. Use UnityEngine.AudioClipLoadType instead (UnityUpgradable) -> [UnityEngine] UnityEngine.AudioClipLoadType", true)]
    public enum AudioImporterLoadType
    {
        CompressedInMemory = -1,
        DecompressOnLoad = -1,
        [Obsolete("UnityEditor.AudioImporterLoadType.StreamFromDisc has been deprecated. Use UnityEngine.AudioClipLoadType.Streaming instead (UnityUpgradable) -> UnityEngine.AudioClipLoadType.Streaming", true)]
        StreamFromDisc = -1
    }
}

