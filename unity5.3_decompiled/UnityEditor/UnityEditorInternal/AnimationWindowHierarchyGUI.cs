namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationWindowHierarchyGUI : TreeViewGUI
    {
        public const float k_AddCurveButtonNodeHeight = 40f;
        private readonly GUIContent k_AnimatePropertyLabel;
        private const float k_ColorIndicatorTopMargin = 3f;
        public const float k_DopeSheetRowHeight = 16f;
        public const float k_DopeSheetRowHeightTall = 32f;
        private static readonly Color k_KeyColorForNonCurves = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        private static readonly Color k_KeyColorInDopesheetMode = new Color(0.7f, 0.7f, 0.7f, 1f);
        private static readonly Color k_LeftoverCurveColor = Color.yellow;
        public const float k_RowBackgroundColorBrightness = 0.28f;
        private const float k_RowRightOffset = 10f;
        private const float k_ValueFieldOffsetFromRightSide = 75f;
        private const float k_ValueFieldWidth = 50f;
        private GUIStyle m_AnimationCurveDropdown;
        private GUIStyle m_AnimationLineStyle;
        private GUIStyle m_AnimationRowEvenStyle;
        private GUIStyle m_AnimationRowOddStyle;
        private GUIStyle m_AnimationSelectionTextField;
        private Color m_LightSkinPropertyTextColor;
        private GUIStyle m_PlusButtonStyle;
        private AnimationWindowHierarchyNode m_RenamedNode;
        internal static int s_WasInsideValueRectFrame = -1;

        public AnimationWindowHierarchyGUI(TreeView treeView, AnimationWindowState state) : base(treeView)
        {
            this.k_AnimatePropertyLabel = new GUIContent("Add Property");
            this.m_LightSkinPropertyTextColor = new Color(0.35f, 0.35f, 0.35f);
            this.state = state;
        }

        private void AddKeysAtCurrentTime(List<AnimationWindowCurve> curves)
        {
            foreach (AnimationWindowCurve curve in curves)
            {
                AnimationWindowUtility.AddKeyframeToCurve(this.state, curve, this.state.time);
            }
        }

        private void AddKeysAtCurrentTime(object obj)
        {
            this.AddKeysAtCurrentTime((List<AnimationWindowCurve>) obj);
        }

        public override bool BeginRename(TreeViewItem item, float delay)
        {
            this.m_RenamedNode = item as AnimationWindowHierarchyNode;
            return base.GetRenameOverlay().BeginRename(this.GetGameObjectName(this.m_RenamedNode.path), item.id, delay);
        }

        public override void BeginRowGUI()
        {
            base.BeginRowGUI();
            this.HandleDelete();
        }

        private void ChangeRotationInterpolation(object interpolationMode)
        {
            RotationCurveInterpolation.Mode newInterpolationMode = (RotationCurveInterpolation.Mode) ((int) interpolationMode);
            AnimationWindowCurve[] curveArray = this.state.activeCurves.ToArray();
            EditorCurveBinding[] curveBindings = new EditorCurveBinding[curveArray.Length];
            for (int i = 0; i < curveArray.Length; i++)
            {
                curveBindings[i] = curveArray[i].binding;
            }
            RotationCurveInterpolation.SetInterpolation(this.state.activeAnimationClip, curveBindings, newInterpolationMode);
            this.MaintainTreeviewStateAfterRotationInterpolation(newInterpolationMode);
            this.state.hierarchyData.ReloadData();
        }

        private void DeleteKeysAtCurrentTime(List<AnimationWindowCurve> curves)
        {
            foreach (AnimationWindowCurve curve in curves)
            {
                curve.RemoveKeyframe(this.state.time);
                this.state.SaveCurve(curve);
            }
        }

        private void DeleteKeysAtCurrentTime(object obj)
        {
            this.DeleteKeysAtCurrentTime((List<AnimationWindowCurve>) obj);
        }

        private void DoAddCurveButton(Rect rect)
        {
            float num2 = (rect.width - 230f) / 2f;
            float num3 = 10f;
            Rect position = new Rect(rect.xMin + num2, rect.yMin + num3, rect.width - (num2 * 2f), rect.height - (num3 * 2f));
            if (GUI.Button(position, this.k_AnimatePropertyLabel))
            {
                AddCurvesPopup.gameObject = this.state.activeRootGameObject;
                AddCurvesPopupHierarchyDataSource.showEntireHierarchy = true;
                if (AddCurvesPopup.ShowAtPosition(position, this.state))
                {
                    GUIUtility.ExitGUI();
                }
            }
        }

        private void DoCurveColorIndicator(Rect rect, AnimationWindowHierarchyNode node)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color = GUI.color;
                if (!this.state.showCurveEditor)
                {
                    GUI.color = k_KeyColorInDopesheetMode;
                }
                else if ((node.curves.Length == 1) && !node.curves[0].isPPtrCurve)
                {
                    GUI.color = CurveUtility.GetPropertyColor(node.curves[0].binding.propertyName);
                }
                else
                {
                    GUI.color = k_KeyColorForNonCurves;
                }
                bool flag = false;
                if (AnimationMode.InAnimationMode())
                {
                    foreach (AnimationWindowCurve curve in node.curves)
                    {
                        if (curve.m_Keyframes.Any<AnimationWindowKeyframe>(key => this.state.time.ContainsTime(key.time)))
                        {
                            flag = true;
                        }
                    }
                }
                Texture image = !flag ? CurveUtility.GetIconCurve() : CurveUtility.GetIconKey();
                rect = new Rect(((rect.xMax - 10f) - (image.width / 2)) - 5f, rect.yMin + 3f, (float) image.width, (float) image.height);
                GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit, true, 1f);
                GUI.color = color;
            }
        }

        private void DoCurveDropdown(Rect rect, AnimationWindowHierarchyNode node)
        {
            rect = new Rect((rect.xMax - 10f) - 12f, rect.yMin + 2f, 22f, 12f);
            if (GUI.Button(rect, GUIContent.none, this.m_AnimationCurveDropdown))
            {
                this.state.SelectHierarchyItem(node.id, false, false);
                AnimationWindowHierarchyNode[] source = new AnimationWindowHierarchyNode[] { node };
                this.GenerateMenu(source.ToList<AnimationWindowHierarchyNode>()).DropDown(rect);
                Event.current.Use();
            }
        }

        private void DoFoldout(AnimationWindowHierarchyNode node, Rect rect, float indent)
        {
            if (base.m_TreeView.data.IsExpandable(node))
            {
                Rect position = rect;
                position.x = indent;
                position.width = base.k_FoldoutWidth;
                EditorGUI.BeginChangeCheck();
                bool expand = GUI.Toggle(position, base.m_TreeView.data.IsExpanded(node), GUIContent.none, TreeViewGUI.s_Styles.foldout);
                if (EditorGUI.EndChangeCheck())
                {
                    if (Event.current.alt)
                    {
                        base.m_TreeView.data.SetExpandedWithChildren(node, expand);
                    }
                    else
                    {
                        base.m_TreeView.data.SetExpanded(node, expand);
                    }
                    if (expand)
                    {
                        base.m_TreeView.UserExpandedNode(node);
                    }
                }
            }
            else
            {
                AnimationWindowHierarchyPropertyNode node2 = node as AnimationWindowHierarchyPropertyNode;
                AnimationWindowHierarchyState state = base.m_TreeView.state as AnimationWindowHierarchyState;
                if ((node2 != null) && node2.isPptrNode)
                {
                    Rect rect3 = rect;
                    rect3.x = indent;
                    rect3.width = base.k_FoldoutWidth;
                    EditorGUI.BeginChangeCheck();
                    bool tallMode = state.GetTallMode(node2);
                    tallMode = GUI.Toggle(rect3, tallMode, GUIContent.none, TreeViewGUI.s_Styles.foldout);
                    if (EditorGUI.EndChangeCheck())
                    {
                        state.SetTallMode(node2, tallMode);
                    }
                }
            }
        }

        private void DoIconAndName(Rect rect, AnimationWindowHierarchyNode node, bool selected, bool focused, float indent)
        {
            EditorGUIUtility.SetIconSize(new Vector2(13f, 13f));
            if (Event.current.type == EventType.Repaint)
            {
                if (selected)
                {
                    TreeViewGUI.s_Styles.selectionStyle.Draw(rect, false, false, true, focused);
                }
                if (AnimationMode.InAnimationMode())
                {
                    rect.width -= 77f;
                }
                bool flag = AnimationWindowUtility.IsNodeLeftOverCurve(node, this.state.activeRootGameObject);
                bool flag2 = AnimationWindowUtility.IsNodeAmbiguous(node, this.state.activeRootGameObject);
                string str = string.Empty;
                string tooltip = string.Empty;
                if (flag)
                {
                    str = " (Missing!)";
                    tooltip = "The GameObject or Component is missing (" + node.path + ")";
                }
                if (flag2)
                {
                    str = " (Duplicate GameObject name!)";
                    tooltip = "Target for curve is ambiguous since there are multiple GameObjects with same name (" + node.path + ")";
                }
                if (node.depth == 0)
                {
                    if ((this.state.activeRootGameObject != null) && (this.state.activeRootGameObject.transform.Find(node.path) == null))
                    {
                        flag = true;
                    }
                    TreeViewGUI.s_Styles.content = new GUIContent(this.GetGameObjectName(node.path) + " : " + node.displayName + str, this.GetIconForNode(node), tooltip);
                    Color textColor = this.m_AnimationLineStyle.normal.textColor;
                    Color color = !EditorGUIUtility.isProSkin ? Color.black : ((Color) (Color.gray * 1.35f));
                    color = (!flag && !flag2) ? color : k_LeftoverCurveColor;
                    this.SetStyleTextColor(this.m_AnimationLineStyle, color);
                    rect.xMin += (int) (indent + base.k_FoldoutWidth);
                    GUI.Label(rect, TreeViewGUI.s_Styles.content, this.m_AnimationLineStyle);
                    this.SetStyleTextColor(this.m_AnimationLineStyle, textColor);
                }
                else
                {
                    TreeViewGUI.s_Styles.content = new GUIContent(node.displayName + str, this.GetIconForNode(node), tooltip);
                    Color color3 = this.m_AnimationLineStyle.normal.textColor;
                    Color color4 = !EditorGUIUtility.isProSkin ? this.m_LightSkinPropertyTextColor : Color.gray;
                    color4 = (!flag && !flag2) ? color4 : k_LeftoverCurveColor;
                    this.SetStyleTextColor(this.m_AnimationLineStyle, color4);
                    rect.xMin += (int) ((indent + base.k_IndentWidth) + base.k_FoldoutWidth);
                    GUI.Label(rect, TreeViewGUI.s_Styles.content, this.m_AnimationLineStyle);
                    this.SetStyleTextColor(this.m_AnimationLineStyle, color3);
                }
            }
            if (this.IsRenaming(node.id) && (Event.current.type != EventType.Layout))
            {
                base.GetRenameOverlay().editFieldRect = new Rect(rect.x + base.k_IndentWidth, rect.y, (rect.width - base.k_IndentWidth) - 1f, rect.height);
            }
        }

        protected void DoNodeGUI(Rect rect, AnimationWindowHierarchyNode node, bool selected, bool focused, int row)
        {
            this.InitStyles();
            if (!(node is AnimationWindowHierarchyMasterNode))
            {
                float indent = base.k_BaseIndent + ((node.depth + node.indent) * base.k_IndentWidth);
                if (node is AnimationWindowHierarchyAddButtonNode)
                {
                    if ((Event.current.type == EventType.MouseMove) && (s_WasInsideValueRectFrame >= 0))
                    {
                        if (s_WasInsideValueRectFrame >= (Time.frameCount - 1))
                        {
                            Event.current.Use();
                        }
                        else
                        {
                            s_WasInsideValueRectFrame = -1;
                        }
                    }
                    bool flag = (this.state.activeGameObject != null) && AnimationWindowUtility.GameObjectIsAnimatable(this.state.activeGameObject, this.state.activeAnimationClip);
                    EditorGUI.BeginDisabledGroup(!flag);
                    this.DoAddCurveButton(rect);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    this.DoRowBackground(rect, row);
                    this.DoIconAndName(rect, node, selected, focused, indent);
                    this.DoFoldout(node, rect, indent);
                    EditorGUI.BeginDisabledGroup(this.state.animationIsReadOnly);
                    this.DoValueField(rect, node, row);
                    this.HandleContextMenu(rect, node);
                    EditorGUI.EndDisabledGroup();
                    this.DoCurveDropdown(rect, node);
                    this.DoCurveColorIndicator(rect, node);
                }
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }
        }

        private void DoRowBackground(Rect rect, int row)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if ((row % 2) == 0)
                {
                    this.m_AnimationRowEvenStyle.Draw(rect, false, false, false, false);
                }
                else
                {
                    this.m_AnimationRowOddStyle.Draw(rect, false, false, false, false);
                }
            }
        }

        private void DoValueField(Rect rect, AnimationWindowHierarchyNode node, int row)
        {
            bool flag = false;
            if (AnimationMode.InAnimationMode())
            {
                EditorGUI.BeginDisabledGroup(this.state.animationIsReadOnly);
                if (node is AnimationWindowHierarchyPropertyNode)
                {
                    List<AnimationWindowCurve> curves = this.state.GetCurves(node, false);
                    if ((curves == null) || (curves.Count == 0))
                    {
                        return;
                    }
                    AnimationWindowCurve curve = curves[0];
                    object currentValue = CurveBindingUtility.GetCurrentValue(this.state.activeRootGameObject, curve.binding);
                    Type editorCurveValueType = CurveBindingUtility.GetEditorCurveValueType(this.state.activeRootGameObject, curve.binding);
                    if (currentValue is float)
                    {
                        float num = (float) currentValue;
                        Rect position = new Rect(rect.xMax - 75f, rect.y, 50f, rect.height);
                        if ((Event.current.type == EventType.MouseMove) && position.Contains(Event.current.mousePosition))
                        {
                            s_WasInsideValueRectFrame = Time.frameCount;
                        }
                        EditorGUI.BeginChangeCheck();
                        if (editorCurveValueType == typeof(bool))
                        {
                            num = !EditorGUI.Toggle(position, num != 0f) ? ((float) 0) : ((float) 1);
                        }
                        else
                        {
                            int id = GUIUtility.GetControlID(0x75bcc20, FocusType.Keyboard, position);
                            bool flag2 = (((GUIUtility.keyboardControl == id) && EditorGUIUtility.editingTextField) && (Event.current.type == EventType.KeyDown)) && ((Event.current.character == '\n') || (Event.current.character == '\x0003'));
                            num = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, new Rect(0f, 0f, 0f, 0f), id, num, EditorGUI.kFloatFieldFormatString, this.m_AnimationSelectionTextField, false);
                            if (flag2)
                            {
                                GUI.changed = true;
                                Event.current.Use();
                            }
                        }
                        if (float.IsInfinity(num) || float.IsNaN(num))
                        {
                            num = 0f;
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            AnimationWindowKeyframe keyframe = null;
                            foreach (AnimationWindowKeyframe keyframe2 in curve.m_Keyframes)
                            {
                                if (Mathf.Approximately(keyframe2.time, this.state.time.time))
                                {
                                    keyframe = keyframe2;
                                }
                            }
                            if (keyframe == null)
                            {
                                AnimationWindowUtility.AddKeyframeToCurve(curve, num, editorCurveValueType, this.state.time);
                            }
                            else
                            {
                                keyframe.value = num;
                            }
                            this.state.SaveCurve(curve);
                            flag = true;
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
                if (flag)
                {
                    this.state.ResampleAnimation();
                }
            }
        }

        private GenericMenu GenerateMenu(List<AnimationWindowHierarchyNode> interactedNodes)
        {
            List<AnimationWindowCurve> curvesAffectedByNodes = this.GetCurvesAffectedByNodes(interactedNodes, false);
            List<AnimationWindowCurve> list2 = this.GetCurvesAffectedByNodes(interactedNodes, true);
            bool flag = (curvesAffectedByNodes.Count == 1) && AnimationWindowUtility.ForceGrouping(curvesAffectedByNodes[0].binding);
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(((curvesAffectedByNodes.Count <= 1) && !flag) ? "Remove Property" : "Remove Properties"), false, new GenericMenu.MenuFunction(this.RemoveCurvesFromSelectedNodes));
            bool flag2 = true;
            EditorCurveBinding[] curves = new EditorCurveBinding[list2.Count];
            for (int i = 0; i < list2.Count; i++)
            {
                curves[i] = list2[i].binding;
            }
            RotationCurveInterpolation.Mode rotationInterpolationMode = this.GetRotationInterpolationMode(curves);
            if (rotationInterpolationMode == RotationCurveInterpolation.Mode.Undefined)
            {
                flag2 = false;
            }
            else
            {
                foreach (AnimationWindowHierarchyNode node in interactedNodes)
                {
                    if (!(node is AnimationWindowHierarchyPropertyGroupNode))
                    {
                        flag2 = false;
                    }
                }
            }
            if (flag2)
            {
                menu.AddItem(new GUIContent("Interpolation/Euler Angles"), rotationInterpolationMode == RotationCurveInterpolation.Mode.RawEuler, new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.RawEuler);
                menu.AddItem(new GUIContent("Interpolation/Euler Angles (Quaternion Approximation)"), rotationInterpolationMode == RotationCurveInterpolation.Mode.Baked, new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.Baked);
                menu.AddItem(new GUIContent("Interpolation/Quaternion"), rotationInterpolationMode == RotationCurveInterpolation.Mode.NonBaked, new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.NonBaked);
            }
            if (AnimationMode.InAnimationMode())
            {
                menu.AddSeparator(string.Empty);
                bool flag3 = true;
                bool flag4 = true;
                bool flag5 = true;
                foreach (AnimationWindowCurve curve in curvesAffectedByNodes)
                {
                    if (!curve.HasKeyframe(this.state.time))
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag4 = false;
                        if (!curve.isPPtrCurve)
                        {
                            flag5 = false;
                        }
                    }
                }
                string text = "Add Key";
                if (flag3)
                {
                    menu.AddDisabledItem(new GUIContent(text));
                }
                else
                {
                    menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.AddKeysAtCurrentTime), curvesAffectedByNodes);
                }
                text = "Delete Key";
                if (flag4)
                {
                    menu.AddDisabledItem(new GUIContent(text));
                }
                else
                {
                    menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.DeleteKeysAtCurrentTime), curvesAffectedByNodes);
                }
                if (flag5)
                {
                    return menu;
                }
                menu.AddSeparator(string.Empty);
                List<KeyIdentifier> list3 = new List<KeyIdentifier>();
                foreach (AnimationWindowCurve curve2 in curvesAffectedByNodes)
                {
                    if (!curve2.isPPtrCurve)
                    {
                        int keyframeIndex = curve2.GetKeyframeIndex(this.state.time);
                        if (keyframeIndex != -1)
                        {
                            CurveRenderer curveRenderer = CurveRendererCache.GetCurveRenderer(this.state.activeAnimationClip, curve2.binding);
                            int curveID = CurveUtility.GetCurveID(this.state.activeAnimationClip, curve2.binding);
                            list3.Add(new KeyIdentifier(curveRenderer, curveID, keyframeIndex));
                        }
                    }
                }
            }
            return menu;
        }

        private List<AnimationWindowCurve> GetCurvesAffectedByNodes(List<AnimationWindowHierarchyNode> nodes, bool includeLinkedCurves)
        {
            List<AnimationWindowCurve> source = new List<AnimationWindowCurve>();
            foreach (AnimationWindowHierarchyNode node in nodes)
            {
                AnimationWindowHierarchyNode parent = node;
                if ((parent.parent is AnimationWindowHierarchyPropertyGroupNode) && includeLinkedCurves)
                {
                    parent = (AnimationWindowHierarchyNode) parent.parent;
                }
                if ((parent is AnimationWindowHierarchyPropertyGroupNode) || (parent is AnimationWindowHierarchyPropertyNode))
                {
                    source.AddRange(AnimationWindowUtility.FilterCurves(this.state.allCurves.ToArray(), parent.path, parent.animatableObjectType, parent.propertyName));
                }
                else
                {
                    source.AddRange(AnimationWindowUtility.FilterCurves(this.state.allCurves.ToArray(), parent.path, parent.animatableObjectType));
                }
            }
            return source.Distinct<AnimationWindowCurve>().ToList<AnimationWindowCurve>();
        }

        public override void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
        {
            firstRowVisible = 0;
            lastRowVisible = base.m_TreeView.data.rowCount - 1;
        }

        private string GetGameObjectName(string path)
        {
            if (string.IsNullOrEmpty(path) && (this.state.activeRootGameObject != null))
            {
                return this.state.activeRootGameObject.name;
            }
            char[] separator = new char[] { '/' };
            string[] strArray = path.Split(separator);
            return strArray[strArray.Length - 1];
        }

        protected override Texture GetIconForNode(TreeViewItem item)
        {
            if (item != null)
            {
                return item.icon;
            }
            return null;
        }

        public float GetNodeHeight(AnimationWindowHierarchyNode node)
        {
            if (node is AnimationWindowHierarchyAddButtonNode)
            {
                return 40f;
            }
            AnimationWindowHierarchyState state = base.m_TreeView.state as AnimationWindowHierarchyState;
            return (!state.GetTallMode(node) ? 16f : 32f);
        }

        private string GetPathWithoutChildmostGameObject(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            int num = path.LastIndexOf('/');
            return path.Substring(0, num + 1);
        }

        private RotationCurveInterpolation.Mode GetRotationInterpolationMode(EditorCurveBinding[] curves)
        {
            if ((curves == null) || (curves.Length == 0))
            {
                return RotationCurveInterpolation.Mode.Undefined;
            }
            RotationCurveInterpolation.Mode modeFromCurveData = RotationCurveInterpolation.GetModeFromCurveData(curves[0]);
            for (int i = 1; i < curves.Length; i++)
            {
                RotationCurveInterpolation.Mode mode2 = RotationCurveInterpolation.GetModeFromCurveData(curves[i]);
                if (modeFromCurveData != mode2)
                {
                    return RotationCurveInterpolation.Mode.Undefined;
                }
            }
            return modeFromCurveData;
        }

        public override Rect GetRowRect(int row, float rowWidth)
        {
            List<TreeViewItem> rows = base.m_TreeView.data.GetRows();
            AnimationWindowHierarchyNode node = rows[row] as AnimationWindowHierarchyNode;
            if (!node.topPixel.HasValue)
            {
                node.topPixel = new float?(this.GetTopPixelOfRow(row, rows));
            }
            return new Rect(0f, node.topPixel.Value, rowWidth, this.GetNodeHeight(node));
        }

        private float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
        {
            float num = 0f;
            for (int i = 0; (i < row) && (i < rows.Count); i++)
            {
                AnimationWindowHierarchyNode node = rows[i] as AnimationWindowHierarchyNode;
                num += this.GetNodeHeight(node);
            }
            return num;
        }

        public override Vector2 GetTotalSize()
        {
            List<TreeViewItem> rows = base.m_TreeView.data.GetRows();
            float y = 0f;
            for (int i = 0; i < rows.Count; i++)
            {
                AnimationWindowHierarchyNode node = rows[i] as AnimationWindowHierarchyNode;
                y += this.GetNodeHeight(node);
            }
            return new Vector2(1f, y);
        }

        private void HandleContextMenu(Rect rect, AnimationWindowHierarchyNode node)
        {
            if ((Event.current.type == EventType.ContextClick) && rect.Contains(Event.current.mousePosition))
            {
                this.state.SelectHierarchyItem(node.id, false, true);
                this.GenerateMenu(this.state.selectedHierarchyNodes).ShowAsContext();
                Event.current.Use();
            }
        }

        private void HandleDelete()
        {
            if (base.m_TreeView.HasFocus())
            {
                EventType type = Event.current.type;
                if (type != EventType.KeyDown)
                {
                    if ((type == EventType.ExecuteCommand) && ((Event.current.commandName == "SoftDelete") || (Event.current.commandName == "Delete")))
                    {
                        if (Event.current.type == EventType.ExecuteCommand)
                        {
                            this.RemoveCurvesFromSelectedNodes();
                        }
                        Event.current.Use();
                    }
                }
                else if ((Event.current.keyCode == KeyCode.Backspace) || (Event.current.keyCode == KeyCode.Delete))
                {
                    this.RemoveCurvesFromSelectedNodes();
                    Event.current.Use();
                }
            }
        }

        protected override void InitStyles()
        {
            base.InitStyles();
            if (this.m_PlusButtonStyle == null)
            {
                this.m_PlusButtonStyle = new GUIStyle("OL Plus");
            }
            if (this.m_AnimationRowEvenStyle == null)
            {
                this.m_AnimationRowEvenStyle = new GUIStyle("AnimationRowEven");
            }
            if (this.m_AnimationRowOddStyle == null)
            {
                this.m_AnimationRowOddStyle = new GUIStyle("AnimationRowOdd");
            }
            if (this.m_AnimationSelectionTextField == null)
            {
                this.m_AnimationSelectionTextField = new GUIStyle("AnimationSelectionTextField");
            }
            if (this.m_AnimationLineStyle == null)
            {
                this.m_AnimationLineStyle = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
                this.m_AnimationLineStyle.padding.left = 0;
            }
            if (this.m_AnimationCurveDropdown == null)
            {
                this.m_AnimationCurveDropdown = new GUIStyle("AnimPropDropdown");
            }
        }

        private void MaintainTreeviewStateAfterRotationInterpolation(RotationCurveInterpolation.Mode newMode)
        {
            List<int> selectedIDs = this.state.hierarchyState.selectedIDs;
            List<int> expandedIDs = this.state.hierarchyState.expandedIDs;
            List<int> list3 = new List<int>();
            List<int> list4 = new List<int>();
            for (int i = 0; i < selectedIDs.Count; i++)
            {
                AnimationWindowHierarchyNode node = this.state.hierarchyData.FindItem(selectedIDs[i]) as AnimationWindowHierarchyNode;
                if ((node != null) && !node.propertyName.Equals(RotationCurveInterpolation.GetPrefixForInterpolation(newMode)))
                {
                    char[] separator = new char[] { '.' };
                    string oldValue = node.propertyName.Split(separator)[0];
                    string str2 = node.propertyName.Replace(oldValue, RotationCurveInterpolation.GetPrefixForInterpolation(newMode));
                    list3.Add(selectedIDs[i]);
                    list4.Add((node.path + node.animatableObjectType.Name + str2).GetHashCode());
                }
            }
            for (int j = 0; j < list3.Count; j++)
            {
                if (selectedIDs.Contains(list3[j]))
                {
                    int index = selectedIDs.IndexOf(list3[j]);
                    selectedIDs[index] = list4[j];
                }
                if (expandedIDs.Contains(list3[j]))
                {
                    int num4 = expandedIDs.IndexOf(list3[j]);
                    expandedIDs[num4] = list4[j];
                }
                if (this.state.hierarchyState.lastClickedID == list3[j])
                {
                    this.state.hierarchyState.lastClickedID = list4[j];
                }
            }
            this.state.hierarchyState.selectedIDs = new List<int>(selectedIDs);
            this.state.hierarchyState.expandedIDs = new List<int>(expandedIDs);
        }

        public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
        {
            AnimationWindowHierarchyNode node2 = node as AnimationWindowHierarchyNode;
            this.DoNodeGUI(rowRect, node2, selected, focused, row);
        }

        private void RemoveCurvesFromNodes(List<AnimationWindowHierarchyNode> nodes)
        {
            foreach (AnimationWindowHierarchyNode node in nodes)
            {
                AnimationWindowHierarchyNode parent = node;
                if (((parent.parent is AnimationWindowHierarchyPropertyGroupNode) && parent.binding.HasValue) && AnimationWindowUtility.ForceGrouping(parent.binding.Value))
                {
                    parent = (AnimationWindowHierarchyNode) parent.parent;
                }
                List<AnimationWindowCurve> list = null;
                if ((parent is AnimationWindowHierarchyPropertyGroupNode) || (parent is AnimationWindowHierarchyPropertyNode))
                {
                    list = AnimationWindowUtility.FilterCurves(this.state.allCurves.ToArray(), parent.path, parent.animatableObjectType, parent.propertyName);
                }
                else
                {
                    list = AnimationWindowUtility.FilterCurves(this.state.allCurves.ToArray(), parent.path, parent.animatableObjectType);
                }
                foreach (AnimationWindowCurve curve in list)
                {
                    this.state.RemoveCurve(curve);
                }
            }
            base.m_TreeView.ReloadData();
        }

        private void RemoveCurvesFromSelectedNodes()
        {
            this.RemoveCurvesFromNodes(this.state.selectedHierarchyNodes);
        }

        protected override void RenameEnded()
        {
            string name = base.GetRenameOverlay().name;
            string originalName = base.GetRenameOverlay().originalName;
            if (name != originalName)
            {
                foreach (AnimationWindowCurve curve in this.m_RenamedNode.curves)
                {
                    string newPath = this.RenamePath(curve.path, name);
                    EditorCurveBinding renamedBinding = AnimationWindowUtility.GetRenamedBinding(curve.binding, newPath);
                    if (AnimationWindowUtility.CurveExists(renamedBinding, this.state.allCurves.ToArray()))
                    {
                        Debug.LogWarning("Curve already exists, renaming cancelled.");
                    }
                    else
                    {
                        AnimationWindowUtility.RenameCurvePath(curve, renamedBinding, this.state.activeAnimationClip);
                    }
                }
            }
            this.m_RenamedNode = null;
        }

        private string RenamePath(string oldPath, string newGameObjectName)
        {
            if (oldPath.Length <= 0)
            {
                return newGameObjectName;
            }
            string pathWithoutChildmostGameObject = this.GetPathWithoutChildmostGameObject(oldPath);
            if (pathWithoutChildmostGameObject.Length > 0)
            {
                pathWithoutChildmostGameObject = pathWithoutChildmostGameObject + "/";
            }
            return (pathWithoutChildmostGameObject + newGameObjectName);
        }

        private void SetStyleTextColor(GUIStyle style, Color color)
        {
            style.normal.textColor = color;
            style.focused.textColor = color;
            style.active.textColor = color;
            style.hover.textColor = color;
        }

        protected override void SyncFakeItem()
        {
        }

        public AnimationWindowState state { get; set; }
    }
}

