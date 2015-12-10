namespace UnityEditor
{
    using System;
    using UnityEditor.Animations;
    using UnityEngine;

    internal class PreviewBlendTree
    {
        private AvatarPreview m_AvatarPreview;
        private BlendTree m_BlendTree;
        private AnimatorController m_Controller;
        private bool m_ControllerIsDirty;
        private bool m_PrevIKOnFeet;
        private AnimatorState m_State;
        private AnimatorStateMachine m_StateMachine;

        private void ClearStateMachine()
        {
            if ((this.m_AvatarPreview != null) && (this.m_AvatarPreview.Animator != null))
            {
                AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, null);
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

        public void CreateParameters()
        {
            for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
            {
                this.m_Controller.AddParameter(this.m_BlendTree.GetRecursiveBlendParameter(i), AnimatorControllerParameterType.Float);
            }
        }

        private void CreateStateMachine()
        {
            if ((this.m_AvatarPreview != null) && (this.m_AvatarPreview.Animator != null))
            {
                if (this.m_Controller == null)
                {
                    this.m_Controller = new AnimatorController();
                    this.m_Controller.pushUndo = false;
                    this.m_Controller.AddLayer("preview");
                    this.m_StateMachine = this.m_Controller.layers[0].stateMachine;
                    this.m_StateMachine.pushUndo = false;
                    this.CreateParameters();
                    this.m_State = this.m_StateMachine.AddState("preview");
                    this.m_State.pushUndo = false;
                    this.m_State.motion = this.m_BlendTree;
                    this.m_State.iKOnFeet = this.m_AvatarPreview.IKOnFeet;
                    this.m_State.hideFlags = HideFlags.HideAndDontSave;
                    this.m_Controller.hideFlags = HideFlags.HideAndDontSave;
                    this.m_StateMachine.hideFlags = HideFlags.HideAndDontSave;
                    AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
                    this.m_Controller.OnAnimatorControllerDirty = (Action) Delegate.Combine(this.m_Controller.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
                    this.m_ControllerIsDirty = false;
                }
                if (AnimatorController.GetEffectiveAnimatorController(this.m_AvatarPreview.Animator) != this.m_Controller)
                {
                    AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
                }
            }
        }

        public bool HasPreviewGUI()
        {
            return true;
        }

        public void Init(BlendTree blendTree, Animator animator)
        {
            this.m_BlendTree = blendTree;
            if (this.m_AvatarPreview == null)
            {
                this.m_AvatarPreview = new AvatarPreview(animator, this.m_BlendTree);
                this.m_AvatarPreview.OnAvatarChangeFunc = new AvatarPreview.OnAvatarChange(this.OnPreviewAvatarChanged);
                this.m_PrevIKOnFeet = this.m_AvatarPreview.IKOnFeet;
            }
            this.CreateStateMachine();
        }

        public void OnDestroy()
        {
            this.ClearStateMachine();
            if (this.m_AvatarPreview != null)
            {
                this.m_AvatarPreview.OnDestroy();
                this.m_AvatarPreview = null;
            }
        }

        public void OnDisable()
        {
            this.ClearStateMachine();
            this.m_AvatarPreview.OnDestroy();
        }

        public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            this.UpdateAvatarState();
            this.m_AvatarPreview.DoAvatarPreview(r, background);
        }

        private void OnPreviewAvatarChanged()
        {
            this.ResetStateMachine();
        }

        public void OnPreviewSettings()
        {
            this.m_AvatarPreview.DoPreviewSettings();
        }

        public void ResetStateMachine()
        {
            this.ClearStateMachine();
            this.CreateStateMachine();
        }

        public void TestForReset()
        {
            if (((this.m_State != null) && (this.m_AvatarPreview != null)) && (this.m_State.iKOnFeet != this.m_AvatarPreview.IKOnFeet))
            {
                this.ResetStateMachine();
            }
        }

        private void UpdateAvatarState()
        {
            if (Event.current.type == EventType.Repaint)
            {
                if ((this.m_AvatarPreview.PreviewObject == null) || this.m_ControllerIsDirty)
                {
                    this.m_AvatarPreview.ResetPreviewInstance();
                    if (this.m_AvatarPreview.PreviewObject != null)
                    {
                        this.ResetStateMachine();
                    }
                }
                if (this.m_AvatarPreview.Animator != null)
                {
                    if (this.m_PrevIKOnFeet != this.m_AvatarPreview.IKOnFeet)
                    {
                        this.m_PrevIKOnFeet = this.m_AvatarPreview.IKOnFeet;
                        Vector3 rootPosition = this.m_AvatarPreview.Animator.rootPosition;
                        Quaternion rootRotation = this.m_AvatarPreview.Animator.rootRotation;
                        this.ResetStateMachine();
                        this.m_AvatarPreview.Animator.Update(this.m_AvatarPreview.timeControl.currentTime);
                        this.m_AvatarPreview.Animator.Update(0f);
                        this.m_AvatarPreview.Animator.rootPosition = rootPosition;
                        this.m_AvatarPreview.Animator.rootRotation = rootRotation;
                    }
                    if (this.m_AvatarPreview.Animator != null)
                    {
                        for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
                        {
                            string recursiveBlendParameter = this.m_BlendTree.GetRecursiveBlendParameter(i);
                            float inputBlendValue = this.m_BlendTree.GetInputBlendValue(recursiveBlendParameter);
                            this.m_AvatarPreview.Animator.SetFloat(recursiveBlendParameter, inputBlendValue);
                        }
                    }
                    this.m_AvatarPreview.timeControl.loop = true;
                    float length = 1f;
                    float normalizedTime = 0f;
                    if (this.m_AvatarPreview.Animator.layerCount > 0)
                    {
                        AnimatorStateInfo currentAnimatorStateInfo = this.m_AvatarPreview.Animator.GetCurrentAnimatorStateInfo(0);
                        length = currentAnimatorStateInfo.length;
                        normalizedTime = currentAnimatorStateInfo.normalizedTime;
                    }
                    this.m_AvatarPreview.timeControl.startTime = 0f;
                    this.m_AvatarPreview.timeControl.stopTime = length;
                    this.m_AvatarPreview.timeControl.Update();
                    float deltaTime = this.m_AvatarPreview.timeControl.deltaTime;
                    if (!this.m_BlendTree.isLooping)
                    {
                        if (normalizedTime >= 1f)
                        {
                            deltaTime -= length;
                        }
                        else if (normalizedTime < 0f)
                        {
                            deltaTime += length;
                        }
                    }
                    this.m_AvatarPreview.Animator.Update(deltaTime);
                }
            }
        }

        public Animator PreviewAnimator
        {
            get
            {
                return this.m_AvatarPreview.Animator;
            }
        }
    }
}

