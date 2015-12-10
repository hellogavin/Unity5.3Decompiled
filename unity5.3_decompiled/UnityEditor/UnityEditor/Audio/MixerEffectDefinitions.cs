namespace UnityEditor.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class MixerEffectDefinitions
    {
        private static readonly List<MixerEffectDefinition> s_MixerEffectDefinitions = new List<MixerEffectDefinition>();

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void AddDefinitionRuntime(string name, MixerParameterDefinition[] parameters);
        public static void ClearDefinitions()
        {
            s_MixerEffectDefinitions.Clear();
            ClearDefinitionsRuntime();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void ClearDefinitionsRuntime();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool EffectCanBeSidechainTarget(AudioMixerEffectController effect);
        public static bool EffectExists(string name)
        {
            foreach (MixerEffectDefinition definition in s_MixerEffectDefinitions)
            {
                if (definition.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetAudioEffectNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern MixerParameterDefinition[] GetAudioEffectParameterDesc(string effectName);
        public static string[] GetEffectList()
        {
            string[] strArray = new string[s_MixerEffectDefinitions.Count];
            for (int i = 0; i < s_MixerEffectDefinitions.Count; i++)
            {
                strArray[i] = s_MixerEffectDefinitions[i].name;
            }
            return strArray;
        }

        public static MixerParameterDefinition[] GetEffectParameters(string effect)
        {
            foreach (MixerEffectDefinition definition in s_MixerEffectDefinitions)
            {
                if (definition.name == effect)
                {
                    return definition.parameters;
                }
            }
            return new MixerParameterDefinition[0];
        }

        public static void Refresh()
        {
            ClearDefinitions();
            RegisterAudioMixerEffect("Attenuation", new MixerParameterDefinition[0]);
            RegisterAudioMixerEffect("Send", new MixerParameterDefinition[0]);
            RegisterAudioMixerEffect("Receive", new MixerParameterDefinition[0]);
            MixerParameterDefinition[] definitions = new MixerParameterDefinition[7];
            MixerParameterDefinition definition = new MixerParameterDefinition {
                name = "Threshold",
                units = "dB",
                displayScale = 1f,
                displayExponent = 1f,
                minRange = -80f,
                maxRange = 0f,
                defaultValue = -10f,
                description = "Threshold of side-chain level detector"
            };
            definitions[0] = definition;
            MixerParameterDefinition definition2 = new MixerParameterDefinition {
                name = "Ratio",
                units = "%",
                displayScale = 100f,
                displayExponent = 1f,
                minRange = 0.2f,
                maxRange = 10f,
                defaultValue = 2f,
                description = "Ratio of compression applied when side-chain signal exceeds threshold"
            };
            definitions[1] = definition2;
            MixerParameterDefinition definition3 = new MixerParameterDefinition {
                name = "Attack Time",
                units = "ms",
                displayScale = 1000f,
                displayExponent = 3f,
                minRange = 0f,
                maxRange = 10f,
                defaultValue = 0.1f,
                description = "Level detector attack time"
            };
            definitions[2] = definition3;
            MixerParameterDefinition definition4 = new MixerParameterDefinition {
                name = "Release Time",
                units = "ms",
                displayScale = 1000f,
                displayExponent = 3f,
                minRange = 0f,
                maxRange = 10f,
                defaultValue = 0.1f,
                description = "Level detector release time"
            };
            definitions[3] = definition4;
            MixerParameterDefinition definition5 = new MixerParameterDefinition {
                name = "Make-up Gain",
                units = "dB",
                displayScale = 1f,
                displayExponent = 1f,
                minRange = -80f,
                maxRange = 40f,
                defaultValue = 0f,
                description = "Make-up gain"
            };
            definitions[4] = definition5;
            MixerParameterDefinition definition6 = new MixerParameterDefinition {
                name = "Knee",
                units = "dB",
                displayScale = 1f,
                displayExponent = 1f,
                minRange = 0f,
                maxRange = 50f,
                defaultValue = 10f,
                description = "Sharpness of compression curve knee"
            };
            definitions[5] = definition6;
            MixerParameterDefinition definition7 = new MixerParameterDefinition {
                name = "Sidechain Mix",
                units = "%",
                displayScale = 100f,
                displayExponent = 1f,
                minRange = 0f,
                maxRange = 1f,
                defaultValue = 1f,
                description = "Sidechain/source mix. If set to 100% the compressor detects level entirely from sidechain signal."
            };
            definitions[6] = definition7;
            RegisterAudioMixerEffect("Duck Volume", definitions);
            AddDefinitionRuntime("Duck Volume", definitions);
            foreach (string str in GetAudioEffectNames())
            {
                MixerParameterDefinition[] audioEffectParameterDesc = GetAudioEffectParameterDesc(str);
                RegisterAudioMixerEffect(str, audioEffectParameterDesc);
            }
        }

        public static bool RegisterAudioMixerEffect(string name, MixerParameterDefinition[] definitions)
        {
            foreach (MixerEffectDefinition definition in s_MixerEffectDefinitions)
            {
                if (definition.name == name)
                {
                    return false;
                }
            }
            MixerEffectDefinition item = new MixerEffectDefinition(name, definitions);
            s_MixerEffectDefinitions.Add(item);
            ClearDefinitionsRuntime();
            foreach (MixerEffectDefinition definition3 in s_MixerEffectDefinitions)
            {
                AddDefinitionRuntime(definition3.name, definition3.parameters);
            }
            return true;
        }
    }
}

