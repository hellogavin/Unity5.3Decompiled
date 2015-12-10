namespace UnityEditor.SceneManagement
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.SceneManagement;

    public sealed class EditorSceneManager : SceneManager
    {
        public static bool CloseScene(Scene scene, bool removeScene)
        {
            return INTERNAL_CALL_CloseScene(ref scene, removeScene);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool CreateSceneAsset(string scenePath, bool createDefaultGameObjects);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool EnsureUntitledSceneHasBeenSaved(string operation);
        internal static Scene GetSceneByHandle(int handle)
        {
            Scene scene;
            INTERNAL_CALL_GetSceneByHandle(handle, out scene);
            return scene;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern SceneSetup[] GetSceneManagerSetup();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_CloseScene(ref Scene scene, bool removeScene);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetSceneByHandle(int handle, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_MarkSceneDirty(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MoveSceneAfter(ref Scene src, ref Scene dst);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MoveSceneBefore(ref Scene src, ref Scene dst);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_NewScene(NewSceneSetup setup, NewSceneMode mode, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_OpenScene(string scenePath, OpenSceneMode mode, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_SaveScene(ref Scene scene, string dstScenePath, bool saveAsCopy);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_SaveSceneAs(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void MarkAllScenesDirty();
        public static bool MarkSceneDirty(Scene scene)
        {
            return INTERNAL_CALL_MarkSceneDirty(ref scene);
        }

        public static void MoveSceneAfter(Scene src, Scene dst)
        {
            INTERNAL_CALL_MoveSceneAfter(ref src, ref dst);
        }

        public static void MoveSceneBefore(Scene src, Scene dst)
        {
            INTERNAL_CALL_MoveSceneBefore(ref src, ref dst);
        }

        [ExcludeFromDocs]
        public static Scene NewScene(NewSceneSetup setup)
        {
            Scene scene;
            NewSceneMode single = NewSceneMode.Single;
            INTERNAL_CALL_NewScene(setup, single, out scene);
            return scene;
        }

        public static Scene NewScene(NewSceneSetup setup, [DefaultValue("NewSceneMode.Single")] NewSceneMode mode)
        {
            Scene scene;
            INTERNAL_CALL_NewScene(setup, mode, out scene);
            return scene;
        }

        [ExcludeFromDocs]
        public static Scene OpenScene(string scenePath)
        {
            Scene scene;
            OpenSceneMode single = OpenSceneMode.Single;
            INTERNAL_CALL_OpenScene(scenePath, single, out scene);
            return scene;
        }

        public static Scene OpenScene(string scenePath, [DefaultValue("OpenSceneMode.Single")] OpenSceneMode mode)
        {
            Scene scene;
            INTERNAL_CALL_OpenScene(scenePath, mode, out scene);
            return scene;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RestoreSceneManagerSetup(SceneSetup[] value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SaveCurrentModifiedScenesIfUserWantsTo();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SaveModifiedScenesIfUserWantsTo(Scene[] scenes);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SaveOpenScenes();
        [ExcludeFromDocs]
        public static bool SaveScene(Scene scene)
        {
            bool saveAsCopy = false;
            string dstScenePath = string.Empty;
            return INTERNAL_CALL_SaveScene(ref scene, dstScenePath, saveAsCopy);
        }

        [ExcludeFromDocs]
        public static bool SaveScene(Scene scene, string dstScenePath)
        {
            bool saveAsCopy = false;
            return INTERNAL_CALL_SaveScene(ref scene, dstScenePath, saveAsCopy);
        }

        public static bool SaveScene(Scene scene, [DefaultValue("\"\"")] string dstScenePath, [DefaultValue("false")] bool saveAsCopy)
        {
            return INTERNAL_CALL_SaveScene(ref scene, dstScenePath, saveAsCopy);
        }

        internal static bool SaveSceneAs(Scene scene)
        {
            return INTERNAL_CALL_SaveSceneAs(ref scene);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SaveScenes(Scene[] scenes);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetTargetSceneForNewGameObjects(int sceneHandle);

        public static int loadedSceneCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

