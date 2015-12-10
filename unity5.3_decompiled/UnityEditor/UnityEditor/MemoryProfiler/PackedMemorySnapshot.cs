namespace UnityEditor.MemoryProfiler
{
    using System;
    using UnityEngine;

    [Serializable]
    public class PackedMemorySnapshot
    {
        [SerializeField]
        internal Connection[] m_Connections;
        [SerializeField]
        internal PackedGCHandle[] m_GcHandles;
        [SerializeField]
        internal MemorySection[] m_ManagedHeapSections;
        [SerializeField]
        internal MemorySection[] m_ManagedStacks;
        [SerializeField]
        internal PackedNativeUnityEngineObject[] m_NativeObjects;
        [SerializeField]
        internal PackedNativeType[] m_NativeTypes;
        [SerializeField]
        internal TypeDescription[] m_TypeDescriptions;
        [SerializeField]
        internal VirtualMachineInformation m_VirtualMachineInformation = new VirtualMachineInformation();

        internal PackedMemorySnapshot()
        {
        }

        public Connection[] connections
        {
            get
            {
                return this.m_Connections;
            }
        }

        public PackedGCHandle[] gcHandles
        {
            get
            {
                return this.m_GcHandles;
            }
        }

        public MemorySection[] managedHeapSections
        {
            get
            {
                return this.m_ManagedHeapSections;
            }
        }

        public PackedNativeUnityEngineObject[] nativeObjects
        {
            get
            {
                return this.m_NativeObjects;
            }
        }

        public PackedNativeType[] nativeTypes
        {
            get
            {
                return this.m_NativeTypes;
            }
        }

        public TypeDescription[] typeDescriptions
        {
            get
            {
                return this.m_TypeDescriptions;
            }
        }

        public VirtualMachineInformation virtualMachineInformation
        {
            get
            {
                return this.m_VirtualMachineInformation;
            }
        }
    }
}

