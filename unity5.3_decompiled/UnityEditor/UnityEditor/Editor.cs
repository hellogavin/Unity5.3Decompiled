namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class Editor : ScriptableObject, IPreviewable
    {
        internal const float kLineHeight = 16f;
        private const float kImageSectionWidth = 44f;
        private Object[] m_Targets;
        private int m_IsDirty;
        private int m_ReferenceTargetIndex;
        private PropertyHandlerCache m_PropertyHandlerCache = new PropertyHandlerCache();
        private IPreviewable m_DummyPreview;
        internal SerializedObject m_SerializedObject;
        private OptimizedGUIBlock m_OptimizedBlock;
        internal InspectorMode m_InspectorMode;
        internal bool hideInspector;
        internal static bool m_AllowMultiObjectAccess = true;
        private static Styles s_Styles;
        internal bool canEditMultipleObjects
        {
            get
            {
                return (base.GetType().GetCustomAttributes(typeof(CanEditMultipleObjects), false).Length > 0);
            }
        }
        [ExcludeFromDocs]
        public static Editor CreateEditor(Object targetObject)
        {
            Type editorType = null;
            return CreateEditor(targetObject, editorType);
        }

        public static Editor CreateEditor(Object targetObject, [DefaultValue("null")] Type editorType)
        {
            Object[] targetObjects = new Object[] { targetObject };
            return CreateEditor(targetObjects, editorType);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Editor CreateEditor(Object[] targetObjects, [DefaultValue("null")] Type editorType);
        [ExcludeFromDocs]
        public static Editor CreateEditor(Object[] targetObjects)
        {
            Type editorType = null;
            return CreateEditor(targetObjects, editorType);
        }

        public static void CreateCachedEditor(Object targetObject, Type editorType, ref Editor previousEditor)
        {
            if (((previousEditor == null) || (previousEditor.m_Targets.Length != 1)) || (previousEditor.m_Targets[0] != targetObject))
            {
                if (previousEditor != null)
                {
                    Object.DestroyImmediate(previousEditor);
                }
                previousEditor = CreateEditor(targetObject, editorType);
            }
        }

        public static void CreateCachedEditor(Object[] targetObjects, Type editorType, ref Editor previousEditor)
        {
            if ((previousEditor == null) || !ArrayUtility.ArrayEquals<Object>(previousEditor.m_Targets, targetObjects))
            {
                if (previousEditor != null)
                {
                    Object.DestroyImmediate(previousEditor);
                }
                previousEditor = CreateEditor(targetObjects, editorType);
            }
        }

        public Object target
        {
            get
            {
                return this.m_Targets[this.referenceTargetIndex];
            }
            set
            {
                throw new InvalidOperationException("You can't set the target on an editor.");
            }
        }
        public Object[] targets
        {
            get
            {
                if (!m_AllowMultiObjectAccess)
                {
                    Debug.LogError("The targets array should not be used inside OnSceneGUI or OnPreviewGUI. Use the single target property instead.");
                }
                return this.m_Targets;
            }
        }
        internal virtual int referenceTargetIndex
        {
            get
            {
                return Mathf.Clamp(this.m_ReferenceTargetIndex, 0, this.m_Targets.Length - 1);
            }
            set
            {
                this.m_ReferenceTargetIndex = (Math.Abs((int) (value * this.m_Targets.Length)) + value) % this.m_Targets.Length;
            }
        }
        internal virtual string targetTitle
        {
            get
            {
                if ((this.m_Targets.Length == 1) || !m_AllowMultiObjectAccess)
                {
                    return this.target.name;
                }
                object[] objArray1 = new object[] { this.m_Targets.Length, " ", ObjectNames.NicifyVariableName(ObjectNames.GetClassName(this.target)), "s" };
                return string.Concat(objArray1);
            }
        }
        public SerializedObject serializedObject
        {
            get
            {
                if (!m_AllowMultiObjectAccess)
                {
                    Debug.LogError("The serializedObject should not be used inside OnSceneGUI or OnPreviewGUI. Use the target property directly instead.");
                }
                return this.GetSerializedObjectInternal();
            }
        }
        internal virtual SerializedObject GetSerializedObjectInternal()
        {
            if (this.m_SerializedObject == null)
            {
                this.m_SerializedObject = new SerializedObject(this.targets);
            }
            return this.m_SerializedObject;
        }

        private void CleanupPropertyEditor()
        {
            if (this.m_OptimizedBlock != null)
            {
                this.m_OptimizedBlock.Dispose();
                this.m_OptimizedBlock = null;
            }
            if (this.m_SerializedObject != null)
            {
                this.m_SerializedObject.Dispose();
                this.m_SerializedObject = null;
            }
        }

        private void OnDisableINTERNAL()
        {
            this.CleanupPropertyEditor();
        }

        internal virtual void OnForceReloadInspector()
        {
            if (this.m_SerializedObject != null)
            {
                this.m_SerializedObject.SetIsDifferentCacheDirty();
            }
        }

        internal bool GetOptimizedGUIBlockImplementation(bool isDirty, bool isVisible, out OptimizedGUIBlock block, out float height)
        {
            if (this.m_OptimizedBlock == null)
            {
                this.m_OptimizedBlock = new OptimizedGUIBlock();
            }
            block = this.m_OptimizedBlock;
            if (!isVisible)
            {
                height = 0f;
                return true;
            }
            if (this.m_SerializedObject == null)
            {
                this.m_SerializedObject = new SerializedObject(this.targets);
            }
            else
            {
                this.m_SerializedObject.Update();
            }
            this.m_SerializedObject.inspectorMode = this.m_InspectorMode;
            SerializedProperty iterator = this.m_SerializedObject.GetIterator();
            height = 2f;
            for (bool flag = true; iterator.NextVisible(flag); flag = false)
            {
                height += EditorGUI.GetPropertyHeight(iterator, null, true) + 2f;
            }
            if (height == 2f)
            {
                height = 0f;
            }
            return true;
        }

        internal bool OptimizedInspectorGUIImplementation(Rect contentRect)
        {
            SerializedProperty iterator = this.m_SerializedObject.GetIterator();
            bool enterChildren = true;
            bool enabled = GUI.enabled;
            contentRect.xMin += 14f;
            contentRect.xMax -= 4f;
            contentRect.y += 2f;
            while (iterator.NextVisible(enterChildren))
            {
                contentRect.height = EditorGUI.GetPropertyHeight(iterator, null, false);
                EditorGUI.indentLevel = iterator.depth;
                using (new EditorGUI.DisabledGroupScope((this.m_InspectorMode == InspectorMode.Normal) && ("m_Script" == iterator.propertyPath)))
                {
                    enterChildren = EditorGUI.PropertyField(contentRect, iterator);
                }
                contentRect.y += contentRect.height + 2f;
            }
            GUI.enabled = enabled;
            return this.m_SerializedObject.ApplyModifiedProperties();
        }

        protected internal static void DrawPropertiesExcluding(SerializedObject obj, params string[] propertyToExclude)
        {
            SerializedProperty iterator = obj.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (!propertyToExclude.Contains<string>(iterator.name))
                {
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                }
            }
        }

        public bool DrawDefaultInspector()
        {
            return this.DoDrawDefaultInspector();
        }

        public virtual void OnInspectorGUI()
        {
            this.DrawDefaultInspector();
        }

        public virtual bool RequiresConstantRepaint()
        {
            return false;
        }

        internal void InternalSetTargets(Object[] t)
        {
            this.m_Targets = t;
        }

        internal void InternalSetHidden(bool hidden)
        {
            this.hideInspector = hidden;
        }

        internal virtual bool GetOptimizedGUIBlock(bool isDirty, bool isVisible, out OptimizedGUIBlock block, out float height)
        {
            block = null;
            height = -1f;
            return false;
        }

        internal virtual bool OnOptimizedInspectorGUI(Rect contentRect)
        {
            Debug.LogError("Not supported");
            return false;
        }

        internal bool isInspectorDirty
        {
            get
            {
                return (this.m_IsDirty != 0);
            }
            set
            {
                this.m_IsDirty = !value ? 0 : 1;
            }
        }
        public void Repaint()
        {
            InspectorWindow.RepaintAllInspectors();
        }

        public virtual bool HasPreviewGUI()
        {
            return this.preview.HasPreviewGUI();
        }

        public virtual GUIContent GetPreviewTitle()
        {
            return this.preview.GetPreviewTitle();
        }

        public virtual Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return null;
        }

        public virtual void OnPreviewGUI(Rect r, GUIStyle background)
        {
            this.preview.OnPreviewGUI(r, background);
        }

        public virtual void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            this.OnPreviewGUI(r, background);
        }

        public virtual void OnPreviewSettings()
        {
            this.preview.OnPreviewSettings();
        }

        public virtual string GetInfoString()
        {
            return this.preview.GetInfoString();
        }

        internal virtual void OnAssetStoreInspectorGUI()
        {
        }

        public virtual void ReloadPreviewInstances()
        {
            this.preview.ReloadPreviewInstances();
        }

        internal virtual IPreviewable preview
        {
            get
            {
                if (this.m_DummyPreview == null)
                {
                    this.m_DummyPreview = new ObjectPreview();
                    this.m_DummyPreview.Initialize(this.targets);
                }
                return this.m_DummyPreview;
            }
        }
        internal PropertyHandlerCache propertyHandlerCache
        {
            get
            {
                return this.m_PropertyHandlerCache;
            }
        }
        internal static bool DoDrawDefaultInspector(SerializedObject obj)
        {
            EditorGUI.BeginChangeCheck();
            obj.Update();
            SerializedProperty iterator = obj.GetIterator();
            for (bool flag = true; iterator.NextVisible(flag); flag = false)
            {
                EditorGUI.BeginDisabledGroup("m_Script" == iterator.propertyPath);
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
            }
            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        internal bool DoDrawDefaultInspector()
        {
            return DoDrawDefaultInspector(this.serializedObject);
        }

        public void DrawHeader()
        {
            if (EditorGUIUtility.hierarchyMode)
            {
                this.DrawHeaderFromInsideHierarchy();
            }
            else
            {
                this.OnHeaderGUI();
            }
        }

        protected virtual void OnHeaderGUI()
        {
            DrawHeaderGUI(this, this.targetTitle);
        }

        internal virtual void OnHeaderControlsGUI()
        {
            GUILayoutUtility.GetRect(10f, 10f, 16f, 16f, EditorStyles.layerMaskField);
            GUILayout.FlexibleSpace();
            bool flag = true;
            if (!(this is AssetImporterInspector))
            {
                if (!AssetDatabase.IsMainAsset(this.targets[0]))
                {
                    flag = false;
                }
                AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.targets[0]));
                if ((atPath != null) && (atPath.GetType() != typeof(AssetImporter)))
                {
                    flag = false;
                }
            }
            if (flag && GUILayout.Button("Open", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                if (this is AssetImporterInspector)
                {
                    AssetDatabase.OpenAsset((this as AssetImporterInspector).assetEditor.targets);
                }
                else
                {
                    AssetDatabase.OpenAsset(this.targets);
                }
                GUIUtility.ExitGUI();
            }
        }

        internal virtual void OnHeaderIconGUI(Rect iconRect)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Texture2D image = null;
            if (!this.HasPreviewGUI())
            {
                bool flag = AssetPreview.IsLoadingAssetPreview(this.target.GetInstanceID());
                image = AssetPreview.GetAssetPreview(this.target);
                if (image == null)
                {
                    if (flag)
                    {
                        this.Repaint();
                    }
                    image = AssetPreview.GetMiniThumbnail(this.target);
                }
            }
            if (this.HasPreviewGUI())
            {
                this.OnPreviewGUI(iconRect, s_Styles.inspectorBigInner);
            }
            else if (image != null)
            {
                GUI.Label(iconRect, image, s_Styles.centerStyle);
            }
        }

        internal virtual void OnHeaderTitleGUI(Rect titleRect, string header)
        {
            titleRect.yMin -= 2f;
            titleRect.yMax += 2f;
            GUI.Label(titleRect, header, EditorStyles.largeLabel);
        }

        internal virtual void DrawHeaderHelpAndSettingsGUI(Rect r)
        {
            Object target = this.target;
            float num2 = 18f;
            if (this.IsEnabled())
            {
                Rect position = new Rect(r.xMax - num2, r.y + 5f, 14f, 14f);
                if (EditorGUI.ButtonMouseDown(position, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Native, EditorStyles.inspectorTitlebarText))
                {
                    EditorUtility.DisplayObjectContextMenu(position, this.targets, 0);
                }
                num2 += 18f;
            }
            EditorGUI.HelpIconButton(new Rect(r.xMax - num2, r.y + 5f, 14f, 14f), target);
        }

        private void DrawHeaderFromInsideHierarchy()
        {
            GUIStyle style = GUILayoutUtility.current.topLevel.style;
            EditorGUILayout.EndVertical();
            this.OnHeaderGUI();
            EditorGUILayout.BeginVertical(style, new GUILayoutOption[0]);
        }

        internal static Rect DrawHeaderGUI(Editor editor, string header)
        {
            return DrawHeaderGUI(editor, header, 0f);
        }

        internal static Rect DrawHeaderGUI(Editor editor, string header, float leftMargin)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            GUILayout.BeginHorizontal(s_Styles.inspectorBig, new GUILayoutOption[0]);
            GUILayout.Space(38f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Space(19f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (leftMargin > 0f)
            {
                GUILayout.Space(leftMargin);
            }
            if (editor != null)
            {
                editor.OnHeaderControlsGUI();
            }
            else
            {
                EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect r = new Rect(lastRect.x + leftMargin, lastRect.y, lastRect.width - leftMargin, lastRect.height);
            Rect iconRect = new Rect(r.x + 6f, r.y + 6f, 32f, 32f);
            if (editor != null)
            {
                editor.OnHeaderIconGUI(iconRect);
            }
            else
            {
                GUI.Label(iconRect, AssetPreview.GetMiniTypeThumbnail(typeof(Object)), s_Styles.centerStyle);
            }
            Rect titleRect = new Rect(r.x + 44f, r.y + 6f, ((r.width - 44f) - 38f) - 4f, 16f);
            if (editor != null)
            {
                editor.OnHeaderTitleGUI(titleRect, header);
            }
            else
            {
                GUI.Label(titleRect, header, EditorStyles.largeLabel);
            }
            if (editor != null)
            {
                editor.DrawHeaderHelpAndSettingsGUI(r);
            }
            Event current = Event.current;
            if ((((editor != null) && !editor.IsEnabled()) && ((current.type == EventType.MouseDown) && (current.button == 1))) && r.Contains(current.mousePosition))
            {
                EditorUtility.DisplayObjectContextMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), editor.targets, 0);
                current.Use();
            }
            return lastRect;
        }

        public virtual void DrawPreview(Rect previewArea)
        {
            ObjectPreview.DrawPreview(this, previewArea, this.targets);
        }

        internal bool CanBeExpandedViaAFoldout()
        {
            if (this.m_SerializedObject == null)
            {
                this.m_SerializedObject = new SerializedObject(this.targets);
            }
            else
            {
                this.m_SerializedObject.Update();
            }
            this.m_SerializedObject.inspectorMode = this.m_InspectorMode;
            SerializedProperty iterator = this.m_SerializedObject.GetIterator();
            for (bool flag = true; iterator.NextVisible(flag); flag = false)
            {
                if (EditorGUI.GetPropertyHeight(iterator, null, true) > 0f)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsAppropriateFileOpenForEdit(Object assetObject)
        {
            string str;
            return IsAppropriateFileOpenForEdit(assetObject, out str);
        }

        internal static bool IsAppropriateFileOpenForEdit(Object assetObject, out string message)
        {
            message = string.Empty;
            if (AssetDatabase.IsNativeAsset(assetObject))
            {
                if (!AssetDatabase.IsOpenForEdit(assetObject, out message))
                {
                    return false;
                }
            }
            else if (AssetDatabase.IsForeignAsset(assetObject) && !AssetDatabase.IsMetaFileOpenForEdit(assetObject, out message))
            {
                return false;
            }
            return true;
        }

        internal virtual bool IsEnabled()
        {
            foreach (Object obj2 in this.targets)
            {
                if ((obj2.hideFlags & HideFlags.NotEditable) != HideFlags.None)
                {
                    return false;
                }
                if (EditorUtility.IsPersistent(obj2) && !IsAppropriateFileOpenForEdit(obj2))
                {
                    return false;
                }
            }
            return true;
        }

        internal bool IsOpenForEdit()
        {
            string str;
            return this.IsOpenForEdit(out str);
        }

        internal bool IsOpenForEdit(out string message)
        {
            message = string.Empty;
            foreach (Object obj2 in this.targets)
            {
                if (EditorUtility.IsPersistent(obj2) && !IsAppropriateFileOpenForEdit(obj2))
                {
                    return false;
                }
            }
            return true;
        }

        public virtual bool UseDefaultMargins()
        {
            return true;
        }

        public void Initialize(Object[] targets)
        {
            throw new InvalidOperationException("You shouldn't call Initialize for Editors");
        }

        public bool MoveNextTarget()
        {
            this.referenceTargetIndex++;
            return (this.referenceTargetIndex < this.targets.Length);
        }

        public void ResetTarget()
        {
            this.referenceTargetIndex = 0;
        }
        private class Styles
        {
            public GUIStyle centerStyle = new GUIStyle();
            public GUIStyle inspectorBig = new GUIStyle(EditorStyles.inspectorBig);
            public GUIStyle inspectorBigInner = new GUIStyle("IN BigTitle inner");

            public Styles()
            {
                this.centerStyle.alignment = TextAnchor.MiddleCenter;
                RectOffset padding = this.inspectorBig.padding;
                padding.bottom--;
            }
        }
    }
}

