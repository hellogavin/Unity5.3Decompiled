namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    [EditorWindowTitle(title="Navigation", icon="Navigation")]
    internal class NavMeshEditorWindow : EditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static Func<GameObject, bool> <>f__am$cache17;
        private const string kRootPath = "m_BuildSettings.";
        private SerializedProperty m_AccuratePlacement;
        private bool m_Advanced;
        private SerializedProperty m_AgentClimb;
        private SerializedProperty m_AgentHeight;
        private SerializedProperty m_AgentRadius;
        private SerializedProperty m_AgentSlope;
        private SerializedProperty m_Areas;
        private ReorderableList m_AreasList;
        private SerializedProperty m_CellSize;
        private bool m_HasPendingAgentDebugInfo;
        private bool m_HasRepaintedForPendingAgentDebugInfo;
        private SerializedProperty m_LedgeDropHeight;
        private SerializedProperty m_ManualCellSize;
        private SerializedProperty m_MaxJumpAcrossDistance;
        private SerializedProperty m_MinRegionArea;
        private Mode m_Mode;
        private SerializedObject m_NavMeshAreasObject;
        private SerializedObject m_Object;
        private Vector2 m_ScrollPos = Vector2.zero;
        private int m_SelectedNavMeshAgentCount;
        private int m_SelectedNavMeshObstacleCount;
        private static NavMeshEditorWindow s_NavMeshEditorWindow;
        private static Styles s_Styles;

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reset Bake Settings"), false, new GenericMenu.MenuFunction(this.ResetBakeSettings));
        }

        private void AreaSettings()
        {
            if ((this.m_NavMeshAreasObject != null) && (this.m_AreasList != null))
            {
                this.m_NavMeshAreasObject.Update();
                this.m_AreasList.DoLayoutList();
                this.m_NavMeshAreasObject.ApplyModifiedProperties();
            }
        }

        public static void BackgroundTaskStatusChanged()
        {
            if (s_NavMeshEditorWindow != null)
            {
                s_NavMeshEditorWindow.Repaint();
            }
        }

        private static void BakeButtons()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            bool enabled = GUI.enabled;
            GUI.enabled &= !Application.isPlaying;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(95f) };
            if (GUILayout.Button("Clear", options))
            {
                NavMeshBuilder.ClearAllNavMeshes();
            }
            GUI.enabled = enabled;
            if (NavMeshBuilder.isRunning)
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(95f) };
                if (GUILayout.Button("Cancel", optionArray2))
                {
                    NavMeshBuilder.Cancel();
                }
            }
            else
            {
                enabled = GUI.enabled;
                GUI.enabled &= !Application.isPlaying;
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(95f) };
                if (GUILayout.Button("Bake", optionArray3))
                {
                    NavMeshBuilder.BuildNavMeshAsync();
                }
                GUI.enabled = enabled;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void BakeSettings()
        {
            EditorGUILayout.LabelField(s_Styles.m_AgentSizeHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            Rect rect = EditorGUILayout.GetControlRect(false, 120f, new GUILayoutOption[0]);
            this.DrawAgentDiagram(rect, this.m_AgentRadius.floatValue, this.m_AgentHeight.floatValue, this.m_AgentClimb.floatValue, this.m_AgentSlope.floatValue);
            float num2 = EditorGUILayout.FloatField(s_Styles.m_AgentRadiusContent, this.m_AgentRadius.floatValue, new GUILayoutOption[0]);
            if ((num2 >= 0.001f) && !Mathf.Approximately(num2 - this.m_AgentRadius.floatValue, 0f))
            {
                this.m_AgentRadius.floatValue = num2;
                if (!this.m_ManualCellSize.boolValue)
                {
                    this.m_CellSize.floatValue = (2f * this.m_AgentRadius.floatValue) / 6f;
                }
            }
            if ((this.m_AgentRadius.floatValue < 0.05f) && !this.m_ManualCellSize.boolValue)
            {
                EditorGUILayout.HelpBox("The agent radius you've set is really small, this can slow down the build.\nIf you intended to allow the agent to move close to the borders and walls, please adjust voxel size in advaced settings to ensure correct bake.", MessageType.Warning);
            }
            float num3 = EditorGUILayout.FloatField(s_Styles.m_AgentHeightContent, this.m_AgentHeight.floatValue, new GUILayoutOption[0]);
            if ((num3 >= 0.001f) && !Mathf.Approximately(num3 - this.m_AgentHeight.floatValue, 0f))
            {
                this.m_AgentHeight.floatValue = num3;
            }
            EditorGUILayout.Slider(this.m_AgentSlope, 0f, 60f, s_Styles.m_AgentSlopeContent, new GUILayoutOption[0]);
            if (this.m_AgentSlope.floatValue > 60f)
            {
                EditorGUILayout.HelpBox("The maximum slope should be set to less than " + 60f + " degrees to prevent NavMesh build artifacts on slopes. ", MessageType.Warning);
            }
            float num5 = EditorGUILayout.FloatField(s_Styles.m_AgentClimbContent, this.m_AgentClimb.floatValue, new GUILayoutOption[0]);
            if ((num5 >= 0f) && !Mathf.Approximately(this.m_AgentClimb.floatValue - num5, 0f))
            {
                this.m_AgentClimb.floatValue = num5;
            }
            if (this.m_AgentClimb.floatValue > this.m_AgentHeight.floatValue)
            {
                EditorGUILayout.HelpBox("Step height should be less than agent height.\nClamping step height to " + this.m_AgentHeight.floatValue + " internally when baking.", MessageType.Warning);
            }
            float floatValue = this.m_CellSize.floatValue;
            float num7 = floatValue * 0.5f;
            int num8 = (int) Mathf.Ceil(this.m_AgentClimb.floatValue / num7);
            float num9 = Mathf.Tan((this.m_AgentSlope.floatValue / 180f) * 3.141593f) * floatValue;
            int num10 = (int) Mathf.Ceil((num9 * 2f) / num7);
            if (num10 > num8)
            {
                float f = (num8 * num7) / (floatValue * 2f);
                float num12 = (Mathf.Atan(f) / 3.141593f) * 180f;
                float num13 = (num10 - 1) * num7;
                EditorGUILayout.HelpBox("Step Height conflicts with Max Slope. This makes some slopes unwalkable.\nConsider decreasing Max Slope to < " + num12.ToString("0.0") + " degrees.\nOr, increase Step Height to > " + num13.ToString("0.00") + ".", MessageType.Warning);
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(s_Styles.m_OffmeshHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            float num14 = EditorGUILayout.FloatField(s_Styles.m_AgentDropContent, this.m_LedgeDropHeight.floatValue, new GUILayoutOption[0]);
            if ((num14 >= 0f) && !Mathf.Approximately(num14 - this.m_LedgeDropHeight.floatValue, 0f))
            {
                this.m_LedgeDropHeight.floatValue = num14;
            }
            float num15 = EditorGUILayout.FloatField(s_Styles.m_AgentJumpContent, this.m_MaxJumpAcrossDistance.floatValue, new GUILayoutOption[0]);
            if ((num15 >= 0f) && !Mathf.Approximately(num15 - this.m_MaxJumpAcrossDistance.floatValue, 0f))
            {
                this.m_MaxJumpAcrossDistance.floatValue = num15;
            }
            EditorGUILayout.Space();
            this.m_Advanced = GUILayout.Toggle(this.m_Advanced, s_Styles.m_AdvancedHeader, EditorStyles.foldout, new GUILayoutOption[0]);
            if (this.m_Advanced)
            {
                EditorGUI.indentLevel++;
                bool flag = EditorGUILayout.Toggle(s_Styles.m_ManualCellSizeContent, this.m_ManualCellSize.boolValue, new GUILayoutOption[0]);
                if (flag != this.m_ManualCellSize.boolValue)
                {
                    this.m_ManualCellSize.boolValue = flag;
                    if (!flag)
                    {
                        this.m_CellSize.floatValue = (2f * this.m_AgentRadius.floatValue) / 6f;
                    }
                }
                EditorGUI.BeginDisabledGroup(!this.m_ManualCellSize.boolValue);
                EditorGUI.indentLevel++;
                float num16 = EditorGUILayout.FloatField(s_Styles.m_CellSizeContent, this.m_CellSize.floatValue, new GUILayoutOption[0]);
                if ((num16 > 0f) && !Mathf.Approximately(num16 - this.m_CellSize.floatValue, 0f))
                {
                    this.m_CellSize.floatValue = Math.Max(0.01f, num16);
                }
                if (num16 < 0.01f)
                {
                    EditorGUILayout.HelpBox("The voxel size should be larger than 0.01.", MessageType.Warning);
                }
                float num17 = (this.m_CellSize.floatValue <= 0f) ? 0f : (this.m_AgentRadius.floatValue / this.m_CellSize.floatValue);
                EditorGUILayout.LabelField(" ", num17.ToString("0.00") + " voxels per agent radius", EditorStyles.miniLabel, new GUILayoutOption[0]);
                if (this.m_ManualCellSize.boolValue)
                {
                    float num19 = this.m_CellSize.floatValue * 0.5f;
                    if (((int) Mathf.Floor(this.m_AgentHeight.floatValue / num19)) > 250)
                    {
                        EditorGUILayout.HelpBox("The number of voxels per agent height is too high. This will reduce the accuracy of the navmesh. Consider using voxel size of at least " + (((this.m_AgentHeight.floatValue / 250f) / 0.5f)).ToString("0.000") + ".", MessageType.Warning);
                    }
                    if (num17 < 1f)
                    {
                        EditorGUILayout.HelpBox("The number of voxels per agent radius is too small. The agent may not avoid walls and ledges properly. Consider using voxel size of at least " + ((this.m_AgentRadius.floatValue / 2f)).ToString("0.000") + " (2 voxels per agent radius).", MessageType.Warning);
                    }
                    else if (num17 > 8f)
                    {
                        EditorGUILayout.HelpBox("The number of voxels per agent radius is too high. It can cause excessive build times. Consider using voxel size closer to " + ((this.m_AgentRadius.floatValue / 8f)).ToString("0.000") + " (8 voxels per radius).", MessageType.Warning);
                    }
                }
                if (this.m_ManualCellSize.boolValue)
                {
                    EditorGUILayout.HelpBox("Voxel size controls how accurately the navigation mesh is generated from the level geometry. A good voxel size is 2-4 voxels per agent radius. Making voxel size smaller will increase build time.", MessageType.None);
                }
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
                float num23 = EditorGUILayout.FloatField(s_Styles.m_MinRegionAreaContent, this.m_MinRegionArea.floatValue, new GUILayoutOption[0]);
                if ((num23 >= 0f) && (num23 != this.m_MinRegionArea.floatValue))
                {
                    this.m_MinRegionArea.floatValue = num23;
                }
                EditorGUILayout.Space();
                bool flag2 = EditorGUILayout.Toggle(s_Styles.m_AgentPlacementContent, this.m_AccuratePlacement.boolValue, new GUILayoutOption[0]);
                if (flag2 != this.m_AccuratePlacement.boolValue)
                {
                    this.m_AccuratePlacement.boolValue = flag2;
                }
                EditorGUI.indentLevel--;
            }
            if (Unsupported.IsDeveloperBuild())
            {
                EditorGUILayout.Space();
                GUILayout.Label("Internal Bake Debug Options", EditorStyles.boldLabel, new GUILayoutOption[0]);
                EditorGUILayout.HelpBox("Note: The debug visualization is build during bake, so you'll need to bake for these settings to take effect.", MessageType.None);
                bool showAutoOffMeshLinkSampling = NavMeshVisualizationSettings.showAutoOffMeshLinkSampling;
                if (showAutoOffMeshLinkSampling != EditorGUILayout.Toggle(new GUIContent("Show Auto-Off-MeshLink Sampling"), showAutoOffMeshLinkSampling, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showAutoOffMeshLinkSampling = !showAutoOffMeshLinkSampling;
                }
                bool showVoxels = NavMeshVisualizationSettings.showVoxels;
                if (showVoxels != EditorGUILayout.Toggle(new GUIContent("Show Voxels"), showVoxels, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showVoxels = !showVoxels;
                }
                bool showWalkable = NavMeshVisualizationSettings.showWalkable;
                if (showWalkable != EditorGUILayout.Toggle(new GUIContent("Show Walkable"), showWalkable, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showWalkable = !showWalkable;
                }
                bool showRawContours = NavMeshVisualizationSettings.showRawContours;
                if (showRawContours != EditorGUILayout.Toggle(new GUIContent("Show Raw Contours"), showRawContours, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showRawContours = !showRawContours;
                }
                bool showContours = NavMeshVisualizationSettings.showContours;
                if (showContours != EditorGUILayout.Toggle(new GUIContent("Show Contours"), showContours, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showContours = !showContours;
                }
                bool showInputs = NavMeshVisualizationSettings.showInputs;
                if (showInputs != EditorGUILayout.Toggle(new GUIContent("Show Inputs"), showInputs, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showInputs = !showInputs;
                }
                if (GUILayout.Button("Clear Visualiation Data", new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.ClearVisualizationData();
                    RepaintSceneAndGameViews();
                }
                EditorGUILayout.Space();
            }
        }

        private int Bit(int a, int b)
        {
            return ((a & (1 << (b & 0x1f))) >> b);
        }

        private static void DisplayAgentControls(Object target, SceneView sceneView)
        {
            EditorGUIUtility.labelWidth = 150f;
            bool flag = false;
            if (Event.current.type == EventType.Layout)
            {
                s_NavMeshEditorWindow.m_HasPendingAgentDebugInfo = NavMeshVisualizationSettings.hasPendingAgentDebugInfo;
            }
            bool showAgentPath = NavMeshVisualizationSettings.showAgentPath;
            if (showAgentPath != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Path Polygons|Shows the polygons leading to goal."), showAgentPath, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentPath = !showAgentPath;
                flag = true;
            }
            bool showAgentPathInfo = NavMeshVisualizationSettings.showAgentPathInfo;
            if (showAgentPathInfo != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Path Query Nodes|Shows the nodes expanded during last path query."), showAgentPathInfo, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentPathInfo = !showAgentPathInfo;
                flag = true;
            }
            bool showAgentNeighbours = NavMeshVisualizationSettings.showAgentNeighbours;
            if (showAgentNeighbours != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Neighbours|Show the agent neighbours cosidered during simulation."), showAgentNeighbours, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentNeighbours = !showAgentNeighbours;
                flag = true;
            }
            bool showAgentWalls = NavMeshVisualizationSettings.showAgentWalls;
            if (showAgentWalls != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Walls|Shows the wall segments handled during simulation."), showAgentWalls, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentWalls = !showAgentWalls;
                flag = true;
            }
            bool showAgentAvoidance = NavMeshVisualizationSettings.showAgentAvoidance;
            if (showAgentAvoidance != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Avoidance|Shows the processed avoidance geometry from simulation."), showAgentAvoidance, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentAvoidance = !showAgentAvoidance;
                flag = true;
            }
            if (showAgentAvoidance)
            {
                if (s_NavMeshEditorWindow.m_HasPendingAgentDebugInfo)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(165f) };
                    EditorGUILayout.BeginVertical(options);
                    EditorGUILayout.HelpBox("Avoidance display is not valid until after next game update.", MessageType.Warning);
                    EditorGUILayout.EndVertical();
                }
                if (s_NavMeshEditorWindow.m_SelectedNavMeshAgentCount > 10)
                {
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MaxWidth(165f) };
                    EditorGUILayout.BeginVertical(optionArray2);
                    EditorGUILayout.HelpBox(string.Concat(new object[] { "Avoidance visualization can be drawn for ", 10, " agents (", s_NavMeshEditorWindow.m_SelectedNavMeshAgentCount, " selected)." }), MessageType.Warning);
                    EditorGUILayout.EndVertical();
                }
            }
            if (flag)
            {
                RepaintSceneAndGameViews();
            }
        }

        private static void DisplayControls(Object target, SceneView sceneView)
        {
            EditorGUIUtility.labelWidth = 150f;
            bool flag = false;
            bool showNavMesh = NavMeshVisualizationSettings.showNavMesh;
            if (showNavMesh != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show NavMesh"), showNavMesh, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showNavMesh = !showNavMesh;
                flag = true;
            }
            EditorGUI.BeginDisabledGroup(!NavMeshVisualizationSettings.hasHeightMesh);
            bool showHeightMesh = NavMeshVisualizationSettings.showHeightMesh;
            if (showHeightMesh != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show HeightMesh"), showHeightMesh, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showHeightMesh = !showHeightMesh;
                flag = true;
            }
            EditorGUI.EndDisabledGroup();
            if (Unsupported.IsDeveloperBuild())
            {
                GUILayout.Label("Internal", new GUILayoutOption[0]);
                bool showNavMeshPortals = NavMeshVisualizationSettings.showNavMeshPortals;
                if (showNavMeshPortals != EditorGUILayout.Toggle(new GUIContent("Show NavMesh Portals"), showNavMeshPortals, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showNavMeshPortals = !showNavMeshPortals;
                    flag = true;
                }
                bool showNavMeshLinks = NavMeshVisualizationSettings.showNavMeshLinks;
                if (showNavMeshLinks != EditorGUILayout.Toggle(new GUIContent("Show NavMesh Links"), showNavMeshLinks, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showNavMeshLinks = !showNavMeshLinks;
                    flag = true;
                }
                bool showProximityGrid = NavMeshVisualizationSettings.showProximityGrid;
                if (showProximityGrid != EditorGUILayout.Toggle(new GUIContent("Show Proximity Grid"), showProximityGrid, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showProximityGrid = !showProximityGrid;
                    flag = true;
                }
                bool showHeightMeshBVTree = NavMeshVisualizationSettings.showHeightMeshBVTree;
                if (showHeightMeshBVTree != EditorGUILayout.Toggle(new GUIContent("Show HeightMesh BV-Tree"), showHeightMeshBVTree, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showHeightMeshBVTree = !showHeightMeshBVTree;
                    flag = true;
                }
            }
            if (flag)
            {
                RepaintSceneAndGameViews();
            }
        }

        private static void DisplayObstacleControls(Object target, SceneView sceneView)
        {
            EditorGUIUtility.labelWidth = 150f;
            bool flag = false;
            bool showObstacleCarveHull = NavMeshVisualizationSettings.showObstacleCarveHull;
            if (showObstacleCarveHull != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Carve Hull|Shows the hull used to carve the obstacle from navmesh."), showObstacleCarveHull, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showObstacleCarveHull = !showObstacleCarveHull;
                flag = true;
            }
            if (flag)
            {
                RepaintSceneAndGameViews();
            }
        }

        private void DrawAgentDiagram(Rect rect, float agentRadius, float agentHeight, float agentClimb, float agentSlope)
        {
            if (Event.current.type == EventType.Repaint)
            {
                float num = agentRadius;
                float num2 = agentHeight;
                float num3 = agentClimb;
                float num4 = 1f;
                float num5 = 0.35f;
                float num6 = 15f;
                float num7 = rect.height - (num6 * 2f);
                num4 = Mathf.Min((float) (num7 / (num2 + ((num * 2f) * num5))), (float) (num7 / (num * 2f)));
                num2 *= num4;
                num *= num4;
                num3 *= num4;
                float x = rect.xMin + (rect.width * 0.5f);
                float y = (rect.yMax - num6) - (num * num5);
                Vector3[] points = new Vector3[40];
                Vector3[] vectorArray2 = new Vector3[20];
                Vector3[] vectorArray3 = new Vector3[20];
                for (int i = 0; i < 20; i++)
                {
                    float f = (((float) i) / 19f) * 3.141593f;
                    float num13 = Mathf.Cos(f);
                    float num14 = Mathf.Sin(f);
                    points[i] = new Vector3(x + (num13 * num), (y - num2) - ((num14 * num) * num5), 0f);
                    points[i + 20] = new Vector3(x - (num13 * num), y + ((num14 * num) * num5), 0f);
                    vectorArray2[i] = new Vector3(x - (num13 * num), (y - num2) + ((num14 * num) * num5), 0f);
                    vectorArray3[i] = new Vector3(x - (num13 * num), (y - num3) + ((num14 * num) * num5), 0f);
                }
                Color color = Handles.color;
                float xMin = rect.xMin;
                float num16 = y - num3;
                float num17 = x - (num7 * 0.75f);
                float num18 = y;
                float num19 = x + (num7 * 0.75f);
                float num20 = y;
                float num21 = num19;
                float num22 = num20;
                float num23 = rect.xMax - num19;
                num21 += Mathf.Cos(agentSlope * 0.01745329f) * num23;
                num22 -= Mathf.Sin(agentSlope * 0.01745329f) * num23;
                Vector3[] vectorArray4 = new Vector3[] { new Vector3(xMin, y, 0f), new Vector3(num21, y, 0f) };
                Vector3[] vectorArray5 = new Vector3[] { new Vector3(xMin, num16, 0f), new Vector3(num17, num16, 0f), new Vector3(num17, num18, 0f), new Vector3(num19, num20, 0f), new Vector3(num21, num22, 0f) };
                Handles.color = !EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.5f) : new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawAAPolyLine((float) 2f, vectorArray4);
                Handles.color = !EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.5f) : new Color(1f, 1f, 1f, 0.5f);
                Handles.DrawAAPolyLine((float) 3f, vectorArray5);
                Handles.color = Color.Lerp(new Color(0f, 0.75f, 1f, 1f), new Color(0.5f, 0.5f, 0.5f, 0.5f), 0.2f);
                Handles.DrawAAConvexPolygon(points);
                Handles.color = new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawAAPolyLine((float) 2f, vectorArray3);
                Handles.color = new Color(1f, 1f, 1f, 0.4f);
                Handles.DrawAAPolyLine((float) 2f, vectorArray2);
                Vector3[] vectorArray6 = new Vector3[] { new Vector3(x, y - num2, 0f), new Vector3(x + num, y - num2, 0f) };
                Handles.color = new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawAAPolyLine((float) 2f, vectorArray6);
                GUI.Label(new Rect((x + num) + 5f, (y - (num2 * 0.5f)) - 10f, 150f, 20f), string.Format("H = {0}", agentHeight));
                GUI.Label(new Rect(x, ((y - num2) - (num * num5)) - 15f, 150f, 20f), string.Format("R = {0}", agentRadius));
                GUI.Label(new Rect(((xMin + num17) * 0.5f) - 20f, num16 - 15f, 150f, 20f), string.Format("{0}", agentClimb));
                GUI.Label(new Rect(num19 + 20f, num20 - 15f, 150f, 20f), string.Format("{0}\x00b0", agentSlope));
                Handles.color = color;
            }
        }

        private void DrawAreaListElement(Rect rect, int index, bool selected, bool focused)
        {
            SerializedProperty arrayElementAtIndex = this.m_Areas.GetArrayElementAtIndex(index);
            if (arrayElementAtIndex != null)
            {
                SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("name");
                SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("cost");
                if ((property != null) && (property3 != null))
                {
                    Rect rect2;
                    Rect rect3;
                    Rect rect4;
                    Rect rect5;
                    rect.height -= 2f;
                    bool flag = false;
                    bool flag2 = true;
                    bool flag3 = true;
                    switch (index)
                    {
                        case 0:
                            flag = true;
                            flag2 = false;
                            flag3 = true;
                            break;

                        case 1:
                            flag = true;
                            flag2 = false;
                            flag3 = false;
                            break;

                        case 2:
                            flag = true;
                            flag2 = false;
                            flag3 = true;
                            break;

                        default:
                            flag = false;
                            flag2 = true;
                            flag3 = true;
                            break;
                    }
                    this.GetAreaListRects(rect, out rect2, out rect3, out rect4, out rect5);
                    bool enabled = GUI.enabled;
                    Color areaColor = this.GetAreaColor(index);
                    Color color = new Color(areaColor.r * 0.1f, areaColor.g * 0.1f, areaColor.b * 0.1f, 0.6f);
                    EditorGUI.DrawRect(rect2, areaColor);
                    EditorGUI.DrawRect(new Rect(rect2.x, rect2.y, 1f, rect2.height), color);
                    EditorGUI.DrawRect(new Rect((rect2.x + rect2.width) - 1f, rect2.y, 1f, rect2.height), color);
                    EditorGUI.DrawRect(new Rect(rect2.x + 1f, rect2.y, rect2.width - 2f, 1f), color);
                    EditorGUI.DrawRect(new Rect(rect2.x + 1f, (rect2.y + rect2.height) - 1f, rect2.width - 2f, 1f), color);
                    if (flag)
                    {
                        GUI.Label(rect3, EditorGUIUtility.TempContent("Built-in " + index));
                    }
                    else
                    {
                        GUI.Label(rect3, EditorGUIUtility.TempContent("User " + index));
                    }
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    EditorGUI.BeginChangeCheck();
                    GUI.enabled = enabled && flag2;
                    EditorGUI.PropertyField(rect4, property, GUIContent.none);
                    GUI.enabled = enabled && flag3;
                    EditorGUI.PropertyField(rect5, property3, GUIContent.none);
                    GUI.enabled = enabled;
                    EditorGUI.indentLevel = indentLevel;
                }
            }
        }

        private void DrawAreaListHeader(Rect rect)
        {
            Rect rect2;
            Rect rect3;
            Rect rect4;
            Rect rect5;
            this.GetAreaListRects(rect, out rect2, out rect3, out rect4, out rect5);
            GUI.Label(rect4, s_Styles.m_NameLabel);
            GUI.Label(rect5, s_Styles.m_CostLabel);
        }

        private Color GetAreaColor(int i)
        {
            if (i == 0)
            {
                return new Color(0f, 0.75f, 1f, 0.5f);
            }
            int num = ((this.Bit(i, 4) + (this.Bit(i, 1) * 2)) + 1) * 0x3f;
            int num2 = ((this.Bit(i, 3) + (this.Bit(i, 2) * 2)) + 1) * 0x3f;
            int num3 = ((this.Bit(i, 5) + (this.Bit(i, 0) * 2)) + 1) * 0x3f;
            return new Color(((float) num) / 255f, ((float) num2) / 255f, ((float) num3) / 255f, 0.5f);
        }

        private void GetAreaListRects(Rect rect, out Rect stripeRect, out Rect labelRect, out Rect nameRect, out Rect costRect)
        {
            float num = EditorGUIUtility.singleLineHeight * 0.8f;
            float num2 = EditorGUIUtility.singleLineHeight * 5f;
            float width = EditorGUIUtility.singleLineHeight * 4f;
            float num4 = ((rect.width - num) - num2) - width;
            float x = rect.x;
            stripeRect = new Rect(x, rect.y, num - 4f, rect.height);
            x += num;
            labelRect = new Rect(x, rect.y, num2 - 4f, rect.height);
            x += num2;
            nameRect = new Rect(x, rect.y, num4 - 4f, rect.height);
            x += num4;
            costRect = new Rect(x, rect.y, width, rect.height);
        }

        private static List<GameObject> GetObjects(bool includeChildren)
        {
            if (!includeChildren)
            {
                return new List<GameObject>(Selection.gameObjects);
            }
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject obj2 in Selection.gameObjects)
            {
                list.AddRange(GetObjectsRecurse(obj2));
            }
            return list;
        }

        private static IEnumerable<GameObject> GetObjectsRecurse(GameObject root)
        {
            List<GameObject> list = new List<GameObject> {
                root
            };
            IEnumerator enumerator = root.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    list.AddRange(GetObjectsRecurse(current.gameObject));
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return list;
        }

        private void Init()
        {
            this.m_Object = new SerializedObject(NavMeshBuilder.navMeshSettingsObject);
            this.m_AgentRadius = this.m_Object.FindProperty("m_BuildSettings.agentRadius");
            this.m_AgentHeight = this.m_Object.FindProperty("m_BuildSettings.agentHeight");
            this.m_AgentSlope = this.m_Object.FindProperty("m_BuildSettings.agentSlope");
            this.m_LedgeDropHeight = this.m_Object.FindProperty("m_BuildSettings.ledgeDropHeight");
            this.m_AgentClimb = this.m_Object.FindProperty("m_BuildSettings.agentClimb");
            this.m_MaxJumpAcrossDistance = this.m_Object.FindProperty("m_BuildSettings.maxJumpAcrossDistance");
            this.m_MinRegionArea = this.m_Object.FindProperty("m_BuildSettings.minRegionArea");
            this.m_ManualCellSize = this.m_Object.FindProperty("m_BuildSettings.manualCellSize");
            this.m_CellSize = this.m_Object.FindProperty("m_BuildSettings.cellSize");
            this.m_AccuratePlacement = this.m_Object.FindProperty("m_BuildSettings.accuratePlacement");
            Object serializedAssetInterfaceSingleton = Unsupported.GetSerializedAssetInterfaceSingleton("NavMeshAreas");
            this.m_NavMeshAreasObject = new SerializedObject(serializedAssetInterfaceSingleton);
            this.m_Areas = this.m_NavMeshAreasObject.FindProperty("areas");
            if (((this.m_AreasList == null) && (this.m_NavMeshAreasObject != null)) && (this.m_Areas != null))
            {
                this.m_AreasList = new ReorderableList(this.m_NavMeshAreasObject, this.m_Areas, false, false, false, false);
                this.m_AreasList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawAreaListElement);
                this.m_AreasList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawAreaListHeader);
                this.m_AreasList.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
            }
        }

        private void ModeToggle()
        {
            this.m_Mode = (Mode) GUILayout.Toolbar((int) this.m_Mode, s_Styles.m_ModeToggles, "LargeButton", new GUILayoutOption[0]);
        }

        private static void ObjectSettings()
        {
            GameObject[] objArray;
            bool flag = true;
            Type[] types = new Type[] { typeof(MeshRenderer), typeof(Terrain) };
            SceneModeUtility.SearchBar(types);
            EditorGUILayout.Space();
            MeshRenderer[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<MeshRenderer>(out objArray, new Type[0]);
            if (objArray.Length > 0)
            {
                flag = false;
                ObjectSettings(selectedObjectsOfType, objArray);
            }
            Terrain[] components = SceneModeUtility.GetSelectedObjectsOfType<Terrain>(out objArray, new Type[0]);
            if (objArray.Length > 0)
            {
                flag = false;
                ObjectSettings(components, objArray);
            }
            if (flag)
            {
                GUILayout.Label("Select a MeshRenderer or a Terrain from the scene.", EditorStyles.helpBox, new GUILayoutOption[0]);
            }
        }

        private static void ObjectSettings(Object[] components, GameObject[] gos)
        {
            EditorGUILayout.MultiSelectionObjectTitleBar(components);
            SerializedObject obj2 = new SerializedObject(gos);
            EditorGUI.BeginDisabledGroup(!SceneModeUtility.StaticFlagField("Navigation Static", obj2.FindProperty("m_StaticEditorFlags"), 8));
            SceneModeUtility.StaticFlagField("Generate OffMeshLinks", obj2.FindProperty("m_StaticEditorFlags"), 0x20);
            SerializedProperty property = obj2.FindProperty("m_NavMeshLayer");
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            string[] navMeshAreaNames = GameObjectUtility.GetNavMeshAreaNames();
            int navMeshArea = GameObjectUtility.GetNavMeshArea(gos[0]);
            int selectedIndex = -1;
            for (int i = 0; i < navMeshAreaNames.Length; i++)
            {
                if (GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[i]) == navMeshArea)
                {
                    selectedIndex = i;
                    break;
                }
            }
            int index = EditorGUILayout.Popup("Navigation Area", selectedIndex, navMeshAreaNames, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                int navMeshAreaFromName = GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[index]);
                GameObjectUtility.ShouldIncludeChildren children = GameObjectUtility.DisplayUpdateChildrenDialogIfNeeded(Selection.gameObjects, "Change Navigation Area", "Do you want change the navigation area to " + navMeshAreaNames[index] + " for all the child objects as well?");
                if (children != GameObjectUtility.ShouldIncludeChildren.Cancel)
                {
                    property.intValue = navMeshAreaFromName;
                    SetNavMeshArea(navMeshAreaFromName, children == GameObjectUtility.ShouldIncludeChildren.IncludeChildren);
                }
            }
            EditorGUI.EndDisabledGroup();
            obj2.ApplyModifiedProperties();
        }

        public void OnBecameInvisible()
        {
            NavMeshVisualizationSettings.showNavigation = false;
            RepaintSceneAndGameViews();
        }

        public void OnBecameVisible()
        {
            if (!NavMeshVisualizationSettings.showNavigation)
            {
                NavMeshVisualizationSettings.showNavigation = true;
                RepaintSceneAndGameViews();
            }
        }

        public void OnDisable()
        {
            s_NavMeshEditorWindow = null;
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
        }

        public void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            s_NavMeshEditorWindow = this;
            this.Init();
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            this.UpdateSelectedAgentAndObstacleState();
            base.Repaint();
        }

        public void OnGUI()
        {
            if (this.m_Object.targetObject == null)
            {
                this.Init();
            }
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_Object.Update();
            EditorGUILayout.Space();
            this.ModeToggle();
            EditorGUILayout.Space();
            this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[0]);
            switch (this.m_Mode)
            {
                case Mode.ObjectSettings:
                    ObjectSettings();
                    break;

                case Mode.BakeSettings:
                    this.BakeSettings();
                    break;

                case Mode.AreaSettings:
                    this.AreaSettings();
                    break;
            }
            EditorGUILayout.EndScrollView();
            BakeButtons();
            this.m_Object.ApplyModifiedProperties();
        }

        private void OnInspectorUpdate()
        {
            if (this.m_SelectedNavMeshAgentCount > 0)
            {
                if (this.m_HasPendingAgentDebugInfo != NavMeshVisualizationSettings.hasPendingAgentDebugInfo)
                {
                    if (!this.m_HasRepaintedForPendingAgentDebugInfo)
                    {
                        this.m_HasRepaintedForPendingAgentDebugInfo = true;
                        RepaintSceneAndGameViews();
                    }
                }
                else
                {
                    this.m_HasRepaintedForPendingAgentDebugInfo = false;
                }
            }
        }

        public void OnSceneViewGUI(SceneView sceneView)
        {
            if (NavMeshVisualizationSettings.showNavigation)
            {
                SceneViewOverlay.Window(new GUIContent("Navmesh Display"), new SceneViewOverlay.WindowFunction(NavMeshEditorWindow.DisplayControls), 300, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
                if (this.m_SelectedNavMeshAgentCount > 0)
                {
                    SceneViewOverlay.Window(new GUIContent("Agent Display"), new SceneViewOverlay.WindowFunction(NavMeshEditorWindow.DisplayAgentControls), 300, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
                }
                if (this.m_SelectedNavMeshObstacleCount > 0)
                {
                    SceneViewOverlay.Window(new GUIContent("Obstacle Display"), new SceneViewOverlay.WindowFunction(NavMeshEditorWindow.DisplayObstacleControls), 300, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
                }
            }
        }

        private void OnSelectionChange()
        {
            this.UpdateSelectedAgentAndObstacleState();
            this.m_ScrollPos = Vector2.zero;
            if (this.m_Mode == Mode.ObjectSettings)
            {
                base.Repaint();
            }
        }

        private static void RepaintSceneAndGameViews()
        {
            SceneView.RepaintAll();
            foreach (GameView view in Resources.FindObjectsOfTypeAll(typeof(GameView)))
            {
                view.Repaint();
            }
        }

        private void ResetBakeSettings()
        {
            Unsupported.SmartReset(NavMeshBuilder.navMeshSettingsObject);
        }

        private static bool SelectionHasChildren()
        {
            if (<>f__am$cache17 == null)
            {
                <>f__am$cache17 = obj => obj.transform.childCount > 0;
            }
            return Selection.gameObjects.Any<GameObject>(<>f__am$cache17);
        }

        private static void SetNavMeshArea(int area, bool includeChildren)
        {
            List<GameObject> objects = GetObjects(includeChildren);
            if (objects.Count > 0)
            {
                Undo.RecordObjects(objects.ToArray(), "Change NavMesh area");
                foreach (GameObject obj2 in objects)
                {
                    GameObjectUtility.SetNavMeshArea(obj2, area);
                }
            }
        }

        [MenuItem("Window/Navigation", false, 0x834)]
        public static void SetupWindow()
        {
            Type[] desiredDockNextTo = new Type[] { typeof(InspectorWindow) };
            EditorWindow.GetWindow<NavMeshEditorWindow>(desiredDockNextTo).minSize = new Vector2(300f, 360f);
        }

        private void UpdateSelectedAgentAndObstacleState()
        {
            Object[] filtered = Selection.GetFiltered(typeof(NavMeshAgent), SelectionMode.Editable | SelectionMode.ExcludePrefab);
            Object[] objArray2 = Selection.GetFiltered(typeof(NavMeshObstacle), SelectionMode.Editable | SelectionMode.ExcludePrefab);
            this.m_SelectedNavMeshAgentCount = filtered.Length;
            this.m_SelectedNavMeshObstacleCount = objArray2.Length;
        }

        private enum Mode
        {
            ObjectSettings,
            BakeSettings,
            AreaSettings
        }

        private class Styles
        {
            public readonly GUIContent m_AdvancedHeader = new GUIContent("Advanced");
            public readonly GUIContent m_AgentClimbContent = EditorGUIUtility.TextContent("Step Height|The height of discontinuities in the level the agent can climb over (i.e. steps and stairs).");
            public readonly GUIContent m_AgentDropContent = EditorGUIUtility.TextContent("Drop Height|Maximum agent drop height.");
            public readonly GUIContent m_AgentHeightContent = EditorGUIUtility.TextContent("Agent Height|How much vertical clearance space must exist.");
            public readonly GUIContent m_AgentJumpContent = EditorGUIUtility.TextContent("Jump Distance|Maximum agent jump distance.");
            public readonly GUIContent m_AgentPlacementContent = EditorGUIUtility.TextContent("Height Mesh|Generate an accurate height mesh for precise agent placement (slower).");
            public readonly GUIContent m_AgentRadiusContent = EditorGUIUtility.TextContent("Agent Radius|How close to the walls navigation mesh exist.");
            public readonly GUIContent m_AgentSizeHeader = new GUIContent("Baked Agent Size");
            public readonly GUIContent m_AgentSlopeContent = EditorGUIUtility.TextContent("Max Slope|Maximum slope the agent can walk up.");
            public readonly GUIContent m_CellSizeContent = EditorGUIUtility.TextContent("Voxel Size|Specifies at the voxelization resolution at which the NavMesh is build.");
            public readonly GUIContent m_CostLabel = new GUIContent("Cost");
            public readonly GUIContent m_ManualCellSizeContent = EditorGUIUtility.TextContent("Manual Voxel Size|Enable to set voxel size manually.");
            public readonly GUIContent m_MinRegionAreaContent = EditorGUIUtility.TextContent("Min Region Area|Minimum area that a navmesh region can be.");
            public readonly GUIContent[] m_ModeToggles = new GUIContent[] { EditorGUIUtility.TextContent("Object|Bake settings for the currently selected object."), EditorGUIUtility.TextContent("Bake|Navmesh bake settings."), EditorGUIUtility.TextContent("Areas|Navmesh area settings.") };
            public readonly GUIContent m_NameLabel = new GUIContent("Name");
            public readonly GUIContent m_OffmeshHeader = new GUIContent("Generated Off Mesh Links");
        }
    }
}

