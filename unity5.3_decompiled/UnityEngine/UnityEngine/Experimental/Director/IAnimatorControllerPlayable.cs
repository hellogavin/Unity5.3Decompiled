namespace UnityEngine.Experimental.Director
{
    using System;
    using UnityEngine;

    public interface IAnimatorControllerPlayable
    {
        void CrossFade(int stateNameHash, float transitionDuration, int layer, float normalizedTime);
        void CrossFade(string stateName, float transitionDuration, int layer, float normalizedTime);
        void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, int layer, float fixedTime);
        void CrossFadeInFixedTime(string stateName, float transitionDuration, int layer, float fixedTime);
        AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex);
        bool GetBool(int id);
        bool GetBool(string name);
        AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex);
        AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex);
        float GetFloat(int id);
        float GetFloat(string name);
        int GetInteger(int id);
        int GetInteger(string name);
        int GetLayerIndex(string layerName);
        string GetLayerName(int layerIndex);
        float GetLayerWeight(int layerIndex);
        AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex);
        AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex);
        AnimatorControllerParameter GetParameter(int index);
        bool HasState(int layerIndex, int stateID);
        bool IsInTransition(int layerIndex);
        bool IsParameterControlledByCurve(int id);
        bool IsParameterControlledByCurve(string name);
        void Play(int stateNameHash, int layer, float normalizedTime);
        void Play(string stateName, int layer, float normalizedTime);
        void PlayInFixedTime(int stateNameHash, int layer, float fixedTime);
        void PlayInFixedTime(string stateName, int layer, float fixedTime);
        void ResetTrigger(int id);
        void ResetTrigger(string name);
        void SetBool(int id, bool value);
        void SetBool(string name, bool value);
        void SetFloat(int id, float value);
        void SetFloat(string name, float value);
        void SetInteger(int id, int value);
        void SetInteger(string name, int value);
        void SetLayerWeight(int layerIndex, float weight);
        void SetTrigger(int id);
        void SetTrigger(string name);

        int layerCount { get; }

        int parameterCount { get; }
    }
}

