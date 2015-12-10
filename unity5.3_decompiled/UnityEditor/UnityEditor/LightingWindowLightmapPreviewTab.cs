namespace UnityEditor
{
    using System;
    using System.Collections;
    using UnityEngine;

    internal class LightingWindowLightmapPreviewTab
    {
        private Vector2 m_ScrollPositionLightmaps = Vector2.zero;
        private Vector2 m_ScrollPositionMaps = Vector2.zero;
        private int m_SelectedLightmap = -1;
        private static Styles s_Styles;

        private static void Header(ref Rect rect, float headerHeight, float headerLeftMargin, float maxLightmaps)
        {
            Rect position = GUILayoutUtility.GetRect(rect.width, headerHeight);
            position.width = rect.width / maxLightmaps;
            position.y -= rect.height;
            rect.y += headerHeight;
            position.x += headerLeftMargin;
            EditorGUI.DropShadowLabel(position, "Intensity");
            position.x += position.width;
            EditorGUI.DropShadowLabel(position, "Directionality");
        }

        private Texture2D LightmapField(Texture2D lightmap, int index)
        {
            Rect rect = GUILayoutUtility.GetRect(100f, 100f, EditorStyles.objectField);
            this.MenuSelectLightmapUsers(rect, index);
            Texture2D textured = EditorGUI.ObjectField(rect, lightmap, typeof(Texture2D), false) as Texture2D;
            if ((index == this.m_SelectedLightmap) && (Event.current.type == EventType.Repaint))
            {
                s_Styles.selectedLightmapHighlight.Draw(rect, false, false, false, false);
            }
            return textured;
        }

        public void LightmapPreview(Rect r)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            bool flag = true;
            GUI.Box(r, string.Empty, "PreBackground");
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(r.height) };
            this.m_ScrollPositionLightmaps = EditorGUILayout.BeginScrollView(this.m_ScrollPositionLightmaps, options);
            int lightmapIndex = 0;
            float maxLightmaps = 2f;
            foreach (LightmapData data in LightmapSettings.lightmaps)
            {
                if ((data.lightmapFar == null) && (data.lightmapNear == null))
                {
                    lightmapIndex++;
                }
                else
                {
                    int num6 = (data.lightmapFar == null) ? -1 : Math.Max(data.lightmapFar.width, data.lightmapFar.height);
                    int num7 = (data.lightmapNear == null) ? -1 : Math.Max(data.lightmapNear.width, data.lightmapNear.height);
                    Texture2D textured = (num6 <= num7) ? data.lightmapNear : data.lightmapFar;
                    GUILayoutOption[] optionArray = new GUILayoutOption[] { GUILayout.MaxWidth(textured.width * maxLightmaps), GUILayout.MaxHeight((float) textured.height) };
                    Rect aspectRect = GUILayoutUtility.GetAspectRect((textured.width * maxLightmaps) / ((float) textured.height), optionArray);
                    if (flag)
                    {
                        Header(ref aspectRect, 20f, 6f, maxLightmaps);
                        flag = false;
                    }
                    aspectRect.width /= maxLightmaps;
                    EditorGUI.DrawPreviewTexture(aspectRect, data.lightmapFar);
                    this.MenuSelectLightmapUsers(aspectRect, lightmapIndex);
                    if (data.lightmapNear != null)
                    {
                        aspectRect.x += aspectRect.width;
                        EditorGUI.DrawPreviewTexture(aspectRect, data.lightmapNear);
                        this.MenuSelectLightmapUsers(aspectRect, lightmapIndex);
                    }
                    lightmapIndex++;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public void Maps()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            GUI.changed = false;
            if (Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.OnDemand)
            {
                SerializedObject obj2 = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
                EditorGUILayout.PropertyField(obj2.FindProperty("m_LightingDataAsset"), s_Styles.LightingDataAsset, new GUILayoutOption[0]);
                obj2.ApplyModifiedProperties();
            }
            GUILayout.Space(10f);
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            this.m_ScrollPositionMaps = GUILayout.BeginScrollView(this.m_ScrollPositionMaps, new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(true);
            for (int i = 0; i < lightmaps.Length; i++)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.Label(i.ToString(), new GUILayoutOption[0]);
                GUILayout.Space(5f);
                lightmaps[i].lightmapFar = this.LightmapField(lightmaps[i].lightmapFar, i);
                GUILayout.Space(10f);
                lightmaps[i].lightmapNear = this.LightmapField(lightmaps[i].lightmapNear, i);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndScrollView();
        }

        private void MenuSelectLightmapUsers(Rect rect, int lightmapIndex)
        {
            if ((Event.current.type == EventType.ContextClick) && rect.Contains(Event.current.mousePosition))
            {
                string[] texts = new string[] { "Select Lightmap Users" };
                Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                EditorUtility.DisplayCustomMenu(position, EditorGUIUtility.TempContent(texts), -1, new EditorUtility.SelectMenuItemFunction(this.SelectLightmapUsers), lightmapIndex);
                Event.current.Use();
            }
        }

        private void SelectLightmapUsers(object userData, string[] options, int selected)
        {
            int num = (int) userData;
            ArrayList list = new ArrayList();
            MeshRenderer[] rendererArray = Object.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
            foreach (MeshRenderer renderer in rendererArray)
            {
                if ((renderer != null) && (renderer.lightmapIndex == num))
                {
                    list.Add(renderer.gameObject);
                }
            }
            Terrain[] terrainArray = Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
            foreach (Terrain terrain in terrainArray)
            {
                if ((terrain != null) && (terrain.lightmapIndex == num))
                {
                    list.Add(terrain.gameObject);
                }
            }
            Selection.objects = list.ToArray(typeof(Object)) as Object[];
        }

        public void UpdateLightmapSelection()
        {
            MeshRenderer renderer;
            Terrain terrain = null;
            if ((Selection.activeGameObject == null) || (((renderer = Selection.activeGameObject.GetComponent<MeshRenderer>()) == null) && ((terrain = Selection.activeGameObject.GetComponent<Terrain>()) == null)))
            {
                this.m_SelectedLightmap = -1;
            }
            else
            {
                this.m_SelectedLightmap = (renderer == null) ? terrain.lightmapIndex : renderer.lightmapIndex;
            }
        }

        private class Styles
        {
            public GUIContent LightingDataAsset = EditorGUIUtility.TextContent("Lighting Data Asset|A different LightingData.asset can be assigned here. These assets are generated by baking a scene in the OnDemand mode.");
            public GUIContent LightProbes = EditorGUIUtility.TextContent("Light Probes|A different LightProbes.asset can be assigned here. These assets are generated by baking a scene containing light probes.");
            public GUIContent MapsArraySize = EditorGUIUtility.TextContent("Array Size|The length of the array of lightmaps.");
            public GUIStyle selectedLightmapHighlight = "LightmapEditorSelectedHighlight";
        }
    }
}

