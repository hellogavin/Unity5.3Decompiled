namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class GUIViewDebuggerHelper
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void DebugWindow(GUIView view);
        internal static GUIContent GetContentFromInstruction(int instructionIndex)
        {
            return new GUIContent { text = GetContentTextFromInstruction(instructionIndex), image = GetContentImageFromInstruction(instructionIndex) };
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Texture GetContentImageFromInstruction(int instructionIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetContentTextFromInstruction(int instructionIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetInstructionCount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern StackFrame[] GetManagedStackTrace(int instructionIndex);
        public static Rect GetRectFromInstruction(int instructionIndex)
        {
            Rect rect;
            INTERNAL_CALL_GetRectFromInstruction(instructionIndex, out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GUIStyle GetStyleFromInstruction(int instructionIndex);
        internal static void GetViews(List<GUIView> views)
        {
            GetViewsInternal(views);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void GetViewsInternal(object views);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetRectFromInstruction(int instructionIndex, out Rect value);
    }
}

