namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class RendererModuleUI : ModuleUI
    {
        private const int k_MaxNumMeshes = 4;
        private SerializedProperty m_CameraVelocityScale;
        private SerializedProperty m_CastShadows;
        private SerializedProperty m_LengthScale;
        private SerializedProperty m_Material;
        private SerializedProperty m_MaxParticleSize;
        private SerializedProperty[] m_Meshes;
        private SerializedProperty m_MinParticleSize;
        private SerializedProperty m_NormalDirection;
        private SerializedProperty m_Pivot;
        private RendererEditorBase.Probes m_Probes;
        private SerializedProperty m_ReceiveShadows;
        private SerializedProperty m_RenderAlignment;
        private SerializedProperty m_RenderMode;
        private SerializedProperty[] m_ShownMeshes;
        private SerializedProperty m_SortingFudge;
        private SerializedProperty m_SortingLayerID;
        private SerializedProperty m_SortingOrder;
        private SerializedProperty m_SortMode;
        private SerializedProperty m_VelocityScale;
        private static Texts s_Texts;

        public RendererModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ParticleSystemRenderer", displayName, ModuleUI.VisibilityState.VisibleAndFolded)
        {
            this.m_Meshes = new SerializedProperty[4];
            base.m_ToolTip = "Specifies how the particles are rendered.";
        }

        private void DoListOfMeshesGUI()
        {
            base.GUIListOfFloatObjectToggleFields(s_Texts.mesh, this.m_ShownMeshes, null, null, false);
            Rect position = GUILayoutUtility.GetRect((float) 0f, (float) 13f);
            position.x = (position.xMax - 24f) - 5f;
            position.width = 12f;
            if ((this.m_ShownMeshes.Length > 1) && ModuleUI.MinusButton(position))
            {
                this.m_ShownMeshes[this.m_ShownMeshes.Length - 1].objectReferenceValue = null;
                List<SerializedProperty> list = new List<SerializedProperty>(this.m_ShownMeshes);
                list.RemoveAt(list.Count - 1);
                this.m_ShownMeshes = list.ToArray();
            }
            if (this.m_ShownMeshes.Length < 4)
            {
                position.x += 17f;
                if (ModuleUI.PlusButton(position))
                {
                    List<SerializedProperty> list2;
                    this.m_ShownMeshes = new List<SerializedProperty>(this.m_ShownMeshes) { this.m_Meshes[list2.Count] }.ToArray();
                }
            }
        }

        protected override void Init()
        {
            if (this.m_CastShadows == null)
            {
                this.m_CastShadows = base.GetProperty0("m_CastShadows");
                this.m_ReceiveShadows = base.GetProperty0("m_ReceiveShadows");
                this.m_Material = base.GetProperty0("m_Materials.Array.data[0]");
                this.m_SortingOrder = base.GetProperty0("m_SortingOrder");
                this.m_SortingLayerID = base.GetProperty0("m_SortingLayerID");
                this.m_RenderMode = base.GetProperty0("m_RenderMode");
                this.m_MinParticleSize = base.GetProperty0("m_MinParticleSize");
                this.m_MaxParticleSize = base.GetProperty0("m_MaxParticleSize");
                this.m_CameraVelocityScale = base.GetProperty0("m_CameraVelocityScale");
                this.m_VelocityScale = base.GetProperty0("m_VelocityScale");
                this.m_LengthScale = base.GetProperty0("m_LengthScale");
                this.m_SortingFudge = base.GetProperty0("m_SortingFudge");
                this.m_SortMode = base.GetProperty0("m_SortMode");
                this.m_NormalDirection = base.GetProperty0("m_NormalDirection");
                this.m_Probes = new RendererEditorBase.Probes();
                this.m_Probes.Initialize(base.serializedObject, false);
                this.m_RenderAlignment = base.GetProperty0("m_RenderAlignment");
                this.m_Pivot = base.GetProperty0("m_Pivot");
                this.m_Meshes[0] = base.GetProperty0("m_Mesh");
                this.m_Meshes[1] = base.GetProperty0("m_Mesh1");
                this.m_Meshes[2] = base.GetProperty0("m_Mesh2");
                this.m_Meshes[3] = base.GetProperty0("m_Mesh3");
                List<SerializedProperty> list = new List<SerializedProperty>();
                for (int i = 0; i < this.m_Meshes.Length; i++)
                {
                    if ((i == 0) || (this.m_Meshes[i].objectReferenceValue != null))
                    {
                        list.Add(this.m_Meshes[i]);
                    }
                }
                this.m_ShownMeshes = list.ToArray();
            }
        }

        public bool IsMeshEmitter()
        {
            return ((this.m_RenderMode != null) && (this.m_RenderMode.intValue == 4));
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            RenderMode intValue = (RenderMode) this.m_RenderMode.intValue;
            RenderMode mode2 = (RenderMode) ModuleUI.GUIPopup(s_Texts.renderMode, this.m_RenderMode, s_Texts.particleTypes);
            switch (mode2)
            {
                case RenderMode.Mesh:
                    EditorGUI.indentLevel++;
                    this.DoListOfMeshesGUI();
                    EditorGUI.indentLevel--;
                    if ((intValue != RenderMode.Mesh) && (this.m_Meshes[0].objectReferenceInstanceIDValue == 0))
                    {
                        this.m_Meshes[0].objectReferenceValue = Resources.GetBuiltinResource(typeof(Mesh), "Cube.fbx");
                    }
                    break;

                case RenderMode.Stretch3D:
                    EditorGUI.indentLevel++;
                    ModuleUI.GUIFloat(s_Texts.cameraSpeedScale, this.m_CameraVelocityScale);
                    ModuleUI.GUIFloat(s_Texts.speedScale, this.m_VelocityScale);
                    ModuleUI.GUIFloat(s_Texts.lengthScale, this.m_LengthScale);
                    EditorGUI.indentLevel--;
                    break;
            }
            if (mode2 != RenderMode.Mesh)
            {
                ModuleUI.GUIFloat(s_Texts.normalDirection, this.m_NormalDirection);
            }
            if (this.m_Material != null)
            {
                ModuleUI.GUIObject(s_Texts.material, this.m_Material);
            }
            ModuleUI.GUIPopup(s_Texts.sortMode, this.m_SortMode, s_Texts.sortTypes);
            ModuleUI.GUIFloat(s_Texts.sortingFudge, this.m_SortingFudge);
            ModuleUI.GUIPopup(s_Texts.castShadows, this.m_CastShadows, this.m_CastShadows.enumDisplayNames);
            EditorGUI.BeginDisabledGroup(SceneView.IsUsingDeferredRenderingPath());
            ModuleUI.GUIToggle(s_Texts.receiveShadows, this.m_ReceiveShadows);
            EditorGUI.EndDisabledGroup();
            ModuleUI.GUIFloat(s_Texts.minParticleSize, this.m_MinParticleSize);
            ModuleUI.GUIFloat(s_Texts.maxParticleSize, this.m_MaxParticleSize);
            EditorGUILayout.Space();
            EditorGUILayout.SortingLayerField(s_Texts.sortingLayer, this.m_SortingLayerID, ParticleSystemStyles.Get().popup, ParticleSystemStyles.Get().label);
            ModuleUI.GUIInt(s_Texts.sortingOrder, this.m_SortingOrder);
            if (mode2 == RenderMode.Billboard)
            {
                ModuleUI.GUIPopup(s_Texts.space, this.m_RenderAlignment, s_Texts.spaces);
            }
            EditorGUILayout.PropertyField(this.m_Pivot, s_Texts.pivot, new GUILayoutOption[0]);
            this.m_Probes.OnGUI(null, s.GetComponent<Renderer>(), true);
        }

        private enum RenderMode
        {
            Billboard,
            Stretch3D,
            BillboardFixedHorizontal,
            BillboardFixedVertical,
            Mesh
        }

        private class Texts
        {
            public GUIContent cameraSpeedScale = new GUIContent("Camera Scale", "How much the camera speed is factored in when determining particle stretching.");
            public GUIContent castShadows = new GUIContent("Cast Shadows", "Only opaque materials cast shadows");
            public GUIContent lengthScale = new GUIContent("Length Scale", "Defines the length of the particle compared to its width.");
            public GUIContent material = new GUIContent("Material", "Defines the material used to render particles.");
            public GUIContent maxParticleSize = new GUIContent("Max Particle Size", "How large is a particle allowed to be on screen at most? 1 is entire viewport. 0.5 is half viewport.");
            public GUIContent mesh = new GUIContent("Mesh", "Defines the mesh that will be rendered as particle.");
            public GUIContent minParticleSize = new GUIContent("Min Particle Size", "How small is a particle allowed to be on screen at least? 1 is entire viewport. 0.5 is half viewport.");
            public GUIContent normalDirection = new GUIContent("Normal Direction", "Value between 0.0 and 1.0. If 1.0 is used, normals will point towards camera. If 0.0 is used, normals will point out in the corner direction of the particle.");
            public string[] particleTypes = new string[] { "Billboard", "Stretched Billboard", "Horizontal Billboard", "Vertical Billboard", "Mesh" };
            public GUIContent pivot = new GUIContent("Pivot", "Applies an offset to the pivot of particles.");
            public GUIContent receiveShadows = new GUIContent("Receive Shadows", "Only opaque materials receive shadows");
            public GUIContent renderMode = new GUIContent("Render Mode", "Defines the render mode of the particle renderer.");
            public GUIContent rotation = new GUIContent("Rotation", "Set whether the rotation of the particles is defined in Screen or World space.");
            public GUIContent sortingFudge = new GUIContent("Sorting Fudge", "Lower the number and most likely these particles will appear in front of other transparent objects, including other particles.");
            public GUIContent sortingLayer = EditorGUIUtility.TextContent("Sorting Layer");
            public GUIContent sortingOrder = EditorGUIUtility.TextContent("Order in Layer");
            public GUIContent sortMode = new GUIContent("Sort Mode", "The draw order of particles can be sorted by distance, oldest in front, or youngest in front.");
            public string[] sortTypes = new string[] { "None", "By Distance", "Oldest in Front", "Youngest in Front" };
            public GUIContent space = new GUIContent("Billboard Alignment", "Specifies if the particles will face the camera, align to world axes, or stay local to the system's transform.");
            public string[] spaces = new string[] { "View", "World", "Local" };
            public GUIContent speedScale = new GUIContent("Speed Scale", "Defines the length of the particle compared to its speed.");
        }
    }
}

