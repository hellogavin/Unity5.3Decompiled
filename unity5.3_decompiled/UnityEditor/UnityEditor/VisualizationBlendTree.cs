namespace UnityEditor
{
    using System;
    using UnityEditor.Animations;
    using UnityEngine;

    internal class VisualizationBlendTree
    {
        private Animator m_Animator;
        private BlendTree m_BlendTree;
        private AnimatorController m_Controller;
        private bool m_ControllerIsDirty;
        private AnimatorState m_State;
        private AnimatorStateMachine m_StateMachine;

        private void ClearStateMachine()
        {
            if (this.m_Animator != null)
            {
                AnimatorController.SetAnimatorController(this.m_Animator, null);
            }
            if (this.m_Controller != null)
            {
                this.m_Controller.OnAnimatorControllerDirty = (Action) Delegate.Remove(this.m_Controller.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
            }
            Object.DestroyImmediate(this.m_Controller);
            Object.DestroyImmediate(this.m_State);
            this.m_StateMachine = null;
            this.m_Controller = null;
            this.m_State = null;
        }

        protected virtual void ControllerDirty()
        {
            this.m_ControllerIsDirty = true;
        }

        private void CreateParameters()
        {
            for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
            {
                this.m_Controller.AddParameter(this.m_BlendTree.GetRecursiveBlendParameter(i), AnimatorControllerParameterType.Float);
            }
        }

        private void CreateStateMachine()
        {
            if (this.m_Controller == null)
            {
                this.m_Controller = new AnimatorController();
                this.m_Controller.pushUndo = false;
                this.m_Controller.AddLayer("viz");
                this.m_StateMachine = this.m_Controller.layers[0].stateMachine;
                this.m_StateMachine.pushUndo = false;
                this.CreateParameters();
                this.m_State = this.m_StateMachine.AddState("viz");
                this.m_State.pushUndo = false;
                this.m_State.motion = this.m_BlendTree;
                this.m_State.iKOnFeet = false;
                this.m_State.hideFlags = HideFlags.HideAndDontSave;
                this.m_StateMachine.hideFlags = HideFlags.HideAndDontSave;
                this.m_Controller.hideFlags = HideFlags.HideAndDontSave;
                AnimatorController.SetAnimatorController(this.m_Animator, this.m_Controller);
                this.m_Controller.OnAnimatorControllerDirty = (Action) Delegate.Combine(this.m_Controller.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
                this.m_ControllerIsDirty = false;
            }
        }

        public void Destroy()
        {
            this.ClearStateMachine();
        }

        public void Init(BlendTree blendTree, Animator animator)
        {
            this.m_BlendTree = blendTree;
            this.m_Animator = animator;
            this.m_Animator.logWarnings = false;
            this.m_Animator.fireEvents = false;
            this.m_Animator.enabled = false;
            this.m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            this.CreateStateMachine();
        }

        public void Reset()
        {
            this.ClearStateMachine();
            this.CreateStateMachine();
        }

        public void Update()
        {
            if (this.m_ControllerIsDirty)
            {
                this.Reset();
            }
            int recursiveBlendParameterCount = this.m_BlendTree.recursiveBlendParameterCount;
            if (this.m_Controller.parameters.Length >= recursiveBlendParameterCount)
            {
                for (int i = 0; i < recursiveBlendParameterCount; i++)
                {
                    string recursiveBlendParameter = this.m_BlendTree.GetRecursiveBlendParameter(i);
                    float inputBlendValue = this.m_BlendTree.GetInputBlendValue(recursiveBlendParameter);
                    this.animator.SetFloat(recursiveBlendParameter, inputBlendValue);
                }
                this.animator.EvaluateController();
            }
        }

        public Animator animator
        {
            get
            {
                return this.m_Animator;
            }
        }

        public bool controllerDirty
        {
            get
            {
                return this.m_ControllerIsDirty;
            }
        }
    }
}

