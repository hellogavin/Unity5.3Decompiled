namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEditor.Advertisements;
    using UnityEditor.Connect;
    using UnityEngine;

    [InitializeOnLoad]
    internal class AdsAccess : CloudServiceAccess
    {
        private const string kServiceDisplayName = "Ads";
        private const string kServiceName = "Unity Ads";
        private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/5.3/production/cloud/ads";

        static AdsAccess()
        {
            UnityConnectServiceData cloudService = new UnityConnectServiceData("Unity Ads", "https://public-cdn.cloud.unity3d.com/editor/5.3/production/cloud/ads", new AdsAccess(), "unity/project/cloud/ads");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public override void EnableService(bool enabled)
        {
            AdvertisementSettings.enabled = enabled;
        }

        public string GetAndroidGameId()
        {
            return AdvertisementSettings.GetGameId(RuntimePlatform.Android);
        }

        public string GetIOSGameId()
        {
            return AdvertisementSettings.GetGameId(RuntimePlatform.IPhonePlayer);
        }

        public override string GetServiceDisplayName()
        {
            return "Ads";
        }

        public override string GetServiceName()
        {
            return "Unity Ads";
        }

        public bool IsAndroidEnabled()
        {
            return AdvertisementSettings.IsPlatformEnabled(RuntimePlatform.Android);
        }

        public bool IsInitializedOnStartup()
        {
            return AdvertisementSettings.initializeOnStartup;
        }

        public bool IsIOSEnabled()
        {
            return AdvertisementSettings.IsPlatformEnabled(RuntimePlatform.IPhonePlayer);
        }

        public override bool IsServiceEnabled()
        {
            return AdvertisementSettings.enabled;
        }

        public bool IsTestModeEnabled()
        {
            return AdvertisementSettings.testMode;
        }

        public void SetAndroidEnabled(bool enabled)
        {
            AdvertisementSettings.SetPlatformEnabled(RuntimePlatform.Android, enabled);
        }

        public void SetAndroidGameId(string value)
        {
            AdvertisementSettings.SetGameId(RuntimePlatform.Android, value);
        }

        public void SetInitializedOnStartup(bool enabled)
        {
            AdvertisementSettings.initializeOnStartup = enabled;
        }

        public void SetIOSEnabled(bool enabled)
        {
            AdvertisementSettings.SetPlatformEnabled(RuntimePlatform.IPhonePlayer, enabled);
        }

        public void SetIOSGameId(string value)
        {
            AdvertisementSettings.SetGameId(RuntimePlatform.IPhonePlayer, value);
        }

        public void SetTestModeEnabled(bool enabled)
        {
            AdvertisementSettings.testMode = enabled;
        }
    }
}

