namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor.Callbacks;
    using UnityEditorInternal;
    using UnityEngine;

    public sealed class PluginImporter : AssetImporter
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern PluginImporter[] GetAllImporters();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetCompatibleWithAnyPlatform();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetCompatibleWithEditor();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetCompatibleWithPlatform(string platformName);
        public bool GetCompatibleWithPlatform(BuildTarget platform)
        {
            return this.GetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetEditorData(string key);
        [DebuggerHidden]
        internal static IEnumerable<PluginDesc> GetExtensionPlugins(BuildTarget target)
        {
            return new <GetExtensionPlugins>c__Iterator0 { target = target, <$>target = target, $PC = -2 };
        }

        public static PluginImporter[] GetImporters(string platformName)
        {
            <GetImporters>c__AnonStorey15 storey = new <GetImporters>c__AnonStorey15 {
                platformName = platformName
            };
            return GetAllImporters().Where<PluginImporter>(new Func<PluginImporter, bool>(storey.<>m__1C)).ToArray<PluginImporter>();
        }

        public static PluginImporter[] GetImporters(BuildTarget platform)
        {
            return GetImporters(BuildPipeline.GetBuildTargetName(platform));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool GetIsPreloaded();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetPlatformData(string platformName, string key);
        public string GetPlatformData(BuildTarget platform, string key)
        {
            return this.GetPlatformData(BuildPipeline.GetBuildTargetName(platform), key);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetCompatibleWithAnyPlatform(bool enable);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetCompatibleWithEditor(bool enable);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetCompatibleWithPlatform(string platformName, bool enable);
        public void SetCompatibleWithPlatform(BuildTarget platform, bool enable)
        {
            this.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform), enable);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetEditorData(string key, string value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetIsPreloaded(bool isPreloaded);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPlatformData(string platformName, string key, string value);
        public void SetPlatformData(BuildTarget platform, string key, string value)
        {
            this.SetPlatformData(BuildPipeline.GetBuildTargetName(platform), key, value);
        }

        public bool isNativePlugin { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [CompilerGenerated]
        private sealed class <GetExtensionPlugins>c__Iterator0 : IDisposable, IEnumerator, IEnumerable<PluginDesc>, IEnumerator<PluginDesc>, IEnumerable
        {
            internal PluginDesc $current;
            internal int $PC;
            internal BuildTarget <$>target;
            internal IEnumerator<IEnumerable<PluginDesc>> <$s_159>__1;
            internal IEnumerator<PluginDesc> <$s_160>__3;
            internal IEnumerable<PluginDesc> <extensionPlugins>__2;
            internal PluginDesc <pluginDesc>__4;
            internal IEnumerable<IEnumerable<PluginDesc>> <pluginDescriptions>__0;
            internal BuildTarget target;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                            try
                            {
                            }
                            finally
                            {
                                if (this.<$s_160>__3 == null)
                                {
                                }
                                this.<$s_160>__3.Dispose();
                            }
                        }
                        finally
                        {
                            if (this.<$s_159>__1 == null)
                            {
                            }
                            this.<$s_159>__1.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                    {
                        object[] arguments = new object[] { this.target };
                        this.<pluginDescriptions>__0 = AttributeHelper.CallMethodsWithAttribute<IEnumerable<PluginDesc>>(typeof(RegisterPluginsAttribute), arguments);
                        this.<$s_159>__1 = this.<pluginDescriptions>__0.GetEnumerator();
                        num = 0xfffffffd;
                        break;
                    }
                    case 1:
                        break;

                    default:
                        goto Label_0135;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_0096;
                    }
                    while (this.<$s_159>__1.MoveNext())
                    {
                        this.<extensionPlugins>__2 = this.<$s_159>__1.Current;
                        this.<$s_160>__3 = this.<extensionPlugins>__2.GetEnumerator();
                        num = 0xfffffffd;
                    Label_0096:
                        try
                        {
                            while (this.<$s_160>__3.MoveNext())
                            {
                                this.<pluginDesc>__4 = this.<$s_160>__3.Current;
                                this.$current = this.<pluginDesc>__4;
                                this.$PC = 1;
                                flag = true;
                                return true;
                            }
                            continue;
                        }
                        finally
                        {
                            if (!flag)
                            {
                            }
                            if (this.<$s_160>__3 == null)
                            {
                            }
                            this.<$s_160>__3.Dispose();
                        }
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.<$s_159>__1 == null)
                    {
                    }
                    this.<$s_159>__1.Dispose();
                }
                this.$PC = -1;
            Label_0135:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<PluginDesc> IEnumerable<PluginDesc>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new PluginImporter.<GetExtensionPlugins>c__Iterator0 { target = this.<$>target };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<UnityEditorInternal.PluginDesc>.GetEnumerator();
            }

            PluginDesc IEnumerator<PluginDesc>.Current
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

        [CompilerGenerated]
        private sealed class <GetImporters>c__AnonStorey15
        {
            internal string platformName;

            internal bool <>m__1C(PluginImporter imp)
            {
                return ((imp.GetCompatibleWithPlatform(this.platformName) || imp.GetCompatibleWithAnyPlatform()) && !string.IsNullOrEmpty(imp.assetPath));
            }
        }
    }
}

