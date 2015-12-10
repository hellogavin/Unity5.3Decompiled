namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PresetLibraryEditor<T> where T: PresetLibrary
    {
        public Action<PresetLibrary> addDefaultPresets;
        private const float kCheckoutButtonMargin = 2f;
        private const float kCheckoutButtonMaxWidth = 100f;
        private const float kGridLabelHeight = 16f;
        private DragState<T> m_DragState;
        private readonly VerticalGrid m_Grid;
        private bool m_IsOpenForEdit;
        private readonly Action<int, object> m_ItemClickedCallback;
        private Vector2 m_MinMaxPreviewHeight;
        private PresetFileLocation m_PresetLibraryFileLocation;
        private float m_PreviewAspect;
        private readonly ScriptableObjectSaveLoadHelper<T> m_SaveLoadHelper;
        private bool m_ShowAddNewPresetItem;
        private bool m_ShowedScrollBarLastFrame;
        private readonly PresetLibraryEditorState m_State;
        public Action presetsWasReordered;
        private static Styles<T> s_Styles;

        public PresetLibraryEditor(ScriptableObjectSaveLoadHelper<T> helper, PresetLibraryEditorState state, Action<int, object> itemClickedCallback)
        {
            this.m_DragState = new DragState<T>();
            this.m_Grid = new VerticalGrid();
            this.m_MinMaxPreviewHeight = new Vector2(14f, 64f);
            this.m_PreviewAspect = 8f;
            this.m_ShowAddNewPresetItem = true;
            this.m_IsOpenForEdit = true;
            this.m_SaveLoadHelper = helper;
            this.m_State = state;
            this.m_ItemClickedCallback = itemClickedCallback;
            this.settingsMenuRightMargin = 10f;
            this.useOnePixelOverlappedGrid = false;
            this.alwaysShowScrollAreaHorizontalLines = true;
            this.marginsForList = new RectOffset(10, 10, 5, 5);
            this.marginsForGrid = new RectOffset(5, 5, 5, 5);
            this.m_PresetLibraryFileLocation = PresetLibraryLocations.GetFileLocationFromPath(this.currentLibraryWithoutExtension);
        }

        private void BeginRenaming(string name, int itemIndex, float delay)
        {
            this.GetRenameOverlay().BeginRename(name, itemIndex, delay);
        }

        private void ClearDragState()
        {
            this.m_DragState.dragUponIndex = -1;
            this.m_DragState.draggingIndex = -1;
        }

        public T CreateNewLibrary(string presetLibraryPathWithoutExtension)
        {
            T library = ScriptableSingleton<PresetLibraryManager>.instance.CreateLibrary<T>(this.m_SaveLoadHelper, presetLibraryPathWithoutExtension);
            if (library != null)
            {
                ScriptableSingleton<PresetLibraryManager>.instance.SaveLibrary<T>(this.m_SaveLoadHelper, library, presetLibraryPathWithoutExtension);
                InternalEditorUtility.RepaintAllViews();
            }
            return library;
        }

        private string CreateNewLibraryCallback(string libraryName, PresetFileLocation fileLocation)
        {
            string presetLibraryPathWithoutExtension = Path.Combine(PresetLibraryLocations.GetDefaultFilePathForFileLocation(fileLocation), libraryName);
            if (this.CreateNewLibrary(presetLibraryPathWithoutExtension) != null)
            {
                this.currentLibraryWithoutExtension = presetLibraryPathWithoutExtension;
            }
            return ScriptableSingleton<PresetLibraryManager>.instance.GetLastError();
        }

        public int CreateNewPreset(object presetObject, string presetName)
        {
            T currentLib = this.GetCurrentLib();
            if (currentLib == null)
            {
                Debug.Log("No current library selected!");
                return -1;
            }
            currentLib.Add(presetObject, presetName);
            this.SaveCurrentLib();
            if (this.presetsWasReordered != null)
            {
                this.presetsWasReordered();
            }
            this.Repaint();
            this.OnLayoutChanged();
            return (currentLib.Count() - 1);
        }

        private void CreateNewPresetButton(Rect buttonRect, object newPresetObject, PresetLibrary lib, bool isOpenForEdit)
        {
            EditorGUI.BeginDisabledGroup(!isOpenForEdit);
            if (GUI.Button(buttonRect, !isOpenForEdit ? PresetLibraryEditor<T>.s_Styles.plusButtonTextNotCheckedOut : PresetLibraryEditor<T>.s_Styles.plusButtonText))
            {
                int itemIndex = this.CreateNewPreset(newPresetObject, string.Empty);
                if (this.drawLabels)
                {
                    this.BeginRenaming(string.Empty, itemIndex, 0f);
                }
                InspectorWindow.RepaintAllInspectors();
            }
            if (Event.current.type == EventType.Repaint)
            {
                Rect rect = new RectOffset(-3, -3, -3, -3).Add(buttonRect);
                lib.Draw(rect, newPresetObject);
                if (buttonRect.width > 30f)
                {
                    PresetLibraryEditor<T>.LabelWithOutline(buttonRect, PresetLibraryEditor<T>.s_Styles.newPreset, new Color(0.1f, 0.1f, 0.1f), PresetLibraryEditor<T>.s_Styles.newPresetStyle);
                }
                else if ((lib.Count() == 0) && isOpenForEdit)
                {
                    buttonRect.x = buttonRect.xMax + 5f;
                    buttonRect.width = 200f;
                    buttonRect.height = 16f;
                    EditorGUI.BeginDisabledGroup(true);
                    GUI.Label(buttonRect, "Click to add new preset");
                    EditorGUI.EndDisabledGroup();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        public void DeletePreset(int presetIndex)
        {
            T currentLib = this.GetCurrentLib();
            if (currentLib != null)
            {
                if ((presetIndex < 0) || (presetIndex >= currentLib.Count()))
                {
                    Debug.LogError("DeletePreset: Invalid index: out of bounds");
                }
                else
                {
                    currentLib.Remove(presetIndex);
                    this.SaveCurrentLib();
                    if (this.presetsWasReordered != null)
                    {
                        this.presetsWasReordered();
                    }
                    this.OnLayoutChanged();
                }
            }
        }

        private void DrawDragInsertionMarker()
        {
            if (this.m_DragState.IsDragging())
            {
                Rect rect2;
                Rect dragRect = this.GetDragRect(this.m_DragState.dragUponRect);
                if (this.m_State.itemViewMode == PresetLibraryEditorState.ItemViewMode.List)
                {
                    if (this.m_DragState.insertAfterIndex)
                    {
                        rect2 = new Rect(dragRect.xMin, dragRect.yMax - 1f, dragRect.width, 2f);
                    }
                    else
                    {
                        rect2 = new Rect(dragRect.xMin, dragRect.yMin - 1f, dragRect.width, 2f);
                    }
                }
                else if (this.m_DragState.insertAfterIndex)
                {
                    rect2 = new Rect(dragRect.xMax - 1f, dragRect.yMin, 2f, dragRect.height);
                }
                else
                {
                    rect2 = new Rect(dragRect.xMin - 1f, dragRect.yMin, 2f, dragRect.height);
                }
                EditorGUI.DrawRect(rect2, new Color(0.3f, 0.3f, 1f));
            }
        }

        private void DrawHoverEffect(Rect itemRect, bool drawAsSelection)
        {
            Color color = GUI.color;
            GUI.color = new Color(0f, 0f, 0.4f, !drawAsSelection ? 0.3f : 0.8f);
            GUI.Label(new RectOffset(3, 3, 3, 3).Add(itemRect), GUIContent.none, EditorStyles.helpBox);
            GUI.color = color;
        }

        private void EndRename()
        {
            if (this.GetRenameOverlay().userAcceptedRename)
            {
                string name = !string.IsNullOrEmpty(this.GetRenameOverlay().name) ? this.GetRenameOverlay().name : this.GetRenameOverlay().originalName;
                int userData = this.GetRenameOverlay().userData;
                T currentLib = this.GetCurrentLib();
                if ((userData >= 0) && (userData < currentLib.Count()))
                {
                    currentLib.SetName(userData, name);
                    this.SaveCurrentLib();
                }
                this.GetRenameOverlay().EndRename(true);
            }
        }

        public T GetCurrentLib()
        {
            T library = ScriptableSingleton<PresetLibraryManager>.instance.GetLibrary<T>(this.m_SaveLoadHelper, this.currentLibraryWithoutExtension);
            if (library == null)
            {
                library = ScriptableSingleton<PresetLibraryManager>.instance.GetLibrary<T>(this.m_SaveLoadHelper, PresetLibraryLocations.defaultPresetLibraryPath);
                if (library == null)
                {
                    library = this.CreateNewLibrary(PresetLibraryLocations.defaultPresetLibraryPath);
                    if (library != null)
                    {
                        if (this.addDefaultPresets != null)
                        {
                            this.addDefaultPresets(library);
                            ScriptableSingleton<PresetLibraryManager>.instance.SaveLibrary<T>(this.m_SaveLoadHelper, library, PresetLibraryLocations.defaultPresetLibraryPath);
                        }
                    }
                    else
                    {
                        Debug.LogError("Could not create Default preset library " + ScriptableSingleton<PresetLibraryManager>.instance.GetLastError());
                    }
                }
                this.currentLibraryWithoutExtension = PresetLibraryLocations.defaultPresetLibraryPath;
            }
            return library;
        }

        private Rect GetDragRect(Rect itemRect)
        {
            int left = Mathf.FloorToInt((this.m_Grid.horizontalSpacing * 0.5f) + 0.5f);
            int top = Mathf.FloorToInt((this.m_Grid.verticalSpacing * 0.5f) + 0.5f);
            return new RectOffset(left, left, top, top).Add(itemRect);
        }

        private RenameOverlay GetRenameOverlay()
        {
            return this.m_State.m_RenameOverlay;
        }

        public void InitializeGrid(float availableWidth)
        {
            T currentLib = this.GetCurrentLib();
            if (currentLib != null)
            {
                if (availableWidth > 0f)
                {
                    this.SetupGrid(availableWidth, currentLib.Count());
                }
            }
            else
            {
                Debug.LogError("Could not load preset library " + this.currentLibraryWithoutExtension);
            }
        }

        private static bool IsItemVisible(float scrollHeight, float itemYMin, float itemYMax, float scrollPos)
        {
            float num = itemYMin - scrollPos;
            float num2 = itemYMax - scrollPos;
            if (num2 < 0f)
            {
                return false;
            }
            if (num > scrollHeight)
            {
                return false;
            }
            return true;
        }

        private bool IsRenaming(int itemID)
        {
            return ((this.GetRenameOverlay().IsRenaming() && (this.GetRenameOverlay().userData == itemID)) && !this.GetRenameOverlay().isWaitingForDelay);
        }

        private static void LabelWithOutline(Rect rect, GUIContent content, Color outlineColor, GUIStyle style)
        {
            Color color = GUI.color;
            GUI.color = outlineColor;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i != 0) || (j != 0))
                    {
                        Rect position = rect;
                        position.x += j;
                        position.y += i;
                        GUI.Label(position, content, style);
                    }
                }
            }
            GUI.color = color;
            GUI.Label(rect, content, style);
        }

        private void ListArea(Rect rect, PresetLibrary lib, object newPresetObject)
        {
            if (lib != null)
            {
                int num2;
                int num3;
                Event current = Event.current;
                if ((this.m_PresetLibraryFileLocation == PresetFileLocation.ProjectFolder) && (current.type == EventType.Repaint))
                {
                    this.m_IsOpenForEdit = AssetDatabase.IsOpenForEdit(this.pathWithExtension);
                }
                else if (this.m_PresetLibraryFileLocation == PresetFileLocation.PreferencesFolder)
                {
                    this.m_IsOpenForEdit = true;
                }
                if (!this.m_IsOpenForEdit)
                {
                    Rect rect2 = new Rect(rect.x, rect.yMax - this.versionControlAreaHeight, rect.width, this.versionControlAreaHeight);
                    this.VersionControlArea(rect2);
                    rect.height -= this.versionControlAreaHeight;
                }
                for (int i = 0; i < 2; i++)
                {
                    this.gridWidth = !this.m_ShowedScrollBarLastFrame ? rect.width : (rect.width - 17f);
                    this.SetupGrid(this.gridWidth, lib.Count());
                    bool flag = this.m_Grid.height > rect.height;
                    if (flag == this.m_ShowedScrollBarLastFrame)
                    {
                        break;
                    }
                    this.m_ShowedScrollBarLastFrame = flag;
                }
                if ((this.m_ShowedScrollBarLastFrame || this.alwaysShowScrollAreaHorizontalLines) && (Event.current.type == EventType.Repaint))
                {
                    Rect rect3 = new RectOffset(1, 1, 1, 1).Add(rect);
                    rect3.height = 1f;
                    EditorGUI.DrawRect(rect3, new Color(0f, 0f, 0f, 0.3f));
                    rect3.y += rect.height + 1f;
                    EditorGUI.DrawRect(rect3, new Color(0f, 0f, 0f, 0.3f));
                }
                Rect viewRect = new Rect(0f, 0f, 1f, this.m_Grid.height);
                this.m_State.m_ScrollPosition = GUI.BeginScrollView(rect, this.m_State.m_ScrollPosition, viewRect);
                float gridStartY = 0f;
                int maxIndex = !this.m_ShowAddNewPresetItem ? (lib.Count() - 1) : lib.Count();
                bool flag2 = this.m_Grid.IsVisibleInScrollView(rect.height, this.m_State.m_ScrollPosition.y, gridStartY, maxIndex, out num2, out num3);
                bool flag3 = false;
                if (flag2)
                {
                    if (this.GetRenameOverlay().IsRenaming() && !this.GetRenameOverlay().isWaitingForDelay)
                    {
                        if (!this.m_State.m_RenameOverlay.OnGUI())
                        {
                            this.EndRename();
                            current.Use();
                        }
                        this.Repaint();
                    }
                    for (int j = num2; j <= num3; j++)
                    {
                        bool flag4;
                        Rect dragRect;
                        int controlID = j + 0xf4240;
                        Rect itemRect = this.m_Grid.CalcRect(j, gridStartY);
                        Rect buttonRect = itemRect;
                        Rect position = itemRect;
                        PresetLibraryEditorState.ItemViewMode itemViewMode = this.m_State.itemViewMode;
                        if ((itemViewMode != PresetLibraryEditorState.ItemViewMode.Grid) && (itemViewMode == PresetLibraryEditorState.ItemViewMode.List))
                        {
                            buttonRect.width = this.m_State.m_PreviewHeight * this.m_PreviewAspect;
                            position.x += buttonRect.width + 8f;
                            position.width -= buttonRect.width + 10f;
                            position.height = 16f;
                            position.y = itemRect.yMin + ((itemRect.height - 16f) * 0.5f);
                        }
                        if (this.m_ShowAddNewPresetItem && (j == lib.Count()))
                        {
                            this.CreateNewPresetButton(buttonRect, newPresetObject, lib, this.m_IsOpenForEdit);
                        }
                        else
                        {
                            flag4 = this.IsRenaming(j);
                            if (flag4)
                            {
                                Rect rect8 = position;
                                rect8.y--;
                                rect8.x--;
                                this.m_State.m_RenameOverlay.editFieldRect = rect8;
                            }
                            switch (current.type)
                            {
                                case EventType.MouseDown:
                                    if ((current.button == 0) && itemRect.Contains(current.mousePosition))
                                    {
                                        GUIUtility.hotControl = controlID;
                                        if (current.clickCount == 1)
                                        {
                                            this.m_ItemClickedCallback(current.clickCount, lib.GetPreset(j));
                                            current.Use();
                                        }
                                    }
                                    break;

                                case EventType.MouseUp:
                                    if (GUIUtility.hotControl == controlID)
                                    {
                                        GUIUtility.hotControl = 0;
                                        if (((current.button == 0) && itemRect.Contains(current.mousePosition)) && (Event.current.alt && this.m_IsOpenForEdit))
                                        {
                                            this.DeletePreset(j);
                                            current.Use();
                                        }
                                    }
                                    break;

                                case EventType.MouseMove:
                                    if (!itemRect.Contains(current.mousePosition))
                                    {
                                        goto Label_0812;
                                    }
                                    if (this.m_State.m_HoverIndex != j)
                                    {
                                        this.m_State.m_HoverIndex = j;
                                        this.Repaint();
                                    }
                                    break;

                                case EventType.MouseDrag:
                                    if ((GUIUtility.hotControl == controlID) && this.m_IsOpenForEdit)
                                    {
                                        DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), controlID);
                                        if (stateObject.CanStartDrag())
                                        {
                                            DragAndDrop.PrepareStartDrag();
                                            DragAndDrop.SetGenericData("DraggingPreset", j);
                                            DragAndDrop.objectReferences = new Object[0];
                                            DragAndDrop.StartDrag(string.Empty);
                                            this.m_DragState.draggingIndex = j;
                                            this.m_DragState.dragUponIndex = j;
                                            GUIUtility.hotControl = 0;
                                        }
                                        current.Use();
                                    }
                                    break;

                                case EventType.Repaint:
                                    if ((this.m_State.m_HoverIndex == j) && !itemRect.Contains(current.mousePosition))
                                    {
                                        goto Label_046D;
                                    }
                                    goto Label_0479;

                                case EventType.DragUpdated:
                                case EventType.DragPerform:
                                    dragRect = this.GetDragRect(itemRect);
                                    if (!dragRect.Contains(current.mousePosition))
                                    {
                                        break;
                                    }
                                    this.m_DragState.dragUponIndex = j;
                                    this.m_DragState.dragUponRect = itemRect;
                                    if (this.m_State.itemViewMode != PresetLibraryEditorState.ItemViewMode.List)
                                    {
                                        goto Label_0694;
                                    }
                                    this.m_DragState.insertAfterIndex = ((current.mousePosition.y - dragRect.y) / dragRect.height) > 0.5f;
                                    goto Label_06C5;

                                case EventType.DragExited:
                                    if (this.m_DragState.IsDragging())
                                    {
                                        this.ClearDragState();
                                        current.Use();
                                    }
                                    break;

                                case EventType.ContextClick:
                                    if (itemRect.Contains(current.mousePosition))
                                    {
                                        PresetContextMenu<T>.Show(this.m_IsOpenForEdit, j, newPresetObject, (PresetLibraryEditor<T>) this);
                                        current.Use();
                                    }
                                    break;
                            }
                        }
                        continue;
                    Label_046D:
                        this.m_State.m_HoverIndex = -1;
                    Label_0479:
                        if ((this.m_DragState.draggingIndex == j) || (GUIUtility.hotControl == controlID))
                        {
                            this.DrawHoverEffect(itemRect, false);
                        }
                        lib.Draw(buttonRect, j);
                        if (!flag4 && this.drawLabels)
                        {
                            GUI.Label(position, GUIContent.Temp(lib.GetName(j)));
                        }
                        if ((this.m_DragState.dragUponIndex == j) && (this.m_DragState.draggingIndex != this.m_DragState.dragUponIndex))
                        {
                            flag3 = true;
                        }
                        if (((GUIUtility.hotControl == 0) && Event.current.alt) && this.m_IsOpenForEdit)
                        {
                            EditorGUIUtility.AddCursorRect(itemRect, MouseCursor.ArrowMinus);
                        }
                        continue;
                    Label_0694:
                        this.m_DragState.insertAfterIndex = ((current.mousePosition.x - dragRect.x) / dragRect.width) > 0.5f;
                    Label_06C5:
                        if (current.type == EventType.DragPerform)
                        {
                            if (this.m_DragState.draggingIndex >= 0)
                            {
                                this.MovePreset(this.m_DragState.draggingIndex, this.m_DragState.dragUponIndex, this.m_DragState.insertAfterIndex);
                                DragAndDrop.AcceptDrag();
                            }
                            this.ClearDragState();
                        }
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        current.Use();
                        continue;
                    Label_0812:
                        if (this.m_State.m_HoverIndex == j)
                        {
                            this.m_State.m_HoverIndex = -1;
                            this.Repaint();
                        }
                    }
                    if (flag3)
                    {
                        this.DrawDragInsertionMarker();
                    }
                }
                GUI.EndScrollView();
            }
        }

        public void MovePreset(int presetIndex, int destPresetIndex, bool insertAfterDestIndex)
        {
            T currentLib = this.GetCurrentLib();
            if (currentLib != null)
            {
                if ((presetIndex < 0) || (presetIndex >= currentLib.Count()))
                {
                    Debug.LogError("ReplacePreset: Invalid index: out of bounds");
                }
                else
                {
                    currentLib.Move(presetIndex, destPresetIndex, insertAfterDestIndex);
                    this.SaveCurrentLib();
                    if (this.presetsWasReordered != null)
                    {
                        this.presetsWasReordered();
                    }
                }
            }
        }

        public void OnGUI(Rect rect, object presetObject)
        {
            if (rect.width >= 2f)
            {
                this.m_State.m_RenameOverlay.OnEvent();
                T currentLib = this.GetCurrentLib();
                if (PresetLibraryEditor<T>.s_Styles == null)
                {
                    PresetLibraryEditor<T>.s_Styles = new Styles<T>();
                }
                Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.topAreaHeight);
                Rect rect3 = new Rect(rect.x, rect2.yMax, rect.width, rect.height - this.topAreaHeight);
                this.TopArea(rect2);
                this.ListArea(rect3, currentLib, presetObject);
            }
        }

        private void OnLayoutChanged()
        {
            T currentLib = this.GetCurrentLib();
            if ((currentLib != null) && (this.gridWidth > 0f))
            {
                this.SetupGrid(this.gridWidth, currentLib.Count());
            }
        }

        private void Repaint()
        {
            HandleUtility.Repaint();
        }

        public void ReplacePreset(int presetIndex, object presetObject)
        {
            T currentLib = this.GetCurrentLib();
            if (currentLib != null)
            {
                if ((presetIndex < 0) || (presetIndex >= currentLib.Count()))
                {
                    Debug.LogError("ReplacePreset: Invalid index: out of bounds");
                }
                else
                {
                    currentLib.Replace(presetIndex, presetObject);
                    this.SaveCurrentLib();
                    if (this.presetsWasReordered != null)
                    {
                        this.presetsWasReordered();
                    }
                }
            }
        }

        public void RevealCurrentLibrary()
        {
            if (this.m_PresetLibraryFileLocation == PresetFileLocation.PreferencesFolder)
            {
                EditorUtility.RevealInFinder(Path.GetFullPath(this.pathWithExtension));
            }
            else
            {
                EditorGUIUtility.PingObject(AssetDatabase.GetMainAssetInstanceID(this.pathWithExtension));
            }
        }

        public void SaveCurrentLib()
        {
            T currentLib = this.GetCurrentLib();
            if (currentLib == null)
            {
                Debug.Log("No current library selected!");
            }
            else
            {
                ScriptableSingleton<PresetLibraryManager>.instance.SaveLibrary<T>(this.m_SaveLoadHelper, currentLib, this.currentLibraryWithoutExtension);
                InternalEditorUtility.RepaintAllViews();
            }
        }

        private void SetupGrid(float width, int itemCount)
        {
            if (width < 1f)
            {
                Debug.LogError(string.Concat(new object[] { "Invalid width ", width, ", ", Event.current.type }));
            }
            else
            {
                if (this.m_ShowAddNewPresetItem)
                {
                    itemCount++;
                }
                this.m_Grid.useFixedHorizontalSpacing = this.useOnePixelOverlappedGrid;
                this.m_Grid.fixedHorizontalSpacing = !this.useOnePixelOverlappedGrid ? ((float) 0) : ((float) (-1));
                switch (this.m_State.itemViewMode)
                {
                    case PresetLibraryEditorState.ItemViewMode.Grid:
                        this.m_Grid.fixedWidth = width;
                        this.m_Grid.topMargin = this.marginsForGrid.top;
                        this.m_Grid.bottomMargin = this.marginsForGrid.bottom;
                        this.m_Grid.leftMargin = this.marginsForGrid.left;
                        this.m_Grid.rightMargin = this.marginsForGrid.right;
                        this.m_Grid.verticalSpacing = !this.useOnePixelOverlappedGrid ? ((float) 2) : ((float) (-1));
                        this.m_Grid.minHorizontalSpacing = 1f;
                        this.m_Grid.itemSize = new Vector2(this.m_State.m_PreviewHeight * this.m_PreviewAspect, this.m_State.m_PreviewHeight);
                        this.m_Grid.InitNumRowsAndColumns(itemCount, 0x7fffffff);
                        break;

                    case PresetLibraryEditorState.ItemViewMode.List:
                        this.m_Grid.fixedWidth = width;
                        this.m_Grid.topMargin = this.marginsForList.top;
                        this.m_Grid.bottomMargin = this.marginsForList.bottom;
                        this.m_Grid.leftMargin = this.marginsForList.left;
                        this.m_Grid.rightMargin = this.marginsForList.right;
                        this.m_Grid.verticalSpacing = 2f;
                        this.m_Grid.minHorizontalSpacing = 0f;
                        this.m_Grid.itemSize = new Vector2(width - this.m_Grid.leftMargin, this.m_State.m_PreviewHeight);
                        this.m_Grid.InitNumRowsAndColumns(itemCount, 0x7fffffff);
                        break;
                }
                float num = this.m_Grid.CalcRect(itemCount - 1, 0f).yMax + this.m_Grid.bottomMargin;
                this.contentHeight = (this.topAreaHeight + num) + (!this.m_IsOpenForEdit ? this.versionControlAreaHeight : 0f);
            }
        }

        private void TopArea(Rect rect)
        {
            GUI.BeginGroup(rect);
            if (this.showHeader)
            {
                GUI.Label(new Rect(10f, 2f, rect.width - 20f, rect.height), PresetLibraryEditor<T>.s_Styles.header);
            }
            Rect position = new Rect((rect.width - 16f) - this.settingsMenuRightMargin, (rect.height - 6f) * 0.5f, 16f, rect.height);
            if (Event.current.type == EventType.Repaint)
            {
                PresetLibraryEditor<T>.s_Styles.optionsButton.Draw(position, false, false, false, false);
            }
            position.y = 0f;
            position.height = rect.height;
            position.width = 24f;
            if (GUI.Button(position, GUIContent.none, GUIStyle.none))
            {
                SettingsMenu<T>.Show(position, (PresetLibraryEditor<T>) this);
            }
            if (this.wantsToCreateLibrary)
            {
                this.wantsToCreateLibrary = false;
                PopupWindow.Show(position, new PopupWindowContentForNewLibrary(new Func<string, PresetFileLocation, string>(this.CreateNewLibraryCallback)));
                GUIUtility.ExitGUI();
            }
            GUI.EndGroup();
        }

        public void UnloadUsedLibraries()
        {
            ScriptableSingleton<PresetLibraryManager>.instance.UnloadAllLibrariesFor<T>(this.m_SaveLoadHelper);
        }

        private void ValidateNoExtension(string value)
        {
            if (Path.HasExtension(value))
            {
                Debug.LogError("currentLibraryWithoutExtension should not have an extension: " + value);
            }
        }

        private void VersionControlArea(Rect rect)
        {
            if (rect.width > 100f)
            {
                rect = new Rect((rect.xMax - 100f) - 2f, rect.y + 2f, 100f, rect.height - 4f);
            }
            if (GUI.Button(rect, "Check out", EditorStyles.miniButton))
            {
                string[] assets = new string[] { this.pathWithExtension };
                Provider.Checkout(assets, CheckoutMode.Asset);
            }
        }

        public bool alwaysShowScrollAreaHorizontalLines { get; set; }

        public float contentHeight { get; private set; }

        public string currentLibraryWithoutExtension
        {
            get
            {
                return this.m_State.m_CurrrentLibrary;
            }
            set
            {
                this.m_State.m_CurrrentLibrary = Path.ChangeExtension(value, null);
                this.m_PresetLibraryFileLocation = PresetLibraryLocations.GetFileLocationFromPath(this.m_State.m_CurrrentLibrary);
                this.OnLayoutChanged();
                this.Repaint();
            }
        }

        private bool drawLabels
        {
            get
            {
                return (this.m_State.itemViewMode == PresetLibraryEditorState.ItemViewMode.List);
            }
        }

        private float gridWidth { get; set; }

        public PresetLibraryEditorState.ItemViewMode itemViewMode
        {
            get
            {
                return this.m_State.itemViewMode;
            }
            set
            {
                this.m_State.itemViewMode = value;
                this.OnLayoutChanged();
                this.Repaint();
            }
        }

        public RectOffset marginsForGrid { get; set; }

        public RectOffset marginsForList { get; set; }

        public Vector2 minMaxPreviewHeight
        {
            get
            {
                return this.m_MinMaxPreviewHeight;
            }
            set
            {
                this.m_MinMaxPreviewHeight = value;
                this.previewHeight = this.previewHeight;
            }
        }

        private string pathWithExtension
        {
            get
            {
                return (this.currentLibraryWithoutExtension + "." + this.m_SaveLoadHelper.fileExtensionWithoutDot);
            }
        }

        public float previewAspect
        {
            get
            {
                return this.m_PreviewAspect;
            }
            set
            {
                this.m_PreviewAspect = value;
            }
        }

        public float previewHeight
        {
            get
            {
                return this.m_State.m_PreviewHeight;
            }
            set
            {
                this.m_State.m_PreviewHeight = Mathf.Clamp(value, this.minMaxPreviewHeight.x, this.minMaxPreviewHeight.y);
                this.Repaint();
            }
        }

        public float settingsMenuRightMargin { get; set; }

        public bool showHeader { get; set; }

        private float topAreaHeight
        {
            get
            {
                return 20f;
            }
        }

        public bool useOnePixelOverlappedGrid { get; set; }

        private float versionControlAreaHeight
        {
            get
            {
                return 20f;
            }
        }

        public bool wantsToCreateLibrary { get; set; }

        private class DragState
        {
            public DragState()
            {
                this.dragUponIndex = -1;
                this.draggingIndex = -1;
            }

            public bool IsDragging()
            {
                return (this.draggingIndex != -1);
            }

            public int draggingIndex { get; set; }

            public int dragUponIndex { get; set; }

            public Rect dragUponRect { get; set; }

            public bool insertAfterIndex { get; set; }
        }

        internal class PresetContextMenu
        {
            private static PresetLibraryEditor<T> s_Caller;
            private static int s_PresetIndex;

            private void Delete(object userData)
            {
                PresetLibraryEditor<T>.PresetContextMenu.s_Caller.DeletePreset(PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex);
            }

            private void MoveToTop(object userData)
            {
                PresetLibraryEditor<T>.PresetContextMenu.s_Caller.MovePreset(PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex, 0, false);
            }

            private void Rename(object userData)
            {
                string name = PresetLibraryEditor<T>.PresetContextMenu.s_Caller.GetCurrentLib().GetName(PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex);
                PresetLibraryEditor<T>.PresetContextMenu.s_Caller.BeginRenaming(name, PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex, 0f);
            }

            private void Replace(object userData)
            {
                object presetObject = userData;
                PresetLibraryEditor<T>.PresetContextMenu.s_Caller.ReplacePreset(PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex, presetObject);
            }

            internal static void Show(bool isOpenForEdit, int presetIndex, object newPresetObject, PresetLibraryEditor<T> caller)
            {
                PresetLibraryEditor<T>.PresetContextMenu.s_Caller = caller;
                PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex = presetIndex;
                GUIContent content = new GUIContent("Replace");
                GUIContent content2 = new GUIContent("Delete");
                GUIContent content3 = new GUIContent("Rename");
                GUIContent content4 = new GUIContent("Move To First");
                GenericMenu menu = new GenericMenu();
                if (isOpenForEdit)
                {
                    menu.AddItem(content, false, new GenericMenu.MenuFunction2(new PresetLibraryEditor<T>.PresetContextMenu().Replace), newPresetObject);
                    menu.AddItem(content2, false, new GenericMenu.MenuFunction2(new PresetLibraryEditor<T>.PresetContextMenu().Delete), 0);
                    if (caller.drawLabels)
                    {
                        menu.AddItem(content3, false, new GenericMenu.MenuFunction2(new PresetLibraryEditor<T>.PresetContextMenu().Rename), 0);
                    }
                    menu.AddItem(content4, false, new GenericMenu.MenuFunction2(new PresetLibraryEditor<T>.PresetContextMenu().MoveToTop), 0);
                }
                else
                {
                    menu.AddDisabledItem(content);
                    menu.AddDisabledItem(content2);
                    if (caller.drawLabels)
                    {
                        menu.AddDisabledItem(content3);
                    }
                    menu.AddDisabledItem(content4);
                }
                menu.ShowAsContext();
            }
        }

        private class SettingsMenu
        {
            private static PresetLibraryEditor<T> s_Owner;

            private static void AddDefaultPresetsToCurrentLibrary(object userData)
            {
                if (PresetLibraryEditor<T>.SettingsMenu.s_Owner.addDefaultPresets != null)
                {
                    PresetLibraryEditor<T>.SettingsMenu.s_Owner.addDefaultPresets(PresetLibraryEditor<T>.SettingsMenu.s_Owner.GetCurrentLib());
                }
                PresetLibraryEditor<T>.SettingsMenu.s_Owner.SaveCurrentLib();
            }

            private static void CreateLibrary(object userData)
            {
                PresetLibraryEditor<T>.SettingsMenu.s_Owner.wantsToCreateLibrary = true;
            }

            private static bool HasDefaultPresets()
            {
                return (PresetLibraryEditor<T>.SettingsMenu.s_Owner.addDefaultPresets != null);
            }

            private static void LibraryModeChange(object userData)
            {
                string str = (string) userData;
                PresetLibraryEditor<T>.SettingsMenu.s_Owner.currentLibraryWithoutExtension = str;
            }

            private static void RevealCurrentLibrary(object userData)
            {
                PresetLibraryEditor<T>.SettingsMenu.s_Owner.RevealCurrentLibrary();
            }

            public static void Show(Rect activatorRect, PresetLibraryEditor<T> owner)
            {
                List<ViewModeData<T>> list;
                List<string> list2;
                List<string> list3;
                List<ViewModeData<T>> list4;
                ViewModeData<T> data;
                PresetLibraryEditor<T>.SettingsMenu.s_Owner = owner;
                GenericMenu menu = new GenericMenu();
                int x = (int) PresetLibraryEditor<T>.SettingsMenu.s_Owner.minMaxPreviewHeight.x;
                int y = (int) PresetLibraryEditor<T>.SettingsMenu.s_Owner.minMaxPreviewHeight.y;
                if (x == y)
                {
                    list4 = new List<ViewModeData<T>>();
                    data = new ViewModeData<T> {
                        text = new GUIContent("Grid"),
                        itemHeight = x,
                        viewmode = PresetLibraryEditorState.ItemViewMode.Grid
                    };
                    list4.Add(data);
                    data = new ViewModeData<T> {
                        text = new GUIContent("List"),
                        itemHeight = x,
                        viewmode = PresetLibraryEditorState.ItemViewMode.List
                    };
                    list4.Add(data);
                    list = list4;
                }
                else
                {
                    list4 = new List<ViewModeData<T>>();
                    data = new ViewModeData<T> {
                        text = new GUIContent("Small Grid"),
                        itemHeight = x,
                        viewmode = PresetLibraryEditorState.ItemViewMode.Grid
                    };
                    list4.Add(data);
                    data = new ViewModeData<T> {
                        text = new GUIContent("Large Grid"),
                        itemHeight = y,
                        viewmode = PresetLibraryEditorState.ItemViewMode.Grid
                    };
                    list4.Add(data);
                    data = new ViewModeData<T> {
                        text = new GUIContent("Small List"),
                        itemHeight = x,
                        viewmode = PresetLibraryEditorState.ItemViewMode.List
                    };
                    list4.Add(data);
                    data = new ViewModeData<T> {
                        text = new GUIContent("Large List"),
                        itemHeight = y,
                        viewmode = PresetLibraryEditorState.ItemViewMode.List
                    };
                    list4.Add(data);
                    list = list4;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    bool on = (PresetLibraryEditor<T>.SettingsMenu.s_Owner.itemViewMode == list[i].viewmode) && (((int) PresetLibraryEditor<T>.SettingsMenu.s_Owner.previewHeight) == list[i].itemHeight);
                    menu.AddItem(list[i].text, on, new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.ViewModeChange), list[i]);
                }
                menu.AddSeparator(string.Empty);
                ScriptableSingleton<PresetLibraryManager>.instance.GetAvailableLibraries<T>(PresetLibraryEditor<T>.SettingsMenu.s_Owner.m_SaveLoadHelper, out list2, out list3);
                list2.Sort();
                list3.Sort();
                string str = PresetLibraryEditor<T>.SettingsMenu.s_Owner.currentLibraryWithoutExtension + "." + PresetLibraryEditor<T>.SettingsMenu.s_Owner.m_SaveLoadHelper.fileExtensionWithoutDot;
                string str2 = " (Project)";
                foreach (string str3 in list2)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str3);
                    menu.AddItem(new GUIContent(fileNameWithoutExtension), str == str3, new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.LibraryModeChange), str3);
                }
                foreach (string str5 in list3)
                {
                    string str6 = Path.GetFileNameWithoutExtension(str5);
                    menu.AddItem(new GUIContent(str6 + str2), str == str5, new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.LibraryModeChange), str5);
                }
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Create New Library..."), false, new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.CreateLibrary), 0);
                if (PresetLibraryEditor<T>.SettingsMenu.HasDefaultPresets())
                {
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("Add Factory Presets To Current Library"), false, new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.AddDefaultPresetsToCurrentLibrary), 0);
                }
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Reveal Current Library Location"), false, new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.RevealCurrentLibrary), 0);
                menu.DropDown(activatorRect);
            }

            private static void ViewModeChange(object userData)
            {
                ViewModeData<T> data = (ViewModeData<T>) userData;
                PresetLibraryEditor<T>.SettingsMenu.s_Owner.itemViewMode = data.viewmode;
                PresetLibraryEditor<T>.SettingsMenu.s_Owner.previewHeight = data.itemHeight;
            }

            private class ViewModeData
            {
                public int itemHeight;
                public GUIContent text;
                public PresetLibraryEditorState.ItemViewMode viewmode;
            }
        }

        private class Styles
        {
            public GUIContent header;
            public GUIStyle innerShadowBg;
            public GUIContent newPreset;
            public GUIStyle newPresetStyle;
            public GUIStyle optionsButton;
            public GUIContent plusButtonText;
            public GUIContent plusButtonTextNotCheckedOut;

            public Styles()
            {
                this.innerShadowBg = PresetLibraryEditor<T>.Styles.GetStyle("InnerShadowBg");
                this.optionsButton = PresetLibraryEditor<T>.Styles.GetStyle("PaneOptions");
                this.newPresetStyle = new GUIStyle(EditorStyles.boldLabel);
                this.plusButtonText = new GUIContent(string.Empty, "Add new preset");
                this.plusButtonTextNotCheckedOut = new GUIContent(string.Empty, "To add presets you need to press the 'Check out' button below");
                this.header = new GUIContent("Presets");
                this.newPreset = new GUIContent("New");
                this.newPresetStyle.alignment = TextAnchor.MiddleCenter;
                this.newPresetStyle.normal.textColor = Color.white;
            }

            private static GUIStyle GetStyle(string styleName)
            {
                return styleName;
            }
        }
    }
}

