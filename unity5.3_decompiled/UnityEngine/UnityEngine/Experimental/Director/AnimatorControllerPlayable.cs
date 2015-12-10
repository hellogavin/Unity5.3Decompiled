namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    public sealed class AnimatorControllerPlayable : AnimationPlayable, IAnimatorControllerPlayable
    {
        public AnimatorControllerPlayable(RuntimeAnimatorController controller) : base(false)
        {
            base.m_Ptr = IntPtr.Zero;
            this.InstantiateEnginePlayable(controller);
        }

        public override int AddInput(AnimationPlayable source)
        {
            Debug.LogError("AnimationClipPlayable doesn't support adding inputs");
            return -1;
        }

        [ExcludeFromDocs]
        public void CrossFade(int stateNameHash, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.CrossFade(stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(string stateName, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.CrossFade(stateName, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(int stateNameHash, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.CrossFade(stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(string stateName, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.CrossFade(stateName, transitionDuration, layer, negativeInfinity);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CrossFade(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);
        public void CrossFade(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            this.CrossFade(StringToHash(stateName), transitionDuration, layer, normalizedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(string stateName, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            this.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(string stateName, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            this.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime);
        public void CrossFadeInFixedTime(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
        {
            this.CrossFadeInFixedTime(StringToHash(stateName), transitionDuration, layer, fixedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex);
        public bool GetBool(int id)
        {
            return this.GetBoolID(id);
        }

        public bool GetBool(string name)
        {
            return this.GetBoolString(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool GetBoolID(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool GetBoolString(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex);
        public float GetFloat(int id)
        {
            return this.GetFloatID(id);
        }

        public float GetFloat(string name)
        {
            return this.GetFloatString(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern float GetFloatID(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern float GetFloatString(string name);
        public int GetInteger(int id)
        {
            return this.GetIntegerID(id);
        }

        public int GetInteger(string name)
        {
            return this.GetIntegerString(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern int GetIntegerID(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern int GetIntegerString(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetLayerIndex(string layerName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetLayerName(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetLayerWeight(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex);
        public AnimatorControllerParameter GetParameter(int index)
        {
            AnimatorControllerParameter[] parameters = this.parameters;
            if ((index < 0) && (index >= this.parameters.Length))
            {
                throw new IndexOutOfRangeException("index");
            }
            return parameters[index];
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool HasState(int layerIndex, int stateID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InstantiateEnginePlayable(RuntimeAnimatorController controller);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsInTransition(int layerIndex);
        public bool IsParameterControlledByCurve(int id)
        {
            return this.IsParameterControlledByCurveID(id);
        }

        public bool IsParameterControlledByCurve(string name)
        {
            return this.IsParameterControlledByCurveString(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool IsParameterControlledByCurveID(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool IsParameterControlledByCurveString(string name);
        [ExcludeFromDocs]
        public void Play(int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.Play(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(string stateName)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.Play(stateName, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.Play(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(string stateName, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.Play(stateName, layer, negativeInfinity);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);
        public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            this.Play(StringToHash(stateName), layer, normalizedTime);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInFixedTime(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(string stateName)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInFixedTime(stateName, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInFixedTime(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(string stateName, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInFixedTime(stateName, layer, negativeInfinity);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void PlayInFixedTime(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime);
        public void PlayInFixedTime(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
        {
            this.PlayInFixedTime(StringToHash(stateName), layer, fixedTime);
        }

        public override bool RemoveAllInputs()
        {
            Debug.LogError("AnimationClipPlayable doesn't support removing inputs");
            return false;
        }

        public override bool RemoveInput(int index)
        {
            Debug.LogError("AnimationClipPlayable doesn't support removing inputs");
            return false;
        }

        public override bool RemoveInput(AnimationPlayable playable)
        {
            Debug.LogError("AnimationClipPlayable doesn't support removing inputs");
            return false;
        }

        public void ResetTrigger(int id)
        {
            this.ResetTriggerID(id);
        }

        public void ResetTrigger(string name)
        {
            this.ResetTriggerString(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void ResetTriggerID(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void ResetTriggerString(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string ResolveHash(int hash);
        public void SetBool(int id, bool value)
        {
            this.SetBoolID(id, value);
        }

        public void SetBool(string name, bool value)
        {
            this.SetBoolString(name, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetBoolID(int id, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetBoolString(string name, bool value);
        public void SetFloat(int id, float value)
        {
            this.SetFloatID(id, value);
        }

        public void SetFloat(string name, float value)
        {
            this.SetFloatString(name, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetFloatID(int id, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetFloatString(string name, float value);
        public override bool SetInput(AnimationPlayable source, int index)
        {
            Debug.LogError("AnimationClipPlayable doesn't support setting inputs");
            return false;
        }

        public override bool SetInputs(IEnumerable<AnimationPlayable> sources)
        {
            Debug.LogError("AnimationClipPlayable doesn't support setting inputs");
            return false;
        }

        public void SetInteger(int id, int value)
        {
            this.SetIntegerID(id, value);
        }

        public void SetInteger(string name, int value)
        {
            this.SetIntegerString(name, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetIntegerID(int id, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetIntegerString(string name, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetLayerWeight(int layerIndex, float weight);
        public void SetTrigger(int id)
        {
            this.SetTriggerID(id);
        }

        public void SetTrigger(string name)
        {
            this.SetTriggerString(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetTriggerID(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetTriggerString(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int StringToHash(string name);

        public RuntimeAnimatorController animatorController { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int layerCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int parameterCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        private AnimatorControllerParameter[] parameters { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

