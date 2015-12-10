namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SubModuleUI : ModuleUI
    {
        private const int k_MaxSubPerType = 2;
        private int m_CheckObjectIndex;
        private int m_CheckObjectTypeIndex;
        private SerializedProperty[,] m_SubEmitters;
        private static Texts s_Texts;

        public SubModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SubModule", displayName)
        {
            this.m_CheckObjectTypeIndex = -1;
            this.m_CheckObjectIndex = -1;
            base.m_ToolTip = "Sub emission of particles. This allows each particle to emit particles in another system.";
            this.Init();
        }

        private void CheckIfChild(Object subEmitter)
        {
            if (subEmitter != null)
            {
                ParticleSystem root = base.m_ParticleSystemUI.m_ParticleEffectUI.GetRoot();
                if (!IsChild(subEmitter as ParticleSystem, root))
                {
                    string message = string.Format("The assigned sub emitter is not a child of the current root particle system GameObject: '{0}' and is therefore NOT considered a part of the current effect. Do you want to reparent it?", root.gameObject.name);
                    if (EditorUtility.DisplayDialog("Reparent GameObjects", message, "Yes, Reparent", "No"))
                    {
                        if (EditorUtility.IsPersistent(subEmitter))
                        {
                            GameObject obj2 = Object.Instantiate(subEmitter) as GameObject;
                            if (obj2 != null)
                            {
                                obj2.transform.parent = base.m_ParticleSystemUI.m_ParticleSystem.transform;
                                obj2.transform.localPosition = Vector3.zero;
                                obj2.transform.localRotation = Quaternion.identity;
                            }
                        }
                        else
                        {
                            ParticleSystem system2 = subEmitter as ParticleSystem;
                            if (system2 != null)
                            {
                                system2.gameObject.transform.parent = base.m_ParticleSystemUI.m_ParticleSystem.transform;
                            }
                        }
                    }
                }
            }
        }

        private void CreateAndAssignSubEmitter(SerializedProperty objectRefProp, SubEmitterType type)
        {
            GameObject obj2 = base.m_ParticleSystemUI.m_ParticleEffectUI.CreateParticleSystem(base.m_ParticleSystemUI.m_ParticleSystem, type);
            obj2.name = "SubEmitter";
            objectRefProp.objectReferenceValue = obj2.GetComponent<ParticleSystem>();
        }

        protected override void Init()
        {
            if (this.m_SubEmitters == null)
            {
                this.m_SubEmitters = new SerializedProperty[,] { { base.GetProperty("subEmitterBirth"), base.GetProperty("subEmitterBirth1") }, { base.GetProperty("subEmitterCollision"), base.GetProperty("subEmitterCollision1") }, { base.GetProperty("subEmitterDeath"), base.GetProperty("subEmitterDeath1") } };
            }
        }

        internal static bool IsChild(ParticleSystem subEmitter, ParticleSystem root)
        {
            return (((subEmitter != null) && (root != null)) && (ParticleSystemEditorUtils.GetRoot(subEmitter) == root));
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            Object[] objArray1 = new Object[,] { { this.m_SubEmitters[0, 0].objectReferenceValue, this.m_SubEmitters[0, 1].objectReferenceValue }, { this.m_SubEmitters[1, 0].objectReferenceValue, this.m_SubEmitters[1, 1].objectReferenceValue }, { this.m_SubEmitters[2, 0].objectReferenceValue, this.m_SubEmitters[2, 1].objectReferenceValue } };
            Object[,] objArray = objArray1;
            for (int i = 0; i < 3; i++)
            {
                SerializedProperty[] objectProps = new SerializedProperty[] { this.m_SubEmitters[i, 0], this.m_SubEmitters[i, 1] };
                int num2 = base.GUIListOfFloatObjectToggleFields(s_Texts.subEmitterTypeTexts[i], objectProps, null, s_Texts.create, true);
                if (num2 != -1)
                {
                    this.CreateAndAssignSubEmitter(this.m_SubEmitters[i, num2], (SubEmitterType) i);
                }
            }
            Object[] objArray3 = new Object[,] { { this.m_SubEmitters[0, 0].objectReferenceValue, this.m_SubEmitters[0, 1].objectReferenceValue }, { this.m_SubEmitters[1, 0].objectReferenceValue, this.m_SubEmitters[1, 1].objectReferenceValue }, { this.m_SubEmitters[2, 0].objectReferenceValue, this.m_SubEmitters[2, 1].objectReferenceValue } };
            Object[,] objArray2 = objArray3;
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    if (objArray[j, k] != objArray2[j, k])
                    {
                        if ((this.m_CheckObjectIndex == -1) && (this.m_CheckObjectTypeIndex == -1))
                        {
                            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
                        }
                        this.m_CheckObjectTypeIndex = j;
                        this.m_CheckObjectIndex = k;
                    }
                }
            }
        }

        private void Update()
        {
            if (((this.m_CheckObjectIndex >= 0) && (this.m_CheckObjectTypeIndex >= 0)) && !ObjectSelector.isVisible)
            {
                Object objectReferenceValue = this.m_SubEmitters[this.m_CheckObjectTypeIndex, this.m_CheckObjectIndex].objectReferenceValue;
                ParticleSystem subEmitter = objectReferenceValue as ParticleSystem;
                if (subEmitter != null)
                {
                    bool flag = true;
                    if (this.ValidateSubemitter(subEmitter))
                    {
                        string str = ParticleSystemEditorUtils.CheckCircularReferences(subEmitter);
                        if (str.Length == 0)
                        {
                            this.CheckIfChild(objectReferenceValue);
                        }
                        else
                        {
                            string message = string.Format("'{0}' could not be assigned as subemitter on '{1}' due to circular referencing!\nBacktrace: {2} \n\nReference will be removed.", subEmitter.gameObject.name, base.m_ParticleSystemUI.m_ParticleSystem.gameObject.name, str);
                            EditorUtility.DisplayDialog("Circular References Detected", message, "Ok");
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    if (!flag)
                    {
                        this.m_SubEmitters[this.m_CheckObjectTypeIndex, this.m_CheckObjectIndex].objectReferenceValue = null;
                        base.m_ParticleSystemUI.ApplyProperties();
                        base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
                    }
                }
                this.m_CheckObjectIndex = -1;
                this.m_CheckObjectTypeIndex = -1;
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
            }
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            text = text + "\n\tSub Emitters are enabled.";
        }

        public override void Validate()
        {
        }

        private bool ValidateSubemitter(ParticleSystem subEmitter)
        {
            if (subEmitter == null)
            {
                return false;
            }
            ParticleSystem root = base.m_ParticleSystemUI.m_ParticleEffectUI.GetRoot();
            if (root.gameObject.activeInHierarchy && !subEmitter.gameObject.activeInHierarchy)
            {
                string message = string.Format("The assigned sub emitter is part of a prefab and can therefore not be assigned.", new object[0]);
                EditorUtility.DisplayDialog("Invalid Sub Emitter", message, "Ok");
                return false;
            }
            if (!root.gameObject.activeInHierarchy && subEmitter.gameObject.activeInHierarchy)
            {
                string str2 = string.Format("The assigned sub emitter is part of a scene object and can therefore not be assigned to a prefab.", new object[0]);
                EditorUtility.DisplayDialog("Invalid Sub Emitter", str2, "Ok");
                return false;
            }
            return true;
        }

        public enum SubEmitterType
        {
            Birth = 0,
            Collision = 1,
            Death = 2,
            None = -1,
            TypesMax = 3
        }

        private class Texts
        {
            public GUIContent create = new GUIContent(string.Empty, "Create and assign a Particle System as sub emitter");
            public GUIContent[] subEmitterTypeTexts = new GUIContent[] { new GUIContent("Birth", "Start spawning on birth of particle."), new GUIContent("Collision", "Spawn on collision of particle. Sub emitter can only emit as burst."), new GUIContent("Death", "Spawn on death of particle. Sub emitter can only emit as burst.") };
        }
    }
}

