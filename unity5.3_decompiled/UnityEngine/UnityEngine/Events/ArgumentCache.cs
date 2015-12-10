namespace UnityEngine.Events
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    internal class ArgumentCache : ISerializationCallbackReceiver
    {
        private const string kCultureString = @", Culture=\w+";
        private const string kTokenString = @", PublicKeyToken=\w+";
        private const string kVersionString = @", Version=\d+.\d+.\d+.\d+";
        [SerializeField]
        private bool m_BoolArgument;
        [FormerlySerializedAs("floatArgument"), SerializeField]
        private float m_FloatArgument;
        [SerializeField, FormerlySerializedAs("intArgument")]
        private int m_IntArgument;
        [SerializeField, FormerlySerializedAs("objectArgument")]
        private Object m_ObjectArgument;
        [FormerlySerializedAs("objectArgumentAssemblyTypeName"), SerializeField]
        private string m_ObjectArgumentAssemblyTypeName;
        [SerializeField, FormerlySerializedAs("stringArgument")]
        private string m_StringArgument;

        public void OnAfterDeserialize()
        {
            this.TidyAssemblyTypeName();
        }

        public void OnBeforeSerialize()
        {
            this.TidyAssemblyTypeName();
        }

        private void TidyAssemblyTypeName()
        {
            if (!string.IsNullOrEmpty(this.m_ObjectArgumentAssemblyTypeName))
            {
                this.m_ObjectArgumentAssemblyTypeName = Regex.Replace(this.m_ObjectArgumentAssemblyTypeName, @", Version=\d+.\d+.\d+.\d+", string.Empty);
                this.m_ObjectArgumentAssemblyTypeName = Regex.Replace(this.m_ObjectArgumentAssemblyTypeName, @", Culture=\w+", string.Empty);
                this.m_ObjectArgumentAssemblyTypeName = Regex.Replace(this.m_ObjectArgumentAssemblyTypeName, @", PublicKeyToken=\w+", string.Empty);
            }
        }

        public bool boolArgument
        {
            get
            {
                return this.m_BoolArgument;
            }
            set
            {
                this.m_BoolArgument = value;
            }
        }

        public float floatArgument
        {
            get
            {
                return this.m_FloatArgument;
            }
            set
            {
                this.m_FloatArgument = value;
            }
        }

        public int intArgument
        {
            get
            {
                return this.m_IntArgument;
            }
            set
            {
                this.m_IntArgument = value;
            }
        }

        public string stringArgument
        {
            get
            {
                return this.m_StringArgument;
            }
            set
            {
                this.m_StringArgument = value;
            }
        }

        public Object unityObjectArgument
        {
            get
            {
                return this.m_ObjectArgument;
            }
            set
            {
                this.m_ObjectArgument = value;
                this.m_ObjectArgumentAssemblyTypeName = (value == null) ? string.Empty : value.GetType().AssemblyQualifiedName;
            }
        }

        public string unityObjectArgumentAssemblyTypeName
        {
            get
            {
                return this.m_ObjectArgumentAssemblyTypeName;
            }
        }
    }
}

