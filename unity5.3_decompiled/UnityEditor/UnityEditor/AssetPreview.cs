namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class AssetPreview
    {
        private const int kSharedClientID = 0;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ClearTemporaryAssetPreviews();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void DeletePreviewTextureManagerByID(int clientID);
        internal static Texture2D GetAssetPreview(int instanceID)
        {
            return GetAssetPreview(instanceID, 0);
        }

        public static Texture2D GetAssetPreview(Object asset)
        {
            if (asset != null)
            {
                return GetAssetPreview(asset.GetInstanceID());
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Texture2D GetAssetPreview(int instanceID, int clientID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D GetMiniThumbnail(Object obj);
        public static Texture2D GetMiniTypeThumbnail(Type type)
        {
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                return EditorGUIUtility.LoadIcon(type.FullName.Replace('.', '/') + " Icon");
            }
            return INTERNAL_GetMiniTypeThumbnailFromType(type);
        }

        internal static Texture2D GetMiniTypeThumbnail(Object obj)
        {
            return INTERNAL_GetMiniTypeThumbnailFromObject(obj);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Texture2D GetMiniTypeThumbnailFromClassID(int classID);
        internal static bool HasAnyNewPreviewTexturesAvailable()
        {
            return HasAnyNewPreviewTexturesAvailable(0);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasAnyNewPreviewTexturesAvailable(int clientID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Texture2D INTERNAL_GetMiniTypeThumbnailFromObject(Object monoObj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Texture2D INTERNAL_GetMiniTypeThumbnailFromType(Type type);
        public static bool IsLoadingAssetPreview(int instanceID)
        {
            return IsLoadingAssetPreview(instanceID, 0);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsLoadingAssetPreview(int instanceID, int clientID);
        public static bool IsLoadingAssetPreviews()
        {
            return IsLoadingAssetPreviews(0);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsLoadingAssetPreviews(int clientID);
        public static void SetPreviewTextureCacheSize(int size)
        {
            SetPreviewTextureCacheSize(size, 0);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetPreviewTextureCacheSize(int size, int clientID);
    }
}

