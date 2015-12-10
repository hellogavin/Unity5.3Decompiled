namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class RendererEditorBase : Editor
    {
        protected Probes m_Probes;
        private SerializedProperty m_SortingLayerID;
        private GUIContent m_SortingLayerStyle = EditorGUIUtility.TextContent("Sorting Layer");
        private SerializedProperty m_SortingOrder;
        private GUIContent m_SortingOrderStyle = EditorGUIUtility.TextContent("Order in Layer");

        protected void InitializeProbeFields()
        {
            this.m_Probes = new Probes();
            this.m_Probes.Initialize(base.serializedObject);
        }

        public virtual void OnEnable()
        {
            this.m_SortingOrder = base.serializedObject.FindProperty("m_SortingOrder");
            this.m_SortingLayerID = base.serializedObject.FindProperty("m_SortingLayerID");
        }

        protected void RenderProbeFields()
        {
            this.m_Probes.OnGUI(base.targets, (Renderer) this.target, false);
        }

        protected void RenderSortingLayerFields()
        {
            EditorGUILayout.Space();
            EditorGUILayout.SortingLayerField(this.m_SortingLayerStyle, this.m_SortingLayerID, EditorStyles.popup, EditorStyles.label);
            EditorGUILayout.PropertyField(this.m_SortingOrder, this.m_SortingOrderStyle, new GUILayoutOption[0]);
        }

        internal class Probes
        {
            [CompilerGenerated]
            private static Func<string, string> <>f__am$cacheB;
            [CompilerGenerated]
            private static Func<string, string> <>f__am$cacheC;
            [CompilerGenerated]
            private static Func<string, GUIContent> <>f__am$cacheD;
            private List<ReflectionProbeBlendInfo> m_BlendInfo;
            private GUIContent m_DeferredNote = EditorGUIUtility.TextContent("In Deferred Shading, all objects receive shadows and get per-pixel reflection probes.");
            private SerializedProperty m_ProbeAnchor;
            private GUIContent m_ProbeAnchorStyle = EditorGUIUtility.TextContent("Anchor Override|If set, the Renderer will use this Transform's position to sample light probes and find the matching reflection probe.");
            private SerializedProperty m_ReceiveShadows;
            private SerializedProperty m_ReflectionProbeUsage;
            private string[] m_ReflectionProbeUsageNames;
            private GUIContent[] m_ReflectionProbeUsageOptions;
            private GUIContent m_ReflectionProbeUsageStyle = EditorGUIUtility.TextContent("Reflection Probes");
            private SerializedProperty m_UseLightProbes;
            private GUIContent m_UseLightProbesStyle = EditorGUIUtility.TextContent("Use Light Probes");

            public Probes()
            {
                if (<>f__am$cacheB == null)
                {
                    <>f__am$cacheB = new Func<string, string>(RendererEditorBase.Probes.<m_ReflectionProbeUsageNames>m__145);
                }
                this.m_ReflectionProbeUsageNames = Enum.GetNames(typeof(ReflectionProbeUsage)).Select<string, string>(<>f__am$cacheB).ToArray<string>();
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = new Func<string, string>(RendererEditorBase.Probes.<m_ReflectionProbeUsageOptions>m__146);
                }
                if (<>f__am$cacheD == null)
                {
                    <>f__am$cacheD = new Func<string, GUIContent>(RendererEditorBase.Probes.<m_ReflectionProbeUsageOptions>m__147);
                }
                this.m_ReflectionProbeUsageOptions = Enum.GetNames(typeof(ReflectionProbeUsage)).Select<string, string>(<>f__am$cacheC).ToArray<string>().Select<string, GUIContent>(<>f__am$cacheD).ToArray<GUIContent>();
                this.m_BlendInfo = new List<ReflectionProbeBlendInfo>();
            }

            [CompilerGenerated]
            private static string <m_ReflectionProbeUsageNames>m__145(string x)
            {
                return ObjectNames.NicifyVariableName(x);
            }

            [CompilerGenerated]
            private static string <m_ReflectionProbeUsageOptions>m__146(string x)
            {
                return ObjectNames.NicifyVariableName(x);
            }

            [CompilerGenerated]
            private static GUIContent <m_ReflectionProbeUsageOptions>m__147(string x)
            {
                return new GUIContent(x);
            }

            internal static string[] GetFieldsStringArray()
            {
                return new string[] { "m_UseLightProbes", "m_ReflectionProbeUsage", "m_ProbeAnchor" };
            }

            internal void Initialize(SerializedObject serializedObject)
            {
                this.Initialize(serializedObject, true);
            }

            internal void Initialize(SerializedObject serializedObject, bool initializeLightProbes)
            {
                if (initializeLightProbes)
                {
                    this.m_UseLightProbes = serializedObject.FindProperty("m_UseLightProbes");
                }
                this.m_ReflectionProbeUsage = serializedObject.FindProperty("m_ReflectionProbeUsage");
                this.m_ProbeAnchor = serializedObject.FindProperty("m_ProbeAnchor");
                this.m_ReceiveShadows = serializedObject.FindProperty("m_ReceiveShadows");
            }

            internal void OnGUI(Object[] targets, Renderer renderer, bool useMiniStyle)
            {
                bool disabled = SceneView.IsUsingDeferredRenderingPath();
                bool flag2 = disabled && (GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredReflections) != BuiltinShaderMode.Disabled);
                bool flag3 = false;
                if (targets != null)
                {
                    foreach (Object obj2 in targets)
                    {
                        if (LightmapEditorSettings.IsLightmappedOrDynamicLightmappedForRendering((Renderer) obj2))
                        {
                            flag3 = true;
                            break;
                        }
                    }
                }
                if (this.m_UseLightProbes != null)
                {
                    EditorGUI.BeginDisabledGroup(flag3);
                    if (!useMiniStyle)
                    {
                        EditorGUILayout.PropertyField(this.m_UseLightProbes, this.m_UseLightProbesStyle, new GUILayoutOption[0]);
                    }
                    else
                    {
                        ModuleUI.GUIToggle(this.m_UseLightProbesStyle, this.m_UseLightProbes);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.BeginDisabledGroup(disabled);
                if (!useMiniStyle)
                {
                    if (flag2)
                    {
                        EditorGUILayout.EnumPopup(this.m_ReflectionProbeUsageStyle, (this.m_ReflectionProbeUsage.intValue == 0) ? ReflectionProbeUsage.Off : ReflectionProbeUsage.Simple, new GUILayoutOption[0]);
                    }
                    else
                    {
                        EditorGUILayout.Popup(this.m_ReflectionProbeUsage, this.m_ReflectionProbeUsageOptions, this.m_ReflectionProbeUsageStyle, new GUILayoutOption[0]);
                    }
                }
                else if (flag2)
                {
                    ModuleUI.GUIPopup(this.m_ReflectionProbeUsageStyle, 3, this.m_ReflectionProbeUsageNames);
                }
                else
                {
                    ModuleUI.GUIPopup(this.m_ReflectionProbeUsageStyle, this.m_ReflectionProbeUsage, this.m_ReflectionProbeUsageNames);
                }
                EditorGUI.EndDisabledGroup();
                bool flag4 = !this.m_ReflectionProbeUsage.hasMultipleDifferentValues && (this.m_ReflectionProbeUsage.intValue != 0);
                bool flag5 = ((this.m_UseLightProbes != null) && !this.m_UseLightProbes.hasMultipleDifferentValues) && this.m_UseLightProbes.boolValue;
                bool flag6 = flag4 || flag5;
                if (flag6)
                {
                    if (!useMiniStyle)
                    {
                        EditorGUILayout.PropertyField(this.m_ProbeAnchor, this.m_ProbeAnchorStyle, new GUILayoutOption[0]);
                    }
                    else
                    {
                        ModuleUI.GUIObject(this.m_ProbeAnchorStyle, this.m_ProbeAnchor);
                    }
                    if (!flag2)
                    {
                        renderer.GetClosestReflectionProbes(this.m_BlendInfo);
                        ShowClosestReflectionProbes(this.m_BlendInfo);
                    }
                }
                bool flag7 = !this.m_ReceiveShadows.hasMultipleDifferentValues && this.m_ReceiveShadows.boolValue;
                if ((disabled && flag7) || (flag2 && flag6))
                {
                    EditorGUILayout.HelpBox(this.m_DeferredNote.text, MessageType.Info);
                }
            }

            internal static void ShowClosestReflectionProbes(List<ReflectionProbeBlendInfo> blendInfos)
            {
                float num = 20f;
                float num2 = 60f;
                EditorGUI.BeginDisabledGroup(true);
                for (int i = 0; i < blendInfos.Count; i++)
                {
                    Rect rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect((float) 0f, (float) 16f));
                    float num4 = (rect.width - num) - num2;
                    Rect position = rect;
                    position.width = num;
                    GUI.Label(position, "#" + i, EditorStyles.miniLabel);
                    position.x += position.width;
                    position.width = num4;
                    ReflectionProbeBlendInfo info = blendInfos[i];
                    EditorGUI.ObjectField(position, info.probe, typeof(ReflectionProbe), true);
                    position.x += position.width;
                    position.width = num2;
                    ReflectionProbeBlendInfo info2 = blendInfos[i];
                    GUI.Label(position, "Weight " + info2.weight.ToString("f2"), EditorStyles.miniLabel);
                }
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}

