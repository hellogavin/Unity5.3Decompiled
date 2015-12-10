namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor.Animations;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [CustomEditor(typeof(Avatar))]
    internal class AvatarEditor : Editor
    {
        internal bool m_CameFromImportSettings;
        private AvatarColliderEditor m_ColliderEditor;
        private EditMode m_EditMode;
        internal GameObject m_GameObject;
        private AvatarHandleEditor m_HandleEditor;
        protected bool m_InspectorLocked;
        private AvatarMappingEditor m_MappingEditor;
        internal Dictionary<Transform, bool> m_ModelBones;
        private AvatarMuscleEditor m_MuscleEditor;
        private GameObject m_PrefabInstance;
        protected List<SceneStateCache> m_SceneStates;
        private bool m_SwitchToEditMode;
        protected int m_TabIndex;
        internal static bool s_EditImmediatelyOnNextOpen;
        private static Styles s_Styles;
        private const int sColliderTab = 3;
        private const int sDefaultTab = 0;
        private const int sHandleTab = 2;
        private const int sMappingTab = 0;
        private const int sMuscleTab = 1;

        private void ChangeInspectorLock(bool locked)
        {
            foreach (InspectorWindow window in InspectorWindow.GetAllInspectorWindows())
            {
                foreach (Editor editor in window.GetTracker().activeEditors)
                {
                    if (editor == this)
                    {
                        this.m_InspectorLocked = window.isLocked;
                        window.isLocked = locked;
                    }
                }
            }
        }

        protected void CreateEditor()
        {
            switch (this.m_TabIndex)
            {
                case 1:
                    this.editor = ScriptableObject.CreateInstance<AvatarMuscleEditor>();
                    break;

                case 2:
                    this.editor = ScriptableObject.CreateInstance<AvatarHandleEditor>();
                    break;

                case 3:
                    this.editor = ScriptableObject.CreateInstance<AvatarColliderEditor>();
                    break;

                default:
                    this.editor = ScriptableObject.CreateInstance<AvatarMappingEditor>();
                    break;
            }
            this.editor.hideFlags = HideFlags.HideAndDontSave;
            this.editor.Enable(this);
        }

        protected void DestroyEditor()
        {
            this.editor.OnDestroy();
            this.editor = null;
        }

        private void EditButtonGUI()
        {
            if ((this.avatar != null) && this.avatar.isHuman)
            {
                ModelImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.avatar)) as ModelImporter;
                if (atPath != null)
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
                    if (GUILayout.Button(styles.editCharacter, options) && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        this.SwitchToEditMode();
                        GUIUtility.ExitGUI();
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void EditingGUI()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            int tabIndex = this.m_TabIndex;
            bool enabled = GUI.enabled;
            GUI.enabled = (this.avatar != null) && this.avatar.isHuman;
            tabIndex = GUILayout.Toolbar(tabIndex, styles.tabs, new GUILayoutOption[0]);
            GUI.enabled = enabled;
            if (tabIndex != this.m_TabIndex)
            {
                this.DestroyEditor();
                this.m_TabIndex = tabIndex;
                this.CreateEditor();
            }
            GUILayout.EndHorizontal();
            this.editor.OnInspectorGUI();
        }

        internal override SerializedObject GetSerializedObjectInternal()
        {
            if (base.m_SerializedObject == null)
            {
                base.m_SerializedObject = SerializedObject.LoadFromCache(base.GetInstanceID());
            }
            if (base.m_SerializedObject == null)
            {
                base.m_SerializedObject = new SerializedObject(AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.target)));
            }
            return base.m_SerializedObject;
        }

        public bool HasFrameBounds()
        {
            if (this.m_ModelBones != null)
            {
                foreach (KeyValuePair<Transform, bool> pair in this.m_ModelBones)
                {
                    if (pair.Key == Selection.activeTransform)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnDestroy()
        {
            if (this.m_EditMode == EditMode.Editing)
            {
                this.SwitchToAssetMode();
            }
        }

        private void OnDisable()
        {
            if (this.m_EditMode == EditMode.Editing)
            {
                this.editor.Disable();
            }
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
            if (base.m_SerializedObject != null)
            {
                base.m_SerializedObject.Cache(base.GetInstanceID());
                base.m_SerializedObject = null;
            }
        }

        private void OnEnable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
            this.m_SwitchToEditMode = false;
            if (this.m_EditMode == EditMode.Editing)
            {
                this.m_ModelBones = AvatarSetupTool.GetModelBones(this.m_GameObject.transform, false, null);
                this.editor.Enable(this);
            }
            else if (this.m_EditMode == EditMode.NotEditing)
            {
                this.editor = null;
                if (s_EditImmediatelyOnNextOpen)
                {
                    this.m_CameFromImportSettings = true;
                    s_EditImmediatelyOnNextOpen = false;
                }
            }
        }

        public Bounds OnGetFrameBounds()
        {
            Transform activeTransform = Selection.activeTransform;
            Bounds bounds = new Bounds(activeTransform.position, new Vector3(0f, 0f, 0f));
            IEnumerator enumerator = activeTransform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    bounds.Encapsulate(current.position);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            if (activeTransform.parent != null)
            {
                bounds.Encapsulate(activeTransform.parent.position);
            }
            return bounds;
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = true;
            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins, new GUILayoutOption[0]);
            if (this.m_EditMode == EditMode.Editing)
            {
                this.EditingGUI();
            }
            else if (!this.m_CameFromImportSettings)
            {
                this.EditButtonGUI();
            }
            else if ((this.m_EditMode == EditMode.NotEditing) && (Event.current.type == EventType.Repaint))
            {
                this.m_SwitchToEditMode = true;
            }
            EditorGUILayout.EndVertical();
        }

        public void OnSceneGUI()
        {
            if (this.m_EditMode == EditMode.Editing)
            {
                this.editor.OnSceneGUI();
            }
        }

        private void SelectAsset()
        {
            Object target;
            if (this.m_CameFromImportSettings)
            {
                target = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(this.target));
            }
            else
            {
                target = this.target;
            }
            Selection.activeObject = target;
        }

        internal void SwitchToAssetMode()
        {
            foreach (SceneStateCache cache in this.m_SceneStates)
            {
                if (cache.view != null)
                {
                    cache.view.m_SceneViewState.showFog = cache.state.showFog;
                    cache.view.m_SceneViewState.showFlares = cache.state.showFlares;
                    cache.view.m_SceneViewState.showMaterialUpdate = cache.state.showMaterialUpdate;
                    cache.view.m_SceneViewState.showSkybox = cache.state.showSkybox;
                }
            }
            this.m_EditMode = EditMode.Stopping;
            this.DestroyEditor();
            this.ChangeInspectorLock(this.m_InspectorLocked);
            if (!EditorApplication.isUpdating && !Unsupported.IsDestroyScriptableObject(this))
            {
                string path = SceneManager.GetActiveScene().path;
                string str2 = string.Empty;
                if ((path.Length <= 0) && (str2.Length <= 0))
                {
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
                }
            }
            else if (Unsupported.IsDestroyScriptableObject(this))
            {
                <SwitchToAssetMode>c__AnonStorey94 storey;
                storey = new <SwitchToAssetMode>c__AnonStorey94 {
                    CleanUpSceneOnDestroy = null,
                    userFileName = string.Empty,
                    CleanUpSceneOnDestroy = new EditorApplication.CallbackFunction(storey.<>m__1BE)
                };
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, storey.CleanUpSceneOnDestroy);
            }
            this.m_GameObject = null;
            this.m_ModelBones = null;
            this.SelectAsset();
            if (!this.m_CameFromImportSettings)
            {
                this.m_EditMode = EditMode.NotEditing;
            }
        }

        internal void SwitchToEditMode()
        {
            this.m_EditMode = EditMode.Starting;
            this.ChangeInspectorLock(true);
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            this.m_GameObject = Object.Instantiate<GameObject>(this.prefab);
            if (base.serializedObject.FindProperty("m_OptimizeGameObjects").boolValue)
            {
                AnimatorUtility.DeoptimizeTransformHierarchy(this.m_GameObject);
            }
            Animator component = this.m_GameObject.GetComponent<Animator>();
            if ((component != null) && (component.runtimeAnimatorController == null))
            {
                AnimatorController controller = new AnimatorController {
                    hideFlags = HideFlags.DontSave
                };
                controller.AddLayer("preview");
                controller.layers[0].stateMachine.hideFlags = HideFlags.DontSave;
                component.runtimeAnimatorController = controller;
            }
            Dictionary<Transform, bool> actualBones = AvatarSetupTool.GetModelBones(this.m_GameObject.transform, true, null);
            AvatarSetupTool.BoneWrapper[] humanBones = AvatarSetupTool.GetHumanBones(base.serializedObject, actualBones);
            this.m_ModelBones = AvatarSetupTool.GetModelBones(this.m_GameObject.transform, false, humanBones);
            Selection.activeObject = this.m_GameObject;
            foreach (SceneHierarchyWindow window in Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow)))
            {
                window.SetExpandedRecursive(this.m_GameObject.GetInstanceID(), true);
            }
            this.CreateEditor();
            this.m_EditMode = EditMode.Editing;
            this.m_SceneStates = new List<SceneStateCache>();
            IEnumerator enumerator = SceneView.sceneViews.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    SceneView current = (SceneView) enumerator.Current;
                    SceneStateCache item = new SceneStateCache {
                        state = new SceneView.SceneViewState(current.m_SceneViewState),
                        view = current
                    };
                    this.m_SceneStates.Add(item);
                    current.m_SceneViewState.showFlares = false;
                    current.m_SceneViewState.showMaterialUpdate = false;
                    current.m_SceneViewState.showFog = false;
                    current.m_SceneViewState.showSkybox = false;
                    current.FrameSelected();
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        public void Update()
        {
            if (this.m_SwitchToEditMode)
            {
                this.m_SwitchToEditMode = false;
                this.SwitchToEditMode();
                base.Repaint();
            }
            if (this.m_EditMode == EditMode.Editing)
            {
                if ((this.m_GameObject == null) || (this.m_ModelBones == null))
                {
                    this.SwitchToAssetMode();
                }
                else if (EditorApplication.isPlaying)
                {
                    this.SwitchToAssetMode();
                }
                else if (this.m_ModelBones != null)
                {
                    foreach (KeyValuePair<Transform, bool> pair in this.m_ModelBones)
                    {
                        if (pair.Key == null)
                        {
                            this.SwitchToAssetMode();
                            break;
                        }
                    }
                }
            }
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        internal Avatar avatar
        {
            get
            {
                return (this.target as Avatar);
            }
        }

        protected AvatarSubEditor editor
        {
            get
            {
                switch (this.m_TabIndex)
                {
                    case 1:
                        return this.m_MuscleEditor;

                    case 2:
                        return this.m_HandleEditor;

                    case 3:
                        return this.m_ColliderEditor;
                }
                return this.m_MappingEditor;
            }
            set
            {
                switch (this.m_TabIndex)
                {
                    case 1:
                        this.m_MuscleEditor = value as AvatarMuscleEditor;
                        return;

                    case 2:
                        this.m_HandleEditor = value as AvatarHandleEditor;
                        return;

                    case 3:
                        this.m_ColliderEditor = value as AvatarColliderEditor;
                        return;
                }
                this.m_MappingEditor = value as AvatarMappingEditor;
            }
        }

        public GameObject prefab
        {
            get
            {
                return (AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(this.target)) as GameObject);
            }
        }

        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        [CompilerGenerated]
        private sealed class <SwitchToAssetMode>c__AnonStorey94
        {
            internal EditorApplication.CallbackFunction CleanUpSceneOnDestroy;
            internal string userFileName;

            internal void <>m__1BE()
            {
                if ((SceneManager.GetActiveScene().path.Length <= 0) && (this.userFileName.Length <= 0))
                {
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
                }
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, this.CleanUpSceneOnDestroy);
            }
        }

        private enum EditMode
        {
            NotEditing,
            Starting,
            Editing,
            Stopping
        }

        [Serializable]
        protected class SceneStateCache
        {
            public SceneView.SceneViewState state;
            public SceneView view;
        }

        private class Styles
        {
            public GUIContent editCharacter = EditorGUIUtility.TextContent("Configure Avatar");
            public GUIContent reset = EditorGUIUtility.TextContent("Reset");
            public GUIContent[] tabs = new GUIContent[] { EditorGUIUtility.TextContent("Mapping"), EditorGUIUtility.TextContent("Muscles & Settings") };
        }
    }
}

