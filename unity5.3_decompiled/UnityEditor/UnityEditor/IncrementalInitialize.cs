namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class IncrementalInitialize
    {
        [NonSerialized]
        private bool m_IncrementOnNextEvent;
        [SerializeField]
        private State m_InitState;

        public void OnEvent()
        {
            if (this.m_IncrementOnNextEvent)
            {
                this.m_InitState += 1;
                this.m_IncrementOnNextEvent = false;
            }
            switch (this.m_InitState)
            {
                case State.PreInitialize:
                    if (Event.current.type == EventType.Repaint)
                    {
                        this.m_IncrementOnNextEvent = true;
                        HandleUtility.Repaint();
                    }
                    break;

                case State.Initialize:
                    this.m_IncrementOnNextEvent = true;
                    break;
            }
        }

        public void Restart()
        {
            this.m_InitState = State.PreInitialize;
        }

        public State state
        {
            get
            {
                return this.m_InitState;
            }
        }

        public enum State
        {
            PreInitialize,
            Initialize,
            Initialized
        }
    }
}

