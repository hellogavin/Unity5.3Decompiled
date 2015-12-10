namespace UnityEditor.Modules
{
    using System;
    using UnityEditor;

    internal abstract class DefaultPlayerSettingsEditorExtension : ISettingEditorExtension
    {
        protected DefaultPlayerSettingsEditorExtension()
        {
        }

        public virtual bool CanShowUnitySplashScreen()
        {
            return false;
        }

        public virtual void ConfigurationSectionGUI()
        {
        }

        public virtual bool HasBundleIdentifier()
        {
            return true;
        }

        public virtual bool HasIdentificationGUI()
        {
            return false;
        }

        public virtual bool HasPublishSection()
        {
            return true;
        }

        public virtual bool HasResolutionSection()
        {
            return false;
        }

        public virtual void IconSectionGUI()
        {
        }

        public virtual void IdentificationSectionGUI()
        {
        }

        public virtual void OnEnable(PlayerSettingsEditor settingsEditor)
        {
        }

        public virtual void PublishSectionGUI(float h, float midWidth, float maxWidth)
        {
        }

        public virtual void ResolutionSectionGUI(float h, float midWidth, float maxWidth)
        {
        }

        public virtual void SplashSectionGUI()
        {
        }

        public virtual bool SupportsDynamicBatching()
        {
            return true;
        }

        public virtual bool SupportsOrientation()
        {
            return false;
        }

        public virtual bool SupportsStaticBatching()
        {
            return true;
        }
    }
}

