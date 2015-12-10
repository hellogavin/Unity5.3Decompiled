namespace UnityEditor.Audio
{
    using System;
    using UnityEditor;

    internal abstract class AudioParameterPath
    {
        public GUID parameter;

        protected AudioParameterPath()
        {
        }

        public abstract string ResolveStringPath(bool getOnlyBasePath);
    }
}

