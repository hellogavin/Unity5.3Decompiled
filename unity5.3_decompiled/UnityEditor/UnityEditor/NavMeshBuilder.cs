namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public sealed class NavMeshBuilder
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BuildNavMesh();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BuildNavMeshAsync();
        public static void BuildNavMeshForMultipleScenes(string[] paths)
        {
            if (paths.Length != 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    for (int j = i + 1; j < paths.Length; j++)
                    {
                        if (paths[i] == paths[j])
                        {
                            throw new Exception("No duplicate scene names are allowed");
                        }
                    }
                }
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    if (!EditorSceneManager.OpenScene(paths[0]).IsValid())
                    {
                        throw new Exception("Could not open scene: " + paths[0]);
                    }
                    for (int k = 1; k < paths.Length; k++)
                    {
                        EditorSceneManager.OpenScene(paths[k], OpenSceneMode.Additive);
                    }
                    BuildNavMesh();
                    Object sceneNavMeshData = NavMeshBuilder.sceneNavMeshData;
                    for (int m = 0; m < paths.Length; m++)
                    {
                        if (EditorSceneManager.OpenScene(paths[m]).IsValid())
                        {
                            NavMeshBuilder.sceneNavMeshData = sceneNavMeshData;
                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Cancel();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearAllNavMeshes();

        public static bool isRunning { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static Object navMeshSettingsObject { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static Object sceneNavMeshData { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

