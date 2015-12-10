namespace UnityEditor.Audio
{
    using System;
    using UnityEditor;

    internal class AudioGroupParameterPath : AudioParameterPath
    {
        public AudioMixerGroupController group;

        public AudioGroupParameterPath(AudioMixerGroupController group, GUID parameter)
        {
            this.group = group;
            base.parameter = parameter;
        }

        protected string GetBasePath(string group, string effect)
        {
            string str = " (of " + group;
            if (!string.IsNullOrEmpty(effect))
            {
                str = str + "➔" + effect;
            }
            return (str + ")");
        }

        public override string ResolveStringPath(bool getOnlyBasePath)
        {
            if (getOnlyBasePath)
            {
                return this.GetBasePath(this.group.GetDisplayString(), null);
            }
            if (this.group.GetGUIDForVolume() == base.parameter)
            {
                return ("Volume" + this.GetBasePath(this.group.GetDisplayString(), null));
            }
            if (this.group.GetGUIDForPitch() == base.parameter)
            {
                return ("Pitch" + this.GetBasePath(this.group.GetDisplayString(), null));
            }
            return "Error finding Parameter path.";
        }
    }
}

