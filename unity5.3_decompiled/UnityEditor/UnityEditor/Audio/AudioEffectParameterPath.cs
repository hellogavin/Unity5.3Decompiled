namespace UnityEditor.Audio
{
    using System;
    using UnityEditor;

    internal sealed class AudioEffectParameterPath : AudioGroupParameterPath
    {
        public AudioMixerEffectController effect;

        public AudioEffectParameterPath(AudioMixerGroupController group, AudioMixerEffectController effect, GUID parameter) : base(group, parameter)
        {
            this.effect = effect;
        }

        public override string ResolveStringPath(bool getOnlyBasePath)
        {
            if (getOnlyBasePath)
            {
                return base.GetBasePath(base.group.GetDisplayString(), this.effect.effectName);
            }
            if (this.effect.GetGUIDForMixLevel() == base.parameter)
            {
                return ("Mix Level" + base.GetBasePath(base.group.GetDisplayString(), this.effect.effectName));
            }
            MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(this.effect.effectName);
            for (int i = 0; i < effectParameters.Length; i++)
            {
                if (this.effect.GetGUIDForParameter(effectParameters[i].name) == base.parameter)
                {
                    return (effectParameters[i].name + base.GetBasePath(base.group.GetDisplayString(), this.effect.effectName));
                }
            }
            return "Error finding Parameter path.";
        }
    }
}

