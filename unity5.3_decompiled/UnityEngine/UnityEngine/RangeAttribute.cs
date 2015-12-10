namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
    public sealed class RangeAttribute : PropertyAttribute
    {
        public readonly float max;
        public readonly float min;

        public RangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}

