namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class FlexibleMenu : PopupWindowContent
    {
        private const float leftMargin = 25f;
        private const float lineHeight = 18f;
        private int m_HoverIndex;
        private readonly Action<int, object> m_ItemClickedCallback;
        private IFlexibleMenuItemProvider m_ItemProvider;
        private float m_MaxTextWidth = -1f;
        private FlexibleMenuModifyItemUI m_ModifyItemUI;
        private Vector2 m_ScrollPosition = Vector2.zero;
        private int[] m_SeperatorIndices;
        private bool m_ShowAddNewPresetItem;
        private int m_ShowEditWindowForIndex = -1;
        private static Styles s_Styles;
        private const float seperatorHeight = 8f;

        public FlexibleMenu(IFlexibleMenuItemProvider itemProvider, int selectionIndex, FlexibleMenuModifyItemUI modifyItemUi, Action<int, object> itemClickedCallback)
        {
            this.m_ItemProvider = itemProvider;
            this.m_ModifyItemUI = modifyItemUi;
            this.m_ItemClickedCallback = itemClickedCallback;
            this.m_SeperatorIndices = this.m_ItemProvider.GetSeperatorIndices();
            this.selectedIndex = selectionIndex;
            this.m_ShowAddNewPresetItem = this.m_ModifyItemUI != null;
        }

        private bool AllowDeleteClick(int index)
        {
            return ((this.IsDeleteModiferPressed() && this.m_ItemProvider.IsModificationAllowed(index)) && (GUIUtility.hotControl == 0));
        }

        private Vector2 CalcSize()
        {
            float y = ((this.maxIndex + 1) * 18f) + (this.m_SeperatorIndices.Length * 8f);
            if (this.m_MaxTextWidth < 0f)
            {
                this.m_MaxTextWidth = Math.Max(200f, this.CalcWidth());
            }
            return new Vector2(this.m_MaxTextWidth, y);
        }

        private float CalcWidth()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            float b = 0f;
            for (int i = 0; i < this.m_ItemProvider.Count(); i++)
            {
                b = Mathf.Max(s_Styles.menuItem.CalcSize(GUIContent.Temp(this.m_ItemProvider.GetName(i))).x, b);
            }
            return (b + 6f);
        }

        private void ClearCachedWidth()
        {
            this.m_MaxTextWidth = -1f;
        }

        private void CreateNewItemButton(Rect itemRect)
        {
            if (this.m_ModifyItemUI != null)
            {
                Rect position = new Rect(itemRect.x + 25f, itemRect.y, 15f, 15f);
                if (GUI.Button(position, s_Styles.plusButtonText, "OL Plus"))
                {
                    position.y -= 15f;
                    this.m_ModifyItemUI.Init(FlexibleMenuModifyItemUI.MenuType.Add, this.m_ItemProvider.Create(), delegate (object obj) {
                        this.ClearCachedWidth();
                        int index = this.m_ItemProvider.Add(obj);
                        this.SelectItem(index);
                        EditorApplication.RequestRepaintAllViews();
                    });
                    PopupWindow.Show(position, this.m_ModifyItemUI);
                }
            }
        }

        private void DeleteItem(int index)
        {
            this.ClearCachedWidth();
            this.m_ItemProvider.Remove(index);
            this.selectedIndex = Mathf.Clamp(this.selectedIndex, 0, this.m_ItemProvider.Count() - 1);
        }

        public static void DrawRect(Rect rect, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color2 = GUI.color;
                GUI.color *= color;
                GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                GUI.color = color2;
            }
        }

        private void EditExistingItem(Rect itemRect, int index)
        {
            <EditExistingItem>c__AnonStorey73 storey = new <EditExistingItem>c__AnonStorey73 {
                index = index,
                <>f__this = this
            };
            if (this.m_ModifyItemUI != null)
            {
                itemRect.y -= itemRect.height;
                itemRect.x += itemRect.width;
                this.m_ModifyItemUI.Init(FlexibleMenuModifyItemUI.MenuType.Edit, this.m_ItemProvider.GetItem(storey.index), new Action<object>(storey.<>m__10A));
                PopupWindow.Show(itemRect, this.m_ModifyItemUI);
            }
        }

        public override Vector2 GetWindowSize()
        {
            return this.CalcSize();
        }

        private bool IsDeleteModiferPressed()
        {
            return Event.current.alt;
        }

        public override void OnGUI(Rect rect)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Event current = Event.current;
            Rect viewRect = new Rect(0f, 0f, 1f, this.CalcSize().y);
            this.m_ScrollPosition = GUI.BeginScrollView(rect, this.m_ScrollPosition, viewRect);
            float y = 0f;
            for (int i = 0; i <= this.maxIndex; i++)
            {
                bool flag2;
                int num3 = i + 0xf4240;
                Rect itemRect = new Rect(0f, y, rect.width, 18f);
                bool flag = Array.IndexOf<int>(this.m_SeperatorIndices, i) >= 0;
                if (this.m_ShowAddNewPresetItem && (i == this.m_ItemProvider.Count()))
                {
                    this.CreateNewItemButton(itemRect);
                    continue;
                }
                if (this.m_ShowEditWindowForIndex == i)
                {
                    this.m_ShowEditWindowForIndex = -1;
                    this.EditExistingItem(itemRect, i);
                }
                switch (current.type)
                {
                    case EventType.MouseDown:
                        if ((current.button == 0) && itemRect.Contains(current.mousePosition))
                        {
                            GUIUtility.hotControl = num3;
                            if (!this.IsDeleteModiferPressed() && (current.clickCount == 1))
                            {
                                GUIUtility.hotControl = 0;
                                this.SelectItem(i);
                                base.editorWindow.Close();
                                current.Use();
                            }
                        }
                        goto Label_0389;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == num3)
                        {
                            GUIUtility.hotControl = 0;
                            if (((current.button == 0) && itemRect.Contains(current.mousePosition)) && this.AllowDeleteClick(i))
                            {
                                this.DeleteItem(i);
                                current.Use();
                            }
                        }
                        goto Label_0389;

                    case EventType.MouseMove:
                        if (!itemRect.Contains(current.mousePosition))
                        {
                            goto Label_036B;
                        }
                        if (this.m_HoverIndex != i)
                        {
                            this.m_HoverIndex = i;
                            this.Repaint();
                        }
                        goto Label_0389;

                    case EventType.Repaint:
                        flag2 = false;
                        if (this.m_HoverIndex == i)
                        {
                            if (!itemRect.Contains(current.mousePosition))
                            {
                                break;
                            }
                            flag2 = true;
                        }
                        goto Label_0147;

                    case EventType.ContextClick:
                        if (itemRect.Contains(current.mousePosition))
                        {
                            current.Use();
                            if ((this.m_ModifyItemUI != null) && this.m_ItemProvider.IsModificationAllowed(i))
                            {
                                ItemContextMenu.Show(i, this);
                            }
                        }
                        goto Label_0389;

                    default:
                        goto Label_0389;
                }
                this.m_HoverIndex = -1;
            Label_0147:
                if ((this.m_ModifyItemUI != null) && this.m_ModifyItemUI.IsShowing())
                {
                    flag2 = this.m_ItemProvider.GetItem(i) == this.m_ModifyItemUI.m_Object;
                }
                s_Styles.menuItem.Draw(itemRect, GUIContent.Temp(this.m_ItemProvider.GetName(i)), flag2, false, i == this.selectedIndex, false);
                if (flag)
                {
                    Rect rect4 = new Rect(itemRect.x + 4f, (itemRect.y + itemRect.height) + 4f, itemRect.width - 8f, 1f);
                    DrawRect(rect4, !EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.32f, 0.32f, 0.32f, 1.333f));
                }
                if (this.AllowDeleteClick(i))
                {
                    EditorGUIUtility.AddCursorRect(itemRect, MouseCursor.ArrowMinus);
                }
                goto Label_0389;
            Label_036B:
                if (this.m_HoverIndex == i)
                {
                    this.m_HoverIndex = -1;
                    this.Repaint();
                }
            Label_0389:
                y += 18f;
                if (flag)
                {
                    y += 8f;
                }
            }
            GUI.EndScrollView();
        }

        private void Repaint()
        {
            HandleUtility.Repaint();
        }

        private void SelectItem(int index)
        {
            this.selectedIndex = index;
            if ((this.m_ItemClickedCallback != null) && (index >= 0))
            {
                this.m_ItemClickedCallback(index, this.m_ItemProvider.GetItem(index));
            }
        }

        private int maxIndex
        {
            get
            {
                return (!this.m_ShowAddNewPresetItem ? (this.m_ItemProvider.Count() - 1) : this.m_ItemProvider.Count());
            }
        }

        public int selectedIndex { get; set; }

        [CompilerGenerated]
        private sealed class <EditExistingItem>c__AnonStorey73
        {
            internal FlexibleMenu <>f__this;
            internal int index;

            internal void <>m__10A(object obj)
            {
                this.<>f__this.ClearCachedWidth();
                this.<>f__this.m_ItemProvider.Replace(this.index, obj);
                EditorApplication.RequestRepaintAllViews();
            }
        }

        internal static class ItemContextMenu
        {
            private static FlexibleMenu s_Caller;

            private static void Delete(object userData)
            {
                int index = (int) userData;
                s_Caller.DeleteItem(index);
            }

            private static void Edit(object userData)
            {
                int num = (int) userData;
                s_Caller.m_ShowEditWindowForIndex = num;
            }

            public static void Show(int itemIndex, FlexibleMenu caller)
            {
                s_Caller = caller;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Edit..."), false, new GenericMenu.MenuFunction2(FlexibleMenu.ItemContextMenu.Edit), itemIndex);
                menu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction2(FlexibleMenu.ItemContextMenu.Delete), itemIndex);
                menu.ShowAsContext();
                GUIUtility.ExitGUI();
            }
        }

        private class Styles
        {
            public GUIStyle menuItem = "MenuItem";
            public GUIContent plusButtonText = new GUIContent(string.Empty, "Add New Item");
        }
    }
}

