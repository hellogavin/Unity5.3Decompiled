namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class AnnotationUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void DeletePreset(string presetName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Annotation[] GetAnnotations();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetNameOfCurrentSetup();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string[] GetPresetList();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Annotation[] GetRecentlyChangedAnnotations();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void LoadPreset(string presetName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ResetToFactorySettings();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SavePreset(string presetName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetGizmoEnabled(int classID, string scriptClass, int gizmoEnabled);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetIconEnabled(int classID, string scriptClass, int iconEnabled);

        internal static float iconSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static bool showGrid { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static bool use3dGizmos { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

