namespace UnityEngine.Scripting
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=false)]
    public class PreserveAttribute : Attribute
    {
    }
}

