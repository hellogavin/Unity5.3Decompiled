namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal abstract class PresetLibrary : ScriptableObject
    {
        protected PresetLibrary()
        {
        }

        public abstract void Add(object presetObject, string presetName);
        public abstract int Count();
        public abstract void Draw(Rect rect, int index);
        public abstract void Draw(Rect rect, object presetObject);
        public abstract string GetName(int index);
        public abstract object GetPreset(int index);
        public abstract void Move(int index, int destIndex, bool insertAfterDestIndex);
        public abstract void Remove(int index);
        public abstract void Replace(int index, object newPresetObject);
        public abstract void SetName(int index, string name);
    }
}

