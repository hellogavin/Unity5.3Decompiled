namespace UnityEngine
{
    using System;

    public sealed class AnimatorControllerParameter
    {
        internal bool m_DefaultBool;
        internal float m_DefaultFloat;
        internal int m_DefaultInt;
        internal string m_Name = string.Empty;
        internal AnimatorControllerParameterType m_Type;

        public override bool Equals(object o)
        {
            AnimatorControllerParameter parameter = o as AnimatorControllerParameter;
            return (((((parameter != null) && (this.m_Name == parameter.m_Name)) && ((this.m_Type == parameter.m_Type) && (this.m_DefaultFloat == parameter.m_DefaultFloat))) && (this.m_DefaultInt == parameter.m_DefaultInt)) && (this.m_DefaultBool == parameter.m_DefaultBool));
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public bool defaultBool
        {
            get
            {
                return this.m_DefaultBool;
            }
            set
            {
                this.m_DefaultBool = value;
            }
        }

        public float defaultFloat
        {
            get
            {
                return this.m_DefaultFloat;
            }
            set
            {
                this.m_DefaultFloat = value;
            }
        }

        public int defaultInt
        {
            get
            {
                return this.m_DefaultInt;
            }
            set
            {
                this.m_DefaultInt = value;
            }
        }

        public string name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        public int nameHash
        {
            get
            {
                return Animator.StringToHash(this.m_Name);
            }
        }

        public AnimatorControllerParameterType type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
    }
}

