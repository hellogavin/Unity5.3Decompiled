namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SerializedModule
    {
        protected string m_ModuleName;
        private SerializedObject m_Object;

        public SerializedModule(SerializedObject o, string name)
        {
            this.m_Object = o;
            this.m_ModuleName = name;
        }

        public static string Concat(string a, string b)
        {
            return (a + "." + b);
        }

        public SerializedProperty GetProperty(string name)
        {
            SerializedProperty property = this.m_Object.FindProperty(Concat(this.m_ModuleName, name));
            if (property == null)
            {
                Debug.LogError("GetProperty: not found: " + Concat(this.m_ModuleName, name));
            }
            return property;
        }

        public SerializedProperty GetProperty(string structName, string propName)
        {
            SerializedProperty property = this.m_Object.FindProperty(Concat(Concat(this.m_ModuleName, structName), propName));
            if (property == null)
            {
                Debug.LogError("GetProperty: not found: " + Concat(Concat(this.m_ModuleName, structName), propName));
            }
            return property;
        }

        public SerializedProperty GetProperty0(string name)
        {
            SerializedProperty property = this.m_Object.FindProperty(name);
            if (property == null)
            {
                Debug.LogError("GetProperty0: not found: " + name);
            }
            return property;
        }

        public SerializedProperty GetProperty0(string structName, string propName)
        {
            SerializedProperty property = this.m_Object.FindProperty(Concat(structName, propName));
            if (property == null)
            {
                Debug.LogError("GetProperty: not found: " + Concat(structName, propName));
            }
            return property;
        }

        public string GetUniqueModuleName()
        {
            return Concat(string.Empty + this.m_Object.targetObject.GetInstanceID(), this.m_ModuleName);
        }

        internal SerializedObject serializedObject
        {
            get
            {
                return this.m_Object;
            }
        }
    }
}

