namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(TagManager))]
    internal class TagManagerInspector : Editor
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1E;
        protected bool m_IsEditable;
        protected SerializedProperty m_Layers;
        private ReorderableList m_LayersList;
        protected SerializedProperty m_SortingLayers;
        private ReorderableList m_SortLayersList;
        protected SerializedProperty m_Tags;
        private ReorderableList m_TagsList;

        private void AddToSortLayerList(ReorderableList list)
        {
            base.serializedObject.ApplyModifiedProperties();
            InternalEditorUtility.AddSortingLayer();
            base.serializedObject.Update();
            list.index = list.serializedProperty.arraySize - 1;
        }

        private void AddToTagsList(ReorderableList list)
        {
            int arraySize = this.m_Tags.arraySize;
            this.m_Tags.InsertArrayElementAtIndex(arraySize);
            this.m_Tags.GetArrayElementAtIndex(arraySize).stringValue = "New Tag";
            list.index = list.serializedProperty.arraySize - 1;
        }

        private bool CanEditSortLayerEntry(int index)
        {
            return (((index >= 0) && (index < InternalEditorUtility.GetSortingLayerCount())) && !InternalEditorUtility.IsSortingLayerDefault(index));
        }

        private bool CanRemoveSortLayerEntry(ReorderableList list)
        {
            return this.CanEditSortLayerEntry(list.index);
        }

        private void DrawLayerListElement(Rect rect, int index, bool selected, bool focused)
        {
            string str2;
            rect.height -= 2f;
            rect.xMin -= 20f;
            bool flag = index >= 8;
            bool enabled = GUI.enabled;
            GUI.enabled = this.m_IsEditable && flag;
            string stringValue = this.m_Layers.GetArrayElementAtIndex(index).stringValue;
            if (flag)
            {
                str2 = EditorGUI.TextField(rect, " User Layer " + index, stringValue);
            }
            else
            {
                str2 = EditorGUI.TextField(rect, " Builtin Layer " + index, stringValue);
            }
            if (str2 != stringValue)
            {
                this.m_Layers.GetArrayElementAtIndex(index).stringValue = str2;
            }
            GUI.enabled = enabled;
        }

        private void DrawSortLayerListElement(Rect rect, int index, bool selected, bool focused)
        {
            rect.height -= 2f;
            rect.xMin -= 20f;
            bool enabled = GUI.enabled;
            GUI.enabled = this.m_IsEditable && this.CanEditSortLayerEntry(index);
            string sortingLayerName = InternalEditorUtility.GetSortingLayerName(index);
            string name = EditorGUI.TextField(rect, " Layer ", sortingLayerName);
            if (name != sortingLayerName)
            {
                base.serializedObject.ApplyModifiedProperties();
                InternalEditorUtility.SetSortingLayerName(index, name);
                base.serializedObject.Update();
            }
            GUI.enabled = enabled;
        }

        private void DrawTagListElement(Rect rect, int index, bool selected, bool focused)
        {
            rect.height -= 2f;
            rect.xMin -= 20f;
            bool enabled = GUI.enabled;
            GUI.enabled = this.m_IsEditable;
            string stringValue = this.m_Tags.GetArrayElementAtIndex(index).stringValue;
            string str2 = EditorGUI.TextField(rect, " Tag " + index, stringValue);
            if (str2 != stringValue)
            {
                this.m_Tags.GetArrayElementAtIndex(index).stringValue = str2;
            }
            GUI.enabled = enabled;
        }

        public virtual void OnEnable()
        {
            this.m_Tags = base.serializedObject.FindProperty("tags");
            if (this.m_TagsList == null)
            {
                this.m_TagsList = new ReorderableList(base.serializedObject, this.m_Tags, false, false, true, true);
                this.m_TagsList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddToTagsList);
                this.m_TagsList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveFromTagsList);
                this.m_TagsList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawTagListElement);
                this.m_TagsList.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
                this.m_TagsList.headerHeight = 3f;
            }
            this.m_SortingLayers = base.serializedObject.FindProperty("m_SortingLayers");
            if (this.m_SortLayersList == null)
            {
                this.m_SortLayersList = new ReorderableList(base.serializedObject, this.m_SortingLayers, true, false, true, true);
                this.m_SortLayersList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.ReorderSortLayerList);
                this.m_SortLayersList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddToSortLayerList);
                this.m_SortLayersList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveFromSortLayerList);
                this.m_SortLayersList.onCanRemoveCallback = new ReorderableList.CanRemoveCallbackDelegate(this.CanRemoveSortLayerEntry);
                this.m_SortLayersList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawSortLayerListElement);
                this.m_SortLayersList.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
                this.m_SortLayersList.headerHeight = 3f;
            }
            this.m_Layers = base.serializedObject.FindProperty("layers");
            if (this.m_LayersList == null)
            {
                this.m_LayersList = new ReorderableList(base.serializedObject, this.m_Layers, false, false, false, false);
                this.m_LayersList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawLayerListElement);
                this.m_LayersList.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
                this.m_LayersList.headerHeight = 3f;
            }
            this.m_Tags.isExpanded = false;
            this.m_SortingLayers.isExpanded = false;
            this.m_Layers.isExpanded = false;
            string defaultExpandedFoldout = this.tagManager.m_DefaultExpandedFoldout;
            if (defaultExpandedFoldout != null)
            {
                int num;
                if (<>f__switch$map1E == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
                    dictionary.Add("Tags", 0);
                    dictionary.Add("SortingLayers", 1);
                    dictionary.Add("Layers", 2);
                    <>f__switch$map1E = dictionary;
                }
                if (<>f__switch$map1E.TryGetValue(defaultExpandedFoldout, out num))
                {
                    switch (num)
                    {
                        case 0:
                            this.m_Tags.isExpanded = true;
                            return;

                        case 1:
                            this.m_SortingLayers.isExpanded = true;
                            return;

                        case 2:
                            this.m_Layers.isExpanded = true;
                            return;
                    }
                }
            }
            this.m_Layers.isExpanded = true;
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.m_IsEditable = AssetDatabase.IsOpenForEdit("ProjectSettings/TagManager.asset");
            bool enabled = GUI.enabled;
            GUI.enabled = this.m_IsEditable;
            this.m_Tags.isExpanded = EditorGUILayout.Foldout(this.m_Tags.isExpanded, "Tags");
            if (this.m_Tags.isExpanded)
            {
                EditorGUI.indentLevel++;
                this.m_TagsList.DoLayoutList();
                EditorGUI.indentLevel--;
            }
            this.m_SortingLayers.isExpanded = EditorGUILayout.Foldout(this.m_SortingLayers.isExpanded, "Sorting Layers");
            if (this.m_SortingLayers.isExpanded)
            {
                EditorGUI.indentLevel++;
                this.m_SortLayersList.DoLayoutList();
                EditorGUI.indentLevel--;
            }
            this.m_Layers.isExpanded = EditorGUILayout.Foldout(this.m_Layers.isExpanded, "Layers");
            if (this.m_Layers.isExpanded)
            {
                EditorGUI.indentLevel++;
                this.m_LayersList.DoLayoutList();
                EditorGUI.indentLevel--;
            }
            GUI.enabled = enabled;
            base.serializedObject.ApplyModifiedProperties();
        }

        private void RemoveFromSortLayerList(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            base.serializedObject.ApplyModifiedProperties();
            base.serializedObject.Update();
            InternalEditorUtility.UpdateSortingLayersOrder();
        }

        private void RemoveFromTagsList(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        public void ReorderSortLayerList(ReorderableList list)
        {
            InternalEditorUtility.UpdateSortingLayersOrder();
        }

        public TagManager tagManager
        {
            get
            {
                return (this.target as TagManager);
            }
        }

        internal override string targetTitle
        {
            get
            {
                return "Tags & Layers";
            }
        }
    }
}

