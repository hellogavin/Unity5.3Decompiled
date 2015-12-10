namespace UnityEditor.Animations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Experimental.Director;
    using UnityEngineInternal;

    public sealed class AnimatorController : RuntimeAnimatorController
    {
        private const string kControllerExtension = "controller";
        internal static AnimatorController lastActiveController;
        internal static int lastActiveLayerIndex;
        internal Action OnAnimatorControllerDirty;
        internal PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

        public AnimatorController()
        {
            Internal_Create(this);
        }

        public T AddEffectiveStateMachineBehaviour<T>(AnimatorState state, int layerIndex) where T: StateMachineBehaviour
        {
            return (this.AddEffectiveStateMachineBehaviour(typeof(T), state, layerIndex) as T);
        }

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public StateMachineBehaviour AddEffectiveStateMachineBehaviour(Type stateMachineBehaviourType, AnimatorState state, int layerIndex)
        {
            return (StateMachineBehaviour) this.Internal_AddStateMachineBehaviourWithType(stateMachineBehaviourType, state, layerIndex);
        }

        public void AddLayer(string name)
        {
            AnimatorControllerLayer layer = new AnimatorControllerLayer {
                name = this.MakeUniqueLayerName(name),
                stateMachine = new AnimatorStateMachine()
            };
            layer.stateMachine.name = layer.name;
            layer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(this));
            }
            this.AddLayer(layer);
        }

        public void AddLayer(AnimatorControllerLayer layer)
        {
            this.undoHandler.DoUndo(this, "Layer added");
            AnimatorControllerLayer[] layers = this.layers;
            ArrayUtility.Add<AnimatorControllerLayer>(ref layers, layer);
            this.layers = layers;
        }

        public AnimatorState AddMotion(Motion motion)
        {
            return this.AddMotion(motion, 0);
        }

        public AnimatorState AddMotion(Motion motion, int layerIndex)
        {
            AnimatorState state = this.layers[layerIndex].stateMachine.AddState(motion.name);
            state.motion = motion;
            return state;
        }

        public void AddParameter(AnimatorControllerParameter paramater)
        {
            this.undoHandler.DoUndo(this, "Parameter added");
            AnimatorControllerParameter[] parameters = this.parameters;
            ArrayUtility.Add<AnimatorControllerParameter>(ref parameters, paramater);
            this.parameters = parameters;
        }

        public void AddParameter(string name, AnimatorControllerParameterType type)
        {
            AnimatorControllerParameter paramater = new AnimatorControllerParameter {
                name = this.MakeUniqueParameterName(name),
                type = type
            };
            this.AddParameter(paramater);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void AddStateEffectiveBehaviour(AnimatorState state, int layerIndex, int instanceID);
        public static AnimationClip AllocateAnimatorClip(string name)
        {
            AnimationClip clip = AnimationWindowUtility.AllocateAndSetupClip(true);
            clip.name = name;
            return clip;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool CanAddStateMachineBehaviours();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern Object[] CollectObjectsUsingParameter(string parameterName);
        internal static T[] ConvertStateMachineBehaviour<T>(ScriptableObject[] rawObjects) where T: StateMachineBehaviour
        {
            if (rawObjects == null)
            {
                return null;
            }
            T[] localArray = new T[rawObjects.Length];
            for (int i = 0; i < localArray.Length; i++)
            {
                localArray[i] = (T) rawObjects[i];
            }
            return localArray;
        }

        public static AnimatorController CreateAnimatorControllerAtPath(string path)
        {
            AnimatorController asset = new AnimatorController {
                name = Path.GetFileName(path)
            };
            AssetDatabase.CreateAsset(asset, path);
            asset.pushUndo = false;
            asset.AddLayer("Base Layer");
            asset.pushUndo = true;
            return asset;
        }

        public static AnimatorController CreateAnimatorControllerAtPathWithClip(string path, AnimationClip clip)
        {
            AnimatorController controller = CreateAnimatorControllerAtPath(path);
            controller.AddMotion(clip);
            return controller;
        }

        internal static AnimatorController CreateAnimatorControllerForClip(AnimationClip clip, GameObject animatedObject)
        {
            string assetPath = AssetDatabase.GetAssetPath(clip);
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }
            assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(FileUtil.DeleteLastPathNameComponent(assetPath), animatedObject.name + ".controller"));
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }
            return CreateAnimatorControllerAtPathWithClip(assetPath, clip);
        }

        public AnimatorState CreateBlendTreeInController(string name, out BlendTree tree)
        {
            return this.CreateBlendTreeInController(name, out tree, 0);
        }

        public AnimatorState CreateBlendTreeInController(string name, out BlendTree tree, int layerIndex)
        {
            tree = new BlendTree();
            tree.name = name;
            string defaultBlendTreeParameter = this.GetDefaultBlendTreeParameter();
            tree.blendParameterY = defaultBlendTreeParameter;
            tree.blendParameter = defaultBlendTreeParameter;
            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                AssetDatabase.AddObjectToAsset(tree, AssetDatabase.GetAssetPath(this));
            }
            AnimatorState state = this.layers[layerIndex].stateMachine.AddState(tree.name);
            state.motion = tree;
            return state;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int CreateStateMachineBehaviour(MonoScript script);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern AnimatorControllerPlayable FindAnimatorControllerPlayable(Animator animator, AnimatorController controller);
        internal AnimatorStateMachine FindEffectiveRootStateMachine(int layerIndex)
        {
            AnimatorControllerLayer layer = this.layers[layerIndex];
            while (layer.syncedLayerIndex != -1)
            {
                layer = this.layers[layer.syncedLayerIndex];
            }
            return layer.stateMachine;
        }

        public static StateMachineBehaviourContext[] FindStateMachineBehaviourContext(StateMachineBehaviour behaviour)
        {
            return Internal_FindStateMachineBehaviourContext(behaviour);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern MonoScript GetBehaviourMonoScript(AnimatorState state, int layerIndex, int behaviourIndex);
        public T[] GetBehaviours<T>() where T: StateMachineBehaviour
        {
            return ConvertStateMachineBehaviour<T>(this.GetBehaviours(typeof(T)));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern ScriptableObject[] GetBehaviours(Type type);
        internal string GetDefaultBlendTreeParameter()
        {
            for (int i = 0; i < this.parameters.Length; i++)
            {
                if (this.parameters[i].type == AnimatorControllerParameterType.Float)
                {
                    return this.parameters[i].name;
                }
            }
            this.AddParameter("Blend", AnimatorControllerParameterType.Float);
            return "Blend";
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern AnimatorController GetEffectiveAnimatorController(Animator animator);
        public StateMachineBehaviour[] GetStateEffectiveBehaviours(AnimatorState state, int layerIndex)
        {
            return this.Internal_GetEffectiveBehaviours(state, layerIndex);
        }

        public Motion GetStateEffectiveMotion(AnimatorState state)
        {
            return this.GetStateEffectiveMotion(state, 0);
        }

        public Motion GetStateEffectiveMotion(AnimatorState state, int layerIndex)
        {
            if (this.layers[layerIndex].syncedLayerIndex == -1)
            {
                return state.motion;
            }
            return this.layers[layerIndex].GetOverrideMotion(state);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern int IndexOfParameter(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern ScriptableObject Internal_AddStateMachineBehaviourWithType(Type stateMachineBehaviourType, AnimatorState state, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create(AnimatorController mono);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern StateMachineBehaviourContext[] Internal_FindStateMachineBehaviourContext(ScriptableObject scriptableObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern StateMachineBehaviour[] Internal_GetEffectiveBehaviours(AnimatorState state, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void Internal_SetEffectiveBehaviours(AnimatorState state, int layerIndex, StateMachineBehaviour[] behaviours);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string MakeUniqueLayerName(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string MakeUniqueParameterName(string name);
        internal static void OnInvalidateAnimatorController(AnimatorController controller)
        {
            if (controller.OnAnimatorControllerDirty != null)
            {
                controller.OnAnimatorControllerDirty();
            }
        }

        public void RemoveLayer(int index)
        {
            this.undoHandler.DoUndo(this, "Layer removed");
            AnimatorControllerLayer[] layers = this.layers;
            this.RemoveLayerInternal(index, ref layers);
            this.layers = layers;
        }

        private void RemoveLayerInternal(int index, ref AnimatorControllerLayer[] layerVector)
        {
            if ((layerVector[index].syncedLayerIndex == -1) && (layerVector[index].stateMachine != null))
            {
                this.undoHandler.DoUndo(layerVector[index].stateMachine, "Layer removed");
                layerVector[index].stateMachine.Clear();
                if (MecanimUtilities.AreSameAsset(this, layerVector[index].stateMachine))
                {
                    Undo.DestroyObjectImmediate(layerVector[index].stateMachine);
                }
            }
            ArrayUtility.Remove<AnimatorControllerLayer>(ref layerVector, layerVector[index]);
        }

        internal void RemoveLayers(List<int> layerIndexes)
        {
            this.undoHandler.DoUndo(this, "Layers removed");
            AnimatorControllerLayer[] layers = this.layers;
            foreach (int num in layerIndexes)
            {
                this.RemoveLayerInternal(num, ref layers);
            }
            this.layers = layers;
        }

        public void RemoveParameter(int index)
        {
            this.undoHandler.DoUndo(this, "Parameter removed");
            AnimatorControllerParameter[] parameters = this.parameters;
            ArrayUtility.Remove<AnimatorControllerParameter>(ref parameters, parameters[index]);
            this.parameters = parameters;
        }

        public void RemoveParameter(AnimatorControllerParameter parameter)
        {
            this.undoHandler.DoUndo(this, "Parameter removed");
            AnimatorControllerParameter[] parameters = this.parameters;
            ArrayUtility.Remove<AnimatorControllerParameter>(ref parameters, parameter);
            this.parameters = parameters;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RemoveStateEffectiveBehaviour(AnimatorState state, int layerIndex, int behaviourIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RenameParameter(string prevName, string newName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAnimatorController(Animator behavior, AnimatorController controller);
        public void SetStateEffectiveBehaviours(AnimatorState state, int layerIndex, StateMachineBehaviour[] behaviours)
        {
            if (this.layers[layerIndex].syncedLayerIndex == -1)
            {
                this.undoHandler.DoUndo(state, "Set Behaviours");
                state.behaviours = behaviours;
            }
            else
            {
                this.undoHandler.DoUndo(this, "Set Behaviours");
                this.Internal_SetEffectiveBehaviours(state, layerIndex, behaviours);
            }
        }

        public void SetStateEffectiveMotion(AnimatorState state, Motion motion)
        {
            this.SetStateEffectiveMotion(state, motion, 0);
        }

        public void SetStateEffectiveMotion(AnimatorState state, Motion motion, int layerIndex)
        {
            if (this.layers[layerIndex].syncedLayerIndex == -1)
            {
                this.undoHandler.DoUndo(state, "Set Motion");
                state.motion = motion;
            }
            else
            {
                this.undoHandler.DoUndo(this, "Set Motion");
                AnimatorControllerLayer[] layers = this.layers;
                layers[layerIndex].SetOverrideMotion(state, motion);
                this.layers = layers;
            }
        }

        internal bool isAssetBundled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("layerCount is obsolete. Use layers.Length instead.", true)]
        private int layerCount
        {
            get
            {
                return 0;
            }
        }

        public AnimatorControllerLayer[] layers { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("parameterCount is obsolete. Use parameters.Length instead.", true)]
        private int parameterCount
        {
            get
            {
                return 0;
            }
        }

        public AnimatorControllerParameter[] parameters { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal bool pushUndo
        {
            set
            {
                this.undoHandler.pushUndo = value;
            }
        }
    }
}

