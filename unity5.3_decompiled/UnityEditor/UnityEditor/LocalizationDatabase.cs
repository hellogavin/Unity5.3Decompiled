namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class LocalizationDatabase
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern SystemLanguage[] GetAvailableEditorLanguages();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern SystemLanguage GetCurrentEditorLanguage();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern SystemLanguage GetDefaultEditorLanguage();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetLocalizedString(string original);
        public static string MarkForTranslation(string value)
        {
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ReadEditorLocalizationResources();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetCurrentEditorLanguage(SystemLanguage lang);
    }
}

