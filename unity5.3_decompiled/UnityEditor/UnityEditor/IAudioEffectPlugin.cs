namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class IAudioEffectPlugin
    {
        protected IAudioEffectPlugin()
        {
        }

        public abstract bool GetFloatBuffer(string name, out float[] data, int numsamples);
        public abstract bool GetFloatParameter(string name, out float value);
        public abstract bool GetFloatParameterInfo(string name, out float minRange, out float maxRange, out float defaultValue);
        public abstract int GetSampleRate();
        public abstract bool IsPluginEditableAndEnabled();
        public abstract bool SetFloatParameter(string name, float value);
    }
}

