namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class AnimationMode
    {
        private static Color s_AnimatedPropertyColorDark = new Color(1f, 0.55f, 0.5f, 1f);
        private static Color s_AnimatedPropertyColorLight = new Color(1f, 0.65f, 0.6f, 1f);
        private static bool s_InAnimationPlaybackMode = false;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void AddPropertyModification(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BeginSampling();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void EndSampling();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool InAnimationMode();
        internal static bool InAnimationPlaybackMode()
        {
            return s_InAnimationPlaybackMode;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsPropertyAnimated(Object target, string propertyPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SampleAnimationClip(GameObject gameObject, AnimationClip clip, float time);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StartAnimationMode();
        internal static void StartAnimationPlaybackMode()
        {
            s_InAnimationPlaybackMode = true;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StopAnimationMode();
        internal static void StopAnimationPlaybackMode()
        {
            s_InAnimationPlaybackMode = false;
        }

        public static Color animatedPropertyColor
        {
            get
            {
                return (!EditorGUIUtility.isProSkin ? s_AnimatedPropertyColorLight : s_AnimatedPropertyColorDark);
            }
        }
    }
}

