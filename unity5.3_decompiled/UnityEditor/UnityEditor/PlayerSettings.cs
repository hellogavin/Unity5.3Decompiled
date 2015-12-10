namespace UnityEditor
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;

    public sealed class PlayerSettings : Object
    {
        private static SerializedObject _serializedObject;
        internal static readonly char[] defineSplits = new char[] { ';', ',', ' ' };

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void AddEnum(string className, string propertyName, int value, string valueName);
        internal static SerializedProperty FindProperty(string name)
        {
            SerializedProperty property = GetSerializedObject().FindProperty(name);
            if (property == null)
            {
                Debug.LogError("Failed to find:" + name);
            }
            return property;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void GetBatchingForPlatform(BuildTarget platform, out int staticBatching, out int dynamicBatching);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GraphicsDeviceType[] GetGraphicsAPIs(BuildTarget platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Texture2D GetIconForPlatformAtSize(string platform, int width, int height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int[] GetIconHeightsForPlatform(string platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Texture2D[] GetIconsForPlatform(string platform);
        public static Texture2D[] GetIconsForTargetGroup(BuildTargetGroup platform)
        {
            Texture2D[] iconsForPlatform = GetIconsForPlatform(GetPlatformName(platform));
            if (iconsForPlatform.Length == 0)
            {
                return new Texture2D[GetIconSizesForTargetGroup(platform).Length];
            }
            return iconsForPlatform;
        }

        public static int[] GetIconSizesForTargetGroup(BuildTargetGroup platform)
        {
            return GetIconWidthsForPlatform(GetPlatformName(platform));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int[] GetIconWidthsForPlatform(string platform);
        internal static string GetPlatformName(BuildTargetGroup targetGroup)
        {
            <GetPlatformName>c__AnonStorey14 storey = new <GetPlatformName>c__AnonStorey14 {
                targetGroup = targetGroup
            };
            BuildPlayerWindow.BuildPlatform platform = BuildPlayerWindow.GetValidPlatforms().Find(new Predicate<BuildPlayerWindow.BuildPlatform>(storey.<>m__1B));
            return ((platform != null) ? platform.name : string.Empty);
        }

        [ExcludeFromDocs]
        public static bool GetPropertyBool(string name)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyBool(name, unknown);
        }

        public static bool GetPropertyBool(string name, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            return GetPropertyBoolInternal(GetPropertyNameForBuildTarget(target, name));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetPropertyBoolInternal(string name);
        [ExcludeFromDocs]
        public static int GetPropertyInt(string name)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyInt(name, unknown);
        }

        public static int GetPropertyInt(string name, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            return GetPropertyIntInternal(GetPropertyNameForBuildTarget(target, name));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetPropertyIntInternal(string name);
        internal static string GetPropertyNameForBuildTarget(BuildTargetGroup target, string name)
        {
            string propertyNameForBuildTargetGroupInternal = GetPropertyNameForBuildTargetGroupInternal(target, name);
            if (propertyNameForBuildTargetGroupInternal == string.Empty)
            {
                throw new ArgumentException("Failed to get property name for the given target.");
            }
            return propertyNameForBuildTargetGroupInternal;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetPropertyNameForBuildTargetGroupInternal(BuildTargetGroup target, string name);
        [ExcludeFromDocs]
        public static bool GetPropertyOptionalBool(string name, ref bool value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyOptionalBool(name, ref value, unknown);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetPropertyOptionalBool(string name, ref bool value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);
        [ExcludeFromDocs]
        public static bool GetPropertyOptionalInt(string name, ref int value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyOptionalInt(name, ref value, unknown);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetPropertyOptionalInt(string name, ref int value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);
        [ExcludeFromDocs]
        public static bool GetPropertyOptionalString(string name, ref string value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyOptionalString(name, ref value, unknown);
        }

        public static bool GetPropertyOptionalString(string name, ref string value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            string propertyOptionalStringInternal = GetPropertyOptionalStringInternal(name, target);
            if (propertyOptionalStringInternal == null)
            {
                return false;
            }
            value = propertyOptionalStringInternal;
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetPropertyOptionalStringInternal(string name, BuildTargetGroup target);
        [ExcludeFromDocs]
        public static string GetPropertyString(string name)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyString(name, unknown);
        }

        public static string GetPropertyString(string name, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            return GetPropertyStringInternal(GetPropertyNameForBuildTarget(target, name));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetPropertyStringInternal(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup);
        internal static SerializedObject GetSerializedObject()
        {
            if (_serializedObject == null)
            {
                _serializedObject = new SerializedObject(InternalGetPlayerSettingsObject());
            }
            return _serializedObject;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern GraphicsDeviceType[] GetSupportedGraphicsAPIs(BuildTarget platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetTemplateCustomValue(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetUseDefaultGraphicsAPIs(BuildTarget platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasAspectRatio(AspectRatio aspectRatio);
        [ExcludeFromDocs]
        internal static void InitializePropertyBool(string name, bool value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            InitializePropertyBool(name, value, unknown);
        }

        internal static void InitializePropertyBool(string name, bool value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            InitializePropertyBoolInternal(GetPropertyNameForBuildTarget(target, name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InitializePropertyBoolInternal(string name, bool value);
        [ExcludeFromDocs]
        internal static void InitializePropertyEnum(string name, object value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            InitializePropertyEnum(name, value, unknown);
        }

        internal static void InitializePropertyEnum(string name, object value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            string propertyNameForBuildTarget = GetPropertyNameForBuildTarget(target, name);
            string[] names = Enum.GetNames(value.GetType());
            Array values = Enum.GetValues(value.GetType());
            for (int i = 0; i < names.Length; i++)
            {
                AddEnum("PlayerSettings", propertyNameForBuildTarget, (int) values.GetValue(i), names[i]);
            }
            InitializePropertyIntInternal(propertyNameForBuildTarget, (int) value);
        }

        [ExcludeFromDocs]
        internal static void InitializePropertyInt(string name, int value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            InitializePropertyInt(name, value, unknown);
        }

        internal static void InitializePropertyInt(string name, int value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            InitializePropertyIntInternal(GetPropertyNameForBuildTarget(target, name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InitializePropertyIntInternal(string name, int value);
        [ExcludeFromDocs]
        internal static void InitializePropertyString(string name, string value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            InitializePropertyString(name, value, unknown);
        }

        internal static void InitializePropertyString(string name, string value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            InitializePropertyStringInternal(GetPropertyNameForBuildTarget(target, name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InitializePropertyStringInternal(string name, string value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Object InternalGetPlayerSettingsObject();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetApiCompatibilityInternal(int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAspectRatio(AspectRatio aspectRatio, bool enable);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetBatchingForPlatform(BuildTarget platform, int staticBatching, int dynamicBatching);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetGraphicsAPIs(BuildTarget platform, GraphicsDeviceType[] apis);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetIconsForPlatform(string platform, Texture2D[] icons);
        public static void SetIconsForTargetGroup(BuildTargetGroup platform, Texture2D[] icons)
        {
            SetIconsForPlatform(GetPlatformName(platform), icons);
        }

        [ExcludeFromDocs]
        public static void SetPropertyBool(string name, bool value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            SetPropertyBool(name, value, unknown);
        }

        public static void SetPropertyBool(string name, bool value, BuildTarget target)
        {
            SetPropertyBool(name, value, BuildPipeline.GetBuildTargetGroup(target));
        }

        public static void SetPropertyBool(string name, bool value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            SetPropertyBoolInternal(GetPropertyNameForBuildTarget(target, name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetPropertyBoolInternal(string name, bool value);
        [ExcludeFromDocs]
        public static void SetPropertyInt(string name, int value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            SetPropertyInt(name, value, unknown);
        }

        public static void SetPropertyInt(string name, int value, BuildTarget target)
        {
            SetPropertyInt(name, value, BuildPipeline.GetBuildTargetGroup(target));
        }

        public static void SetPropertyInt(string name, int value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            SetPropertyIntInternal(GetPropertyNameForBuildTarget(target, name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetPropertyIntInternal(string name, int value);
        [ExcludeFromDocs]
        public static void SetPropertyString(string name, string value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            SetPropertyString(name, value, unknown);
        }

        public static void SetPropertyString(string name, string value, BuildTarget target)
        {
            SetPropertyString(name, value, BuildPipeline.GetBuildTargetGroup(target));
        }

        public static void SetPropertyString(string name, string value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            SetPropertyStringInternal(GetPropertyNameForBuildTarget(target, name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetPropertyStringInternal(string name, string value);
        public static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string defines)
        {
            if (!string.IsNullOrEmpty(defines))
            {
                defines = string.Join(";", defines.Split(defineSplits, StringSplitOptions.RemoveEmptyEntries));
            }
            SetScriptingDefineSymbolsForGroupInternal(targetGroup, defines);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetScriptingDefineSymbolsForGroupInternal(BuildTargetGroup targetGroup, string defines);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetTemplateCustomValue(string key, string value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetUseDefaultGraphicsAPIs(BuildTarget platform, bool automatic);

        public static int accelerometerFrequency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static ActionOnDotNetUnhandledException actionOnDotNetUnhandledException { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool advancedLicense { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool allowedAutorotateToLandscapeLeft { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool allowedAutorotateToLandscapeRight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool allowedAutorotateToPortrait { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool allowedAutorotateToPortraitUpsideDown { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool allowFullscreenSwitch { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("The option alwaysDisplayWatermark is deprecated and is always false.")]
        public static bool alwaysDisplayWatermark
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public static string aotOptions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static ApiCompatibilityLevel apiCompatibilityLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool bakeCollisionMeshes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string bundleIdentifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string bundleVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool captureSingleScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string cloudProjectId
        {
            get
            {
                return cloudProjectIdRaw;
            }
            internal set
            {
                cloudProjectIdRaw = value;
            }
        }

        private static string cloudProjectIdRaw { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static ColorSpace colorSpace { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string companyName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static D3D11FullscreenMode d3d11FullscreenMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static D3D9FullscreenMode d3d9FullscreenMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static UIOrientation defaultInterfaceOrientation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool defaultIsFullScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool defaultIsNativeResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int defaultScreenHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int defaultScreenWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int defaultWebScreenHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int defaultWebScreenWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static ResolutionDialogSetting displayResolutionDialog { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool enableCrashReportAPI { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool enableInternalProfiler { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Use AssetBundles instead for streaming data", true)]
        public static int firstStreamedLevelWithResources
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public static bool forceSingleInstance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool gpuSkinning { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string iPhoneBundleIdentifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string keyaliasPass { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string keystorePass { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string locationUsageDescription { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool logObjCUncaughtExceptions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static MacFullscreenMode macFullscreenMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool mobileMTRendering { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static RenderingPath mobileRenderingPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool MTRendering { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Guid productGUID
        {
            get
            {
                return new Guid(productGUIDRaw);
            }
        }

        private static byte[] productGUIDRaw { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string productName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static RenderingPath renderingPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool resizableWindow { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Texture2D resolutionDialogBanner { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool runInBackground { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool showUnitySplashScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static string spritePackerPolicy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool statusBarHidden { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool stereoscopic3D { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool stripEngineCode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static StrippingLevel strippingLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool stripUnusedMeshComponents { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static bool submitAnalytics { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("targetGlesGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs")]
        public static TargetGlesGraphics targetGlesGraphics { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("targetIOSGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs")]
        public static TargetIOSGraphics targetIOSGraphics { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static string[] templateCustomKeys { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool use32BitDisplayBuffer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool useAnimatedAutorotation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Use SetGraphicsAPIs/GetGraphicsAPIs instead")]
        public static bool useDirect3D11 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool useMacAppStoreValidation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool usePlayerLog { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Texture2D virtualRealitySplashScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool virtualRealitySupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool visibleInBackground { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static string webPlayerTemplate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int xboxAdditionalTitleMemorySize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool xboxDeployKinectHeadOrientation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool xboxDeployKinectHeadPosition { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool xboxDeployKinectResources { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool xboxEnableAvatar { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool xboxEnableGuest { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool xboxEnableKinect { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool xboxEnableKinectAutoTracking { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool xboxEnableSpeech { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool xboxGenerateSpa { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string xboxImageXexFilePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int xboxOneResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool xboxPIXTextureCapture { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string xboxSpaFilePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static uint xboxSpeechDB { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static Texture2D xboxSplashScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string xboxTitleId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [CompilerGenerated]
        private sealed class <GetPlatformName>c__AnonStorey14
        {
            internal BuildTargetGroup targetGroup;

            internal bool <>m__1B(BuildPlayerWindow.BuildPlatform p)
            {
                return (p.targetGroup == this.targetGroup);
            }
        }

        public sealed class Android
        {
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            internal static extern Texture2D GetAndroidBannerForHeight(int height);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            internal static extern AndroidBanner[] GetAndroidBanners();
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            internal static extern void SetAndroidBanners(Texture2D[] banners);

            internal static bool androidBannerEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            internal static AndroidGamepadSupportLevel androidGamepadSupportLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool androidIsGame { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool androidTVCompatibility { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int bundleVersionCode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            internal static bool createWallpaper { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool disableDepthAndStencilBuffers { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool forceInternetPermission { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool forceSDCardPermission { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string keyaliasName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string keyaliasPass { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string keystoreName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string keystorePass { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool licenseVerification { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static AndroidSdkVersions minSdkVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static AndroidPreferredInstallLocation preferredInstallLocation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static AndroidShowActivityIndicatorOnLoading showActivityIndicatorOnLoading { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static AndroidSplashScreenScale splashScreenScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static AndroidTargetDevice targetDevice { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("This has been replaced by disableDepthAndStencilBuffers")]
            public static bool use24BitDepthBuffer
            {
                get
                {
                    return !disableDepthAndStencilBuffers;
                }
                set
                {
                }
            }

            public static bool useAPKExpansionFiles { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        }

        public sealed class BlackBerry
        {
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern bool HasCameraPermissions();
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern bool HasGamepadSupport();
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern bool HasGPSPermissions();
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern bool HasIdentificationPermissions();
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern bool HasMicrophonePermissions();
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern bool HasSharedPermissions();
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void SetCameraPermissions(bool enable);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void SetGamepadSupport(bool enable);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void SetGPSPermissions(bool enable);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void SetIdentificationPermissions(bool enable);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void SetMicrophonePermissions(bool enable);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void SetSharedPermissions(bool enable);

            public static string cskPassword { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string deviceAddress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string devicePassword { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string saveLogPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string tokenAuthor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string tokenAuthorId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string tokenExpires { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string tokenPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        }

        public sealed class iOS
        {
            internal static iOSDeviceRequirementGroup AddDeviceRequirementsForAssetBundleVariant(string name)
            {
                return new iOSDeviceRequirementGroup(name);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool CheckAssetBundleVariantHasDeviceRequirements(string name);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            internal static extern string[] GetAssetBundleVariantsWithDeviceRequirements();
            internal static iOSDeviceRequirementGroup GetDeviceRequirementsForAssetBundleVariant(string name)
            {
                if (!CheckAssetBundleVariantHasDeviceRequirements(name))
                {
                    return null;
                }
                return new iOSDeviceRequirementGroup(name);
            }

            internal static void RemoveDeviceRequirementsForAssetBundleVariant(string name)
            {
                iOSDeviceRequirementGroup deviceRequirementsForAssetBundleVariant = GetDeviceRequirementsForAssetBundleVariant(name);
                for (int i = 0; i < deviceRequirementsForAssetBundleVariant.count; i++)
                {
                    deviceRequirementsForAssetBundleVariant.RemoveAt(0);
                }
            }

            public static bool allowHTTPDownload { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static iOSAppInBackgroundBehavior appInBackgroundBehavior { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string applicationDisplayName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string buildNumber { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("use appInBackgroundBehavior")]
            public static bool exitOnSuspend
            {
                get
                {
                    return (appInBackgroundBehavior == iOSAppInBackgroundBehavior.Exit);
                }
                set
                {
                    appInBackgroundBehavior = iOSAppInBackgroundBehavior.Exit;
                }
            }

            public static bool overrideIPodMusic { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool prerenderedIcon { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool requiresFullScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool requiresPersistentWiFi { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static ScriptCallOptimizationLevel scriptCallOptimization { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static iOSSdkVersion sdkVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static iOSShowActivityIndicatorOnLoading showActivityIndicatorOnLoading { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static iOSStatusBarStyle statusBarStyle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static iOSTargetDevice targetDevice { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static iOSTargetOSVersion targetOSVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("use Screen.SetResolution at runtime", true)]
            public static iOSTargetResolution targetResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool useOnDemandResources { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        }

        public sealed class Nintendo3DS
        {
            public static string applicationId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool compressStaticMem { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool disableDepthAndStencilBuffers { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool disableStereoscopicView { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool enableSharedListOpt { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool enableVSync { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string extSaveDataNumber { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static LogoStyle logoStyle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static MediaSize mediaSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string productCode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static Region region { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int stackSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static TargetPlatform targetPlatform { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string title { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool useExtSaveData { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public enum LogoStyle
            {
                Nintendo,
                Distributed,
                iQue,
                Licensed
            }

            public enum MediaSize
            {
                _128MB,
                _256MB,
                _512MB,
                _1GB,
                _2GB
            }

            public enum Region
            {
                All = 7,
                America = 2,
                China = 4,
                Europe = 3,
                Japan = 1,
                Korea = 5,
                Taiwan = 6
            }

            public enum TargetPlatform
            {
                NewNintendo3DS = 2,
                Nintendo3DS = 1
            }
        }

        public sealed class PS3
        {
            public static string backgroundPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int bootCheckMaxSaveGameSizeKB { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool DisableDolbyEncoding { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string dlcConfigPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool EnableMoveSupport { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool EnableVerboseMemoryStats { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int npAgeRating { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npCommunicationPassphrase { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npTrophyCommId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npTrophyCommSig { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npTrophyPackagePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static Texture2D ps3SplashScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int saveGameSlots { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string soundPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string thumbnailPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string titleConfigPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool trialMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool UseSPUForUmbra { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int videoMemoryForAudio { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int videoMemoryForVertexBuffers { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        }

        public sealed class PS4
        {
            public static int applicationParameter1 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int applicationParameter2 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int applicationParameter3 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int applicationParameter4 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int appType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string appVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool attrib3DSupport { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int attribCpuUsage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool attribMoveSupport { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool attribShareSupport { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool attribUserManagement { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int audio3dVirtualSpeakerCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string BackgroundImagePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string BGMPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PS4AppCategory category { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string contentID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int downloadDataSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PS4EnterButtonAssignment enterButtonAssignment { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int garlicHeapSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string[] includedModules { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string masterVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string monoEnv { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int npAgeRating { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string NPtitleDatPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npTitleSecret { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npTrophyPackPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string paramSfxPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int parentalLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string passcode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string PatchChangeinfoPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string PatchLatestPkgPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string PatchPkgPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool playerPrefsSupport { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool pnFriends { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool pnGameCustomData { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool pnPresence { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool pnSessions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string PrivacyGuardImagePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string PronunciationSIGPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string PronunciationXMLPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PS4RemotePlayKeyAssignment remotePlayKeyAssignment { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string remotePlayKeyMappingDir { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool reprojectionSupport { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string SaveDataImagePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string SdkOverride { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string ShareFilePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string ShareOverlayImagePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int socialScreenEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string StartupImagePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool useAudio3dBackend { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int videoOutPixelFormat { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int videoOutResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public enum PS4AppCategory
            {
                Application,
                Patch
            }

            public enum PS4EnterButtonAssignment
            {
                CircleButton,
                CrossButton
            }

            public enum PS4RemotePlayKeyAssignment
            {
                None = -1,
                PatternA = 0,
                PatternB = 1,
                PatternC = 2,
                PatternD = 3,
                PatternE = 4,
                PatternF = 5,
                PatternG = 6,
                PatternH = 7
            }
        }

        public sealed class PSM
        {
        }

        public sealed class PSVita
        {
            public static bool acquireBGM { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool AllowTwitterDialog { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string appVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PSVitaAppCategory category { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string contentID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PSVitaDRMType drmType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PSVitaEnterButtonAssignment enterButtonAssignment { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool healthWarning { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool infoBarColor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool infoBarOnStartup { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string keystoneFile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string liveAreaBackroundPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string liveAreaGatePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string liveAreaPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string liveAreaTrialPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string manualPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string masterVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int mediaCapacity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PSVitaMemoryExpansionMode memoryExpansionMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int npAgeRating { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npCommsPassphrase { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npCommsSig { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npCommunicationsID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool npSupportGBMorGJP { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npTitleDatPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string npTrophyPackPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string packagePassword { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string paramSfxPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int parentalLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string patchChangeInfoPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string patchOriginalPackage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PSVitaPowerMode powerMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int saveDataQuota { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string shortTitle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int storageType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PSVitaTvBootMode tvBootMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool tvDisableEmu { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool upgradable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool useLibLocation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public enum PSVitaAppCategory
            {
                Application,
                ApplicationPatch
            }

            public enum PSVitaDRMType
            {
                PaidFor,
                Free
            }

            public enum PSVitaEnterButtonAssignment
            {
                Default,
                CircleButton,
                CrossButton
            }

            public enum PSVitaMemoryExpansionMode
            {
                None,
                ExpandBy29MB,
                ExpandBy77MB,
                ExpandBy109MB
            }

            public enum PSVitaPowerMode
            {
                ModeA,
                ModeB,
                ModeC
            }

            public enum PSVitaTvBootMode
            {
                Default,
                PSVitaBootablePSVitaTvBootable,
                PSVitaBootablePSVitaTvNotBootable
            }
        }

        public sealed class SamsungTV
        {
            public static string deviceAddress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string productAuthor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string productAuthorEmail { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static SamsungTVProductCategories productCategory { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string productDescription { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string productLink { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public enum SamsungTVProductCategories
            {
                Games,
                Videos,
                Sports,
                Lifestyle,
                Information,
                Education,
                Kids
            }
        }

        public sealed class Tizen
        {
            public static bool GetCapability(PlayerSettings.TizenCapability capability)
            {
                string str = InternalGetCapability(capability.ToString());
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                try
                {
                    return (bool) TypeDescriptor.GetConverter(typeof(bool)).ConvertFromString(str);
                }
                catch
                {
                    Debug.LogError("Failed to parse value  ('" + capability.ToString() + "," + str + "') to bool type.");
                    return false;
                }
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern string InternalGetCapability(string name);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void InternalSetCapability(string name, string value);
            public static void SetCapability(PlayerSettings.TizenCapability capability, bool value)
            {
                InternalSetCapability(capability.ToString(), value.ToString());
            }

            public static string productDescription { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string productURL { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string signingProfileName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        }

        public enum TizenCapability
        {
            Location,
            DataSharing,
            NetworkGet,
            WifiDirect,
            CallHistoryRead,
            Power,
            ContactWrite,
            MessageWrite,
            ContentWrite,
            Push,
            AccountRead,
            ExternalStorage,
            Recorder,
            PackageManagerInfo,
            NFCCardEmulation,
            CalendarWrite,
            WindowPrioritySet,
            VolumeSet,
            CallHistoryWrite,
            AlarmSet,
            Call,
            Email,
            ContactRead,
            Shortcut,
            KeyManager,
            LED,
            NetworkProfile,
            AlarmGet,
            Display,
            CalendarRead,
            NFC,
            AccountWrite,
            Bluetooth,
            Notification,
            NetworkSet,
            ExternalStorageAppData,
            Download,
            Telephony,
            MessageRead
        }

        public sealed class WiiU
        {
            public static int accountBossSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int accountSaveSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string[] addOnUniqueIDs { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool allowScreenCapture { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int commonBossSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int commonSaveSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int controllerCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int gamePadMSAA { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static Texture2D gamePadStartupScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static string groupID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string joinGameId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string joinGameModeMask { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int loaderThreadStackSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int mainThreadStackSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string olvAccessKey { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string profilerLibraryPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool supportsBalanceBoard { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool supportsClassicController { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool supportsMotionPlus { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool supportsNunchuk { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool supportsProController { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static int systemHeapSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string tinCode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string titleID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static WiiUTVResolution tvResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static Texture2D tvStartupScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        }

        public sealed class WSA
        {
            public static bool GetCapability(PlayerSettings.WSACapability capability)
            {
                string str = InternalGetCapability(capability.ToString());
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                try
                {
                    return (bool) TypeDescriptor.GetConverter(typeof(bool)).ConvertFromString(str);
                }
                catch
                {
                    Debug.LogError("Failed to parse value  ('" + capability.ToString() + "," + str + "') to bool type.");
                    return false;
                }
            }

            public static string GetVisualAssetsImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
            {
                ValidateWSAImageType(type);
                ValidateWSAImageScale(scale);
                return GetWSAImage(type, scale);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern string GetWSAImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_get_splashScreenBackgroundColorRaw(out Color value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_get_tileBackgroundColor(out Color value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_set_splashScreenBackgroundColorRaw(ref Color value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_set_tileBackgroundColor(ref Color value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern string InternalGetCapability(string name);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void InternalSetCapability(string name, string value);
            public static void SetCapability(PlayerSettings.WSACapability capability, bool value)
            {
                InternalSetCapability(capability.ToString(), value.ToString());
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern bool SetCertificate(string path, string password);
            public static void SetVisualAssetsImage(string image, PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
            {
                ValidateWSAImageType(type);
                ValidateWSAImageScale(scale);
                SetWSAImage(image, type, scale);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetWSAImage(string image, PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);
            internal static string ValidatePackageVersion(string value)
            {
                Regex regex = new Regex(@"^(\d+)\.(\d+)\.(\d+)\.(\d+)$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                if (regex.IsMatch(value))
                {
                    return value;
                }
                return "1.0.0.0";
            }

            private static void ValidateWSAImageScale(PlayerSettings.WSAImageScale scale)
            {
                PlayerSettings.WSAImageScale scale2 = scale;
                if (((((scale2 != PlayerSettings.WSAImageScale.Target16) && (scale2 != PlayerSettings.WSAImageScale.Target24)) && ((scale2 != PlayerSettings.WSAImageScale.Target48) && (scale2 != PlayerSettings.WSAImageScale._80))) && (((scale2 != PlayerSettings.WSAImageScale._100) && (scale2 != PlayerSettings.WSAImageScale._125)) && ((scale2 != PlayerSettings.WSAImageScale._140) && (scale2 != PlayerSettings.WSAImageScale._150)))) && ((((scale2 != PlayerSettings.WSAImageScale._180) && (scale2 != PlayerSettings.WSAImageScale._200)) && ((scale2 != PlayerSettings.WSAImageScale._240) && (scale2 != PlayerSettings.WSAImageScale.Target256))) && (scale2 != PlayerSettings.WSAImageScale._400)))
                {
                    throw new Exception("Unknown image scale: " + scale);
                }
            }

            private static void ValidateWSAImageType(PlayerSettings.WSAImageType type)
            {
                PlayerSettings.WSAImageType type2 = type;
                switch (type2)
                {
                    case PlayerSettings.WSAImageType.StoreTileLogo:
                    case PlayerSettings.WSAImageType.StoreTileWideLogo:
                    case PlayerSettings.WSAImageType.StoreTileSmallLogo:
                    case PlayerSettings.WSAImageType.StoreSmallTile:
                    case PlayerSettings.WSAImageType.StoreLargeTile:
                    case PlayerSettings.WSAImageType.PhoneAppIcon:
                    case PlayerSettings.WSAImageType.PhoneSmallTile:
                    case PlayerSettings.WSAImageType.PhoneMediumTile:
                    case PlayerSettings.WSAImageType.PhoneWideTile:
                    case PlayerSettings.WSAImageType.PhoneSplashScreen:
                    case PlayerSettings.WSAImageType.UWPSquare44x44Logo:
                    case PlayerSettings.WSAImageType.UWPSquare71x71Logo:
                    case PlayerSettings.WSAImageType.UWPSquare150x150Logo:
                    case PlayerSettings.WSAImageType.UWPSquare310x310Logo:
                    case PlayerSettings.WSAImageType.UWPWide310x150Logo:
                        return;
                }
                if ((type2 != PlayerSettings.WSAImageType.PackageLogo) && (type2 != PlayerSettings.WSAImageType.SplashScreenImage))
                {
                    throw new Exception("Unknown WSA image type: " + type);
                }
            }

            public static string applicationDescription { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string certificateIssuer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static DateTime? certificateNotAfter
            {
                get
                {
                    long certificateNotAfterRaw = PlayerSettings.WSA.certificateNotAfterRaw;
                    if (certificateNotAfterRaw != 0)
                    {
                        return new DateTime?(DateTime.FromFileTime(certificateNotAfterRaw));
                    }
                    return null;
                }
            }

            private static long certificateNotAfterRaw { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            internal static string certificatePassword { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static string certificatePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static string certificateSubject { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static string commandLineArgsFile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PlayerSettings.WSACompilationOverrides compilationOverrides { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PlayerSettings.WSADefaultTileSize defaultTileSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool enableIndependentInputSource { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool enableLowLatencyPresentationAPI { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool largeTileShowName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool mediumTileShowName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string packageLogo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string packageLogo140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string packageLogo180 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string packageLogo240 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string packageName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static Version packageVersion
            {
                get
                {
                    Version version;
                    try
                    {
                        version = new Version(ValidatePackageVersion(packageVersionRaw));
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(string.Format("{0}, the raw string was {1}", exception.Message, packageVersionRaw));
                    }
                    return version;
                }
                set
                {
                    packageVersionRaw = value.ToString();
                }
            }

            private static string packageVersionRaw { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneAppIcon { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneAppIcon140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneAppIcon240 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneMediumTile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneMediumTile140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneMediumTile240 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSmallTile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSmallTile140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSmallTile240 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSplashScreenImage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSplashScreenImageScale140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSplashScreenImageScale240 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneWideTile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneWideTile140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneWideTile240 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static Color? splashScreenBackgroundColor
            {
                get
                {
                    if (splashScreenUseBackgroundColor)
                    {
                        return new Color?(splashScreenBackgroundColorRaw);
                    }
                    return null;
                }
                set
                {
                    splashScreenUseBackgroundColor = value.HasValue;
                    if (value.HasValue)
                    {
                        splashScreenBackgroundColorRaw = value.Value;
                    }
                }
            }

            private static Color splashScreenBackgroundColorRaw
            {
                get
                {
                    Color color;
                    INTERNAL_get_splashScreenBackgroundColorRaw(out color);
                    return color;
                }
                set
                {
                    INTERNAL_set_splashScreenBackgroundColorRaw(ref value);
                }
            }

            private static bool splashScreenUseBackgroundColor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeLargeTile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeLargeTile140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeLargeTile180 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeLargeTile80 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSmallTile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSmallTile140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSmallTile180 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSmallTile80 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSplashScreenImage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSplashScreenImageScale140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSplashScreenImageScale180 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileLogo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileLogo140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileLogo180 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileLogo80 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileSmallLogo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileSmallLogo140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileSmallLogo180 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileSmallLogo80 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileWideLogo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileWideLogo140 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileWideLogo180 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileWideLogo80 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static Color tileBackgroundColor
            {
                get
                {
                    Color color;
                    INTERNAL_get_tileBackgroundColor(out color);
                    return color;
                }
                set
                {
                    INTERNAL_set_tileBackgroundColor(ref value);
                }
            }

            public static PlayerSettings.WSAApplicationForegroundText tileForegroundText { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string tileShortName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static PlayerSettings.WSAApplicationShowName tileShowName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool wideTileShowName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        }

        public enum WSAApplicationForegroundText
        {
            Dark = 2,
            Light = 1
        }

        public enum WSAApplicationShowName
        {
            NotSet,
            AllLogos,
            NoLogos,
            StandardLogoOnly,
            WideLogoOnly
        }

        public enum WSACapability
        {
            EnterpriseAuthentication,
            InternetClient,
            InternetClientServer,
            MusicLibrary,
            PicturesLibrary,
            PrivateNetworkClientServer,
            RemovableStorage,
            SharedUserCertificates,
            VideosLibrary,
            WebCam,
            Proximity,
            Microphone,
            Location,
            HumanInterfaceDevice,
            AllJoyn,
            BlockedChatMessages,
            Chat,
            CodeGeneration,
            Objects3D,
            PhoneCall,
            UserAccountInformation,
            VoipCall
        }

        public enum WSACompilationOverrides
        {
            None,
            UseNetCore,
            UseNetCorePartially
        }

        public enum WSADefaultTileSize
        {
            NotSet,
            Medium,
            Wide
        }

        public enum WSAImageScale
        {
            _100 = 100,
            _125 = 0x7d,
            _140 = 140,
            _150 = 150,
            _180 = 180,
            _200 = 200,
            _240 = 240,
            _400 = 400,
            _80 = 80,
            Target16 = 0x10,
            Target24 = 0x18,
            Target256 = 0x100,
            Target48 = 0x30
        }

        public enum WSAImageType
        {
            PackageLogo = 1,
            PhoneAppIcon = 0x15,
            PhoneMediumTile = 0x17,
            PhoneSmallTile = 0x16,
            PhoneSplashScreen = 0x19,
            PhoneWideTile = 0x18,
            SplashScreenImage = 2,
            StoreLargeTile = 15,
            StoreSmallTile = 14,
            StoreTileLogo = 11,
            StoreTileSmallLogo = 13,
            StoreTileWideLogo = 12,
            UWPSquare150x150Logo = 0x21,
            UWPSquare310x310Logo = 0x22,
            UWPSquare44x44Logo = 0x1f,
            UWPSquare71x71Logo = 0x20,
            UWPWide310x150Logo = 0x23
        }

        public sealed class XboxOne
        {
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern bool AddAllowedProductId(string id);
            public static bool GetCapability(string capability)
            {
                try
                {
                    bool flag = false;
                    PlayerSettings.GetPropertyOptionalBool(capability, ref flag, BuildTargetGroup.XboxOne);
                    return flag;
                }
                catch
                {
                    return false;
                }
            }

            public static int GetGameRating(string name)
            {
                try
                {
                    int num = 0;
                    PlayerSettings.GetPropertyOptionalInt(name, ref num, BuildTargetGroup.XboxOne);
                    return num;
                }
                catch
                {
                    return 0;
                }
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void GetSocketDefinition(string name, out string port, out int protocol, out int[] usages, out string templateName, out int sessionRequirment, out int[] deviceUsages);
            internal static bool GetSupportedLanguage(string language)
            {
                try
                {
                    bool flag = false;
                    PlayerSettings.GetPropertyOptionalBool(language, ref flag, BuildTargetGroup.XboxOne);
                    return flag;
                }
                catch
                {
                    return false;
                }
            }

            internal static void InitializeGameRating(string name, int value)
            {
                PlayerSettings.InitializePropertyInt(name, value, BuildTargetGroup.XboxOne);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void RemoveAllowedProductId(string id);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void RemoveSocketDefinition(string name);
            public static void SetCapability(string capability, bool value)
            {
                PlayerSettings.SetPropertyBool(capability, value, BuildTargetGroup.XboxOne);
            }

            public static void SetGameRating(string name, int value)
            {
                PlayerSettings.SetPropertyInt(name, value, BuildTargetGroup.XboxOne);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void SetSocketDefinition(string name, string port, int protocol, int[] usages, string templateName, int sessionRequirment, int[] deviceUsages);
            internal static void SetSupportedLanguage(string language, bool enabled)
            {
                PlayerSettings.SetPropertyBool(language, enabled, BuildTargetGroup.XboxOne);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            public static extern void UpdateAllowedProductId(int idx, string id);

            public static string[] AllowedProductIds { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static string AppManifestOverridePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string ContentId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string Description { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool DisableKinectGpuReservation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool EnablePIXSampling { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool EnableVariableGPU { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string GameOsOverridePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static bool IsContentPackage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static XboxOnePackageUpdateGranularity PackageUpdateGranularity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static XboxOneEncryptionLevel PackagingEncryption { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string PackagingOverridePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static uint PersistentLocalStorageSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string ProductId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string SandboxId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string SCID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string[] SocketNames { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static string TitleId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string UpdateKey { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

            public static string Version { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        }
    }
}

