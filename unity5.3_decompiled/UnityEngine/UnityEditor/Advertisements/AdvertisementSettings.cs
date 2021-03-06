﻿namespace UnityEditor.Advertisements
{
    using System;
    using UnityEngine;
    using UnityEngine.Advertisements;

    public static class AdvertisementSettings
    {
        public static string GetGameId(RuntimePlatform platform)
        {
            return UnityAdsManager.GetGameId(platform);
        }

        public static bool IsPlatformEnabled(RuntimePlatform platform)
        {
            return UnityAdsManager.IsPlatformEnabled(platform);
        }

        public static void SetGameId(RuntimePlatform platform, string gameId)
        {
            UnityAdsManager.SetGameId(platform, gameId);
        }

        public static void SetPlatformEnabled(RuntimePlatform platform, bool value)
        {
            UnityAdsManager.SetPlatformEnabled(platform, value);
        }

        public static bool enabled
        {
            get
            {
                return UnityAdsManager.enabled;
            }
            set
            {
                UnityAdsManager.enabled = value;
            }
        }

        public static bool initializeOnStartup
        {
            get
            {
                return UnityAdsManager.initializeOnStartup;
            }
            set
            {
                UnityAdsManager.initializeOnStartup = value;
            }
        }

        public static bool testMode
        {
            get
            {
                return UnityAdsManager.testMode;
            }
            set
            {
                UnityAdsManager.testMode = value;
            }
        }
    }
}

