namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(GameObject))]
    internal class GameObjectInspector : Editor
    {
        public static GameObject dragObject;
        private const float kIconSize = 24f;
        private const float kLeft = 52f;
        private const float kToggleSize = 14f;
        private const float kTop = 4f;
        private const float kTop2 = 24f;
        private const float kTop3 = 44f;
        private bool m_AllOfSamePrefabType = true;
        private bool m_HasInstance;
        private SerializedProperty m_Icon;
        private SerializedProperty m_IsActive;
        private SerializedProperty m_Layer;
        private SerializedProperty m_Name;
        private List<GameObject> m_PreviewInstances;
        private PreviewRenderUtility m_PreviewUtility;
        private SerializedProperty m_StaticEditorFlags;
        private SerializedProperty m_Tag;
        private Vector2 previewDir;
        private static Styles s_styles;

        private GameObjectInspector()
        {
            if (EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D)
            {
                this.previewDir = new Vector2(0f, 0f);
            }
            else
            {
                this.previewDir = new Vector2(120f, -20f);
            }
        }

        private void CalculatePrefabStatus()
        {
            this.m_HasInstance = false;
            this.m_AllOfSamePrefabType = true;
            PrefabType prefabType = PrefabUtility.GetPrefabType(base.targets[0] as GameObject);
            foreach (GameObject obj2 in base.targets)
            {
                PrefabType type2 = PrefabUtility.GetPrefabType(obj2);
                if (type2 != prefabType)
                {
                    this.m_AllOfSamePrefabType = false;
                }
                if (((type2 != PrefabType.None) && (type2 != PrefabType.Prefab)) && (type2 != PrefabType.ModelPrefab))
                {
                    this.m_HasInstance = true;
                }
            }
        }

        private void CreatePreviewInstances()
        {
            this.DestroyPreviewInstances();
            if (this.m_PreviewInstances == null)
            {
                this.m_PreviewInstances = new List<GameObject>(base.targets.Length);
            }
            for (int i = 0; i < base.targets.Length; i++)
            {
                GameObject go = EditorUtility.InstantiateForAnimatorPreview(base.targets[i]);
                SetEnabledRecursive(go, false);
                this.m_PreviewInstances.Add(go);
            }
        }

        private void DestroyPreviewInstances()
        {
            if ((this.m_PreviewInstances != null) && (this.m_PreviewInstances.Count != 0))
            {
                foreach (GameObject obj2 in this.m_PreviewInstances)
                {
                    Object.DestroyImmediate(obj2);
                }
                this.m_PreviewInstances.Clear();
            }
        }

        private void DoRenderPreview()
        {
            GameObject go = this.m_PreviewInstances[this.referenceTargetIndex];
            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
            GetRenderableBoundsRecurse(ref bounds, go);
            float num = Mathf.Max(bounds.extents.magnitude, 0.0001f);
            float num2 = num * 3.8f;
            Quaternion quaternion = Quaternion.Euler(-this.previewDir.y, -this.previewDir.x, 0f);
            Vector3 vector = bounds.center - ((Vector3) (quaternion * (Vector3.forward * num2)));
            this.m_PreviewUtility.m_Camera.transform.position = vector;
            this.m_PreviewUtility.m_Camera.transform.rotation = quaternion;
            this.m_PreviewUtility.m_Camera.nearClipPlane = num2 - (num * 1.1f);
            this.m_PreviewUtility.m_Camera.farClipPlane = num2 + (num * 1.1f);
            this.m_PreviewUtility.m_Light[0].intensity = 0.7f;
            this.m_PreviewUtility.m_Light[0].transform.rotation = quaternion * Quaternion.Euler(40f, 40f, 0f);
            this.m_PreviewUtility.m_Light[1].intensity = 0.7f;
            this.m_PreviewUtility.m_Light[1].transform.rotation = quaternion * Quaternion.Euler(340f, 218f, 177f);
            Color ambient = new Color(0.1f, 0.1f, 0.1f, 0f);
            InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
            bool fog = RenderSettings.fog;
            Unsupported.SetRenderSettingsUseFogNoDirty(false);
            SetEnabledRecursive(go, true);
            this.m_PreviewUtility.m_Camera.Render();
            SetEnabledRecursive(go, false);
            Unsupported.SetRenderSettingsUseFogNoDirty(fog);
            InternalEditorUtility.RemoveCustomLighting();
        }

        internal bool DrawInspector(Rect contentRect)
        {
            int num5;
            bool flag4;
            if (s_styles == null)
            {
                s_styles = new Styles();
            }
            base.serializedObject.Update();
            GameObject target = this.target as GameObject;
            EditorGUIUtility.labelWidth = 52f;
            bool enabled = GUI.enabled;
            GUI.enabled = true;
            GUI.Label(new Rect(contentRect.x, contentRect.y, contentRect.width, contentRect.height + 3f), GUIContent.none, EditorStyles.inspectorBig);
            GUI.enabled = enabled;
            float width = contentRect.width;
            float y = contentRect.y;
            GUIContent goIcon = null;
            PrefabType none = PrefabType.None;
            if (this.m_AllOfSamePrefabType)
            {
                none = PrefabUtility.GetPrefabType(target);
                switch (none)
                {
                    case PrefabType.None:
                        goIcon = s_styles.goIcon;
                        break;

                    case PrefabType.Prefab:
                    case PrefabType.PrefabInstance:
                    case PrefabType.DisconnectedPrefabInstance:
                        goIcon = s_styles.prefabIcon;
                        break;

                    case PrefabType.ModelPrefab:
                    case PrefabType.ModelPrefabInstance:
                    case PrefabType.DisconnectedModelPrefabInstance:
                        goIcon = s_styles.modelIcon;
                        break;

                    case PrefabType.MissingPrefabInstance:
                        goIcon = s_styles.prefabIcon;
                        break;
                }
            }
            else
            {
                goIcon = s_styles.typelessIcon;
            }
            EditorGUI.ObjectIconDropDown(new Rect(3f, 4f + y, 24f, 24f), base.targets, true, goIcon.image as Texture2D, this.m_Icon);
            EditorGUI.BeginDisabledGroup(none == PrefabType.ModelPrefab);
            EditorGUI.PropertyField(new Rect(34f, 4f + y, 14f, 14f), this.m_IsActive, GUIContent.none);
            float num3 = s_styles.staticFieldToggleWidth + 15f;
            float num4 = ((width - 52f) - num3) - 5f;
            EditorGUI.DelayedTextField(new Rect(52f, (4f + y) + 1f, num4, 16f), this.m_Name, GUIContent.none);
            Rect totalPosition = new Rect(width - num3, 4f + y, s_styles.staticFieldToggleWidth, 16f);
            EditorGUI.BeginProperty(totalPosition, GUIContent.none, this.m_StaticEditorFlags);
            EditorGUI.BeginChangeCheck();
            Rect position = totalPosition;
            EditorGUI.showMixedValue |= ShowMixedStaticEditorFlags((StaticEditorFlags) this.m_StaticEditorFlags.intValue);
            Event current = Event.current;
            EventType type = current.type;
            bool flag2 = (current.type == EventType.MouseDown) && (current.button != 0);
            if (flag2)
            {
                current.type = EventType.Ignore;
            }
            bool flagValue = EditorGUI.ToggleLeft(position, "Static", target.isStatic);
            if (flag2)
            {
                current.type = type;
            }
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                SceneModeUtility.SetStaticFlags(base.targets, -1, flagValue);
                base.serializedObject.SetIsDifferentCacheDirty();
            }
            EditorGUI.EndProperty();
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_StaticEditorFlags.hasMultipleDifferentValues;
            EditorGUI.EnumMaskField(new Rect(totalPosition.x + s_styles.staticFieldToggleWidth, totalPosition.y, 10f, 14f), GameObjectUtility.GetStaticEditorFlags(target), s_styles.staticDropdown, out num5, out flag4);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                SceneModeUtility.SetStaticFlags(base.targets, num5, flag4);
                base.serializedObject.SetIsDifferentCacheDirty();
            }
            float num6 = 4f;
            float num7 = 4f;
            EditorGUIUtility.fieldWidth = ((((width - num6) - 52f) - s_styles.layerFieldWidth) - num7) / 2f;
            string tag = null;
            try
            {
                tag = target.tag;
            }
            catch (Exception)
            {
                tag = "Undefined";
            }
            EditorGUIUtility.labelWidth = s_styles.tagFieldWidth;
            Rect rect3 = new Rect(52f - EditorGUIUtility.labelWidth, 24f + y, EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth, 16f);
            EditorGUI.BeginProperty(rect3, GUIContent.none, this.m_Tag);
            EditorGUI.BeginChangeCheck();
            string str2 = EditorGUI.TagField(rect3, EditorGUIUtility.TempContent("Tag"), tag);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Tag.stringValue = str2;
                Undo.RecordObjects(base.targets, "Change Tag of " + this.targetTitle);
                foreach (Object obj3 in base.targets)
                {
                    (obj3 as GameObject).tag = str2;
                }
            }
            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = s_styles.layerFieldWidth;
            rect3 = new Rect((52f + EditorGUIUtility.fieldWidth) + num6, 24f + y, EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth, 16f);
            EditorGUI.BeginProperty(rect3, GUIContent.none, this.m_Layer);
            EditorGUI.BeginChangeCheck();
            int layer = EditorGUI.LayerField(rect3, EditorGUIUtility.TempContent("Layer"), target.layer);
            if (EditorGUI.EndChangeCheck())
            {
                GameObjectUtility.ShouldIncludeChildren children = GameObjectUtility.DisplayUpdateChildrenDialogIfNeeded(base.targets.OfType<GameObject>(), "Change Layer", "Do you want to set layer to " + InternalEditorUtility.GetLayerName(layer) + " for all child objects as well?");
                if (children != GameObjectUtility.ShouldIncludeChildren.Cancel)
                {
                    this.m_Layer.intValue = layer;
                    this.SetLayer(layer, children == GameObjectUtility.ShouldIncludeChildren.IncludeChildren);
                }
            }
            EditorGUI.EndProperty();
            if (!this.m_HasInstance || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                goto Label_093F;
            }
            float num10 = ((width - 52f) - 5f) / 3f;
            Rect rect4 = new Rect(52f + (num10 * 0f), 44f + y, num10, 15f);
            Rect rect5 = new Rect(52f + (num10 * 1f), 44f + y, num10, 15f);
            Rect rect6 = new Rect(52f + (num10 * 2f), 44f + y, num10, 15f);
            Rect rect7 = new Rect(52f, 44f + y, num10 * 3f, 15f);
            GUIContent content2 = (base.targets.Length <= 1) ? s_styles.goTypeLabel[(int) none] : s_styles.goTypeLabelMultiple;
            if (content2 != null)
            {
                float x = GUI.skin.label.CalcSize(content2).x;
                switch (none)
                {
                    case PrefabType.DisconnectedModelPrefabInstance:
                    case PrefabType.MissingPrefabInstance:
                    case PrefabType.DisconnectedPrefabInstance:
                        GUI.contentColor = GUI.skin.GetStyle("CN StatusWarn").normal.textColor;
                        if (none == PrefabType.MissingPrefabInstance)
                        {
                            GUI.Label(new Rect(52f, 44f + y, (width - 52f) - 5f, 18f), content2, EditorStyles.whiteLabel);
                        }
                        else
                        {
                            GUI.Label(new Rect((52f - x) - 5f, 44f + y, (width - 52f) - 5f, 18f), content2, EditorStyles.whiteLabel);
                        }
                        GUI.contentColor = Color.white;
                        goto Label_072D;
                }
                Rect rect8 = new Rect((52f - x) - 5f, 44f + y, x, 18f);
                GUI.Label(rect8, content2);
            }
        Label_072D:
            if (base.targets.Length > 1)
            {
                GUI.Label(rect7, "Instance Management Disabled", s_styles.instanceManagementInfo);
            }
            else
            {
                if ((none != PrefabType.MissingPrefabInstance) && GUI.Button(rect4, "Select", "MiniButtonLeft"))
                {
                    Selection.activeObject = PrefabUtility.GetPrefabParent(this.target);
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }
                if (((none == PrefabType.DisconnectedModelPrefabInstance) || (none == PrefabType.DisconnectedPrefabInstance)) && GUI.Button(rect5, "Revert", "MiniButtonMid"))
                {
                    Undo.RegisterFullObjectHierarchyUndo(target, "Revert to prefab");
                    PrefabUtility.ReconnectToLastPrefab(target);
                    PrefabUtility.RevertPrefabInstance(target);
                    this.CalculatePrefabStatus();
                    Undo.RegisterCreatedObjectUndo(target, "Reconnect prefab");
                    GUIUtility.ExitGUI();
                }
                bool flag5 = GUI.enabled;
                GUI.enabled = GUI.enabled && !AnimationMode.InAnimationMode();
                if (((none == PrefabType.ModelPrefabInstance) || (none == PrefabType.PrefabInstance)) && GUI.Button(rect5, "Revert", "MiniButtonMid"))
                {
                    Undo.RegisterFullObjectHierarchyUndo(target, "Revert Prefab Instance");
                    PrefabUtility.RevertPrefabInstance(target);
                    this.CalculatePrefabStatus();
                    Undo.RegisterCreatedObjectUndo(target, "Revert prefab");
                    GUIUtility.ExitGUI();
                }
                if ((none == PrefabType.PrefabInstance) || (none == PrefabType.DisconnectedPrefabInstance))
                {
                    GameObject source = PrefabUtility.FindValidUploadPrefabInstanceRoot(target);
                    GUI.enabled = (source != null) && !AnimationMode.InAnimationMode();
                    if (GUI.Button(rect6, "Apply", "MiniButtonRight"))
                    {
                        Object prefabParent = PrefabUtility.GetPrefabParent(source);
                        string assetPath = AssetDatabase.GetAssetPath(prefabParent);
                        string[] assets = new string[] { assetPath };
                        if (Provider.PromptAndCheckoutIfNeeded(assets, "The version control requires you to check out the prefab before applying changes."))
                        {
                            PrefabUtility.ReplacePrefab(source, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
                            this.CalculatePrefabStatus();
                            GUIUtility.ExitGUI();
                        }
                    }
                }
                GUI.enabled = flag5;
                if (((none == PrefabType.DisconnectedModelPrefabInstance) || (none == PrefabType.ModelPrefabInstance)) && GUI.Button(rect6, "Open", "MiniButtonRight"))
                {
                    AssetDatabase.OpenAsset(PrefabUtility.GetPrefabParent(this.target));
                    GUIUtility.ExitGUI();
                }
            }
        Label_093F:
            EditorGUI.EndDisabledGroup();
            base.serializedObject.ApplyModifiedProperties();
            return true;
        }

        private Object[] GetObjects(bool includeChildren)
        {
            return SceneModeUtility.GetObjects(base.targets, includeChildren);
        }

        public static void GetRenderableBoundsRecurse(ref Bounds bounds, GameObject go)
        {
            MeshRenderer component = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter filter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
            if (((component != null) && (filter != null)) && (filter.sharedMesh != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = component.bounds;
                }
                else
                {
                    bounds.Encapsulate(component.bounds);
                }
            }
            SkinnedMeshRenderer renderer2 = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            if ((renderer2 != null) && (renderer2.sharedMesh != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = renderer2.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer2.bounds);
                }
            }
            SpriteRenderer renderer3 = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            if ((renderer3 != null) && (renderer3.sprite != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = renderer3.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer3.bounds);
                }
            }
            IEnumerator enumerator = go.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    GetRenderableBoundsRecurse(ref bounds, current.gameObject);
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

        public static Vector3 GetRenderableCenterRecurse(GameObject go, int minDepth, int maxDepth)
        {
            Vector3 zero = Vector3.zero;
            float num = GetRenderableCenterRecurse(ref zero, go, 0, minDepth, maxDepth);
            if (num > 0f)
            {
                return (Vector3) (zero / num);
            }
            return go.transform.position;
        }

        private static float GetRenderableCenterRecurse(ref Vector3 center, GameObject go, int depth, int minDepth, int maxDepth)
        {
            if (depth > maxDepth)
            {
                return 0f;
            }
            float num = 0f;
            if (depth > minDepth)
            {
                MeshRenderer component = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                MeshFilter filter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
                SkinnedMeshRenderer renderer2 = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
                SpriteRenderer renderer3 = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                if (((component == null) && (filter == null)) && ((renderer2 == null) && (renderer3 == null)))
                {
                    num = 1f;
                    center += go.transform.position;
                }
                else if ((component != null) && (filter != null))
                {
                    if (Vector3.Distance(component.bounds.center, go.transform.position) < 0.01f)
                    {
                        num = 1f;
                        center += go.transform.position;
                    }
                }
                else if (renderer2 != null)
                {
                    if (Vector3.Distance(renderer2.bounds.center, go.transform.position) < 0.01f)
                    {
                        num = 1f;
                        center += go.transform.position;
                    }
                }
                else if ((renderer3 != null) && (Vector3.Distance(renderer3.bounds.center, go.transform.position) < 0.01f))
                {
                    num = 1f;
                    center += go.transform.position;
                }
            }
            depth++;
            IEnumerator enumerator = go.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    num += GetRenderableCenterRecurse(ref center, current.gameObject, depth, minDepth, maxDepth);
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
            return num;
        }

        public override bool HasPreviewGUI()
        {
            if (!EditorUtility.IsPersistent(this.target))
            {
                return false;
            }
            return this.HasStaticPreview();
        }

        public static bool HasRenderablePartsRecurse(GameObject go)
        {
            MeshRenderer component = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter filter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
            if (((component != null) && (filter != null)) && (filter.sharedMesh != null))
            {
                return true;
            }
            SkinnedMeshRenderer renderer2 = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            if ((renderer2 != null) && (renderer2.sharedMesh != null))
            {
                return true;
            }
            SpriteRenderer renderer3 = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            if ((renderer3 != null) && (renderer3.sprite != null))
            {
                return true;
            }
            IEnumerator enumerator = go.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (HasRenderablePartsRecurse(current.gameObject))
                    {
                        return true;
                    }
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
            return false;
        }

        private bool HasStaticPreview()
        {
            if (base.targets.Length > 1)
            {
                return true;
            }
            if (this.target == null)
            {
                return false;
            }
            GameObject target = this.target as GameObject;
            Camera component = target.GetComponent(typeof(Camera)) as Camera;
            return ((component != null) || HasRenderablePartsRecurse(target));
        }

        private void InitPreview()
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility(true);
                this.m_PreviewUtility.m_CameraFieldOfView = 30f;
                this.m_PreviewUtility.m_Camera.cullingMask = ((int) 1) << Camera.PreviewCullingLayer;
                this.CreatePreviewInstances();
            }
        }

        public void OnDestroy()
        {
            this.DestroyPreviewInstances();
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
            }
        }

        private void OnDisable()
        {
        }

        public void OnEnable()
        {
            this.m_Name = base.serializedObject.FindProperty("m_Name");
            this.m_IsActive = base.serializedObject.FindProperty("m_IsActive");
            this.m_Layer = base.serializedObject.FindProperty("m_Layer");
            this.m_Tag = base.serializedObject.FindProperty("m_TagString");
            this.m_StaticEditorFlags = base.serializedObject.FindProperty("m_StaticEditorFlags");
            this.m_Icon = base.serializedObject.FindProperty("m_Icon");
            this.CalculatePrefabStatus();
        }

        protected override void OnHeaderGUI()
        {
            Rect contentRect = GUILayoutUtility.GetRect(0f, !this.m_HasInstance ? ((float) 40) : ((float) 60));
            this.DrawInspector(contentRect);
        }

        public override void OnInspectorGUI()
        {
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Preview requires\nrender texture support");
                }
            }
            else
            {
                this.InitPreview();
                this.previewDir = PreviewGUI.Drag2D(this.previewDir, r);
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_PreviewUtility.BeginPreview(r, background);
                    this.DoRenderPreview();
                    this.m_PreviewUtility.EndAndDrawPreview(r);
                }
            }
        }

        public override void OnPreviewSettings()
        {
            if (ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                GUI.enabled = true;
                this.InitPreview();
            }
        }

        public void OnSceneDrag(SceneView sceneView)
        {
            GameObject target = this.target as GameObject;
            switch (PrefabUtility.GetPrefabType(target))
            {
                case PrefabType.Prefab:
                case PrefabType.ModelPrefab:
                {
                    Event current = Event.current;
                    EventType type = current.type;
                    if (type != EventType.DragUpdated)
                    {
                        if (type == EventType.DragPerform)
                        {
                            string uniqueNameForSibling = GameObjectUtility.GetUniqueNameForSibling(null, dragObject.name);
                            dragObject.hideFlags = HideFlags.None;
                            Undo.RegisterCreatedObjectUndo(dragObject, "Place " + dragObject.name);
                            EditorUtility.SetDirty(dragObject);
                            DragAndDrop.AcceptDrag();
                            Selection.activeObject = dragObject;
                            HandleUtility.ignoreRaySnapObjects = null;
                            EditorWindow.mouseOverWindow.Focus();
                            dragObject.name = uniqueNameForSibling;
                            dragObject = null;
                            current.Use();
                            return;
                        }
                        if ((type == EventType.DragExited) && (dragObject != null))
                        {
                            Object.DestroyImmediate(dragObject, false);
                            HandleUtility.ignoreRaySnapObjects = null;
                            dragObject = null;
                            current.Use();
                        }
                    }
                    else
                    {
                        if (dragObject == null)
                        {
                            dragObject = (GameObject) PrefabUtility.InstantiatePrefab(PrefabUtility.FindPrefabRoot(target));
                            HandleUtility.ignoreRaySnapObjects = dragObject.GetComponentsInChildren<Transform>();
                            dragObject.hideFlags = HideFlags.HideInHierarchy;
                            dragObject.name = target.name;
                        }
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        object obj3 = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
                        if (obj3 != null)
                        {
                            RaycastHit hit = (RaycastHit) obj3;
                            float num = 0f;
                            if (Tools.pivotMode == PivotMode.Center)
                            {
                                float num2 = HandleUtility.CalcRayPlaceOffset(HandleUtility.ignoreRaySnapObjects, hit.normal);
                                if (num2 != float.PositiveInfinity)
                                {
                                    num = Vector3.Dot(dragObject.transform.position, hit.normal) - num2;
                                }
                            }
                            dragObject.transform.position = Matrix4x4.identity.MultiplyPoint(hit.point + ((Vector3) (hit.normal * num)));
                        }
                        else
                        {
                            dragObject.transform.position = HandleUtility.GUIPointToWorldRay(current.mousePosition).GetPoint(10f);
                        }
                        if (sceneView.in2DMode)
                        {
                            Vector3 position = dragObject.transform.position;
                            position.z = PrefabUtility.FindPrefabRoot(target).transform.position.z;
                            dragObject.transform.position = position;
                        }
                        current.Use();
                    }
                    break;
                }
            }
        }

        public override void ReloadPreviewInstances()
        {
            this.CreatePreviewInstances();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (!this.HasStaticPreview() || !ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                return null;
            }
            this.InitPreview();
            this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float) width, (float) height));
            this.DoRenderPreview();
            return this.m_PreviewUtility.EndStaticPreview();
        }

        public static void SetEnabledRecursive(GameObject go, bool enabled)
        {
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = enabled;
            }
        }

        private void SetLayer(int layer, bool includeChildren)
        {
            Object[] objects = this.GetObjects(includeChildren);
            Undo.RecordObjects(objects, "Change Layer of " + this.targetTitle);
            foreach (GameObject obj2 in objects)
            {
                obj2.layer = layer;
            }
        }

        private static bool ShowMixedStaticEditorFlags(StaticEditorFlags mask)
        {
            uint num = 0;
            uint num2 = 0;
            IEnumerator enumerator = Enum.GetValues(typeof(StaticEditorFlags)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    num2++;
                    if ((mask & ((int) current)) > 0)
                    {
                        num++;
                    }
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
            return ((num > 0) && (num != num2));
        }

        private class Styles
        {
            public GUIContent dataTemplateIcon = EditorGUIUtility.IconContent("PrefabNormal Icon");
            public GUIContent dropDownIcon = EditorGUIUtility.IconContent("Icon Dropdown");
            public GUIContent goIcon = EditorGUIUtility.IconContent("GameObject Icon");
            public GUIContent[] goTypeLabel;
            public GUIContent goTypeLabelMultiple = new GUIContent("Multiple");
            public GUIStyle instanceManagementInfo = new GUIStyle(EditorStyles.helpBox);
            public float layerFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Layer")).x;
            public GUIContent modelIcon = EditorGUIUtility.IconContent("PrefabModel Icon");
            public float navLayerFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Nav Layer")).x;
            public GUIContent prefabIcon = EditorGUIUtility.IconContent("PrefabNormal Icon");
            public GUIStyle staticDropdown = "StaticDropdown";
            public float staticFieldToggleWidth = (EditorStyles.toggle.CalcSize(EditorGUIUtility.TempContent("Static")).x + 6f);
            public float tagFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Tag")).x;
            public GUIContent typelessIcon = EditorGUIUtility.IconContent("Prefab Icon");

            public Styles()
            {
                GUIContent[] contentArray1 = new GUIContent[8];
                contentArray1[1] = EditorGUIUtility.TextContent("Prefab");
                contentArray1[2] = EditorGUIUtility.TextContent("Model");
                contentArray1[3] = EditorGUIUtility.TextContent("Prefab");
                contentArray1[4] = EditorGUIUtility.TextContent("Model");
                contentArray1[5] = EditorGUIUtility.TextContent("Missing|The source Prefab or Model has been deleted.");
                contentArray1[6] = EditorGUIUtility.TextContent("Prefab|You have broken the prefab connection. Changes to the prefab will not be applied to this object before you Apply or Revert.");
                contentArray1[7] = EditorGUIUtility.TextContent("Model|You have broken the prefab connection. Changes to the model will not be applied to this object before you Revert.");
                this.goTypeLabel = contentArray1;
                GUIStyle style = "MiniButtonMid";
                this.instanceManagementInfo.padding = style.padding;
                this.instanceManagementInfo.alignment = style.alignment;
            }
        }
    }
}

