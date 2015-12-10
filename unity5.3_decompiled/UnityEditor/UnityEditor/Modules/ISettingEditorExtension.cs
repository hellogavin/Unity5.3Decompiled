namespace UnityEditor.Modules
{
    using System;
    using UnityEditor;

    internal interface ISettingEditorExtension
    {
        bool CanShowUnitySplashScreen();
        void ConfigurationSectionGUI();
        bool HasBundleIdentifier();
        bool HasIdentificationGUI();
        bool HasPublishSection();
        bool HasResolutionSection();
        void IconSectionGUI();
        void IdentificationSectionGUI();
        void OnEnable(PlayerSettingsEditor settingsEditor);
        void PublishSectionGUI(float h, float midWidth, float maxWidth);
        void ResolutionSectionGUI(float h, float midWidth, float maxWidth);
        void SplashSectionGUI();
        bool SupportsDynamicBatching();
        bool SupportsOrientation();
        bool SupportsStaticBatching();
    }
}

