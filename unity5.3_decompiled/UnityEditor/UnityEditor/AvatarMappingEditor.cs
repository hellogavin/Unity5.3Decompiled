namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable]
    internal class AvatarMappingEditor : AvatarSubEditor
    {
        protected bool[] m_BodyPartFoldout;
        protected int[][] m_BodyPartHumanBone;
        protected bool[] m_BodyPartToggle;
        protected int m_BodyView;
        [SerializeField]
        protected AvatarSetupTool.BoneWrapper[] m_Bones;
        private Editor m_CurrentTransformEditor;
        private bool m_CurrentTransformEditorFoldout;
        private Vector2 m_FoldoutScroll;
        protected bool m_HasSkinnedMesh;
        private bool m_IsBiped;
        internal static bool s_DirtySelection;
        internal static int s_SelectedBoneIndex = -1;
        private static Styles s_Styles;

        public AvatarMappingEditor()
        {
            int[][] numArrayArray1 = new int[9][];
            numArrayArray1[0] = new int[] { -1 };
            int[] numArray2 = new int[3];
            numArray2[1] = 7;
            numArray2[2] = 8;
            numArrayArray1[1] = numArray2;
            numArrayArray1[2] = new int[] { 9, 10, 0x15, 0x16, 0x17 };
            numArrayArray1[3] = new int[] { 11, 13, 15, 0x11 };
            numArrayArray1[4] = new int[] { 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 30, 0x1f, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26 };
            numArrayArray1[5] = new int[] { 12, 14, 0x10, 0x12 };
            numArrayArray1[6] = new int[] { 0x27, 40, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 0x30, 0x31, 50, 0x33, 0x34, 0x35 };
            numArrayArray1[7] = new int[] { 1, 3, 5, 0x13 };
            numArrayArray1[8] = new int[] { 2, 4, 6, 20 };
            this.m_BodyPartHumanBone = numArrayArray1;
            this.m_FoldoutScroll = Vector2.zero;
            if (AvatarSetupTool.sHumanParent.Length != HumanTrait.BoneCount)
            {
                throw new Exception("Avatar's Human parent list is out of sync");
            }
            this.m_BodyPartToggle = new bool[9];
            this.m_BodyPartFoldout = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                this.m_BodyPartToggle[i] = false;
                this.m_BodyPartFoldout[i] = true;
            }
        }

        protected void ApplyTemplate()
        {
            Undo.RegisterCompleteObjectUndo(this, "Apply Template");
            HumanTemplate template = this.OpenHumanTemplate();
            if (template != null)
            {
                for (int i = 0; i < this.m_Bones.Length; i++)
                {
                    <ApplyTemplate>c__AnonStorey96 storey = new <ApplyTemplate>c__AnonStorey96 {
                        boneName = template.Find(this.m_Bones[i].humanBoneName)
                    };
                    if (storey.boneName.Length > 0)
                    {
                        Transform transform = base.modelBones.Keys.FirstOrDefault<Transform>(new Func<Transform, bool>(storey.<>m__1C0));
                        this.m_Bones[i].bone = transform;
                    }
                    else
                    {
                        this.m_Bones[i].bone = null;
                    }
                    this.m_Bones[i].Serialize(base.serializedObject);
                }
                this.ValidateMapping();
                SceneView.RepaintAll();
            }
        }

        protected void AutoMapping()
        {
            foreach (KeyValuePair<int, Transform> pair in AvatarAutoMapper.MapBones(base.gameObject.transform, base.modelBones))
            {
                AvatarSetupTool.BoneWrapper wrapper = this.m_Bones[pair.Key];
                wrapper.bone = pair.Value;
                wrapper.Serialize(base.serializedObject);
            }
        }

        protected void BipedMapping()
        {
            foreach (KeyValuePair<int, Transform> pair in AvatarBipedMapper.MapBones(base.gameObject.transform))
            {
                AvatarSetupTool.BoneWrapper wrapper = this.m_Bones[pair.Key];
                wrapper.bone = pair.Value;
                wrapper.Serialize(base.serializedObject);
            }
        }

        protected void BipedPose()
        {
            AvatarBipedMapper.BipedPose(base.gameObject);
            AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
            base.m_Inspector.Repaint();
        }

        protected void ClearMapping()
        {
            if (base.serializedObject != null)
            {
                Undo.RegisterCompleteObjectUndo(this, "Clear Mapping");
                AvatarSetupTool.ClearHumanBoneArray(base.serializedObject);
                this.ResetBones();
                this.ValidateMapping();
                SceneView.RepaintAll();
            }
        }

        protected void CopyPrefabPose()
        {
            AvatarSetupTool.CopyPose(base.gameObject, base.prefab);
            AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
            base.m_Inspector.Repaint();
        }

        protected void DebugPoseButtons()
        {
            if (GUILayout.Button("Default Pose", new GUILayoutOption[0]) && this.IsValidHuman())
            {
                base.gameObject.GetComponent<Animator>().WriteDefaultPose();
            }
            if (GUILayout.Button("Description Pose", new GUILayoutOption[0]))
            {
                AvatarSetupTool.TransferDescriptionToPose(base.serializedObject, base.root);
            }
        }

        public override void Disable()
        {
            if (this.m_CurrentTransformEditor != null)
            {
                Object.DestroyImmediate(this.m_CurrentTransformEditor);
            }
            base.Disable();
        }

        protected void DisplayFoldout()
        {
            Dictionary<Transform, bool> modelBones = base.modelBones;
            EditorGUIUtility.SetIconSize((Vector2) (Vector2.one * 16f));
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUI.color = Color.grey;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label(styles.dotFrameDotted.image, options);
            GUI.color = Color.white;
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            GUILayout.Label("Optional Bone", optionArray2);
            EditorGUILayout.EndHorizontal();
            for (int i = 1; i < this.m_BodyPartToggle.Length; i++)
            {
                if (this.m_BodyPartToggle[i])
                {
                    if (s_DirtySelection && !this.m_BodyPartFoldout[i])
                    {
                        for (int j = 0; j < this.m_BodyPartHumanBone[i].Length; j++)
                        {
                            int num3 = this.m_BodyPartHumanBone[i][j];
                            if (s_SelectedBoneIndex == num3)
                            {
                                this.m_BodyPartFoldout[i] = true;
                            }
                        }
                    }
                    this.m_BodyPartFoldout[i] = GUILayout.Toggle(this.m_BodyPartFoldout[i], styles.BodyPartMapping[i], EditorStyles.foldout, new GUILayoutOption[0]);
                    EditorGUI.indentLevel++;
                    if (this.m_BodyPartFoldout[i])
                    {
                        for (int k = 0; k < this.m_BodyPartHumanBone[i].Length; k++)
                        {
                            int index = this.m_BodyPartHumanBone[i][k];
                            if (index != -1)
                            {
                                AvatarSetupTool.BoneWrapper wrapper = this.m_Bones[index];
                                string humanBoneName = wrapper.humanBoneName;
                                switch (i)
                                {
                                    case 5:
                                    case 6:
                                    case 8:
                                        humanBoneName = humanBoneName.Replace("Right", string.Empty);
                                        break;
                                }
                                if (((i == 3) || (i == 4)) || (i == 7))
                                {
                                    humanBoneName = humanBoneName.Replace("Left", string.Empty);
                                }
                                humanBoneName = ObjectNames.NicifyVariableName(humanBoneName);
                                Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
                                Rect selectRect = controlRect;
                                selectRect.width -= 15f;
                                wrapper.HandleClickSelection(selectRect, index);
                                wrapper.BoneDotGUI(new Rect(controlRect.x + EditorGUI.indent, controlRect.y - 1f, 19f, 19f), index, false, false, base.serializedObject, this);
                                controlRect.xMin += 19f;
                                Transform key = EditorGUI.ObjectField(controlRect, new GUIContent(humanBoneName), wrapper.bone, typeof(Transform), true) as Transform;
                                if (key != wrapper.bone)
                                {
                                    Undo.RegisterCompleteObjectUndo(this, "Avatar mapping modified");
                                    wrapper.bone = key;
                                    wrapper.Serialize(base.serializedObject);
                                    if ((key != null) && !modelBones.ContainsKey(key))
                                    {
                                        modelBones[key] = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(wrapper.error))
                                {
                                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                                    GUILayout.Space((EditorGUI.indent + 19f) + 4f);
                                    GUILayout.Label(wrapper.error, s_Styles.errorLabel, new GUILayoutOption[0]);
                                    GUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            s_DirtySelection = false;
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        protected void DisplayMappingButtons()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            GUILayout.BeginHorizontal(string.Empty, styles.toolbar, options);
            Rect position = GUILayoutUtility.GetRect(styles.mapping, styles.toolbarDropDown);
            if (GUI.Button(position, styles.mapping, styles.toolbarDropDown))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(styles.clearMapping, false, new GenericMenu.MenuFunction(this.ClearMapping));
                if (this.m_IsBiped)
                {
                    menu.AddItem(styles.bipedMapping, false, new GenericMenu.MenuFunction(this.PerformBipedMapping));
                }
                menu.AddItem(styles.autoMapping, false, new GenericMenu.MenuFunction(this.PerformAutoMapping));
                menu.AddItem(styles.loadMapping, false, new GenericMenu.MenuFunction(this.ApplyTemplate));
                menu.AddItem(styles.saveMapping, false, new GenericMenu.MenuFunction(this.SaveHumanTemplate));
                menu.DropDown(position);
            }
            position = GUILayoutUtility.GetRect(styles.pose, styles.toolbarDropDown);
            if (GUI.Button(position, styles.pose, styles.toolbarDropDown))
            {
                GenericMenu menu2 = new GenericMenu();
                menu2.AddItem(styles.resetPose, false, new GenericMenu.MenuFunction(this.CopyPrefabPose));
                if (this.m_IsBiped)
                {
                    menu2.AddItem(styles.bipedPose, false, new GenericMenu.MenuFunction(this.BipedPose));
                }
                if (this.m_HasSkinnedMesh)
                {
                    menu2.AddItem(styles.sampleBindPose, false, new GenericMenu.MenuFunction(this.SampleBindPose));
                }
                else
                {
                    menu2.AddItem(styles.sampleBindPose, false, null);
                }
                menu2.AddItem(styles.enforceTPose, false, new GenericMenu.MenuFunction(this.MakePoseValid));
                menu2.DropDown(position);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public override void Enable(AvatarEditor inspector)
        {
            base.Enable(inspector);
            this.Init();
        }

        private void EnableBodyParts(bool[] toggles, params int[] parts)
        {
            for (int i = 0; i < this.m_BodyPartToggle.Length; i++)
            {
                toggles[i] = false;
            }
            foreach (int num2 in parts)
            {
                toggles[num2] = true;
            }
        }

        protected BoneState GetBoneState(int i, out string error)
        {
            <GetBoneState>c__AnonStorey95 storey = new <GetBoneState>c__AnonStorey95();
            error = string.Empty;
            storey.bone = this.m_Bones[i];
            if (storey.bone.bone == null)
            {
                return BoneState.None;
            }
            AvatarSetupTool.BoneWrapper wrapper = this.m_Bones[AvatarSetupTool.GetFirstHumanBoneAncestor(this.m_Bones, i)];
            if ((i == 0) && (storey.bone.bone.parent == null))
            {
                error = storey.bone.messageName + " cannot be the root transform";
                return BoneState.InvalidHierarchy;
            }
            if ((wrapper.bone != null) && !storey.bone.bone.IsChildOf(wrapper.bone))
            {
                error = storey.bone.messageName + " is not a child of " + wrapper.messageName + ".";
                return BoneState.InvalidHierarchy;
            }
            if (((i != 0x17) && (wrapper.bone != null)) && (wrapper.bone != storey.bone.bone))
            {
                Vector3 vector = storey.bone.bone.position - wrapper.bone.position;
                if (vector.sqrMagnitude < Mathf.Epsilon)
                {
                    error = storey.bone.messageName + " has bone length of zero.";
                    return BoneState.BoneLenghtIsZero;
                }
            }
            if (this.m_Bones.Where<AvatarSetupTool.BoneWrapper>(new Func<AvatarSetupTool.BoneWrapper, bool>(storey.<>m__1BF)).Count<AvatarSetupTool.BoneWrapper>() <= 1)
            {
                return BoneState.Valid;
            }
            error = storey.bone.messageName + " is also assigned to ";
            bool flag = true;
            for (int j = 0; j < this.m_Bones.Length; j++)
            {
                if ((i != j) && (this.m_Bones[i].bone == this.m_Bones[j].bone))
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        error = error + ", ";
                    }
                    error = error + ObjectNames.NicifyVariableName(this.m_Bones[j].humanBoneName);
                }
            }
            error = error + ".";
            return BoneState.Duplicate;
        }

        private void HandleBodyView(int bodyView)
        {
            if (bodyView == 0)
            {
                this.EnableBodyParts(this.m_BodyPartToggle, new int[] { 1, 3, 5, 7, 8 });
            }
            if (bodyView == 1)
            {
                int[] parts = new int[] { 2 };
                this.EnableBodyParts(this.m_BodyPartToggle, parts);
            }
            if (bodyView == 2)
            {
                int[] numArray2 = new int[] { 4 };
                this.EnableBodyParts(this.m_BodyPartToggle, numArray2);
            }
            if (bodyView == 3)
            {
                int[] numArray3 = new int[] { 6 };
                this.EnableBodyParts(this.m_BodyPartToggle, numArray3);
            }
        }

        protected void Init()
        {
            if (base.gameObject != null)
            {
                this.m_IsBiped = AvatarBipedMapper.IsBiped(base.gameObject.transform, null);
                if (this.m_Bones == null)
                {
                    this.m_Bones = AvatarSetupTool.GetHumanBones(base.serializedObject, base.modelBones);
                }
                this.ValidateMapping();
                if (this.m_CurrentTransformEditor != null)
                {
                    Object.DestroyImmediate(this.m_CurrentTransformEditor);
                    this.m_CurrentTransformEditor = null;
                }
                this.m_CurrentTransformEditorFoldout = true;
                this.m_HasSkinnedMesh = base.gameObject.GetComponentInChildren<SkinnedMeshRenderer>() != null;
                this.InitPose();
                SceneView.RepaintAll();
            }
        }

        protected void InitPose()
        {
            if (this.IsValidHuman())
            {
                base.gameObject.GetComponent<Animator>().WriteDefaultPose();
                AvatarSetupTool.TransferDescriptionToPose(base.serializedObject, base.root);
            }
        }

        protected bool IsAnyBodyPartActive()
        {
            for (int i = 1; i < this.m_BodyPartToggle.Length; i++)
            {
                if (this.m_BodyPartToggle[i])
                {
                    return true;
                }
            }
            return false;
        }

        protected AvatarControl.BodyPartColor IsValidBodyPart(BodyPart bodyPart)
        {
            AvatarControl.BodyPartColor off = AvatarControl.BodyPartColor.Off;
            bool flag = false;
            int index = (int) bodyPart;
            if ((bodyPart != BodyPart.LeftFingers) && (bodyPart != BodyPart.RightFingers))
            {
                for (int i = 0; i < this.m_BodyPartHumanBone[index].Length; i++)
                {
                    if (this.m_BodyPartHumanBone[index][i] != -1)
                    {
                        BoneState state = this.m_Bones[this.m_BodyPartHumanBone[index][i]].state;
                        flag |= state == BoneState.Valid;
                        if (HumanTrait.RequiredBone(this.m_BodyPartHumanBone[index][i]) && (state == BoneState.None))
                        {
                            return AvatarControl.BodyPartColor.Red;
                        }
                        if ((state != BoneState.Valid) && (state != BoneState.None))
                        {
                            return AvatarControl.BodyPartColor.Red;
                        }
                    }
                }
            }
            else
            {
                bool flag2 = true;
                int num4 = 3;
                for (int j = 0; j < (this.m_BodyPartHumanBone[index].Length / num4); j++)
                {
                    bool flag3 = false;
                    int num5 = j * num4;
                    for (int k = num4 - 1; k >= 0; k--)
                    {
                        bool flag4 = this.m_Bones[this.m_BodyPartHumanBone[index][num5 + k]].state == BoneState.Valid;
                        flag2 &= flag4;
                        if (flag3)
                        {
                            if (!flag4)
                            {
                                return (AvatarControl.BodyPartColor.IKRed | AvatarControl.BodyPartColor.Red);
                            }
                        }
                        else
                        {
                            flag |= flag3 = !flag3 && flag4;
                        }
                    }
                }
                off = !flag2 ? AvatarControl.BodyPartColor.IKRed : AvatarControl.BodyPartColor.IKGreen;
            }
            if (!flag)
            {
                return AvatarControl.BodyPartColor.IKRed;
            }
            return (AvatarControl.BodyPartColor.Green | off);
        }

        protected bool IsValidHuman()
        {
            Animator component = base.gameObject.GetComponent<Animator>();
            if (component == null)
            {
                return false;
            }
            Avatar avatar = component.avatar;
            return ((avatar != null) && avatar.isHuman);
        }

        protected void MakePoseValid()
        {
            AvatarSetupTool.MakePoseValid(this.m_Bones);
            AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
            base.m_Inspector.Repaint();
        }

        public static bool MatchName(string transformName, string boneName)
        {
            char[] separator = ":".ToCharArray();
            string[] strArray = transformName.Split(separator);
            string[] strArray2 = boneName.Split(separator);
            return ((((transformName == boneName) || ((strArray.Length > 1) && (strArray[1] == boneName))) || ((strArray2.Length > 1) && (transformName == strArray2[1]))) || (((strArray.Length > 1) && (strArray2.Length > 1)) && (strArray[1] == strArray2[1])));
        }

        public override void OnDestroy()
        {
            if (this.m_CurrentTransformEditor != null)
            {
                Object.DestroyImmediate(this.m_CurrentTransformEditor);
            }
            base.OnDestroy();
        }

        public override void OnInspectorGUI()
        {
            if ((Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "UndoRedoPerformed"))
            {
                AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
                for (int i = 0; i < this.m_Bones.Length; i++)
                {
                    this.m_Bones[i].Serialize(base.serializedObject);
                }
            }
            this.UpdateSelectedBone();
            if (((Event.current.type == EventType.KeyDown) && ((Event.current.keyCode == KeyCode.Backspace) || (Event.current.keyCode == KeyCode.Delete))) && ((s_SelectedBoneIndex != -1) && (s_SelectedBoneIndex < this.m_Bones.Length)))
            {
                Undo.RegisterCompleteObjectUndo(this, "Avatar mapping modified");
                AvatarSetupTool.BoneWrapper wrapper = this.m_Bones[s_SelectedBoneIndex];
                wrapper.bone = null;
                wrapper.state = BoneState.None;
                wrapper.Serialize(base.serializedObject);
                Selection.activeTransform = null;
                GUI.changed = true;
                Event.current.Use();
            }
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical(string.Empty, "TE NodeBackground", new GUILayoutOption[0]);
            this.m_BodyView = AvatarControl.ShowBoneMapping(this.m_BodyView, new AvatarControl.BodyPartFeedback(this.IsValidBodyPart), this.m_Bones, base.serializedObject, this);
            this.HandleBodyView(this.m_BodyView);
            GUILayout.EndVertical();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(80f), GUILayout.MaxHeight(500f), GUILayout.ExpandHeight(true) };
            this.m_FoldoutScroll = GUILayout.BeginScrollView(this.m_FoldoutScroll, styles.box, options);
            this.DisplayFoldout();
            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
            if (EditorGUI.EndChangeCheck())
            {
                this.ValidateMapping();
                SceneView.RepaintAll();
            }
            this.DisplayMappingButtons();
            GUILayout.EndVertical();
            if (GUIUtility.hotControl == 0)
            {
                this.TransferPoseIfChanged();
            }
            base.ApplyRevertGUI();
            if (Selection.activeTransform != null)
            {
                if ((this.m_CurrentTransformEditor != null) && (this.m_CurrentTransformEditor.target != Selection.activeTransform))
                {
                    Object.DestroyImmediate(this.m_CurrentTransformEditor);
                }
                if (this.m_CurrentTransformEditor == null)
                {
                    this.m_CurrentTransformEditor = Editor.CreateEditor(Selection.activeTransform);
                }
                EditorGUILayout.Space();
                this.m_CurrentTransformEditorFoldout = EditorGUILayout.InspectorTitlebar(this.m_CurrentTransformEditorFoldout, Selection.activeTransform, true);
                if (this.m_CurrentTransformEditorFoldout && (this.m_CurrentTransformEditor != null))
                {
                    this.m_CurrentTransformEditor.OnInspectorGUI();
                }
            }
            else if (this.m_CurrentTransformEditor != null)
            {
                Object.DestroyImmediate(this.m_CurrentTransformEditor);
                this.m_CurrentTransformEditor = null;
            }
        }

        public override void OnSceneGUI()
        {
            if (s_Styles != null)
            {
                AvatarSkeletonDrawer.DrawSkeleton(base.root, base.modelBones, this.m_Bones);
                if (GUIUtility.hotControl == 0)
                {
                    this.TransferPoseIfChanged();
                }
            }
        }

        private HumanTemplate OpenHumanTemplate()
        {
            string directory = "Assets/";
            string path = EditorUtility.OpenFilePanel("Open Human Template", directory, "ht");
            if (path == string.Empty)
            {
                return null;
            }
            HumanTemplate template = AssetDatabase.LoadMainAssetAtPath(FileUtil.GetProjectRelativePath(path)) as HumanTemplate;
            if ((template == null) && EditorUtility.DisplayDialog("Human Template not found in project", "Import asset '" + path + "' into project", "Yes", "No"))
            {
                string to = AssetDatabase.GenerateUniqueAssetPath(directory + FileUtil.GetLastPathNameComponent(path));
                FileUtil.CopyFileOrDirectory(path, to);
                AssetDatabase.Refresh();
                template = AssetDatabase.LoadMainAssetAtPath(to) as HumanTemplate;
                if (template == null)
                {
                    Debug.Log("Failed importing file '" + path + "' to '" + to + "'");
                }
            }
            return template;
        }

        protected void PerformAutoMapping()
        {
            this.AutoMapping();
            this.ValidateMapping();
            SceneView.RepaintAll();
        }

        protected void PerformBipedMapping()
        {
            this.BipedMapping();
            this.ValidateMapping();
            SceneView.RepaintAll();
        }

        protected Vector4 QuaternionToVector4(Quaternion rot)
        {
            return new Vector4(rot.x, rot.y, rot.z, rot.w);
        }

        protected void ResetBones()
        {
            for (int i = 0; i < this.m_Bones.Length; i++)
            {
                this.m_Bones[i].Reset(base.serializedObject, base.modelBones);
            }
        }

        protected override void ResetValues()
        {
            base.ResetValues();
            this.ResetBones();
            this.Init();
        }

        protected void SampleBindPose()
        {
            AvatarSetupTool.SampleBindPose(base.gameObject);
            AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
            base.m_Inspector.Repaint();
        }

        private void SaveHumanTemplate()
        {
            string message = string.Format("Create a new human template", new object[0]);
            string path = EditorUtility.SaveFilePanelInProject("Create New Human Template", "New Human Template", "ht", message);
            if (path != string.Empty)
            {
                HumanTemplate asset = new HumanTemplate();
                asset.ClearTemplate();
                for (int i = 0; i < this.m_Bones.Length; i++)
                {
                    if (this.m_Bones[i].bone != null)
                    {
                        asset.Insert(this.m_Bones[i].humanBoneName, this.m_Bones[i].bone.name);
                    }
                }
                AssetDatabase.CreateAsset(asset, path);
            }
        }

        protected void TransferPoseIfChanged()
        {
            foreach (GameObject obj2 in Selection.gameObjects)
            {
                if (this.TransformChanged(obj2.transform))
                {
                    AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
                    base.m_Inspector.Repaint();
                    break;
                }
            }
        }

        private bool TransformChanged(Transform tr)
        {
            SerializedProperty property = AvatarSetupTool.FindSkeletonBone(base.serializedObject, tr, false, false);
            if (property != null)
            {
                SerializedProperty property2 = property.FindPropertyRelative(AvatarSetupTool.sPosition);
                if ((property2 != null) && (property2.vector3Value != tr.localPosition))
                {
                    return true;
                }
                SerializedProperty property3 = property.FindPropertyRelative(AvatarSetupTool.sRotation);
                if ((property3 != null) && (property3.quaternionValue != tr.localRotation))
                {
                    return true;
                }
                SerializedProperty property4 = property.FindPropertyRelative(AvatarSetupTool.sScale);
                if ((property4 != null) && (property4.vector3Value != tr.localScale))
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateSelectedBone()
        {
            int num = s_SelectedBoneIndex;
            if (((s_SelectedBoneIndex < 0) || (s_SelectedBoneIndex >= this.m_Bones.Length)) || (this.m_Bones[s_SelectedBoneIndex].bone != Selection.activeTransform))
            {
                s_SelectedBoneIndex = -1;
                if (Selection.activeTransform != null)
                {
                    for (int i = 0; i < this.m_Bones.Length; i++)
                    {
                        if (this.m_Bones[i].bone == Selection.activeTransform)
                        {
                            s_SelectedBoneIndex = i;
                            break;
                        }
                    }
                }
            }
            if (s_SelectedBoneIndex != num)
            {
                List<int> viewsThatContainBone = AvatarControl.GetViewsThatContainBone(s_SelectedBoneIndex);
                if ((viewsThatContainBone.Count > 0) && !viewsThatContainBone.Contains(this.m_BodyView))
                {
                    this.m_BodyView = viewsThatContainBone[0];
                }
            }
        }

        protected void ValidateMapping()
        {
            for (int i = 0; i < this.m_Bones.Length; i++)
            {
                string str;
                this.m_Bones[i].state = this.GetBoneState(i, out str);
                this.m_Bones[i].error = str;
            }
        }

        protected Quaternion Vector4ToQuaternion(Vector4 rot)
        {
            return new Quaternion(rot.x, rot.y, rot.z, rot.w);
        }

        internal static Styles styles
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
        private sealed class <ApplyTemplate>c__AnonStorey96
        {
            internal string boneName;

            internal bool <>m__1C0(Transform f)
            {
                return AvatarMappingEditor.MatchName(f.name, this.boneName);
            }
        }

        [CompilerGenerated]
        private sealed class <GetBoneState>c__AnonStorey95
        {
            internal AvatarSetupTool.BoneWrapper bone;

            internal bool <>m__1BF(AvatarSetupTool.BoneWrapper f)
            {
                return (f.bone == this.bone.bone);
            }
        }

        internal class Styles
        {
            public GUIContent autoMapping = EditorGUIUtility.TextContent("Automap");
            public GUIContent bipedMapping = EditorGUIUtility.TextContent("Biped");
            public GUIContent bipedPose = EditorGUIUtility.TextContent("Biped Pose");
            public GUIContent[] BodyPartMapping = new GUIContent[] { EditorGUIUtility.TextContent("Avatar"), EditorGUIUtility.TextContent("Body"), EditorGUIUtility.TextContent("Head"), EditorGUIUtility.TextContent("Left Arm"), EditorGUIUtility.TextContent("Left Fingers"), EditorGUIUtility.TextContent("Right Arm"), EditorGUIUtility.TextContent("Right Fingers"), EditorGUIUtility.TextContent("Left Leg"), EditorGUIUtility.TextContent("Right Leg") };
            public GUIStyle box = new GUIStyle("box");
            public GUIContent clearMapping = EditorGUIUtility.TextContent("Clear");
            public GUIContent CloseError = EditorGUIUtility.TextContent("Close Error (s)");
            public GUIContent DoneCharacter = EditorGUIUtility.TextContent("Done");
            public GUIContent dotFill = EditorGUIUtility.IconContent("AvatarInspector/DotFill");
            public GUIContent dotFrame = EditorGUIUtility.IconContent("AvatarInspector/DotFrame");
            public GUIContent dotFrameDotted = EditorGUIUtility.IconContent("AvatarInspector/DotFrameDotted");
            public GUIContent dotSelection = EditorGUIUtility.IconContent("AvatarInspector/DotSelection");
            public GUIContent enforceTPose = EditorGUIUtility.TextContent("Enforce T-Pose");
            public GUIStyle errorLabel = new GUIStyle(EditorStyles.wordWrappedMiniLabel);
            public GUIContent loadMapping = EditorGUIUtility.TextContent("Load");
            public GUIContent mapping = EditorGUIUtility.TextContent("Mapping");
            public GUIContent pose = EditorGUIUtility.TextContent("Pose");
            public GUIContent RequiredBone = EditorGUIUtility.TextContent("Optional Bones");
            public GUIContent resetPose = EditorGUIUtility.TextContent("Reset");
            public GUIContent sampleBindPose = EditorGUIUtility.TextContent("Sample Bind-Pose");
            public GUIContent saveMapping = EditorGUIUtility.TextContent("Save");
            public GUIContent ShowError = EditorGUIUtility.TextContent("Show Error (s)...");
            public GUIStyle toolbar = "TE Toolbar";
            public GUIStyle toolbarDropDown = "TE ToolbarDropDown";

            public Styles()
            {
                this.box.padding = new RectOffset(0, 0, 0, 0);
                this.box.margin = new RectOffset(0, 0, 0, 0);
                this.errorLabel.normal.textColor = new Color(0.6f, 0f, 0f, 1f);
            }
        }
    }
}

