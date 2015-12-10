namespace UnityEditor.Animations
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public sealed class AnimatorControllerLayer
    {
        private AvatarMask m_AvatarMask;
        private StateBehavioursPair[] m_Behaviours;
        private AnimatorLayerBlendingMode m_BlendingMode;
        private float m_DefaultWeight;
        private bool m_IKPass;
        private StateMotionPair[] m_Motions;
        private string m_Name;
        private AnimatorStateMachine m_StateMachine;
        private bool m_SyncedLayerAffectsTiming;
        private int m_SyncedLayerIndex = -1;

        public StateMachineBehaviour[] GetOverrideBehaviours(AnimatorState state)
        {
            if (this.m_Behaviours != null)
            {
                foreach (StateBehavioursPair pair in this.m_Behaviours)
                {
                    if (pair.m_State == state)
                    {
                        return pair.m_Behaviours;
                    }
                }
            }
            return new StateMachineBehaviour[0];
        }

        public Motion GetOverrideMotion(AnimatorState state)
        {
            if (this.m_Motions != null)
            {
                foreach (StateMotionPair pair in this.m_Motions)
                {
                    if (pair.m_State == state)
                    {
                        return pair.m_Motion;
                    }
                }
            }
            return null;
        }

        public void SetOverrideBehaviours(AnimatorState state, StateMachineBehaviour[] behaviours)
        {
            StateBehavioursPair pair;
            if (this.m_Behaviours == null)
            {
                this.m_Behaviours = new StateBehavioursPair[0];
            }
            for (int i = 0; i < this.m_Behaviours.Length; i++)
            {
                if (this.m_Behaviours[i].m_State == state)
                {
                    this.m_Behaviours[i].m_Behaviours = behaviours;
                    return;
                }
            }
            pair.m_State = state;
            pair.m_Behaviours = behaviours;
            ArrayUtility.Add<StateBehavioursPair>(ref this.m_Behaviours, pair);
        }

        public void SetOverrideMotion(AnimatorState state, Motion motion)
        {
            StateMotionPair pair;
            if (this.m_Motions == null)
            {
                this.m_Motions = new StateMotionPair[0];
            }
            for (int i = 0; i < this.m_Motions.Length; i++)
            {
                if (this.m_Motions[i].m_State == state)
                {
                    this.m_Motions[i].m_Motion = motion;
                    return;
                }
            }
            pair.m_State = state;
            pair.m_Motion = motion;
            ArrayUtility.Add<StateMotionPair>(ref this.m_Motions, pair);
        }

        public AvatarMask avatarMask
        {
            get
            {
                return this.m_AvatarMask;
            }
            set
            {
                this.m_AvatarMask = value;
            }
        }

        public AnimatorLayerBlendingMode blendingMode
        {
            get
            {
                return this.m_BlendingMode;
            }
            set
            {
                this.m_BlendingMode = value;
            }
        }

        public float defaultWeight
        {
            get
            {
                return this.m_DefaultWeight;
            }
            set
            {
                this.m_DefaultWeight = value;
            }
        }

        public bool iKPass
        {
            get
            {
                return this.m_IKPass;
            }
            set
            {
                this.m_IKPass = value;
            }
        }

        public string name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        public AnimatorStateMachine stateMachine
        {
            get
            {
                return this.m_StateMachine;
            }
            set
            {
                this.m_StateMachine = value;
            }
        }

        public bool syncedLayerAffectsTiming
        {
            get
            {
                return this.m_SyncedLayerAffectsTiming;
            }
            set
            {
                this.m_SyncedLayerAffectsTiming = value;
            }
        }

        public int syncedLayerIndex
        {
            get
            {
                return this.m_SyncedLayerIndex;
            }
            set
            {
                this.m_SyncedLayerIndex = value;
            }
        }
    }
}

