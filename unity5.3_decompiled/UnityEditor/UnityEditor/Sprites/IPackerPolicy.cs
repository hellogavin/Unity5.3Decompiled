namespace UnityEditor.Sprites
{
    using System;
    using UnityEditor;

    public interface IPackerPolicy
    {
        int GetVersion();
        void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs);
    }
}

