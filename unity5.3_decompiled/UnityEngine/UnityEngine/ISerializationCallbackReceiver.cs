namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public interface ISerializationCallbackReceiver
    {
        void OnAfterDeserialize();
        void OnBeforeSerialize();
    }
}

