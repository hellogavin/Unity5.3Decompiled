namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ShapeModuleUI : ModuleUI
    {
        private SerializedProperty m_Angle;
        private SerializedProperty m_Arc;
        private BoxEditor m_BoxEditor;
        private SerializedProperty m_BoxX;
        private SerializedProperty m_BoxY;
        private SerializedProperty m_BoxZ;
        private string[] m_GuiNames;
        private ShapeTypes[] m_GuiTypes;
        private SerializedProperty m_Length;
        private Material m_Material;
        private SerializedProperty m_Mesh;
        private SerializedProperty m_MeshMaterialIndex;
        private SerializedProperty m_MeshNormalOffset;
        private SerializedProperty m_MeshRenderer;
        private SerializedProperty m_PlacementMode;
        private SerializedProperty m_Radius;
        private SerializedProperty m_RandomDirection;
        private SerializedProperty m_SkinnedMeshRenderer;
        private SerializedProperty m_Type;
        private int[] m_TypeToGuiTypeIndex;
        private SerializedProperty m_UseMeshColors;
        private SerializedProperty m_UseMeshMaterialIndex;
        private static int s_BoxHash = "BoxColliderEditor".GetHashCode();
        private static Color s_ShapeGizmoColor = new Color(0.5803922f, 0.8980392f, 1f, 0.9f);
        private static Texts s_Texts = new Texts();

        public ShapeModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ShapeModule", displayName, ModuleUI.VisibilityState.VisibleAndFolded)
        {
            this.m_BoxEditor = new BoxEditor(true, s_BoxHash);
            this.m_GuiNames = new string[] { "Sphere", "Hemisphere", "Cone", "Box", "Mesh", "Mesh Renderer", "Skinned Mesh Renderer", "Circle", "Edge" };
            ShapeTypes[] typesArray1 = new ShapeTypes[9];
            typesArray1[1] = ShapeTypes.Hemisphere;
            typesArray1[2] = ShapeTypes.Cone;
            typesArray1[3] = ShapeTypes.Box;
            typesArray1[4] = ShapeTypes.Mesh;
            typesArray1[5] = ShapeTypes.MeshRenderer;
            typesArray1[6] = ShapeTypes.SkinnedMeshRenderer;
            typesArray1[7] = ShapeTypes.Circle;
            typesArray1[8] = ShapeTypes.SingleSidedEdge;
            this.m_GuiTypes = typesArray1;
            this.m_TypeToGuiTypeIndex = new int[] { 0, 0, 1, 1, 2, 3, 4, 2, 2, 2, 7, 7, 8, 5, 6 };
            base.m_ToolTip = "Shape of the emitter volume, which controls where particles are emitted and their initial direction.";
        }

        private ShapeTypes ConvertConeEmitFromToConeType(int emitFrom)
        {
            if (emitFrom == 0)
            {
                return ShapeTypes.Cone;
            }
            if (emitFrom == 1)
            {
                return ShapeTypes.ConeShell;
            }
            if (emitFrom == 2)
            {
                return ShapeTypes.ConeVolume;
            }
            return ShapeTypes.ConeVolumeShell;
        }

        private int ConvertConeTypeToConeEmitFrom(ShapeTypes shapeType)
        {
            if (shapeType != ShapeTypes.Cone)
            {
                if (shapeType == ShapeTypes.ConeShell)
                {
                    return 1;
                }
                if (shapeType == ShapeTypes.ConeVolume)
                {
                    return 2;
                }
                if (shapeType == ShapeTypes.ConeVolumeShell)
                {
                    return 3;
                }
            }
            return 0;
        }

        private bool GetUsesShell(ShapeTypes shapeType)
        {
            if (((shapeType != ShapeTypes.HemisphereShell) && (shapeType != ShapeTypes.SphereShell)) && (((shapeType != ShapeTypes.ConeShell) && (shapeType != ShapeTypes.ConeVolumeShell)) && (shapeType != ShapeTypes.CircleEdge)))
            {
                return false;
            }
            return true;
        }

        public override float GetXAxisScalar()
        {
            return base.m_ParticleSystemUI.GetEmitterDuration();
        }

        protected override void Init()
        {
            if (this.m_Type == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_Type = base.GetProperty("type");
                this.m_Radius = base.GetProperty("radius");
                this.m_Angle = base.GetProperty("angle");
                this.m_Length = base.GetProperty("length");
                this.m_BoxX = base.GetProperty("boxX");
                this.m_BoxY = base.GetProperty("boxY");
                this.m_BoxZ = base.GetProperty("boxZ");
                this.m_Arc = base.GetProperty("arc");
                this.m_PlacementMode = base.GetProperty("placementMode");
                this.m_Mesh = base.GetProperty("m_Mesh");
                this.m_MeshRenderer = base.GetProperty("m_MeshRenderer");
                this.m_SkinnedMeshRenderer = base.GetProperty("m_SkinnedMeshRenderer");
                this.m_MeshMaterialIndex = base.GetProperty("m_MeshMaterialIndex");
                this.m_UseMeshMaterialIndex = base.GetProperty("m_UseMeshMaterialIndex");
                this.m_UseMeshColors = base.GetProperty("m_UseMeshColors");
                this.m_MeshNormalOffset = base.GetProperty("m_MeshNormalOffset");
                this.m_RandomDirection = base.GetProperty("randomDirection");
                this.m_Material = EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material;
                this.m_BoxEditor.SetAlwaysDisplayHandles(true);
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            int intValue = this.m_Type.intValue;
            int num2 = this.m_TypeToGuiTypeIndex[intValue];
            bool usesShell = this.GetUsesShell((ShapeTypes) intValue);
            int index = ModuleUI.GUIPopup(s_Texts.shape, num2, this.m_GuiNames);
            ShapeTypes types = this.m_GuiTypes[index];
            if (index != num2)
            {
                intValue = (int) types;
            }
            switch (types)
            {
                case ShapeTypes.Sphere:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromShell, usesShell) ? 0 : 1;
                    goto Label_0366;

                case ShapeTypes.Hemisphere:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromShell, usesShell) ? 2 : 3;
                    goto Label_0366;

                case ShapeTypes.Cone:
                {
                    ModuleUI.GUIFloat(s_Texts.coneAngle, this.m_Angle);
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius);
                    bool disabled = (intValue != 8) && (intValue != 9);
                    EditorGUI.BeginDisabledGroup(disabled);
                    ModuleUI.GUIFloat(s_Texts.coneLength, this.m_Length);
                    EditorGUI.EndDisabledGroup();
                    string[] options = new string[] { "Base", "Base Shell", "Volume", "Volume Shell" };
                    int num4 = this.ConvertConeTypeToConeEmitFrom((ShapeTypes) intValue);
                    num4 = ModuleUI.GUIPopup(s_Texts.emitFrom, num4, options);
                    intValue = (int) this.ConvertConeEmitFromToConeType(num4);
                    goto Label_0366;
                }
                case ShapeTypes.Box:
                    ModuleUI.GUIFloat(s_Texts.boxX, this.m_BoxX);
                    ModuleUI.GUIFloat(s_Texts.boxY, this.m_BoxY);
                    ModuleUI.GUIFloat(s_Texts.boxZ, this.m_BoxZ);
                    goto Label_0366;

                case ShapeTypes.Mesh:
                case ShapeTypes.MeshRenderer:
                case ShapeTypes.SkinnedMeshRenderer:
                {
                    string[] strArray2 = new string[] { "Vertex", "Edge", "Triangle" };
                    ModuleUI.GUIPopup(string.Empty, this.m_PlacementMode, strArray2);
                    if (types != ShapeTypes.Mesh)
                    {
                        if (types == ShapeTypes.MeshRenderer)
                        {
                            ModuleUI.GUIObject(s_Texts.meshRenderer, this.m_MeshRenderer);
                        }
                        else
                        {
                            ModuleUI.GUIObject(s_Texts.skinnedMeshRenderer, this.m_SkinnedMeshRenderer);
                        }
                        break;
                    }
                    ModuleUI.GUIObject(s_Texts.mesh, this.m_Mesh);
                    break;
                }
                case ShapeTypes.Circle:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius);
                    ModuleUI.GUIFloat(s_Texts.arc, this.m_Arc);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromEdge, usesShell) ? 10 : 11;
                    goto Label_0366;

                case ShapeTypes.SingleSidedEdge:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius);
                    goto Label_0366;

                default:
                    goto Label_0366;
            }
            ModuleUI.GUIToggleWithIntField(s_Texts.meshMaterialIndex, this.m_UseMeshMaterialIndex, this.m_MeshMaterialIndex, false);
            ModuleUI.GUIToggle(s_Texts.useMeshColors, this.m_UseMeshColors);
            ModuleUI.GUIFloat(s_Texts.meshNormalOffset, this.m_MeshNormalOffset);
        Label_0366:
            this.m_Type.intValue = intValue;
            ModuleUI.GUIToggle(s_Texts.randomDirection, this.m_RandomDirection);
        }

        public override void OnSceneGUI(ParticleSystem s, InitialModuleUI initial)
        {
            Color color = Handles.color;
            Handles.color = s_ShapeGizmoColor;
            Matrix4x4 matrix = Handles.matrix;
            Matrix4x4 transform = new Matrix4x4();
            transform.SetTRS(s.transform.position, s.transform.rotation, s.transform.lossyScale);
            Handles.matrix = transform;
            EditorGUI.BeginChangeCheck();
            int intValue = this.m_Type.intValue;
            switch (intValue)
            {
                case 0:
                case 1:
                    this.m_Radius.floatValue = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue, false);
                    break;
            }
            if ((intValue == 10) || (intValue == 11))
            {
                float floatValue = this.m_Radius.floatValue;
                float arc = this.m_Arc.floatValue;
                Handles.DoSimpleRadiusArcHandleXY(Quaternion.identity, Vector3.zero, ref floatValue, ref arc);
                this.m_Radius.floatValue = floatValue;
                this.m_Arc.floatValue = arc;
            }
            else if ((intValue == 2) || (intValue == 3))
            {
                this.m_Radius.floatValue = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue, true);
            }
            else if ((intValue == 4) || (intValue == 7))
            {
                Vector3 radiusAngleRange = new Vector3(this.m_Radius.floatValue, this.m_Angle.floatValue, initial.m_Speed.scalar.floatValue);
                radiusAngleRange = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange);
                this.m_Radius.floatValue = radiusAngleRange.x;
                this.m_Angle.floatValue = radiusAngleRange.y;
                initial.m_Speed.scalar.floatValue = radiusAngleRange.z;
            }
            else if ((intValue == 8) || (intValue == 9))
            {
                Vector3 vector2 = new Vector3(this.m_Radius.floatValue, this.m_Angle.floatValue, this.m_Length.floatValue);
                vector2 = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, vector2);
                this.m_Radius.floatValue = vector2.x;
                this.m_Angle.floatValue = vector2.y;
                this.m_Length.floatValue = vector2.z;
            }
            else
            {
                switch (intValue)
                {
                    case 5:
                    {
                        Vector3 zero = Vector3.zero;
                        Vector3 size = new Vector3(this.m_BoxX.floatValue, this.m_BoxY.floatValue, this.m_BoxZ.floatValue);
                        if (this.m_BoxEditor.OnSceneGUI(transform, s_ShapeGizmoColor, false, ref zero, ref size))
                        {
                            this.m_BoxX.floatValue = size.x;
                            this.m_BoxY.floatValue = size.y;
                            this.m_BoxZ.floatValue = size.z;
                        }
                        break;
                    }
                    case 12:
                        this.m_Radius.floatValue = Handles.DoSimpleEdgeHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue);
                        break;

                    case 6:
                    {
                        Mesh objectReferenceValue = (Mesh) this.m_Mesh.objectReferenceValue;
                        if (objectReferenceValue != null)
                        {
                            bool wireframe = GL.wireframe;
                            GL.wireframe = true;
                            this.m_Material.SetPass(0);
                            Graphics.DrawMeshNow(objectReferenceValue, s.transform.localToWorldMatrix);
                            GL.wireframe = wireframe;
                        }
                        break;
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
            }
            Handles.color = color;
            Handles.matrix = matrix;
        }

        private enum ShapeTypes
        {
            Sphere,
            SphereShell,
            Hemisphere,
            HemisphereShell,
            Cone,
            Box,
            Mesh,
            ConeShell,
            ConeVolume,
            ConeVolumeShell,
            Circle,
            CircleEdge,
            SingleSidedEdge,
            MeshRenderer,
            SkinnedMeshRenderer
        }

        private class Texts
        {
            public GUIContent arc = new GUIContent("Arc", "Circle arc angle.");
            public GUIContent boxX = new GUIContent("Box X", "Scale of the box in X Axis.");
            public GUIContent boxY = new GUIContent("Box Y", "Scale of the box in Y Axis.");
            public GUIContent boxZ = new GUIContent("Box Z", "Scale of the box in Z Axis.");
            public GUIContent coneAngle = new GUIContent("Angle", "Angle of the cone.");
            public GUIContent coneLength = new GUIContent("Length", "Length of the cone.");
            public GUIContent emitFrom = new GUIContent("Emit from:", "Specifies from where particles are emitted.");
            public GUIContent emitFromEdge = new GUIContent("Emit from Edge", "Emit from edge of the shape. If disabled particles will be emitted from the volume of the shape.");
            public GUIContent emitFromShell = new GUIContent("Emit from Shell", "Emit from shell of the sphere. If disabled particles will be emitted from the volume of the shape.");
            public GUIContent mesh = new GUIContent("Mesh", "Mesh that the particle system will emit from.");
            public GUIContent meshMaterialIndex = new GUIContent("Single Material", "Only emit from a specific material of the mesh.");
            public GUIContent meshNormalOffset = new GUIContent("Normal Offset", "Offset particle spawn positions along the mesh normal.");
            public GUIContent meshRenderer = new GUIContent("Mesh", "MeshRenderer that the particle system will emit from.");
            public GUIContent radius = new GUIContent("Radius", "Radius of the shape.");
            public GUIContent randomDirection = new GUIContent("Random Direction", "Randomizes the starting direction of particles.");
            public GUIContent shape = new GUIContent("Shape", "Defines the shape of the volume from which particles can be emitted, and the direction of the start velocity.");
            public GUIContent skinnedMeshRenderer = new GUIContent("Mesh", "SkinnedMeshRenderer that the particle system will emit from.");
            public GUIContent useMeshColors = new GUIContent("Use Mesh Colors", "Modulate particle color with mesh/material colors.");
        }
    }
}

