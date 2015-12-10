namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ExposablePopupMenu
    {
        private List<ItemData> m_Items;
        private float m_ItemSpacing;
        private float m_MinWidthOfPopup;
        private PopupButtonData m_PopupButtonData;
        private Action<ItemData> m_SelectionChangedCallback;
        private float m_WidthOfButtons;
        private float m_WidthOfPopup;

        private void CalcWidths()
        {
            this.m_WidthOfButtons = 0f;
            foreach (ItemData data in this.m_Items)
            {
                data.m_Width = data.m_Style.CalcSize(data.m_GUIContent).x;
                this.m_WidthOfButtons += data.m_Width;
            }
            this.m_WidthOfButtons += (this.m_Items.Count - 1) * this.m_ItemSpacing;
            Vector2 vector = this.m_PopupButtonData.m_Style.CalcSize(this.m_PopupButtonData.m_GUIContent);
            vector.x += 3f;
            this.m_WidthOfPopup = vector.x;
        }

        public void Init(List<ItemData> items, float itemSpacing, float minWidthOfPopup, PopupButtonData popupButtonData, Action<ItemData> selectionChangedCallback)
        {
            this.m_Items = items;
            this.m_ItemSpacing = itemSpacing;
            this.m_PopupButtonData = popupButtonData;
            this.m_SelectionChangedCallback = selectionChangedCallback;
            this.m_MinWidthOfPopup = minWidthOfPopup;
            this.CalcWidths();
        }

        public float OnGUI(Rect rect)
        {
            if ((rect.width >= this.m_WidthOfButtons) && (rect.width > this.m_MinWidthOfPopup))
            {
                Rect position = rect;
                foreach (ItemData data in this.m_Items)
                {
                    position.width = data.m_Width;
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.BeginDisabledGroup(!data.m_Enabled);
                    GUI.Toggle(position, data.m_On, data.m_GUIContent, data.m_Style);
                    EditorGUI.EndDisabledGroup();
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.SelectionChanged(data);
                        GUIUtility.ExitGUI();
                    }
                    position.x += data.m_Width + this.m_ItemSpacing;
                }
                return this.m_WidthOfButtons;
            }
            if (this.m_WidthOfPopup < rect.width)
            {
                rect.width = this.m_WidthOfPopup;
            }
            if (EditorGUI.ButtonMouseDown(rect, this.m_PopupButtonData.m_GUIContent, FocusType.Passive, this.m_PopupButtonData.m_Style))
            {
                PopUpMenu.Show(rect, this.m_Items, this);
            }
            return this.m_WidthOfPopup;
        }

        private void SelectionChanged(ItemData item)
        {
            if (this.m_SelectionChangedCallback != null)
            {
                this.m_SelectionChangedCallback(item);
            }
            else
            {
                Debug.LogError("Callback is null");
            }
        }

        public class ItemData
        {
            public bool m_Enabled;
            public GUIContent m_GUIContent;
            public bool m_On;
            public GUIStyle m_Style;
            public object m_UserData;
            public float m_Width;

            public ItemData(GUIContent content, GUIStyle style, bool on, bool enabled, object userData)
            {
                this.m_GUIContent = content;
                this.m_Style = style;
                this.m_On = on;
                this.m_Enabled = enabled;
                this.m_UserData = userData;
            }
        }

        public class PopupButtonData
        {
            public GUIContent m_GUIContent;
            public GUIStyle m_Style;

            public PopupButtonData(GUIContent content, GUIStyle style)
            {
                this.m_GUIContent = content;
                this.m_Style = style;
            }
        }

        internal class PopUpMenu
        {
            private static ExposablePopupMenu m_Caller;
            private static List<ExposablePopupMenu.ItemData> m_Data;

            private static void SelectionCallback(object userData)
            {
                ExposablePopupMenu.ItemData item = (ExposablePopupMenu.ItemData) userData;
                m_Caller.SelectionChanged(item);
                m_Caller = null;
                m_Data = null;
            }

            internal static void Show(Rect activatorRect, List<ExposablePopupMenu.ItemData> buttonData, ExposablePopupMenu caller)
            {
                m_Data = buttonData;
                m_Caller = caller;
                GenericMenu menu = new GenericMenu();
                foreach (ExposablePopupMenu.ItemData data in m_Data)
                {
                    if (data.m_Enabled)
                    {
                        menu.AddItem(data.m_GUIContent, data.m_On, new GenericMenu.MenuFunction2(ExposablePopupMenu.PopUpMenu.SelectionCallback), data);
                    }
                    else
                    {
                        menu.AddDisabledItem(data.m_GUIContent);
                    }
                }
                menu.DropDown(activatorRect);
            }
        }
    }
}

