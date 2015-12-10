namespace UnityEditor.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Audio;

    internal sealed class AudioMixerGroupController : AudioMixerGroup
    {
        public AudioMixerGroupController(AudioMixer owner)
        {
            Internal_CreateAudioMixerGroupController(this, owner);
        }

        public void DumpHierarchy(string title, int level)
        {
            if (title != string.Empty)
            {
                Console.WriteLine(title);
            }
            string str = string.Empty;
            int num = level;
            while (num-- > 0)
            {
                str = str + "  ";
            }
            Console.WriteLine(str + "name=" + base.name);
            str = str + "  ";
            foreach (AudioMixerEffectController controller in this.effects)
            {
                Console.WriteLine(str + "effect=" + controller.ToString());
            }
            foreach (AudioMixerGroupController controller2 in this.children)
            {
                controller2.DumpHierarchy(string.Empty, level + 1);
            }
        }

        public string GetDisplayString()
        {
            return base.name;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern GUID GetGUIDForPitch();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern GUID GetGUIDForVolume();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetValueForPitch(AudioMixerController controller, AudioMixerSnapshotController snapshot);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetValueForVolume(AudioMixerController controller, AudioMixerSnapshotController snapshot);
        public bool HasAttenuation()
        {
            foreach (AudioMixerEffectController controller in this.effects)
            {
                if (controller.IsAttenuation())
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool HasDependentMixers();
        public void InsertEffect(AudioMixerEffectController effect, int index)
        {
            List<AudioMixerEffectController> list = new List<AudioMixerEffectController>(this.effects) { null };
            for (int i = list.Count - 1; i > index; i--)
            {
                list[i] = list[i - 1];
            }
            list[index] = effect;
            this.effects = list.ToArray();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateAudioMixerGroupController(AudioMixerGroupController mono, AudioMixer owner);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void PreallocateGUIDs();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetValueForPitch(AudioMixerController controller, AudioMixerSnapshotController snapshot, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetValueForVolume(AudioMixerController controller, AudioMixerSnapshotController snapshot, float value);
        public override string ToString()
        {
            return base.name;
        }

        public bool bypassEffects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AudioMixerGroupController[] children { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AudioMixerController controller { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public AudioMixerEffectController[] effects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public GUID groupID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool mute { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool solo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int userColorIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

