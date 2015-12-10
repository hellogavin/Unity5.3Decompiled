namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class AudioImporter : AssetImporter
    {
        public bool ClearSampleSettingOverride(string platform)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (buildTargetGroupByName == BuildTargetGroup.Unknown)
            {
                Debug.LogError("Unknown platform passed to AudioImporter.ClearSampleSettingOverride (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS3', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', 'WSA' or 'BlackBerry'");
                return false;
            }
            return this.Internal_ClearSampleSettingOverride(buildTargetGroupByName);
        }

        public bool ContainsSampleSettingsOverride(string platform)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (buildTargetGroupByName == BuildTargetGroup.Unknown)
            {
                Debug.LogError("Unknown platform passed to AudioImporter.ContainsSampleSettingsOverride (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS3', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', 'WSA' or 'BlackBerry'");
                return false;
            }
            return this.Internal_ContainsSampleSettingsOverride(buildTargetGroupByName);
        }

        public AudioImporterSampleSettings GetOverrideSampleSettings(string platform)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (buildTargetGroupByName == BuildTargetGroup.Unknown)
            {
                Debug.LogError("Unknown platform passed to AudioImporter.GetOverrideSampleSettings (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS3', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', 'WSA' or 'BlackBerry'");
                return this.defaultSampleSettings;
            }
            return this.Internal_GetOverrideSampleSettings(buildTargetGroupByName);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool Internal_ClearSampleSettingOverride(BuildTargetGroup platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool Internal_ContainsSampleSettingsOverride(BuildTargetGroup platformGroup);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool Internal_GetLoadInBackground();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern AudioImporterSampleSettings Internal_GetOverrideSampleSettings(BuildTargetGroup platformGroup);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool Internal_GetPreloadAudioData();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetLoadInBackground(bool flag);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool Internal_SetOverrideSampleSettings(BuildTargetGroup platformGroup, AudioImporterSampleSettings settings);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetPreloadAudioData(bool flag);
        [Obsolete("AudioImporter.maxBitrate is deprecated.", true)]
        internal int maxBitrate(AudioType type)
        {
            return 0;
        }

        [Obsolete("AudioImporter.minBitrate is deprecated.", true)]
        internal int minBitrate(AudioType type)
        {
            return 0;
        }

        public bool SetOverrideSampleSettings(string platform, AudioImporterSampleSettings settings)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (buildTargetGroupByName == BuildTargetGroup.Unknown)
            {
                Debug.LogError("Unknown platform passed to AudioImporter.SetOverrideSampleSettings (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS3', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', 'WSA' or 'BlackBerry'");
                return false;
            }
            return this.Internal_SetOverrideSampleSettings(buildTargetGroupByName, settings);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("AudioImporter.updateOrigData is deprecated.", true)]
        internal extern void updateOrigData();

        [Obsolete("Setting and getting import channels is not used anymore (use forceToMono instead)", true)]
        public AudioImporterChannels channels
        {
            get
            {
                return AudioImporterChannels.Automatic;
            }
            set
            {
            }
        }

        [Obsolete("AudioImporter.compressionBitrate is no longer supported", true)]
        public int compressionBitrate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal int compSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Setting/Getting decompressOnLoad is deprecated. Use AudioImporterSampleSettings.loadType instead.")]
        private bool decompressOnLoad
        {
            get
            {
                return (this.defaultSampleSettings.loadType == AudioClipLoadType.DecompressOnLoad);
            }
            set
            {
                AudioImporterSampleSettings defaultSampleSettings = this.defaultSampleSettings;
                defaultSampleSettings.loadType = !value ? AudioClipLoadType.CompressedInMemory : AudioClipLoadType.DecompressOnLoad;
                this.defaultSampleSettings = defaultSampleSettings;
            }
        }

        [Obsolete("AudioImporter.defaultBitrate is deprecated.", true)]
        internal int defaultBitrate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public AudioImporterSampleSettings defaultSampleSettings { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("AudioImporter.durationMS is deprecated.", true)]
        internal int durationMS { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool forceToMono { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Setting and getting the compression format is not used anymore (use compressionFormat in defaultSampleSettings instead). Source audio file is assumed to be PCM Wav.")]
        private AudioImporterFormat format
        {
            get
            {
                return ((this.defaultSampleSettings.compressionFormat != AudioCompressionFormat.PCM) ? AudioImporterFormat.Compressed : AudioImporterFormat.Native);
            }
            set
            {
                AudioImporterSampleSettings defaultSampleSettings = this.defaultSampleSettings;
                defaultSampleSettings.compressionFormat = (value != AudioImporterFormat.Native) ? AudioCompressionFormat.Vorbis : AudioCompressionFormat.PCM;
                this.defaultSampleSettings = defaultSampleSettings;
            }
        }

        [Obsolete("AudioImporter.frequency is deprecated.", true)]
        internal int frequency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("AudioImporter.hardware is no longer supported. All mixing of audio is done by software and only some platforms use hardware acceleration to perform decoding.")]
        public bool hardware { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool loadInBackground
        {
            get
            {
                return this.Internal_GetLoadInBackground();
            }
            set
            {
                this.Internal_SetLoadInBackground(value);
            }
        }

        [Obsolete("AudioImporter.loopable is no longer supported. All audio assets encoded by Unity are by default loopable.")]
        public bool loopable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("AudioImporter.origChannelCount is deprecated.", true)]
        internal int origChannelCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("AudioImporter.origFileSize is deprecated.", true)]
        internal int origFileSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("AudioImporter.origIsCompressible is deprecated.", true)]
        internal bool origIsCompressible { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("AudioImporter.origIsMonoForcable is deprecated.", true)]
        internal bool origIsMonoForcable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal int origSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("AudioImporter.origType is deprecated.", true)]
        internal AudioType origType
        {
            get
            {
                return AudioType.UNKNOWN;
            }
        }

        public bool preloadAudioData
        {
            get
            {
                return this.Internal_GetPreloadAudioData();
            }
            set
            {
                this.Internal_SetPreloadAudioData(value);
            }
        }

        [Obsolete("AudioImporter.quality is no longer supported. Use AudioImporterSampleSettings.")]
        private float quality
        {
            get
            {
                return this.defaultSampleSettings.quality;
            }
            set
            {
                AudioImporterSampleSettings defaultSampleSettings = this.defaultSampleSettings;
                defaultSampleSettings.quality = value;
                this.defaultSampleSettings = defaultSampleSettings;
            }
        }

        [Obsolete("AudioImporter.threeD is no longer supported")]
        public bool threeD { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

