namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class PropertyHandler
    {
        public List<ContextMenuItemAttribute> contextMenuItems;
        private List<DecoratorDrawer> m_DecoratorDrawers;
        private PropertyDrawer m_PropertyDrawer;
        public string tooltip;

        public void AddMenuItems(SerializedProperty property, GenericMenu menu)
        {
            <AddMenuItems>c__AnonStoreyA6 ya = new <AddMenuItems>c__AnonStoreyA6 {
                property = property,
                <>f__this = this
            };
            if (this.contextMenuItems != null)
            {
                Type type = ya.property.serializedObject.targetObject.GetType();
                foreach (ContextMenuItemAttribute attribute in this.contextMenuItems)
                {
                    <AddMenuItems>c__AnonStoreyA7 ya2 = new <AddMenuItems>c__AnonStoreyA7 {
                        <>f__ref$166 = ya,
                        <>f__this = this,
                        method = type.GetMethod(attribute.function, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    };
                    if (ya2.method != null)
                    {
                        menu.AddItem(new GUIContent(attribute.name), false, new GenericMenu.MenuFunction(ya2.<>m__1EE));
                    }
                }
            }
        }

        public void CallMenuCallback(object[] targets, MethodInfo method)
        {
            foreach (object obj2 in targets)
            {
                method.Invoke(obj2, new object[0]);
            }
        }

        public float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            float num = 0f;
            if ((this.m_DecoratorDrawers != null) && !this.isCurrentlyNested)
            {
                foreach (DecoratorDrawer drawer in this.m_DecoratorDrawers)
                {
                    num += drawer.GetHeight();
                }
            }
            if (this.propertyDrawer != null)
            {
                if (label == null)
                {
                }
                return (num + this.propertyDrawer.GetPropertyHeightSafe(property.Copy(), EditorGUIUtility.TempContent(property.displayName)));
            }
            if (!includeChildren)
            {
                return (num + EditorGUI.GetSinglePropertyHeight(property, label));
            }
            property = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();
            num += EditorGUI.GetSinglePropertyHeight(property, label);
            bool enterChildren = property.isExpanded && EditorGUI.HasVisibleChildFields(property);
            while (property.NextVisible(enterChildren) && !SerializedProperty.EqualContents(property, endProperty))
            {
                num += ScriptAttributeUtility.GetHandler(property).GetHeight(property, EditorGUIUtility.TempContent(property.displayName), true);
                enterChildren = false;
                num += 2f;
            }
            return num;
        }

        public void HandleAttribute(PropertyAttribute attribute, FieldInfo field, Type propertyType)
        {
            if (attribute is TooltipAttribute)
            {
                this.tooltip = (attribute as TooltipAttribute).tooltip;
            }
            else if (attribute is ContextMenuItemAttribute)
            {
                if (!propertyType.IsArrayOrList())
                {
                    if (this.contextMenuItems == null)
                    {
                        this.contextMenuItems = new List<ContextMenuItemAttribute>();
                    }
                    this.contextMenuItems.Add(attribute as ContextMenuItemAttribute);
                }
            }
            else
            {
                this.HandleDrawnType(attribute.GetType(), propertyType, field, attribute);
            }
        }

        public void HandleDrawnType(Type drawnType, Type propertyType, FieldInfo field, PropertyAttribute attribute)
        {
            Type drawerTypeForType = ScriptAttributeUtility.GetDrawerTypeForType(drawnType);
            if (drawerTypeForType != null)
            {
                if (typeof(PropertyDrawer).IsAssignableFrom(drawerTypeForType))
                {
                    if ((propertyType == null) || !propertyType.IsArrayOrList())
                    {
                        this.m_PropertyDrawer = (PropertyDrawer) Activator.CreateInstance(drawerTypeForType);
                        this.m_PropertyDrawer.m_FieldInfo = field;
                        this.m_PropertyDrawer.m_Attribute = attribute;
                    }
                }
                else if (typeof(DecoratorDrawer).IsAssignableFrom(drawerTypeForType) && (((field == null) || !field.FieldType.IsArrayOrList()) || propertyType.IsArrayOrList()))
                {
                    DecoratorDrawer item = (DecoratorDrawer) Activator.CreateInstance(drawerTypeForType);
                    item.m_Attribute = attribute;
                    if (this.m_DecoratorDrawers == null)
                    {
                        this.m_DecoratorDrawers = new List<DecoratorDrawer>();
                    }
                    this.m_DecoratorDrawers.Add(item);
                }
            }
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            float labelWidth;
            float fieldWidth;
            float height = position.height;
            position.height = 0f;
            if ((this.m_DecoratorDrawers != null) && !this.isCurrentlyNested)
            {
                foreach (DecoratorDrawer drawer in this.m_DecoratorDrawers)
                {
                    position.height = drawer.GetHeight();
                    labelWidth = EditorGUIUtility.labelWidth;
                    fieldWidth = EditorGUIUtility.fieldWidth;
                    drawer.OnGUI(position);
                    EditorGUIUtility.labelWidth = labelWidth;
                    EditorGUIUtility.fieldWidth = fieldWidth;
                    position.y += position.height;
                    height -= position.height;
                }
            }
            position.height = height;
            if (this.propertyDrawer != null)
            {
                labelWidth = EditorGUIUtility.labelWidth;
                fieldWidth = EditorGUIUtility.fieldWidth;
                if (label == null)
                {
                }
                this.propertyDrawer.OnGUISafe(position, property.Copy(), EditorGUIUtility.TempContent(property.displayName));
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUIUtility.fieldWidth = fieldWidth;
                return false;
            }
            if (!includeChildren)
            {
                return EditorGUI.DefaultPropertyField(position, property, label);
            }
            Vector2 iconSize = EditorGUIUtility.GetIconSize();
            bool enabled = GUI.enabled;
            int indentLevel = EditorGUI.indentLevel;
            int num5 = indentLevel - property.depth;
            SerializedProperty property2 = property.Copy();
            SerializedProperty endProperty = property2.GetEndProperty();
            position.height = EditorGUI.GetSinglePropertyHeight(property2, label);
            EditorGUI.indentLevel = property2.depth + num5;
            bool enterChildren = EditorGUI.DefaultPropertyField(position, property2, label) && EditorGUI.HasVisibleChildFields(property2);
            position.y += position.height + 2f;
            while (property2.NextVisible(enterChildren) && !SerializedProperty.EqualContents(property2, endProperty))
            {
                EditorGUI.indentLevel = property2.depth + num5;
                position.height = EditorGUI.GetPropertyHeight(property2, null, false);
                EditorGUI.BeginChangeCheck();
                enterChildren = ScriptAttributeUtility.GetHandler(property2).OnGUI(position, property2, null, false) && EditorGUI.HasVisibleChildFields(property2);
                if (EditorGUI.EndChangeCheck())
                {
                    break;
                }
                position.y += position.height + 2f;
            }
            GUI.enabled = enabled;
            EditorGUIUtility.SetIconSize(iconSize);
            EditorGUI.indentLevel = indentLevel;
            return false;
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
        {
            Rect toggleRect;
            if (((property.propertyType == SerializedPropertyType.Boolean) && (this.propertyDrawer == null)) && ((this.m_DecoratorDrawers == null) || (this.m_DecoratorDrawers.Count == 0)))
            {
                toggleRect = EditorGUILayout.GetToggleRect(true, options);
            }
            else
            {
                toggleRect = EditorGUILayout.GetControlRect(EditorGUI.LabelHasContent(label), this.GetHeight(property, label, includeChildren), options);
            }
            EditorGUILayout.s_LastRect = toggleRect;
            return this.OnGUI(toggleRect, property, label, includeChildren);
        }

        public bool empty
        {
            get
            {
                return ((((this.m_DecoratorDrawers == null) && (this.tooltip == null)) && (this.propertyDrawer == null)) && (this.contextMenuItems == null));
            }
        }

        public bool hasPropertyDrawer
        {
            get
            {
                return (this.propertyDrawer != null);
            }
        }

        private bool isCurrentlyNested
        {
            get
            {
                return (((this.m_PropertyDrawer != null) && ScriptAttributeUtility.s_DrawerStack.Any<PropertyDrawer>()) && (this.m_PropertyDrawer == ScriptAttributeUtility.s_DrawerStack.Peek()));
            }
        }

        private PropertyDrawer propertyDrawer
        {
            get
            {
                return (!this.isCurrentlyNested ? this.m_PropertyDrawer : null);
            }
        }

        [CompilerGenerated]
        private sealed class <AddMenuItems>c__AnonStoreyA6
        {
            internal PropertyHandler <>f__this;
            internal SerializedProperty property;
        }

        [CompilerGenerated]
        private sealed class <AddMenuItems>c__AnonStoreyA7
        {
            internal PropertyHandler.<AddMenuItems>c__AnonStoreyA6 <>f__ref$166;
            internal PropertyHandler <>f__this;
            internal MethodInfo method;

            internal void <>m__1EE()
            {
                this.<>f__this.CallMenuCallback(this.<>f__ref$166.property.serializedObject.targetObjects, this.method);
            }
        }
    }
}

