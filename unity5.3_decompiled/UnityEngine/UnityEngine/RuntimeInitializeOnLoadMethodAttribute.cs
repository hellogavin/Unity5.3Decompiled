namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class RuntimeInitializeOnLoadMethodAttribute : PreserveAttribute
    {
        public RuntimeInitializeOnLoadMethodAttribute()
        {
            this.loadType = RuntimeInitializeLoadType.AfterSceneLoad;
        }

        public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType)
        {
            this.loadType = loadType;
        }

        public RuntimeInitializeLoadType loadType { get; private set; }
    }
}

