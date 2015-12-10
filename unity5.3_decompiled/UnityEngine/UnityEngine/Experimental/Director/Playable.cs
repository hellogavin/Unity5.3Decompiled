namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public class Playable : IDisposable
    {
        internal IntPtr m_Ptr;
        internal int m_UniqueId;

        public Playable()
        {
            this.m_Ptr = IntPtr.Zero;
            this.m_UniqueId = this.GenerateUniqueId();
            this.InstantiateEnginePlayable();
        }

        internal Playable(bool callCPPConstructor)
        {
            this.m_Ptr = IntPtr.Zero;
            this.m_UniqueId = this.GenerateUniqueId();
            if (callCPPConstructor)
            {
                this.InstantiateEnginePlayable();
            }
        }

        internal bool CheckInputBounds(int inputIndex)
        {
            return this.CheckInputBounds(inputIndex, false);
        }

        internal bool CheckInputBounds(int inputIndex, bool acceptAny)
        {
            if ((inputIndex != -1) || !acceptAny)
            {
                if (inputIndex < 0)
                {
                    throw new IndexOutOfRangeException("Index must be greater than 0");
                }
                Playable[] inputs = this.GetInputs();
                if (inputs.Length <= inputIndex)
                {
                    object[] objArray1 = new object[] { "inputIndex ", inputIndex, " is greater than the number of available inputs (", inputs.Length, ")." };
                    throw new IndexOutOfRangeException(string.Concat(objArray1));
                }
            }
            return true;
        }

        internal static bool CheckPlayableValidity(Playable playable, string name)
        {
            if (playable == null)
            {
                throw new NullReferenceException("Playable " + name + "is null");
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearInputs();
        internal static bool CompareIntPtr(Playable lhs, Playable rhs)
        {
            bool flag = (lhs == null) || !IsNativePlayableAlive(lhs);
            bool flag2 = (rhs == null) || !IsNativePlayableAlive(rhs);
            if (flag2 && flag)
            {
                return true;
            }
            if (flag2)
            {
                return !IsNativePlayableAlive(lhs);
            }
            if (flag)
            {
                return !IsNativePlayableAlive(rhs);
            }
            return (lhs.GetUniqueIDInternal() == rhs.GetUniqueIDInternal());
        }

        public static bool Connect(Playable source, Playable target)
        {
            return Connect(source, target, -1, -1);
        }

        public static bool Connect(Playable source, Playable target, int sourceOutputPort, int targetInputPort)
        {
            if (!CheckPlayableValidity(source, "source") && !CheckPlayableValidity(target, "target"))
            {
                return false;
            }
            if ((source != null) && !source.CheckInputBounds(sourceOutputPort, true))
            {
                return false;
            }
            if (!target.CheckInputBounds(targetInputPort, true))
            {
                return false;
            }
            return ConnectInternal(source, target, sourceOutputPort, targetInputPort);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool ConnectInternal(Playable source, Playable target, int sourceOutputPort, int targetInputPort);
        public static void Disconnect(Playable target, int inputPort)
        {
            if (CheckPlayableValidity(target, "target") && target.CheckInputBounds(inputPort))
            {
                DisconnectInternal(target, inputPort);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void DisconnectInternal(Playable target, int inputPort);
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            this.ReleaseEnginePlayable();
            this.m_Ptr = IntPtr.Zero;
        }

        public override bool Equals(object p)
        {
            return CompareIntPtr(this, p as Playable);
        }

        ~Playable()
        {
            this.Dispose(false);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern int GenerateUniqueId();
        public override int GetHashCode()
        {
            return this.m_UniqueId;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Playable GetInput(int inputPort);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Playable[] GetInputs();
        public void GetInputs(List<Playable> inputList)
        {
            inputList.Clear();
            this.GetInputsInternal(inputList);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetInputsInternal(object list);
        public float GetInputWeight(int inputIndex)
        {
            if (this.CheckInputBounds(inputIndex))
            {
                return this.GetInputWeightInternal(inputIndex);
            }
            return -1f;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern float GetInputWeightInternal(int inputIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Playable GetOutput(int outputPort);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Playable[] GetOutputs();
        public void GetOutputs(List<Playable> outputList)
        {
            outputList.Clear();
            this.GetOutputsInternal(outputList);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetOutputsInternal(object list);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern int GetUniqueIDInternal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InstantiateEnginePlayable();
        internal static bool IsNativePlayableAlive(Playable p)
        {
            return (p.m_Ptr != IntPtr.Zero);
        }

        public virtual void OnSetPlayState(PlayState newState)
        {
        }

        public virtual void OnSetTime(float localTime)
        {
        }

        public static bool operator ==(Playable x, Playable y)
        {
            return CompareIntPtr(x, y);
        }

        public static implicit operator bool(Playable exists)
        {
            return !CompareIntPtr(exists, null);
        }

        public static bool operator !=(Playable x, Playable y)
        {
            return !CompareIntPtr(x, y);
        }

        public virtual void PrepareFrame(FrameData info)
        {
        }

        public virtual void ProcessFrame(FrameData info, object playerData)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void ReleaseEnginePlayable();
        public bool SetInputWeight(int inputIndex, float weight)
        {
            return (this.CheckInputBounds(inputIndex) && this.SetInputWeightInternal(inputIndex, weight));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool SetInputWeightInternal(int inputIndex, float weight);

        public int inputCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int outputCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public PlayState state { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public double time { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

