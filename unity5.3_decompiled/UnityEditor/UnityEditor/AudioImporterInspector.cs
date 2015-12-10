namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(AudioImporter))]
    internal class AudioImporterInspector : AssetImporterInspector
    {
        [CompilerGenerated]
        private static Converter<AudioCompressionFormat, string> <>f__am$cache8;
        [CompilerGenerated]
        private static Converter<AudioCompressionFormat, int> <>f__am$cache9;
        public SerializedProperty m_CompSize;
        private SampleSettingProperties m_DefaultSampleSettings;
        public SerializedProperty m_ForceToMono;
        public SerializedProperty m_LoadInBackground;
        public SerializedProperty m_Normalize;
        public SerializedProperty m_OrigSize;
        public SerializedProperty m_PreloadAudioData;
        private Dictionary<BuildTargetGroup, SampleSettingProperties> m_SampleSettingOverrides;

        internal override void Apply()
        {
            base.Apply();
            this.SyncSettingsToBackend();
        }

        private bool CompressionFormatHasQuality(AudioCompressionFormat format)
        {
            switch (format)
            {
                case AudioCompressionFormat.Vorbis:
                case AudioCompressionFormat.MP3:
                case AudioCompressionFormat.XMA:
                case AudioCompressionFormat.AAC:
                    return true;
            }
            return false;
        }

        public bool CurrentPlatformHasAutoTranslatedCompression()
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            IEnumerator<AudioImporter> enumerator = this.GetAllAudioImporterTargets().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AudioImporter current = enumerator.Current;
                    AudioCompressionFormat compressionFormat = current.defaultSampleSettings.compressionFormat;
                    if (!current.Internal_ContainsSampleSettingsOverride(buildTargetGroup))
                    {
                        AudioCompressionFormat format2 = current.Internal_GetOverrideSampleSettings(buildTargetGroup).compressionFormat;
                        if (compressionFormat != format2)
                        {
                            return true;
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return false;
        }

        public bool CurrentSelectionContainsHardwareSounds()
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            IEnumerator<AudioImporter> enumerator = this.GetAllAudioImporterTargets().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AudioImporterSampleSettings settings = enumerator.Current.Internal_GetOverrideSampleSettings(buildTargetGroup);
                    if (this.IsHardwareSound(settings.compressionFormat))
                    {
                        return true;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return false;
        }

        [DebuggerHidden]
        private IEnumerable<AudioImporter> GetAllAudioImporterTargets()
        {
            return new <GetAllAudioImporterTargets>c__Iterator7 { <>f__this = this, $PC = -2 };
        }

        private AudioCompressionFormat[] GetFormatsForPlatform(BuildTargetGroup platform)
        {
            List<AudioCompressionFormat> list = new List<AudioCompressionFormat>();
            if (platform == BuildTargetGroup.WebGL)
            {
                list.Add(AudioCompressionFormat.AAC);
                return list.ToArray();
            }
            list.Add(AudioCompressionFormat.PCM);
            if (((platform != BuildTargetGroup.PS3) && (platform != BuildTargetGroup.PSM)) && (platform != BuildTargetGroup.PSP2))
            {
                list.Add(AudioCompressionFormat.Vorbis);
            }
            list.Add(AudioCompressionFormat.ADPCM);
            if ((((platform != BuildTargetGroup.Standalone) && (platform != BuildTargetGroup.WebPlayer)) && ((platform != BuildTargetGroup.Metro) && (platform != BuildTargetGroup.WiiU))) && (((platform != BuildTargetGroup.XboxOne) && (platform != BuildTargetGroup.XBOX360)) && (platform != BuildTargetGroup.Unknown)))
            {
                list.Add(AudioCompressionFormat.MP3);
            }
            if (platform == BuildTargetGroup.PSM)
            {
                list.Add(AudioCompressionFormat.VAG);
            }
            if (platform == BuildTargetGroup.PSP2)
            {
                list.Add(AudioCompressionFormat.HEVAG);
            }
            if (platform == BuildTargetGroup.WiiU)
            {
                list.Add(AudioCompressionFormat.GCADPCM);
            }
            if (platform == BuildTargetGroup.XboxOne)
            {
                list.Add(AudioCompressionFormat.XMA);
            }
            return list.ToArray();
        }

        private MultiValueStatus GetMultiValueStatus(BuildTargetGroup platform)
        {
            MultiValueStatus status;
            status.multiLoadType = false;
            status.multiSampleRateSetting = false;
            status.multiSampleRateOverride = false;
            status.multiCompressionFormat = false;
            status.multiQuality = false;
            status.multiConversionMode = false;
            if (this.GetAllAudioImporterTargets().Any<AudioImporter>())
            {
                AudioImporterSampleSettings defaultSampleSettings;
                AudioImporter importer = this.GetAllAudioImporterTargets().First<AudioImporter>();
                if (platform == BuildTargetGroup.Unknown)
                {
                    defaultSampleSettings = importer.defaultSampleSettings;
                }
                else
                {
                    defaultSampleSettings = importer.Internal_GetOverrideSampleSettings(platform);
                }
                AudioImporter[] second = new AudioImporter[] { importer };
                IEnumerator<AudioImporter> enumerator = this.GetAllAudioImporterTargets().Except<AudioImporter>(second).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        AudioImporterSampleSettings settings2;
                        AudioImporter current = enumerator.Current;
                        if (platform == BuildTargetGroup.Unknown)
                        {
                            settings2 = current.defaultSampleSettings;
                        }
                        else
                        {
                            settings2 = current.Internal_GetOverrideSampleSettings(platform);
                        }
                        status.multiLoadType |= defaultSampleSettings.loadType != settings2.loadType;
                        status.multiSampleRateSetting |= defaultSampleSettings.sampleRateSetting != settings2.sampleRateSetting;
                        status.multiSampleRateOverride |= defaultSampleSettings.sampleRateOverride != settings2.sampleRateOverride;
                        status.multiCompressionFormat |= defaultSampleSettings.compressionFormat != settings2.compressionFormat;
                        status.multiQuality |= defaultSampleSettings.quality != settings2.quality;
                        status.multiConversionMode |= defaultSampleSettings.conversionMode != settings2.conversionMode;
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return status;
        }

        private OverrideStatus GetOverrideStatus(BuildTargetGroup platform)
        {
            bool flag = false;
            bool flag2 = false;
            if (this.GetAllAudioImporterTargets().Any<AudioImporter>())
            {
                AudioImporter importer = this.GetAllAudioImporterTargets().First<AudioImporter>();
                flag2 = importer.Internal_ContainsSampleSettingsOverride(platform);
                AudioImporter[] second = new AudioImporter[] { importer };
                IEnumerator<AudioImporter> enumerator = this.GetAllAudioImporterTargets().Except<AudioImporter>(second).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        bool flag3 = enumerator.Current.Internal_ContainsSampleSettingsOverride(platform);
                        if (flag3 != flag2)
                        {
                            flag |= true;
                        }
                        flag2 |= flag3;
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            if (!flag2)
            {
                return OverrideStatus.NoOverrides;
            }
            if (flag)
            {
                return OverrideStatus.MixedOverrides;
            }
            return OverrideStatus.AllOverrides;
        }

        internal override bool HasModified()
        {
            if (base.HasModified())
            {
                return true;
            }
            if (this.m_DefaultSampleSettings.HasModified())
            {
                return true;
            }
            foreach (SampleSettingProperties properties in this.m_SampleSettingOverrides.Values)
            {
                if (properties.HasModified())
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsHardwareSound(AudioCompressionFormat format)
        {
            switch (format)
            {
                case AudioCompressionFormat.VAG:
                case AudioCompressionFormat.HEVAG:
                case AudioCompressionFormat.XMA:
                case AudioCompressionFormat.GCADPCM:
                    return true;
            }
            return false;
        }

        private void OnAudioImporterGUI(bool selectionContainsTrackerFile)
        {
            if (!selectionContainsTrackerFile)
            {
                EditorGUILayout.PropertyField(this.m_ForceToMono, new GUILayoutOption[0]);
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!this.m_ForceToMono.boolValue);
                EditorGUILayout.PropertyField(this.m_Normalize, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(this.m_LoadInBackground, new GUILayoutOption[0]);
            }
            EditorGUILayout.PropertyField(this.m_PreloadAudioData, new GUILayoutOption[0]);
            GUILayout.Space(10f);
            BuildPlayerWindow.BuildPlatform[] platforms = BuildPlayerWindow.GetValidPlatforms().ToArray();
            int index = EditorGUILayout.BeginPlatformGrouping(platforms, GUIContent.Temp("Default"));
            if (index == -1)
            {
                MultiValueStatus multiValueStatus = this.GetMultiValueStatus(BuildTargetGroup.Unknown);
                this.OnSampleSettingGUI(BuildTargetGroup.Unknown, multiValueStatus, selectionContainsTrackerFile, ref this.m_DefaultSampleSettings);
            }
            else
            {
                BuildTargetGroup targetGroup = platforms[index].targetGroup;
                SampleSettingProperties properties = this.m_SampleSettingOverrides[targetGroup];
                OverrideStatus overrideStatus = this.GetOverrideStatus(targetGroup);
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = (overrideStatus == OverrideStatus.MixedOverrides) && !properties.overrideIsForced;
                bool flag = (properties.overrideIsForced && properties.forcedOverrideState) || (!properties.overrideIsForced && (overrideStatus != OverrideStatus.NoOverrides));
                flag = EditorGUILayout.Toggle("Override for " + platforms[index].name, flag, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    properties.forcedOverrideState = flag;
                    properties.overrideIsForced = true;
                }
                MultiValueStatus status = this.GetMultiValueStatus(targetGroup);
                bool disabled = (!properties.overrideIsForced || !properties.forcedOverrideState) && (overrideStatus != OverrideStatus.AllOverrides);
                EditorGUI.BeginDisabledGroup(disabled);
                this.OnSampleSettingGUI(targetGroup, status, selectionContainsTrackerFile, ref properties);
                EditorGUI.EndDisabledGroup();
                this.m_SampleSettingOverrides[targetGroup] = properties;
            }
            EditorGUILayout.EndPlatformGrouping();
        }

        public void OnEnable()
        {
            this.m_ForceToMono = base.serializedObject.FindProperty("m_ForceToMono");
            this.m_Normalize = base.serializedObject.FindProperty("m_Normalize");
            this.m_PreloadAudioData = base.serializedObject.FindProperty("m_PreloadAudioData");
            this.m_LoadInBackground = base.serializedObject.FindProperty("m_LoadInBackground");
            this.m_OrigSize = base.serializedObject.FindProperty("m_PreviewData.m_OrigSize");
            this.m_CompSize = base.serializedObject.FindProperty("m_PreviewData.m_CompSize");
            this.ResetSettingsFromBackend();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.UpdateIfDirtyOrScript();
            bool selectionContainsTrackerFile = false;
            IEnumerator<AudioImporter> enumerator = this.GetAllAudioImporterTargets().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AudioImporter current = enumerator.Current;
                    switch (FileUtil.GetPathExtension(current.assetPath).ToLowerInvariant())
                    {
                        case "mod":
                        case "it":
                        case "s3m":
                        case "xm":
                            selectionContainsTrackerFile = true;
                            goto Label_009F;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        Label_009F:
            this.OnAudioImporterGUI(selectionContainsTrackerFile);
            int bytes = 0;
            int num2 = 0;
            IEnumerator<AudioImporter> enumerator2 = this.GetAllAudioImporterTargets().GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    AudioImporter importer2 = enumerator2.Current;
                    bytes += importer2.origSize;
                    num2 += importer2.compSize;
                }
            }
            finally
            {
                if (enumerator2 == null)
                {
                }
                enumerator2.Dispose();
            }
            GUILayout.Space(10f);
            string[] textArray1 = new string[] { "Original Size: \t", EditorUtility.FormatBytes(bytes), "\nImported Size: \t", EditorUtility.FormatBytes(num2), "\nRatio: \t\t", ((100f * num2) / ((float) bytes)).ToString("0.00"), "%" };
            EditorGUILayout.HelpBox(string.Concat(textArray1), MessageType.Info);
            if (this.CurrentPlatformHasAutoTranslatedCompression())
            {
                GUILayout.Space(10f);
                EditorGUILayout.HelpBox("The selection contains different compression formats to the default settings for the current build platform.", MessageType.Info);
            }
            if (this.CurrentSelectionContainsHardwareSounds())
            {
                GUILayout.Space(10f);
                EditorGUILayout.HelpBox("The selection contains sounds that are decompressed in hardware. Advanced mixing is not available for these sounds.", MessageType.Info);
            }
            base.ApplyRevertGUI();
        }

        private void OnSampleSettingGUI(BuildTargetGroup platform, MultiValueStatus status, bool selectionContainsTrackerFile, ref SampleSettingProperties properties)
        {
            EditorGUI.showMixedValue = status.multiLoadType && !properties.loadTypeChanged;
            EditorGUI.BeginChangeCheck();
            AudioClipLoadType type = (AudioClipLoadType) EditorGUILayout.EnumPopup("Load Type", properties.settings.loadType, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                properties.settings.loadType = type;
                properties.loadTypeChanged = true;
            }
            if (!selectionContainsTrackerFile)
            {
                AudioCompressionFormat[] formatsForPlatform = this.GetFormatsForPlatform(platform);
                EditorGUI.showMixedValue = status.multiCompressionFormat && !properties.compressionFormatChanged;
                EditorGUI.BeginChangeCheck();
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = value => value.ToString();
                }
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = value => (int) value;
                }
                AudioCompressionFormat format = (AudioCompressionFormat) EditorGUILayout.IntPopup("Compression Format", (int) properties.settings.compressionFormat, Array.ConvertAll<AudioCompressionFormat, string>(formatsForPlatform, <>f__am$cache8), Array.ConvertAll<AudioCompressionFormat, int>(formatsForPlatform, <>f__am$cache9), new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    properties.settings.compressionFormat = format;
                    properties.compressionFormatChanged = true;
                }
                if (this.CompressionFormatHasQuality(properties.settings.compressionFormat))
                {
                    EditorGUI.showMixedValue = status.multiQuality && !properties.qualityChanged;
                    EditorGUI.BeginChangeCheck();
                    int num = EditorGUILayout.IntSlider("Quality", (int) Mathf.Clamp((float) (properties.settings.quality * 100f), (float) 1f, (float) 100f), 1, 100, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        properties.settings.quality = 0.01f * num;
                        properties.qualityChanged = true;
                    }
                }
                EditorGUI.showMixedValue = status.multiSampleRateSetting && !properties.sampleRateSettingChanged;
                EditorGUI.BeginChangeCheck();
                AudioSampleRateSetting setting = (AudioSampleRateSetting) EditorGUILayout.EnumPopup("Sample Rate Setting", properties.settings.sampleRateSetting, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    properties.settings.sampleRateSetting = setting;
                    properties.sampleRateSettingChanged = true;
                }
                if (properties.settings.sampleRateSetting == AudioSampleRateSetting.OverrideSampleRate)
                {
                    EditorGUI.showMixedValue = status.multiSampleRateOverride && !properties.sampleRateOverrideChanged;
                    EditorGUI.BeginChangeCheck();
                    int num2 = EditorGUILayout.IntPopup("Sample Rate", (int) properties.settings.sampleRateOverride, Styles.kSampleRateStrings, Styles.kSampleRateValues, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        properties.settings.sampleRateOverride = (uint) num2;
                        properties.sampleRateOverrideChanged = true;
                    }
                }
                EditorGUI.showMixedValue = false;
            }
        }

        private bool ResetSettingsFromBackend()
        {
            if (this.GetAllAudioImporterTargets().Any<AudioImporter>())
            {
                AudioImporter importer = this.GetAllAudioImporterTargets().First<AudioImporter>();
                this.m_DefaultSampleSettings.settings = importer.defaultSampleSettings;
                this.m_DefaultSampleSettings.ClearChangedFlags();
                this.m_SampleSettingOverrides = new Dictionary<BuildTargetGroup, SampleSettingProperties>();
                foreach (BuildPlayerWindow.BuildPlatform platform in BuildPlayerWindow.GetValidPlatforms())
                {
                    BuildTargetGroup targetGroup = platform.targetGroup;
                    IEnumerator<AudioImporter> enumerator = this.GetAllAudioImporterTargets().GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            AudioImporter current = enumerator.Current;
                            if (current.Internal_ContainsSampleSettingsOverride(targetGroup))
                            {
                                SampleSettingProperties properties = new SampleSettingProperties {
                                    settings = current.Internal_GetOverrideSampleSettings(targetGroup)
                                };
                                this.m_SampleSettingOverrides[targetGroup] = properties;
                                goto Label_00D8;
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                Label_00D8:
                    if (!this.m_SampleSettingOverrides.ContainsKey(targetGroup))
                    {
                        SampleSettingProperties properties2 = new SampleSettingProperties {
                            settings = importer.Internal_GetOverrideSampleSettings(targetGroup)
                        };
                        this.m_SampleSettingOverrides[targetGroup] = properties2;
                    }
                }
            }
            return true;
        }

        internal override void ResetValues()
        {
            base.ResetValues();
            this.ResetSettingsFromBackend();
        }

        private bool SyncSettingsToBackend()
        {
            BuildPlayerWindow.BuildPlatform[] platformArray = BuildPlayerWindow.GetValidPlatforms().ToArray();
            IEnumerator<AudioImporter> enumerator = this.GetAllAudioImporterTargets().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AudioImporter current = enumerator.Current;
                    AudioImporterSampleSettings defaultSampleSettings = current.defaultSampleSettings;
                    if (this.m_DefaultSampleSettings.loadTypeChanged)
                    {
                        defaultSampleSettings.loadType = this.m_DefaultSampleSettings.settings.loadType;
                    }
                    if (this.m_DefaultSampleSettings.sampleRateSettingChanged)
                    {
                        defaultSampleSettings.sampleRateSetting = this.m_DefaultSampleSettings.settings.sampleRateSetting;
                    }
                    if (this.m_DefaultSampleSettings.sampleRateOverrideChanged)
                    {
                        defaultSampleSettings.sampleRateOverride = this.m_DefaultSampleSettings.settings.sampleRateOverride;
                    }
                    if (this.m_DefaultSampleSettings.compressionFormatChanged)
                    {
                        defaultSampleSettings.compressionFormat = this.m_DefaultSampleSettings.settings.compressionFormat;
                    }
                    if (this.m_DefaultSampleSettings.qualityChanged)
                    {
                        defaultSampleSettings.quality = this.m_DefaultSampleSettings.settings.quality;
                    }
                    if (this.m_DefaultSampleSettings.conversionModeChanged)
                    {
                        defaultSampleSettings.conversionMode = this.m_DefaultSampleSettings.settings.conversionMode;
                    }
                    current.defaultSampleSettings = defaultSampleSettings;
                    foreach (BuildPlayerWindow.BuildPlatform platform in platformArray)
                    {
                        BuildTargetGroup targetGroup = platform.targetGroup;
                        if (this.m_SampleSettingOverrides.ContainsKey(targetGroup))
                        {
                            SampleSettingProperties properties = this.m_SampleSettingOverrides[targetGroup];
                            if (properties.overrideIsForced && !properties.forcedOverrideState)
                            {
                                current.Internal_ClearSampleSettingOverride(targetGroup);
                            }
                            else if (current.Internal_ContainsSampleSettingsOverride(targetGroup) || (properties.overrideIsForced && properties.forcedOverrideState))
                            {
                                AudioImporterSampleSettings settings = current.Internal_GetOverrideSampleSettings(targetGroup);
                                if (properties.loadTypeChanged)
                                {
                                    settings.loadType = properties.settings.loadType;
                                }
                                if (properties.sampleRateSettingChanged)
                                {
                                    settings.sampleRateSetting = properties.settings.sampleRateSetting;
                                }
                                if (properties.sampleRateOverrideChanged)
                                {
                                    settings.sampleRateOverride = properties.settings.sampleRateOverride;
                                }
                                if (properties.compressionFormatChanged)
                                {
                                    settings.compressionFormat = properties.settings.compressionFormat;
                                }
                                if (properties.qualityChanged)
                                {
                                    settings.quality = properties.settings.quality;
                                }
                                if (properties.conversionModeChanged)
                                {
                                    settings.conversionMode = properties.settings.conversionMode;
                                }
                                current.Internal_SetOverrideSampleSettings(targetGroup, settings);
                            }
                            this.m_SampleSettingOverrides[targetGroup] = properties;
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            this.m_DefaultSampleSettings.ClearChangedFlags();
            foreach (BuildPlayerWindow.BuildPlatform platform2 in platformArray)
            {
                BuildTargetGroup key = platform2.targetGroup;
                if (this.m_SampleSettingOverrides.ContainsKey(key))
                {
                    SampleSettingProperties properties2 = this.m_SampleSettingOverrides[key];
                    properties2.ClearChangedFlags();
                    this.m_SampleSettingOverrides[key] = properties2;
                }
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <GetAllAudioImporterTargets>c__Iterator7 : IDisposable, IEnumerator, IEnumerable, IEnumerable<AudioImporter>, IEnumerator<AudioImporter>
        {
            internal AudioImporter $current;
            internal int $PC;
            internal Object[] <$s_1146>__0;
            internal int <$s_1147>__1;
            internal AudioImporterInspector <>f__this;
            internal AudioImporter <audioImporter>__3;
            internal Object <importer>__2;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<$s_1146>__0 = this.<>f__this.targets;
                        this.<$s_1147>__1 = 0;
                        goto Label_0099;

                    case 1:
                        break;

                    default:
                        goto Label_00B3;
                }
            Label_008B:
                this.<$s_1147>__1++;
            Label_0099:
                if (this.<$s_1147>__1 < this.<$s_1146>__0.Length)
                {
                    this.<importer>__2 = this.<$s_1146>__0[this.<$s_1147>__1];
                    this.<audioImporter>__3 = this.<importer>__2 as AudioImporter;
                    if (this.<audioImporter>__3 != null)
                    {
                        this.$current = this.<audioImporter>__3;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_008B;
                }
                this.$PC = -1;
            Label_00B3:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<AudioImporter> IEnumerable<AudioImporter>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AudioImporterInspector.<GetAllAudioImporterTargets>c__Iterator7 { <>f__this = this.<>f__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<UnityEditor.AudioImporter>.GetEnumerator();
            }

            AudioImporter IEnumerator<AudioImporter>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MultiValueStatus
        {
            public bool multiLoadType;
            public bool multiSampleRateSetting;
            public bool multiSampleRateOverride;
            public bool multiCompressionFormat;
            public bool multiQuality;
            public bool multiConversionMode;
        }

        private enum OverrideStatus
        {
            NoOverrides,
            MixedOverrides,
            AllOverrides
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SampleSettingProperties
        {
            public AudioImporterSampleSettings settings;
            public bool forcedOverrideState;
            public bool overrideIsForced;
            public bool loadTypeChanged;
            public bool sampleRateSettingChanged;
            public bool sampleRateOverrideChanged;
            public bool compressionFormatChanged;
            public bool qualityChanged;
            public bool conversionModeChanged;
            public bool HasModified()
            {
                return ((((this.overrideIsForced || this.loadTypeChanged) || (this.sampleRateSettingChanged || this.sampleRateOverrideChanged)) || (this.compressionFormatChanged || this.qualityChanged)) || this.conversionModeChanged);
            }

            public void ClearChangedFlags()
            {
                this.forcedOverrideState = false;
                this.overrideIsForced = false;
                this.loadTypeChanged = false;
                this.sampleRateSettingChanged = false;
                this.sampleRateOverrideChanged = false;
                this.compressionFormatChanged = false;
                this.qualityChanged = false;
                this.conversionModeChanged = false;
            }
        }

        private static class Styles
        {
            public static readonly string[] kSampleRateStrings = new string[] { "8,000 Hz", "11,025 Hz", "22,050 Hz", "44,100 Hz", "48,000 Hz", "96,000 Hz", "192,000 Hz" };
            public static readonly int[] kSampleRateValues = new int[] { 0x1f40, 0x2b11, 0x5622, 0xac44, 0xbb80, 0x17700, 0x2ee00 };
        }
    }
}

