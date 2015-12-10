namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEditorInternal.VersionControl;
    using UnityEngine;

    internal class ObjectListArea
    {
        [CompilerGenerated]
        private static Predicate<AssetStoreGroup> <>f__am$cache39;
        private const float k_ListModeVersionControlOverlayPadding = 14f;
        private const double kDelayQueryAfterScroll = 0.0;
        private const int kEnd = 0x7fffffff;
        private const int kHome = -2147483648;
        private const int kListLineHeight = 0x10;
        private const int kPageDown = 0x7ffffffe;
        private const int kPageUp = -2147483647;
        private const double kQueryDelay = 0.2;
        private const int kSpaceForScrollBar = 0x10;
        private double LastScrollTime;
        private bool m_AllowRenameOnMouseUp = true;
        private bool m_AllowThumbnails = true;
        private string m_AssetStoreError = string.Empty;
        private Action m_AssetStoreSearchEnded;
        public float m_BottomMargin = 10f;
        private Func<Rect, float> m_DrawLocalAssetHeader;
        private bool m_FrameLastClickedItem;
        private Action m_GotKeyboardFocus;
        private List<Group> m_Groups;
        private Dictionary<int, string> m_InstanceIDToCroppedNameMap = new Dictionary<int, string>();
        private Action<bool> m_ItemSelectedCallback;
        private int m_KeyboardControlID;
        private Action m_KeyboardInputCallback;
        private double m_LastAssetStoreQueryChangeTime;
        private string[] m_LastAssetStoreQueryClassName = new string[0];
        private string[] m_LastAssetStoreQueryLabels = new string[0];
        private string m_LastAssetStoreQuerySearchFilter = string.Empty;
        private Vector2 m_LastScrollPosition = new Vector2(0f, 0f);
        public float m_LeftMargin = 10f;
        private int m_LeftPaddingForPinging;
        private LocalGroup m_LocalAssets;
        private int m_MaxGridSize = 0x60;
        private int m_MinGridSize = 0x10;
        private int m_MinIconSize = 0x20;
        private double m_NextDirtyCheck;
        private EditorWindow m_Owner;
        private PingData m_Ping = new PingData();
        private int m_pingIndex;
        private bool m_QueryInProgress;
        private Action m_RepaintWantedCallback;
        public bool m_RequeryAssetStore;
        private int m_ResizePreviewCacheTo;
        public float m_RightMargin = 10f;
        internal Texture m_SelectedObjectIcon;
        private int m_SelectionOffset;
        private bool m_ShowLocalAssetsOnly = true;
        public float m_SpaceBetween = 6f;
        private ObjectListAreaState m_State;
        private List<AssetStoreGroup> m_StoreAssets;
        public float m_TopMargin = 10f;
        private Rect m_TotalRect;
        private Rect m_VisibleRect;
        private int m_WidthUsedForCroppingName;
        internal static bool s_Debug;
        private static Styles s_Styles;
        private static bool s_VCEnabled;
        public bool selectedAssetStoreAsset;

        public ObjectListArea(ObjectListAreaState state, EditorWindow owner, bool showNoneItem)
        {
            this.m_State = state;
            this.m_Owner = owner;
            AssetStorePreviewManager.MaxCachedImages = 0x48;
            this.m_StoreAssets = new List<AssetStoreGroup>();
            this.m_RequeryAssetStore = false;
            this.m_LocalAssets = new LocalGroup(this, string.Empty, showNoneItem);
            this.m_Groups = new List<Group>();
            this.m_Groups.Add(this.m_LocalAssets);
        }

        private static Rect AdjustRectForFraming(Rect r)
        {
            r.height += s_Styles.resultsGridLabel.fixedHeight * 2f;
            r.y -= s_Styles.resultsGridLabel.fixedHeight;
            return r;
        }

        private bool AllowLeftRightArrowNavigation()
        {
            bool flag = !this.m_LocalAssets.ListMode && !this.IsPreviewIconExpansionModifierPressed();
            bool flag2 = !this.m_ShowLocalAssetsOnly || (this.m_LocalAssets.ItemCount > 1);
            return (flag && flag2);
        }

        internal void BeginNamingNewAsset(string newAssetName, int instanceID, bool isCreatingNewFolder)
        {
            this.m_State.m_NewAssetIndexInList = this.m_LocalAssets.IndexOfNewText(newAssetName, isCreatingNewFolder, this.foldersFirst);
            if (this.m_State.m_NewAssetIndexInList != -1)
            {
                this.Frame(instanceID, true, false);
                this.GetRenameOverlay().BeginRename(newAssetName, instanceID, 0f);
            }
            else
            {
                Debug.LogError("Failed to insert new asset into list");
            }
            this.Repaint();
        }

        public void BeginPing(int instanceID)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            int index = this.m_LocalAssets.IndexOf(instanceID);
            if (index != -1)
            {
                <BeginPing>c__AnonStorey3C storeyc = new <BeginPing>c__AnonStorey3C();
                string fullText = null;
                storeyc.hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
                if (storeyc.hierarchyProperty.Find(instanceID, null))
                {
                    fullText = storeyc.hierarchyProperty.name;
                }
                if (fullText != null)
                {
                    this.m_Ping.m_TimeStart = Time.realtimeSinceStartup;
                    this.m_Ping.m_AvailableWidth = this.m_VisibleRect.width;
                    this.m_pingIndex = index;
                    float num2 = !s_VCEnabled ? 0f : 14f;
                    GUIContent content = new GUIContent(!this.m_LocalAssets.ListMode ? this.GetCroppedLabelText(instanceID, fullText, (float) this.m_WidthUsedForCroppingName) : fullText);
                    storeyc.label = content.text;
                    if (this.m_LocalAssets.ListMode)
                    {
                        <BeginPing>c__AnonStorey3B storeyb = new <BeginPing>c__AnonStorey3B {
                            <>f__ref$60 = storeyc
                        };
                        this.m_Ping.m_PingStyle = s_Styles.ping;
                        Vector2 vector = this.m_Ping.m_PingStyle.CalcSize(content);
                        this.m_Ping.m_ContentRect.width = (vector.x + num2) + 16f;
                        this.m_Ping.m_ContentRect.height = vector.y;
                        this.m_LeftPaddingForPinging = !storeyc.hierarchyProperty.isMainRepresentation ? 0x1c : 0x10;
                        storeyb.res = this.m_LocalAssets.LookupByInstanceID(instanceID);
                        this.m_Ping.m_ContentDraw = new Action<Rect>(storeyb.<>m__51);
                    }
                    else
                    {
                        this.m_Ping.m_PingStyle = s_Styles.miniPing;
                        Vector2 vector2 = this.m_Ping.m_PingStyle.CalcSize(content);
                        this.m_Ping.m_ContentRect.width = vector2.x;
                        this.m_Ping.m_ContentRect.height = vector2.y;
                        this.m_Ping.m_ContentDraw = new Action<Rect>(storeyc.<>m__52);
                    }
                    Vector2 vector3 = this.CalculatePingPosition();
                    this.m_Ping.m_ContentRect.x = vector3.x;
                    this.m_Ping.m_ContentRect.y = vector3.y;
                    this.Repaint();
                }
            }
        }

        public bool BeginRename(float delay)
        {
            if (!this.allowRenaming)
            {
                return false;
            }
            if (this.m_State.m_SelectedInstanceIDs.Count != 1)
            {
                return false;
            }
            int instanceID = this.m_State.m_SelectedInstanceIDs[0];
            if (AssetDatabase.IsSubAsset(instanceID))
            {
                return false;
            }
            if (this.m_LocalAssets.IsBuiltinAsset(instanceID))
            {
                return false;
            }
            if (!AssetDatabase.Contains(instanceID))
            {
                return false;
            }
            string nameOfLocalAsset = this.m_LocalAssets.GetNameOfLocalAsset(instanceID);
            if (nameOfLocalAsset == null)
            {
                return false;
            }
            return this.GetRenameOverlay().BeginRename(nameOfLocalAsset, instanceID, delay);
        }

        private Vector2 CalculatePingPosition()
        {
            Rect rect = this.m_LocalAssets.m_Grid.CalcRect(this.m_pingIndex, 0f);
            if (this.m_LocalAssets.ListMode)
            {
                return new Vector2((float) this.m_LeftPaddingForPinging, rect.y);
            }
            float width = this.m_Ping.m_ContentRect.width;
            return new Vector2((rect.center.x - (width / 2f)) + this.m_Ping.m_PingStyle.padding.left, (rect.yMax - s_Styles.resultsGridLabel.fixedHeight) + 3f);
        }

        public bool CanShowThumbnails()
        {
            return this.m_AllowThumbnails;
        }

        private void CenterRect(Rect r)
        {
            float num = (r.yMax + r.yMin) / 2f;
            float num2 = this.m_TotalRect.height / 2f;
            this.m_State.m_ScrollPosition.y = num - num2;
            this.ScrollToPosition(r);
        }

        private void ClearAssetStoreGroups()
        {
            this.m_Groups.Clear();
            this.m_Groups.Add(this.m_LocalAssets);
            this.m_StoreAssets.Clear();
            this.Repaint();
        }

        private void ClearCroppedLabelCache()
        {
            this.m_InstanceIDToCroppedNameMap.Clear();
        }

        private void ClearRenameState()
        {
            this.GetRenameOverlay().Clear();
            this.GetCreateAssetUtility().Clear();
            this.m_State.m_NewAssetIndexInList = -1;
        }

        private static string CreateFilterString(string searchString, string requiredClassName)
        {
            string str = searchString;
            if (!string.IsNullOrEmpty(requiredClassName))
            {
                str = str + " t:" + requiredClassName;
            }
            return str;
        }

        private void DoOffsetSelection()
        {
            if (this.m_SelectionOffset != 0)
            {
                int maxIdx = this.GetMaxIdx();
                if (this.maxGridSize != -1)
                {
                    int selectedAssetIdx = this.GetSelectedAssetIdx();
                    selectedAssetIdx = (selectedAssetIdx >= 0) ? selectedAssetIdx : 0;
                    this.DoOffsetSelectionSpecialKeys(selectedAssetIdx, maxIdx);
                    if (this.m_SelectionOffset != 0)
                    {
                        int a = selectedAssetIdx + this.m_SelectionOffset;
                        this.m_SelectionOffset = 0;
                        if (a < 0)
                        {
                            a = selectedAssetIdx;
                        }
                        else
                        {
                            a = Mathf.Min(a, maxIdx);
                        }
                        int selectedIdx = a;
                        this.SetSelectedAssetByIdx(selectedIdx);
                    }
                }
            }
        }

        private void DoOffsetSelectionSpecialKeys(int idx, int maxIndex)
        {
            float num = this.m_LocalAssets.m_Grid.itemSize.y + this.m_LocalAssets.m_Grid.verticalSpacing;
            int columns = this.m_LocalAssets.m_Grid.columns;
            int selectionOffset = this.m_SelectionOffset;
            switch (selectionOffset)
            {
                case 0x7ffffffe:
                {
                    if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        this.m_State.m_ScrollPosition.y += this.m_TotalRect.height;
                        this.m_SelectionOffset = 0;
                        return;
                    }
                    this.m_SelectionOffset = Mathf.RoundToInt(this.m_TotalRect.height / num) * columns;
                    int num3 = maxIndex - idx;
                    this.m_SelectionOffset = Mathf.Min(Mathf.FloorToInt(((float) num3) / ((float) columns)) * columns, this.m_SelectionOffset);
                    break;
                }
                case 0x7fffffff:
                    this.m_SelectionOffset = maxIndex - idx;
                    break;

                case -2147483648:
                    this.m_SelectionOffset = 0;
                    this.SetSelectedAssetByIdx(0);
                    return;

                default:
                    if (selectionOffset == -2147483647)
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor)
                        {
                            this.m_State.m_ScrollPosition.y -= this.m_TotalRect.height;
                            this.m_SelectionOffset = 0;
                            return;
                        }
                        this.m_SelectionOffset = -Mathf.RoundToInt(this.m_TotalRect.height / num) * columns;
                        this.m_SelectionOffset = Mathf.Max(-Mathf.FloorToInt(((float) idx) / ((float) columns)) * columns, this.m_SelectionOffset);
                    }
                    break;
            }
        }

        public void EndPing()
        {
            this.m_Ping.m_TimeStart = -1f;
        }

        public void EndRename(bool acceptChanges)
        {
            if (this.GetRenameOverlay().IsRenaming())
            {
                this.GetRenameOverlay().EndRename(acceptChanges);
                this.RenameEnded();
            }
        }

        private void EnsureAssetStoreGroupsAreOpenIfAllClosed()
        {
            if (this.m_StoreAssets.Count > 0)
            {
                int num = 0;
                foreach (AssetStoreGroup group in this.m_StoreAssets)
                {
                    if (group.Visible)
                    {
                        num++;
                    }
                }
                if (num == 0)
                {
                    foreach (AssetStoreGroup group2 in this.m_StoreAssets)
                    {
                        bool flag = true;
                        group2.visiblePreference = flag;
                        group2.Visible = flag;
                    }
                }
            }
        }

        public bool Frame(int instanceID, bool frame, bool ping)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            int itemIdx = -1;
            if ((this.GetCreateAssetUtility().IsCreatingNewAsset() && (this.m_State.m_NewAssetIndexInList != -1)) && (this.GetCreateAssetUtility().instanceID == instanceID))
            {
                itemIdx = this.m_State.m_NewAssetIndexInList;
            }
            if (frame)
            {
                this.Reveal(instanceID);
            }
            if (itemIdx == -1)
            {
                itemIdx = this.m_LocalAssets.IndexOf(instanceID);
            }
            if (itemIdx == -1)
            {
                return false;
            }
            if (frame)
            {
                float yOffset = 0f;
                Rect r = this.m_LocalAssets.m_Grid.CalcRect(itemIdx, yOffset);
                this.CenterRect(AdjustRectForFraming(r));
                this.Repaint();
            }
            if (ping)
            {
                this.BeginPing(instanceID);
            }
            return true;
        }

        private void FrameLastClickedItemIfWanted()
        {
            if (this.m_FrameLastClickedItem && (Event.current.type == EventType.Repaint))
            {
                this.m_FrameLastClickedItem = false;
                double num = EditorApplication.timeSinceStartup - this.m_LocalAssets.m_LastClickedDrawTime;
                if ((this.m_State.m_SelectedInstanceIDs.Count > 0) && (num < 0.2))
                {
                    this.Frame(this.m_State.m_LastClickedInstanceID, true, false);
                }
            }
        }

        internal int GetAssetPreviewManagerID()
        {
            return this.m_Owner.GetInstanceID();
        }

        public string GetAssetStoreButtonText()
        {
            string str = "Asset Store";
            if (this.ShowAssetStoreHitsWhileSearchingLocalAssets())
            {
                for (int i = 0; i < this.m_StoreAssets.Count; i++)
                {
                    if (i == 0)
                    {
                        str = str + ": ";
                    }
                    else
                    {
                        str = str + " ∕ ";
                    }
                    AssetStoreGroup group = this.m_StoreAssets[i];
                    str = str + ((group.ItemsAvailable <= 0x3e7) ? group.ItemsAvailable.ToString() : "999+");
                }
            }
            return str;
        }

        private CreateAssetUtility GetCreateAssetUtility()
        {
            return this.m_State.m_CreateAssetUtility;
        }

        protected string GetCroppedLabelText(int instanceID, string fullText, float cropWidth)
        {
            string str;
            if (this.m_WidthUsedForCroppingName != ((int) cropWidth))
            {
                this.ClearCroppedLabelCache();
            }
            if (!this.m_InstanceIDToCroppedNameMap.TryGetValue(instanceID, out str))
            {
                if (this.m_InstanceIDToCroppedNameMap.Count > ((this.GetMaxNumVisibleItems() * 2) + 30))
                {
                    this.ClearCroppedLabelCache();
                }
                int numCharactersThatFitWithinWidth = s_Styles.resultsGridLabel.GetNumCharactersThatFitWithinWidth(fullText, cropWidth);
                if (numCharactersThatFitWithinWidth == -1)
                {
                    this.Repaint();
                    return fullText;
                }
                if ((numCharactersThatFitWithinWidth > 1) && (numCharactersThatFitWithinWidth != fullText.Length))
                {
                    str = fullText.Substring(0, numCharactersThatFitWithinWidth - 1) + "…";
                }
                else
                {
                    str = fullText;
                }
                this.m_InstanceIDToCroppedNameMap[instanceID] = str;
                this.m_WidthUsedForCroppingName = (int) cropWidth;
            }
            return str;
        }

        protected Texture GetIconByInstanceID(int instanceID)
        {
            Texture cachedIcon = null;
            if (instanceID != 0)
            {
                cachedIcon = AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(instanceID));
            }
            return cachedIcon;
        }

        public int GetInstanceIDByIndex(int index)
        {
            int num;
            if (this.m_LocalAssets.InstanceIdAtIndex(index, out num))
            {
                return num;
            }
            return 0;
        }

        private int GetMaxIdx()
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            foreach (Group group in this.m_Groups)
            {
                if (!this.SkipGroup(group) && group.Visible)
                {
                    num2 += num3;
                    num3 = group.m_Grid.rows * group.m_Grid.columns;
                    num = group.ItemCount - 1;
                }
            }
            int num4 = num2 + num;
            return (((num3 + num4) != 0) ? num4 : -1);
        }

        private int GetMaxNumVisibleItems()
        {
            foreach (Group group in this.m_Groups)
            {
                if (!this.SkipGroup(group))
                {
                    return group.m_Grid.GetMaxVisibleItems(this.m_TotalRect.height);
                }
            }
            return 0;
        }

        private RenameOverlay GetRenameOverlay()
        {
            return this.m_State.m_RenameOverlay;
        }

        private int GetSelectedAssetIdx()
        {
            int index = this.m_LocalAssets.IndexOf(this.m_State.m_LastClickedInstanceID);
            if (index != -1)
            {
                return index;
            }
            index = this.m_LocalAssets.m_Grid.rows * this.m_LocalAssets.m_Grid.columns;
            if (AssetStoreAssetSelection.Count != 0)
            {
                AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
                if (firstAsset == null)
                {
                    return -1;
                }
                int id = firstAsset.id;
                foreach (AssetStoreGroup group in this.m_StoreAssets)
                {
                    if (group.Visible)
                    {
                        int num3 = group.IndexOf(id);
                        if (num3 != -1)
                        {
                            return (index + num3);
                        }
                        index += group.m_Grid.rows * group.m_Grid.columns;
                    }
                }
            }
            return -1;
        }

        public int[] GetSelection()
        {
            return this.m_State.m_SelectedInstanceIDs.ToArray();
        }

        internal float GetVisibleWidth()
        {
            return this.m_VisibleRect.width;
        }

        public void HandleKeyboard(bool checkKeyboardControl)
        {
            if ((!checkKeyboardControl || (GUIUtility.keyboardControl == this.m_KeyboardControlID)) && GUI.enabled)
            {
                if (this.m_KeyboardInputCallback != null)
                {
                    this.m_KeyboardInputCallback();
                }
                if (Event.current.type == EventType.KeyDown)
                {
                    int columns = 0;
                    if (this.IsLastClickedItemVisible())
                    {
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.UpArrow:
                                columns = -this.m_LocalAssets.m_Grid.columns;
                                break;

                            case KeyCode.DownArrow:
                                columns = this.m_LocalAssets.m_Grid.columns;
                                break;

                            case KeyCode.RightArrow:
                                if (this.AllowLeftRightArrowNavigation())
                                {
                                    columns = 1;
                                }
                                break;

                            case KeyCode.LeftArrow:
                                if (this.AllowLeftRightArrowNavigation())
                                {
                                    columns = -1;
                                }
                                break;

                            case KeyCode.Home:
                                columns = -2147483648;
                                break;

                            case KeyCode.End:
                                columns = 0x7fffffff;
                                break;

                            case KeyCode.PageUp:
                                columns = -2147483647;
                                break;

                            case KeyCode.PageDown:
                                columns = 0x7ffffffe;
                                break;
                        }
                    }
                    else
                    {
                        bool flag = false;
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.UpArrow:
                            case KeyCode.DownArrow:
                            case KeyCode.Home:
                            case KeyCode.End:
                            case KeyCode.PageUp:
                            case KeyCode.PageDown:
                                flag = true;
                                break;

                            case KeyCode.RightArrow:
                            case KeyCode.LeftArrow:
                                flag = this.AllowLeftRightArrowNavigation();
                                break;
                        }
                        if (flag)
                        {
                            this.SelectFirst();
                            Event.current.Use();
                        }
                    }
                    if (columns != 0)
                    {
                        if ((this.GetSelectedAssetIdx() < 0) && !this.m_LocalAssets.ShowNone)
                        {
                            this.SetSelectedAssetByIdx(0);
                        }
                        else
                        {
                            this.m_SelectionOffset = columns;
                        }
                        Event.current.Use();
                        GUI.changed = true;
                    }
                    else if (this.allowFindNextShortcut && this.m_LocalAssets.DoCharacterOffsetSelection())
                    {
                        Event.current.Use();
                    }
                }
            }
        }

        private void HandleListArea()
        {
            this.SetupData(false);
            if (!this.IsObjectSelector() && !this.m_QueryInProgress)
            {
                if (<>f__am$cache39 == null)
                {
                    <>f__am$cache39 = g => g.NeedItems;
                }
                if (this.m_StoreAssets.Exists(<>f__am$cache39) || this.m_RequeryAssetStore)
                {
                    this.QueryAssetStore();
                }
            }
            float height = 0f;
            foreach (Group group in this.m_Groups)
            {
                if (!this.SkipGroup(group))
                {
                    height += group.Height;
                    if (this.m_LocalAssets.ShowNone)
                    {
                        break;
                    }
                }
            }
            Rect totalRect = this.m_TotalRect;
            Rect viewRect = new Rect(0f, 0f, 1f, height);
            bool flag2 = height > this.m_TotalRect.height;
            this.m_VisibleRect = this.m_TotalRect;
            if (flag2)
            {
                this.m_VisibleRect.width -= 16f;
            }
            double timeSinceStartup = EditorApplication.timeSinceStartup;
            this.m_LastScrollPosition = this.m_State.m_ScrollPosition;
            bool flag3 = false;
            this.m_State.m_ScrollPosition = GUI.BeginScrollView(totalRect, this.m_State.m_ScrollPosition, viewRect);
            Vector2 scrollPosition = this.m_State.m_ScrollPosition;
            if (this.m_LastScrollPosition != this.m_State.m_ScrollPosition)
            {
                this.LastScrollTime = timeSinceStartup;
            }
            float yOffset = 0f;
            foreach (Group group2 in this.m_Groups)
            {
                if (!this.SkipGroup(group2))
                {
                    group2.Draw(yOffset, scrollPosition);
                    flag3 = flag3 || group2.NeedsRepaint;
                    yOffset += group2.Height;
                    if (this.m_LocalAssets.ShowNone)
                    {
                        break;
                    }
                }
            }
            this.HandlePing();
            if (flag3)
            {
                this.Repaint();
            }
            GUI.EndScrollView();
            if ((this.m_ResizePreviewCacheTo > 0) && (AssetStorePreviewManager.MaxCachedImages != this.m_ResizePreviewCacheTo))
            {
                AssetStorePreviewManager.MaxCachedImages = this.m_ResizePreviewCacheTo;
            }
            if (Event.current.type == EventType.Repaint)
            {
                AssetStorePreviewManager.AbortOlderThan(timeSinceStartup);
            }
            if (!this.m_ShowLocalAssetsOnly && !string.IsNullOrEmpty(this.m_AssetStoreError))
            {
                Vector2 vector2 = EditorStyles.label.CalcSize(s_Styles.m_AssetStoreNotAvailableText);
                Rect position = new Rect((this.m_TotalRect.x + 2f) + Mathf.Max((float) 0f, (float) ((this.m_TotalRect.width - vector2.x) * 0.5f)), this.m_TotalRect.y + 10f, vector2.x, 20f);
                EditorGUI.BeginDisabledGroup(true);
                GUI.Label(position, s_Styles.m_AssetStoreNotAvailableText, EditorStyles.label);
                EditorGUI.EndDisabledGroup();
            }
        }

        private void HandlePing()
        {
            if (this.m_Ping.isPinging && !this.m_LocalAssets.ListMode)
            {
                Vector2 vector = this.CalculatePingPosition();
                this.m_Ping.m_ContentRect.x = vector.x;
                this.m_Ping.m_ContentRect.y = vector.y;
            }
            this.m_Ping.HandlePing();
            if (this.m_Ping.isPinging)
            {
                this.Repaint();
            }
        }

        internal void HandleRenameOverlay()
        {
            if (this.GetRenameOverlay().IsRenaming())
            {
                GUIStyle textFieldStyle = !this.IsListMode() ? s_Styles.miniRenameField : null;
                if (!this.GetRenameOverlay().OnGUI(textFieldStyle))
                {
                    this.RenameEnded();
                    GUIUtility.ExitGUI();
                }
            }
        }

        private void HandleUnusedEvents()
        {
            if ((this.allowDeselection && (Event.current.type == EventType.MouseDown)) && ((Event.current.button == 0) && this.m_TotalRect.Contains(Event.current.mousePosition)))
            {
                this.SetSelection(new int[0], false);
            }
        }

        private void HandleZoomScrolling()
        {
            if ((EditorGUI.actionKey && (Event.current.type == EventType.ScrollWheel)) && this.m_TotalRect.Contains(Event.current.mousePosition))
            {
                int num = (Event.current.delta.y <= 0f) ? 1 : -1;
                this.gridSize = Mathf.Clamp(this.gridSize + (num * 7), this.minGridSize, this.maxGridSize);
                if ((num < 0) && (this.gridSize < this.m_MinIconSize))
                {
                    this.gridSize = this.m_MinGridSize;
                }
                if ((num > 0) && (this.gridSize < this.m_MinIconSize))
                {
                    this.gridSize = this.m_MinIconSize;
                }
                Event.current.Use();
                GUI.changed = true;
            }
        }

        private bool HasFocus()
        {
            return (!this.allowFocusRendering || ((this.m_KeyboardControlID == GUIUtility.keyboardControl) && this.m_Owner.m_Parent.hasFocus));
        }

        public void Init(Rect rect, HierarchyType hierarchyType, SearchFilter searchFilter, bool checkThumbnails)
        {
            this.m_TotalRect = this.m_VisibleRect = rect;
            this.m_LocalAssets.UpdateFilter(hierarchyType, searchFilter, this.foldersFirst);
            this.m_LocalAssets.UpdateAssets();
            foreach (AssetStoreGroup group in this.m_StoreAssets)
            {
                group.UpdateFilter(hierarchyType, searchFilter, this.foldersFirst);
            }
            bool flag = searchFilter.GetState() == SearchFilter.State.FolderBrowsing;
            if (flag)
            {
                this.m_LastAssetStoreQuerySearchFilter = string.Empty;
                this.m_LastAssetStoreQueryClassName = new string[0];
                this.m_LastAssetStoreQueryLabels = new string[0];
            }
            else
            {
                this.m_LastAssetStoreQuerySearchFilter = (searchFilter.nameFilter != null) ? searchFilter.nameFilter : string.Empty;
                bool flag2 = (searchFilter.classNames == null) || (Array.IndexOf<string>(searchFilter.classNames, "Object") >= 0);
                this.m_LastAssetStoreQueryClassName = !flag2 ? searchFilter.classNames : new string[0];
                this.m_LastAssetStoreQueryLabels = (searchFilter.assetLabels != null) ? searchFilter.assetLabels : new string[0];
            }
            this.m_LastAssetStoreQueryChangeTime = EditorApplication.timeSinceStartup;
            this.m_RequeryAssetStore = true;
            this.m_ShowLocalAssetsOnly = flag || (searchFilter.GetState() != SearchFilter.State.SearchingInAssetStore);
            this.m_AssetStoreError = string.Empty;
            if (checkThumbnails)
            {
                this.m_AllowThumbnails = this.ObjectsHaveThumbnails(hierarchyType, searchFilter);
            }
            else
            {
                this.m_AllowThumbnails = true;
            }
            this.Repaint();
            this.ClearCroppedLabelCache();
            this.SetupData(true);
        }

        public void InitSelection(int[] selectedInstanceIDs)
        {
            this.m_State.m_SelectedInstanceIDs = new List<int>(selectedInstanceIDs);
            if (this.m_State.m_SelectedInstanceIDs.Count > 0)
            {
                if (!this.m_State.m_SelectedInstanceIDs.Contains(this.m_State.m_LastClickedInstanceID))
                {
                    this.m_State.m_LastClickedInstanceID = this.m_State.m_SelectedInstanceIDs[this.m_State.m_SelectedInstanceIDs.Count - 1];
                }
            }
            else
            {
                this.m_State.m_LastClickedInstanceID = 0;
            }
            if ((Selection.activeObject == null) || (Selection.activeObject.GetType() != typeof(AssetStoreAssetInspector)))
            {
                this.selectedAssetStoreAsset = false;
                AssetStoreAssetSelection.Clear();
            }
        }

        public bool IsLastClickedItemVisible()
        {
            return (this.GetSelectedAssetIdx() >= 0);
        }

        private bool IsListMode()
        {
            if (this.allowMultiSelect)
            {
                return (this.gridSize == 0x10);
            }
            return ((this.gridSize == 0x10) || !this.CanShowThumbnails());
        }

        private bool IsLocalAssetsCurrentlySelected()
        {
            int instanceID = this.m_State.m_SelectedInstanceIDs.FirstOrDefault<int>();
            return ((instanceID != 0) && (this.m_LocalAssets.IndexOf(instanceID) != -1));
        }

        private bool IsObjectSelector()
        {
            return this.m_LocalAssets.ShowNone;
        }

        private bool IsPreviewIconExpansionModifierPressed()
        {
            return Event.current.alt;
        }

        public bool IsSelected(int instanceID)
        {
            return this.m_State.m_SelectedInstanceIDs.Contains(instanceID);
        }

        public bool IsShowing(int instanceID)
        {
            return (this.m_LocalAssets.IndexOf(instanceID) >= 0);
        }

        public bool IsShowingAny(int[] instanceIDs)
        {
            if (instanceIDs.Length != 0)
            {
                foreach (int num in instanceIDs)
                {
                    if (this.IsShowing(num))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ObjectsHaveThumbnails(HierarchyType type, SearchFilter searchFilter)
        {
            if (this.m_LocalAssets.HasBuiltinResources)
            {
                return true;
            }
            FilteredHierarchy filteredHierarchy = new FilteredHierarchy(type) {
                searchFilter = searchFilter
            };
            IHierarchyProperty property = FilteredHierarchyProperty.CreateHierarchyPropertyForFilter(filteredHierarchy);
            int[] expanded = new int[0];
            if (property.CountRemaining(expanded) == 0)
            {
                return true;
            }
            property.Reset();
            while (property.Next(expanded))
            {
                if (property.hasFullPreviewImage)
                {
                    return true;
                }
            }
            return false;
        }

        public void OffsetSelection(int selectionOffset)
        {
            this.m_SelectionOffset = selectionOffset;
        }

        internal void OnDestroy()
        {
            AssetPreview.DeletePreviewTextureManagerByID(this.GetAssetPreviewManagerID());
        }

        public void OnEvent()
        {
            this.GetRenameOverlay().OnEvent();
        }

        public void OnGUI(Rect position, int keyboardControlID)
        {
            int num;
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            s_VCEnabled = Provider.isActive;
            Event current = Event.current;
            this.m_TotalRect = position;
            this.FrameLastClickedItemIfWanted();
            GUI.Label(this.m_TotalRect, GUIContent.none, s_Styles.iconAreaBg);
            this.m_KeyboardControlID = keyboardControlID;
            if ((current.type == EventType.MouseDown) && position.Contains(Event.current.mousePosition))
            {
                GUIUtility.keyboardControl = this.m_KeyboardControlID;
                this.m_AllowRenameOnMouseUp = true;
                this.Repaint();
            }
            bool flag = this.m_KeyboardControlID == GUIUtility.keyboardControl;
            if (flag != this.m_State.m_HadKeyboardFocusLastEvent)
            {
                this.m_State.m_HadKeyboardFocusLastEvent = flag;
                if (flag)
                {
                    if (current.type == EventType.MouseDown)
                    {
                        this.m_AllowRenameOnMouseUp = false;
                    }
                    if (this.m_GotKeyboardFocus != null)
                    {
                        this.m_GotKeyboardFocus();
                    }
                }
            }
            if ((((current.keyCode == KeyCode.Tab) && (current.type == EventType.KeyDown)) && (!flag && !this.IsShowingAny(this.GetSelection()))) && this.m_LocalAssets.InstanceIdAtIndex(0, out num))
            {
                Selection.activeInstanceID = num;
            }
            this.HandleKeyboard(true);
            this.HandleZoomScrolling();
            this.HandleListArea();
            this.DoOffsetSelection();
            this.HandleUnusedEvents();
        }

        public void OnInspectorUpdate()
        {
            if ((EditorApplication.timeSinceStartup > this.m_NextDirtyCheck) && this.m_LocalAssets.IsAnyLastRenderedAssetsDirty())
            {
                AssetPreview.ClearTemporaryAssetPreviews();
                this.Repaint();
                this.m_NextDirtyCheck = EditorApplication.timeSinceStartup + 0.77;
            }
            if (AssetStorePreviewManager.CheckRepaint())
            {
                this.Repaint();
            }
        }

        private void QueryAssetStore()
        {
            <QueryAssetStore>c__AnonStorey38 storey = new <QueryAssetStore>c__AnonStorey38 {
                <>f__this = this
            };
            bool requeryAssetStore = this.m_RequeryAssetStore;
            this.m_RequeryAssetStore = false;
            if (!this.m_ShowLocalAssetsOnly || this.ShowAssetStoreHitsWhileSearchingLocalAssets())
            {
                bool flag2 = ((this.m_LastAssetStoreQuerySearchFilter != string.Empty) || (this.m_LastAssetStoreQueryClassName.Length != 0)) || (this.m_LastAssetStoreQueryLabels.Length != 0);
                if (!this.m_QueryInProgress)
                {
                    if (!flag2)
                    {
                        this.ClearAssetStoreGroups();
                    }
                    else if ((this.m_LastAssetStoreQueryChangeTime + 0.2) > EditorApplication.timeSinceStartup)
                    {
                        this.m_RequeryAssetStore = true;
                        this.Repaint();
                    }
                    else
                    {
                        this.m_QueryInProgress = true;
                        storey.queryFilter = this.m_LastAssetStoreQuerySearchFilter + this.m_LastAssetStoreQueryClassName + this.m_LastAssetStoreQueryLabels;
                        AssetStoreResultBase<AssetStoreSearchResults>.Callback callback = new AssetStoreResultBase<AssetStoreSearchResults>.Callback(storey.<>m__4F);
                        List<AssetStoreClient.SearchCount> counts = new List<AssetStoreClient.SearchCount>();
                        if (!requeryAssetStore)
                        {
                            foreach (AssetStoreGroup group in this.m_StoreAssets)
                            {
                                AssetStoreClient.SearchCount item = new AssetStoreClient.SearchCount();
                                if (group.Visible && group.NeedItems)
                                {
                                    item.offset = group.Assets.Count;
                                    item.limit = group.ItemsWantedShown - item.offset;
                                }
                                item.name = group.Name;
                                counts.Add(item);
                            }
                        }
                        AssetStoreClient.SearchAssets(this.m_LastAssetStoreQuerySearchFilter, this.m_LastAssetStoreQueryClassName, this.m_LastAssetStoreQueryLabels, counts, callback);
                    }
                }
            }
        }

        private void Reflow()
        {
            float num;
            if (this.gridSize < 20)
            {
                this.gridSize = this.m_MinGridSize;
            }
            else if (this.gridSize < this.m_MinIconSize)
            {
                this.gridSize = this.m_MinIconSize;
            }
            if (!this.IsListMode())
            {
                num = 0f;
                foreach (Group group2 in this.m_Groups)
                {
                    if (!this.SkipGroup(group2))
                    {
                        group2.ListMode = false;
                        this.UpdateGroupSizes(group2);
                        num += group2.Height;
                        if (this.m_LocalAssets.ShowNone)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (Group group in this.m_Groups)
                {
                    if (!this.SkipGroup(group))
                    {
                        group.ListMode = true;
                        this.UpdateGroupSizes(group);
                        if (this.m_LocalAssets.ShowNone)
                        {
                            break;
                        }
                    }
                }
                this.m_ResizePreviewCacheTo = Mathf.CeilToInt(this.m_TotalRect.height / 16f) + 10;
                return;
            }
            if (this.m_TotalRect.height < num)
            {
                foreach (Group group3 in this.m_Groups)
                {
                    if (!this.SkipGroup(group3))
                    {
                        group3.m_Grid.fixedWidth = this.m_TotalRect.width - 16f;
                        group3.m_Grid.InitNumRowsAndColumns(group3.ItemCount, group3.m_Grid.CalcRows(group3.ItemsWantedShown));
                        group3.UpdateHeight();
                        if (this.m_LocalAssets.ShowNone)
                        {
                            break;
                        }
                    }
                }
            }
            int maxNumVisibleItems = this.GetMaxNumVisibleItems();
            this.m_ResizePreviewCacheTo = maxNumVisibleItems * 2;
            AssetPreview.SetPreviewTextureCacheSize((maxNumVisibleItems * 2) + 30, this.GetAssetPreviewManagerID());
        }

        private void RenameEnded()
        {
            string name = !string.IsNullOrEmpty(this.GetRenameOverlay().name) ? this.GetRenameOverlay().name : this.GetRenameOverlay().originalName;
            int userData = this.GetRenameOverlay().userData;
            if (this.GetCreateAssetUtility().IsCreatingNewAsset())
            {
                if (this.GetRenameOverlay().userAcceptedRename)
                {
                    this.GetCreateAssetUtility().EndNewAssetCreation(name);
                }
            }
            else if (this.GetRenameOverlay().userAcceptedRename)
            {
                ObjectNames.SetNameSmartWithInstanceID(userData, name);
            }
            if (this.GetRenameOverlay().HasKeyboardFocus())
            {
                GUIUtility.keyboardControl = this.m_KeyboardControlID;
            }
            if (this.GetRenameOverlay().userAcceptedRename)
            {
                this.Frame(userData, true, false);
            }
            this.ClearRenameState();
        }

        private void Repaint()
        {
            if (this.m_RepaintWantedCallback != null)
            {
                this.m_RepaintWantedCallback();
            }
        }

        private void RequeryAssetStore()
        {
            this.m_RequeryAssetStore = true;
        }

        private void Reveal(int instanceID)
        {
            if (AssetDatabase.Contains(instanceID))
            {
                int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(AssetDatabase.GetAssetPath(instanceID));
                if (mainAssetInstanceID != instanceID)
                {
                    this.m_LocalAssets.ChangeExpandedState(mainAssetInstanceID, true);
                }
            }
        }

        private void ScrollToPosition(Rect r)
        {
            float y = r.y;
            float yMax = r.yMax;
            float height = this.m_TotalRect.height;
            if (yMax > (height + this.m_State.m_ScrollPosition.y))
            {
                this.m_State.m_ScrollPosition.y = yMax - height;
            }
            if (y < this.m_State.m_ScrollPosition.y)
            {
                this.m_State.m_ScrollPosition.y = y;
            }
            this.m_State.m_ScrollPosition.y = Mathf.Max(this.m_State.m_ScrollPosition.y, 0f);
        }

        public void SelectAll()
        {
            this.SetSelection(this.m_LocalAssets.GetInstanceIDs().ToArray(), false);
        }

        public void SelectFirst()
        {
            int selectedIdx = 0;
            if ((this.m_ShowLocalAssetsOnly && this.m_LocalAssets.ShowNone) && (this.m_LocalAssets.ItemCount > 1))
            {
                selectedIdx = 1;
            }
            this.SetSelectedAssetByIdx(selectedIdx);
        }

        private void SetSelectedAssetByIdx(int selectedIdx)
        {
            int num;
            if (this.m_LocalAssets.InstanceIdAtIndex(selectedIdx, out num))
            {
                int[] numArray;
                Rect r = this.m_LocalAssets.m_Grid.CalcRect(selectedIdx, 0f);
                this.ScrollToPosition(AdjustRectForFraming(r));
                this.Repaint();
                if (this.IsLocalAssetsCurrentlySelected())
                {
                    numArray = this.m_LocalAssets.GetNewSelection(num, false, true).ToArray();
                }
                else
                {
                    numArray = new int[] { num };
                }
                this.SetSelection(numArray, false);
                this.m_State.m_LastClickedInstanceID = num;
            }
            else
            {
                selectedIdx -= this.m_LocalAssets.m_Grid.rows * this.m_LocalAssets.m_Grid.columns;
                float height = this.m_LocalAssets.Height;
                foreach (AssetStoreGroup group in this.m_StoreAssets)
                {
                    if (!group.Visible)
                    {
                        height += group.Height;
                    }
                    else
                    {
                        AssetStoreAsset assetStoreResult = group.AssetAtIndex(selectedIdx);
                        if (assetStoreResult != null)
                        {
                            Rect rect2 = group.m_Grid.CalcRect(selectedIdx, height);
                            this.ScrollToPosition(AdjustRectForFraming(rect2));
                            this.Repaint();
                            this.SetSelection(assetStoreResult, false);
                            break;
                        }
                        selectedIdx -= group.m_Grid.rows * group.m_Grid.columns;
                        height += group.Height;
                    }
                }
            }
        }

        private void SetSelection(int[] selectedInstanceIDs, bool doubleClicked)
        {
            this.InitSelection(selectedInstanceIDs);
            if (this.m_ItemSelectedCallback != null)
            {
                this.Repaint();
                this.m_ItemSelectedCallback(doubleClicked);
            }
        }

        private void SetSelection(AssetStoreAsset assetStoreResult, bool doubleClicked)
        {
            this.m_State.m_SelectedInstanceIDs.Clear();
            this.selectedAssetStoreAsset = true;
            AssetStoreAssetSelection.Clear();
            Texture2D image = AssetStorePreviewManager.TextureFromUrl(assetStoreResult.staticPreviewURL, assetStoreResult.name, this.gridSize, s_Styles.resultsGridLabel, s_Styles.resultsGrid, true).image;
            AssetStoreAssetSelection.AddAsset(assetStoreResult, image);
            if (this.m_ItemSelectedCallback != null)
            {
                this.Repaint();
                this.m_ItemSelectedCallback(doubleClicked);
            }
        }

        private void SetupData(bool forceReflow)
        {
            foreach (Group group in this.m_Groups)
            {
                if (!this.SkipGroup(group))
                {
                    group.UpdateAssets();
                }
            }
            if (forceReflow || (Event.current.type == EventType.Repaint))
            {
                this.Reflow();
            }
        }

        public void ShowAssetStoreHitCountWhileSearchingLocalAssetsChanged()
        {
            if (this.ShowAssetStoreHitsWhileSearchingLocalAssets())
            {
                this.RequeryAssetStore();
            }
            else if (this.m_ShowLocalAssetsOnly)
            {
                this.ClearAssetStoreGroups();
            }
            this.Repaint();
        }

        private bool ShowAssetStoreHitsWhileSearchingLocalAssets()
        {
            return EditorPrefs.GetBool("ShowAssetStoreSearchHits", true);
        }

        public void ShowObjectsInList(int[] instanceIDs)
        {
            this.Init(this.m_TotalRect, HierarchyType.Assets, new SearchFilter(), false);
            this.m_LocalAssets.ShowObjectsInList(instanceIDs);
        }

        private bool SkipGroup(Group group)
        {
            if (this.m_ShowLocalAssetsOnly)
            {
                if (group is AssetStoreGroup)
                {
                    return true;
                }
            }
            else if (group is LocalGroup)
            {
                return true;
            }
            return false;
        }

        private void UpdateGroupSizes(Group g)
        {
            if (g.ListMode)
            {
                g.m_Grid.fixedWidth = this.m_VisibleRect.width;
                g.m_Grid.itemSize = new Vector2(this.m_VisibleRect.width, 16f);
                g.m_Grid.topMargin = 0f;
                g.m_Grid.bottomMargin = 0f;
                g.m_Grid.leftMargin = 0f;
                g.m_Grid.rightMargin = 0f;
                g.m_Grid.verticalSpacing = 0f;
                g.m_Grid.minHorizontalSpacing = 0f;
                g.m_Grid.InitNumRowsAndColumns(g.ItemCount, g.ItemsWantedShown);
                g.UpdateHeight();
            }
            else
            {
                g.m_Grid.fixedWidth = this.m_TotalRect.width;
                g.m_Grid.itemSize = new Vector2((float) this.gridSize, (float) (this.gridSize + 14));
                g.m_Grid.topMargin = 10f;
                g.m_Grid.bottomMargin = 10f;
                g.m_Grid.leftMargin = 10f;
                g.m_Grid.rightMargin = 10f;
                g.m_Grid.verticalSpacing = 15f;
                g.m_Grid.minHorizontalSpacing = 12f;
                g.m_Grid.InitNumRowsAndColumns(g.ItemCount, g.m_Grid.CalcRows(g.ItemsWantedShown));
                g.UpdateHeight();
            }
        }

        public bool allowBuiltinResources { get; set; }

        public bool allowDeselection { get; set; }

        public bool allowDragging { get; set; }

        public bool allowFindNextShortcut { get; set; }

        public bool allowFocusRendering { get; set; }

        public bool allowMultiSelect { get; set; }

        public bool allowRenaming { get; set; }

        public bool allowUserRenderingHook { get; set; }

        public Action assetStoreSearchEnded
        {
            get
            {
                return this.m_AssetStoreSearchEnded;
            }
            set
            {
                this.m_AssetStoreSearchEnded = value;
            }
        }

        public Func<Rect, float> drawLocalAssetHeader
        {
            get
            {
                return this.m_DrawLocalAssetHeader;
            }
            set
            {
                this.m_DrawLocalAssetHeader = value;
            }
        }

        public bool foldersFirst { get; set; }

        public Action gotKeyboardFocus
        {
            get
            {
                return this.m_GotKeyboardFocus;
            }
            set
            {
                this.m_GotKeyboardFocus = value;
            }
        }

        public int gridSize
        {
            get
            {
                return this.m_State.m_GridSize;
            }
            set
            {
                if (this.m_State.m_GridSize != value)
                {
                    this.m_State.m_GridSize = value;
                    this.m_FrameLastClickedItem = true;
                }
            }
        }

        public Action<bool> itemSelectedCallback
        {
            get
            {
                return this.m_ItemSelectedCallback;
            }
            set
            {
                this.m_ItemSelectedCallback = value;
            }
        }

        public Action keyboardCallback
        {
            get
            {
                return this.m_KeyboardInputCallback;
            }
            set
            {
                this.m_KeyboardInputCallback = value;
            }
        }

        public int maxGridSize
        {
            get
            {
                return this.m_MaxGridSize;
            }
        }

        public int minGridSize
        {
            get
            {
                return this.m_MinGridSize;
            }
        }

        public int numItemsDisplayed
        {
            get
            {
                return this.m_LocalAssets.ItemCount;
            }
        }

        public Action repaintCallback
        {
            get
            {
                return this.m_RepaintWantedCallback;
            }
            set
            {
                this.m_RepaintWantedCallback = value;
            }
        }

        [CompilerGenerated]
        private sealed class <BeginPing>c__AnonStorey3B
        {
            internal ObjectListArea.<BeginPing>c__AnonStorey3C <>f__ref$60;
            internal FilteredHierarchy.FilterResult res;

            internal void <>m__51(Rect r)
            {
                ObjectListArea.LocalGroup.DrawIconAndLabel(r, this.res, this.<>f__ref$60.label, this.<>f__ref$60.hierarchyProperty.icon, false, false);
            }
        }

        [CompilerGenerated]
        private sealed class <BeginPing>c__AnonStorey3C
        {
            internal HierarchyProperty hierarchyProperty;
            internal string label;

            internal void <>m__52(Rect r)
            {
                TextAnchor alignment = ObjectListArea.s_Styles.resultsGridLabel.alignment;
                ObjectListArea.s_Styles.resultsGridLabel.alignment = TextAnchor.UpperLeft;
                ObjectListArea.s_Styles.resultsGridLabel.Draw(r, this.label, false, false, false, false);
                ObjectListArea.s_Styles.resultsGridLabel.alignment = alignment;
            }
        }

        [CompilerGenerated]
        private sealed class <QueryAssetStore>c__AnonStorey38
        {
            internal ObjectListArea <>f__this;
            internal string queryFilter;

            internal void <>m__4F(AssetStoreSearchResults results)
            {
                this.<>f__this.m_QueryInProgress = false;
                if (this.queryFilter != (this.<>f__this.m_LastAssetStoreQuerySearchFilter + this.<>f__this.m_LastAssetStoreQueryClassName + this.<>f__this.m_LastAssetStoreQueryLabels))
                {
                    this.<>f__this.m_RequeryAssetStore = true;
                }
                if ((results.error != null) && (results.error != string.Empty))
                {
                    if (ObjectListArea.s_Debug)
                    {
                        Debug.LogError("Error performing Asset Store search: " + results.error);
                    }
                    else
                    {
                        Console.Write("Error performing Asset Store search: " + results.error);
                    }
                    this.<>f__this.m_AssetStoreError = results.error;
                    this.<>f__this.m_Groups.Clear();
                    this.<>f__this.m_Groups.Add(this.<>f__this.m_LocalAssets);
                    this.<>f__this.Repaint();
                    if (this.<>f__this.assetStoreSearchEnded != null)
                    {
                        this.<>f__this.assetStoreSearchEnded();
                    }
                }
                else
                {
                    this.<>f__this.m_AssetStoreError = string.Empty;
                    List<string> list = new List<string>();
                    foreach (ObjectListArea.AssetStoreGroup group in this.<>f__this.m_StoreAssets)
                    {
                        list.Add(group.Name);
                    }
                    this.<>f__this.m_Groups.Clear();
                    this.<>f__this.m_Groups.Add(this.<>f__this.m_LocalAssets);
                    <QueryAssetStore>c__AnonStorey39 storey = new <QueryAssetStore>c__AnonStorey39();
                    using (List<AssetStoreSearchResults.Group>.Enumerator enumerator2 = results.groups.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            storey.inGroup = enumerator2.Current;
                            list.Remove(storey.inGroup.name);
                            ObjectListArea.AssetStoreGroup item = this.<>f__this.m_StoreAssets.Find(new Predicate<ObjectListArea.AssetStoreGroup>(storey.<>m__53));
                            if (item == null)
                            {
                                item = new ObjectListArea.AssetStoreGroup(this.<>f__this, storey.inGroup.label, storey.inGroup.name);
                                this.<>f__this.m_StoreAssets.Add(item);
                            }
                            this.<>f__this.m_Groups.Add(item);
                            if (storey.inGroup.limit != 0)
                            {
                                item.ItemsAvailable = storey.inGroup.totalFound;
                            }
                            if ((storey.inGroup.offset == 0) && (storey.inGroup.limit != 0))
                            {
                                item.Assets = storey.inGroup.assets;
                            }
                            else
                            {
                                item.Assets.AddRange(storey.inGroup.assets);
                            }
                        }
                    }
                    <QueryAssetStore>c__AnonStorey3A storeya = new <QueryAssetStore>c__AnonStorey3A();
                    using (List<string>.Enumerator enumerator3 = list.GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            storeya.k = enumerator3.Current;
                            this.<>f__this.m_StoreAssets.RemoveAll(new Predicate<ObjectListArea.AssetStoreGroup>(storeya.<>m__54));
                        }
                    }
                    this.<>f__this.EnsureAssetStoreGroupsAreOpenIfAllClosed();
                    this.<>f__this.Repaint();
                    if (this.<>f__this.assetStoreSearchEnded != null)
                    {
                        this.<>f__this.assetStoreSearchEnded();
                    }
                }
            }

            private sealed class <QueryAssetStore>c__AnonStorey39
            {
                internal AssetStoreSearchResults.Group inGroup;

                internal bool <>m__53(ObjectListArea.AssetStoreGroup g)
                {
                    return (g.Name == this.inGroup.name);
                }
            }

            private sealed class <QueryAssetStore>c__AnonStorey3A
            {
                internal string k;

                internal bool <>m__54(ObjectListArea.AssetStoreGroup g)
                {
                    return (g.Name == this.k);
                }
            }
        }

        private class AssetStoreGroup : ObjectListArea.Group
        {
            public const int kDefaultRowsShown = 3;
            public const int kDefaultRowsShownListMode = 10;
            private const int kMaxQueryItems = 0x3e8;
            private const int kMoreButtonOffset = 3;
            private const int kMoreRowsAdded = 10;
            private const int kMoreRowsAddedListMode = 0x4b;
            private List<AssetStoreAsset> m_Assets;
            private GUIContent m_Content;
            private bool m_ListMode;
            private string m_Name;
            private Vector3 m_ShowMoreDims;

            public AssetStoreGroup(ObjectListArea owner, string groupTitle, string groupName) : base(owner, groupTitle)
            {
                this.m_Content = new GUIContent();
                this.m_Assets = new List<AssetStoreAsset>();
                this.m_Name = groupName;
                this.m_ListMode = false;
                this.m_ShowMoreDims = (Vector3) EditorStyles.miniButton.CalcSize(new GUIContent("Show more"));
                base.m_Owner.UpdateGroupSizes(this);
                base.ItemsWantedShown = 3 * base.m_Grid.columns;
            }

            public AssetStoreAsset AssetAtIndex(int selectedIdx)
            {
                if (selectedIdx < (base.m_Grid.rows * base.m_Grid.columns))
                {
                    if ((selectedIdx < (base.m_Grid.rows * base.m_Grid.columns)) && (selectedIdx > this.ItemCount))
                    {
                        return this.m_Assets.Last<AssetStoreAsset>();
                    }
                    int num = 0;
                    foreach (AssetStoreAsset asset in this.m_Assets)
                    {
                        if (selectedIdx == num)
                        {
                            return asset;
                        }
                        num++;
                    }
                }
                return null;
            }

            private void DrawIcon(Rect position, AssetStoreAsset assetStoreResource)
            {
                bool on = false;
                this.m_Content.text = null;
                AssetStorePreviewManager.CachedAssetStoreImage iconForAssetStoreAsset = this.GetIconForAssetStoreAsset(assetStoreResource);
                if (iconForAssetStoreAsset == null)
                {
                    Texture2D iconForFile = InternalEditorUtility.GetIconForFile(assetStoreResource.name);
                    ObjectListArea.s_Styles.resultsGrid.Draw(position, iconForFile, false, false, on, on);
                }
                else
                {
                    this.m_Content.image = iconForAssetStoreAsset.image;
                    Color color = iconForAssetStoreAsset.color;
                    Color color2 = GUI.color;
                    if (color.a != 1f)
                    {
                        GUI.color = color;
                    }
                    ObjectListArea.s_Styles.resultsGrid.Draw(position, this.m_Content, false, false, on, on);
                    if (color.a != 1f)
                    {
                        GUI.color = color2;
                        this.NeedsRepaint = true;
                    }
                    base.DrawDropShadowOverlay(position, on, false, false);
                }
            }

            protected override void DrawInternal(int itemIdx, int endItem, float yOffset)
            {
                Rect rect;
                int count = this.m_Assets.Count;
                int num2 = itemIdx;
                yOffset += base.kGroupSeparatorHeight;
                bool flag = Event.current.type == EventType.Repaint;
                if (this.ListMode)
                {
                    while ((itemIdx < endItem) && (itemIdx < count))
                    {
                        rect = base.m_Grid.CalcRect(itemIdx, yOffset);
                        int num3 = this.HandleMouse(rect);
                        if (num3 != 0)
                        {
                            base.m_Owner.SetSelection(this.m_Assets[itemIdx], num3 == 2);
                        }
                        if (flag)
                        {
                            bool selected = !AssetStoreAssetSelection.Empty && AssetStoreAssetSelection.ContainsAsset(this.m_Assets[itemIdx].id);
                            this.DrawLabel(rect, this.m_Assets[itemIdx], selected);
                        }
                        itemIdx++;
                    }
                }
                else
                {
                    while ((itemIdx < endItem) && (itemIdx < count))
                    {
                        rect = base.m_Grid.CalcRect(itemIdx, yOffset);
                        int num4 = this.HandleMouse(rect);
                        if (num4 != 0)
                        {
                            base.m_Owner.SetSelection(this.m_Assets[itemIdx], num4 == 2);
                        }
                        if (flag)
                        {
                            Rect position = new Rect(rect.x, rect.y, rect.width, rect.height - ObjectListArea.s_Styles.resultsGridLabel.fixedHeight);
                            this.DrawIcon(position, this.m_Assets[itemIdx]);
                        }
                        itemIdx++;
                    }
                    itemIdx = num2;
                    if (flag)
                    {
                        while ((itemIdx < endItem) && (itemIdx < count))
                        {
                            rect = base.m_Grid.CalcRect(itemIdx, yOffset);
                            bool flag3 = !AssetStoreAssetSelection.Empty && AssetStoreAssetSelection.ContainsAsset(this.m_Assets[itemIdx].id);
                            this.DrawLabel(rect, this.m_Assets[itemIdx], flag3);
                            itemIdx++;
                        }
                    }
                }
                if (base.ItemsAvailable > (base.m_Grid.rows * base.m_Grid.columns))
                {
                    rect = new Rect((base.m_Owner.GetVisibleWidth() - this.m_ShowMoreDims.x) - 6f, (yOffset + base.m_Grid.height) + 3f, this.m_ShowMoreDims.x, this.m_ShowMoreDims.y);
                    if (((base.ItemsAvailable > (base.m_Grid.rows * base.m_Grid.columns)) && (base.ItemsAvailable >= this.Assets.Count)) && (this.Assets.Count < 0x3e8))
                    {
                        Event current = Event.current;
                        switch (current.type)
                        {
                            case EventType.MouseDown:
                                if ((current.button == 0) && rect.Contains(current.mousePosition))
                                {
                                    if (this.ListMode)
                                    {
                                        base.ItemsWantedShown += 0x4b;
                                    }
                                    else
                                    {
                                        int num5 = base.m_Grid.columns - (this.ItemCount % base.m_Grid.columns);
                                        num5 = num5 % base.m_Grid.columns;
                                        base.ItemsWantedShown += (10 * base.m_Grid.columns) + num5;
                                    }
                                    if (this.NeedItems)
                                    {
                                        base.m_Owner.QueryAssetStore();
                                    }
                                    current.Use();
                                }
                                break;

                            case EventType.Repaint:
                                EditorStyles.miniButton.Draw(rect, "More", false, false, false, false);
                                break;
                        }
                    }
                }
            }

            private void DrawLabel(Rect position, AssetStoreAsset assetStoreResource, bool selected)
            {
                if (this.ListMode)
                {
                    position.width = Mathf.Max(position.width, 500f);
                    this.m_Content.text = assetStoreResource.displayName;
                    this.m_Content.image = InternalEditorUtility.GetIconForFile(assetStoreResource.name);
                    ObjectListArea.s_Styles.resultsLabel.Draw(position, this.m_Content, false, false, selected, selected);
                }
                else
                {
                    int instanceID = assetStoreResource.id + 0x989680;
                    string text = base.m_Owner.GetCroppedLabelText(instanceID, assetStoreResource.displayName, position.width);
                    position.height -= ObjectListArea.s_Styles.resultsGridLabel.fixedHeight;
                    ObjectListArea.s_Styles.resultsGridLabel.Draw(new Rect(position.x, position.yMax + 1f, position.width - 1f, ObjectListArea.s_Styles.resultsGridLabel.fixedHeight), text, false, false, selected, base.m_Owner.HasFocus());
                }
            }

            private AssetStorePreviewManager.CachedAssetStoreImage GetIconForAssetStoreAsset(AssetStoreAsset assetStoreResource)
            {
                if (!string.IsNullOrEmpty(assetStoreResource.staticPreviewURL))
                {
                    base.m_Owner.LastScrollTime++;
                    return AssetStorePreviewManager.TextureFromUrl(assetStoreResource.staticPreviewURL, assetStoreResource.name, base.m_Owner.gridSize, ObjectListArea.s_Styles.resultsGridLabel, ObjectListArea.s_Styles.previewBg, false);
                }
                return null;
            }

            protected int HandleMouse(Rect position)
            {
                Event current = Event.current;
                EventType type = current.type;
                if (type != EventType.MouseDown)
                {
                    if ((type == EventType.ContextClick) && position.Contains(current.mousePosition))
                    {
                        return 1;
                    }
                }
                else if ((current.button == 0) && position.Contains(current.mousePosition))
                {
                    base.m_Owner.Repaint();
                    if (current.clickCount == 2)
                    {
                        current.Use();
                        return 2;
                    }
                    base.m_Owner.ScrollToPosition(ObjectListArea.AdjustRectForFraming(position));
                    current.Use();
                    return 1;
                }
                return 0;
            }

            public int IndexOf(int assetID)
            {
                int num = 0;
                foreach (AssetStoreAsset asset in this.m_Assets)
                {
                    if (asset.id == assetID)
                    {
                        return num;
                    }
                    num++;
                }
                return -1;
            }

            public override void UpdateAssets()
            {
            }

            public override void UpdateFilter(HierarchyType hierarchyType, SearchFilter searchFilter, bool showFoldersFirst)
            {
                base.ItemsWantedShown = !this.ListMode ? (3 * base.m_Grid.columns) : 10;
                this.Assets.Clear();
            }

            public override void UpdateHeight()
            {
                base.m_Height = (int) base.kGroupSeparatorHeight;
                if (base.Visible)
                {
                    base.m_Height += base.m_Grid.height;
                    if (base.ItemsAvailable > (base.m_Grid.rows * base.m_Grid.columns))
                    {
                        base.m_Height += 6 + ((int) this.m_ShowMoreDims.y);
                    }
                }
            }

            public List<AssetStoreAsset> Assets
            {
                get
                {
                    return this.m_Assets;
                }
                set
                {
                    this.m_Assets = value;
                }
            }

            public override int ItemCount
            {
                get
                {
                    return Math.Min(this.m_Assets.Count, base.ItemsWantedShown);
                }
            }

            public override bool ListMode
            {
                get
                {
                    return this.m_ListMode;
                }
                set
                {
                    this.m_ListMode = value;
                }
            }

            public string Name
            {
                get
                {
                    return this.m_Name;
                }
            }

            public bool NeedItems
            {
                get
                {
                    int num = Math.Min(0x3e8, base.ItemsWantedShown);
                    int count = this.Assets.Count;
                    return (((base.ItemsAvailable >= num) && (count < num)) || ((base.ItemsAvailable < num) && (count < base.ItemsAvailable)));
                }
            }

            public override bool NeedsRepaint { get; protected set; }
        }

        private abstract class Group
        {
            public int ItemsAvailable;
            public int ItemsWantedShown;
            protected readonly float kGroupSeparatorHeight = EditorStyles.toolbar.fixedHeight;
            protected bool m_Collapsable = true;
            public VerticalGrid m_Grid = new VerticalGrid();
            protected string m_GroupSeparatorTitle;
            public float m_Height;
            public double m_LastClickedDrawTime;
            public ObjectListArea m_Owner;
            protected static int[] s_Empty;
            public bool Visible = true;

            public Group(ObjectListArea owner, string groupTitle)
            {
                this.m_GroupSeparatorTitle = groupTitle;
                if (s_Empty == null)
                {
                    s_Empty = new int[0];
                }
                this.m_Owner = owner;
                this.Visible = this.visiblePreference;
            }

            public void Draw(float yOffset, Vector2 scrollPos)
            {
                this.NeedsRepaint = false;
                bool flag = (Event.current.type == EventType.Repaint) || (Event.current.type == EventType.Layout);
                if (!flag)
                {
                    this.DrawHeader(yOffset, this.m_Collapsable);
                }
                if (this.IsInView(yOffset, scrollPos, this.m_Owner.m_VisibleRect.height))
                {
                    int num2 = this.FirstVisibleRow(yOffset, scrollPos) * this.m_Grid.columns;
                    int itemCount = this.ItemCount;
                    if ((num2 >= 0) && (num2 < itemCount))
                    {
                        int itemIdx = num2;
                        int num5 = Math.Min(itemCount, this.m_Grid.rows * this.m_Grid.columns);
                        float num6 = this.m_Grid.itemSize.y + this.m_Grid.verticalSpacing;
                        int num7 = (int) Math.Ceiling((double) (this.m_Owner.m_VisibleRect.height / num6));
                        num5 = Math.Min(num5, (itemIdx + (num7 * this.m_Grid.columns)) + this.m_Grid.columns);
                        this.DrawInternal(itemIdx, num5, yOffset);
                    }
                    if (flag)
                    {
                        this.DrawHeader(yOffset, this.m_Collapsable);
                    }
                    this.HandleUnusedDragEvents(yOffset);
                }
            }

            protected void DrawDropShadowOverlay(Rect position, bool selected, bool isDropTarget, bool isRenaming)
            {
                float num = position.width / 128f;
                Rect rect = new Rect(position.x - (4f * num), position.y - (2f * num), position.width + (8f * num), (position.height + (12f * num)) - 0.5f);
                ObjectListArea.s_Styles.iconDropShadow.Draw(rect, GUIContent.none, false, false, selected || isDropTarget, (this.m_Owner.HasFocus() || isRenaming) || isDropTarget);
            }

            protected virtual void DrawHeader(float yOffset, bool collapsable)
            {
                Rect rect = new Rect(0f, this.GetHeaderYPosInScrollArea(yOffset), this.m_Owner.GetVisibleWidth(), this.kGroupSeparatorHeight - 1f);
                this.DrawHeaderBackground(rect, yOffset == 0f);
                rect.x += 7f;
                if (collapsable)
                {
                    bool visible = this.Visible;
                    this.Visible = GUI.Toggle(rect, this.Visible, GUIContent.none, ObjectListArea.s_Styles.groupFoldout);
                    if (visible ^ this.Visible)
                    {
                        this.visiblePreference = this.Visible;
                    }
                }
                GUIStyle groupHeaderLabel = ObjectListArea.s_Styles.groupHeaderLabel;
                if (collapsable)
                {
                    rect.x += 14f;
                }
                rect.y++;
                if (!string.IsNullOrEmpty(this.m_GroupSeparatorTitle))
                {
                    GUI.Label(rect, this.m_GroupSeparatorTitle, groupHeaderLabel);
                }
                if (ObjectListArea.s_Debug)
                {
                    Rect position = rect;
                    position.x += 120f;
                    GUI.Label(position, AssetStorePreviewManager.StatsString());
                }
                rect.y--;
                if (this.m_Owner.GetVisibleWidth() > 150f)
                {
                    this.DrawItemCount(rect);
                }
            }

            protected void DrawHeaderBackground(Rect rect, bool firstHeader)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUI.Label(rect, GUIContent.none, !firstHeader ? ObjectListArea.s_Styles.groupHeaderMiddle : ObjectListArea.s_Styles.groupHeaderTop);
                }
            }

            protected abstract void DrawInternal(int itemIdx, int endItem, float yOffset);
            protected void DrawItemCount(Rect rect)
            {
                string text = this.ItemsAvailable.ToString() + " Total";
                Vector2 vector = ObjectListArea.s_Styles.groupHeaderLabelCount.CalcSize(new GUIContent(text));
                if (vector.x < rect.width)
                {
                    rect.x = (this.m_Owner.GetVisibleWidth() - vector.x) - 4f;
                }
                rect.width = vector.x;
                rect.y += 2f;
                GUI.Label(rect, text, ObjectListArea.s_Styles.groupHeaderLabelCount);
            }

            protected void DrawObjectIcon(Rect position, Texture icon)
            {
                if (icon != null)
                {
                    int width = icon.width;
                    FilterMode filterMode = icon.filterMode;
                    icon.filterMode = FilterMode.Point;
                    GUI.DrawTexture(new Rect(position.x + ((((int) position.width) - width) / 2), position.y + ((((int) position.height) - width) / 2), (float) width, (float) width), icon, ScaleMode.ScaleToFit);
                    icon.filterMode = filterMode;
                }
            }

            private int FirstVisibleRow(float yOffset, Vector2 scrollPos)
            {
                if (!this.Visible)
                {
                    return -1;
                }
                float num = scrollPos.y - (yOffset + this.GetHeaderHeight());
                int num2 = 0;
                if (num > 0f)
                {
                    float num3 = this.m_Grid.itemSize.y + this.m_Grid.verticalSpacing;
                    num2 = (int) Mathf.Max(0f, Mathf.Floor(num / num3));
                }
                return num2;
            }

            protected virtual float GetHeaderHeight()
            {
                return this.kGroupSeparatorHeight;
            }

            protected float GetHeaderYPosInScrollArea(float yOffset)
            {
                float num = yOffset;
                float y = this.m_Owner.m_State.m_ScrollPosition.y;
                if (y > yOffset)
                {
                    num = Mathf.Min(y, (yOffset + this.Height) - this.kGroupSeparatorHeight);
                }
                return num;
            }

            private static string[] GetMainSelectedPaths()
            {
                List<string> list = new List<string>();
                foreach (int num in Selection.instanceIDs)
                {
                    if (AssetDatabase.IsMainAsset(num))
                    {
                        string assetPath = AssetDatabase.GetAssetPath(num);
                        list.Add(assetPath);
                    }
                }
                return list.ToArray();
            }

            private Object[] GetSelectedReferences()
            {
                return Selection.objects;
            }

            protected virtual void HandleUnusedDragEvents(float yOffset)
            {
            }

            private bool IsInView(float yOffset, Vector2 scrollPos, float scrollViewHeight)
            {
                if ((scrollPos.y + scrollViewHeight) < yOffset)
                {
                    return false;
                }
                if ((yOffset + this.Height) < scrollPos.y)
                {
                    return false;
                }
                return true;
            }

            public abstract void UpdateAssets();
            public abstract void UpdateFilter(HierarchyType hierarchyType, SearchFilter searchFilter, bool showFoldersFirst);
            public abstract void UpdateHeight();

            public float Height
            {
                get
                {
                    return this.m_Height;
                }
            }

            public abstract int ItemCount { get; }

            public abstract bool ListMode { get; set; }

            public abstract bool NeedsRepaint { get; protected set; }

            public bool visiblePreference
            {
                get
                {
                    return (string.IsNullOrEmpty(this.m_GroupSeparatorTitle) || EditorPrefs.GetBool(this.m_GroupSeparatorTitle, true));
                }
                set
                {
                    if (!string.IsNullOrEmpty(this.m_GroupSeparatorTitle))
                    {
                        EditorPrefs.SetBool(this.m_GroupSeparatorTitle, value);
                    }
                }
            }
        }

        private class LocalGroup : ObjectListArea.Group
        {
            public const int k_ListModeLeftPadding = 0x10;
            public const int k_ListModeLeftPaddingForSubAssets = 0x1c;
            public const int k_ListModeVersionControlOverlayPadding = 14;
            private BuiltinResource[] m_ActiveBuiltinList;
            private Dictionary<string, BuiltinResource[]> m_BuiltinResourceMap;
            private GUIContent m_Content;
            private BuiltinResource[] m_CurrentBuiltinResources;
            private List<int> m_DragSelection;
            private int m_DropTargetControlID;
            private FilteredHierarchy m_FilteredHierarchy;
            private ItemFader m_ItemFader;
            private List<int> m_LastRenderedAssetDirtyIDs;
            private List<int> m_LastRenderedAssetInstanceIDs;
            public bool m_ListMode;
            private BuiltinResource[] m_NoneList;
            private bool m_ShowNoneItem;

            public LocalGroup(ObjectListArea owner, string groupTitle, bool showNone) : base(owner, groupTitle)
            {
                this.m_Content = new GUIContent();
                this.m_DragSelection = new List<int>();
                this.m_LastRenderedAssetInstanceIDs = new List<int>();
                this.m_LastRenderedAssetDirtyIDs = new List<int>();
                this.m_ItemFader = new ItemFader();
                this.m_ShowNoneItem = showNone;
                this.m_ListMode = false;
                this.InitBuiltinResources();
                base.ItemsWantedShown = 0x7fffffff;
                base.m_Collapsable = false;
            }

            private static Rect ActualImageDrawPosition(Rect position, float imageWidth, float imageHeight)
            {
                if ((imageWidth > position.width) || (imageHeight > position.height))
                {
                    Rect outScreenRect = new Rect();
                    Rect outSourceRect = new Rect();
                    float imageAspect = imageWidth / imageHeight;
                    GUI.CalculateScaledTextureRects(position, ScaleMode.ScaleToFit, imageAspect, ref outScreenRect, ref outSourceRect);
                    return outScreenRect;
                }
                float x = position.x + Mathf.Round((position.width - imageWidth) / 2f);
                return new Rect(x, position.y + Mathf.Round((position.height - imageHeight) / 2f), imageWidth, imageHeight);
            }

            private void AddDirtyStateFor(int instanceID)
            {
                this.m_LastRenderedAssetInstanceIDs.Add(instanceID);
                this.m_LastRenderedAssetDirtyIDs.Add(EditorUtility.GetDirtyIndex(instanceID));
            }

            private void BeginPing(int instanceID)
            {
            }

            public void ChangeExpandedState(int instanceID, bool expanded)
            {
                base.m_Owner.m_State.m_ExpandedInstanceIDs.Remove(instanceID);
                if (expanded)
                {
                    base.m_Owner.m_State.m_ExpandedInstanceIDs.Add(instanceID);
                }
                this.m_FilteredHierarchy.RefreshVisibleItems(base.m_Owner.m_State.m_ExpandedInstanceIDs);
            }

            private void ClearDirtyStateTracking()
            {
                this.m_LastRenderedAssetInstanceIDs.Clear();
                this.m_LastRenderedAssetDirtyIDs.Clear();
            }

            public bool DoCharacterOffsetSelection()
            {
                if ((Event.current.type == EventType.KeyDown) && Event.current.shift)
                {
                    StringComparison currentCultureIgnoreCase = StringComparison.CurrentCultureIgnoreCase;
                    string name = string.Empty;
                    if (Selection.activeObject != null)
                    {
                        name = Selection.activeObject.name;
                    }
                    char[] chArray1 = new char[] { Event.current.character };
                    string str2 = new string(chArray1);
                    List<KeyValuePair<string, int>> visibleNameAndInstanceIDs = this.GetVisibleNameAndInstanceIDs();
                    if (visibleNameAndInstanceIDs.Count == 0)
                    {
                        return false;
                    }
                    int num = 0;
                    if (name.StartsWith(str2, currentCultureIgnoreCase))
                    {
                        for (int j = 0; j < visibleNameAndInstanceIDs.Count; j++)
                        {
                            KeyValuePair<string, int> pair = visibleNameAndInstanceIDs[j];
                            if (pair.Key == name)
                            {
                                num = j + 1;
                            }
                        }
                    }
                    for (int i = 0; i < visibleNameAndInstanceIDs.Count; i++)
                    {
                        int num4 = (i + num) % visibleNameAndInstanceIDs.Count;
                        KeyValuePair<string, int> pair2 = visibleNameAndInstanceIDs[num4];
                        if (pair2.Key.StartsWith(str2, currentCultureIgnoreCase))
                        {
                            KeyValuePair<string, int> pair3 = visibleNameAndInstanceIDs[num4];
                            Selection.activeInstanceID = pair3.Value;
                            base.m_Owner.Repaint();
                            return true;
                        }
                    }
                }
                return false;
            }

            public DragAndDropVisualMode DoDrag(int dragToInstanceID, bool perform)
            {
                HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
                if (!property.Find(dragToInstanceID, null))
                {
                    property = null;
                }
                return InternalEditorUtility.ProjectWindowDrag(property, perform);
            }

            protected override void DrawHeader(float yOffset, bool collapsable)
            {
                if (this.GetHeaderHeight() > 0f)
                {
                    Rect rect = new Rect(0f, base.GetHeaderYPosInScrollArea(yOffset), base.m_Owner.GetVisibleWidth(), base.kGroupSeparatorHeight);
                    base.DrawHeaderBackground(rect, true);
                    if (collapsable)
                    {
                        rect.x += 7f;
                        bool visible = base.Visible;
                        base.Visible = GUI.Toggle(new Rect(rect.x, rect.y, 14f, rect.height), base.Visible, GUIContent.none, ObjectListArea.s_Styles.groupFoldout);
                        if (visible ^ base.Visible)
                        {
                            EditorPrefs.SetBool(base.m_GroupSeparatorTitle, base.Visible);
                        }
                        rect.x += 7f;
                    }
                    float num = 0f;
                    if (base.m_Owner.drawLocalAssetHeader != null)
                    {
                        num = base.m_Owner.drawLocalAssetHeader(rect) + 10f;
                    }
                    rect.x += num;
                    rect.width -= num;
                    if (rect.width > 0f)
                    {
                        base.DrawItemCount(rect);
                    }
                }
            }

            public static void DrawIconAndLabel(Rect rect, FilteredHierarchy.FilterResult filterItem, string label, Texture2D icon, bool selected, bool focus)
            {
                float num = !ObjectListArea.s_VCEnabled ? 0f : 14f;
                ObjectListArea.s_Styles.resultsLabel.padding.left = (int) ((num + 16f) + 2f);
                ObjectListArea.s_Styles.resultsLabel.Draw(rect, label, false, false, selected, focus);
                Rect position = rect;
                position.width = 16f;
                position.x += num * 0.5f;
                if (icon != null)
                {
                    GUI.DrawTexture(position, icon);
                }
                if ((filterItem != null) && filterItem.isMainRepresentation)
                {
                    Rect drawRect = rect;
                    drawRect.width = num + 16f;
                    ProjectHooks.OnProjectWindowItem(filterItem.guid, drawRect);
                }
            }

            protected override void DrawInternal(int beginIndex, int endIndex, float yOffset)
            {
                Rect rect;
                int itemIdx = beginIndex;
                int num2 = 0;
                FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
                bool isFolderBrowsing = this.m_FilteredHierarchy.searchFilter.GetState() == SearchFilter.State.FolderBrowsing;
                yOffset += this.GetHeaderHeight();
                if (this.m_NoneList.Length > 0)
                {
                    if (beginIndex < 1)
                    {
                        rect = base.m_Grid.CalcRect(itemIdx, yOffset);
                        this.DrawItem(rect, null, this.m_NoneList[0], isFolderBrowsing);
                        itemIdx++;
                    }
                    num2++;
                }
                if (!this.ListMode && isFolderBrowsing)
                {
                    this.DrawSubAssetBackground(beginIndex, endIndex, yOffset);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    this.ClearDirtyStateTracking();
                }
                int index = itemIdx - num2;
                while (true)
                {
                    if (this.IsCreatingAtThisIndex(itemIdx))
                    {
                        BuiltinResource builtinResource = new BuiltinResource {
                            m_Name = base.m_Owner.GetCreateAssetUtility().originalName,
                            m_InstanceID = base.m_Owner.GetCreateAssetUtility().instanceID
                        };
                        this.DrawItem(base.m_Grid.CalcRect(itemIdx, yOffset), null, builtinResource, isFolderBrowsing);
                        itemIdx++;
                        num2++;
                    }
                    if ((itemIdx > endIndex) || (index >= results.Length))
                    {
                        break;
                    }
                    FilteredHierarchy.FilterResult filterItem = results[index];
                    rect = base.m_Grid.CalcRect(itemIdx, yOffset);
                    this.DrawItem(rect, filterItem, null, isFolderBrowsing);
                    itemIdx++;
                    index++;
                }
                num2 += results.Length;
                if (this.m_ActiveBuiltinList.Length > 0)
                {
                    int num4 = beginIndex - num2;
                    for (int i = Math.Max(num4, 0); (i < this.m_ActiveBuiltinList.Length) && (itemIdx <= endIndex); i++)
                    {
                        this.DrawItem(base.m_Grid.CalcRect(itemIdx, yOffset), null, this.m_ActiveBuiltinList[i], isFolderBrowsing);
                        itemIdx++;
                    }
                }
                if (!this.ListMode && AssetPreview.IsLoadingAssetPreviews(base.m_Owner.GetAssetPreviewManagerID()))
                {
                    base.m_Owner.Repaint();
                }
            }

            private void DrawItem(Rect position, FilteredHierarchy.FilterResult filterItem, BuiltinResource builtinResource, bool isFolderBrowsing)
            {
                bool flag2;
                Event current = Event.current;
                Rect selectionRect = position;
                int instanceID = 0;
                bool flag = false;
                if (filterItem != null)
                {
                    instanceID = filterItem.instanceID;
                    flag = (filterItem.hasChildren && !filterItem.isFolder) && isFolderBrowsing;
                }
                else if (builtinResource != null)
                {
                    instanceID = builtinResource.m_InstanceID;
                }
                int controlIDFromInstanceID = GetControlIDFromInstanceID(instanceID);
                if (base.m_Owner.allowDragging)
                {
                    flag2 = (this.m_DragSelection.Count <= 0) ? base.m_Owner.IsSelected(instanceID) : this.m_DragSelection.Contains(instanceID);
                }
                else
                {
                    flag2 = base.m_Owner.IsSelected(instanceID);
                }
                if (flag2 && (instanceID == base.m_Owner.m_State.m_LastClickedInstanceID))
                {
                    base.m_LastClickedDrawTime = EditorApplication.timeSinceStartup;
                }
                Rect rect2 = new Rect(position.x + 2f, position.y, 16f, 16f);
                if (flag && !this.ListMode)
                {
                    float num3 = position.height / 128f;
                    float width = 28f * num3;
                    float height = 32f * num3;
                    rect2 = new Rect(position.xMax - (width * 0.5f), (position.y + ((position.height - ObjectListArea.s_Styles.resultsGridLabel.fixedHeight) * 0.5f)) - (width * 0.5f), width, height);
                }
                bool flag3 = false;
                if ((flag2 && (current.type == EventType.KeyDown)) && base.m_Owner.HasFocus())
                {
                    KeyCode keyCode = current.keyCode;
                    if (keyCode != KeyCode.RightArrow)
                    {
                        if ((keyCode == KeyCode.LeftArrow) && (this.ListMode || base.m_Owner.IsPreviewIconExpansionModifierPressed()))
                        {
                            if (this.IsExpanded(instanceID))
                            {
                                flag3 = true;
                            }
                            else
                            {
                                this.SelectAndFrameParentOf(instanceID);
                            }
                            current.Use();
                        }
                    }
                    else if (this.ListMode || base.m_Owner.IsPreviewIconExpansionModifierPressed())
                    {
                        if (!this.IsExpanded(instanceID))
                        {
                            flag3 = true;
                        }
                        current.Use();
                    }
                }
                if ((flag && (current.type == EventType.MouseDown)) && ((current.button == 0) && rect2.Contains(current.mousePosition)))
                {
                    flag3 = true;
                }
                if (flag3)
                {
                    bool expanded = !this.IsExpanded(instanceID);
                    if (expanded)
                    {
                        this.m_ItemFader.Start(this.m_FilteredHierarchy.GetSubAssetInstanceIDs(instanceID));
                    }
                    this.ChangeExpandedState(instanceID, expanded);
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                bool flag5 = this.IsRenaming(instanceID);
                Rect rect3 = position;
                if (!this.ListMode)
                {
                    rect3 = new Rect(position.x, (position.yMax + 1f) - ObjectListArea.s_Styles.resultsGridLabel.fixedHeight, position.width - 1f, ObjectListArea.s_Styles.resultsGridLabel.fixedHeight);
                }
                int num6 = (!Provider.isActive || !this.ListMode) ? 0 : 14;
                float x = 16f;
                if (this.ListMode)
                {
                    selectionRect.x = 16f;
                    if (((filterItem != null) && !filterItem.isMainRepresentation) && isFolderBrowsing)
                    {
                        x = 28f;
                        selectionRect.x = 28f + (num6 * 0.5f);
                    }
                    selectionRect.width -= selectionRect.x;
                }
                if (Event.current.type == EventType.Repaint)
                {
                    if ((this.m_DropTargetControlID == controlIDFromInstanceID) && !position.Contains(current.mousePosition))
                    {
                        this.m_DropTargetControlID = 0;
                    }
                    bool flag6 = (controlIDFromInstanceID == this.m_DropTargetControlID) && (this.m_DragSelection.IndexOf(this.m_DropTargetControlID) == -1);
                    string label = (filterItem == null) ? builtinResource.m_Name : filterItem.name;
                    if (this.ListMode)
                    {
                        if (flag5)
                        {
                            flag2 = false;
                            label = string.Empty;
                        }
                        position.width = Mathf.Max(position.width, 500f);
                        this.m_Content.text = label;
                        this.m_Content.image = null;
                        Texture2D icon = (filterItem == null) ? AssetPreview.GetAssetPreview(instanceID, base.m_Owner.GetAssetPreviewManagerID()) : filterItem.icon;
                        if ((icon == null) && (base.m_Owner.GetCreateAssetUtility().icon != null))
                        {
                            icon = base.m_Owner.GetCreateAssetUtility().icon;
                        }
                        if (flag2)
                        {
                            ObjectListArea.s_Styles.resultsLabel.Draw(position, GUIContent.none, false, false, flag2, base.m_Owner.HasFocus());
                        }
                        if (flag6)
                        {
                            ObjectListArea.s_Styles.resultsLabel.Draw(position, GUIContent.none, true, true, false, false);
                        }
                        DrawIconAndLabel(new Rect(x, position.y, position.width - x, position.height), filterItem, label, icon, flag2, base.m_Owner.HasFocus());
                        if (flag)
                        {
                            ObjectListArea.s_Styles.groupFoldout.Draw(rect2, !this.ListMode, !this.ListMode, this.IsExpanded(instanceID), false);
                        }
                    }
                    else
                    {
                        bool flag7 = false;
                        if ((base.m_Owner.GetCreateAssetUtility().instanceID == instanceID) && (base.m_Owner.GetCreateAssetUtility().icon != null))
                        {
                            this.m_Content.image = base.m_Owner.GetCreateAssetUtility().icon;
                        }
                        else
                        {
                            this.m_Content.image = AssetPreview.GetAssetPreview(instanceID, base.m_Owner.GetAssetPreviewManagerID());
                            if (this.m_Content.image != null)
                            {
                                flag7 = true;
                            }
                            if (filterItem != null)
                            {
                                if (this.m_Content.image == null)
                                {
                                    this.m_Content.image = filterItem.icon;
                                }
                                if (isFolderBrowsing && !filterItem.isMainRepresentation)
                                {
                                    flag7 = false;
                                }
                            }
                        }
                        float num8 = !flag7 ? 0f : 2f;
                        position.height -= ObjectListArea.s_Styles.resultsGridLabel.fixedHeight + (2f * num8);
                        position.y += num8;
                        Rect rect4 = (this.m_Content.image != null) ? ActualImageDrawPosition(position, (float) this.m_Content.image.width, (float) this.m_Content.image.height) : new Rect();
                        this.m_Content.text = null;
                        float a = 1f;
                        if (filterItem != null)
                        {
                            this.AddDirtyStateFor(filterItem.instanceID);
                            if (!filterItem.isMainRepresentation && isFolderBrowsing)
                            {
                                position.x += 4f;
                                position.y += 4f;
                                position.width -= 8f;
                                position.height -= 8f;
                                rect4 = (this.m_Content.image != null) ? ActualImageDrawPosition(position, (float) this.m_Content.image.width, (float) this.m_Content.image.height) : new Rect();
                                a = this.m_ItemFader.GetAlpha(filterItem.instanceID);
                                if (a < 1f)
                                {
                                    base.m_Owner.Repaint();
                                }
                            }
                            if (flag7 && (filterItem.iconDrawStyle == IconDrawStyle.NonTexture))
                            {
                                ObjectListArea.s_Styles.previewBg.Draw(rect4, GUIContent.none, false, false, false, false);
                            }
                        }
                        Color color = GUI.color;
                        if (flag2)
                        {
                            GUI.color *= new Color(0.85f, 0.9f, 1f);
                        }
                        if (this.m_Content.image != null)
                        {
                            Color color2 = GUI.color;
                            if (a < 1f)
                            {
                                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
                            }
                            ObjectListArea.s_Styles.resultsGrid.Draw(rect4, this.m_Content, false, false, flag2, base.m_Owner.HasFocus());
                            if (a < 1f)
                            {
                                GUI.color = color2;
                            }
                        }
                        if (flag2)
                        {
                            GUI.color = color;
                        }
                        if (flag7)
                        {
                            Rect rect5 = new RectOffset(1, 1, 1, 1).Remove(ObjectListArea.s_Styles.textureIconDropShadow.border.Add(rect4));
                            ObjectListArea.s_Styles.textureIconDropShadow.Draw(rect5, GUIContent.none, false, false, flag2 || flag6, (base.m_Owner.HasFocus() || flag5) || flag6);
                        }
                        if (!flag5)
                        {
                            if (flag6)
                            {
                                ObjectListArea.s_Styles.resultsLabel.Draw(new Rect(rect3.x - 10f, rect3.y, rect3.width + 20f, rect3.height), GUIContent.none, true, true, false, false);
                            }
                            label = base.m_Owner.GetCroppedLabelText(instanceID, label, position.width);
                            ObjectListArea.s_Styles.resultsGridLabel.Draw(rect3, label, false, false, flag2, base.m_Owner.HasFocus());
                        }
                        if (flag)
                        {
                            ObjectListArea.s_Styles.subAssetExpandButton.Draw(rect2, !this.ListMode, !this.ListMode, this.IsExpanded(instanceID), false);
                        }
                        if ((filterItem != null) && filterItem.isMainRepresentation)
                        {
                            ProjectHooks.OnProjectWindowItem(filterItem.guid, position);
                        }
                    }
                }
                if (flag5)
                {
                    if (this.ListMode)
                    {
                        float num10 = num6 + 0x10;
                        rect3.x = selectionRect.x + num10;
                        rect3.width -= rect3.x;
                    }
                    else
                    {
                        rect3.x -= 4f;
                        rect3.width += 8f;
                    }
                    base.m_Owner.GetRenameOverlay().editFieldRect = rect3;
                    base.m_Owner.HandleRenameOverlay();
                }
                if (((EditorApplication.projectWindowItemOnGUI != null) && (filterItem != null)) && base.m_Owner.allowUserRenderingHook)
                {
                    EditorApplication.projectWindowItemOnGUI(filterItem.guid, selectionRect);
                }
                if (base.m_Owner.allowDragging)
                {
                    this.HandleMouseWithDragging(instanceID, controlIDFromInstanceID, position);
                }
                else
                {
                    this.HandleMouseWithoutDragging(instanceID, controlIDFromInstanceID, position);
                }
            }

            private void DrawSubAssetBackground(int beginIndex, int endIndex, float yOffset)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
                    int columns = base.m_Grid.columns;
                    int num2 = ((endIndex - beginIndex) / columns) + 1;
                    for (int i = 0; i < num2; i++)
                    {
                        int startSubAssetIndex = -1;
                        int endSubAssetIndex = -1;
                        for (int j = 0; j < columns; j++)
                        {
                            int index = beginIndex + (j + (i * columns));
                            if (index >= results.Length)
                            {
                                break;
                            }
                            FilteredHierarchy.FilterResult result = results[index];
                            if (!result.isMainRepresentation)
                            {
                                if (startSubAssetIndex == -1)
                                {
                                    startSubAssetIndex = index;
                                }
                                endSubAssetIndex = index;
                            }
                            else if (startSubAssetIndex != -1)
                            {
                                this.DrawSubAssetRowBg(startSubAssetIndex, endSubAssetIndex, false, yOffset);
                                startSubAssetIndex = -1;
                                endSubAssetIndex = -1;
                            }
                        }
                        if (startSubAssetIndex != -1)
                        {
                            bool continued = false;
                            if (i < (num2 - 1))
                            {
                                int num8 = beginIndex + ((i + 1) * columns);
                                if (num8 < results.Length)
                                {
                                    continued = !results[num8].isMainRepresentation;
                                }
                            }
                            this.DrawSubAssetRowBg(startSubAssetIndex, endSubAssetIndex, continued, yOffset);
                        }
                    }
                }
            }

            protected void DrawSubAssetRowBg(int startSubAssetIndex, int endSubAssetIndex, bool continued, float yOffset)
            {
                Rect rect3;
                Rect rect4;
                Rect rect = base.m_Grid.CalcRect(startSubAssetIndex, yOffset);
                Rect rect2 = base.m_Grid.CalcRect(endSubAssetIndex, yOffset);
                float num = 30f;
                float num2 = 128f;
                float num3 = rect.width / num2;
                float num4 = 9f * num3;
                float num5 = 4f;
                float num6 = ((startSubAssetIndex % base.m_Grid.columns) != 0) ? (base.m_Grid.horizontalSpacing + (num3 * 10f)) : (18f * num3);
                rect3 = new Rect(rect.x - num6, rect.y + num5, num * num3, ((rect.width - (num5 * 2f)) + num4) - 1f) {
                    y = Mathf.Round(rect3.y),
                    height = Mathf.Ceil(rect3.height)
                };
                ObjectListArea.s_Styles.subAssetBg.Draw(rect3, GUIContent.none, false, false, false, false);
                float width = num * num3;
                bool flag2 = (endSubAssetIndex % base.m_Grid.columns) == (base.m_Grid.columns - 1);
                float num8 = (!continued && !flag2) ? (8f * num3) : (16f * num3);
                rect4 = new Rect((rect2.xMax - width) + num8, rect2.y + num5, width, rect3.height) {
                    y = Mathf.Round(rect4.y),
                    height = Mathf.Ceil(rect4.height)
                };
                (!continued ? ObjectListArea.s_Styles.subAssetBgCloseEnded : ObjectListArea.s_Styles.subAssetBgOpenEnded).Draw(rect4, GUIContent.none, false, false, false, false);
                rect3 = new Rect(rect3.xMax, rect3.y, rect4.xMin - rect3.xMax, rect3.height) {
                    y = Mathf.Round(rect3.y),
                    height = Mathf.Ceil(rect3.height)
                };
                ObjectListArea.s_Styles.subAssetBgMiddle.Draw(rect3, GUIContent.none, false, false, false, false);
            }

            internal static int GetControlIDFromInstanceID(int instanceID)
            {
                return (instanceID + 0x5f5e100);
            }

            protected override float GetHeaderHeight()
            {
                return 0f;
            }

            public List<int> GetInstanceIDs()
            {
                List<int> list = new List<int>();
                if (this.m_NoneList.Length > 0)
                {
                    list.Add(this.m_NoneList[0].m_InstanceID);
                }
                foreach (FilteredHierarchy.FilterResult result in this.m_FilteredHierarchy.results)
                {
                    list.Add(result.instanceID);
                }
                if (base.m_Owner.m_State.m_NewAssetIndexInList >= 0)
                {
                    list.Add(base.m_Owner.GetCreateAssetUtility().instanceID);
                }
                for (int i = 0; i < this.m_ActiveBuiltinList.Length; i++)
                {
                    list.Add(this.m_ActiveBuiltinList[i].m_InstanceID);
                }
                return list;
            }

            public string GetNameOfLocalAsset(int instanceID)
            {
                foreach (FilteredHierarchy.FilterResult result in this.m_FilteredHierarchy.results)
                {
                    if (result.instanceID == instanceID)
                    {
                        return result.name;
                    }
                }
                return null;
            }

            public List<int> GetNewSelection(int clickedInstanceID, bool beginOfDrag, bool useShiftAsActionKey)
            {
                List<int> instanceIDs = this.GetInstanceIDs();
                List<int> selectedInstanceIDs = base.m_Owner.m_State.m_SelectedInstanceIDs;
                int lastClickedInstanceID = base.m_Owner.m_State.m_LastClickedInstanceID;
                bool allowMultiSelect = base.m_Owner.allowMultiSelect;
                return InternalEditorUtility.GetNewSelection(clickedInstanceID, instanceIDs, selectedInstanceIDs, lastClickedInstanceID, beginOfDrag, useShiftAsActionKey, allowMultiSelect);
            }

            public List<KeyValuePair<string, int>> GetVisibleNameAndInstanceIDs()
            {
                List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
                if (this.m_NoneList.Length > 0)
                {
                    list.Add(new KeyValuePair<string, int>(this.m_NoneList[0].m_Name, this.m_NoneList[0].m_InstanceID));
                }
                foreach (FilteredHierarchy.FilterResult result in this.m_FilteredHierarchy.results)
                {
                    list.Add(new KeyValuePair<string, int>(result.name, result.instanceID));
                }
                for (int i = 0; i < this.m_ActiveBuiltinList.Length; i++)
                {
                    list.Add(new KeyValuePair<string, int>(this.m_ActiveBuiltinList[i].m_Name, this.m_ActiveBuiltinList[i].m_InstanceID));
                }
                return list;
            }

            private void HandleMouseWithDragging(int instanceID, int controlID, Rect rect)
            {
                Event current = Event.current;
                EventType typeForControl = current.GetTypeForControl(controlID);
                switch (typeForControl)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                    {
                        bool perform = current.type == EventType.DragPerform;
                        if (rect.Contains(current.mousePosition))
                        {
                            DragAndDropVisualMode mode = this.DoDrag(instanceID, perform);
                            if (mode != DragAndDropVisualMode.None)
                            {
                                if (perform)
                                {
                                    DragAndDrop.AcceptDrag();
                                }
                                this.m_DropTargetControlID = controlID;
                                DragAndDrop.visualMode = mode;
                                current.Use();
                            }
                            if (perform)
                            {
                                this.m_DropTargetControlID = 0;
                            }
                        }
                        if (perform)
                        {
                            this.m_DragSelection.Clear();
                        }
                        return;
                    }
                    case EventType.DragExited:
                        this.m_DragSelection.Clear();
                        return;

                    case EventType.ContextClick:
                    {
                        Rect drawRect = rect;
                        drawRect.x += 2f;
                        drawRect = ProjectHooks.GetOverlayRect(drawRect);
                        if (((drawRect.width != rect.width) && Provider.isActive) && drawRect.Contains(current.mousePosition))
                        {
                            EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/Version Control", new MenuCommand(null, 0));
                            current.Use();
                        }
                        return;
                    }
                    default:
                        switch (typeForControl)
                        {
                            case EventType.MouseDown:
                            {
                                if ((Event.current.button != 0) || !rect.Contains(Event.current.mousePosition))
                                {
                                    if ((Event.current.button == 1) && rect.Contains(Event.current.mousePosition))
                                    {
                                        base.m_Owner.SetSelection(this.GetNewSelection(instanceID, true, false).ToArray(), false);
                                    }
                                    return;
                                }
                                if (current.clickCount != 2)
                                {
                                    this.m_DragSelection = this.GetNewSelection(instanceID, true, false);
                                    GUIUtility.hotControl = controlID;
                                    DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), controlID);
                                    stateObject.mouseDownPosition = Event.current.mousePosition;
                                    base.m_Owner.ScrollToPosition(ObjectListArea.AdjustRectForFraming(rect));
                                    break;
                                }
                                int[] selectedInstanceIDs = new int[] { instanceID };
                                base.m_Owner.SetSelection(selectedInstanceIDs, true);
                                this.m_DragSelection.Clear();
                                break;
                            }
                            case EventType.MouseUp:
                                if (GUIUtility.hotControl != controlID)
                                {
                                    return;
                                }
                                if (rect.Contains(current.mousePosition))
                                {
                                    bool flag2;
                                    if (!this.ListMode)
                                    {
                                        rect.y = (rect.y + rect.height) - ObjectListArea.s_Styles.resultsGridLabel.fixedHeight;
                                        rect.height = ObjectListArea.s_Styles.resultsGridLabel.fixedHeight;
                                        flag2 = rect.Contains(current.mousePosition);
                                    }
                                    else
                                    {
                                        rect.x += 28f;
                                        rect.width += 28f;
                                        flag2 = rect.Contains(current.mousePosition);
                                    }
                                    List<int> list = base.m_Owner.m_State.m_SelectedInstanceIDs;
                                    if (((flag2 && base.m_Owner.allowRenaming) && (base.m_Owner.m_AllowRenameOnMouseUp && (list.Count == 1))) && ((list[0] == instanceID) && !EditorGUIUtility.HasHolddownKeyModifiers(current)))
                                    {
                                        base.m_Owner.BeginRename(0.5f);
                                    }
                                    else
                                    {
                                        base.m_Owner.SetSelection(this.GetNewSelection(instanceID, false, false).ToArray(), false);
                                    }
                                    GUIUtility.hotControl = 0;
                                    current.Use();
                                }
                                goto Label_035E;

                            case EventType.MouseMove:
                                return;

                            case EventType.MouseDrag:
                                if (GUIUtility.hotControl == controlID)
                                {
                                    DragAndDropDelay delay2 = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), controlID);
                                    if (delay2.CanStartDrag())
                                    {
                                        this.StartDrag(instanceID, this.m_DragSelection);
                                        GUIUtility.hotControl = 0;
                                    }
                                    current.Use();
                                }
                                return;

                            default:
                                return;
                        }
                        current.Use();
                        return;
                }
            Label_035E:
                this.m_DragSelection.Clear();
            }

            private void HandleMouseWithoutDragging(int instanceID, int controlID, Rect position)
            {
                Event current = Event.current;
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if ((current.button == 0) && position.Contains(current.mousePosition))
                        {
                            base.m_Owner.Repaint();
                            if (current.clickCount == 1)
                            {
                                base.m_Owner.ScrollToPosition(ObjectListArea.AdjustRectForFraming(position));
                            }
                            current.Use();
                            base.m_Owner.SetSelection(this.GetNewSelection(instanceID, false, false).ToArray(), current.clickCount == 2);
                        }
                        break;

                    case EventType.ContextClick:
                        if (position.Contains(current.mousePosition))
                        {
                            int[] selectedInstanceIDs = new int[] { instanceID };
                            base.m_Owner.SetSelection(selectedInstanceIDs, false);
                            Rect drawRect = position;
                            drawRect.x += 2f;
                            drawRect = ProjectHooks.GetOverlayRect(drawRect);
                            if (((drawRect.width != position.width) && Provider.isActive) && drawRect.Contains(current.mousePosition))
                            {
                                EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/Version Control", new MenuCommand(null, 0));
                                current.Use();
                            }
                        }
                        break;
                }
            }

            protected override void HandleUnusedDragEvents(float yOffset)
            {
                if (base.m_Owner.allowDragging)
                {
                    Event current = Event.current;
                    switch (current.type)
                    {
                        case EventType.DragUpdated:
                        case EventType.DragPerform:
                        {
                            Rect rect = new Rect(0f, yOffset, base.m_Owner.m_TotalRect.width, (base.m_Owner.m_TotalRect.height <= base.Height) ? base.Height : base.m_Owner.m_TotalRect.height);
                            if (rect.Contains(current.mousePosition))
                            {
                                DragAndDropVisualMode none;
                                if ((this.m_FilteredHierarchy.searchFilter.GetState() == SearchFilter.State.FolderBrowsing) && (this.m_FilteredHierarchy.searchFilter.folders.Length == 1))
                                {
                                    string assetPath = this.m_FilteredHierarchy.searchFilter.folders[0];
                                    int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(assetPath);
                                    bool perform = current.type == EventType.DragPerform;
                                    none = this.DoDrag(mainAssetInstanceID, perform);
                                    if (perform && (none != DragAndDropVisualMode.None))
                                    {
                                        DragAndDrop.AcceptDrag();
                                    }
                                }
                                else
                                {
                                    none = DragAndDropVisualMode.None;
                                }
                                DragAndDrop.visualMode = none;
                                current.Use();
                            }
                            break;
                        }
                    }
                }
            }

            public int IndexOf(int instanceID)
            {
                int num = 0;
                if (this.m_ShowNoneItem)
                {
                    if (instanceID == 0)
                    {
                        return 0;
                    }
                    num++;
                }
                else if (instanceID == 0)
                {
                    return -1;
                }
                foreach (FilteredHierarchy.FilterResult result in this.m_FilteredHierarchy.results)
                {
                    if (base.m_Owner.m_State.m_NewAssetIndexInList == num)
                    {
                        num++;
                    }
                    if (result.instanceID == instanceID)
                    {
                        return num;
                    }
                    num++;
                }
                foreach (BuiltinResource resource in this.m_ActiveBuiltinList)
                {
                    if (instanceID == resource.m_InstanceID)
                    {
                        return num;
                    }
                    num++;
                }
                return -1;
            }

            public int IndexOfNewText(string newText, bool isCreatingNewFolder, bool foldersFirst)
            {
                int index = 0;
                if (this.m_ShowNoneItem)
                {
                    index++;
                }
                while (index < this.m_FilteredHierarchy.results.Length)
                {
                    FilteredHierarchy.FilterResult result = this.m_FilteredHierarchy.results[index];
                    if ((!foldersFirst || !result.isFolder) || isCreatingNewFolder)
                    {
                        if ((foldersFirst && !result.isFolder) && isCreatingNewFolder)
                        {
                            return index;
                        }
                        if (EditorUtility.NaturalCompare(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(result.instanceID)), newText) > 0)
                        {
                            return index;
                        }
                    }
                    index++;
                }
                return index;
            }

            private void InitBuiltinAssetType(Type type)
            {
                if (type == null)
                {
                    Debug.LogWarning("ObjectSelector::InitBuiltinAssetType: type is null!");
                }
                else
                {
                    string classString = type.ToString().Substring(type.Namespace.ToString().Length + 1);
                    int classID = BaseObjectTools.StringToClassID(classString);
                    if (classID < 0)
                    {
                        Debug.LogWarning("ObjectSelector::InitBuiltinAssetType: class '" + classString + "' not found");
                    }
                    else
                    {
                        BuiltinResource[] builtinResourceList = EditorGUIUtility.GetBuiltinResourceList(classID);
                        if (builtinResourceList != null)
                        {
                            this.m_BuiltinResourceMap.Add(classString, builtinResourceList);
                        }
                    }
                }
            }

            public void InitBuiltinResources()
            {
                if (this.m_BuiltinResourceMap == null)
                {
                    this.m_BuiltinResourceMap = new Dictionary<string, BuiltinResource[]>();
                    if (this.m_ShowNoneItem)
                    {
                        this.m_NoneList = new BuiltinResource[] { new BuiltinResource() };
                        this.m_NoneList[0].m_InstanceID = 0;
                        this.m_NoneList[0].m_Name = "None";
                    }
                    else
                    {
                        this.m_NoneList = new BuiltinResource[0];
                    }
                    this.InitBuiltinAssetType(typeof(Mesh));
                    this.InitBuiltinAssetType(typeof(Material));
                    this.InitBuiltinAssetType(typeof(Texture2D));
                    this.InitBuiltinAssetType(typeof(Font));
                    this.InitBuiltinAssetType(typeof(Shader));
                    this.InitBuiltinAssetType(typeof(Sprite));
                    this.InitBuiltinAssetType(typeof(LightmapParameters));
                }
            }

            public bool InstanceIdAtIndex(int index, out int instanceID)
            {
                instanceID = 0;
                if (index >= (base.m_Grid.rows * base.m_Grid.columns))
                {
                    return false;
                }
                int num = 0;
                if (this.m_ShowNoneItem)
                {
                    if (index == 0)
                    {
                        return true;
                    }
                    num++;
                }
                foreach (FilteredHierarchy.FilterResult result in this.m_FilteredHierarchy.results)
                {
                    instanceID = result.instanceID;
                    if (num == index)
                    {
                        return true;
                    }
                    num++;
                }
                foreach (BuiltinResource resource in this.m_ActiveBuiltinList)
                {
                    instanceID = resource.m_InstanceID;
                    if (num == index)
                    {
                        return true;
                    }
                    num++;
                }
                return (index < (base.m_Grid.rows * base.m_Grid.columns));
            }

            public bool IsAnyLastRenderedAssetsDirty()
            {
                for (int i = 0; i < this.m_LastRenderedAssetInstanceIDs.Count; i++)
                {
                    int dirtyIndex = EditorUtility.GetDirtyIndex(this.m_LastRenderedAssetInstanceIDs[i]);
                    if (dirtyIndex != this.m_LastRenderedAssetDirtyIDs[i])
                    {
                        this.m_LastRenderedAssetDirtyIDs[i] = dirtyIndex;
                        return true;
                    }
                }
                return false;
            }

            public bool IsBuiltinAsset(int instanceID)
            {
                foreach (KeyValuePair<string, BuiltinResource[]> pair in this.m_BuiltinResourceMap)
                {
                    BuiltinResource[] resourceArray = pair.Value;
                    for (int i = 0; i < resourceArray.Length; i++)
                    {
                        if (resourceArray[i].m_InstanceID == instanceID)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            private bool IsCreatingAtThisIndex(int itemIdx)
            {
                return (base.m_Owner.m_State.m_NewAssetIndexInList == itemIdx);
            }

            private bool IsExpanded(int instanceID)
            {
                return (base.m_Owner.m_State.m_ExpandedInstanceIDs.IndexOf(instanceID) >= 0);
            }

            private bool IsRenaming(int instanceID)
            {
                RenameOverlay renameOverlay = base.m_Owner.GetRenameOverlay();
                return ((renameOverlay.IsRenaming() && (renameOverlay.userData == instanceID)) && !renameOverlay.isWaitingForDelay);
            }

            public FilteredHierarchy.FilterResult LookupByInstanceID(int instanceID)
            {
                if (instanceID != 0)
                {
                    int num = 0;
                    foreach (FilteredHierarchy.FilterResult result in this.m_FilteredHierarchy.results)
                    {
                        if (base.m_Owner.m_State.m_NewAssetIndexInList == num)
                        {
                            num++;
                        }
                        if (result.instanceID == instanceID)
                        {
                            return result;
                        }
                        num++;
                    }
                }
                return null;
            }

            public void PrintBuiltinResourcesAvailable()
            {
                string message = string.Empty + "ObjectSelector -Builtin Assets Available:\n";
                foreach (KeyValuePair<string, BuiltinResource[]> pair in this.m_BuiltinResourceMap)
                {
                    BuiltinResource[] resourceArray = pair.Value;
                    message = message + "    " + pair.Key + ": ";
                    for (int i = 0; i < resourceArray.Length; i++)
                    {
                        if (i != 0)
                        {
                            message = message + ", ";
                        }
                        message = message + resourceArray[i].m_Name;
                    }
                    message = message + "\n";
                }
                Debug.Log(message);
            }

            private void RefreshBuiltinResourceList(SearchFilter searchFilter)
            {
                if ((!base.m_Owner.allowBuiltinResources || (searchFilter.GetState() == SearchFilter.State.FolderBrowsing)) || (searchFilter.GetState() == SearchFilter.State.EmptySearchFilter))
                {
                    this.m_CurrentBuiltinResources = new BuiltinResource[0];
                }
                else
                {
                    List<BuiltinResource> list = new List<BuiltinResource>();
                    if ((searchFilter.assetLabels != null) && (searchFilter.assetLabels.Length > 0))
                    {
                        this.m_CurrentBuiltinResources = list.ToArray();
                    }
                    else
                    {
                        List<int> list2 = new List<int>();
                        foreach (string str in searchFilter.classNames)
                        {
                            int item = BaseObjectTools.StringToClassIDCaseInsensitive(str);
                            if (item >= 0)
                            {
                                list2.Add(item);
                            }
                        }
                        if (list2.Count > 0)
                        {
                            foreach (KeyValuePair<string, BuiltinResource[]> pair in this.m_BuiltinResourceMap)
                            {
                                int classID = BaseObjectTools.StringToClassID(pair.Key);
                                foreach (int num4 in list2)
                                {
                                    if (BaseObjectTools.IsDerivedFromClassID(classID, num4))
                                    {
                                        list.AddRange(pair.Value);
                                    }
                                }
                            }
                        }
                        BuiltinResource[] resourceArray = list.ToArray();
                        if ((resourceArray.Length > 0) && !string.IsNullOrEmpty(searchFilter.nameFilter))
                        {
                            List<BuiltinResource> list3 = new List<BuiltinResource>();
                            string str2 = searchFilter.nameFilter.ToLower();
                            foreach (BuiltinResource resource in resourceArray)
                            {
                                if (resource.m_Name.ToLower().Contains(str2))
                                {
                                    list3.Add(resource);
                                }
                            }
                            resourceArray = list3.ToArray();
                        }
                        this.m_CurrentBuiltinResources = resourceArray;
                    }
                }
            }

            private void RefreshHierarchy(HierarchyType hierarchyType, SearchFilter searchFilter, bool foldersFirst)
            {
                this.m_FilteredHierarchy = new FilteredHierarchy(hierarchyType);
                this.m_FilteredHierarchy.foldersFirst = foldersFirst;
                this.m_FilteredHierarchy.searchFilter = searchFilter;
                this.m_FilteredHierarchy.RefreshVisibleItems(base.m_Owner.m_State.m_ExpandedInstanceIDs);
            }

            private void SelectAndFrameParentOf(int instanceID)
            {
                int num = 0;
                FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i].instanceID == instanceID)
                    {
                        if (results[i].isMainRepresentation)
                        {
                            num = 0;
                        }
                        break;
                    }
                    if (results[i].isMainRepresentation)
                    {
                        num = results[i].instanceID;
                    }
                }
                if (num != 0)
                {
                    int[] selectedInstanceIDs = new int[] { num };
                    base.m_Owner.SetSelection(selectedInstanceIDs, false);
                    base.m_Owner.Frame(num, true, false);
                }
            }

            public void ShowObjectsInList(int[] instanceIDs)
            {
                this.m_FilteredHierarchy = new FilteredHierarchy(HierarchyType.Assets);
                this.m_FilteredHierarchy.SetResults(instanceIDs);
            }

            public virtual void StartDrag(int draggedInstanceID, List<int> selectedInstanceIDs)
            {
                ProjectWindowUtil.StartDrag(draggedInstanceID, selectedInstanceIDs);
            }

            public override void UpdateAssets()
            {
                if (this.m_FilteredHierarchy.hierarchyType == HierarchyType.Assets)
                {
                    this.m_ActiveBuiltinList = this.m_CurrentBuiltinResources;
                }
                else
                {
                    this.m_ActiveBuiltinList = new BuiltinResource[0];
                }
                base.ItemsAvailable = this.m_FilteredHierarchy.results.Length + this.m_ActiveBuiltinList.Length;
            }

            public override void UpdateFilter(HierarchyType hierarchyType, SearchFilter searchFilter, bool foldersFirst)
            {
                this.RefreshHierarchy(hierarchyType, searchFilter, foldersFirst);
                this.RefreshBuiltinResourceList(searchFilter);
            }

            public override void UpdateHeight()
            {
                base.m_Height = this.GetHeaderHeight();
                if (base.Visible)
                {
                    base.m_Height += base.m_Grid.height;
                }
            }

            public bool HasBuiltinResources
            {
                get
                {
                    return (this.m_CurrentBuiltinResources.Length > 0);
                }
            }

            public override int ItemCount
            {
                get
                {
                    int length = this.m_FilteredHierarchy.results.Length;
                    int num2 = length + this.m_ActiveBuiltinList.Length;
                    int num3 = !this.m_ShowNoneItem ? 0 : 1;
                    int num4 = (base.m_Owner.m_State.m_NewAssetIndexInList == -1) ? 0 : 1;
                    return ((num2 + num3) + num4);
                }
            }

            public override bool ListMode
            {
                get
                {
                    return this.m_ListMode;
                }
                set
                {
                    this.m_ListMode = value;
                }
            }

            public override bool NeedsRepaint
            {
                get
                {
                    return false;
                }
                protected set
                {
                }
            }

            public SearchFilter searchFilter
            {
                get
                {
                    return this.m_FilteredHierarchy.searchFilter;
                }
            }

            public bool ShowNone
            {
                get
                {
                    return this.m_ShowNoneItem;
                }
            }

            private class ItemFader
            {
                private double m_FadeDuration = 0.3;
                private double m_FadeStartTime;
                private double m_FirstToLastDuration = 0.3;
                private List<int> m_InstanceIDs;
                private double m_TimeBetweenEachItem;

                public float GetAlpha(int instanceID)
                {
                    if (this.m_InstanceIDs == null)
                    {
                        return 1f;
                    }
                    if (EditorApplication.timeSinceStartup > ((this.m_FadeStartTime + this.m_FadeDuration) + this.m_FirstToLastDuration))
                    {
                        this.m_InstanceIDs = null;
                        return 1f;
                    }
                    int index = this.m_InstanceIDs.IndexOf(instanceID);
                    if (index < 0)
                    {
                        return 1f;
                    }
                    double num2 = EditorApplication.timeSinceStartup - this.m_FadeStartTime;
                    double num3 = this.m_TimeBetweenEachItem * index;
                    float num4 = 0f;
                    if (num3 < num2)
                    {
                        num4 = Mathf.Clamp((float) ((num2 - num3) / this.m_FadeDuration), 0f, 1f);
                    }
                    return num4;
                }

                public void Start(List<int> instanceIDs)
                {
                    this.m_InstanceIDs = instanceIDs;
                    this.m_FadeStartTime = EditorApplication.timeSinceStartup;
                    this.m_FirstToLastDuration = Math.Min((double) 0.5, (double) (instanceIDs.Count * 0.03));
                    this.m_TimeBetweenEachItem = 0.0;
                    if (this.m_InstanceIDs.Count > 1)
                    {
                        this.m_TimeBetweenEachItem = this.m_FirstToLastDuration / ((double) (this.m_InstanceIDs.Count - 1));
                    }
                }
            }
        }

        private class Styles
        {
            public GUIStyle background = "ObjectPickerBackground";
            public GUIStyle groupFoldout = "Foldout";
            public GUIStyle groupHeaderLabel = "Label";
            public GUIStyle groupHeaderLabelCount = "MiniLabel";
            public GUIStyle groupHeaderMiddle = GetStyle("ProjectBrowserHeaderBgMiddle");
            public GUIStyle groupHeaderTop = GetStyle("ProjectBrowserHeaderBgTop");
            public GUIStyle iconAreaBg = GetStyle("ProjectBrowserIconAreaBg");
            public GUIStyle iconDropShadow = GetStyle("ProjectBrowserIconDropShadow");
            public GUIContent m_AssetStoreNotAvailableText = new GUIContent("The Asset Store is not available");
            public GUIStyle miniPing = new GUIStyle("PR Ping");
            public GUIStyle miniRenameField = new GUIStyle("PR TextField");
            public GUIStyle ping = new GUIStyle("PR Ping");
            public GUIStyle previewBg = GetStyle("ProjectBrowserPreviewBg");
            public GUIStyle previewTextureBackground = "ObjectPickerPreviewBackground";
            public GUIStyle resultsFocusMarker;
            public GUIStyle resultsGrid = "ObjectPickerResultsGrid";
            public GUIStyle resultsGridLabel = GetStyle("ProjectBrowserGridLabel");
            public GUIStyle resultsLabel = new GUIStyle("PR Label");
            public GUIStyle subAssetBg = GetStyle("ProjectBrowserSubAssetBg");
            public GUIStyle subAssetBgCloseEnded = GetStyle("ProjectBrowserSubAssetBgCloseEnded");
            public GUIStyle subAssetBgDivider = GetStyle("ProjectBrowserSubAssetBgDivider");
            public GUIStyle subAssetBgMiddle = GetStyle("ProjectBrowserSubAssetBgMiddle");
            public GUIStyle subAssetBgOpenEnded = GetStyle("ProjectBrowserSubAssetBgOpenEnded");
            public GUIStyle subAssetExpandButton = GetStyle("ProjectBrowserSubAssetExpandBtn");
            public GUIStyle textureIconDropShadow = GetStyle("ProjectBrowserTextureIconDropShadow");
            public GUIStyle toolbarBack = "ObjectPickerToolbar";

            public Styles()
            {
                this.resultsFocusMarker = new GUIStyle(this.resultsGridLabel);
                float num = 0f;
                this.resultsFocusMarker.fixedWidth = num;
                this.resultsFocusMarker.fixedHeight = num;
                this.miniRenameField.font = EditorStyles.miniLabel.font;
                this.miniRenameField.alignment = TextAnchor.LowerCenter;
                this.ping.fixedHeight = 16f;
                this.ping.padding.right = 10;
                this.miniPing.font = EditorStyles.miniLabel.font;
                this.miniPing.alignment = TextAnchor.MiddleCenter;
                this.resultsLabel.alignment = TextAnchor.MiddleLeft;
            }

            private static GUIStyle GetStyle(string styleName)
            {
                return styleName;
            }
        }
    }
}

