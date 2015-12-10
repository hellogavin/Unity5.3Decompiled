namespace UnityEngine
{
    using System;
    using UnityEngine.Experimental.Director;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public abstract class StateMachineBehaviour : ScriptableObject
    {
        protected StateMachineBehaviour()
        {
        }

        public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
        }

        public virtual void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
        }

        public virtual void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
        }

        public virtual void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
        }

        public virtual void OnStateMachineEnter(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
        {
        }

        public virtual void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
        }

        public virtual void OnStateMachineExit(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
        {
        }

        public virtual void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
        }

        public virtual void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
        }
    }
}

