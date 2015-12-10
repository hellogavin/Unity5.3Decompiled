namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SavedGUIState
    {
        internal GUILayoutUtility.LayoutCache layoutCache;
        internal IntPtr guiState;
        internal Vector2 screenManagerSize;
        internal Rect renderManagerRect;
        internal GUISkin skin;
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetupSavedGUIState(out IntPtr state, out Vector2 screenManagerSize);
        private static void Internal_ApplySavedGUIState(IntPtr state, Vector2 screenManagerSize)
        {
            INTERNAL_CALL_Internal_ApplySavedGUIState(state, ref screenManagerSize);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_ApplySavedGUIState(IntPtr state, ref Vector2 screenManagerSize);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int Internal_GetGUIDepth();
        internal static SavedGUIState Create()
        {
            SavedGUIState state = new SavedGUIState();
            if (Internal_GetGUIDepth() > 0)
            {
                state.skin = GUI.skin;
                state.layoutCache = new GUILayoutUtility.LayoutCache(GUILayoutUtility.current);
                Internal_SetupSavedGUIState(out state.guiState, out state.screenManagerSize);
            }
            return state;
        }

        internal void ApplyAndForget()
        {
            if (this.layoutCache != null)
            {
                GUILayoutUtility.current = this.layoutCache;
                GUI.skin = this.skin;
                Internal_ApplySavedGUIState(this.guiState, this.screenManagerSize);
                GUIClip.Reapply();
            }
        }
    }
}

