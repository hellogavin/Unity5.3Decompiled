namespace UnityEditor.Modules
{
    using System;

    internal interface IPreferenceWindowExtension
    {
        bool HasExternalApplications();
        void ReadPreferences();
        void ShowExternalApplications();
        void WritePreferences();
    }
}

