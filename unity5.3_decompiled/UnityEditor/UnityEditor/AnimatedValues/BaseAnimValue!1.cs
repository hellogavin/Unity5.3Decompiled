namespace UnityEditor.AnimatedValues
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    public abstract class BaseAnimValue<T>
    {
        private bool m_Animating;
        private double m_LastTime;
        private double m_LerpPosition;
        private T m_Start;
        [SerializeField]
        private T m_Target;
        public float speed;
        [NonSerialized]
        public UnityEvent valueChanged;

        protected BaseAnimValue(T value)
        {
            this.m_LerpPosition = 1.0;
            this.speed = 2f;
            this.m_Start = value;
            this.m_Target = value;
            this.valueChanged = new UnityEvent();
        }

        protected BaseAnimValue(T value, UnityAction callback)
        {
            this.m_LerpPosition = 1.0;
            this.speed = 2f;
            this.m_Start = value;
            this.m_Target = value;
            this.valueChanged = new UnityEvent();
            this.valueChanged.AddListener(callback);
        }

        protected void BeginAnimating(T newTarget, T newStart)
        {
            this.m_Start = newStart;
            this.m_Target = newTarget;
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
            this.m_Animating = true;
            this.m_LastTime = EditorApplication.timeSinceStartup;
            this.m_LerpPosition = 0.0;
        }

        private static T2 Clamp<T2>(T2 val, T2 min, T2 max) where T2: IComparable<T2>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }
            if (val.CompareTo(max) > 0)
            {
                return max;
            }
            return val;
        }

        protected abstract T GetValue();
        protected void StopAnim(T newValue)
        {
            bool flag = false;
            if ((!newValue.Equals(this.GetValue()) || (this.m_LerpPosition < 1.0)) && (this.valueChanged != null))
            {
                flag = true;
            }
            this.m_Target = newValue;
            this.m_Start = newValue;
            this.m_LerpPosition = 1.0;
            this.m_Animating = false;
            if (flag)
            {
                this.valueChanged.Invoke();
            }
        }

        private void Update()
        {
            if (this.m_Animating)
            {
                this.UpdateLerpPosition();
                if (this.valueChanged != null)
                {
                    this.valueChanged.Invoke();
                }
                if (this.lerpPosition >= 1f)
                {
                    this.m_Animating = false;
                    EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
                }
            }
        }

        private void UpdateLerpPosition()
        {
            double timeSinceStartup = EditorApplication.timeSinceStartup;
            double num2 = timeSinceStartup - this.m_LastTime;
            this.m_LerpPosition = BaseAnimValue<T>.Clamp<double>(this.m_LerpPosition + (num2 * this.speed), 0.0, 1.0);
            this.m_LastTime = timeSinceStartup;
        }

        public bool isAnimating
        {
            get
            {
                return this.m_Animating;
            }
        }

        protected float lerpPosition
        {
            get
            {
                double num = 1.0 - this.m_LerpPosition;
                double num2 = 1.0 - (((num * num) * num) * num);
                return (float) num2;
            }
        }

        protected T start
        {
            get
            {
                return this.m_Start;
            }
        }

        public T target
        {
            get
            {
                return this.m_Target;
            }
            set
            {
                if (!this.m_Target.Equals(value))
                {
                    this.BeginAnimating(value, this.value);
                }
            }
        }

        public T value
        {
            get
            {
                return this.GetValue();
            }
            set
            {
                this.StopAnim(value);
            }
        }
    }
}

