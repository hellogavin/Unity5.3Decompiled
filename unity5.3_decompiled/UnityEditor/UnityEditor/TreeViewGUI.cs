namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal abstract class TreeViewGUI : ITreeViewGUI
    {
        protected float k_BaseIndent;
        protected float k_BottomRowMargin;
        protected float k_FoldoutWidth;
        protected float k_HalfDropBetweenHeight;
        protected float k_IconWidth;
        protected float k_IndentWidth;
        protected float k_LineHeight;
        protected float k_SpaceBetweenIconAndText;
        protected float k_TopRowMargin;
        private bool m_AnimateScrollBarOnExpandCollapse;
        protected Rect m_DraggingInsertionMarkerRect;
        protected PingData m_Ping;
        protected TreeView m_TreeView;
        protected bool m_UseHorizontalScroll;
        protected static Styles s_Styles;

        public TreeViewGUI(TreeView treeView)
        {
            this.m_Ping = new PingData();
            this.m_AnimateScrollBarOnExpandCollapse = true;
            this.k_LineHeight = 16f;
            this.k_BaseIndent = 2f;
            this.k_IndentWidth = 14f;
            this.k_FoldoutWidth = 12f;
            this.k_IconWidth = 16f;
            this.k_SpaceBetweenIconAndText = 2f;
            this.k_HalfDropBetweenHeight = 4f;
            this.m_TreeView = treeView;
        }

        public TreeViewGUI(TreeView treeView, bool useHorizontalScroll)
        {
            this.m_Ping = new PingData();
            this.m_AnimateScrollBarOnExpandCollapse = true;
            this.k_LineHeight = 16f;
            this.k_BaseIndent = 2f;
            this.k_IndentWidth = 14f;
            this.k_FoldoutWidth = 12f;
            this.k_IconWidth = 16f;
            this.k_SpaceBetweenIconAndText = 2f;
            this.k_HalfDropBetweenHeight = 4f;
            this.m_TreeView = treeView;
            this.m_UseHorizontalScroll = useHorizontalScroll;
        }

        public virtual void BeginPingNode(TreeViewItem item, float topPixelOfRow, float availableWidth)
        {
            <BeginPingNode>c__AnonStorey2A storeya = new <BeginPingNode>c__AnonStorey2A {
                item = item,
                <>f__this = this
            };
            if ((storeya.item != null) && (topPixelOfRow >= 0f))
            {
                <BeginPingNode>c__AnonStorey2B storeyb = new <BeginPingNode>c__AnonStorey2B {
                    <>f__ref$42 = storeya,
                    <>f__this = this
                };
                this.m_Ping.m_TimeStart = Time.realtimeSinceStartup;
                this.m_Ping.m_PingStyle = s_Styles.ping;
                GUIContent content = GUIContent.Temp(storeya.item.displayName);
                Vector2 vector = this.m_Ping.m_PingStyle.CalcSize(content);
                this.m_Ping.m_ContentRect = new Rect(this.GetContentIndent(storeya.item), topPixelOfRow, ((this.k_IconWidth + this.k_SpaceBetweenIconAndText) + vector.x) + this.iconTotalPadding, vector.y);
                this.m_Ping.m_AvailableWidth = availableWidth;
                storeyb.useBoldFont = storeya.item.displayName.Equals("Assets");
                this.m_Ping.m_ContentDraw = new Action<Rect>(storeyb.<>m__3F);
                this.m_TreeView.Repaint();
            }
        }

        public virtual bool BeginRename(TreeViewItem item, float delay)
        {
            return this.GetRenameOverlay().BeginRename(item.displayName, item.id, delay);
        }

        public virtual void BeginRowGUI()
        {
            this.InitStyles();
            this.m_DraggingInsertionMarkerRect.x = -1f;
            this.SyncFakeItem();
            if (Event.current.type != EventType.Repaint)
            {
                this.DoRenameOverlay();
            }
        }

        protected virtual void ClearRenameAndNewNodeState()
        {
            this.m_TreeView.data.RemoveFakeItem();
            this.GetRenameOverlay().Clear();
        }

        protected virtual Rect DoFoldout(Rect rowRect, TreeViewItem item, int row)
        {
            bool flag;
            float foldoutIndent = this.GetFoldoutIndent(item);
            Rect position = new Rect(foldoutIndent, rowRect.y, this.k_FoldoutWidth, rowRect.height);
            TreeViewItemExpansionAnimator expansionAnimator = this.m_TreeView.expansionAnimator;
            EditorGUI.BeginChangeCheck();
            if (expansionAnimator.IsAnimating(item.id))
            {
                float num3;
                Matrix4x4 matrix = GUI.matrix;
                float num2 = Mathf.Min((float) 1f, (float) (expansionAnimator.expandedValueNormalized * 2f));
                if (!expansionAnimator.isExpanding)
                {
                    num3 = num2 * 90f;
                }
                else
                {
                    num3 = (1f - num2) * -90f;
                }
                GUIUtility.RotateAroundPivot(num3, position.center);
                bool isExpanding = expansionAnimator.isExpanding;
                flag = GUI.Toggle(position, isExpanding, GUIContent.none, s_Styles.foldout);
                GUI.matrix = matrix;
            }
            else
            {
                flag = GUI.Toggle(position, this.m_TreeView.data.IsExpanded(item), GUIContent.none, s_Styles.foldout);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.m_TreeView.UserInputChangedExpandedState(item, row, flag);
            }
            return position;
        }

        protected virtual void DoNodeGUI(Rect rect, int row, TreeViewItem item, bool selected, bool focused, bool useBoldFont)
        {
            EditorGUIUtility.SetIconSize(new Vector2(this.k_IconWidth, this.k_IconWidth));
            float foldoutIndent = this.GetFoldoutIndent(item);
            int itemControlID = TreeView.GetItemControlID(item);
            bool flag = false;
            if (this.m_TreeView.dragging != null)
            {
                flag = (this.m_TreeView.dragging.GetDropTargetControlID() == itemControlID) && this.m_TreeView.data.CanBeParent(item);
            }
            bool flag2 = this.IsRenaming(item.id);
            bool flag3 = this.m_TreeView.data.IsExpandable(item);
            if (flag2 && (Event.current.type == EventType.Repaint))
            {
                float num3 = (item.icon != null) ? this.k_IconWidth : 0f;
                float num4 = (((foldoutIndent + this.k_FoldoutWidth) + num3) + this.iconTotalPadding) - 1f;
                this.GetRenameOverlay().editFieldRect = new Rect(rect.x + num4, rect.y, rect.width - num4, rect.height);
            }
            if (Event.current.type == EventType.Repaint)
            {
                string displayName = item.displayName;
                if (flag2)
                {
                    selected = false;
                    displayName = string.Empty;
                }
                if (selected)
                {
                    s_Styles.selectionStyle.Draw(rect, false, false, true, focused);
                }
                if (flag)
                {
                    s_Styles.lineStyle.Draw(rect, GUIContent.none, true, true, false, false);
                }
                this.DrawIconAndLabel(rect, item, displayName, selected, focused, useBoldFont, false);
                if ((this.m_TreeView.dragging != null) && (this.m_TreeView.dragging.GetRowMarkerControlID() == itemControlID))
                {
                    this.m_DraggingInsertionMarkerRect = new Rect((rect.x + foldoutIndent) + this.k_FoldoutWidth, rect.y, rect.width - foldoutIndent, rect.height);
                }
            }
            if (flag3)
            {
                this.DoFoldout(rect, item, row);
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        public virtual void DoRenameOverlay()
        {
            if (this.GetRenameOverlay().IsRenaming() && !this.GetRenameOverlay().OnGUI())
            {
                this.EndRename();
            }
        }

        protected virtual void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (!isPinging)
            {
                float contentIndent = this.GetContentIndent(item);
                rect.x += contentIndent;
                rect.width -= contentIndent;
            }
            GUIStyle style = !useBoldFont ? s_Styles.lineStyle : s_Styles.lineBoldStyle;
            style.padding.left = (int) ((this.k_IconWidth + this.iconTotalPadding) + this.k_SpaceBetweenIconAndText);
            style.Draw(rect, label, false, false, selected, focused);
            Rect position = rect;
            position.width = this.k_IconWidth;
            position.height = this.k_IconWidth;
            position.x += this.iconLeftPadding;
            Texture iconForNode = this.GetIconForNode(item);
            if (iconForNode != null)
            {
                GUI.DrawTexture(position, iconForNode);
            }
            if (this.iconOverlayGUI != null)
            {
                Rect rect3 = rect;
                rect3.width = this.k_IconWidth + this.iconTotalPadding;
                this.iconOverlayGUI(item, rect3);
            }
        }

        public virtual void EndPingNode()
        {
            this.m_Ping.m_TimeStart = -1f;
        }

        public virtual void EndRename()
        {
            if (this.GetRenameOverlay().HasKeyboardFocus())
            {
                this.m_TreeView.GrabKeyboardFocus();
            }
            this.RenameEnded();
            this.ClearRenameAndNewNodeState();
        }

        public virtual void EndRowGUI()
        {
            if ((this.m_DraggingInsertionMarkerRect.x >= 0f) && (Event.current.type == EventType.Repaint))
            {
                if (this.m_TreeView.dragging.drawRowMarkerAbove)
                {
                    s_Styles.insertionAbove.Draw(this.m_DraggingInsertionMarkerRect, false, false, false, false);
                }
                else
                {
                    s_Styles.insertion.Draw(this.m_DraggingInsertionMarkerRect, false, false, false, false);
                }
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.DoRenameOverlay();
            }
            this.HandlePing();
        }

        public virtual float GetContentIndent(TreeViewItem item)
        {
            return (this.GetFoldoutIndent(item) + this.k_FoldoutWidth);
        }

        public virtual void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
        {
            if (this.m_TreeView.data.rowCount == 0)
            {
                firstRowVisible = lastRowVisible = -1;
            }
            else
            {
                float y = this.m_TreeView.state.scrollPos.y;
                float height = this.m_TreeView.GetTotalRect().height;
                firstRowVisible = (int) Mathf.Floor((y - this.topRowMargin) / this.k_LineHeight);
                lastRowVisible = firstRowVisible + ((int) Mathf.Ceil(height / this.k_LineHeight));
                firstRowVisible = Mathf.Max(firstRowVisible, 0);
                lastRowVisible = Mathf.Min(lastRowVisible, this.m_TreeView.data.rowCount - 1);
                if ((firstRowVisible >= this.m_TreeView.data.rowCount) && (firstRowVisible > 0))
                {
                    this.m_TreeView.state.scrollPos.y = 0f;
                    this.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
                }
            }
        }

        public virtual float GetFoldoutIndent(TreeViewItem item)
        {
            if (this.m_TreeView.isSearching)
            {
                return this.k_BaseIndent;
            }
            return (this.k_BaseIndent + (item.depth * this.indentWidth));
        }

        protected virtual Texture GetIconForNode(TreeViewItem item)
        {
            return item.icon;
        }

        protected float GetMaxWidth(List<TreeViewItem> rows)
        {
            this.InitStyles();
            float num = 1f;
            foreach (TreeViewItem item in rows)
            {
                float num3;
                float num4;
                float num2 = 0f;
                num2 += this.GetContentIndent(item);
                if (item.icon != null)
                {
                    num2 += this.k_IconWidth;
                }
                s_Styles.lineStyle.CalcMinMaxWidth(GUIContent.Temp(item.displayName), out num3, out num4);
                num2 += num4;
                num2 += this.k_BaseIndent;
                if (num2 > num)
                {
                    num = num2;
                }
            }
            return num;
        }

        public virtual int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
        {
            return (int) Mathf.Floor(heightOfTreeView / this.k_LineHeight);
        }

        public virtual Rect GetRectForFraming(int row)
        {
            return this.GetRowRect(row, 1f);
        }

        protected RenameOverlay GetRenameOverlay()
        {
            return this.m_TreeView.state.renameOverlay;
        }

        public virtual Rect GetRowRect(int row, float rowWidth)
        {
            return new Rect(0f, this.GetTopPixelOfRow(row), rowWidth, this.k_LineHeight);
        }

        private float GetTopPixelOfRow(int row)
        {
            return ((row * this.k_LineHeight) + this.topRowMargin);
        }

        public virtual Vector2 GetTotalSize()
        {
            this.InitStyles();
            float x = 1f;
            if (this.m_UseHorizontalScroll)
            {
                List<TreeViewItem> rows = this.m_TreeView.data.GetRows();
                x = this.GetMaxWidth(rows);
            }
            float y = ((this.m_TreeView.data.rowCount * this.k_LineHeight) + this.topRowMargin) + this.bottomRowMargin;
            if (this.m_AnimateScrollBarOnExpandCollapse && this.m_TreeView.expansionAnimator.isAnimating)
            {
                y -= this.m_TreeView.expansionAnimator.deltaHeight;
            }
            return new Vector2(x, y);
        }

        private void HandlePing()
        {
            this.m_Ping.HandlePing();
            if (this.m_Ping.isPinging)
            {
                this.m_TreeView.Repaint();
            }
        }

        protected virtual void InitStyles()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
        }

        protected virtual bool IsRenaming(int id)
        {
            return ((this.GetRenameOverlay().IsRenaming() && (this.GetRenameOverlay().userData == id)) && !this.GetRenameOverlay().isWaitingForDelay);
        }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused)
        {
            this.DoNodeGUI(rowRect, row, item, selected, focused, false);
        }

        protected virtual void RenameEnded()
        {
        }

        protected virtual void SyncFakeItem()
        {
        }

        public virtual float bottomRowMargin
        {
            get
            {
                return this.k_BottomRowMargin;
            }
        }

        public float halfDropBetweenHeight
        {
            get
            {
                return this.k_HalfDropBetweenHeight;
            }
        }

        public float iconLeftPadding { get; set; }

        public Action<TreeViewItem, Rect> iconOverlayGUI { get; set; }

        public float iconRightPadding { get; set; }

        public float iconTotalPadding
        {
            get
            {
                return (this.iconLeftPadding + this.iconRightPadding);
            }
        }

        protected float indentWidth
        {
            get
            {
                return (this.k_IndentWidth + this.iconTotalPadding);
            }
        }

        public virtual float topRowMargin
        {
            get
            {
                return this.k_TopRowMargin;
            }
        }

        [CompilerGenerated]
        private sealed class <BeginPingNode>c__AnonStorey2A
        {
            internal TreeViewGUI <>f__this;
            internal TreeViewItem item;
        }

        [CompilerGenerated]
        private sealed class <BeginPingNode>c__AnonStorey2B
        {
            internal TreeViewGUI.<BeginPingNode>c__AnonStorey2A <>f__ref$42;
            internal TreeViewGUI <>f__this;
            internal bool useBoldFont;

            internal void <>m__3F(Rect r)
            {
                this.<>f__this.DrawIconAndLabel(r, this.<>f__ref$42.item, this.<>f__ref$42.item.displayName, false, false, this.useBoldFont, true);
            }
        }

        internal class Styles
        {
            public GUIContent content = new GUIContent(EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName));
            public GUIStyle foldout = "IN Foldout";
            public GUIStyle insertion = "PR Insertion";
            public GUIStyle insertionAbove = "PR Insertion Above";
            public GUIStyle lineBoldStyle;
            public GUIStyle lineStyle = new GUIStyle("PR Label");
            public GUIStyle ping = new GUIStyle("PR Ping");
            public GUIStyle selectionStyle = new GUIStyle("PR Label");
            public GUIStyle toolbarButton = "ToolbarButton";

            public Styles()
            {
                Texture2D background = this.lineStyle.hover.background;
                this.lineStyle.onNormal.background = background;
                this.lineStyle.onActive.background = background;
                this.lineStyle.onFocused.background = background;
                this.lineStyle.alignment = TextAnchor.MiddleLeft;
                this.lineBoldStyle = new GUIStyle(this.lineStyle);
                this.lineBoldStyle.font = EditorStyles.boldLabel.font;
                this.lineBoldStyle.fontStyle = EditorStyles.boldLabel.fontStyle;
                this.ping.padding.left = 0x10;
                this.ping.padding.right = 0x10;
                this.ping.fixedHeight = 16f;
            }
        }
    }
}

