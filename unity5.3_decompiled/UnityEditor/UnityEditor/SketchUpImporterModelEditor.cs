namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SketchUpImporterModelEditor : ModelImporterModelEditor
    {
        private const float kInchToMeter = 0.0254f;
        private float lengthToUnit;
        private SerializedProperty m_FileUnit;
        private SerializedProperty m_GenerateBackFace;
        private SerializedProperty m_GlobalScale;
        private SerializedProperty m_Latitude;
        private SerializedProperty m_Longitude;
        private SerializedProperty m_MergeCoplanarFaces;
        private SerializedProperty m_NorthCorrection;
        private SerializedProperty m_SelectedNodes;
        private SketchUpImporter m_Target;

        private static float ConvertGlobalScaleToUnit(EFileUnit measurement, float globalScale)
        {
            switch (measurement)
            {
                case EFileUnit.Meters:
                    return (globalScale / 0.0254f);

                case EFileUnit.Centimeters:
                    return (globalScale / 0.000254f);

                case EFileUnit.Millimeters:
                    return (globalScale / 2.54E-05f);

                case EFileUnit.Feet:
                    return (globalScale / 0.00774192f);

                case EFileUnit.Inches:
                    return globalScale;
            }
            Debug.LogError("File Unit value is invalid");
            return 1f;
        }

        private static float CovertUnitToGlobalScale(EFileUnit measurement, float unit)
        {
            switch (measurement)
            {
                case EFileUnit.Meters:
                    return (0.0254f * unit);

                case EFileUnit.Centimeters:
                    return (0.000254f * unit);

                case EFileUnit.Millimeters:
                    return (unit * 2.54E-05f);

                case EFileUnit.Feet:
                    return (unit * 0.00774192f);

                case EFileUnit.Inches:
                    return unit;
            }
            Debug.LogError("File Unit value is invalid");
            return 1f;
        }

        internal override void OnEnable()
        {
            this.m_GenerateBackFace = base.serializedObject.FindProperty("m_GenerateBackFace");
            this.m_MergeCoplanarFaces = base.serializedObject.FindProperty("m_MergeCoplanarFaces");
            this.m_FileUnit = base.serializedObject.FindProperty("m_FileUnit");
            this.m_GlobalScale = base.serializedObject.FindProperty("m_GlobalScale");
            this.m_Latitude = base.serializedObject.FindProperty("m_Latitude");
            this.m_Longitude = base.serializedObject.FindProperty("m_Longitude");
            this.m_NorthCorrection = base.serializedObject.FindProperty("m_NorthCorrection");
            this.m_SelectedNodes = base.serializedObject.FindProperty("m_SelectedNodes");
            this.m_Target = this.target as SketchUpImporter;
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label(Styles.sketchUpLabel, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_GenerateBackFace, Styles.generateBackFaceLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_MergeCoplanarFaces, Styles.mergeCoplanarFaces, new GUILayoutOption[0]);
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(EditorGUIUtility.labelWidth) };
            GUILayout.Label(Styles.fileUnitLabel, options);
            GUILayout.Label("1", new GUILayoutOption[0]);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MaxWidth(100f) };
            EditorGUILayout.Popup(this.m_FileUnit, Styles.measurementOptions, GUIContent.Temp(string.Empty), optionArray2);
            this.lengthToUnit = ConvertGlobalScaleToUnit((EFileUnit) this.m_FileUnit.intValue, this.m_GlobalScale.floatValue);
            GUILayout.Label("=", new GUILayoutOption[0]);
            this.lengthToUnit = EditorGUILayout.FloatField(this.lengthToUnit, new GUILayoutOption[0]);
            this.m_GlobalScale.floatValue = CovertUnitToGlobalScale((EFileUnit) this.m_FileUnit.intValue, this.lengthToUnit);
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField(Styles.longitudeLabel, this.m_Longitude.floatValue, new GUILayoutOption[0]);
            EditorGUILayout.FloatField(Styles.latitudeLabel, this.m_Latitude.floatValue, new GUILayoutOption[0]);
            EditorGUILayout.FloatField(Styles.northCorrectionLabel, this.m_NorthCorrection.floatValue, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button(Styles.selectNodeButton, new GUILayoutOption[0]))
            {
                SketchUpImportDlg.Launch(this.m_Target.GetNodes(), this);
                GUIUtility.ExitGUI();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            base.OnInspectorGUI();
        }

        public void SetSelectedNodes(int[] selectedNodes)
        {
            if (selectedNodes != null)
            {
                this.m_SelectedNodes.ClearArray();
                for (int i = 0; i < selectedNodes.Length; i++)
                {
                    this.m_SelectedNodes.InsertArrayElementAtIndex(i);
                    this.m_SelectedNodes.GetArrayElementAtIndex(i).intValue = selectedNodes[i];
                }
            }
        }

        private enum EFileUnit
        {
            Meters,
            Centimeters,
            Millimeters,
            Feet,
            Inches
        }

        private class Styles
        {
            public static readonly GUIContent fileUnitLabel = EditorGUIUtility.TextContent("Unit conversion|Length measurement to unit conversion. The value in Scale Factor is calculated based on the value here");
            public static readonly GUIContent generateBackFaceLabel = EditorGUIUtility.TextContent("Generate Back Face|Enable/disable generation of back facing polygons");
            public static readonly GUIContent latitudeLabel = EditorGUIUtility.TextContent("Latitude|Latitude Geo-location");
            public static readonly GUIContent longitudeLabel = EditorGUIUtility.TextContent("Longitude|Longitude Geo-location");
            public static readonly GUIContent[] measurementOptions = new GUIContent[] { EditorGUIUtility.TextContent("Meters"), EditorGUIUtility.TextContent("Centimeters"), EditorGUIUtility.TextContent("Millimeters"), EditorGUIUtility.TextContent("Feet"), EditorGUIUtility.TextContent("Inches") };
            public static readonly GUIContent mergeCoplanarFaces = EditorGUIUtility.TextContent("Merge Coplanar Faces|Enable/disable merging of coplanar faces when generating meshes");
            public static readonly GUIContent northCorrectionLabel = EditorGUIUtility.TextContent("North Correction|The angle which will rotate the north direction to the z-axis for a the model");
            public static readonly GUIContent selectNodeButton = EditorGUIUtility.TextContent("Select Nodes...|Brings up the node selection dialog box");
            public static readonly GUIContent sketchUpLabel = EditorGUIUtility.TextContent("SketchUp|SketchUp import settings");
        }
    }
}

