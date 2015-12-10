namespace UnityEditor.Animations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngineInternal;

    public sealed class AnimatorStateMachine : Object
    {
        private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

        public AnimatorStateMachine()
        {
            Internal_Create(this);
        }

        private AnimatorStateTransition AddAnyStateTransition()
        {
            this.undoHandler.DoUndo(this, "AnyState Transition Added");
            AnimatorStateTransition[] anyStateTransitions = this.anyStateTransitions;
            AnimatorStateTransition objectToAdd = new AnimatorStateTransition {
                hasExitTime = false,
                hasFixedDuration = true
            };
            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            objectToAdd.hideFlags = HideFlags.HideInHierarchy;
            ArrayUtility.Add<AnimatorStateTransition>(ref anyStateTransitions, objectToAdd);
            this.anyStateTransitions = anyStateTransitions;
            return objectToAdd;
        }

        public AnimatorStateTransition AddAnyStateTransition(AnimatorState destinationState)
        {
            AnimatorStateTransition transition = this.AddAnyStateTransition();
            transition.destinationState = destinationState;
            return transition;
        }

        public AnimatorStateTransition AddAnyStateTransition(AnimatorStateMachine destinationStateMachine)
        {
            AnimatorStateTransition transition = this.AddAnyStateTransition();
            transition.destinationStateMachine = destinationStateMachine;
            return transition;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void AddBehaviour(int instanceID);
        private AnimatorTransition AddEntryTransition()
        {
            this.undoHandler.DoUndo(this, "Entry Transition Added");
            AnimatorTransition[] entryTransitions = this.entryTransitions;
            AnimatorTransition objectToAdd = new AnimatorTransition();
            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            objectToAdd.hideFlags = HideFlags.HideInHierarchy;
            ArrayUtility.Add<AnimatorTransition>(ref entryTransitions, objectToAdd);
            this.entryTransitions = entryTransitions;
            return objectToAdd;
        }

        public AnimatorTransition AddEntryTransition(AnimatorState destinationState)
        {
            AnimatorTransition transition = this.AddEntryTransition();
            transition.destinationState = destinationState;
            return transition;
        }

        public AnimatorTransition AddEntryTransition(AnimatorStateMachine destinationStateMachine)
        {
            AnimatorTransition transition = this.AddEntryTransition();
            transition.destinationStateMachine = destinationStateMachine;
            return transition;
        }

        public AnimatorState AddState(string name)
        {
            return this.AddState(name, (this.states.Length <= 0) ? new Vector3(200f, 0f, 0f) : (this.states[this.states.Length - 1].position + new Vector3(35f, 65f)));
        }

        public AnimatorState AddState(string name, Vector3 position)
        {
            AnimatorState objectToAdd = new AnimatorState {
                hideFlags = HideFlags.HideInHierarchy,
                name = this.MakeUniqueStateName(name)
            };
            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            this.AddState(objectToAdd, position);
            return objectToAdd;
        }

        public void AddState(AnimatorState state, Vector3 position)
        {
            this.undoHandler.DoUndo(this, "State added");
            ChildAnimatorState item = new ChildAnimatorState {
                state = state,
                position = position
            };
            ChildAnimatorState[] states = this.states;
            ArrayUtility.Add<ChildAnimatorState>(ref states, item);
            this.states = states;
        }

        public AnimatorStateMachine AddStateMachine(string name)
        {
            return this.AddStateMachine(name, Vector3.zero);
        }

        public AnimatorStateMachine AddStateMachine(string name, Vector3 position)
        {
            AnimatorStateMachine stateMachine = new AnimatorStateMachine {
                hideFlags = HideFlags.HideInHierarchy,
                name = this.MakeUniqueStateMachineName(name)
            };
            this.AddStateMachine(stateMachine, position);
            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                AssetDatabase.AddObjectToAsset(stateMachine, AssetDatabase.GetAssetPath(this));
            }
            return stateMachine;
        }

        public void AddStateMachine(AnimatorStateMachine stateMachine, Vector3 position)
        {
            this.undoHandler.DoUndo(this, "StateMachine " + stateMachine.name + " added");
            ChildAnimatorStateMachine item = new ChildAnimatorStateMachine {
                stateMachine = stateMachine,
                position = position
            };
            ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
            ArrayUtility.Add<ChildAnimatorStateMachine>(ref stateMachines, item);
            this.stateMachines = stateMachines;
        }

        public T AddStateMachineBehaviour<T>() where T: StateMachineBehaviour
        {
            return (this.AddStateMachineBehaviour(typeof(T)) as T);
        }

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public StateMachineBehaviour AddStateMachineBehaviour(Type stateMachineBehaviourType)
        {
            return (StateMachineBehaviour) this.Internal_AddStateMachineBehaviourWithType(stateMachineBehaviourType);
        }

        public AnimatorTransition AddStateMachineExitTransition(AnimatorStateMachine sourceStateMachine)
        {
            AnimatorTransition transition = this.AddStateMachineTransition(sourceStateMachine);
            transition.isExit = true;
            return transition;
        }

        public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine sourceStateMachine)
        {
            AnimatorStateMachine destinationStateMachine = null;
            return this.AddStateMachineTransition(sourceStateMachine, destinationStateMachine);
        }

        public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine sourceStateMachine, AnimatorState destinationState)
        {
            AnimatorTransition transition = this.AddStateMachineTransition(sourceStateMachine);
            transition.destinationState = destinationState;
            return transition;
        }

        public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine sourceStateMachine, AnimatorStateMachine destinationStateMachine)
        {
            this.undoHandler.DoUndo(this, "StateMachine Transition Added");
            AnimatorTransition[] stateMachineTransitions = this.GetStateMachineTransitions(sourceStateMachine);
            AnimatorTransition objectToAdd = new AnimatorTransition();
            if (destinationStateMachine != null)
            {
                objectToAdd.destinationStateMachine = destinationStateMachine;
            }
            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            objectToAdd.hideFlags = HideFlags.HideInHierarchy;
            ArrayUtility.Add<AnimatorTransition>(ref stateMachineTransitions, objectToAdd);
            this.SetStateMachineTransitions(sourceStateMachine, stateMachineTransitions);
            return objectToAdd;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void Clear();
        internal AnimatorStateMachine FindParent(AnimatorStateMachine stateMachine)
        {
            <FindParent>c__AnonStorey1F storeyf = new <FindParent>c__AnonStorey1F {
                stateMachine = stateMachine
            };
            if (this.stateMachines.Any<ChildAnimatorStateMachine>(new Func<ChildAnimatorStateMachine, bool>(storeyf.<>m__29)))
            {
                return this;
            }
            return this.stateMachinesRecursive.Find(new Predicate<ChildAnimatorStateMachine>(storeyf.<>m__2A)).stateMachine;
        }

        internal ChildAnimatorState FindState(int nameHash)
        {
            <FindState>c__AnonStorey19 storey = new <FindState>c__AnonStorey19 {
                nameHash = nameHash
            };
            return new List<ChildAnimatorState>(this.states).Find(new Predicate<ChildAnimatorState>(storey.<>m__22));
        }

        internal ChildAnimatorState FindState(string name)
        {
            <FindState>c__AnonStorey1A storeya = new <FindState>c__AnonStorey1A {
                name = name
            };
            return new List<ChildAnimatorState>(this.states).Find(new Predicate<ChildAnimatorState>(storeya.<>m__23));
        }

        internal AnimatorStateMachine FindStateMachine(string path)
        {
            <FindStateMachine>c__AnonStorey20 storey = new <FindStateMachine>c__AnonStorey20();
            char[] separator = new char[] { '.' };
            storey.smNames = path.Split(separator);
            AnimatorStateMachine machine = this;
            <FindStateMachine>c__AnonStorey21 storey2 = new <FindStateMachine>c__AnonStorey21 {
                <>f__ref$32 = storey,
                i = 1
            };
            while ((storey2.i < (storey.smNames.Length - 1)) && (machine != null))
            {
                int index = Array.FindIndex<ChildAnimatorStateMachine>(machine.stateMachines, new Predicate<ChildAnimatorStateMachine>(storey2.<>m__2B));
                machine = (index < 0) ? null : machine.stateMachines[index].stateMachine;
                storey2.i++;
            }
            return ((machine != null) ? machine : this);
        }

        internal AnimatorStateMachine FindStateMachine(AnimatorState state)
        {
            <FindStateMachine>c__AnonStorey22 storey = new <FindStateMachine>c__AnonStorey22 {
                state = state
            };
            if (this.HasState(storey.state, false))
            {
                return this;
            }
            List<ChildAnimatorStateMachine> stateMachinesRecursive = this.stateMachinesRecursive;
            int num = stateMachinesRecursive.FindIndex(new Predicate<ChildAnimatorStateMachine>(storey.<>m__2C));
            return ((num < 0) ? null : stateMachinesRecursive[num].stateMachine);
        }

        internal AnimatorStateTransition FindTransition(AnimatorState destinationState)
        {
            <FindTransition>c__AnonStorey23 storey = new <FindTransition>c__AnonStorey23 {
                destinationState = destinationState
            };
            return new List<AnimatorStateTransition>(this.anyStateTransitions).Find(new Predicate<AnimatorStateTransition>(storey.<>m__2D));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern MonoScript GetBehaviourMonoScript(int index);
        internal Vector3 GetStateMachinePosition(AnimatorStateMachine stateMachine)
        {
            ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
            for (int i = 0; i < stateMachines.Length; i++)
            {
                if (stateMachine == stateMachines[i].stateMachine)
                {
                    return stateMachines[i].position;
                }
            }
            return Vector3.zero;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorTransition[] GetStateMachineTransitions(AnimatorStateMachine sourceStateMachine);
        internal Vector3 GetStatePosition(AnimatorState state)
        {
            ChildAnimatorState[] states = this.states;
            for (int i = 0; i < states.Length; i++)
            {
                if (state == states[i].state)
                {
                    return states[i].position;
                }
            }
            return Vector3.zero;
        }

        [Obsolete("GetTransitionsFromState is obsolete. Use AnimatorState.transitions instead.", true)]
        private AnimatorState GetTransitionsFromState(AnimatorState state)
        {
            return null;
        }

        internal bool HasState(AnimatorState state)
        {
            <HasState>c__AnonStorey1B storeyb = new <HasState>c__AnonStorey1B {
                state = state
            };
            return this.statesRecursive.Any<ChildAnimatorState>(new Func<ChildAnimatorState, bool>(storeyb.<>m__24));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool HasState(AnimatorState state, bool recursive);
        internal bool HasStateMachine(AnimatorStateMachine child)
        {
            <HasStateMachine>c__AnonStorey1D storeyd = new <HasStateMachine>c__AnonStorey1D {
                child = child
            };
            return this.stateMachinesRecursive.Any<ChildAnimatorStateMachine>(new Func<ChildAnimatorStateMachine, bool>(storeyd.<>m__26));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool HasStateMachine(AnimatorStateMachine state, bool recursive);
        internal bool HasTransition(AnimatorState stateA, AnimatorState stateB)
        {
            <HasTransition>c__AnonStorey1E storeye = new <HasTransition>c__AnonStorey1E {
                stateB = stateB,
                stateA = stateA
            };
            return (storeye.stateA.transitions.Any<AnimatorStateTransition>(new Func<AnimatorStateTransition, bool>(storeye.<>m__27)) || storeye.stateB.transitions.Any<AnimatorStateTransition>(new Func<AnimatorStateTransition, bool>(storeye.<>m__28)));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern ScriptableObject Internal_AddStateMachineBehaviourWithType(Type stateMachineBehaviourType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create(AnimatorStateMachine mono);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_anyStatePosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_entryPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_exitPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_parentStateMachinePosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_anyStatePosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_entryPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_exitPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_parentStateMachinePosition(ref Vector3 value);
        internal bool IsDirectParent(AnimatorStateMachine stateMachine)
        {
            <IsDirectParent>c__AnonStorey1C storeyc = new <IsDirectParent>c__AnonStorey1C {
                stateMachine = stateMachine
            };
            return this.stateMachines.Any<ChildAnimatorStateMachine>(new Func<ChildAnimatorStateMachine, bool>(storeyc.<>m__25));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string MakeUniqueStateMachineName(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string MakeUniqueStateName(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void MoveState(AnimatorState state, AnimatorStateMachine target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void MoveStateMachine(AnimatorStateMachine stateMachine, AnimatorStateMachine target);
        public bool RemoveAnyStateTransition(AnimatorStateTransition transition)
        {
            <RemoveAnyStateTransition>c__AnonStorey17 storey = new <RemoveAnyStateTransition>c__AnonStorey17 {
                transition = transition
            };
            if (!new List<AnimatorStateTransition>(this.anyStateTransitions).Any<AnimatorStateTransition>(new Func<AnimatorStateTransition, bool>(storey.<>m__20)))
            {
                return false;
            }
            this.undoHandler.DoUndo(this, "AnyState Transition Removed");
            AnimatorStateTransition[] anyStateTransitions = this.anyStateTransitions;
            ArrayUtility.Remove<AnimatorStateTransition>(ref anyStateTransitions, storey.transition);
            this.anyStateTransitions = anyStateTransitions;
            if (MecanimUtilities.AreSameAsset(this, storey.transition))
            {
                Undo.DestroyObjectImmediate(storey.transition);
            }
            return true;
        }

        internal void RemoveAnyStateTransitionRecursive(AnimatorStateTransition transition)
        {
            if (!this.RemoveAnyStateTransition(transition))
            {
                foreach (ChildAnimatorStateMachine machine in this.stateMachinesRecursive)
                {
                    if (machine.stateMachine.RemoveAnyStateTransition(transition))
                    {
                        break;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RemoveBehaviour(int index);
        public bool RemoveEntryTransition(AnimatorTransition transition)
        {
            <RemoveEntryTransition>c__AnonStorey18 storey = new <RemoveEntryTransition>c__AnonStorey18 {
                transition = transition
            };
            if (!new List<AnimatorTransition>(this.entryTransitions).Any<AnimatorTransition>(new Func<AnimatorTransition, bool>(storey.<>m__21)))
            {
                return false;
            }
            this.undoHandler.DoUndo(this, "Entry Transition Removed");
            AnimatorTransition[] entryTransitions = this.entryTransitions;
            ArrayUtility.Remove<AnimatorTransition>(ref entryTransitions, storey.transition);
            this.entryTransitions = entryTransitions;
            if (MecanimUtilities.AreSameAsset(this, storey.transition))
            {
                Undo.DestroyObjectImmediate(storey.transition);
            }
            return true;
        }

        public void RemoveState(AnimatorState state)
        {
            this.undoHandler.DoUndo(this, "State removed");
            this.undoHandler.DoUndo(state, "State removed");
            this.RemoveStateInternal(state);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RemoveStateInternal(AnimatorState state);
        public void RemoveStateMachine(AnimatorStateMachine stateMachine)
        {
            this.undoHandler.DoUndo(this, "StateMachine removed");
            this.undoHandler.DoUndo(stateMachine, "StateMachine removed");
            this.RemoveStateMachineInternal(stateMachine);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RemoveStateMachineInternal(AnimatorStateMachine stateMachine);
        public bool RemoveStateMachineTransition(AnimatorStateMachine sourceStateMachine, AnimatorTransition transition)
        {
            this.undoHandler.DoUndo(this, "StateMachine Transition Removed");
            AnimatorTransition[] stateMachineTransitions = this.GetStateMachineTransitions(sourceStateMachine);
            int length = stateMachineTransitions.Length;
            ArrayUtility.Remove<AnimatorTransition>(ref stateMachineTransitions, transition);
            this.SetStateMachineTransitions(sourceStateMachine, stateMachineTransitions);
            if (MecanimUtilities.AreSameAsset(this, transition))
            {
                Undo.DestroyObjectImmediate(transition);
            }
            return (length != stateMachineTransitions.Length);
        }

        internal void SetStateMachinePosition(AnimatorStateMachine stateMachine, Vector3 position)
        {
            ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
            for (int i = 0; i < stateMachines.Length; i++)
            {
                if (stateMachine == stateMachines[i].stateMachine)
                {
                    stateMachines[i].position = position;
                    this.stateMachines = stateMachines;
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetStateMachineTransitions(AnimatorStateMachine sourceStateMachine, AnimatorTransition[] transitions);
        internal void SetStatePosition(AnimatorState state, Vector3 position)
        {
            ChildAnimatorState[] states = this.states;
            for (int i = 0; i < states.Length; i++)
            {
                if (state == states[i].state)
                {
                    states[i].position = position;
                    this.states = states;
                    return;
                }
            }
        }

        public Vector3 anyStatePosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_anyStatePosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_anyStatePosition(ref value);
            }
        }

        public AnimatorStateTransition[] anyStateTransitions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal List<AnimatorStateTransition> anyStateTransitionsRecursive
        {
            get
            {
                List<AnimatorStateTransition> list = new List<AnimatorStateTransition>();
                list.AddRange(this.anyStateTransitions);
                foreach (ChildAnimatorStateMachine machine in this.stateMachines)
                {
                    list.AddRange(machine.stateMachine.anyStateTransitionsRecursive);
                }
                return list;
            }
        }

        public StateMachineBehaviour[] behaviours { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AnimatorState defaultState { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 entryPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_entryPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_entryPosition(ref value);
            }
        }

        public AnimatorTransition[] entryTransitions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 exitPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_exitPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_exitPosition(ref value);
            }
        }

        public Vector3 parentStateMachinePosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_parentStateMachinePosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_parentStateMachinePosition(ref value);
            }
        }

        internal bool pushUndo
        {
            set
            {
                this.undoHandler.pushUndo = value;
            }
        }

        [Obsolete("stateCount is obsolete. Use .states.Length  instead.", true)]
        private int stateCount
        {
            get
            {
                return 0;
            }
        }

        [Obsolete("stateMachineCount is obsolete. Use .stateMachines.Length instead.", true)]
        private int stateMachineCount
        {
            get
            {
                return 0;
            }
        }

        public ChildAnimatorStateMachine[] stateMachines { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal List<ChildAnimatorStateMachine> stateMachinesRecursive
        {
            get
            {
                List<ChildAnimatorStateMachine> list = new List<ChildAnimatorStateMachine>();
                list.AddRange(this.stateMachines);
                for (int i = 0; i < this.stateMachines.Length; i++)
                {
                    list.AddRange(this.stateMachines[i].stateMachine.stateMachinesRecursive);
                }
                return list;
            }
        }

        public ChildAnimatorState[] states { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal List<ChildAnimatorState> statesRecursive
        {
            get
            {
                List<ChildAnimatorState> list = new List<ChildAnimatorState>();
                list.AddRange(this.states);
                for (int i = 0; i < this.stateMachines.Length; i++)
                {
                    list.AddRange(this.stateMachines[i].stateMachine.statesRecursive);
                }
                return list;
            }
        }

        internal int transitionCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("uniqueNameHash does not exist anymore.", true)]
        private int uniqueNameHash
        {
            get
            {
                return -1;
            }
        }

        [CompilerGenerated]
        private sealed class <FindParent>c__AnonStorey1F
        {
            internal AnimatorStateMachine stateMachine;

            internal bool <>m__29(ChildAnimatorStateMachine childSM)
            {
                return (childSM.stateMachine == this.stateMachine);
            }

            internal bool <>m__2A(ChildAnimatorStateMachine sm)
            {
                return sm.stateMachine.stateMachines.Any<ChildAnimatorStateMachine>(childSM => (childSM.stateMachine == this.stateMachine));
            }

            internal bool <>m__2E(ChildAnimatorStateMachine childSM)
            {
                return (childSM.stateMachine == this.stateMachine);
            }
        }

        [CompilerGenerated]
        private sealed class <FindState>c__AnonStorey19
        {
            internal int nameHash;

            internal bool <>m__22(ChildAnimatorState s)
            {
                return (s.state.nameHash == this.nameHash);
            }
        }

        [CompilerGenerated]
        private sealed class <FindState>c__AnonStorey1A
        {
            internal string name;

            internal bool <>m__23(ChildAnimatorState s)
            {
                return (s.state.name == this.name);
            }
        }

        [CompilerGenerated]
        private sealed class <FindStateMachine>c__AnonStorey20
        {
            internal string[] smNames;
        }

        [CompilerGenerated]
        private sealed class <FindStateMachine>c__AnonStorey21
        {
            internal AnimatorStateMachine.<FindStateMachine>c__AnonStorey20 <>f__ref$32;
            internal int i;

            internal bool <>m__2B(ChildAnimatorStateMachine t)
            {
                return (t.stateMachine.name == this.<>f__ref$32.smNames[this.i]);
            }
        }

        [CompilerGenerated]
        private sealed class <FindStateMachine>c__AnonStorey22
        {
            internal AnimatorState state;

            internal bool <>m__2C(ChildAnimatorStateMachine sm)
            {
                return sm.stateMachine.HasState(this.state, false);
            }
        }

        [CompilerGenerated]
        private sealed class <FindTransition>c__AnonStorey23
        {
            internal AnimatorState destinationState;

            internal bool <>m__2D(AnimatorStateTransition t)
            {
                return (t.destinationState == this.destinationState);
            }
        }

        [CompilerGenerated]
        private sealed class <HasState>c__AnonStorey1B
        {
            internal AnimatorState state;

            internal bool <>m__24(ChildAnimatorState s)
            {
                return (s.state == this.state);
            }
        }

        [CompilerGenerated]
        private sealed class <HasStateMachine>c__AnonStorey1D
        {
            internal AnimatorStateMachine child;

            internal bool <>m__26(ChildAnimatorStateMachine sm)
            {
                return (sm.stateMachine == this.child);
            }
        }

        [CompilerGenerated]
        private sealed class <HasTransition>c__AnonStorey1E
        {
            internal AnimatorState stateA;
            internal AnimatorState stateB;

            internal bool <>m__27(AnimatorStateTransition t)
            {
                return (t.destinationState == this.stateB);
            }

            internal bool <>m__28(AnimatorStateTransition t)
            {
                return (t.destinationState == this.stateA);
            }
        }

        [CompilerGenerated]
        private sealed class <IsDirectParent>c__AnonStorey1C
        {
            internal AnimatorStateMachine stateMachine;

            internal bool <>m__25(ChildAnimatorStateMachine sm)
            {
                return (sm.stateMachine == this.stateMachine);
            }
        }

        [CompilerGenerated]
        private sealed class <RemoveAnyStateTransition>c__AnonStorey17
        {
            internal AnimatorStateTransition transition;

            internal bool <>m__20(AnimatorStateTransition t)
            {
                return (t == this.transition);
            }
        }

        [CompilerGenerated]
        private sealed class <RemoveEntryTransition>c__AnonStorey18
        {
            internal AnimatorTransition transition;

            internal bool <>m__21(AnimatorTransition t)
            {
                return (t == this.transition);
            }
        }
    }
}

