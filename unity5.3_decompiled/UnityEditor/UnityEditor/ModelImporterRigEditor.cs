namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    internal class ModelImporterRigEditor : AssetImporterInspector
    {
        private const float kDeleteWidth = 17f;
        private SerializedProperty m_AnimationCompression;
        private SerializedProperty m_AnimationType;
        private Avatar m_Avatar;
        private bool m_AvatarCopyIsUpToDate;
        private SerializedProperty m_AvatarSource;
        private List<string> m_BipedMappingReport;
        private bool m_CanMultiEditTransformList;
        private SerializedProperty m_CopyAvatar;
        private ExposeTransformEditor m_ExposeTransformEditor;
        private bool m_IsBiped;
        private SerializedProperty m_LegacyGenerateAnimations;
        private SerializedProperty m_OptimizeGameObjects;
        private GUIContent[] m_RootMotionBoneList;
        private SerializedProperty m_RootMotionBoneName;
        public int m_SelectedClipIndex = -1;
        private static Styles styles;

        internal override void Apply()
        {
            MappingRelevantSettings[] sourceArray = new MappingRelevantSettings[base.targets.Length];
            for (int i = 0; i < base.targets.Length; i++)
            {
                SerializedObject obj2 = new SerializedObject(base.targets[i]);
                SerializedProperty property = obj2.FindProperty("m_AnimationType");
                SerializedProperty property2 = obj2.FindProperty("m_CopyAvatar");
                sourceArray[i].humanoid = property.intValue == 3;
                sourceArray[i].hasNoAnimation = property.intValue == 0;
                sourceArray[i].copyAvatar = property2.boolValue;
            }
            MappingRelevantSettings[] destinationArray = new MappingRelevantSettings[base.targets.Length];
            Array.Copy(sourceArray, destinationArray, base.targets.Length);
            for (int j = 0; j < base.targets.Length; j++)
            {
                if (!this.m_AnimationType.hasMultipleDifferentValues)
                {
                    destinationArray[j].humanoid = this.m_AnimationType.intValue == 3;
                }
                if (!this.m_CopyAvatar.hasMultipleDifferentValues)
                {
                    destinationArray[j].copyAvatar = this.m_CopyAvatar.boolValue;
                }
            }
            base.serializedObject.ApplyModifiedProperties();
            for (int k = 0; k < base.targets.Length; k++)
            {
                if (sourceArray[k].usesOwnAvatar && !destinationArray[k].usesOwnAvatar)
                {
                    SerializedObject serializedObject = new SerializedObject(base.targets[k]);
                    AvatarSetupTool.ClearAll(serializedObject);
                    serializedObject.ApplyModifiedProperties();
                }
                if (!sourceArray[k].usesOwnAvatar && destinationArray[k].usesOwnAvatar)
                {
                    ModelImporter importer = base.targets[k] as ModelImporter;
                    if (sourceArray[k].hasNoAnimation)
                    {
                        AssetDatabase.ImportAsset(importer.assetPath);
                    }
                    SerializedObject modelImporterSerializedObject = new SerializedObject(base.targets[k]);
                    GameObject original = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as GameObject;
                    Animator component = original.GetComponent<Animator>();
                    bool flag = (component != null) && !component.hasTransformHierarchy;
                    if (flag)
                    {
                        original = Object.Instantiate<GameObject>(original);
                        AnimatorUtility.DeoptimizeTransformHierarchy(original);
                    }
                    AvatarSetupTool.AutoSetupOnInstance(original, modelImporterSerializedObject);
                    this.m_IsBiped = AvatarBipedMapper.IsBiped(original.transform, this.m_BipedMappingReport);
                    if (flag)
                    {
                        Object.DestroyImmediate(original);
                    }
                    modelImporterSerializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void AvatarSourceGUI()
        {
            EditorGUI.BeginChangeCheck();
            int selectedIndex = !this.m_CopyAvatar.boolValue ? 0 : 1;
            EditorGUI.showMixedValue = this.m_CopyAvatar.hasMultipleDifferentValues;
            selectedIndex = EditorGUILayout.Popup(styles.AvatarDefinition, selectedIndex, styles.AvatarDefinitionOpt, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_CopyAvatar.boolValue = selectedIndex == 1;
            }
        }

        private bool CanMultiEditTransformList()
        {
            string[] transformPaths = this.singleImporter.transformPaths;
            for (int i = 1; i < base.targets.Length; i++)
            {
                ModelImporter importer = base.targets[i] as ModelImporter;
                if (!ArrayUtility.ArrayEquals<string>(transformPaths, importer.transformPaths))
                {
                    return false;
                }
            }
            return true;
        }

        private void CheckAvatar(Avatar sourceAvatar)
        {
            if (sourceAvatar != null)
            {
                if (sourceAvatar.isHuman && (this.animationType != ModelImporterAnimationType.Human))
                {
                    if (EditorUtility.DisplayDialog("Asigning an Humanoid Avatar on a Generic Rig", "Do you want to change Animation Type to Humanoid ?", "Yes", "No"))
                    {
                        this.animationType = ModelImporterAnimationType.Human;
                    }
                    else
                    {
                        this.m_AvatarSource.objectReferenceValue = null;
                    }
                }
                else if (!sourceAvatar.isHuman && (this.animationType != ModelImporterAnimationType.Generic))
                {
                    if (EditorUtility.DisplayDialog("Asigning an Generic Avatar on a Humanoid Rig", "Do you want to change Animation Type to Generic ?", "Yes", "No"))
                    {
                        this.animationType = ModelImporterAnimationType.Generic;
                    }
                    else
                    {
                        this.m_AvatarSource.objectReferenceValue = null;
                    }
                }
            }
        }

        private void CheckIfAvatarCopyIsUpToDate()
        {
            if (((this.animationType != ModelImporterAnimationType.Human) && (this.animationType != ModelImporterAnimationType.Generic)) || (this.m_AvatarSource.objectReferenceValue == null))
            {
                this.m_AvatarCopyIsUpToDate = true;
            }
            else
            {
                ModelImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.m_AvatarSource.objectReferenceValue)) as ModelImporter;
                this.m_AvatarCopyIsUpToDate = DoesHumanDescriptionMatch(this.singleImporter, atPath);
            }
        }

        private void ConfigureAvatarGUI()
        {
            if (base.targets.Length > 1)
            {
                GUILayout.Label("Can't configure avatar in multi-editing mode", EditorStyles.helpBox, new GUILayoutOption[0]);
            }
            else if (this.singleImporter.transformPaths.Length <= HumanTrait.RequiredBoneCount)
            {
                GUILayout.Label("Not enough bones to create human avatar (requires " + HumanTrait.RequiredBoneCount + ")", EditorStyles.helpBox, new GUILayoutOption[0]);
            }
            else
            {
                GUIContent avatarValid;
                if ((this.m_Avatar != null) && !this.HasModified())
                {
                    if (this.m_Avatar.isHuman)
                    {
                        avatarValid = styles.avatarValid;
                    }
                    else
                    {
                        avatarValid = styles.avatarInvalid;
                    }
                }
                else
                {
                    avatarValid = styles.avatarPending;
                    GUILayout.Label("The avatar can be configured after settings have been applied.", EditorStyles.helpBox, new GUILayoutOption[0]);
                }
                Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
                GUI.Label(new Rect((controlRect.xMax - 75f) - 18f, controlRect.y, 18f, controlRect.height), avatarValid, EditorStyles.label);
                EditorGUI.BeginDisabledGroup(this.m_Avatar == null);
                if (GUI.Button(new Rect(controlRect.xMax - 75f, controlRect.y + 1f, 75f, controlRect.height - 1f), styles.configureAvatar, EditorStyles.miniButton))
                {
                    if (!this.isLocked)
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            Selection.activeObject = this.m_Avatar;
                            AvatarEditor.s_EditImmediatelyOnNextOpen = true;
                        }
                        GUIUtility.ExitGUI();
                    }
                    else
                    {
                        Debug.Log("Cannot configure avatar, inspector is locked");
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        private void CopyAvatarGUI()
        {
            GUILayout.Label("If you have already created an Avatar for another model with a rig identical to this one, you can copy its Avatar definition.\nWith this option, this model will not create any avatar but only import animations.", EditorStyles.helpBox, new GUILayoutOption[0]);
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_AvatarSource, GUIContent.Temp("Source"), new GUILayoutOption[0]);
            Avatar objectReferenceValue = this.m_AvatarSource.objectReferenceValue as Avatar;
            if (EditorGUI.EndChangeCheck())
            {
                this.CheckAvatar(objectReferenceValue);
                AvatarSetupTool.ClearAll(base.serializedObject);
                if (objectReferenceValue != null)
                {
                    this.CopyHumanDescriptionFromOtherModel(objectReferenceValue);
                }
                this.m_AvatarCopyIsUpToDate = true;
            }
            if (((objectReferenceValue != null) && !this.m_AvatarSource.hasMultipleDifferentValues) && (!this.m_AvatarCopyIsUpToDate && GUILayout.Button(styles.UpdateMuscleDefinitionFromSource, EditorStyles.miniButton, new GUILayoutOption[0])))
            {
                AvatarSetupTool.ClearAll(base.serializedObject);
                this.CopyHumanDescriptionFromOtherModel(objectReferenceValue);
                this.m_AvatarCopyIsUpToDate = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CopyHumanDescriptionFromOtherModel(Avatar sourceAvatar)
        {
            SerializedObject modelImporterSerializedObject = GetModelImporterSerializedObject(AssetDatabase.GetAssetPath(sourceAvatar));
            CopyHumanDescriptionToDestination(modelImporterSerializedObject, base.serializedObject);
            modelImporterSerializedObject.Dispose();
        }

        private static void CopyHumanDescriptionToDestination(SerializedObject sourceObject, SerializedObject targetObject)
        {
            targetObject.CopyFromSerializedProperty(sourceObject.FindProperty("m_HumanDescription"));
        }

        private static bool DoesHumanDescriptionMatch(ModelImporter importer, ModelImporter otherImporter)
        {
            Object[] objs = new Object[] { importer, otherImporter };
            SerializedObject obj2 = new SerializedObject(objs);
            bool flag = !obj2.FindProperty("m_HumanDescription").hasMultipleDifferentValues;
            obj2.Dispose();
            return flag;
        }

        private void GenericGUI()
        {
            this.AvatarSourceGUI();
            if (!this.m_CopyAvatar.hasMultipleDifferentValues)
            {
                if (!this.m_CopyAvatar.boolValue)
                {
                    EditorGUI.BeginDisabledGroup(!this.m_CanMultiEditTransformList);
                    EditorGUI.BeginChangeCheck();
                    this.rootIndex = EditorGUILayout.Popup(styles.RootNode, this.rootIndex, this.m_RootMotionBoneList, new GUILayoutOption[0]);
                    EditorGUI.EndDisabledGroup();
                    if (EditorGUI.EndChangeCheck())
                    {
                        if ((this.rootIndex > 0) && (this.rootIndex < this.m_RootMotionBoneList.Length))
                        {
                            this.m_RootMotionBoneName.stringValue = FileUtil.GetLastPathNameComponent(this.m_RootMotionBoneList[this.rootIndex].text);
                        }
                        else
                        {
                            this.m_RootMotionBoneName.stringValue = string.Empty;
                        }
                    }
                }
                else
                {
                    this.CopyAvatarGUI();
                }
            }
        }

        private static SerializedObject GetModelImporterSerializedObject(string assetPath)
        {
            ModelImporter atPath = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (atPath == null)
            {
                return null;
            }
            return new SerializedObject(atPath);
        }

        private void HumanoidGUI()
        {
            this.AvatarSourceGUI();
            if (!this.m_CopyAvatar.hasMultipleDifferentValues)
            {
                if (!this.m_CopyAvatar.boolValue)
                {
                    this.ConfigureAvatarGUI();
                }
                else
                {
                    this.CopyAvatarGUI();
                }
            }
            if (this.m_IsBiped)
            {
                if (this.m_BipedMappingReport.Count > 0)
                {
                    string message = "A Biped was detected, but cannot be configured properly because of an unsupported hierarchy. Adjust Biped settings in 3DS Max before exporting to correct this problem.\n";
                    for (int i = 0; i < this.m_BipedMappingReport.Count; i++)
                    {
                        message = message + this.m_BipedMappingReport[i];
                    }
                    EditorGUILayout.HelpBox(message, MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("A Biped was detected. Default Biped mapping and T-Pose have been configured for this avatar. Translation DoFs have been activated. Use Configure to modify default Biped setup.", MessageType.Info);
                }
            }
            EditorGUILayout.Space();
        }

        private void LegacyGUI()
        {
            EditorGUILayout.Popup(this.m_LegacyGenerateAnimations, styles.AnimationsOpt, styles.AnimLabel, new GUILayoutOption[0]);
            if (((this.m_LegacyGenerateAnimations.intValue == 1) || (this.m_LegacyGenerateAnimations.intValue == 2)) || (this.m_LegacyGenerateAnimations.intValue == 3))
            {
                EditorGUILayout.HelpBox("The animation import setting \"" + styles.AnimationsOpt[this.m_LegacyGenerateAnimations.intValue].text + "\" is deprecated.", MessageType.Warning);
            }
        }

        public void OnEnable()
        {
            this.m_AnimationType = base.serializedObject.FindProperty("m_AnimationType");
            this.m_AvatarSource = base.serializedObject.FindProperty("m_LastHumanDescriptionAvatarSource");
            this.m_OptimizeGameObjects = base.serializedObject.FindProperty("m_OptimizeGameObjects");
            this.m_RootMotionBoneName = base.serializedObject.FindProperty("m_HumanDescription.m_RootMotionBoneName");
            this.m_ExposeTransformEditor = new ExposeTransformEditor();
            string[] transformPaths = this.singleImporter.transformPaths;
            this.m_RootMotionBoneList = new GUIContent[transformPaths.Length];
            for (int i = 0; i < transformPaths.Length; i++)
            {
                this.m_RootMotionBoneList[i] = new GUIContent(transformPaths[i]);
            }
            if (this.m_RootMotionBoneList.Length > 0)
            {
                this.m_RootMotionBoneList[0] = new GUIContent("None");
            }
            this.rootIndex = ArrayUtility.FindIndex<GUIContent>(this.m_RootMotionBoneList, content => FileUtil.GetLastPathNameComponent(content.text) == this.m_RootMotionBoneName.stringValue);
            this.rootIndex = (this.rootIndex >= 1) ? this.rootIndex : 0;
            this.m_CopyAvatar = base.serializedObject.FindProperty("m_CopyAvatar");
            this.m_LegacyGenerateAnimations = base.serializedObject.FindProperty("m_LegacyGenerateAnimations");
            this.m_AnimationCompression = base.serializedObject.FindProperty("m_AnimationCompression");
            this.m_ExposeTransformEditor.OnEnable(this.singleImporter.transformPaths, base.serializedObject);
            this.m_CanMultiEditTransformList = this.CanMultiEditTransformList();
            this.CheckIfAvatarCopyIsUpToDate();
            this.m_IsBiped = false;
            this.m_BipedMappingReport = new List<string>();
            if (this.m_AnimationType.intValue == 3)
            {
                GameObject obj2 = AssetDatabase.LoadMainAssetAtPath(this.singleImporter.assetPath) as GameObject;
                this.m_IsBiped = AvatarBipedMapper.IsBiped(obj2.transform, this.m_BipedMappingReport);
            }
        }

        public override void OnInspectorGUI()
        {
            if (styles == null)
            {
                styles = new Styles();
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Popup(this.m_AnimationType, styles.AnimationTypeOpt, styles.AnimationType, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_AvatarSource.objectReferenceValue = null;
                if (this.animationType == ModelImporterAnimationType.Legacy)
                {
                    this.m_AnimationCompression.intValue = 1;
                }
                else if ((this.animationType == ModelImporterAnimationType.Generic) || (this.animationType == ModelImporterAnimationType.Human))
                {
                    this.m_AnimationCompression.intValue = 3;
                }
            }
            EditorGUILayout.Space();
            if (!this.m_AnimationType.hasMultipleDifferentValues)
            {
                if (this.animationType == ModelImporterAnimationType.Human)
                {
                    this.HumanoidGUI();
                }
                else if (this.animationType == ModelImporterAnimationType.Generic)
                {
                    this.GenericGUI();
                }
                else if (this.animationType == ModelImporterAnimationType.Legacy)
                {
                    this.LegacyGUI();
                }
            }
            if (((this.m_Avatar != null) && this.m_Avatar.isValid) && this.m_Avatar.isHuman)
            {
                this.ShowUpdateReferenceClip();
            }
            bool flag = true;
            if ((this.animationType != ModelImporterAnimationType.Human) && (this.animationType != ModelImporterAnimationType.Generic))
            {
                flag = false;
            }
            if (this.m_CopyAvatar.boolValue)
            {
                flag = false;
            }
            if (flag)
            {
                EditorGUILayout.PropertyField(this.m_OptimizeGameObjects, new GUILayoutOption[0]);
                if (this.m_OptimizeGameObjects.boolValue && (base.serializedObject.targetObjects.Length == 1))
                {
                    EditorGUILayout.Space();
                    EditorGUI.BeginDisabledGroup(!this.m_CanMultiEditTransformList);
                    this.m_ExposeTransformEditor.OnGUI();
                    EditorGUI.EndDisabledGroup();
                }
            }
            base.ApplyRevertGUI();
        }

        internal override void ResetValues()
        {
            base.ResetValues();
            this.m_Avatar = AssetDatabase.LoadAssetAtPath((this.target as ModelImporter).assetPath, typeof(Avatar)) as Avatar;
        }

        private void SetupReferencedClip(string otherModelImporterPath)
        {
            SerializedObject modelImporterSerializedObject = GetModelImporterSerializedObject(otherModelImporterPath);
            if (modelImporterSerializedObject != null)
            {
                modelImporterSerializedObject.CopyFromSerializedProperty(base.serializedObject.FindProperty("m_AnimationType"));
                SerializedProperty property = modelImporterSerializedObject.FindProperty("m_CopyAvatar");
                if (property != null)
                {
                    property.boolValue = true;
                }
                SerializedProperty property2 = modelImporterSerializedObject.FindProperty("m_LastHumanDescriptionAvatarSource");
                if (property2 != null)
                {
                    property2.objectReferenceValue = this.m_Avatar;
                }
                CopyHumanDescriptionToDestination(base.serializedObject, modelImporterSerializedObject);
                modelImporterSerializedObject.ApplyModifiedProperties();
                modelImporterSerializedObject.Dispose();
            }
        }

        private void ShowUpdateReferenceClip()
        {
            if (((base.targets.Length <= 1) && (this.animationType == ModelImporterAnimationType.Human)) && !this.m_CopyAvatar.boolValue)
            {
                string[] array = new string[0];
                ModelImporter target = this.target as ModelImporter;
                if (target.referencedClips.Length > 0)
                {
                    foreach (string str in target.referencedClips)
                    {
                        ArrayUtility.Add<string>(ref array, AssetDatabase.GUIDToAssetPath(str));
                    }
                }
                if (array.Length > 0)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(150f) };
                    if (GUILayout.Button(styles.UpdateReferenceClips, options))
                    {
                        foreach (string str2 in array)
                        {
                            this.SetupReferencedClip(str2);
                        }
                        try
                        {
                            AssetDatabase.StartAssetEditing();
                            foreach (string str3 in array)
                            {
                                AssetDatabase.ImportAsset(str3);
                            }
                        }
                        finally
                        {
                            AssetDatabase.StopAssetEditing();
                        }
                    }
                }
            }
        }

        private ModelImporterAnimationType animationType
        {
            get
            {
                return (ModelImporterAnimationType) this.m_AnimationType.intValue;
            }
            set
            {
                this.m_AnimationType.intValue = (int) value;
            }
        }

        public bool isLocked
        {
            get
            {
                foreach (InspectorWindow window in InspectorWindow.GetAllInspectorWindows())
                {
                    foreach (Editor editor in window.GetTracker().activeEditors)
                    {
                        if (editor == this)
                        {
                            return window.isLocked;
                        }
                    }
                }
                return false;
            }
        }

        public int rootIndex { get; set; }

        private ModelImporter singleImporter
        {
            get
            {
                return (base.targets[0] as ModelImporter);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MappingRelevantSettings
        {
            public bool humanoid;
            public bool copyAvatar;
            public bool hasNoAnimation;
            public bool usesOwnAvatar
            {
                get
                {
                    return (this.humanoid && !this.copyAvatar);
                }
            }
        }

        private class Styles
        {
            public GUIContent[] AnimationsOpt = new GUIContent[] { EditorGUIUtility.TextContent("Don't Import|No animation or skinning is imported."), EditorGUIUtility.TextContent("Store in Original Roots (Deprecated)|Animations are stored in the root objects of your animation package (these might be different from the root objects in Unity)."), EditorGUIUtility.TextContent("Store in Nodes (Deprecated)|Animations are stored together with the objects they animate. Use this when you have a complex animation setup and want full scripting control."), EditorGUIUtility.TextContent("Store in Root (Deprecated)|Animations are stored in the scene's transform root objects. Use this when animating anything that has a hierarchy."), EditorGUIUtility.TextContent("Store in Root (New)") };
            public GUIContent AnimationType = EditorGUIUtility.TextContent("Animation Type|The type of animation to support / import.");
            public GUIContent[] AnimationTypeOpt = new GUIContent[] { EditorGUIUtility.TextContent("None|No animation present."), EditorGUIUtility.TextContent("Legacy|Legacy animation system."), EditorGUIUtility.TextContent("Generic|Generic Mecanim animation."), EditorGUIUtility.TextContent("Humanoid|Humanoid Mecanim animation system.") };
            public GUIContent AnimLabel = EditorGUIUtility.TextContent("Generation|Controls how animations are imported.");
            public GUIContent avatar = new GUIContent("Animator");
            public GUIContent AvatarDefinition = EditorGUIUtility.TextContent("Avatar Definition|Choose between Create From This Model or Copy From Other Avatar. The first one create an Avatar for this file and the second one use an Avatar from another file to import animation.");
            public GUIContent[] AvatarDefinitionOpt = new GUIContent[] { EditorGUIUtility.TextContent("Create From This Model|Create an Avatar based on the model from this file."), EditorGUIUtility.TextContent("Copy From Other Avatar|Copy an Avatar from another file to import muscle clip. No avatar will be created.") };
            public GUIContent avatarInvalid = EditorGUIUtility.TextContent("✕");
            public GUIContent avatarPending = EditorGUIUtility.TextContent("...");
            public GUIContent avatarValid = EditorGUIUtility.TextContent("✓");
            public GUIContent configureAvatar = EditorGUIUtility.TextContent("Configure...");
            public GUIStyle helpText = new GUIStyle(EditorStyles.helpBox);
            public GUIContent RootNode = EditorGUIUtility.TextContent("Root node|Specify the root node used to extract the animation translation.");
            public GUIContent UpdateMuscleDefinitionFromSource = EditorGUIUtility.TextContent("Update|Update the copy of the muscle definition from the source.");
            public GUIContent UpdateReferenceClips = EditorGUIUtility.TextContent("Update reference clips|Click on this button to update all the @convention file referencing this file. Should set all these files to Copy From Other Avatar, set the source Avatar to this one and reimport all these files.");

            public Styles()
            {
                this.helpText.normal.background = null;
                this.helpText.alignment = TextAnchor.MiddleLeft;
                this.helpText.padding = new RectOffset(0, 0, 0, 0);
            }
        }
    }
}

