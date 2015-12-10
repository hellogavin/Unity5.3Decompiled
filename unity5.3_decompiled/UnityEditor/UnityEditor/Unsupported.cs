namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Animations;
    using UnityEngine;

    public sealed class Unsupported
    {
        private static bool s_FakeNonDeveloperBuild = EditorPrefs.GetBool("FakeNonDeveloperBuild", false);

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CaptureScreenshotImmediate(string filePath, int x, int y, int width, int height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearSkinCache();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CopyComponentToPasteboard(Component component);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CopyGameObjectsToPasteboard();
        public static void CopyStateMachineDataToPasteboard(Object stateMachineObject, AnimatorController controller, int layerIndex)
        {
            Object[] stateMachineObjects = new Object[] { stateMachineObject };
            Vector3[] monoPositions = new Vector3[] { new Vector3() };
            CopyStateMachineDataToPasteboard(stateMachineObjects, monoPositions, controller, layerIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CopyStateMachineDataToPasteboard(Object[] stateMachineObjects, Vector3[] monoPositions, AnimatorController controller, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DeleteGameObjectSelection();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DuplicateGameObjectsUsingPasteboard();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetApplicationSettingCompressAssetsOnImport();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetBaseUnityDeveloperFolder();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetLocalIdentifierInFile(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern NETVersion GetNETVersion();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object GetSerializedAssetInterfaceSingleton(string className);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetSubmenus(string menuPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetSubmenusCommands(string menuPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetSubmenusIncludingSeparators(string menuPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Type GetTypeFromFullName(string fullName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasStateMachineDataInPasteboard();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MakeNiceVector3(ref Vector3 vector, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_PasteToStateMachineFromPasteboardInternal(AnimatorStateMachine sm, AnimatorController controller, int layerIndex, ref Vector3 position);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsBleedingEdgeBuild();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsDestroyScriptableObject(ScriptableObject target);
        public static bool IsDeveloperBuild()
        {
            return (IsDeveloperBuildInternal() && !s_FakeNonDeveloperBuild);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool IsDeveloperBuildInternal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsHiddenFile(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsNativeCodeBuiltInReleaseMode();
        internal static Vector3 MakeNiceVector3(Vector3 vector)
        {
            Vector3 vector2;
            INTERNAL_CALL_MakeNiceVector3(ref vector, out vector2);
            return vector2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool PasteComponentFromPasteboard(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool PasteComponentValuesFromPasteboard(Component component);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PasteGameObjectsFromPasteboard();
        public static void PasteToStateMachineFromPasteboard(AnimatorStateMachine sm, AnimatorController controller, int layerIndex, Vector3 position)
        {
            Undo.RegisterCompleteObjectUndo(sm, "Paste to StateMachine");
            PasteToStateMachineFromPasteboardInternal(sm, controller, layerIndex, position);
        }

        internal static void PasteToStateMachineFromPasteboardInternal(AnimatorStateMachine sm, AnimatorController controller, int layerIndex, Vector3 position)
        {
            INTERNAL_CALL_PasteToStateMachineFromPasteboardInternal(sm, controller, layerIndex, ref position);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PrepareObjectContextMenu(Object c, int contextUserData);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string ResolveSymlinks(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SceneTrackerFlushDirty();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAllowCursorHide(bool allow);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAllowCursorLock(bool allow);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetApplicationSettingCompressAssetsOnImport(bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetQualitySettingsShadowDistanceTemporarily(float distance);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetRenderSettingsUseFogNoDirty(bool fog);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SmartReset(Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StopPlayingImmediately();

        internal static bool fakeNonDeveloperBuild
        {
            get
            {
                return s_FakeNonDeveloperBuild;
            }
            set
            {
                s_FakeNonDeveloperBuild = value;
                EditorPrefs.SetBool("FakeNonDeveloperBuild", value);
            }
        }
    }
}

