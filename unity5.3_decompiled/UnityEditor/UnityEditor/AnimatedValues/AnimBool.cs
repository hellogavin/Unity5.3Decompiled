namespace UnityEditor.AnimatedValues
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class AnimBool : BaseAnimValue<bool>
    {
        [SerializeField]
        private float m_Value;

        public AnimBool() : base(false)
        {
        }

        public AnimBool(bool value) : base(value)
        {
        }

        public AnimBool(UnityAction callback) : base(false, callback)
        {
        }

        public AnimBool(bool value, UnityAction callback) : base(value, callback)
        {
        }

        public float Fade(float from, float to)
        {
            return Mathf.Lerp(from, to, this.faded);
        }

        protected override bool GetValue()
        {
            float a = !base.target ? 1f : 0f;
            float b = 1f - a;
            this.m_Value = Mathf.Lerp(a, b, base.lerpPosition);
            return (this.m_Value > 0.5f);
        }

        public float faded
        {
            get
            {
                this.GetValue();
                return this.m_Value;
            }
        }
    }
}

