namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    [RequiredByNativeCode, AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyIsEditorAssembly : Attribute
    {
    }
}

