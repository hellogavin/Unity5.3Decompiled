namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class SerializedObject
    {
        private IntPtr m_Property;

        public SerializedObject(Object obj)
        {
            Object[] monoObjs = new Object[] { obj };
            this.InternalCreate(monoObjs);
        }

        public SerializedObject(Object[] objs)
        {
            this.InternalCreate(objs);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool ApplyModifiedProperties();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool ApplyModifiedPropertiesWithoutUndo();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void Cache(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CopyFromSerializedProperty(SerializedProperty prop);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern PropertyModification ExtractPropertyModification(string propertyPath);
        ~SerializedObject()
        {
            this.Dispose();
        }

        public SerializedProperty FindProperty(string propertyPath)
        {
            SerializedProperty property = this.GetIterator_Internal();
            property.m_SerializedObject = this;
            if (property.FindPropertyInternal(propertyPath))
            {
                return property;
            }
            return null;
        }

        public SerializedProperty GetIterator()
        {
            SerializedProperty property = this.GetIterator_Internal();
            property.m_SerializedObject = this;
            return property;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern SerializedProperty GetIterator_Internal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalCreate(Object[] monoObjs);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern SerializedObject LoadFromCache(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetIsDifferentCacheDirty();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Update();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void UpdateIfDirtyOrScript();

        internal bool hasModifiedProperties { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal InspectorMode inspectorMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isEditingMultipleObjects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Object targetObject { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Object[] targetObjects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

