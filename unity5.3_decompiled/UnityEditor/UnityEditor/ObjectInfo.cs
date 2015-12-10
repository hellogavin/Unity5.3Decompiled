namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    internal class ObjectInfo
    {
        public string className;
        public int instanceId;
        public int memorySize;
        public string name;
        public int reason;
        public List<ObjectInfo> referencedBy;
    }
}

