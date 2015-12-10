namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Animations;
    using UnityEngine;

    [CustomEditor(typeof(AvatarMask))]
    internal class AvatarMaskInspector : Editor
    {
        [CompilerGenerated]
        private static Func<char, bool> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<char, bool> <>f__am$cacheF;
        private SerializedProperty m_AnimationType;
        private SerializedProperty m_BodyMask;
        private bool m_BodyMaskFoldout;
        private bool m_CanImport = true;
        private AnimationClipInfoProperties m_ClipInfo;
        private string[] m_HumanTransform;
        private NodeInfo[] m_NodeInfos;
        private Avatar m_RefAvatar;
        private ModelImporter m_RefImporter;
        private bool m_ShowBodyMask = true;
        private SerializedProperty m_TransformMask;
        private bool m_TransformMaskFoldout;
        private string[] m_TransformPaths;
        private static Styles styles = new Styles();

        private void CheckChildren(int index, bool value)
        {
            foreach (int num in this.m_NodeInfos[index].m_ChildIndices)
            {
                if (this.m_NodeInfos[num].m_Enabled)
                {
                    this.m_NodeInfos[num].m_Weight.floatValue = !value ? 0f : 1f;
                }
                this.CheckChildren(num, value);
            }
        }

        private void ComputeShownElements()
        {
            for (int i = 0; i < this.m_NodeInfos.Length; i++)
            {
                if (this.m_NodeInfos[i].m_ParentIndex == -1)
                {
                    this.ComputeShownElements(i, true);
                }
            }
        }

        private void ComputeShownElements(int currentIndex, bool show)
        {
            this.m_NodeInfos[currentIndex].m_Show = show;
            bool flag = show && this.m_NodeInfos[currentIndex].m_Expanded;
            foreach (int num in this.m_NodeInfos[currentIndex].m_ChildIndices)
            {
                this.ComputeShownElements(num, flag);
            }
        }

        protected void CopyFromOtherGUI()
        {
            if (this.clipInfo != null)
            {
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.clipInfo.maskSourceProperty, GUIContent.Temp("Source"), new GUILayoutOption[0]);
                AvatarMask objectReferenceValue = this.clipInfo.maskSourceProperty.objectReferenceValue as AvatarMask;
                if (EditorGUI.EndChangeCheck() && (objectReferenceValue != null))
                {
                    this.UpdateMask(this.clipInfo.maskType);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DeselectAll()
        {
            this.SetAllTransformActive(false);
        }

        private void FillNodeInfos()
        {
            this.m_NodeInfos = new NodeInfo[this.m_TransformMask.arraySize];
            for (int i = 1; i < this.m_NodeInfos.Length; i++)
            {
                <FillNodeInfos>c__AnonStorey7E storeye = new <FillNodeInfos>c__AnonStorey7E();
                this.m_NodeInfos[i].m_Path = this.m_TransformMask.GetArrayElementAtIndex(i).FindPropertyRelative("m_Path");
                this.m_NodeInfos[i].m_Weight = this.m_TransformMask.GetArrayElementAtIndex(i).FindPropertyRelative("m_Weight");
                storeye.fullPath = this.m_NodeInfos[i].m_Path.stringValue;
                if (this.humanTransforms != null)
                {
                    this.m_NodeInfos[i].m_Enabled = ArrayUtility.FindIndex<string>(this.humanTransforms, new Predicate<string>(storeye.<>m__13B)) == -1;
                }
                else
                {
                    this.m_NodeInfos[i].m_Enabled = true;
                }
                this.m_NodeInfos[i].m_Expanded = true;
                this.m_NodeInfos[i].m_ParentIndex = -1;
                this.m_NodeInfos[i].m_ChildIndices = new List<int>();
                if (<>f__am$cacheE == null)
                {
                    <>f__am$cacheE = f => f == '/';
                }
                this.m_NodeInfos[i].m_Depth = (i != 0) ? storeye.fullPath.Count<char>(<>f__am$cacheE) : 0;
                string str = string.Empty;
                int length = storeye.fullPath.LastIndexOf('/');
                if (length > 0)
                {
                    str = storeye.fullPath.Substring(0, length);
                }
                length = (length != -1) ? (length + 1) : 0;
                this.m_NodeInfos[i].m_Name = storeye.fullPath.Substring(length);
                int arraySize = this.m_TransformMask.arraySize;
                for (int j = 0; j < arraySize; j++)
                {
                    string stringValue = this.m_TransformMask.GetArrayElementAtIndex(j).FindPropertyRelative("m_Path").stringValue;
                    if ((str != string.Empty) && (stringValue == str))
                    {
                        this.m_NodeInfos[i].m_ParentIndex = j;
                    }
                    if (stringValue.StartsWith(storeye.fullPath))
                    {
                        if (<>f__am$cacheF == null)
                        {
                            <>f__am$cacheF = f => f == '/';
                        }
                        if (stringValue.Count<char>(<>f__am$cacheF) == (this.m_NodeInfos[i].m_Depth + 1))
                        {
                            this.m_NodeInfos[i].m_ChildIndices.Add(j);
                        }
                    }
                }
            }
        }

        private void ImportAvatarReference()
        {
            EditorGUI.BeginChangeCheck();
            this.m_RefAvatar = EditorGUILayout.ObjectField("Use skeleton from", this.m_RefAvatar, typeof(Avatar), true, new GUILayoutOption[0]) as Avatar;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_RefImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.m_RefAvatar)) as ModelImporter;
            }
            if ((this.m_RefImporter != null) && GUILayout.Button("Import skeleton", new GUILayoutOption[0]))
            {
                AvatarMaskUtility.UpdateTransformMask(this.m_TransformMask, this.m_RefImporter.transformPaths, null);
            }
        }

        private void InitializeSerializedProperties()
        {
            if (this.clipInfo != null)
            {
                this.m_BodyMask = this.clipInfo.bodyMaskProperty;
                this.m_TransformMask = this.clipInfo.transformMaskProperty;
            }
            else
            {
                this.m_BodyMask = base.serializedObject.FindProperty("m_Mask");
                this.m_TransformMask = base.serializedObject.FindProperty("m_Elements");
            }
        }

        public bool IsMaskUpToDate()
        {
            if (this.clipInfo == null)
            {
                return false;
            }
            if (this.m_TransformMask.arraySize != this.m_TransformPaths.Length)
            {
                return false;
            }
            for (int i = 0; i < this.m_TransformPaths.Length; i++)
            {
                if (this.m_TransformMask.GetArrayElementAtIndex(i).FindPropertyRelative("m_Path").stringValue != this.m_TransformPaths[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void OnBodyInspectorGUI()
        {
            if (this.m_ShowBodyMask)
            {
                bool changed = GUI.changed;
                this.m_BodyMaskFoldout = EditorGUILayout.Foldout(this.m_BodyMaskFoldout, styles.BodyMask);
                GUI.changed = changed;
                if (this.m_BodyMaskFoldout)
                {
                    BodyMaskEditor.Show(this.m_BodyMask, 13);
                }
            }
        }

        private void OnEnable()
        {
            this.InitializeSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            Profiler.BeginSample("AvatarMaskInspector.OnInspectorGUI()");
            if (this.clipInfo == null)
            {
                base.serializedObject.Update();
            }
            bool flag = false;
            if (this.clipInfo != null)
            {
                EditorGUI.BeginChangeCheck();
                int maskType = (int) this.clipInfo.maskType;
                EditorGUI.showMixedValue = this.clipInfo.maskTypeProperty.hasMultipleDifferentValues;
                maskType = EditorGUILayout.Popup(styles.MaskDefinition, maskType, styles.MaskDefinitionOpt, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    this.clipInfo.maskType = (ClipAnimationMaskType) maskType;
                    this.UpdateMask(this.clipInfo.maskType);
                }
                flag = this.clipInfo.maskType == ClipAnimationMaskType.CopyFromOther;
            }
            if (flag)
            {
                this.CopyFromOtherGUI();
            }
            bool enabled = GUI.enabled;
            GUI.enabled = !flag;
            EditorGUI.BeginChangeCheck();
            this.OnBodyInspectorGUI();
            this.OnTransformInspectorGUI();
            if ((this.clipInfo != null) && EditorGUI.EndChangeCheck())
            {
                AvatarMask target = this.target as AvatarMask;
                this.clipInfo.MaskFromClip(target);
            }
            GUI.enabled = enabled;
            if (this.clipInfo == null)
            {
                base.serializedObject.ApplyModifiedProperties();
            }
            Profiler.EndSample();
        }

        public void OnTransformInspectorGUI()
        {
            float xmin = 0f;
            float ymin = 0f;
            float a = 0f;
            float ymax = 0f;
            bool changed = GUI.changed;
            this.m_TransformMaskFoldout = EditorGUILayout.Foldout(this.m_TransformMaskFoldout, styles.TransformMask);
            GUI.changed = changed;
            if (this.m_TransformMaskFoldout)
            {
                if (this.canImport)
                {
                    this.ImportAvatarReference();
                }
                if ((this.m_NodeInfos == null) || (this.m_TransformMask.arraySize != this.m_NodeInfos.Length))
                {
                    this.FillNodeInfos();
                }
                this.ComputeShownElements();
                GUILayout.Space(1f);
                int indentLevel = EditorGUI.indentLevel;
                int arraySize = this.m_TransformMask.arraySize;
                for (int i = 1; i < arraySize; i++)
                {
                    if (this.m_NodeInfos[i].m_Show)
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        EditorGUI.indentLevel = this.m_NodeInfos[i].m_Depth + 1;
                        EditorGUI.BeginChangeCheck();
                        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                        Rect position = GUILayoutUtility.GetRect((float) 15f, (float) 15f, options);
                        GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                        GUILayoutUtility.GetRect((float) 10f, (float) 15f, optionArray2);
                        position.x += 15f;
                        bool enabled = GUI.enabled;
                        GUI.enabled = this.m_NodeInfos[i].m_Enabled;
                        bool flag3 = Event.current.button == 1;
                        bool flag4 = this.m_NodeInfos[i].m_Weight.floatValue > 0f;
                        flag4 = GUI.Toggle(position, flag4, string.Empty);
                        GUI.enabled = enabled;
                        if (EditorGUI.EndChangeCheck())
                        {
                            this.m_NodeInfos[i].m_Weight.floatValue = !flag4 ? 0f : 1f;
                            if (!flag3)
                            {
                                this.CheckChildren(i, flag4);
                            }
                        }
                        if (this.m_NodeInfos[i].m_ChildIndices.Count > 0)
                        {
                            this.m_NodeInfos[i].m_Expanded = EditorGUILayout.Foldout(this.m_NodeInfos[i].m_Expanded, this.m_NodeInfos[i].m_Name);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(this.m_NodeInfos[i].m_Name, new GUILayoutOption[0]);
                        }
                        if (i == 1)
                        {
                            ymin = position.yMin;
                            xmin = position.xMin;
                        }
                        else if (i == (arraySize - 1))
                        {
                            ymax = position.yMax;
                        }
                        a = Mathf.Max(a, GUILayoutUtility.GetLastRect().xMax);
                        GUILayout.EndHorizontal();
                    }
                }
                EditorGUI.indentLevel = indentLevel;
            }
            Rect rect2 = Rect.MinMaxRect(xmin, ymin, a, ymax);
            if (((Event.current != null) && (Event.current.type == EventType.MouseUp)) && ((Event.current.button == 1) && rect2.Contains(Event.current.mousePosition)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Select all"), false, new GenericMenu.MenuFunction(this.SelectAll));
                menu.AddItem(new GUIContent("Deselect all"), false, new GenericMenu.MenuFunction(this.DeselectAll));
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        private void SelectAll()
        {
            this.SetAllTransformActive(true);
        }

        private void SetAllTransformActive(bool active)
        {
            for (int i = 0; i < this.m_NodeInfos.Length; i++)
            {
                if (this.m_NodeInfos[i].m_Enabled)
                {
                    this.m_NodeInfos[i].m_Weight.floatValue = !active ? 0f : 1f;
                }
            }
        }

        private void UpdateMask(ClipAnimationMaskType maskType)
        {
            if (this.clipInfo != null)
            {
                if (maskType == ClipAnimationMaskType.CreateFromThisModel)
                {
                    ModelImporter targetObject = this.clipInfo.maskTypeProperty.serializedObject.targetObject as ModelImporter;
                    AvatarMaskUtility.UpdateTransformMask(this.m_TransformMask, targetObject.transformPaths, this.humanTransforms);
                    this.FillNodeInfos();
                }
                else if (maskType == ClipAnimationMaskType.CopyFromOther)
                {
                    AvatarMask objectReferenceValue = this.clipInfo.maskSourceProperty.objectReferenceValue as AvatarMask;
                    if (objectReferenceValue != null)
                    {
                        AvatarMask mask = this.target as AvatarMask;
                        mask.Copy(objectReferenceValue);
                        if (this.humanTransforms != null)
                        {
                            AvatarMaskUtility.SetActiveHumanTransforms(mask, this.humanTransforms);
                        }
                        this.clipInfo.MaskToClip(mask);
                        this.FillNodeInfos();
                    }
                }
                AvatarMask target = this.target as AvatarMask;
                this.clipInfo.MaskFromClip(target);
            }
        }

        private ModelImporterAnimationType animationType
        {
            get
            {
                if (this.m_AnimationType != null)
                {
                    return (ModelImporterAnimationType) this.m_AnimationType.intValue;
                }
                return ModelImporterAnimationType.None;
            }
        }

        public bool canImport
        {
            get
            {
                return this.m_CanImport;
            }
            set
            {
                this.m_CanImport = value;
            }
        }

        public AnimationClipInfoProperties clipInfo
        {
            get
            {
                return this.m_ClipInfo;
            }
            set
            {
                this.m_ClipInfo = value;
                if (this.m_ClipInfo != null)
                {
                    this.m_ClipInfo.MaskFromClip(this.target as AvatarMask);
                    SerializedObject serializedObject = this.m_ClipInfo.maskTypeProperty.serializedObject;
                    this.m_AnimationType = serializedObject.FindProperty("m_AnimationType");
                    ModelImporter targetObject = serializedObject.targetObject as ModelImporter;
                    this.m_TransformPaths = targetObject.transformPaths;
                }
                else
                {
                    this.m_TransformPaths = null;
                    this.m_AnimationType = null;
                }
                this.InitializeSerializedProperties();
            }
        }

        public string[] humanTransforms
        {
            get
            {
                if ((this.animationType == ModelImporterAnimationType.Human) && (this.clipInfo != null))
                {
                    if (this.m_HumanTransform == null)
                    {
                        SerializedObject serializedObject = this.clipInfo.maskTypeProperty.serializedObject;
                        ModelImporter targetObject = serializedObject.targetObject as ModelImporter;
                        this.m_HumanTransform = AvatarMaskUtility.GetAvatarHumanTransform(serializedObject, targetObject.transformPaths);
                    }
                }
                else
                {
                    this.m_HumanTransform = null;
                }
                return this.m_HumanTransform;
            }
        }

        public bool showBody
        {
            get
            {
                return this.m_ShowBodyMask;
            }
            set
            {
                this.m_ShowBodyMask = value;
            }
        }

        [CompilerGenerated]
        private sealed class <FillNodeInfos>c__AnonStorey7E
        {
            internal string fullPath;

            internal bool <>m__13B(string s)
            {
                return (this.fullPath == s);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NodeInfo
        {
            public bool m_Expanded;
            public bool m_Show;
            public bool m_Enabled;
            public int m_ParentIndex;
            public List<int> m_ChildIndices;
            public int m_Depth;
            public SerializedProperty m_Path;
            public SerializedProperty m_Weight;
            public string m_Name;
        }

        private class Styles
        {
            public GUIContent BodyMask = EditorGUIUtility.TextContent("Humanoid|Define which body part are active. Also define which animation curves will be imported for an Animation Clip.");
            public GUIContent MaskDefinition = EditorGUIUtility.TextContent("Definition|Choose between Create From This Model, Copy From Other Avatar. The first one create a Mask for this file and the second one use a Mask from another file to import animation.");
            public GUIContent[] MaskDefinitionOpt = new GUIContent[] { EditorGUIUtility.TextContent("Create From This Model|Create a Mask based on the model from this file. For Humanoid rig all the human transform are always imported and converted to muscle curve, thus they cannot be unchecked."), EditorGUIUtility.TextContent("Copy From Other Mask|Copy a Mask from another file to import animation clip.") };
            public GUIContent TransformMask = EditorGUIUtility.TextContent("Transform|Define which transform are active. Also define which animation curves will be imported for an Animation Clip.");
        }
    }
}

