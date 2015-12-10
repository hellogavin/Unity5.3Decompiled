namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class BuildPipeline
    {
        [Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundle(Object mainAsset, Object[] assets, string pathName)
        {
            BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies;
            return BuildAssetBundle(mainAsset, assets, pathName, assetBundleOptions);
        }

        [Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundle(Object mainAsset, Object[] assets, string pathName, BuildAssetBundleOptions assetBundleOptions)
        {
            BuildTarget webPlayer = BuildTarget.WebPlayer;
            return BuildAssetBundle(mainAsset, assets, pathName, assetBundleOptions, webPlayer);
        }

        [Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundle(Object mainAsset, Object[] assets, string pathName, out uint crc)
        {
            BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies;
            return BuildAssetBundle(mainAsset, assets, pathName, out crc, assetBundleOptions);
        }

        [Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundle(Object mainAsset, Object[] assets, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions)
        {
            BuildTarget webPlayer = BuildTarget.WebPlayer;
            return BuildAssetBundle(mainAsset, assets, pathName, out crc, assetBundleOptions, webPlayer);
        }

        [Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundle(Object mainAsset, Object[] assets, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            uint num;
            return BuildAssetBundle(mainAsset, assets, pathName, out num, assetBundleOptions, targetPlatform);
        }

        [Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundle(Object mainAsset, Object[] assets, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            crc = 0;
            try
            {
                return BuildAssetBundleInternal(mainAsset, assets, null, pathName, assetBundleOptions, targetPlatform, out crc);
            }
            catch (Exception exception)
            {
                LogBuildExceptionAndExit("BuildPipeline.BuildAssetBundle", exception);
                return false;
            }
        }

        [Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundleExplicitAssetNames(Object[] assets, string[] assetNames, string pathName)
        {
            BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies;
            return BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, assetBundleOptions);
        }

        [Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundleExplicitAssetNames(Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions)
        {
            BuildTarget webPlayer = BuildTarget.WebPlayer;
            return BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, assetBundleOptions, webPlayer);
        }

        [Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundleExplicitAssetNames(Object[] assets, string[] assetNames, string pathName, out uint crc)
        {
            BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies;
            return BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out crc, assetBundleOptions);
        }

        [Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundleExplicitAssetNames(Object[] assets, string[] assetNames, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions)
        {
            BuildTarget webPlayer = BuildTarget.WebPlayer;
            return BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out crc, assetBundleOptions, webPlayer);
        }

        [Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundleExplicitAssetNames(Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            uint num;
            return BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out num, assetBundleOptions, targetPlatform);
        }

        [Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static bool BuildAssetBundleExplicitAssetNames(Object[] assets, string[] assetNames, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            crc = 0;
            try
            {
                return BuildAssetBundleInternal(null, assets, assetNames, pathName, assetBundleOptions, targetPlatform, out crc);
            }
            catch (Exception exception)
            {
                LogBuildExceptionAndExit("BuildPipeline.BuildAssetBundleExplicitAssetNames", exception);
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool BuildAssetBundleInternal(Object mainAsset, Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform, out uint crc);
        [ExcludeFromDocs]
        public static AssetBundleManifest BuildAssetBundles(string outputPath)
        {
            BuildTarget webPlayer = BuildTarget.WebPlayer;
            BuildAssetBundleOptions none = BuildAssetBundleOptions.None;
            return BuildAssetBundles(outputPath, none, webPlayer);
        }

        [ExcludeFromDocs]
        public static AssetBundleManifest BuildAssetBundles(string outputPath, BuildAssetBundleOptions assetBundleOptions)
        {
            BuildTarget webPlayer = BuildTarget.WebPlayer;
            return BuildAssetBundles(outputPath, assetBundleOptions, webPlayer);
        }

        [ExcludeFromDocs]
        public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds)
        {
            BuildTarget webPlayer = BuildTarget.WebPlayer;
            BuildAssetBundleOptions none = BuildAssetBundleOptions.None;
            return BuildAssetBundles(outputPath, builds, none, webPlayer);
        }

        [ExcludeFromDocs]
        public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions)
        {
            BuildTarget webPlayer = BuildTarget.WebPlayer;
            return BuildAssetBundles(outputPath, builds, assetBundleOptions, webPlayer);
        }

        public static AssetBundleManifest BuildAssetBundles(string outputPath, [DefaultValue("BuildAssetBundleOptions.None")] BuildAssetBundleOptions assetBundleOptions, [DefaultValue("BuildTarget.WebPlayer")] BuildTarget targetPlatform)
        {
            if (!Directory.Exists(outputPath))
            {
                Debug.LogError("The output path \"" + outputPath + "\" doesn't exist");
                return null;
            }
            try
            {
                return BuildAssetBundlesInternal(outputPath, assetBundleOptions, targetPlatform);
            }
            catch (Exception exception)
            {
                LogBuildExceptionAndExit("BuildPipeline.BuildAssetBundles", exception);
                return null;
            }
        }

        public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, [DefaultValue("BuildAssetBundleOptions.None")] BuildAssetBundleOptions assetBundleOptions, [DefaultValue("BuildTarget.WebPlayer")] BuildTarget targetPlatform)
        {
            if (!Directory.Exists(outputPath))
            {
                Debug.LogError("The output path \"" + outputPath + "\" doesn't exist");
                return null;
            }
            if (builds == null)
            {
                Debug.LogError("AssetBundleBuild cannot be null.");
                return null;
            }
            try
            {
                return BuildAssetBundlesWithInfoInternal(outputPath, builds, assetBundleOptions, targetPlatform);
            }
            catch (Exception exception)
            {
                LogBuildExceptionAndExit("BuildPipeline.BuildAssetBundles", exception);
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AssetBundleManifest BuildAssetBundlesInternal(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AssetBundleManifest BuildAssetBundlesWithInfoInternal(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform);
        public static string BuildPlayer(string[] levels, string locationPathName, BuildTarget target, BuildOptions options)
        {
            try
            {
                uint num;
                return BuildPlayerInternal(levels, locationPathName, target, options, out num);
            }
            catch (Exception exception)
            {
                LogBuildExceptionAndExit("BuildPipeline.BuildPlayer", exception);
                return string.Empty;
            }
        }

        private static string BuildPlayerInternal(string[] levels, string locationPathName, BuildTarget target, BuildOptions options, out uint crc)
        {
            crc = 0;
            if (((BuildOptions.EnableHeadlessMode & options) != BuildOptions.CompressTextures) && ((BuildOptions.Development & options) != BuildOptions.CompressTextures))
            {
                return "Unsupported build setting: cannot build headless development player";
            }
            if (target == BuildTarget.WP8Player)
            {
                return "Windows Phone 8.0 is no longer supported, please switch to Windows Phone 8.1";
            }
            if ((target == BuildTarget.WSAPlayer) && (EditorUserBuildSettings.wsaSDK == WSASDK.SDK80))
            {
                return "Windows SDK 8.0 is no longer supported, please switch to Windows SDK 8.1";
            }
            return BuildPlayerInternalNoCheck(levels, locationPathName, target, options, false, out crc);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string BuildPlayerInternalNoCheck(string[] levels, string locationPathName, BuildTarget target, BuildOptions options, bool delayToAfterScriptReload, out uint crc);
        [Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target)
        {
            return BuildPlayer(levels, locationPath, target, BuildOptions.BuildAdditionalStreamedScenes);
        }

        [Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, BuildOptions options)
        {
            return BuildPlayer(levels, locationPath, target, options | BuildOptions.BuildAdditionalStreamedScenes);
        }

        [Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, out uint crc)
        {
            return BuildStreamedSceneAssetBundle(levels, locationPath, target, out crc, BuildOptions.CompressTextures);
        }

        [Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
        public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, out uint crc, BuildOptions options)
        {
            crc = 0;
            try
            {
                return BuildPlayerInternal(levels, locationPath, target, options | BuildOptions.BuildAdditionalStreamedScenes, out crc);
            }
            catch (Exception exception)
            {
                LogBuildExceptionAndExit("BuildPipeline.BuildStreamedSceneAssetBundle", exception);
                return string.Empty;
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetBuildTargetAdvancedLicenseName(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern BuildTarget GetBuildTargetByName(string platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern BuildTargetGroup GetBuildTargetGroup(BuildTarget platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern BuildTargetGroup GetBuildTargetGroupByName(string platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetBuildTargetGroupDisplayName(BuildTargetGroup targetPlatformGroup);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetBuildTargetGroupName(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetBuildTargetName(BuildTarget targetPlatform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetBuildToolsDirectory(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetCRCForAssetBundle(string targetPath, out uint crc);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetEditorTargetName();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetHashForAssetBundle(string targetPath, out Hash128 hash);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetMonoBinDirectory(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetMonoLibDirectory(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetMonoProfileLibDirectory(BuildTarget target, string profile);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetPlaybackEngineDirectory(BuildTarget target, BuildOptions options);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsBuildTargetSupported(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsUnityScriptEvalSupported(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool LicenseCheck(BuildTarget target);
        private static void LogBuildExceptionAndExit(string buildFunctionName, Exception exception)
        {
            object[] args = new object[] { buildFunctionName };
            Debug.LogErrorFormat("Internal Error in {0}:", args);
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("PopAssetDependencies has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details."), WrapperlessIcall]
        public static extern void PopAssetDependencies();
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("PushAssetDependencies has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details."), WrapperlessIcall]
        public static extern void PushAssetDependencies();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetPlaybackEngineDirectory(BuildTarget target, BuildOptions options, string playbackEngineDirectory);

        public static bool isBuildingPlayer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

