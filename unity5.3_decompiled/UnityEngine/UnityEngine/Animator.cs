namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Experimental.Director;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    public sealed class Animator : DirectorPlayer, IAnimatorControllerPlayable
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ApplyBuiltinRootMotion();
        private void CheckIfInIKPass()
        {
            if (this.logWarnings && !this.CheckIfInIKPassInternal())
            {
                Debug.LogWarning("Setting and getting IK Goals, Lookat and BoneLocalRotation should only be done in OnAnimatorIK or OnStateIK");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool CheckIfInIKPassInternal();
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
        internal extern void EvaluateController();
        [Obsolete("ForceStateNormalizedTime is deprecated. Please use Play or CrossFade instead.")]
        public void ForceStateNormalizedTime(float normalizedTime)
        {
            this.Play(0, 0, normalizedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex);
        public T GetBehaviour<T>() where T: StateMachineBehaviour
        {
            return (this.GetBehaviour(typeof(T)) as T);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern ScriptableObject GetBehaviour(Type type);
        public T[] GetBehaviours<T>() where T: StateMachineBehaviour
        {
            return ConvertStateMachineBehaviour<T>(this.GetBehaviours(typeof(T)));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern ScriptableObject[] GetBehaviours(Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Transform GetBoneTransform(HumanBodyBones humanBoneId);
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
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("GetCurrentAnimationClipState is obsolete. Use GetCurrentAnimatorClipInfo instead (UnityUpgradable) -> GetCurrentAnimatorClipInfo(*)", true)]
        public AnimationInfo[] GetCurrentAnimationClipState(int layerIndex)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string GetCurrentStateName(int layerIndex);
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
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern float GetHintWeightPositionInternal(AvatarIKHint hint);
        public Vector3 GetIKHintPosition(AvatarIKHint hint)
        {
            this.CheckIfInIKPass();
            return this.GetIKHintPositionInternal(hint);
        }

        internal Vector3 GetIKHintPositionInternal(AvatarIKHint hint)
        {
            Vector3 vector;
            INTERNAL_CALL_GetIKHintPositionInternal(this, hint, out vector);
            return vector;
        }

        public float GetIKHintPositionWeight(AvatarIKHint hint)
        {
            this.CheckIfInIKPass();
            return this.GetHintWeightPositionInternal(hint);
        }

        public Vector3 GetIKPosition(AvatarIKGoal goal)
        {
            this.CheckIfInIKPass();
            return this.GetIKPositionInternal(goal);
        }

        internal Vector3 GetIKPositionInternal(AvatarIKGoal goal)
        {
            Vector3 vector;
            INTERNAL_CALL_GetIKPositionInternal(this, goal, out vector);
            return vector;
        }

        public float GetIKPositionWeight(AvatarIKGoal goal)
        {
            this.CheckIfInIKPass();
            return this.GetIKPositionWeightInternal(goal);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern float GetIKPositionWeightInternal(AvatarIKGoal goal);
        public Quaternion GetIKRotation(AvatarIKGoal goal)
        {
            this.CheckIfInIKPass();
            return this.GetIKRotationInternal(goal);
        }

        internal Quaternion GetIKRotationInternal(AvatarIKGoal goal)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetIKRotationInternal(this, goal, out quaternion);
            return quaternion;
        }

        public float GetIKRotationWeight(AvatarIKGoal goal)
        {
            this.CheckIfInIKPass();
            return this.GetIKRotationWeightInternal(goal);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern float GetIKRotationWeightInternal(AvatarIKGoal goal);
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
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("GetNextAnimationClipState is obsolete. Use GetNextAnimatorClipInfo instead (UnityUpgradable) -> GetNextAnimatorClipInfo(*)", true)]
        public AnimationInfo[] GetNextAnimationClipState(int layerIndex)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string GetNextStateName(int layerIndex);
        public AnimatorControllerParameter GetParameter(int index)
        {
            AnimatorControllerParameter[] parameters = this.parameters;
            if ((index < 0) && (index >= this.parameters.Length))
            {
                throw new IndexOutOfRangeException("index");
            }
            return parameters[index];
        }

        [Obsolete("GetQuaternion is deprecated.")]
        public Quaternion GetQuaternion(int id)
        {
            return Quaternion.identity;
        }

        [Obsolete("GetQuaternion is deprecated.")]
        public Quaternion GetQuaternion(string name)
        {
            return Quaternion.identity;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string GetStats();
        [Obsolete("GetVector is deprecated.")]
        public Vector3 GetVector(int id)
        {
            return Vector3.zero;
        }

        [Obsolete("GetVector is deprecated.")]
        public Vector3 GetVector(string name)
        {
            return Vector3.zero;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool HasState(int layerIndex, int stateID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetIKHintPositionInternal(Animator self, AvatarIKHint hint, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetIKPositionInternal(Animator self, AvatarIKGoal goal, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetIKRotationInternal(Animator self, AvatarIKGoal goal, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MatchTarget(Animator self, ref Vector3 matchPosition, ref Quaternion matchRotation, AvatarTarget targetBodyPart, ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetBoneLocalRotationInternal(Animator self, HumanBodyBones humanBoneId, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetIKHintPositionInternal(Animator self, AvatarIKHint hint, ref Vector3 hintPosition);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetIKPositionInternal(Animator self, AvatarIKGoal goal, ref Vector3 goalPosition);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetIKRotationInternal(Animator self, AvatarIKGoal goal, ref Quaternion goalRotation);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetLookAtPositionInternal(Animator self, ref Vector3 lookAtPosition);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_angularVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_bodyPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_bodyRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_deltaPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_deltaRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_pivotPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_rootPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_rootRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_targetPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_targetRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_velocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_bodyPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_bodyRotation(ref Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_rootPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_rootRotation(ref Quaternion value);
        [ExcludeFromDocs]
        public void InterruptMatchTarget()
        {
            bool completeMatch = true;
            this.InterruptMatchTarget(completeMatch);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InterruptMatchTarget([DefaultValue("true")] bool completeMatch);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool IsBoneTransform(Transform transform);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use mask and layers to control subset of transfroms in a skeleton", true), WrapperlessIcall]
        public extern bool IsControlled(Transform transform);
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
        public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime)
        {
            float targetNormalizedTime = 1f;
            INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
        }

        public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [DefaultValue("1")] float targetNormalizedTime)
        {
            INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
        }

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

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Rebind();
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
        public void SetBoneLocalRotation(HumanBodyBones humanBoneId, Quaternion rotation)
        {
            this.CheckIfInIKPass();
            this.SetBoneLocalRotationInternal(humanBoneId, rotation);
        }

        internal void SetBoneLocalRotationInternal(HumanBodyBones humanBoneId, Quaternion rotation)
        {
            INTERNAL_CALL_SetBoneLocalRotationInternal(this, humanBoneId, ref rotation);
        }

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

        public void SetFloat(int id, float value, float dampTime, float deltaTime)
        {
            this.SetFloatIDDamp(id, value, dampTime, deltaTime);
        }

        public void SetFloat(string name, float value, float dampTime, float deltaTime)
        {
            this.SetFloatStringDamp(name, value, dampTime, deltaTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetFloatID(int id, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetFloatString(string name, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime);
        public void SetIKHintPosition(AvatarIKHint hint, Vector3 hintPosition)
        {
            this.CheckIfInIKPass();
            this.SetIKHintPositionInternal(hint, hintPosition);
        }

        internal void SetIKHintPositionInternal(AvatarIKHint hint, Vector3 hintPosition)
        {
            INTERNAL_CALL_SetIKHintPositionInternal(this, hint, ref hintPosition);
        }

        public void SetIKHintPositionWeight(AvatarIKHint hint, float value)
        {
            this.CheckIfInIKPass();
            this.SetIKHintPositionWeightInternal(hint, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetIKHintPositionWeightInternal(AvatarIKHint hint, float value);
        public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
        {
            this.CheckIfInIKPass();
            this.SetIKPositionInternal(goal, goalPosition);
        }

        internal void SetIKPositionInternal(AvatarIKGoal goal, Vector3 goalPosition)
        {
            INTERNAL_CALL_SetIKPositionInternal(this, goal, ref goalPosition);
        }

        public void SetIKPositionWeight(AvatarIKGoal goal, float value)
        {
            this.CheckIfInIKPass();
            this.SetIKPositionWeightInternal(goal, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetIKPositionWeightInternal(AvatarIKGoal goal, float value);
        public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
        {
            this.CheckIfInIKPass();
            this.SetIKRotationInternal(goal, goalRotation);
        }

        internal void SetIKRotationInternal(AvatarIKGoal goal, Quaternion goalRotation)
        {
            INTERNAL_CALL_SetIKRotationInternal(this, goal, ref goalRotation);
        }

        public void SetIKRotationWeight(AvatarIKGoal goal, float value)
        {
            this.CheckIfInIKPass();
            this.SetIKRotationWeightInternal(goal, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetIKRotationWeightInternal(AvatarIKGoal goal, float value);
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
        public void SetLookAtPosition(Vector3 lookAtPosition)
        {
            this.CheckIfInIKPass();
            this.SetLookAtPositionInternal(lookAtPosition);
        }

        internal void SetLookAtPositionInternal(Vector3 lookAtPosition)
        {
            INTERNAL_CALL_SetLookAtPositionInternal(this, ref lookAtPosition);
        }

        [ExcludeFromDocs]
        public void SetLookAtWeight(float weight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            float headWeight = 1f;
            float bodyWeight = 0f;
            this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        public void SetLookAtWeight(float weight, float bodyWeight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            float headWeight = 1f;
            this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight)
        {
            float clampWeight = 0.5f;
            this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        public void SetLookAtWeight(float weight, [DefaultValue("0.00f")] float bodyWeight, [DefaultValue("1.00f")] float headWeight, [DefaultValue("0.00f")] float eyesWeight, [DefaultValue("0.50f")] float clampWeight)
        {
            this.CheckIfInIKPass();
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        internal void SetLookAtWeightInternal(float weight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            float headWeight = 1f;
            float bodyWeight = 0f;
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        internal void SetLookAtWeightInternal(float weight, float bodyWeight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            float headWeight = 1f;
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        internal void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        internal void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight, float eyesWeight)
        {
            float clampWeight = 0.5f;
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetLookAtWeightInternal(float weight, [DefaultValue("0.00f")] float bodyWeight, [DefaultValue("1.00f")] float headWeight, [DefaultValue("0.00f")] float eyesWeight, [DefaultValue("0.50f")] float clampWeight);
        [Obsolete("SetQuaternion is deprecated.")]
        public void SetQuaternion(int id, Quaternion value)
        {
        }

        [Obsolete("SetQuaternion is deprecated.")]
        public void SetQuaternion(string name, Quaternion value)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime);
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
        [Obsolete("SetVector is deprecated.")]
        public void SetVector(int id, Vector3 value)
        {
        }

        [Obsolete("SetVector is deprecated.")]
        public void SetVector(string name, Vector3 value)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void StartPlayback();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void StartRecording(int frameCount);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void StopPlayback();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void StopRecording();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int StringToHash(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Update(float deltaTime);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void WriteDefaultPose();

        internal bool allowConstantClipSamplingOptimization { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 angularVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_angularVelocity(out vector);
                return vector;
            }
        }

        [Obsolete("Use Animator.updateMode instead")]
        public bool animatePhysics
        {
            get
            {
                return (this.updateMode == AnimatorUpdateMode.AnimatePhysics);
            }
            set
            {
                this.updateMode = !value ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.AnimatePhysics;
            }
        }

        public bool applyRootMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Avatar avatar { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal Transform avatarRoot { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3 bodyPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_bodyPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_bodyPosition(ref value);
            }
        }

        public Quaternion bodyRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_bodyRotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_bodyRotation(ref value);
            }
        }

        public AnimatorCullingMode cullingMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 deltaPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_deltaPosition(out vector);
                return vector;
            }
        }

        public Quaternion deltaRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_deltaRotation(out quaternion);
                return quaternion;
            }
        }

        public float feetPivotActive { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool fireEvents { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float gravityWeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool hasRootMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool hasTransformHierarchy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float humanScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isHuman { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isInitialized { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isMatchingTarget { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isOptimizable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal bool isRootPositionOrRotationControlledByCurves { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int layerCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool layersAffectMassCenter { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float leftFeetBottomHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool linearVelocityBlending { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool logWarnings { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int parameterCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public AnimatorControllerParameter[] parameters { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3 pivotPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_pivotPosition(out vector);
                return vector;
            }
        }

        public float pivotWeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float playbackTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AnimatorRecorderMode recorderMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float recorderStartTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float recorderStopTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float rightFeetBottomHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3 rootPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_rootPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_rootPosition(ref value);
            }
        }

        public Quaternion rootRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_rootRotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_rootRotation(ref value);
            }
        }

        public RuntimeAnimatorController runtimeAnimatorController { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float speed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool stabilizeFeet { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal bool supportsOnAnimatorMove { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3 targetPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_targetPosition(out vector);
                return vector;
            }
        }

        public Quaternion targetRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_targetRotation(out quaternion);
                return quaternion;
            }
        }

        public AnimatorUpdateMode updateMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 velocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_velocity(out vector);
                return vector;
            }
        }
    }
}

