namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class ADBannerView
    {
        private static bool _AlwaysFalseDummy;
        private IntPtr _bannerView;
        private Layout _layout;

        public static  event BannerFailedToLoadDelegate onBannerFailedToLoad;

        public static  event BannerWasClickedDelegate onBannerWasClicked;

        public static  event BannerWasLoadedDelegate onBannerWasLoaded;

        public ADBannerView(Type type, Layout layout)
        {
            if (_AlwaysFalseDummy)
            {
                FireBannerWasClicked();
                FireBannerWasLoaded();
                FireBannerFailedToLoad();
            }
            this._bannerView = Native_CreateBanner((int) type, (int) layout);
        }

        ~ADBannerView()
        {
            Native_DestroyBanner(this._bannerView);
        }

        [RequiredByNativeCode]
        private static void FireBannerFailedToLoad()
        {
            if (onBannerFailedToLoad != null)
            {
                onBannerFailedToLoad();
            }
        }

        [RequiredByNativeCode]
        private static void FireBannerWasClicked()
        {
            if (onBannerWasClicked != null)
            {
                onBannerWasClicked();
            }
        }

        [RequiredByNativeCode]
        private static void FireBannerWasLoaded()
        {
            if (onBannerWasLoaded != null)
            {
                onBannerWasLoaded();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Native_MoveBanner(IntPtr view, ref Vector2 pos);
        public static bool IsAvailable(Type type)
        {
            return Native_BannerTypeAvailable((int) type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Native_BannerAdLoaded(IntPtr view);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Native_BannerAdVisible(IntPtr view);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Native_BannerPosition(IntPtr view, out Vector2 pos);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Native_BannerSize(IntPtr view, out Vector2 pos);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Native_BannerTypeAvailable(int type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern IntPtr Native_CreateBanner(int type, int layout);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Native_DestroyBanner(IntPtr view);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Native_LayoutBanner(IntPtr view, int layout);
        private static void Native_MoveBanner(IntPtr view, Vector2 pos)
        {
            INTERNAL_CALL_Native_MoveBanner(view, ref pos);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Native_ShowBanner(IntPtr view, bool show);
        private Vector2 OSToScreenCoords(Vector2 v)
        {
            return new Vector2(v.x * Screen.width, v.y * Screen.height);
        }

        public Layout layout
        {
            get
            {
                return this._layout;
            }
            set
            {
                this._layout = value;
                Native_LayoutBanner(this._bannerView, (int) this._layout);
            }
        }

        public bool loaded
        {
            get
            {
                return Native_BannerAdLoaded(this._bannerView);
            }
        }

        public Vector2 position
        {
            get
            {
                Vector2 vector;
                Native_BannerPosition(this._bannerView, out vector);
                return this.OSToScreenCoords(vector);
            }
            set
            {
                Vector2 pos = new Vector2(value.x / ((float) Screen.width), value.y / ((float) Screen.height));
                Native_MoveBanner(this._bannerView, pos);
            }
        }

        public Vector2 size
        {
            get
            {
                Vector2 vector;
                Native_BannerSize(this._bannerView, out vector);
                return this.OSToScreenCoords(vector);
            }
        }

        public bool visible
        {
            get
            {
                return Native_BannerAdVisible(this._bannerView);
            }
            set
            {
                Native_ShowBanner(this._bannerView, value);
            }
        }

        public delegate void BannerFailedToLoadDelegate();

        public delegate void BannerWasClickedDelegate();

        public delegate void BannerWasLoadedDelegate();

        public enum Layout
        {
            Bottom = 1,
            BottomCenter = 9,
            BottomLeft = 1,
            BottomRight = 5,
            Center = 10,
            CenterLeft = 2,
            CenterRight = 6,
            Manual = -1,
            Top = 0,
            TopCenter = 8,
            TopLeft = 0,
            TopRight = 4
        }

        public enum Type
        {
            Banner,
            MediumRect
        }
    }
}

