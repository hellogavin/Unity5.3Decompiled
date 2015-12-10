namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    public sealed class Screen
    {
        [ExcludeFromDocs]
        public static void SetResolution(int width, int height, bool fullscreen)
        {
            int preferredRefreshRate = 0;
            SetResolution(width, height, fullscreen, preferredRefreshRate);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetResolution(int width, int height, bool fullscreen, [DefaultValue("0")] int preferredRefreshRate);

        public static bool autorotateToLandscapeLeft { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool autorotateToLandscapeRight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool autorotateToPortrait { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool autorotateToPortraitUpsideDown { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Resolution currentResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float dpi { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool fullScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property GetResolution has been deprecated. Use resolutions instead (UnityUpgradable) -> resolutions", true)]
        public static Resolution[] GetResolution
        {
            get
            {
                return null;
            }
        }

        public static int height { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Property lockCursor has been deprecated. Use Cursor.lockState and Cursor.visible instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static bool lockCursor
        {
            get
            {
                return (CursorLockMode.Locked == Cursor.lockState);
            }
            set
            {
                if (value)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        public static ScreenOrientation orientation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Resolution[] resolutions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Property showCursor has been deprecated. Use Cursor.visible instead (UnityUpgradable) -> UnityEngine.Cursor.visible", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static bool showCursor
        {
            [CompilerGenerated]
            get
            {
                return <showCursor>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <showCursor>k__BackingField = value;
            }
        }

        public static int sleepTimeout { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int width { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

