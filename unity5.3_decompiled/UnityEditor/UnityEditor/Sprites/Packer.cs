namespace UnityEditor.Sprites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class Packer
    {
        [CompilerGenerated]
        private static Func<Type, string> <>f__am$cache4;
        public static string kDefaultPolicy = typeof(DefaultPackerPolicy).Name;
        private static string[] m_policies = null;
        private static Dictionary<string, Type> m_policyTypeCache = null;
        private static string m_selectedPolicy = null;

        internal static void ExecuteSelectedPolicy(BuildTarget target, int[] textureImporterInstanceIDs)
        {
            RegenerateList();
            Type type = m_policyTypeCache[m_selectedPolicy];
            (Activator.CreateInstance(type) as IPackerPolicy).OnGroupAtlases(target, new PackerJob(), textureImporterInstanceIDs);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D[] GetAlphaTexturesForAtlas(string atlasName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void GetAtlasDataForSprite(Sprite sprite, out string atlasName, [Writable] out Texture2D atlasTexture);
        internal static string GetSelectedPolicyId()
        {
            RegenerateList();
            Type type = m_policyTypeCache[m_selectedPolicy];
            IPackerPolicy policy = Activator.CreateInstance(type) as IPackerPolicy;
            return string.Format("{0}::{1}", type.AssemblyQualifiedName, policy.GetVersion());
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D[] GetTexturesForAtlas(string atlasName);
        [ExcludeFromDocs]
        public static void RebuildAtlasCacheIfNeeded(BuildTarget target)
        {
            Execution normal = Execution.Normal;
            bool displayProgressBar = false;
            RebuildAtlasCacheIfNeeded(target, displayProgressBar, normal);
        }

        [ExcludeFromDocs]
        public static void RebuildAtlasCacheIfNeeded(BuildTarget target, bool displayProgressBar)
        {
            Execution normal = Execution.Normal;
            RebuildAtlasCacheIfNeeded(target, displayProgressBar, normal);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RebuildAtlasCacheIfNeeded(BuildTarget target, [DefaultValue("false")] bool displayProgressBar, [DefaultValue("Execution.Normal")] Execution execution);
        private static void RegenerateList()
        {
            if (m_policies == null)
            {
                List<Type> source = new List<Type>();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (typeof(IPackerPolicy).IsAssignableFrom(type) && (type != typeof(IPackerPolicy)))
                            {
                                source.Add(type);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.Log(string.Format("SpritePacker failed to get types from {0}. Error: {1}", assembly.FullName, exception.Message));
                    }
                }
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = t => t.Name;
                }
                m_policies = source.Select<Type, string>(<>f__am$cache4).ToArray<string>();
                m_policyTypeCache = new Dictionary<string, Type>();
                foreach (Type type2 in source)
                {
                    if (m_policyTypeCache.ContainsKey(type2.Name))
                    {
                        Type type3 = m_policyTypeCache[type2.Name];
                        Debug.LogError(string.Format("Duplicate Sprite Packer policies found: {0} and {1}. Please rename one.", type2.FullName, type3.FullName));
                    }
                    else
                    {
                        m_policyTypeCache[type2.Name] = type2;
                    }
                }
                m_selectedPolicy = !string.IsNullOrEmpty(PlayerSettings.spritePackerPolicy) ? PlayerSettings.spritePackerPolicy : kDefaultPolicy;
                if (!m_policies.Contains<string>(m_selectedPolicy))
                {
                    SetSelectedPolicy(kDefaultPolicy);
                }
            }
        }

        internal static void SaveUnappliedTextureImporterSettings()
        {
            foreach (InspectorWindow window in InspectorWindow.GetAllInspectorWindows())
            {
                foreach (Editor editor in window.GetTracker().activeEditors)
                {
                    TextureImporterInspector inspector = editor as TextureImporterInspector;
                    if ((inspector != null) && inspector.HasModified())
                    {
                        TextureImporter target = inspector.target as TextureImporter;
                        if (EditorUtility.DisplayDialog("Unapplied import settings", "Unapplied import settings for '" + target.assetPath + "'", "Apply", "Revert"))
                        {
                            inspector.ApplyAndImport();
                        }
                    }
                }
            }
        }

        private static void SetSelectedPolicy(string value)
        {
            m_selectedPolicy = value;
            PlayerSettings.spritePackerPolicy = m_selectedPolicy;
        }

        public static string[] atlasNames { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string[] Policies
        {
            get
            {
                RegenerateList();
                return m_policies;
            }
        }

        public static string SelectedPolicy
        {
            get
            {
                RegenerateList();
                return m_selectedPolicy;
            }
            set
            {
                RegenerateList();
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (!m_policies.Contains<string>(value))
                {
                    throw new ArgumentException("Specified policy {0} is not in the policy list.", value);
                }
                SetSelectedPolicy(value);
            }
        }

        public enum Execution
        {
            Normal,
            ForceRegroup
        }
    }
}

