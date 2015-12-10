namespace UnityEditor
{
    using System;

    public enum ModelImporterTangentSpaceMode
    {
        [Obsolete("Use ModelImporterNormals.Calculate instead")]
        Calculate = 1,
        [Obsolete("Use ModelImporterNormals.Import instead")]
        Import = 0,
        [Obsolete("Use ModelImporterNormals.None instead")]
        None = 2
    }
}

