namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class AvatarMuscleEditor : AvatarSubEditor
    {
        private const float kLineHeight = 16f;
        private const float kNumberWidth = 38f;
        private const float kPreviewWidth = 80f;
        [SerializeField]
        protected float m_ArmStretchFactor;
        private SerializedProperty m_ArmStretchProperty;
        [SerializeField]
        protected float m_ArmTwistFactor;
        private SerializedProperty m_ArmTwistProperty;
        protected AvatarSetupTool.BoneWrapper[] m_Bones;
        [SerializeField]
        protected float m_FeetSpacingFactor;
        private SerializedProperty m_FeetSpacingProperty;
        private int m_FocusedMuscle;
        [SerializeField]
        protected float m_ForeArmTwistFactor;
        private SerializedProperty m_ForeArmTwistProperty;
        [SerializeField]
        protected bool m_HasTranslationDoF;
        private SerializedProperty m_HasTranslationDoFProperty;
        [SerializeField]
        protected float m_LegStretchFactor;
        private SerializedProperty m_LegStretchProperty;
        [SerializeField]
        protected float m_LegTwistFactor;
        private SerializedProperty m_LegTwistProperty;
        protected int[][] m_MasterMuscle = new int[][] { new int[] { 
            0, 3, 6, 9, 0x12, 0x15, 0x17, 0x1a, 0x1d, 0x1f, 0x22, 0x24, 0x27, 0x29, 0x2b, 0x2d, 
            0x30, 50
         }, new int[] { 1, 4, 7, 10 }, new int[] { 2, 5, 8, 11 }, new int[] { 0x13, 0x18, 0x1b, 0x20, 0x23, 0x25, 0x2a, 0x2c, 0x2e, 0x33 }, new int[] { 20, 0x16, 0x1c, 30, 0x26, 40, 0x2f, 0x31 }, new int[] { 
            0x34, 0x36, 0x37, 0x38, 0x3a, 0x3b, 60, 0x3e, 0x3f, 0x40, 0x42, 0x43, 0x44, 70, 0x47, 0x48, 
            0x4a, 0x4b, 0x4c, 0x4e, 0x4f, 80, 0x52, 0x53, 0x54, 0x56, 0x57, 0x58, 90, 0x5b
         }, new int[] { 0x35, 0x39, 0x3d, 0x41, 0x45, 0x49, 0x4d, 0x51, 0x55, 0x59 } };
        private SerializedProperty[] m_Modified;
        private bool[] m_MuscleBodyGroupToggle;
        private int m_MuscleCount;
        [SerializeField]
        private float[] m_MuscleMasterValue;
        private SerializedProperty[] m_MuscleMax;
        [SerializeField]
        private float[] m_MuscleMaxEdit;
        private SerializedProperty[] m_MuscleMin;
        [SerializeField]
        private float[] m_MuscleMinEdit;
        private string[] m_MuscleName;
        protected int[][] m_Muscles = new int[][] { new int[] { 0, 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0x10, 0x11 }, new int[] { 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 40, 0x29, 0x2a }, new int[] { 
            0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 60, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 
            0x44, 0x45, 70, 0x47
         }, new int[] { 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 0x30, 0x31, 50, 0x33 }, new int[] { 
            0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 80, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 
            0x58, 0x59, 90, 0x5b
         }, new int[] { 0x12, 0x13, 20, 0x15, 0x16, 0x17, 0x18, 0x19 }, new int[] { 0x1a, 0x1b, 0x1c, 0x1d, 30, 0x1f, 0x20, 0x21 } };
        private bool[] m_MuscleToggle;
        [SerializeField]
        private float[] m_MuscleValue;
        [SerializeField]
        protected float m_UpperLegTwistFactor;
        private SerializedProperty m_UpperLegTwistProperty;
        private static Styles s_Styles;
        private const string sArmStretch = "m_HumanDescription.m_ArmStretch";
        private const string sArmTwist = "m_HumanDescription.m_ArmTwist";
        private const string sFeetSpacing = "m_HumanDescription.m_FeetSpacing";
        private const string sForeArmTwist = "m_HumanDescription.m_ForeArmTwist";
        private const string sHasTranslationDoF = "m_HumanDescription.m_HasTranslationDoF";
        private const string sLegStretch = "m_HumanDescription.m_LegStretch";
        private const string sLegTwist = "m_HumanDescription.m_LegTwist";
        private const string sMaxX = "m_Limit.m_Max.x";
        private const string sMaxY = "m_Limit.m_Max.y";
        private const string sMaxZ = "m_Limit.m_Max.z";
        private const string sMinX = "m_Limit.m_Min.x";
        private const string sMinY = "m_Limit.m_Min.y";
        private const string sMinZ = "m_Limit.m_Min.z";
        private const string sModified = "m_Limit.m_Modified";
        private const float sMuscleMax = 180f;
        private const float sMuscleMin = -180f;
        private const string sUpperLegTwist = "m_HumanDescription.m_UpperLegTwist";

        protected void DisplayMuscleButtons()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            GUILayout.BeginHorizontal(string.Empty, styles.toolbar, options);
            Rect position = GUILayoutUtility.GetRect(styles.muscle, styles.toolbarDropDown);
            if (GUI.Button(position, styles.muscle, styles.toolbarDropDown))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(styles.resetMuscle, false, new GenericMenu.MenuFunction(this.ResetMuscleToDefault));
                menu.DropDown(position);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public void DrawMuscleHandle(Transform t, int humanId)
        {
            Vector3 vector2;
            Animator component = base.gameObject.GetComponent(typeof(Animator)) as Animator;
            Avatar avatar = component.avatar;
            int index = HumanTrait.MuscleFromBone(humanId, 0);
            int num2 = HumanTrait.MuscleFromBone(humanId, 1);
            int num3 = HumanTrait.MuscleFromBone(humanId, 2);
            float axisLength = avatar.GetAxisLength(humanId);
            Quaternion preRotation = avatar.GetPreRotation(humanId);
            Quaternion postRotation = avatar.GetPostRotation(humanId);
            preRotation = t.parent.rotation * preRotation;
            postRotation = t.rotation * postRotation;
            Color color = new Color(1f, 1f, 1f, 0.5f);
            Quaternion zYRoll = avatar.GetZYRoll(humanId, Vector3.zero);
            Vector3 limitSign = avatar.GetLimitSign(humanId);
            Vector3 axis = (Vector3) (postRotation * Vector3.right);
            Vector3 vector4 = t.position + ((Vector3) (axis * axisLength));
            Handles.color = Color.white;
            Handles.DrawLine(t.position, vector4);
            if (index != -1)
            {
                Quaternion quaternion4 = avatar.GetZYPostQ(humanId, t.parent.rotation, t.rotation);
                float angle = this.m_MuscleMinEdit[index];
                float num6 = this.m_MuscleMaxEdit[index];
                axis = (Vector3) (postRotation * Vector3.right);
                vector2 = (Vector3) (quaternion4 * Vector3.forward);
                Handles.color = Color.black;
                Vector3 center = t.position + ((Vector3) ((axis * axisLength) * 0.75f));
                axis = (Vector3) ((postRotation * Vector3.right) * limitSign.x);
                vector2 = (Vector3) (Quaternion.AngleAxis(angle, axis) * vector2);
                Handles.color = Color.yellow;
                Handles.color = Handles.xAxisColor * color;
                Handles.DrawSolidArc(center, axis, vector2, num6 - angle, axisLength * 0.25f);
                vector2 = (Vector3) (postRotation * Vector3.forward);
                Handles.color = Handles.centerColor;
                Handles.DrawLine(center, center + ((Vector3) ((vector2 * axisLength) * 0.25f)));
            }
            if (num2 != -1)
            {
                float num7 = this.m_MuscleMinEdit[num2];
                float num8 = this.m_MuscleMaxEdit[num2];
                axis = (Vector3) ((preRotation * Vector3.up) * limitSign.y);
                vector2 = (Vector3) ((preRotation * zYRoll) * Vector3.right);
                Handles.color = Color.black;
                vector2 = (Vector3) (Quaternion.AngleAxis(num7, axis) * vector2);
                Handles.color = Color.yellow;
                Handles.color = Handles.yAxisColor * color;
                Handles.DrawSolidArc(t.position, axis, vector2, num8 - num7, axisLength * 0.25f);
            }
            if (num3 != -1)
            {
                float num9 = this.m_MuscleMinEdit[num3];
                float num10 = this.m_MuscleMaxEdit[num3];
                axis = (Vector3) ((preRotation * Vector3.forward) * limitSign.z);
                vector2 = (Vector3) ((preRotation * zYRoll) * Vector3.right);
                Handles.color = Color.black;
                vector2 = (Vector3) (Quaternion.AngleAxis(num9, axis) * vector2);
                Handles.color = Color.yellow;
                Handles.color = Handles.zAxisColor * color;
                Handles.DrawSolidArc(t.position, axis, vector2, num10 - num9, axisLength * 0.25f);
            }
        }

        public override void Enable(AvatarEditor inspector)
        {
            base.Enable(inspector);
            this.Initialize();
            this.WritePose();
        }

        private static Rect GetPreviewRect(Rect wholeWidthRect)
        {
            wholeWidthRect.width = 71f;
            wholeWidthRect.x += 5f;
            wholeWidthRect.height = 16f;
            return wholeWidthRect;
        }

        private static Rect GetSettingsRect()
        {
            return GetSettingsRect(GUILayoutUtility.GetRect((float) 10f, (float) 16f));
        }

        private static Rect GetSettingsRect(Rect wholeWidthRect)
        {
            wholeWidthRect.xMin += 83f;
            wholeWidthRect.width -= 4f;
            return wholeWidthRect;
        }

        private void HeaderGUI(string h1, string h2)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(80f) };
            GUILayout.Label(h1, styles.title, options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            GUILayout.Label(h2, styles.title, optionArray2);
            GUILayout.EndHorizontal();
        }

        internal void Initialize()
        {
            if (this.m_Bones == null)
            {
                this.m_Bones = AvatarSetupTool.GetHumanBones(base.serializedObject, base.modelBones);
            }
            this.m_FocusedMuscle = -1;
            this.m_MuscleBodyGroupToggle = new bool[this.m_Muscles.Length];
            for (int i = 0; i < this.m_Muscles.Length; i++)
            {
                this.m_MuscleBodyGroupToggle[i] = false;
            }
            this.m_MuscleName = HumanTrait.MuscleName;
            for (int j = 0; j < this.m_MuscleName.Length; j++)
            {
                if (this.m_MuscleName[j].StartsWith("Right"))
                {
                    this.m_MuscleName[j] = this.m_MuscleName[j].Substring(5).Trim();
                }
                if (this.m_MuscleName[j].StartsWith("Left"))
                {
                    this.m_MuscleName[j] = this.m_MuscleName[j].Substring(4).Trim();
                }
            }
            this.m_MuscleCount = HumanTrait.MuscleCount;
            this.m_MuscleToggle = new bool[this.m_MuscleCount];
            this.m_MuscleValue = new float[this.m_MuscleCount];
            this.m_MuscleMin = new SerializedProperty[this.m_MuscleCount];
            this.m_MuscleMax = new SerializedProperty[this.m_MuscleCount];
            this.m_MuscleMinEdit = new float[this.m_MuscleCount];
            this.m_MuscleMaxEdit = new float[this.m_MuscleCount];
            for (int k = 0; k < this.m_MuscleCount; k++)
            {
                this.m_MuscleToggle[k] = false;
                this.m_MuscleValue[k] = 0f;
                this.m_MuscleMin[k] = null;
                this.m_MuscleMin[k] = null;
            }
            this.m_Modified = new SerializedProperty[this.m_Bones.Length];
            for (int m = 0; m < this.m_Bones.Length; m++)
            {
                this.m_Modified[m] = null;
            }
            this.InitializeProperties();
            this.ResetValuesFromProperties();
            this.m_MuscleMasterValue = new float[this.m_MasterMuscle.Length];
            for (int n = 0; n < this.m_MasterMuscle.Length; n++)
            {
                this.m_MuscleMasterValue[n] = 0f;
            }
        }

        internal void InitializeProperties()
        {
            this.m_ArmTwistProperty = base.serializedObject.FindProperty("m_HumanDescription.m_ArmTwist");
            this.m_ForeArmTwistProperty = base.serializedObject.FindProperty("m_HumanDescription.m_ForeArmTwist");
            this.m_UpperLegTwistProperty = base.serializedObject.FindProperty("m_HumanDescription.m_UpperLegTwist");
            this.m_LegTwistProperty = base.serializedObject.FindProperty("m_HumanDescription.m_LegTwist");
            this.m_ArmStretchProperty = base.serializedObject.FindProperty("m_HumanDescription.m_ArmStretch");
            this.m_LegStretchProperty = base.serializedObject.FindProperty("m_HumanDescription.m_LegStretch");
            this.m_FeetSpacingProperty = base.serializedObject.FindProperty("m_HumanDescription.m_FeetSpacing");
            this.m_HasTranslationDoFProperty = base.serializedObject.FindProperty("m_HumanDescription.m_HasTranslationDoF");
            for (int i = 0; i < this.m_Bones.Length; i++)
            {
                SerializedProperty serializedProperty = this.m_Bones[i].GetSerializedProperty(base.serializedObject, false);
                if (serializedProperty != null)
                {
                    this.m_Modified[i] = serializedProperty.FindPropertyRelative("m_Limit.m_Modified");
                    int index = HumanTrait.MuscleFromBone(i, 0);
                    int num3 = HumanTrait.MuscleFromBone(i, 1);
                    int num4 = HumanTrait.MuscleFromBone(i, 2);
                    if (index != -1)
                    {
                        this.m_MuscleMin[index] = serializedProperty.FindPropertyRelative("m_Limit.m_Min.x");
                        this.m_MuscleMax[index] = serializedProperty.FindPropertyRelative("m_Limit.m_Max.x");
                    }
                    if (num3 != -1)
                    {
                        this.m_MuscleMin[num3] = serializedProperty.FindPropertyRelative("m_Limit.m_Min.y");
                        this.m_MuscleMax[num3] = serializedProperty.FindPropertyRelative("m_Limit.m_Max.y");
                    }
                    if (num4 != -1)
                    {
                        this.m_MuscleMin[num4] = serializedProperty.FindPropertyRelative("m_Limit.m_Min.z");
                        this.m_MuscleMax[num4] = serializedProperty.FindPropertyRelative("m_Limit.m_Max.z");
                    }
                }
            }
        }

        protected void MuscleGroupGUI()
        {
            bool flag = false;
            this.HeaderGUI("Preview", "Muscle Group Preview");
            GUILayout.BeginVertical(styles.box, new GUILayoutOption[0]);
            Rect wholeWidthRect = GUILayoutUtility.GetRect((float) 10f, (float) 16f);
            Rect settingsRect = GetSettingsRect(wholeWidthRect);
            if (GUI.Button(GetPreviewRect(wholeWidthRect), "Reset All", EditorStyles.miniButton))
            {
                for (int j = 0; j < this.m_MuscleMasterValue.Length; j++)
                {
                    this.m_MuscleMasterValue[j] = 0f;
                }
                for (int k = 0; k < this.m_MuscleValue.Length; k++)
                {
                    this.m_MuscleValue[k] = 0f;
                }
                flag = true;
            }
            GUI.Label(settingsRect, "Reset All Preview Values", EditorStyles.label);
            for (int i = 0; i < this.m_MasterMuscle.Length; i++)
            {
                Rect rect = GUILayoutUtility.GetRect((float) 10f, (float) 16f);
                Rect position = GetSettingsRect(rect);
                float num4 = this.m_MuscleMasterValue[i];
                this.m_MuscleMasterValue[i] = PreviewSlider(rect, this.m_MuscleMasterValue[i]);
                if (this.m_MuscleMasterValue[i] != num4)
                {
                    Undo.RegisterCompleteObjectUndo(this, "Muscle preview");
                    for (int m = 0; m < this.m_MasterMuscle[i].Length; m++)
                    {
                        if (this.m_MasterMuscle[i][m] != -1)
                        {
                            this.m_MuscleValue[this.m_MasterMuscle[i][m]] = this.m_MuscleMasterValue[i];
                        }
                    }
                }
                flag |= (this.m_MuscleMasterValue[i] != num4) && (base.gameObject != null);
                GUI.Label(position, styles.muscleTypeGroup[i], EditorStyles.label);
            }
            GUILayout.EndVertical();
            if (flag)
            {
                this.WritePose();
            }
        }

        protected void MuscleGUI()
        {
            bool flag = false;
            this.HeaderGUI("Preview", "Per-Muscle Settings");
            GUILayout.BeginVertical(styles.box, new GUILayoutOption[0]);
            for (int i = 0; i < this.m_MuscleBodyGroupToggle.Length; i++)
            {
                Rect settingsRect = GetSettingsRect(GUILayoutUtility.GetRect((float) 10f, (float) 16f));
                this.m_MuscleBodyGroupToggle[i] = GUI.Toggle(settingsRect, this.m_MuscleBodyGroupToggle[i], styles.muscleBodyGroup[i], EditorStyles.foldout);
                if (this.m_MuscleBodyGroupToggle[i])
                {
                    for (int j = 0; j < this.m_Muscles[i].Length; j++)
                    {
                        int index = this.m_Muscles[i][j];
                        if (((index != -1) && (this.m_MuscleMin[index] != null)) && (this.m_MuscleMax[index] != null))
                        {
                            bool flag2 = this.m_MuscleToggle[index];
                            Rect wholeWidthRect = GUILayoutUtility.GetRect(10f, !flag2 ? 16f : 32f);
                            settingsRect = GetSettingsRect(wholeWidthRect);
                            settingsRect.xMin += 15f;
                            settingsRect.height = 16f;
                            this.m_MuscleToggle[index] = GUI.Toggle(settingsRect, this.m_MuscleToggle[index], this.m_MuscleName[index], EditorStyles.foldout);
                            float num5 = PreviewSlider(wholeWidthRect, this.m_MuscleValue[index]);
                            if (this.m_MuscleValue[index] != num5)
                            {
                                Undo.RegisterCompleteObjectUndo(this, "Muscle preview");
                                this.m_FocusedMuscle = index;
                                this.m_MuscleValue[index] = num5;
                                flag |= base.gameObject != null;
                            }
                            if (flag2)
                            {
                                bool flag3 = false;
                                settingsRect.xMin += 15f;
                                settingsRect.y += 16f;
                                Rect position = settingsRect;
                                if (settingsRect.width > 160f)
                                {
                                    Rect rect4 = settingsRect;
                                    rect4.width = 38f;
                                    EditorGUI.BeginChangeCheck();
                                    this.m_MuscleMinEdit[index] = EditorGUI.FloatField(rect4, this.m_MuscleMinEdit[index]);
                                    flag3 |= EditorGUI.EndChangeCheck();
                                    rect4.x = settingsRect.xMax - 38f;
                                    EditorGUI.BeginChangeCheck();
                                    this.m_MuscleMaxEdit[index] = EditorGUI.FloatField(rect4, this.m_MuscleMaxEdit[index]);
                                    flag3 |= EditorGUI.EndChangeCheck();
                                    position.xMin += 43f;
                                    position.xMax -= 43f;
                                }
                                EditorGUI.BeginChangeCheck();
                                EditorGUI.MinMaxSlider(position, ref this.m_MuscleMinEdit[index], ref this.m_MuscleMaxEdit[index], -180f, 180f);
                                if (flag3 | EditorGUI.EndChangeCheck())
                                {
                                    this.m_MuscleMinEdit[index] = Mathf.Clamp(this.m_MuscleMinEdit[index], -180f, 0f);
                                    this.m_MuscleMaxEdit[index] = Mathf.Clamp(this.m_MuscleMaxEdit[index], 0f, 180f);
                                    flag |= this.UpdateMuscle(index, this.m_MuscleMinEdit[index], this.m_MuscleMaxEdit[index]);
                                }
                            }
                        }
                    }
                }
            }
            GUILayout.EndVertical();
            if (flag)
            {
                this.WritePose();
            }
        }

        public override void OnInspectorGUI()
        {
            if ((Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "UndoRedoPerformed"))
            {
                this.WritePose();
            }
            EditorGUI.BeginDisabledGroup(!base.avatarAsset.isHuman);
            EditorGUIUtility.labelWidth = 110f;
            EditorGUIUtility.fieldWidth = 40f;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            this.MuscleGroupGUI();
            EditorGUILayout.Space();
            this.MuscleGUI();
            EditorGUILayout.Space();
            this.PropertiesGUI();
            GUILayout.EndVertical();
            GUILayout.Space(1f);
            GUILayout.EndHorizontal();
            this.DisplayMuscleButtons();
            base.ApplyRevertGUI();
            EditorGUI.EndDisabledGroup();
        }

        public override void OnSceneGUI()
        {
            AvatarSkeletonDrawer.DrawSkeleton(base.root, base.modelBones);
            if (base.gameObject != null)
            {
                Animator component = base.gameObject.GetComponent(typeof(Animator)) as Animator;
                if ((this.m_FocusedMuscle != -1) && (component != null))
                {
                    int humanId = HumanTrait.BoneFromMuscle(this.m_FocusedMuscle);
                    if (humanId != -1)
                    {
                        this.DrawMuscleHandle(this.m_Bones[humanId].bone, humanId);
                    }
                }
            }
        }

        private static float PreviewSlider(Rect position, float val)
        {
            val = GUI.HorizontalSlider(GetPreviewRect(position), val, -1f, 1f);
            if ((val > -0.1f) && (val < 0.1f))
            {
                val = 0f;
            }
            return val;
        }

        protected void PropertiesGUI()
        {
            bool flag = false;
            this.HeaderGUI(string.Empty, "Additional Settings");
            GUILayout.BeginVertical(styles.box, new GUILayoutOption[0]);
            this.m_ArmTwistFactor = EditorGUI.Slider(GetSettingsRect(), styles.armTwist, this.m_ArmTwistFactor, 0f, 1f);
            if (this.m_ArmTwistProperty.floatValue != this.m_ArmTwistFactor)
            {
                Undo.RegisterCompleteObjectUndo(this, "Upper arm twist");
                this.m_ArmTwistProperty.floatValue = this.m_ArmTwistFactor;
                this.UpdateAvatarParameter(HumanParameter.UpperArmTwist, this.m_ArmTwistFactor);
                flag = true;
            }
            this.m_ForeArmTwistFactor = EditorGUI.Slider(GetSettingsRect(), styles.foreArmTwist, this.m_ForeArmTwistFactor, 0f, 1f);
            if (this.m_ForeArmTwistProperty.floatValue != this.m_ForeArmTwistFactor)
            {
                Undo.RegisterCompleteObjectUndo(this, "Lower arm twist");
                this.m_ForeArmTwistProperty.floatValue = this.m_ForeArmTwistFactor;
                this.UpdateAvatarParameter(HumanParameter.LowerArmTwist, this.m_ForeArmTwistFactor);
                flag = true;
            }
            this.m_UpperLegTwistFactor = EditorGUI.Slider(GetSettingsRect(), styles.upperLegTwist, this.m_UpperLegTwistFactor, 0f, 1f);
            if (this.m_UpperLegTwistProperty.floatValue != this.m_UpperLegTwistFactor)
            {
                Undo.RegisterCompleteObjectUndo(this, "Upper leg twist");
                this.m_UpperLegTwistProperty.floatValue = this.m_UpperLegTwistFactor;
                this.UpdateAvatarParameter(HumanParameter.UpperLegTwist, this.m_UpperLegTwistFactor);
                flag = true;
            }
            this.m_LegTwistFactor = EditorGUI.Slider(GetSettingsRect(), styles.legTwist, this.m_LegTwistFactor, 0f, 1f);
            if (this.m_LegTwistProperty.floatValue != this.m_LegTwistFactor)
            {
                Undo.RegisterCompleteObjectUndo(this, "Lower leg twist");
                this.m_LegTwistProperty.floatValue = this.m_LegTwistFactor;
                this.UpdateAvatarParameter(HumanParameter.LowerLegTwist, this.m_LegTwistFactor);
                flag = true;
            }
            this.m_ArmStretchFactor = EditorGUI.Slider(GetSettingsRect(), styles.armStretch, this.m_ArmStretchFactor, 0f, 1f);
            if (this.m_ArmStretchProperty.floatValue != this.m_ArmStretchFactor)
            {
                Undo.RegisterCompleteObjectUndo(this, "Arm stretch");
                this.m_ArmStretchProperty.floatValue = this.m_ArmStretchFactor;
                this.UpdateAvatarParameter(HumanParameter.ArmStretch, this.m_ArmStretchFactor);
                flag = true;
            }
            this.m_LegStretchFactor = EditorGUI.Slider(GetSettingsRect(), styles.legStretch, this.m_LegStretchFactor, 0f, 1f);
            if (this.m_LegStretchProperty.floatValue != this.m_LegStretchFactor)
            {
                Undo.RegisterCompleteObjectUndo(this, "Leg stretch");
                this.m_LegStretchProperty.floatValue = this.m_LegStretchFactor;
                this.UpdateAvatarParameter(HumanParameter.LegStretch, this.m_LegStretchFactor);
                flag = true;
            }
            this.m_FeetSpacingFactor = EditorGUI.Slider(GetSettingsRect(), styles.feetSpacing, this.m_FeetSpacingFactor, 0f, 1f);
            if (this.m_FeetSpacingProperty.floatValue != this.m_FeetSpacingFactor)
            {
                Undo.RegisterCompleteObjectUndo(this, "Feet spacing");
                this.m_FeetSpacingProperty.floatValue = this.m_FeetSpacingFactor;
                this.UpdateAvatarParameter(HumanParameter.FeetSpacing, this.m_FeetSpacingFactor);
                flag = true;
            }
            this.m_HasTranslationDoF = EditorGUI.Toggle(GetSettingsRect(), styles.hasTranslationDoF, this.m_HasTranslationDoF);
            if (this.m_HasTranslationDoFProperty.boolValue != this.m_HasTranslationDoF)
            {
                Undo.RegisterCompleteObjectUndo(this, "Translation DoF");
                this.m_HasTranslationDoFProperty.boolValue = this.m_HasTranslationDoF;
            }
            GUILayout.EndVertical();
            if (flag)
            {
                this.WritePose();
            }
        }

        protected void ResetMuscleToDefault()
        {
            Avatar avatar = null;
            if (base.gameObject != null)
            {
                Animator component = base.gameObject.GetComponent(typeof(Animator)) as Animator;
                avatar = component.avatar;
            }
            for (int i = 0; i < this.m_MuscleCount; i++)
            {
                float muscleDefaultMin = HumanTrait.GetMuscleDefaultMin(i);
                float muscleDefaultMax = HumanTrait.GetMuscleDefaultMax(i);
                if ((this.m_MuscleMin[i] != null) && (this.m_MuscleMax[i] != null))
                {
                    this.m_MuscleMin[i].floatValue = this.m_MuscleMinEdit[i] = muscleDefaultMin;
                    this.m_MuscleMax[i].floatValue = this.m_MuscleMaxEdit[i] = muscleDefaultMax;
                }
                int index = HumanTrait.BoneFromMuscle(i);
                if ((this.m_Modified[index] != null) && (index != -1))
                {
                    this.m_Modified[index].boolValue = false;
                }
                if (avatar != null)
                {
                    avatar.SetMuscleMinMax(i, muscleDefaultMin, muscleDefaultMax);
                }
            }
            this.WritePose();
        }

        protected override void ResetValues()
        {
            base.serializedObject.Update();
            this.ResetValuesFromProperties();
        }

        internal void ResetValuesFromProperties()
        {
            this.m_ArmTwistFactor = this.m_ArmTwistProperty.floatValue;
            this.m_ForeArmTwistFactor = this.m_ForeArmTwistProperty.floatValue;
            this.m_UpperLegTwistFactor = this.m_UpperLegTwistProperty.floatValue;
            this.m_LegTwistFactor = this.m_LegTwistProperty.floatValue;
            this.m_ArmStretchFactor = this.m_ArmStretchProperty.floatValue;
            this.m_LegStretchFactor = this.m_LegStretchProperty.floatValue;
            this.m_FeetSpacingFactor = this.m_FeetSpacingProperty.floatValue;
            this.m_HasTranslationDoF = this.m_HasTranslationDoFProperty.boolValue;
            for (int i = 0; i < this.m_Bones.Length; i++)
            {
                if (this.m_Modified[i] != null)
                {
                    bool boolValue = this.m_Modified[i].boolValue;
                    int index = HumanTrait.MuscleFromBone(i, 0);
                    int num3 = HumanTrait.MuscleFromBone(i, 1);
                    int num4 = HumanTrait.MuscleFromBone(i, 2);
                    if (index != -1)
                    {
                        this.m_MuscleMinEdit[index] = !boolValue ? HumanTrait.GetMuscleDefaultMin(index) : this.m_MuscleMin[index].floatValue;
                        this.m_MuscleMaxEdit[index] = !boolValue ? HumanTrait.GetMuscleDefaultMax(index) : this.m_MuscleMax[index].floatValue;
                    }
                    if (num3 != -1)
                    {
                        this.m_MuscleMinEdit[num3] = !boolValue ? HumanTrait.GetMuscleDefaultMin(num3) : this.m_MuscleMin[num3].floatValue;
                        this.m_MuscleMaxEdit[num3] = !boolValue ? HumanTrait.GetMuscleDefaultMax(num3) : this.m_MuscleMax[num3].floatValue;
                    }
                    if (num4 != -1)
                    {
                        this.m_MuscleMinEdit[num4] = !boolValue ? HumanTrait.GetMuscleDefaultMin(num4) : this.m_MuscleMin[num4].floatValue;
                        this.m_MuscleMaxEdit[num4] = !boolValue ? HumanTrait.GetMuscleDefaultMax(num4) : this.m_MuscleMax[num4].floatValue;
                    }
                }
            }
        }

        protected void UpdateAvatarParameter(HumanParameter parameterId, float value)
        {
            if (base.gameObject != null)
            {
                Animator component = base.gameObject.GetComponent(typeof(Animator)) as Animator;
                component.avatar.SetParameter((int) parameterId, value);
            }
        }

        protected bool UpdateMuscle(int muscleId, float min, float max)
        {
            Undo.RegisterCompleteObjectUndo(this, "Updated muscle range");
            this.m_MuscleMin[muscleId].floatValue = min;
            this.m_MuscleMax[muscleId].floatValue = max;
            int index = HumanTrait.BoneFromMuscle(muscleId);
            if (index != -1)
            {
                this.m_Modified[index].boolValue = true;
            }
            this.m_FocusedMuscle = muscleId;
            if (base.gameObject != null)
            {
                Animator component = base.gameObject.GetComponent(typeof(Animator)) as Animator;
                component.avatar.SetMuscleMinMax(muscleId, min, max);
            }
            SceneView.RepaintAll();
            return (base.gameObject != null);
        }

        protected void WritePose()
        {
            if (base.gameObject != null)
            {
                Animator component = base.gameObject.GetComponent(typeof(Animator)) as Animator;
                if (component != null)
                {
                    Avatar avatar = component.avatar;
                    if (((avatar != null) && avatar.isValid) && avatar.isHuman)
                    {
                        AvatarUtility.SetHumanPose(component, this.m_MuscleValue);
                        SceneView.RepaintAll();
                    }
                }
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

        private class Styles
        {
            public GUIContent armStretch = EditorGUIUtility.TextContent("Arm Stretch");
            public GUIContent armTwist = EditorGUIUtility.TextContent("Upper Arm Twist");
            public GUIStyle box = new GUIStyle("OL box noexpand");
            public GUIContent feetSpacing = EditorGUIUtility.TextContent("Feet Spacing");
            public GUIContent foreArmTwist = EditorGUIUtility.TextContent("Lower Arm Twist");
            public GUIContent hasTranslationDoF = EditorGUIUtility.TextContent("Translation DoF");
            public GUIContent legStretch = EditorGUIUtility.TextContent("Leg Stretch");
            public GUIContent legTwist = EditorGUIUtility.TextContent("Lower Leg Twist");
            public GUIContent muscle = EditorGUIUtility.TextContent("Muscles");
            public GUIContent[] muscleBodyGroup = new GUIContent[] { EditorGUIUtility.TextContent("Body"), EditorGUIUtility.TextContent("Head"), EditorGUIUtility.TextContent("Left Arm"), EditorGUIUtility.TextContent("Left Fingers"), EditorGUIUtility.TextContent("Right Arm"), EditorGUIUtility.TextContent("Right Fingers"), EditorGUIUtility.TextContent("Left Leg"), EditorGUIUtility.TextContent("Right Leg") };
            public GUIContent[] muscleTypeGroup = new GUIContent[] { EditorGUIUtility.TextContent("Open Close"), EditorGUIUtility.TextContent("Left Right"), EditorGUIUtility.TextContent("Roll Left Right"), EditorGUIUtility.TextContent("In Out"), EditorGUIUtility.TextContent("Roll In Out"), EditorGUIUtility.TextContent("Finger Open Close"), EditorGUIUtility.TextContent("Finger In Out") };
            public GUIContent resetMuscle = EditorGUIUtility.TextContent("Reset");
            public GUIStyle title = new GUIStyle("OL TITLE");
            public GUIStyle toolbar = "TE Toolbar";
            public GUIStyle toolbarDropDown = "TE ToolbarDropDown";
            public GUIContent upperLegTwist = EditorGUIUtility.TextContent("Upper Leg Twist");

            public Styles()
            {
                this.box.padding = new RectOffset(0, 0, 4, 4);
            }
        }
    }
}

