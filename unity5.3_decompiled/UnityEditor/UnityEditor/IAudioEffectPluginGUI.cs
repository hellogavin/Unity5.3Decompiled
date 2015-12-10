namespace UnityEditor
{
    using System;

    public abstract class IAudioEffectPluginGUI
    {
        protected IAudioEffectPluginGUI()
        {
        }

        public abstract bool OnGUI(IAudioEffectPlugin plugin);

        public abstract string Description { get; }

        public abstract string Name { get; }

        public abstract string Vendor { get; }
    }
}

