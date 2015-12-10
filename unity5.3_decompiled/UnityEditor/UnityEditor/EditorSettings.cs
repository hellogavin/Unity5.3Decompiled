namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.VisualStudioIntegration;
    using UnityEngine;

    public sealed class EditorSettings : Object
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;

        public static EditorBehaviorMode defaultBehaviorMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string externalVersionControl { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static string Internal_ProjectGenerationUserExtensions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string[] projectGenerationBuiltinExtensions
        {
            get
            {
                return SolutionSynchronizer.BuiltinSupportedExtensions.Keys.ToArray<string>();
            }
        }

        public static string projectGenerationRootNamespace { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string[] projectGenerationUserExtensions
        {
            get
            {
                char[] separator = new char[] { ';' };
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = s => s.TrimStart(new char[] { '.', '*' });
                }
                return Internal_ProjectGenerationUserExtensions.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select<string, string>(<>f__am$cache0).ToArray<string>();
            }
            set
            {
                Internal_ProjectGenerationUserExtensions = string.Join(";", value);
            }
        }

        public static SerializationMode serializationMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static SpritePackerMode spritePackerMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int spritePackerPaddingPower { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string unityRemoteCompression { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string unityRemoteDevice { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string unityRemoteResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool webSecurityEmulationEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string webSecurityEmulationHostUrl { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

