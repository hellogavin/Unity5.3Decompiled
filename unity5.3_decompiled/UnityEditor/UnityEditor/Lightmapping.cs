namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public sealed class Lightmapping
    {
        public static OnCompletedFunction completed;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Bake();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool BakeAllReflectionProbesSnapshots();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeAsync();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeLightProbesOnly();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeLightProbesOnlyAsync();
        public static void BakeMultipleScenes(string[] paths)
        {
            if (paths.Length != 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    for (int j = i + 1; j < paths.Length; j++)
                    {
                        if (paths[i] == paths[j])
                        {
                            throw new Exception("no duplication of scenes is allowed");
                        }
                    }
                }
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    SceneSetup[] sceneManagerSetup = EditorSceneManager.GetSceneManagerSetup();
                    EditorSceneManager.OpenScene(paths[0]);
                    for (int k = 1; k < paths.Length; k++)
                    {
                        EditorSceneManager.OpenScene(paths[0], OpenSceneMode.Additive);
                    }
                    Bake();
                    EditorSceneManager.SaveOpenScenes();
                    EditorSceneManager.RestoreSceneManagerSetup(sceneManagerSetup);
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeReflectionProbe(ReflectionProbe probe, string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool BakeReflectionProbeSnapshot(ReflectionProbe probe);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeSelected();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeSelectedAsync();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Cancel();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearDiskCache();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearLightingDataAsset();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void GetTerrainGIChunks(Terrain terrain, ref int numChunksX, ref int numChunksY);
        private static void Internal_CallCompletedFunctions()
        {
            if (completed != null)
            {
                completed();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void PrintStateToConsole();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Tetrahedralize(Vector3[] positions, out int[] outIndices, out Vector3[] outPositions);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void UpdateCachePath();

        internal static bool bakedLightmapsEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float bounceBoost { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float buildProgress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static ConcurrentJobsType concurrentJobsType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static string diskCachePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static long diskCacheSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static bool enlightenForceUpdates { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static bool enlightenForceWhiteAlbedo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static FilterMode filterMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static GIWorkflowMode giWorkflowMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float indirectOutputScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isRunning { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static LightingDataAsset lightingDataAsset { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("lightmapSnapshot has been deprecated. Use lightingDataAsset instead (UnityUpgradable) -> lightingDataAsset", true)]
        public static LightmapSnapshot lightmapSnapshot
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        internal static bool openRLEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static bool realtimeLightmapsEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal enum ConcurrentJobsType
        {
            Min,
            Low,
            High
        }

        public enum GIWorkflowMode
        {
            Iterative,
            OnDemand,
            Legacy
        }

        public delegate void OnCompletedFunction();
    }
}

