namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AnimationPlayable : Playable
    {
        public AnimationPlayable() : base(false)
        {
            base.m_Ptr = IntPtr.Zero;
            this.InstantiateEnginePlayable();
        }

        public AnimationPlayable(bool final) : base(false)
        {
            base.m_Ptr = IntPtr.Zero;
            if (final)
            {
                this.InstantiateEnginePlayable();
            }
        }

        public virtual int AddInput(AnimationPlayable source)
        {
            Playable.Connect(source, this, -1, -1);
            return (base.GetInputs().Length - 1);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InstantiateEnginePlayable();
        public virtual bool RemoveAllInputs()
        {
            Playable[] inputs = base.GetInputs();
            for (int i = 0; i < inputs.Length; i++)
            {
                this.RemoveInput(inputs[i] as AnimationPlayable);
            }
            return true;
        }

        public virtual bool RemoveInput(int index)
        {
            if (!base.CheckInputBounds(index))
            {
                return false;
            }
            Playable.Disconnect(this, index);
            return true;
        }

        public virtual bool RemoveInput(AnimationPlayable playable)
        {
            if (Playable.CheckPlayableValidity(playable, "playable"))
            {
                Playable[] inputs = base.GetInputs();
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i] == playable)
                    {
                        Playable.Disconnect(this, i);
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool SetInput(AnimationPlayable source, int index)
        {
            if (!base.CheckInputBounds(index))
            {
                return false;
            }
            if (base.GetInputs()[index] != null)
            {
                Playable.Disconnect(this, index);
            }
            return Playable.Connect(source, this, -1, index);
        }

        public virtual bool SetInputs(IEnumerable<AnimationPlayable> sources)
        {
            int length = base.GetInputs().Length;
            for (int i = 0; i < length; i++)
            {
                Playable.Disconnect(this, i);
            }
            bool flag = false;
            int targetInputPort = 0;
            IEnumerator<AnimationPlayable> enumerator = sources.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AnimationPlayable current = enumerator.Current;
                    if (targetInputPort < length)
                    {
                        flag |= Playable.Connect(current, this, -1, targetInputPort);
                    }
                    else
                    {
                        flag |= Playable.Connect(current, this, -1, -1);
                    }
                    base.SetInputWeight(targetInputPort, 1f);
                    targetInputPort++;
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            for (int j = targetInputPort; j < length; j++)
            {
                base.SetInputWeight(j, 0f);
            }
            return flag;
        }
    }
}

