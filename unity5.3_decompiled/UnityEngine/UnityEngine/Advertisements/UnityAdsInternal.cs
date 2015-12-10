namespace UnityEngine.Advertisements
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    internal sealed class UnityAdsInternal
    {
        public static  event UnityAdsDelegate onCampaignsAvailable;

        public static  event UnityAdsDelegate onCampaignsFetchFailed;

        public static  event UnityAdsDelegate onHide;

        public static  event UnityAdsDelegate onShow;

        public static  event UnityAdsDelegate<string, bool> onVideoCompleted;

        public static  event UnityAdsDelegate onVideoStarted;

        public static void CallUnityAdsCampaignsAvailable()
        {
            UnityAdsDelegate onCampaignsAvailable = UnityAdsInternal.onCampaignsAvailable;
            if (onCampaignsAvailable != null)
            {
                onCampaignsAvailable();
            }
        }

        public static void CallUnityAdsCampaignsFetchFailed()
        {
            UnityAdsDelegate onCampaignsFetchFailed = UnityAdsInternal.onCampaignsFetchFailed;
            if (onCampaignsFetchFailed != null)
            {
                onCampaignsFetchFailed();
            }
        }

        public static void CallUnityAdsHide()
        {
            UnityAdsDelegate onHide = UnityAdsInternal.onHide;
            if (onHide != null)
            {
                onHide();
            }
        }

        public static void CallUnityAdsShow()
        {
            UnityAdsDelegate onShow = UnityAdsInternal.onShow;
            if (onShow != null)
            {
                onShow();
            }
        }

        public static void CallUnityAdsVideoCompleted(string rewardItemKey, bool skipped)
        {
            UnityAdsDelegate<string, bool> onVideoCompleted = UnityAdsInternal.onVideoCompleted;
            if (onVideoCompleted != null)
            {
                onVideoCompleted(rewardItemKey, skipped);
            }
        }

        public static void CallUnityAdsVideoStarted()
        {
            UnityAdsDelegate onVideoStarted = UnityAdsInternal.onVideoStarted;
            if (onVideoStarted != null)
            {
                onVideoStarted();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CanShowAds(string zoneId);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Init(string gameId, bool testModeEnabled, bool debugModeEnabled, string unityVersion);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RegisterNative();
        public static void RemoveAllEventHandlers()
        {
            onCampaignsAvailable = null;
            onCampaignsFetchFailed = null;
            onShow = null;
            onHide = null;
            onVideoCompleted = null;
            onVideoStarted = null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetCampaignDataURL(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetLogLevel(int logLevel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Show(string zoneId, string rewardItemKey, string options);
    }
}

