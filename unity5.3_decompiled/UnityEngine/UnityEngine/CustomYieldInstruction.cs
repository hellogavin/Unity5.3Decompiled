namespace UnityEngine
{
    using System;
    using System.Collections;

    public abstract class CustomYieldInstruction : IEnumerator
    {
        protected CustomYieldInstruction()
        {
        }

        public bool MoveNext()
        {
            return this.keepWaiting;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get
            {
                return null;
            }
        }

        public abstract bool keepWaiting { get; }
    }
}

