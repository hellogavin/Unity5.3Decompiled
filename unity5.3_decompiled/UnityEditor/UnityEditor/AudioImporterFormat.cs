namespace UnityEditor
{
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("UnityEditor.AudioImporterFormat has been deprecated. Use UnityEngine.AudioCompressionFormat instead.")]
    public enum AudioImporterFormat
    {
        Compressed = 0,
        Native = -1
    }
}

