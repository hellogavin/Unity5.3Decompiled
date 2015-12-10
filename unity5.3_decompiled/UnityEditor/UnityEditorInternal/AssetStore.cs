namespace UnityEditorInternal
{
    using System;
    using UnityEditor;

    public sealed class AssetStore
    {
        public static void Open(string assetStoreURL)
        {
            if (assetStoreURL != string.Empty)
            {
                AssetStoreWindow.OpenURL(assetStoreURL);
            }
            else
            {
                AssetStoreWindow.Init();
            }
        }
    }
}

