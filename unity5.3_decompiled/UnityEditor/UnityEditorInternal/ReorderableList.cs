namespace UnityEditorInternal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    public class ReorderableList
    {
        public bool displayAdd;
        public bool displayRemove;
        public ElementCallbackDelegate drawElementBackgroundCallback;
        public ElementCallbackDelegate drawElementCallback;
        public FooterCallbackDelegate drawFooterCallback;
        public HeaderCallbackDelegate drawHeaderCallback;
        public float elementHeight;
        public ElementHeightCallbackDelegate elementHeightCallback;
        public float footerHeight;
        public float headerHeight;
        private int id;
        private int m_ActiveElement;
        private bool m_DisplayHeader;
        private bool m_Draggable;
        private float m_DraggedY;
        private bool m_Dragging;
        private float m_DragOffset;
        private IList m_ElementList;
        private SerializedProperty m_Elements;
        private List<int> m_NonDragTargetIndices;
        private SerializedObject m_SerializedObject;
        private GUISlideGroup m_SlideGroup;
        public AddCallbackDelegate onAddCallback;
        public AddDropdownCallbackDelegate onAddDropdownCallback;
        public CanRemoveCallbackDelegate onCanRemoveCallback;
        public ChangedCallbackDelegate onChangedCallback;
        public SelectCallbackDelegate onMouseUpCallback;
        public RemoveCallbackDelegate onRemoveCallback;
        public ReorderCallbackDelegate onReorderCallback;
        public SelectCallbackDelegate onSelectCallback;
        private static Defaults s_Defaults;
        public bool showDefaultBackground;

        public ReorderableList(IList elements, Type elementType)
        {
            this.m_ActiveElement = -1;
            this.id = -1;
            this.elementHeight = 21f;
            this.headerHeight = 18f;
            this.footerHeight = 13f;
            this.showDefaultBackground = true;
            this.InitList(null, null, elements, true, true, true, true);
        }

        public ReorderableList(SerializedObject serializedObject, SerializedProperty elements)
        {
            this.m_ActiveElement = -1;
            this.id = -1;
            this.elementHeight = 21f;
            this.headerHeight = 18f;
            this.footerHeight = 13f;
            this.showDefaultBackground = true;
            this.InitList(serializedObject, elements, null, true, true, true, true);
        }

        public ReorderableList(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            this.m_ActiveElement = -1;
            this.id = -1;
            this.elementHeight = 21f;
            this.headerHeight = 18f;
            this.footerHeight = 13f;
            this.showDefaultBackground = true;
            this.InitList(null, null, elements, draggable, displayHeader, displayAddButton, displayRemoveButton);
        }

        public ReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            this.m_ActiveElement = -1;
            this.id = -1;
            this.elementHeight = 21f;
            this.headerHeight = 18f;
            this.footerHeight = 13f;
            this.showDefaultBackground = true;
            this.InitList(serializedObject, elements, null, draggable, displayHeader, displayAddButton, displayRemoveButton);
        }

        private int CalculateRowIndex()
        {
            return this.GetRowIndex(this.m_DraggedY);
        }

        private void DoDraggingAndSelection(Rect listRect)
        {
            Event current = Event.current;
            int activeElement = this.m_ActiveElement;
            bool flag = false;
            switch (current.GetTypeForControl(this.id))
            {
                case EventType.MouseDown:
                    if (listRect.Contains(Event.current.mousePosition) && (Event.current.button == 0))
                    {
                        EditorGUI.EndEditingActiveTextField();
                        this.m_ActiveElement = this.GetRowIndex(Event.current.mousePosition.y - listRect.y);
                        if (this.m_Draggable)
                        {
                            this.m_DragOffset = (Event.current.mousePosition.y - listRect.y) - this.GetElementYOffset(this.m_ActiveElement);
                            this.UpdateDraggedY(listRect);
                            GUIUtility.hotControl = this.id;
                            this.m_SlideGroup.Reset();
                            this.m_NonDragTargetIndices = new List<int>();
                        }
                        this.GrabKeyboardFocus();
                        current.Use();
                        flag = true;
                        break;
                    }
                    break;

                case EventType.MouseUp:
                    if (this.m_Draggable)
                    {
                        if (GUIUtility.hotControl == this.id)
                        {
                            current.Use();
                            this.m_Dragging = false;
                            int dstIndex = this.CalculateRowIndex();
                            if (this.m_ActiveElement != dstIndex)
                            {
                                if ((this.m_SerializedObject != null) && (this.m_Elements != null))
                                {
                                    this.m_Elements.MoveArrayElement(this.m_ActiveElement, dstIndex);
                                    this.m_SerializedObject.ApplyModifiedProperties();
                                    this.m_SerializedObject.Update();
                                }
                                else if (this.m_ElementList != null)
                                {
                                    object obj2 = this.m_ElementList[this.m_ActiveElement];
                                    for (int i = 0; i < (this.m_ElementList.Count - 1); i++)
                                    {
                                        if (i >= this.m_ActiveElement)
                                        {
                                            this.m_ElementList[i] = this.m_ElementList[i + 1];
                                        }
                                    }
                                    for (int j = this.m_ElementList.Count - 1; j > 0; j--)
                                    {
                                        if (j > dstIndex)
                                        {
                                            this.m_ElementList[j] = this.m_ElementList[j - 1];
                                        }
                                    }
                                    this.m_ElementList[dstIndex] = obj2;
                                }
                                this.m_ActiveElement = dstIndex;
                                if (this.onReorderCallback != null)
                                {
                                    this.onReorderCallback(this);
                                }
                                if (this.onChangedCallback != null)
                                {
                                    this.onChangedCallback(this);
                                }
                            }
                            else if (this.onMouseUpCallback != null)
                            {
                                this.onMouseUpCallback(this);
                            }
                            GUIUtility.hotControl = 0;
                            this.m_NonDragTargetIndices = null;
                        }
                        break;
                    }
                    if ((this.onMouseUpCallback != null) && this.IsMouseInsideActiveElement(listRect))
                    {
                        this.onMouseUpCallback(this);
                    }
                    break;

                case EventType.MouseDrag:
                    if (this.m_Draggable && (GUIUtility.hotControl == this.id))
                    {
                        this.m_Dragging = true;
                        this.UpdateDraggedY(listRect);
                        current.Use();
                        break;
                    }
                    break;

                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == this.id)
                    {
                        if (current.keyCode == KeyCode.DownArrow)
                        {
                            this.m_ActiveElement++;
                            current.Use();
                        }
                        if (current.keyCode == KeyCode.UpArrow)
                        {
                            this.m_ActiveElement--;
                            current.Use();
                        }
                        if ((current.keyCode == KeyCode.Escape) && (GUIUtility.hotControl == this.id))
                        {
                            GUIUtility.hotControl = 0;
                            this.m_Dragging = false;
                            current.Use();
                        }
                        this.m_ActiveElement = Mathf.Clamp(this.m_ActiveElement, 0, (this.m_Elements == null) ? (this.m_ElementList.Count - 1) : (this.m_Elements.arraySize - 1));
                        break;
                    }
                    return;
            }
            if (((this.m_ActiveElement != activeElement) || flag) && (this.onSelectCallback != null))
            {
                this.onSelectCallback(this);
            }
        }

        public void DoLayoutList()
        {
            if (s_Defaults == null)
            {
                s_Defaults = new Defaults();
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect headerRect = GUILayoutUtility.GetRect((float) 0f, this.headerHeight, options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect listRect = GUILayoutUtility.GetRect((float) 10f, this.GetListElementHeight(), optionArray2);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect footerRect = GUILayoutUtility.GetRect((float) 4f, this.footerHeight, optionArray3);
            this.DoListHeader(headerRect);
            this.DoListElements(listRect);
            this.DoListFooter(footerRect);
        }

        public void DoList(Rect rect)
        {
            if (s_Defaults == null)
            {
                s_Defaults = new Defaults();
            }
            Rect headerRect = new Rect(rect.x, rect.y, rect.width, this.headerHeight);
            Rect listRect = new Rect(rect.x, headerRect.y + headerRect.height, rect.width, this.GetListElementHeight());
            Rect footerRect = new Rect(rect.x, listRect.y + listRect.height, rect.width, this.footerHeight);
            this.DoListHeader(headerRect);
            this.DoListElements(listRect);
            this.DoListFooter(footerRect);
        }

        private void DoListElements(Rect listRect)
        {
            int count = this.count;
            if (this.showDefaultBackground && (Event.current.type == EventType.Repaint))
            {
                s_Defaults.boxBackground.Draw(listRect, false, false, false, false);
            }
            listRect.yMin += 2f;
            listRect.yMax -= 5f;
            Rect r = listRect;
            r.height = this.elementHeight;
            Rect rect = r;
            if ((((this.m_Elements != null) && this.m_Elements.isArray) || (this.m_ElementList != null)) && (count > 0))
            {
                if (this.IsDragging() && (Event.current.type == EventType.Repaint))
                {
                    int index = this.CalculateRowIndex();
                    this.m_NonDragTargetIndices.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        if (i != this.m_ActiveElement)
                        {
                            this.m_NonDragTargetIndices.Add(i);
                        }
                    }
                    this.m_NonDragTargetIndices.Insert(index, -1);
                    for (int j = 0; j < this.m_NonDragTargetIndices.Count; j++)
                    {
                        if (this.m_NonDragTargetIndices[j] != -1)
                        {
                            r.y = listRect.y + this.GetElementYOffset(j);
                            r = this.m_SlideGroup.GetRect(this.m_NonDragTargetIndices[j], r);
                            if (this.drawElementBackgroundCallback == null)
                            {
                                s_Defaults.DrawElementBackground(r, j, false, false, this.m_Draggable);
                            }
                            else
                            {
                                this.drawElementBackgroundCallback(r, j, false, false);
                            }
                            s_Defaults.DrawElementDraggingHandle(r, j, false, false, this.m_Draggable);
                            rect = this.GetContentRect(r);
                            if (this.drawElementCallback == null)
                            {
                                if (this.m_Elements != null)
                                {
                                    s_Defaults.DrawElement(rect, this.m_Elements.GetArrayElementAtIndex(this.m_NonDragTargetIndices[j]), null, false, false, this.m_Draggable);
                                }
                                else
                                {
                                    s_Defaults.DrawElement(rect, null, this.m_ElementList[this.m_NonDragTargetIndices[j]], false, false, this.m_Draggable);
                                }
                            }
                            else
                            {
                                this.drawElementCallback(rect, this.m_NonDragTargetIndices[j], false, false);
                            }
                        }
                    }
                    r.y = (this.m_DraggedY - this.m_DragOffset) + listRect.y;
                    if (this.drawElementBackgroundCallback == null)
                    {
                        s_Defaults.DrawElementBackground(r, this.m_ActiveElement, true, true, this.m_Draggable);
                    }
                    else
                    {
                        this.drawElementBackgroundCallback(r, this.m_ActiveElement, true, true);
                    }
                    s_Defaults.DrawElementDraggingHandle(r, this.m_ActiveElement, true, true, this.m_Draggable);
                    rect = this.GetContentRect(r);
                    if (this.drawElementCallback == null)
                    {
                        if (this.m_Elements != null)
                        {
                            s_Defaults.DrawElement(rect, this.m_Elements.GetArrayElementAtIndex(this.m_ActiveElement), null, true, true, this.m_Draggable);
                        }
                        else
                        {
                            s_Defaults.DrawElement(rect, null, this.m_ElementList[this.m_ActiveElement], true, true, this.m_Draggable);
                        }
                    }
                    else
                    {
                        this.drawElementCallback(rect, this.m_ActiveElement, true, true);
                    }
                }
                else
                {
                    for (int k = 0; k < count; k++)
                    {
                        bool selected = k == this.m_ActiveElement;
                        bool focused = (k == this.m_ActiveElement) && this.HasKeyboardControl();
                        r.y = listRect.y + this.GetElementYOffset(k);
                        if (this.drawElementBackgroundCallback == null)
                        {
                            s_Defaults.DrawElementBackground(r, k, selected, focused, this.m_Draggable);
                        }
                        else
                        {
                            this.drawElementBackgroundCallback(r, k, selected, focused);
                        }
                        s_Defaults.DrawElementDraggingHandle(r, k, selected, focused, this.m_Draggable);
                        rect = this.GetContentRect(r);
                        if (this.drawElementCallback == null)
                        {
                            if (this.m_Elements != null)
                            {
                                s_Defaults.DrawElement(rect, this.m_Elements.GetArrayElementAtIndex(k), null, selected, focused, this.m_Draggable);
                            }
                            else
                            {
                                s_Defaults.DrawElement(rect, null, this.m_ElementList[k], selected, focused, this.m_Draggable);
                            }
                        }
                        else
                        {
                            this.drawElementCallback(rect, k, selected, focused);
                        }
                    }
                }
                this.DoDraggingAndSelection(listRect);
            }
            else
            {
                r.y = listRect.y;
                if (this.drawElementBackgroundCallback == null)
                {
                    s_Defaults.DrawElementBackground(r, -1, false, false, false);
                }
                else
                {
                    this.drawElementBackgroundCallback(r, -1, false, false);
                }
                s_Defaults.DrawElementDraggingHandle(r, -1, false, false, false);
                rect = r;
                rect.xMin += 6f;
                rect.xMax -= 6f;
                s_Defaults.DrawNoneElement(rect, this.m_Draggable);
            }
        }

        private void DoListFooter(Rect footerRect)
        {
            if (this.drawFooterCallback != null)
            {
                this.drawFooterCallback(footerRect);
            }
            else if (this.displayAdd || this.displayRemove)
            {
                s_Defaults.DrawFooter(footerRect, this);
            }
        }

        private void DoListHeader(Rect headerRect)
        {
            if (this.showDefaultBackground && (Event.current.type == EventType.Repaint))
            {
                s_Defaults.DrawHeaderBackground(headerRect);
            }
            headerRect.xMin += 6f;
            headerRect.xMax -= 6f;
            headerRect.height -= 2f;
            headerRect.y++;
            if (this.drawHeaderCallback != null)
            {
                this.drawHeaderCallback(headerRect);
            }
            else if (this.m_DisplayHeader)
            {
                s_Defaults.DrawHeader(headerRect, this.m_SerializedObject, this.m_Elements, this.m_ElementList);
            }
        }

        private Rect GetContentRect(Rect rect)
        {
            Rect rect2 = rect;
            if (this.draggable)
            {
                rect2.xMin += 20f;
            }
            else
            {
                rect2.xMin += 6f;
            }
            rect2.xMax -= 6f;
            return rect2;
        }

        private float GetElementHeight(int index)
        {
            if (this.elementHeightCallback == null)
            {
                return this.elementHeight;
            }
            return this.elementHeightCallback(index);
        }

        private float GetElementYOffset(int index)
        {
            if (this.elementHeightCallback == null)
            {
                return (index * this.elementHeight);
            }
            float num = 0f;
            for (int i = 0; i < index; i++)
            {
                num += this.elementHeightCallback(i);
            }
            return num;
        }

        public float GetHeight()
        {
            float num = 0f;
            num += this.GetListElementHeight();
            num += this.headerHeight;
            return (num + this.footerHeight);
        }

        private float GetListElementHeight()
        {
            int count = this.count;
            if (count == 0)
            {
                return (this.elementHeight + 7f);
            }
            if (this.elementHeightCallback != null)
            {
                return ((this.GetElementYOffset(count - 1) + this.GetElementHeight(count - 1)) + 7f);
            }
            return ((this.elementHeight * count) + 7f);
        }

        private int GetRowIndex(float localY)
        {
            if (this.elementHeightCallback == null)
            {
                return Mathf.Clamp(Mathf.FloorToInt(localY / this.elementHeight), 0, this.count - 1);
            }
            float num = 0f;
            for (int i = 0; i < this.count; i++)
            {
                float num3 = this.elementHeightCallback(i);
                float num4 = num + num3;
                if ((localY >= num) && (localY < num4))
                {
                    return i;
                }
                num += num3;
            }
            return (this.count - 1);
        }

        private Rect GetRowRect(int index, Rect listRect)
        {
            return new Rect(listRect.x, listRect.y + this.GetElementYOffset(index), listRect.width, this.GetElementHeight(index));
        }

        public void GrabKeyboardFocus()
        {
            GUIUtility.keyboardControl = this.id;
        }

        public bool HasKeyboardControl()
        {
            return (GUIUtility.keyboardControl == this.id);
        }

        private void InitList(SerializedObject serializedObject, SerializedProperty elements, IList elementList, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            this.id = GUIUtility.GetPermanentControlID();
            this.m_SerializedObject = serializedObject;
            this.m_Elements = elements;
            this.m_ElementList = elementList;
            this.m_Draggable = draggable;
            this.m_Dragging = false;
            this.m_SlideGroup = new GUISlideGroup();
            this.displayAdd = displayAddButton;
            this.m_DisplayHeader = displayHeader;
            this.displayRemove = displayRemoveButton;
            if ((this.m_Elements != null) && !this.m_Elements.editable)
            {
                this.m_Draggable = false;
            }
            if ((this.m_Elements != null) && !this.m_Elements.isArray)
            {
                Debug.LogError("Input elements should be an Array SerializedProperty");
            }
        }

        private bool IsDragging()
        {
            return this.m_Dragging;
        }

        private bool IsMouseInsideActiveElement(Rect listRect)
        {
            int rowIndex = this.GetRowIndex(Event.current.mousePosition.y - listRect.y);
            return ((rowIndex == this.m_ActiveElement) && this.GetRowRect(rowIndex, listRect).Contains(Event.current.mousePosition));
        }

        public void ReleaseKeyboardFocus()
        {
            if (GUIUtility.keyboardControl == this.id)
            {
                GUIUtility.keyboardControl = 0;
            }
        }

        private void UpdateDraggedY(Rect listRect)
        {
            this.m_DraggedY = Mathf.Clamp(Event.current.mousePosition.y - listRect.y, this.m_DragOffset, listRect.height - (this.GetElementHeight(this.m_ActiveElement) - this.m_DragOffset));
        }

        public int count
        {
            get
            {
                if (this.m_Elements == null)
                {
                    return this.m_ElementList.Count;
                }
                if (!this.m_Elements.hasMultipleDifferentValues)
                {
                    return this.m_Elements.arraySize;
                }
                int arraySize = this.m_Elements.arraySize;
                foreach (Object obj2 in this.m_Elements.serializedObject.targetObjects)
                {
                    SerializedObject obj3 = new SerializedObject(obj2);
                    arraySize = Math.Min(obj3.FindProperty(this.m_Elements.propertyPath).arraySize, arraySize);
                }
                return arraySize;
            }
        }

        public static Defaults defaultBehaviours
        {
            get
            {
                return s_Defaults;
            }
        }

        public bool draggable
        {
            get
            {
                return this.m_Draggable;
            }
            set
            {
                this.m_Draggable = value;
            }
        }

        public int index
        {
            get
            {
                return this.m_ActiveElement;
            }
            set
            {
                this.m_ActiveElement = value;
            }
        }

        public IList list
        {
            get
            {
                return this.m_ElementList;
            }
            set
            {
                this.m_ElementList = value;
            }
        }

        public SerializedProperty serializedProperty
        {
            get
            {
                return this.m_Elements;
            }
            set
            {
                this.m_Elements = value;
            }
        }

        public delegate void AddCallbackDelegate(ReorderableList list);

        public delegate void AddDropdownCallbackDelegate(Rect buttonRect, ReorderableList list);

        public delegate bool CanRemoveCallbackDelegate(ReorderableList list);

        public delegate void ChangedCallbackDelegate(ReorderableList list);

        public class Defaults
        {
            public readonly GUIStyle boxBackground = "RL Background";
            private const int buttonWidth = 0x19;
            public readonly GUIStyle draggingHandle = "RL DragHandle";
            public const int dragHandleWidth = 20;
            public GUIStyle elementBackground = new GUIStyle("RL Element");
            public readonly GUIStyle footerBackground = "RL Footer";
            public readonly GUIStyle headerBackground = "RL Header";
            public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");
            public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
            public GUIContent iconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add to list");
            public const int padding = 6;
            public readonly GUIStyle preButton = "RL FooterButton";

            public void DoAddButton(ReorderableList list)
            {
                if (list.serializedProperty != null)
                {
                    SerializedProperty serializedProperty = list.serializedProperty;
                    serializedProperty.arraySize++;
                    list.index = list.serializedProperty.arraySize - 1;
                }
                else
                {
                    Type elementType = list.list.GetType().GetElementType();
                    if (elementType == typeof(string))
                    {
                        list.index = list.list.Add(string.Empty);
                    }
                    else if ((elementType != null) && (elementType.GetConstructor(Type.EmptyTypes) == null))
                    {
                        Debug.LogError("Cannot add element. Type " + elementType.ToString() + " has no default constructor. Implement a default constructor or implement your own add behaviour.");
                    }
                    else if (list.list.GetType().GetGenericArguments()[0] != null)
                    {
                        list.index = list.list.Add(Activator.CreateInstance(list.list.GetType().GetGenericArguments()[0]));
                    }
                    else if (elementType != null)
                    {
                        list.index = list.list.Add(Activator.CreateInstance(elementType));
                    }
                    else
                    {
                        Debug.LogError("Cannot add element of type Null.");
                    }
                }
            }

            public void DoRemoveButton(ReorderableList list)
            {
                if (list.serializedProperty != null)
                {
                    list.serializedProperty.DeleteArrayElementAtIndex(list.index);
                    if (list.index >= (list.serializedProperty.arraySize - 1))
                    {
                        list.index = list.serializedProperty.arraySize - 1;
                    }
                }
                else
                {
                    list.list.RemoveAt(list.index);
                    if (list.index >= (list.list.Count - 1))
                    {
                        list.index = list.list.Count - 1;
                    }
                }
            }

            public void DrawElement(Rect rect, SerializedProperty element, object listItem, bool selected, bool focused, bool draggable)
            {
                EditorGUI.LabelField(rect, EditorGUIUtility.TempContent((element == null) ? listItem.ToString() : element.displayName));
            }

            public void DrawElementBackground(Rect rect, int index, bool selected, bool focused, bool draggable)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    this.elementBackground.Draw(rect, false, selected, selected, focused);
                }
            }

            public void DrawElementDraggingHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
            {
                if ((Event.current.type == EventType.Repaint) && draggable)
                {
                    this.draggingHandle.Draw(new Rect(rect.x + 5f, rect.y + 7f, 10f, rect.height - (rect.height - 7f)), false, false, false, false);
                }
            }

            public void DrawFooter(Rect rect, ReorderableList list)
            {
                float xMax = rect.xMax;
                float x = xMax - 8f;
                if (list.displayAdd)
                {
                    x -= 25f;
                }
                if (list.displayRemove)
                {
                    x -= 25f;
                }
                rect = new Rect(x, rect.y, xMax - x, rect.height);
                Rect position = new Rect(x + 4f, rect.y - 3f, 25f, 13f);
                Rect rect3 = new Rect(xMax - 29f, rect.y - 3f, 25f, 13f);
                if (Event.current.type == EventType.Repaint)
                {
                    this.footerBackground.Draw(rect, false, false, false, false);
                }
                if (list.displayAdd && GUI.Button(position, (list.onAddDropdownCallback == null) ? this.iconToolbarPlus : this.iconToolbarPlusMore, this.preButton))
                {
                    if (list.onAddDropdownCallback != null)
                    {
                        list.onAddDropdownCallback(position, list);
                    }
                    else if (list.onAddCallback != null)
                    {
                        list.onAddCallback(list);
                    }
                    else
                    {
                        this.DoAddButton(list);
                    }
                    if (list.onChangedCallback != null)
                    {
                        list.onChangedCallback(list);
                    }
                }
                if (list.displayRemove)
                {
                    EditorGUI.BeginDisabledGroup(((list.index < 0) || (list.index >= list.count)) || ((list.onCanRemoveCallback != null) && !list.onCanRemoveCallback(list)));
                    if (GUI.Button(rect3, this.iconToolbarMinus, this.preButton))
                    {
                        if (list.onRemoveCallback == null)
                        {
                            this.DoRemoveButton(list);
                        }
                        else
                        {
                            list.onRemoveCallback(list);
                        }
                        if (list.onChangedCallback != null)
                        {
                            list.onChangedCallback(list);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }

            public void DrawHeader(Rect headerRect, SerializedObject serializedObject, SerializedProperty element, IList elementList)
            {
                EditorGUI.LabelField(headerRect, EditorGUIUtility.TempContent((element == null) ? "IList" : "Serialized Property"));
            }

            public void DrawHeaderBackground(Rect headerRect)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    this.headerBackground.Draw(headerRect, false, false, false, false);
                }
            }

            public void DrawNoneElement(Rect rect, bool draggable)
            {
                EditorGUI.LabelField(rect, EditorGUIUtility.TempContent("List is Empty"));
            }
        }

        public delegate void ElementCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused);

        public delegate float ElementHeightCallbackDelegate(int index);

        public delegate void FooterCallbackDelegate(Rect rect);

        public delegate void HeaderCallbackDelegate(Rect rect);

        public delegate void RemoveCallbackDelegate(ReorderableList list);

        public delegate void ReorderCallbackDelegate(ReorderableList list);

        public delegate void SelectCallbackDelegate(ReorderableList list);
    }
}

