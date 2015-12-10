namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    [Obsolete("StateMachine is obsolete. Use UnityEditor.Animations.AnimatorStateMachine instead (UnityUpgradable) -> UnityEditor.Animations.AnimatorStateMachine", true)]
    public class StateMachine : Object
    {
        public Transition AddAnyStateTransition(State dst)
        {
            return null;
        }

        public State AddState(string stateName)
        {
            return null;
        }

        public StateMachine AddStateMachine(string stateMachineName)
        {
            return null;
        }

        public Transition AddTransition(State src, State dst)
        {
            return null;
        }

        public State GetState(int index)
        {
            return null;
        }

        public StateMachine GetStateMachine(int index)
        {
            return null;
        }

        public Vector3 GetStateMachinePosition(int i)
        {
            return new Vector3();
        }

        public Transition[] GetTransitionsFromState(State srcState)
        {
            return null;
        }

        public Vector3 anyStatePosition
        {
            get
            {
                return new Vector3();
            }
            set
            {
            }
        }

        public State defaultState
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public Vector3 parentStateMachinePosition
        {
            get
            {
                return new Vector3();
            }
            set
            {
            }
        }
    }
}

