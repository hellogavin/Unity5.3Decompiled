namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(Cloth)), CanEditMultipleObjects]
    internal class ClothInspector : Editor
    {
        private const float kDisabledValue = float.MaxValue;
        private bool m_DidSelect;
        private Vector3[] m_LastVertices;
        private float[] m_MaxVisualizedValue = new float[3];
        private int m_MeshVerticesPerSelectionVertex;
        private float[] m_MinVisualizedValue = new float[3];
        private int m_MouseOver = -1;
        private bool m_RectSelecting;
        private bool[] m_RectSelection;
        private RectSelectionMode m_RectSelectionMode = RectSelectionMode.Add;
        private Mesh[] m_SelectedMesh;
        private bool[] m_Selection;
        private Mesh[] m_SelectionMesh;
        private Vector2 m_SelectMousePoint;
        private Vector2 m_SelectStartPoint;
        private Mesh m_VertexMesh;
        private Mesh m_VertexMeshSelected;
        private static Texture2D s_ColorTexture = null;
        private static int s_MaxVertices;
        private static GUIContent[] s_ModeStrings = new GUIContent[] { EditorGUIUtility.TextContent("Fixed"), EditorGUIUtility.TextContent("Max Distance"), EditorGUIUtility.TextContent("Surface Penetration") };
        private static GUIContent s_PaintIcon = EditorGUIUtility.IconContent("ClothInspector.PaintValue", "Change this vertex coefficient value by painting in the scene view.");
        private static Material s_SelectedMaterial = null;
        private static Color s_SelectionColor;
        private static Material s_SelectionMaterial = null;
        private static Material s_SelectionMaterialBackfaces = null;
        private static GUIContent[] s_ToolIcons = new GUIContent[] { EditorGUIUtility.TextContent("Select|Select vertices and edit their cloth coefficients in the inspector."), EditorGUIUtility.TextContent("Paint|Paint cloth coefficients on to vertices.") };

        private unsafe void ApplyRectSelection()
        {
            ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
            for (int i = 0; i < coefficients.Length; i++)
            {
                switch (this.m_RectSelectionMode)
                {
                    case RectSelectionMode.Replace:
                        this.m_Selection[i] = this.m_RectSelection[i];
                        break;

                    case RectSelectionMode.Add:
                        *((sbyte*) &(this.m_Selection[i])) |= this.m_RectSelection[i];
                        break;

                    case RectSelectionMode.Substract:
                        this.m_Selection[i] = this.m_Selection[i] && !this.m_RectSelection[i];
                        break;
                }
            }
        }

        private void AssignColorsToMeshArray(Color[] colors, Mesh[] meshArray)
        {
            int num = colors.Length / this.m_MeshVerticesPerSelectionVertex;
            int num2 = (num / s_MaxVertices) + 1;
            for (int i = 0; i < num2; i++)
            {
                int num4 = num - (i * s_MaxVertices);
                if (num4 > s_MaxVertices)
                {
                    num4 = s_MaxVertices;
                }
                Color[] destinationArray = new Color[num4 * this.m_MeshVerticesPerSelectionVertex];
                Array.Copy(colors, (i * s_MaxVertices) * this.m_MeshVerticesPerSelectionVertex, destinationArray, 0, num4 * this.m_MeshVerticesPerSelectionVertex);
                meshArray[i].colors = destinationArray;
            }
        }

        private float CoefficientField(float value, float useValue, bool enabled, DrawMode mode)
        {
            GUIContent modeString = this.GetModeString(mode);
            EditorGUI.BeginDisabledGroup(!enabled);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUI.showMixedValue = useValue < 0f;
            EditorGUI.BeginChangeCheck();
            useValue = !EditorGUILayout.Toggle(GUIContent.none, useValue != 0f, new GUILayoutOption[0]) ? ((float) 0) : ((float) 1);
            if (EditorGUI.EndChangeCheck())
            {
                if (useValue > 0f)
                {
                    value = 0f;
                }
                else
                {
                    value = float.MaxValue;
                }
                this.drawMode = mode;
            }
            GUILayout.Space(-152f);
            EditorGUI.showMixedValue = false;
            EditorGUI.BeginDisabledGroup(useValue != 1f);
            float num = value;
            EditorGUI.showMixedValue = value < 0f;
            EditorGUI.BeginChangeCheck();
            int keyboardControl = GUIUtility.keyboardControl;
            if (useValue > 0f)
            {
                num = EditorGUILayout.FloatField(modeString, value, new GUILayoutOption[0]);
            }
            else
            {
                EditorGUILayout.FloatField(modeString, 0f, new GUILayoutOption[0]);
            }
            bool flag = EditorGUI.EndChangeCheck();
            if (flag)
            {
                value = num;
                if (value < 0f)
                {
                    value = 0f;
                }
            }
            if (flag || (keyboardControl != GUIUtility.keyboardControl))
            {
                this.drawMode = mode;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
            if (useValue > 0f)
            {
                float num3 = this.m_MinVisualizedValue[(int) mode];
                float num4 = this.m_MaxVisualizedValue[(int) mode];
                if ((num4 - num3) > 0f)
                {
                    this.DrawColorBox(null, this.GetGradientColor((value - num3) / (num4 - num3)));
                }
                else
                {
                    this.DrawColorBox(null, this.GetGradientColor((value > num3) ? ((float) 1) : ((float) 0)));
                }
            }
            else
            {
                this.DrawColorBox(null, Color.black);
            }
            EditorGUI.showMixedValue = false;
            GUILayout.EndHorizontal();
            return value;
        }

        public void DrawColorBox(Texture gradientTex, Color col)
        {
            if (!GUI.enabled)
            {
                col = new Color(0.3f, 0.3f, 0.3f, 1f);
                EditorGUI.showMixedValue = false;
            }
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(10f) };
            Rect position = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none, options);
            GUI.Box(position, GUIContent.none);
            position = new Rect(position.x + 1f, position.y + 1f, position.width - 2f, position.height - 2f);
            if (gradientTex != null)
            {
                GUI.DrawTexture(position, gradientTex);
            }
            else
            {
                EditorGUIUtility.DrawColorSwatch(position, col, false);
            }
            GUILayout.EndVertical();
        }

        private void DrawVertices()
        {
            if (this.SelectionMeshDirty())
            {
                this.GenerateSelectionMesh();
            }
            if (this.state.ToolMode == ToolMode.Select)
            {
                for (int j = 0; j < s_SelectedMaterial.passCount; j++)
                {
                    s_SelectedMaterial.SetPass(j);
                    foreach (Mesh mesh in this.m_SelectedMesh)
                    {
                        Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
                    }
                }
            }
            Material material = !this.state.ManipulateBackfaces ? s_SelectionMaterial : s_SelectionMaterialBackfaces;
            for (int i = 0; i < material.passCount; i++)
            {
                material.SetPass(i);
                foreach (Mesh mesh2 in this.m_SelectionMesh)
                {
                    Graphics.DrawMeshNow(mesh2, Matrix4x4.identity);
                }
            }
            if (this.m_MouseOver != -1)
            {
                Matrix4x4 matrix = Matrix4x4.TRS(this.m_LastVertices[this.m_MouseOver], Quaternion.identity, (Vector3) (Vector3.one * 1.2f));
                if (this.state.ToolMode == ToolMode.Select)
                {
                    material = s_SelectedMaterial;
                    material.color = new Color(s_SelectionColor.r, s_SelectionColor.g, s_SelectionColor.b, 0.5f);
                }
                else
                {
                    int index = this.m_MouseOver / s_MaxVertices;
                    int num6 = this.m_MouseOver - (s_MaxVertices * index);
                    material.color = this.m_SelectionMesh[index].colors[num6];
                }
                for (int k = 0; k < material.passCount; k++)
                {
                    material.SetPass(k);
                    Graphics.DrawMeshNow(this.m_VertexMeshSelected, matrix);
                }
                material.color = Color.white;
            }
        }

        private Texture2D GenerateColorTexture(int width)
        {
            Texture2D textured = new Texture2D(width, 1, TextureFormat.ARGB32, false) {
                hideFlags = HideFlags.HideAndDontSave,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave
            };
            Color[] colors = new Color[width];
            for (int i = 0; i < width; i++)
            {
                colors[i] = this.GetGradientColor(((float) i) / ((float) (width - 1)));
            }
            textured.SetPixels(colors);
            textured.Apply();
            return textured;
        }

        private void GenerateSelectionMesh()
        {
            SkinnedMeshRenderer component = this.cloth.GetComponent<SkinnedMeshRenderer>();
            Vector3[] vertices = this.cloth.vertices;
            int length = vertices.Length;
            this.m_Selection = new bool[vertices.Length];
            this.m_RectSelection = new bool[vertices.Length];
            if (this.m_SelectionMesh != null)
            {
                foreach (Mesh mesh in this.m_SelectionMesh)
                {
                    Object.DestroyImmediate(mesh);
                }
                foreach (Mesh mesh2 in this.m_SelectedMesh)
                {
                    Object.DestroyImmediate(mesh2);
                }
            }
            int num4 = (length / s_MaxVertices) + 1;
            this.m_SelectionMesh = new Mesh[num4];
            this.m_SelectedMesh = new Mesh[num4];
            this.m_LastVertices = new Vector3[length];
            this.m_MeshVerticesPerSelectionVertex = this.m_VertexMesh.vertices.Length;
            Transform actualRootBone = component.actualRootBone;
            for (int i = 0; i < num4; i++)
            {
                this.m_SelectionMesh[i] = new Mesh();
                Mesh mesh1 = this.m_SelectionMesh[i];
                mesh1.hideFlags |= HideFlags.DontSave;
                this.m_SelectedMesh[i] = new Mesh();
                Mesh mesh3 = this.m_SelectedMesh[i];
                mesh3.hideFlags |= HideFlags.DontSave;
                int num6 = length - (i * s_MaxVertices);
                if (num6 > s_MaxVertices)
                {
                    num6 = s_MaxVertices;
                }
                CombineInstance[] combine = new CombineInstance[num6];
                int num7 = i * s_MaxVertices;
                for (int j = 0; j < num6; j++)
                {
                    this.m_LastVertices[num7 + j] = ((Vector3) (actualRootBone.rotation * vertices[num7 + j])) + actualRootBone.position;
                    combine[j].mesh = this.m_VertexMesh;
                    combine[j].transform = Matrix4x4.TRS(this.m_LastVertices[num7 + j], Quaternion.identity, Vector3.one);
                }
                this.m_SelectionMesh[i].CombineMeshes(combine);
                for (int k = 0; k < num6; k++)
                {
                    combine[k].mesh = this.m_VertexMeshSelected;
                }
                this.m_SelectedMesh[i].CombineMeshes(combine);
            }
            this.SetupSelectionMeshColors();
        }

        private Bounds GetClothBounds()
        {
            if (this.target is Cloth)
            {
                SkinnedMeshRenderer component = ((Cloth) this.target).GetComponent<SkinnedMeshRenderer>();
                if (component != null)
                {
                    return component.bounds;
                }
            }
            return new Bounds();
        }

        private float GetCoefficient(ClothSkinningCoefficient coefficient)
        {
            DrawMode drawMode = this.drawMode;
            if (drawMode != DrawMode.MaxDistance)
            {
                if (drawMode == DrawMode.CollisionSphereDistance)
                {
                    return coefficient.collisionSphereDistance;
                }
                return 0f;
            }
            return coefficient.maxDistance;
        }

        private Color GetGradientColor(float val)
        {
            if (val < 0.3f)
            {
                return Color.Lerp(Color.red, Color.magenta, val / 0.2f);
            }
            if (val < 0.7f)
            {
                return Color.Lerp(Color.magenta, Color.yellow, (val - 0.2f) / 0.5f);
            }
            return Color.Lerp(Color.yellow, Color.green, (val - 0.7f) / 0.3f);
        }

        private GUIContent GetModeString(DrawMode mode)
        {
            return s_ModeStrings[(int) mode];
        }

        private int GetMouseVertex(Event e)
        {
            if (Tools.current != Tool.None)
            {
                return -1;
            }
            SkinnedMeshRenderer component = this.cloth.GetComponent<SkinnedMeshRenderer>();
            Vector3[] normals = this.cloth.normals;
            ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            float num = 1000f;
            int num2 = -1;
            Quaternion rotation = component.actualRootBone.rotation;
            for (int i = 0; i < coefficients.Length; i++)
            {
                Vector3 lhs = this.m_LastVertices[i] - ray.origin;
                float sqrMagnitude = Vector3.Cross(lhs, ray.direction).sqrMagnitude;
                if (((Vector3.Dot((Vector3) (rotation * normals[i]), Camera.current.transform.forward) <= 0f) || this.state.ManipulateBackfaces) && ((sqrMagnitude < num) && (sqrMagnitude < 0.0025f)))
                {
                    num = sqrMagnitude;
                    num2 = i;
                }
            }
            return num2;
        }

        private bool IsConstrained()
        {
            foreach (ClothSkinningCoefficient coefficient in this.cloth.coefficients)
            {
                if (coefficient.maxDistance < float.MaxValue)
                {
                    return true;
                }
                if (coefficient.collisionSphereDistance < float.MaxValue)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnDisable()
        {
            if (this.m_SelectionMesh != null)
            {
                foreach (Mesh mesh in this.m_SelectionMesh)
                {
                    Object.DestroyImmediate(mesh);
                }
                foreach (Mesh mesh2 in this.m_SelectedMesh)
                {
                    Object.DestroyImmediate(mesh2);
                }
            }
            Object.DestroyImmediate(this.m_VertexMesh);
            Object.DestroyImmediate(this.m_VertexMeshSelected);
        }

        private void OnEnable()
        {
            if (s_SelectionMaterial == null)
            {
                s_SelectionMaterial = EditorGUIUtility.LoadRequired("SceneView/VertexSelectionMaterial.mat") as Material;
                s_SelectionMaterialBackfaces = EditorGUIUtility.LoadRequired("SceneView/VertexSelectionBackfacesMaterial.mat") as Material;
                s_SelectedMaterial = EditorGUIUtility.LoadRequired("SceneView/VertexSelectedMaterial.mat") as Material;
            }
            if (s_ColorTexture == null)
            {
                s_ColorTexture = this.GenerateColorTexture(100);
            }
            this.m_VertexMesh = new Mesh();
            this.m_VertexMesh.hideFlags |= HideFlags.DontSave;
            Mesh builtinResource = (Mesh) Resources.GetBuiltinResource(typeof(Mesh), "Cube.fbx");
            this.m_VertexMesh.vertices = new Vector3[builtinResource.vertices.Length];
            this.m_VertexMesh.normals = builtinResource.normals;
            Vector4[] vectorArray = new Vector4[builtinResource.vertices.Length];
            Vector3[] vertices = builtinResource.vertices;
            for (int i = 0; i < builtinResource.vertices.Length; i++)
            {
                vectorArray[i] = (Vector4) (vertices[i] * -0.01f);
            }
            this.m_VertexMesh.tangents = vectorArray;
            this.m_VertexMesh.triangles = builtinResource.triangles;
            this.m_VertexMeshSelected = new Mesh();
            this.m_VertexMeshSelected.hideFlags |= HideFlags.DontSave;
            this.m_VertexMeshSelected.vertices = this.m_VertexMesh.vertices;
            this.m_VertexMeshSelected.normals = this.m_VertexMesh.normals;
            for (int j = 0; j < builtinResource.vertices.Length; j++)
            {
                vectorArray[j] = (Vector4) (vertices[j] * -0.02f);
            }
            this.m_VertexMeshSelected.tangents = vectorArray;
            this.m_VertexMeshSelected.triangles = this.m_VertexMesh.triangles;
            s_MaxVertices = 0x10000 / this.m_VertexMesh.vertices.Length;
            this.GenerateSelectionMesh();
            this.SetupSelectedMeshColors();
        }

        public override void OnInspectorGUI()
        {
            EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Cloth, "Edit Constraints", EditorGUIUtility.IconContent("EditCollider"), this.GetClothBounds(), this);
            base.OnInspectorGUI();
        }

        public void OnPreSceneGUI()
        {
            if (this.editing)
            {
                Tools.current = Tool.None;
                if (this.state.ToolMode == ~ToolMode.Select)
                {
                    this.state.ToolMode = ToolMode.Select;
                }
                ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
                if ((this.m_Selection.Length != coefficients.Length) && (this.m_Selection.Length != s_MaxVertices))
                {
                    this.OnEnable();
                }
                Handles.BeginGUI();
                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                Event current = Event.current;
                EventType typeForControl = current.GetTypeForControl(controlID);
                switch (typeForControl)
                {
                    case EventType.MouseMove:
                    case EventType.MouseDrag:
                    {
                        int mouseOver = this.m_MouseOver;
                        this.m_MouseOver = this.GetMouseVertex(current);
                        if (this.m_MouseOver != mouseOver)
                        {
                            SceneView.RepaintAll();
                        }
                        break;
                    }
                    default:
                        if (typeForControl == EventType.Layout)
                        {
                            HandleUtility.AddDefaultControl(controlID);
                        }
                        break;
                }
                switch (this.state.ToolMode)
                {
                    case ToolMode.Select:
                        this.SelectionPreSceneGUI(controlID);
                        break;

                    case ToolMode.Paint:
                        this.PaintPreSceneGUI(controlID);
                        break;
                }
                Handles.EndGUI();
            }
        }

        public void OnSceneGUI()
        {
            if (this.editing && (Selection.gameObjects.Length <= 1))
            {
                s_SelectionColor = GUI.skin.settings.selectionColor;
                if (Event.current.type == EventType.Repaint)
                {
                    this.DrawVertices();
                }
                Event current = Event.current;
                if (current.commandName == "SelectAll")
                {
                    if (current.type == EventType.ValidateCommand)
                    {
                        current.Use();
                    }
                    if (current.type == EventType.ExecuteCommand)
                    {
                        int length = this.cloth.vertices.Length;
                        for (int i = 0; i < length; i++)
                        {
                            this.m_Selection[i] = true;
                        }
                        this.SetupSelectedMeshColors();
                        SceneView.RepaintAll();
                        this.state.ToolMode = ToolMode.Select;
                        current.Use();
                    }
                }
                Handles.BeginGUI();
                if ((this.m_RectSelecting && (this.state.ToolMode == ToolMode.Select)) && (Event.current.type == EventType.Repaint))
                {
                    EditorStyles.selectionRect.Draw(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), GUIContent.none, false, false, false, false);
                }
                Handles.EndGUI();
                SceneViewOverlay.Window(new GUIContent("Cloth Constraints"), new SceneViewOverlay.WindowFunction(this.VertexEditing), 0, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
            }
        }

        private float PaintField(float value, ref bool enabled, DrawMode mode)
        {
            GUIContent modeString = this.GetModeString(mode);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            enabled = GUILayout.Toggle(enabled, s_PaintIcon, "MiniButton", options);
            EditorGUI.BeginDisabledGroup(!enabled);
            EditorGUI.BeginChangeCheck();
            bool flag = EditorGUILayout.Toggle(GUIContent.none, value < float.MaxValue, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                if (flag)
                {
                    value = 0f;
                }
                else
                {
                    value = float.MaxValue;
                }
                this.drawMode = mode;
            }
            GUILayout.Space(-162f);
            EditorGUI.BeginDisabledGroup(!flag);
            float num = value;
            int keyboardControl = GUIUtility.keyboardControl;
            EditorGUI.BeginChangeCheck();
            if (flag)
            {
                num = EditorGUILayout.FloatField(modeString, value, new GUILayoutOption[0]);
            }
            else
            {
                EditorGUILayout.FloatField(modeString, 0f, new GUILayoutOption[0]);
            }
            if (num < 0f)
            {
                num = 0f;
            }
            if (EditorGUI.EndChangeCheck() || (keyboardControl != GUIUtility.keyboardControl))
            {
                this.drawMode = mode;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
            if (flag)
            {
                float num3 = this.m_MinVisualizedValue[(int) mode];
                float num4 = this.m_MaxVisualizedValue[(int) mode];
                if ((num4 - num3) > 0f)
                {
                    this.DrawColorBox(null, this.GetGradientColor((value - num3) / (num4 - num3)));
                }
                else
                {
                    this.DrawColorBox(null, this.GetGradientColor((value > num3) ? ((float) 1) : ((float) 0)));
                }
            }
            else
            {
                this.DrawColorBox(null, Color.black);
            }
            GUILayout.EndHorizontal();
            return num;
        }

        private void PaintGUI()
        {
            this.state.PaintMaxDistance = this.PaintField(this.state.PaintMaxDistance, ref this.state.PaintMaxDistanceEnabled, DrawMode.MaxDistance);
            this.state.PaintCollisionSphereDistance = this.PaintField(this.state.PaintCollisionSphereDistance, ref this.state.PaintCollisionSphereDistanceEnabled, DrawMode.CollisionSphereDistance);
            if (this.state.PaintMaxDistanceEnabled && !this.state.PaintCollisionSphereDistanceEnabled)
            {
                this.drawMode = DrawMode.MaxDistance;
            }
            else if (!this.state.PaintMaxDistanceEnabled && this.state.PaintCollisionSphereDistanceEnabled)
            {
                this.drawMode = DrawMode.CollisionSphereDistance;
            }
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Set constraints to paint onto cloth vertices.", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private void PaintPreSceneGUI(int id)
        {
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(id);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                case EventType.MouseDrag:
                {
                    ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
                    if ((GUIUtility.hotControl != id) && ((current.alt || current.control) || (current.command || (current.button != 0))))
                    {
                        return;
                    }
                    if (typeForControl == EventType.MouseDown)
                    {
                        GUIUtility.hotControl = id;
                    }
                    int mouseVertex = this.GetMouseVertex(current);
                    if (mouseVertex != -1)
                    {
                        bool flag = false;
                        if (this.state.PaintMaxDistanceEnabled && (coefficients[mouseVertex].maxDistance != this.state.PaintMaxDistance))
                        {
                            coefficients[mouseVertex].maxDistance = this.state.PaintMaxDistance;
                            flag = true;
                        }
                        if (this.state.PaintCollisionSphereDistanceEnabled && (coefficients[mouseVertex].collisionSphereDistance != this.state.PaintCollisionSphereDistance))
                        {
                            coefficients[mouseVertex].collisionSphereDistance = this.state.PaintCollisionSphereDistance;
                            flag = true;
                        }
                        if (flag)
                        {
                            Undo.RegisterCompleteObjectUndo(this.target, "Paint Cloth");
                            this.cloth.coefficients = coefficients;
                            this.SetupSelectionMeshColors();
                            base.Repaint();
                        }
                    }
                    current.Use();
                    break;
                }
                default:
                    if (((typeForControl == EventType.MouseUp) && (GUIUtility.hotControl == id)) && (current.button == 0))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;
            }
        }

        private bool RectSelectionModeFromEvent()
        {
            Event current = Event.current;
            RectSelectionMode replace = RectSelectionMode.Replace;
            if (current.shift)
            {
                replace = RectSelectionMode.Add;
            }
            if (current.alt)
            {
                replace = RectSelectionMode.Substract;
            }
            if (this.m_RectSelectionMode != replace)
            {
                this.m_RectSelectionMode = replace;
                return true;
            }
            return false;
        }

        private void SelectionGUI()
        {
            ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
            float maxDistance = 0f;
            float useValue = 0f;
            float collisionSphereDistance = 0f;
            float num4 = 0f;
            int num5 = 0;
            bool flag = true;
            for (int i = 0; i < this.m_Selection.Length; i++)
            {
                if (this.m_Selection[i])
                {
                    if (flag)
                    {
                        maxDistance = coefficients[i].maxDistance;
                        useValue = (maxDistance >= float.MaxValue) ? ((float) 0) : ((float) 1);
                        collisionSphereDistance = coefficients[i].collisionSphereDistance;
                        num4 = (collisionSphereDistance >= float.MaxValue) ? ((float) 0) : ((float) 1);
                        flag = false;
                    }
                    if (coefficients[i].maxDistance != maxDistance)
                    {
                        maxDistance = -1f;
                    }
                    if (coefficients[i].collisionSphereDistance != collisionSphereDistance)
                    {
                        collisionSphereDistance = -1f;
                    }
                    if (useValue != ((coefficients[i].maxDistance >= float.MaxValue) ? ((float) 0) : ((float) 1)))
                    {
                        useValue = -1f;
                    }
                    if (num4 != ((coefficients[i].collisionSphereDistance >= float.MaxValue) ? ((float) 0) : ((float) 1)))
                    {
                        num4 = -1f;
                    }
                    num5++;
                }
            }
            float num7 = this.CoefficientField(maxDistance, useValue, num5 > 0, DrawMode.MaxDistance);
            if (num7 != maxDistance)
            {
                for (int j = 0; j < coefficients.Length; j++)
                {
                    if (this.m_Selection[j])
                    {
                        coefficients[j].maxDistance = num7;
                    }
                }
                this.cloth.coefficients = coefficients;
                this.SetupSelectionMeshColors();
                Undo.RegisterCompleteObjectUndo(this.target, "Change Cloth Coefficients");
            }
            float num9 = this.CoefficientField(collisionSphereDistance, num4, num5 > 0, DrawMode.CollisionSphereDistance);
            if (num9 != collisionSphereDistance)
            {
                for (int k = 0; k < coefficients.Length; k++)
                {
                    if (this.m_Selection[k])
                    {
                        coefficients[k].collisionSphereDistance = num9;
                    }
                }
                this.cloth.coefficients = coefficients;
                this.SetupSelectionMeshColors();
                Undo.RegisterCompleteObjectUndo(this.target, "Change Cloth Coefficients");
            }
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (num5 > 0)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(num5 + " selected", new GUILayoutOption[0]);
            }
            else
            {
                GUILayout.Label("Select cloth vertices to edit their constraints.", new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Backspace))
            {
                for (int m = 0; m < coefficients.Length; m++)
                {
                    if (this.m_Selection[m])
                    {
                        switch (this.drawMode)
                        {
                            case DrawMode.MaxDistance:
                                coefficients[m].maxDistance = float.MaxValue;
                                break;

                            case DrawMode.CollisionSphereDistance:
                                goto Label_02DC;
                        }
                    }
                    continue;
                Label_02DC:
                    coefficients[m].collisionSphereDistance = float.MaxValue;
                }
                this.cloth.coefficients = coefficients;
                this.SetupSelectionMeshColors();
            }
        }

        private bool SelectionMeshDirty()
        {
            SkinnedMeshRenderer component = this.cloth.GetComponent<SkinnedMeshRenderer>();
            Vector3[] vertices = this.cloth.vertices;
            Transform actualRootBone = component.actualRootBone;
            if (this.m_LastVertices.Length != vertices.Length)
            {
                return true;
            }
            for (int i = 0; i < this.m_LastVertices.Length; i++)
            {
                Vector3 vector = ((Vector3) (actualRootBone.rotation * vertices[i])) + actualRootBone.position;
                if (!(this.m_LastVertices[i] == vector))
                {
                    return true;
                }
            }
            return false;
        }

        private void SelectionPreSceneGUI(int id)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (((!current.alt && !current.control) && !current.command) && (current.button == 0))
                    {
                        GUIUtility.hotControl = id;
                        int mouseVertex = this.GetMouseVertex(current);
                        if (mouseVertex != -1)
                        {
                            if (current.shift)
                            {
                                this.m_Selection[mouseVertex] = !this.m_Selection[mouseVertex];
                            }
                            else
                            {
                                for (int i = 0; i < this.m_Selection.Length; i++)
                                {
                                    this.m_Selection[i] = false;
                                }
                                this.m_Selection[mouseVertex] = true;
                            }
                            this.m_DidSelect = true;
                            this.SetupSelectedMeshColors();
                            base.Repaint();
                        }
                        else
                        {
                            this.m_DidSelect = false;
                        }
                        this.m_SelectStartPoint = current.mousePosition;
                        current.Use();
                        return;
                    }
                    return;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && (current.button == 0))
                    {
                        GUIUtility.hotControl = 0;
                        if (!this.m_RectSelecting)
                        {
                            if ((!this.m_DidSelect && !current.alt) && (!current.control && !current.command))
                            {
                                ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
                                for (int j = 0; j < coefficients.Length; j++)
                                {
                                    this.m_Selection[j] = false;
                                }
                            }
                        }
                        else
                        {
                            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendCommandsOnModifierKeys));
                            this.m_RectSelecting = false;
                            this.RectSelectionModeFromEvent();
                            this.ApplyRectSelection();
                        }
                        GUIUtility.keyboardControl = 0;
                        this.SetupSelectedMeshColors();
                        SceneView.RepaintAll();
                    }
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        if (!this.m_RectSelecting)
                        {
                            Vector2 vector = current.mousePosition - this.m_SelectStartPoint;
                            if (((vector.magnitude > 2f) && !current.alt) && (!current.control && !current.command))
                            {
                                EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendCommandsOnModifierKeys));
                                this.m_RectSelecting = true;
                                this.RectSelectionModeFromEvent();
                                this.SetupSelectedMeshColors();
                            }
                        }
                        if (this.m_RectSelecting)
                        {
                            this.m_SelectMousePoint = new Vector2(Mathf.Max(current.mousePosition.x, 0f), Mathf.Max(current.mousePosition.y, 0f));
                            if (this.RectSelectionModeFromEvent() || this.UpdateRectSelection())
                            {
                                this.SetupSelectedMeshColors();
                            }
                            current.Use();
                        }
                    }
                    return;

                case EventType.ExecuteCommand:
                    if ((this.m_RectSelecting && (current.commandName == "ModifierKeysChanged")) && (this.RectSelectionModeFromEvent() || this.UpdateRectSelection()))
                    {
                        this.SetupSelectedMeshColors();
                    }
                    return;
            }
        }

        internal void SendCommandsOnModifierKeys()
        {
            SceneView.lastActiveSceneView.SendEvent(EditorGUIUtility.CommandEvent("ModifierKeysChanged"));
        }

        private void SetupSelectedMeshColors()
        {
            int length = this.cloth.coefficients.Length;
            Color[] colors = new Color[length * this.m_MeshVerticesPerSelectionVertex];
            for (int i = 0; i < length; i++)
            {
                bool flag = this.m_Selection[i];
                if (this.m_RectSelecting)
                {
                    switch (this.m_RectSelectionMode)
                    {
                        case RectSelectionMode.Replace:
                            flag = this.m_RectSelection[i];
                            break;

                        case RectSelectionMode.Add:
                            flag |= this.m_RectSelection[i];
                            break;

                        case RectSelectionMode.Substract:
                            flag = flag && !this.m_RectSelection[i];
                            break;
                    }
                }
                Color color = !flag ? Color.clear : s_SelectionColor;
                for (int j = 0; j < this.m_MeshVerticesPerSelectionVertex; j++)
                {
                    colors[(i * this.m_MeshVerticesPerSelectionVertex) + j] = color;
                }
            }
            this.AssignColorsToMeshArray(colors, this.m_SelectedMesh);
        }

        private void SetupSelectionMeshColors()
        {
            ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
            int length = coefficients.Length;
            Color[] colors = new Color[length * this.m_MeshVerticesPerSelectionVertex];
            float num2 = 0f;
            float num3 = 0f;
            for (int i = 0; i < coefficients.Length; i++)
            {
                float coefficient = this.GetCoefficient(coefficients[i]);
                if (coefficient < float.MaxValue)
                {
                    if (coefficient < num2)
                    {
                        num2 = coefficient;
                    }
                    if (coefficient > num3)
                    {
                        num3 = coefficient;
                    }
                }
            }
            for (int j = 0; j < length; j++)
            {
                Color black;
                float val = this.GetCoefficient(coefficients[j]);
                if (val >= float.MaxValue)
                {
                    black = Color.black;
                }
                else
                {
                    if ((num3 - num2) != 0f)
                    {
                        val = (val - num2) / (num3 - num2);
                    }
                    else
                    {
                        val = 0f;
                    }
                    black = this.GetGradientColor(val);
                }
                for (int k = 0; k < this.m_MeshVerticesPerSelectionVertex; k++)
                {
                    colors[(j * this.m_MeshVerticesPerSelectionVertex) + k] = black;
                }
            }
            this.m_MaxVisualizedValue[(int) this.drawMode] = num3;
            this.m_MinVisualizedValue[(int) this.drawMode] = num2;
            this.AssignColorsToMeshArray(colors, this.m_SelectionMesh);
        }

        private bool UpdateRectSelection()
        {
            bool flag = false;
            SkinnedMeshRenderer component = this.cloth.GetComponent<SkinnedMeshRenderer>();
            Vector3[] normals = this.cloth.normals;
            ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
            float x = Mathf.Min(this.m_SelectStartPoint.x, this.m_SelectMousePoint.x);
            float num2 = Mathf.Max(this.m_SelectStartPoint.x, this.m_SelectMousePoint.x);
            float y = Mathf.Min(this.m_SelectStartPoint.y, this.m_SelectMousePoint.y);
            float num4 = Mathf.Max(this.m_SelectStartPoint.y, this.m_SelectMousePoint.y);
            Ray ray = HandleUtility.GUIPointToWorldRay(new Vector2(x, y));
            Ray ray2 = HandleUtility.GUIPointToWorldRay(new Vector2(num2, y));
            Ray ray3 = HandleUtility.GUIPointToWorldRay(new Vector2(x, num4));
            Ray ray4 = HandleUtility.GUIPointToWorldRay(new Vector2(num2, num4));
            Plane plane = new Plane(ray2.origin + ray2.direction, ray.origin + ray.direction, ray.origin);
            Plane plane2 = new Plane(ray3.origin + ray3.direction, ray4.origin + ray4.direction, ray4.origin);
            Plane plane3 = new Plane(ray.origin + ray.direction, ray3.origin + ray3.direction, ray3.origin);
            Plane plane4 = new Plane(ray4.origin + ray4.direction, ray2.origin + ray2.direction, ray2.origin);
            Quaternion rotation = component.actualRootBone.rotation;
            for (int i = 0; i < coefficients.Length; i++)
            {
                Vector3 inPt = this.m_LastVertices[i];
                bool flag2 = Vector3.Dot((Vector3) (rotation * normals[i]), Camera.current.transform.forward) <= 0f;
                bool flag3 = (((plane.GetSide(inPt) && plane2.GetSide(inPt)) && plane3.GetSide(inPt)) && plane4.GetSide(inPt)) && (this.state.ManipulateBackfaces || flag2);
                if (this.m_RectSelection[i] != flag3)
                {
                    this.m_RectSelection[i] = flag3;
                    flag = true;
                }
            }
            return flag;
        }

        private void VertexEditing(Object unused, SceneView sceneView)
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(300f) };
            GUILayout.BeginVertical(options);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label("Visualization: ", optionArray2);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (EditorGUILayout.ButtonMouseDown(this.GetModeString(this.drawMode), FocusType.Passive, EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
            {
                Rect last = GUILayoutUtility.topLevel.GetLast();
                GenericMenu menu = new GenericMenu();
                menu.AddItem(this.GetModeString(DrawMode.MaxDistance), this.drawMode == DrawMode.MaxDistance, new GenericMenu.MenuFunction(this.VisualizationMenuSetMaxDistanceMode));
                menu.AddItem(this.GetModeString(DrawMode.CollisionSphereDistance), this.drawMode == DrawMode.CollisionSphereDistance, new GenericMenu.MenuFunction(this.VisualizationMenuSetCollisionSphereMode));
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Manipulate Backfaces"), this.state.ManipulateBackfaces, new GenericMenu.MenuFunction(this.VisualizationMenuToggleManipulateBackfaces));
                menu.DropDown(last);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label(this.m_MinVisualizedValue[(int) this.drawMode].ToString(), optionArray3);
            this.DrawColorBox(s_ColorTexture, Color.clear);
            GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label(this.m_MaxVisualizedValue[(int) this.drawMode].ToString(), optionArray4);
            GUILayout.Label("Unconstrained:", new GUILayoutOption[0]);
            GUILayout.Space(-24f);
            GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.Width(20f) };
            GUILayout.BeginHorizontal(optionArray5);
            this.DrawColorBox(null, Color.black);
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
            if (Tools.current != Tool.None)
            {
                this.state.ToolMode = ~ToolMode.Select;
            }
            ToolMode toolMode = this.state.ToolMode;
            this.state.ToolMode = (ToolMode) GUILayout.Toolbar((int) this.state.ToolMode, s_ToolIcons, new GUILayoutOption[0]);
            if (this.state.ToolMode != toolMode)
            {
                GUIUtility.keyboardControl = 0;
                SceneView.RepaintAll();
                this.SetupSelectionMeshColors();
                this.SetupSelectedMeshColors();
            }
            switch (this.state.ToolMode)
            {
                case ToolMode.Select:
                    Tools.current = Tool.None;
                    this.SelectionGUI();
                    break;

                case ToolMode.Paint:
                    Tools.current = Tool.None;
                    this.PaintGUI();
                    break;
            }
            GUILayout.EndVertical();
            if (!this.IsConstrained())
            {
                EditorGUILayout.HelpBox("No constraints have been set up, so the cloth will move freely. Set up vertex constraints here to restrict it.", MessageType.Info);
            }
            GUILayout.EndVertical();
            GUILayout.Space(-4f);
        }

        public void VisualizationMenuSetCollisionSphereMode()
        {
            this.drawMode = DrawMode.CollisionSphereDistance;
            if (!this.state.PaintCollisionSphereDistanceEnabled)
            {
                this.state.PaintCollisionSphereDistanceEnabled = true;
                this.state.PaintMaxDistanceEnabled = false;
            }
        }

        public void VisualizationMenuSetMaxDistanceMode()
        {
            this.drawMode = DrawMode.MaxDistance;
            if (!this.state.PaintMaxDistanceEnabled)
            {
                this.state.PaintCollisionSphereDistanceEnabled = false;
                this.state.PaintMaxDistanceEnabled = true;
            }
        }

        public void VisualizationMenuToggleManipulateBackfaces()
        {
            this.state.ManipulateBackfaces = !this.state.ManipulateBackfaces;
        }

        private Cloth cloth
        {
            get
            {
                return (Cloth) this.target;
            }
        }

        private DrawMode drawMode
        {
            get
            {
                return this.state.DrawMode;
            }
            set
            {
                if (this.state.DrawMode != value)
                {
                    this.state.DrawMode = value;
                    this.SetupSelectionMeshColors();
                    base.Repaint();
                }
            }
        }

        public bool editing
        {
            get
            {
                return ((EditMode.editMode == EditMode.SceneViewEditMode.Cloth) && EditMode.IsOwner(this));
            }
        }

        private ClothInspectorState state
        {
            get
            {
                return ScriptableSingleton<ClothInspectorState>.instance;
            }
        }

        public enum DrawMode
        {
            CollisionSphereDistance = 2,
            MaxDistance = 1
        }

        private enum RectSelectionMode
        {
            Replace,
            Add,
            Substract
        }

        public enum ToolMode
        {
            Select,
            Paint
        }
    }
}

