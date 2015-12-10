namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public sealed class StateMachineBehaviourContext
    {
        public AnimatorController animatorController;
        public Object animatorObject;
        public int layerIndex;
    }
}

