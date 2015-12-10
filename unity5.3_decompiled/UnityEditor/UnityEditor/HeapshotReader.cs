namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class HeapshotReader
    {
        private List<ObjectInfo> allObjects = new List<ObjectInfo>();
        private List<TypeInfo> allTypes = new List<TypeInfo>();
        private const string kLogFileLabel = "heap-shot logfile";
        private const int kLogVersion = 6;
        private const uint kMagicNumber = 0x4eabfdd1;
        private ObjectInfo kUnity = new ObjectInfo(new TypeInfo("<Unity>"), 0, ObjectType.UnityRoot);
        private ObjectInfo kUnmanagedObject = new ObjectInfo(new TypeInfo("Unmanaged"), 0);
        private Dictionary<uint, ObjectInfo> objects = new Dictionary<uint, ObjectInfo>();
        private List<ReferenceInfo> roots = new List<ReferenceInfo>();
        private Dictionary<uint, TypeInfo> types = new Dictionary<uint, TypeInfo>();

        public List<ObjectInfo> GetObjectsOfType(string name)
        {
            List<ObjectInfo> list = new List<ObjectInfo>();
            foreach (ObjectInfo info in this.allObjects)
            {
                if (info.typeInfo.name == name)
                {
                    list.Add(info);
                }
            }
            return list;
        }

        public bool Open(string fileName)
        {
            Stream stream;
            BinaryReader reader;
            this.types.Clear();
            this.objects.Clear();
            this.roots.Clear();
            this.allObjects.Clear();
            this.allTypes.Clear();
            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
            try
            {
                reader = new BinaryReader(stream);
            }
            catch (Exception exception2)
            {
                Console.WriteLine(exception2.Message);
                stream.Close();
                return false;
            }
            this.ReadHeader(reader);
            while (this.ReadData(reader))
            {
            }
            this.ResolveReferences();
            this.ResolveInverseReferences();
            this.ResolveRoots();
            reader.Close();
            stream.Close();
            return true;
        }

        private bool ReadData(BinaryReader reader)
        {
            Tag tag = (Tag) reader.ReadByte();
            switch (tag)
            {
                case Tag.Type:
                    this.ReadType(reader);
                    break;

                case Tag.Object:
                    this.ReadObject(reader);
                    break;

                case Tag.UnityObjects:
                    this.ReadUnityObjects(reader);
                    break;

                case Tag.EndOfFile:
                    return false;

                default:
                    throw new Exception("Unknown tag! " + tag);
            }
            return true;
        }

        private void ReadHeader(BinaryReader reader)
        {
            uint num = reader.ReadUInt32();
            if (num != 0x4eabfdd1)
            {
                throw new Exception(string.Format("Bad magic number: expected {0}, found {1}", 0x4eabfdd1, num));
            }
            int num2 = reader.ReadInt32();
            string str = reader.ReadString();
            if (str != "heap-shot logfile")
            {
                throw new Exception("Unknown file label in heap-shot outfile");
            }
            int num3 = 6;
            if (num2 != num3)
            {
                throw new Exception(string.Format("Version error in {0}: expected {1}, found {2}", str, num3, num2));
            }
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
        }

        private void ReadObject(BinaryReader reader)
        {
            uint num3;
            uint num = reader.ReadUInt32();
            uint key = reader.ReadUInt32();
            ObjectInfo item = new ObjectInfo {
                code = num,
                size = reader.ReadUInt32()
            };
            if (!this.types.ContainsKey(key))
            {
                throw new Exception(string.Format("Failed to find type info {0} for object {1}!!!", key, num));
            }
            item.typeInfo = this.types[key];
            while ((num3 = reader.ReadUInt32()) != 0)
            {
                ReferenceInfo info2 = new ReferenceInfo {
                    code = num3
                };
                uint num4 = reader.ReadUInt32();
                if (num4 == 0)
                {
                    info2.fieldInfo = null;
                }
                else if (item.typeInfo.fields.ContainsKey(num4))
                {
                    info2.fieldInfo = item.typeInfo.fields[num4];
                }
                else
                {
                    info2.fieldInfo = null;
                }
                item.references.Add(info2);
            }
            if (this.objects.ContainsKey(num))
            {
                throw new Exception(string.Format("Object {0} was already loaded?!", num));
            }
            item.type = (num != key) ? ObjectType.Managed : ObjectType.Root;
            this.objects[num] = item;
            this.allObjects.Add(item);
        }

        private void ReadType(BinaryReader reader)
        {
            uint num2;
            uint key = reader.ReadUInt32();
            TypeInfo item = new TypeInfo {
                name = reader.ReadString()
            };
            while ((num2 = reader.ReadUInt32()) != 0)
            {
                FieldInfo info2 = new FieldInfo {
                    name = reader.ReadString()
                };
                item.fields[num2] = info2;
            }
            if (this.types.ContainsKey(key))
            {
                throw new Exception(string.Format("Type info for object {0} was already loaded!!!", key));
            }
            this.types[key] = item;
            this.allTypes.Add(item);
        }

        private void ReadUnityObjects(BinaryReader reader)
        {
            uint num;
            while ((num = reader.ReadUInt32()) != 0)
            {
                if (this.objects.ContainsKey(num))
                {
                    BackReferenceInfo item = new BackReferenceInfo {
                        parentObject = this.kUnity,
                        fieldInfo = new FieldInfo(this.objects[num].typeInfo.name)
                    };
                    this.objects[num].inverseReferences.Add(item);
                }
            }
        }

        private void ResolveInverseReferences()
        {
            foreach (KeyValuePair<uint, ObjectInfo> pair in this.objects)
            {
                foreach (ReferenceInfo info in pair.Value.references)
                {
                    BackReferenceInfo item = new BackReferenceInfo {
                        parentObject = pair.Value,
                        fieldInfo = info.fieldInfo
                    };
                    info.referencedObject.inverseReferences.Add(item);
                }
            }
        }

        private void ResolveReferences()
        {
            foreach (KeyValuePair<uint, ObjectInfo> pair in this.objects)
            {
                foreach (ReferenceInfo info in pair.Value.references)
                {
                    if (!this.objects.ContainsKey(info.code))
                    {
                        info.referencedObject = this.kUnmanagedObject;
                    }
                    else
                    {
                        info.referencedObject = this.objects[info.code];
                        if (info.fieldInfo == null)
                        {
                            info.fieldInfo = new FieldInfo();
                            info.fieldInfo.name = info.referencedObject.typeInfo.name;
                        }
                    }
                }
            }
        }

        private void ResolveRoots()
        {
            foreach (KeyValuePair<uint, ObjectInfo> pair in this.objects)
            {
                if (pair.Value.type == ObjectType.Root)
                {
                    foreach (ReferenceInfo info in pair.Value.references)
                    {
                        this.roots.Add(info);
                    }
                }
            }
        }

        public List<ObjectInfo> Objects
        {
            get
            {
                return this.allObjects;
            }
        }

        public List<ReferenceInfo> Roots
        {
            get
            {
                return this.roots;
            }
        }

        public List<TypeInfo> Types
        {
            get
            {
                return this.allTypes;
            }
        }

        public class BackReferenceInfo
        {
            public HeapshotReader.FieldInfo fieldInfo;
            public HeapshotReader.ObjectInfo parentObject;
        }

        public class FieldInfo
        {
            public string name;

            public FieldInfo()
            {
                this.name = string.Empty;
            }

            public FieldInfo(string name)
            {
                this.name = string.Empty;
                this.name = name;
            }
        }

        public class ObjectInfo
        {
            public uint code;
            public List<HeapshotReader.BackReferenceInfo> inverseReferences;
            public List<HeapshotReader.ReferenceInfo> references;
            public uint size;
            public HeapshotReader.ObjectType type;
            public HeapshotReader.TypeInfo typeInfo;

            public ObjectInfo()
            {
                this.references = new List<HeapshotReader.ReferenceInfo>();
                this.inverseReferences = new List<HeapshotReader.BackReferenceInfo>();
            }

            public ObjectInfo(HeapshotReader.TypeInfo typeInfo, uint size)
            {
                this.references = new List<HeapshotReader.ReferenceInfo>();
                this.inverseReferences = new List<HeapshotReader.BackReferenceInfo>();
                this.typeInfo = typeInfo;
                this.size = size;
            }

            public ObjectInfo(HeapshotReader.TypeInfo typeInfo, uint size, HeapshotReader.ObjectType type)
            {
                this.references = new List<HeapshotReader.ReferenceInfo>();
                this.inverseReferences = new List<HeapshotReader.BackReferenceInfo>();
                this.typeInfo = typeInfo;
                this.size = size;
                this.type = type;
            }
        }

        public enum ObjectType
        {
            None,
            Root,
            Managed,
            UnityRoot
        }

        public class ReferenceInfo
        {
            public uint code;
            public HeapshotReader.FieldInfo fieldInfo;
            public HeapshotReader.ObjectInfo referencedObject;

            public ReferenceInfo()
            {
            }

            public ReferenceInfo(HeapshotReader.ObjectInfo refObj, HeapshotReader.FieldInfo field)
            {
                this.referencedObject = refObj;
                this.fieldInfo = field;
            }
        }

        public enum Tag
        {
            EndOfFile = 0xff,
            Object = 2,
            Type = 1,
            UnityObjects = 3
        }

        public class TypeInfo
        {
            public Dictionary<uint, HeapshotReader.FieldInfo> fields;
            public string name;

            public TypeInfo()
            {
                this.name = string.Empty;
                this.fields = new Dictionary<uint, HeapshotReader.FieldInfo>();
            }

            public TypeInfo(string name)
            {
                this.name = string.Empty;
                this.fields = new Dictionary<uint, HeapshotReader.FieldInfo>();
                this.name = name;
            }
        }
    }
}

