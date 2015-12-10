namespace UnityEngine
{
    using System;

    public sealed class WaitWhile : CustomYieldInstruction
    {
        private Func<bool> m_Predicate;

        public WaitWhile(Func<bool> predicate)
        {
            this.m_Predicate = predicate;
        }

        public override bool keepWaiting
        {
            get
            {
                return this.m_Predicate();
            }
        }
    }
}

