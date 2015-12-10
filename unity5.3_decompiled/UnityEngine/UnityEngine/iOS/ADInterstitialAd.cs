namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class ADInterstitialAd
    {
        private static bool _AlwaysFalseDummy;
        private IntPtr interstitialView;

        public static  event InterstitialWasLoadedDelegate onInterstitialWasLoaded;

        public static  event InterstitialWasViewedDelegate onInterstitialWasViewed;

        public ADInterstitialAd()
        {
            this.CtorImpl(false);
        }

        public ADInterstitialAd(bool autoReload)
        {
            this.CtorImpl(autoReload);
        }

        private void CtorImpl(bool autoReload)
        {
            if (_AlwaysFalseDummy)
            {
                FireInterstitialWasLoaded();
                FireInterstitialWasViewed();
            }
            this.interstitialView = Native_CreateInterstitial(autoReload);
        }

        ~ADInterstitialAd()
        {
            Native_DestroyInterstitial(this.interstitialView);
        }

        [RequiredByNativeCode]
        private static void FireInterstitialWasLoaded()
        {
            if (onInterstitialWasLoaded != null)
            {
                onInterstitialWasLoaded();
            }
        }

        [RequiredByNativeCode]
        private static void FireInterstitialWasViewed()
        {
            if (onInterstitialWasViewed != null)
            {
                onInterstitialWasViewed();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern IntPtr Native_CreateInterstitial(bool autoReload);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Native_DestroyInterstitial(IntPtr view);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Native_InterstitialAdLoaded(IntPtr view);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Native_InterstitialAvailable();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Native_ReloadInterstitial(IntPtr view);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Native_ShowInterstitial(IntPtr view);
        public void ReloadAd()
        {
            Native_ReloadInterstitial(this.interstitialView);
        }

        public void Show()
        {
            if (this.loaded)
            {
                Native_ShowInterstitial(this.interstitialView);
            }
            else
            {
                Debug.Log("Calling ADInterstitialAd.Show() when the ad is not loaded");
            }
        }

        public static bool isAvailable
        {
            get
            {
                return Native_InterstitialAvailable();
            }
        }

        public bool loaded
        {
            get
            {
                return Native_InterstitialAdLoaded(this.interstitialView);
            }
        }

        public delegate void InterstitialWasLoadedDelegate();

        public delegate void InterstitialWasViewedDelegate();
    }
}

