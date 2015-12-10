namespace UnityEditor
{
    using System;
    using UnityEngine;

    [EditorWindowTitle(title="Occlusion", icon="Occlusion")]
    internal class OcclusionCullingWindow : EditorWindow
    {
        private Mode m_Mode;
        private Object[] m_Objects;
        private bool m_PreVis;
        private Vector2 m_ScrollPosition = Vector2.zero;
        private string m_Warning;
        private static OcclusionCullingWindow ms_OcclusionCullingWindow;
        private static bool s_IsVisible;
        private static Styles s_Styles;

        private void AreaSelectionGUI()
        {
            GameObject[] objArray;
            bool flag = true;
            Type[] types = new Type[] { typeof(Renderer), typeof(OcclusionArea) };
            Type type = SceneModeUtility.SearchBar(types);
            EditorGUILayout.Space();
            OcclusionArea[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<OcclusionArea>(out objArray, new Type[0]);
            if (objArray.Length > 0)
            {
                flag = false;
                EditorGUILayout.MultiSelectionObjectTitleBar(selectedObjectsOfType);
                SerializedObject obj2 = new SerializedObject(selectedObjectsOfType);
                EditorGUILayout.PropertyField(obj2.FindProperty("m_IsViewVolume"), new GUILayoutOption[0]);
                obj2.ApplyModifiedProperties();
            }
            Type[] typeArray2 = new Type[] { typeof(MeshRenderer), typeof(SkinnedMeshRenderer) };
            Renderer[] objects = SceneModeUtility.GetSelectedObjectsOfType<Renderer>(out objArray, typeArray2);
            if (objArray.Length > 0)
            {
                flag = false;
                EditorGUILayout.MultiSelectionObjectTitleBar(objects);
                SerializedObject obj3 = new SerializedObject(objArray);
                SceneModeUtility.StaticFlagField("Occluder Static", obj3.FindProperty("m_StaticEditorFlags"), 2);
                SceneModeUtility.StaticFlagField("Occludee Static", obj3.FindProperty("m_StaticEditorFlags"), 0x10);
                obj3.ApplyModifiedProperties();
            }
            if (flag)
            {
                GUILayout.Label(s_Styles.emptyAreaSelection, EditorStyles.helpBox, new GUILayoutOption[0]);
                if (type == typeof(OcclusionArea))
                {
                    EditorGUIUtility.labelWidth = 80f;
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PrefixLabel("Create New");
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button("Occlusion Area", EditorStyles.miniButton, options))
                    {
                        this.CreateNewArea();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private static void BackgroundTaskStatusChanged()
        {
            if (ms_OcclusionCullingWindow != null)
            {
                ms_OcclusionCullingWindow.Repaint();
            }
        }

        private void BakeButtons()
        {
            float width = 95f;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            bool flag = !EditorApplication.isPlayingOrWillChangePlaymode;
            GUI.enabled = (StaticOcclusionCulling.umbraDataSize != 0) && flag;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width) };
            if (GUILayout.Button("Clear", options))
            {
                StaticOcclusionCulling.Clear();
            }
            GUI.enabled = flag;
            if (StaticOcclusionCulling.isRunning)
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(width) };
                if (GUILayout.Button("Cancel", optionArray2))
                {
                    StaticOcclusionCulling.Cancel();
                }
            }
            else
            {
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(width) };
                if (GUILayout.Button("Bake", optionArray3))
                {
                    StaticOcclusionCulling.GenerateInBackground();
                }
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private void BakeSettings()
        {
            float width = 150f;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width) };
            if (GUILayout.Button("Set default parameters", options))
            {
                GUIUtility.keyboardControl = 0;
                StaticOcclusionCulling.SetDefaultOcclusionBakeSettings();
            }
            GUILayout.Label(s_Styles.defaultParameterText.tooltip, EditorStyles.helpBox, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            float num2 = EditorGUILayout.FloatField(s_Styles.smallestOccluder, StaticOcclusionCulling.smallestOccluder, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                StaticOcclusionCulling.smallestOccluder = num2;
            }
            EditorGUI.BeginChangeCheck();
            float num3 = EditorGUILayout.FloatField(s_Styles.smallestHole, StaticOcclusionCulling.smallestHole, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                StaticOcclusionCulling.smallestHole = num3;
            }
            EditorGUI.BeginChangeCheck();
            float num4 = EditorGUILayout.Slider(s_Styles.backfaceThreshold, StaticOcclusionCulling.backfaceThreshold, 5f, 100f, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                StaticOcclusionCulling.backfaceThreshold = num4;
            }
        }

        private void CameraSelectionGUI()
        {
            Type[] types = new Type[] { typeof(Camera) };
            SceneModeUtility.SearchBar(types);
            EditorGUILayout.Space();
            Camera component = null;
            if (Selection.activeGameObject != null)
            {
                component = Selection.activeGameObject.GetComponent<Camera>();
            }
            if (component != null)
            {
                Camera[] objects = new Camera[] { component };
                EditorGUILayout.MultiSelectionObjectTitleBar(objects);
                EditorGUILayout.HelpBox(s_Styles.seeVisualizationInScene.text, MessageType.Info);
            }
            else
            {
                GUILayout.Label(s_Styles.emptyCameraSelection, EditorStyles.helpBox, new GUILayoutOption[0]);
            }
        }

        private OcclusionArea CreateNewArea()
        {
            GameObject obj2 = new GameObject("Occlusion Area");
            OcclusionArea area = obj2.AddComponent<OcclusionArea>();
            Selection.activeGameObject = obj2;
            return area;
        }

        private void DisplayControls(Object target, SceneView sceneView)
        {
            if ((sceneView != null) && s_IsVisible)
            {
                bool flag2 = this.ShowModePopup(GUILayoutUtility.GetRect(170f, EditorGUIUtility.singleLineHeight));
                if (Event.current.type == EventType.Layout)
                {
                    this.m_Warning = string.Empty;
                    if (!flag2)
                    {
                        if (StaticOcclusionCullingVisualization.previewOcclucionCamera == null)
                        {
                            this.m_Warning = "No camera selected for occlusion preview.";
                        }
                        else if (!StaticOcclusionCullingVisualization.isPreviewOcclusionCullingCameraInPVS)
                        {
                            this.m_Warning = "Camera is not inside an Occlusion View Area.";
                        }
                    }
                }
                int num = 12;
                if (!string.IsNullOrEmpty(this.m_Warning))
                {
                    Rect position = GUILayoutUtility.GetRect(100f, (float) (num + 0x13));
                    position.x += EditorGUI.indent;
                    position.width -= EditorGUI.indent;
                    GUI.Label(position, this.m_Warning, EditorStyles.helpBox);
                }
                else
                {
                    Rect rect = GUILayoutUtility.GetRect(200f, (float) num);
                    rect.x += EditorGUI.indent;
                    rect.width -= EditorGUI.indent;
                    Rect rect3 = new Rect(rect.x, rect.y, rect.width, rect.height);
                    if (flag2)
                    {
                        EditorGUI.DrawLegend(rect3, Color.white, "View Volumes", StaticOcclusionCullingVisualization.showViewVolumes);
                    }
                    else
                    {
                        EditorGUI.DrawLegend(rect3, Color.white, "Camera Volumes", StaticOcclusionCullingVisualization.showViewVolumes);
                    }
                    bool flag = GUI.Toggle(rect3, StaticOcclusionCullingVisualization.showViewVolumes, string.Empty, GUIStyle.none);
                    if (flag != StaticOcclusionCullingVisualization.showViewVolumes)
                    {
                        StaticOcclusionCullingVisualization.showViewVolumes = flag;
                        SceneView.RepaintAll();
                    }
                    if (!flag2)
                    {
                        rect = GUILayoutUtility.GetRect(100f, (float) num);
                        rect.x += EditorGUI.indent;
                        rect.width -= EditorGUI.indent;
                        rect3 = new Rect(rect.x, rect.y, rect.width, rect.height);
                        EditorGUI.DrawLegend(rect3, Color.green, "Visibility Lines", StaticOcclusionCullingVisualization.showVisibilityLines);
                        flag = GUI.Toggle(rect3, StaticOcclusionCullingVisualization.showVisibilityLines, string.Empty, GUIStyle.none);
                        if (flag != StaticOcclusionCullingVisualization.showVisibilityLines)
                        {
                            StaticOcclusionCullingVisualization.showVisibilityLines = flag;
                            SceneView.RepaintAll();
                        }
                        rect = GUILayoutUtility.GetRect(100f, (float) num);
                        rect.x += EditorGUI.indent;
                        rect.width -= EditorGUI.indent;
                        rect3 = new Rect(rect.x, rect.y, rect.width, rect.height);
                        EditorGUI.DrawLegend(rect3, Color.grey, "Portals", StaticOcclusionCullingVisualization.showPortals);
                        flag = GUI.Toggle(rect3, StaticOcclusionCullingVisualization.showPortals, string.Empty, GUIStyle.none);
                        if (flag != StaticOcclusionCullingVisualization.showPortals)
                        {
                            StaticOcclusionCullingVisualization.showPortals = flag;
                            SceneView.RepaintAll();
                        }
                    }
                    if (!flag2)
                    {
                        flag = GUILayout.Toggle(StaticOcclusionCullingVisualization.showGeometryCulling, "Occlusion culling", new GUILayoutOption[0]);
                        if (flag != StaticOcclusionCullingVisualization.showGeometryCulling)
                        {
                            StaticOcclusionCullingVisualization.showGeometryCulling = flag;
                            SceneView.RepaintAll();
                        }
                    }
                }
            }
        }

        [MenuItem("Window/Occlusion Culling", false, 0x833)]
        private static void GenerateWindow()
        {
            Type[] desiredDockNextTo = new Type[] { typeof(InspectorWindow) };
            EditorWindow.GetWindow<OcclusionCullingWindow>(desiredDockNextTo).minSize = new Vector2(300f, 250f);
        }

        private void ModeToggle()
        {
            this.m_Mode = (Mode) GUILayout.Toolbar((int) this.m_Mode, s_Styles.ModeToggles, "LargeButton", new GUILayoutOption[0]);
            if (GUI.changed)
            {
                if ((this.m_Mode == Mode.Visualization) && (StaticOcclusionCulling.umbraDataSize > 0))
                {
                    StaticOcclusionCullingVisualization.showPreVisualization = false;
                }
                else
                {
                    StaticOcclusionCullingVisualization.showPreVisualization = true;
                }
                SceneView.RepaintAll();
            }
        }

        private void OnBecameInvisible()
        {
            s_IsVisible = false;
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            StaticOcclusionCullingVisualization.showOcclusionCulling = false;
            SceneView.RepaintAll();
        }

        private void OnBecameVisible()
        {
            if (!s_IsVisible)
            {
                s_IsVisible = true;
                SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
                StaticOcclusionCullingVisualization.showOcclusionCulling = true;
                SceneView.RepaintAll();
            }
        }

        private void OnDidOpenScene()
        {
            base.Repaint();
        }

        private void OnDisable()
        {
            ms_OcclusionCullingWindow = null;
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            ms_OcclusionCullingWindow = this;
            base.autoRepaintOnSceneChange = true;
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
            base.Repaint();
        }

        private void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            if ((this.m_Mode != Mode.Visualization) && !StaticOcclusionCullingVisualization.showPreVisualization)
            {
                this.m_Mode = Mode.Visualization;
            }
            EditorGUILayout.Space();
            this.ModeToggle();
            EditorGUILayout.Space();
            this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
            switch (this.m_Mode)
            {
                case Mode.AreaSettings:
                    this.AreaSelectionGUI();
                    break;

                case Mode.BakeSettings:
                    this.BakeSettings();
                    break;

                case Mode.Visualization:
                    if (StaticOcclusionCulling.umbraDataSize <= 0)
                    {
                        GUILayout.Label(s_Styles.noOcclusionData, EditorStyles.helpBox, new GUILayoutOption[0]);
                        break;
                    }
                    this.CameraSelectionGUI();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(s_Styles.visualizationNote, EditorStyles.helpBox, new GUILayoutOption[0]);
                    break;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
            this.BakeButtons();
            EditorGUILayout.Space();
            this.SummaryGUI();
        }

        public void OnSceneViewGUI(SceneView sceneView)
        {
            if (s_IsVisible)
            {
                SceneViewOverlay.Window(new GUIContent("Occlusion Culling"), new SceneViewOverlay.WindowFunction(this.DisplayControls), 100, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
            }
        }

        private void OnSelectionChange()
        {
            if ((this.m_Mode == Mode.AreaSettings) || (this.m_Mode == Mode.Visualization))
            {
                base.Repaint();
            }
        }

        private void SetShowVolumeCulling()
        {
            StaticOcclusionCullingVisualization.showPreVisualization = false;
            this.m_Mode = Mode.Visualization;
            if (ms_OcclusionCullingWindow != null)
            {
                ms_OcclusionCullingWindow.Repaint();
            }
            SceneView.RepaintAll();
        }

        private void SetShowVolumePreVis()
        {
            StaticOcclusionCullingVisualization.showPreVisualization = true;
            if (this.m_Mode == Mode.Visualization)
            {
                this.m_Mode = Mode.AreaSettings;
            }
            if (ms_OcclusionCullingWindow != null)
            {
                ms_OcclusionCullingWindow.Repaint();
            }
            SceneView.RepaintAll();
        }

        private bool ShowModePopup(Rect popupRect)
        {
            int umbraDataSize = StaticOcclusionCulling.umbraDataSize;
            if (this.m_PreVis != StaticOcclusionCullingVisualization.showPreVisualization)
            {
                SceneView.RepaintAll();
            }
            if (Event.current.type == EventType.Layout)
            {
                this.m_PreVis = StaticOcclusionCullingVisualization.showPreVisualization;
            }
            string[] strArray = new string[] { "Edit", "Visualize" };
            int index = !this.m_PreVis ? 1 : 0;
            if (EditorGUI.ButtonMouseDown(popupRect, new GUIContent(strArray[index]), FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(strArray[0]), index == 0, new GenericMenu.MenuFunction(this.SetShowVolumePreVis));
                if (umbraDataSize > 0)
                {
                    menu.AddItem(new GUIContent(strArray[1]), index == 1, new GenericMenu.MenuFunction(this.SetShowVolumeCulling));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(strArray[1]));
                }
                menu.Popup(popupRect, index);
            }
            return this.m_PreVis;
        }

        private void SummaryGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            if (StaticOcclusionCulling.umbraDataSize == 0)
            {
                GUILayout.Label(s_Styles.noOcclusionData, s_Styles.labelStyle, new GUILayoutOption[0]);
            }
            else
            {
                GUILayout.Label("Last bake:", s_Styles.labelStyle, new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.Label("Occlusion data size ", s_Styles.labelStyle, new GUILayoutOption[0]);
                GUILayout.EndVertical();
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.Label(EditorUtility.FormatBytes(StaticOcclusionCulling.umbraDataSize), s_Styles.labelStyle, new GUILayoutOption[0]);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private enum Mode
        {
            AreaSettings,
            BakeSettings,
            Visualization
        }

        private class Styles
        {
            public GUIContent backfaceThreshold = EditorGUIUtility.TextContent("Backface Threshold|The backface threshold is a size optimization that reduces unnecessary details by testing backfaces. A value of 100 is robust and never removes any backfaces. A value of 5 aggressively reduces the data based on locations with visible backfaces. The idea is that typically valid camera positions cannot see many backfaces.  For example geometry under terrain and inside solid objects can be removed.");
            public GUIContent defaultParameterText = EditorGUIUtility.TextContent("Default Parameters|The default parameters guarantee that any given scene computes fast and the occlusion culling results are good. As the parameters are always scene specific, better results will be achieved when fine tuning the parameters on a scene to scene basis. All the parameters are dependent on the unit scale of the scene and it is imperative that the unit scale parameter is set correctly before setting the default values.");
            public GUIContent emptyAreaSelection = new GUIContent("Select a Mesh Renderer or an Occlusion Area from the scene.");
            public GUIContent emptyCameraSelection = new GUIContent("Select a Camera from the scene.");
            public GUIContent farClipPlane = EditorGUIUtility.TextContent("Far Clip Plane|Far Clip Plane used during baking. This should match the largest far clip plane used by any camera in the scene. A value of 0.0 sets the far plane to Infinity.");
            public GUIStyle labelStyle = EditorStyles.wordWrappedMiniLabel;
            public GUIContent[] ModeToggles = new GUIContent[] { new GUIContent("Object"), new GUIContent("Bake"), new GUIContent("Visualization") };
            public GUIContent noOcclusionData = new GUIContent("No occlusion data has been baked.");
            public GUIContent seeVisualizationInScene = new GUIContent("See the occlusion culling visualization in the Scene View based on the selected Camera.");
            public GUIContent smallestHole = EditorGUIUtility.TextContent("Smallest Hole|Smallest hole in the geometry through which the camera is supposed to see. The single float value of the parameter represents the diameter of the imaginary smallest hole, i.e. the maximum extent of a 3D object that fits through the hole.");
            public GUIContent smallestOccluder = EditorGUIUtility.TextContent("Smallest Occluder|The size of the smallest object that will be used to hide other objects when doing occlusion culling. For example, if a value of 4 is chosen, then all the objects that are higher or wider than 4 meters will block visibility and the objects that are smaller than that will not. This value is a tradeoff between occlusion accuracy and storage size.");
            public GUIContent visualizationNote = new GUIContent("The visualization may not correspond to current bake settings and Occlusion Area placements if they have been changed since last bake.");
        }
    }
}

