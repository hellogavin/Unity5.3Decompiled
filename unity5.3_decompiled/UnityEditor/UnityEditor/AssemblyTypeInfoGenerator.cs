namespace UnityEditor
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.SerializationLogic;
    using UnityEngine;

    internal class AssemblyTypeInfoGenerator
    {
        private AssemblyDefinition assembly_;
        private List<ClassInfo> classes_;
        private TypeResolver typeResolver;

        public AssemblyTypeInfoGenerator(string assembly, string[] searchDirs)
        {
            this.classes_ = new List<ClassInfo>();
            this.typeResolver = new TypeResolver(null);
            ReaderParameters parameters = new ReaderParameters {
                AssemblyResolver = AssemblyResolver.WithSearchDirs(searchDirs)
            };
            this.assembly_ = AssemblyDefinition.ReadAssembly(assembly, parameters);
        }

        public AssemblyTypeInfoGenerator(string assembly, IAssemblyResolver resolver)
        {
            this.classes_ = new List<ClassInfo>();
            this.typeResolver = new TypeResolver(null);
            ReaderParameters parameters = new ReaderParameters {
                AssemblyResolver = resolver
            };
            this.assembly_ = AssemblyDefinition.ReadAssembly(assembly, parameters);
        }

        private void AddBaseType(TypeReference typeRef, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
        {
            TypeReference baseType = typeRef.Resolve().BaseType;
            if (baseType != null)
            {
                if (typeRef.IsGenericInstance && baseType.IsGenericInstance)
                {
                    GenericInstanceType type = (GenericInstanceType) baseType;
                    baseType = this.MakeGenericInstance(type.ElementType, type.GenericArguments, genericInstanceTypeMap);
                }
                this.AddType(baseType, genericInstanceTypeMap);
            }
        }

        private void AddNestedTypes(TypeDefinition type, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
        {
            foreach (TypeDefinition definition in type.NestedTypes)
            {
                this.AddType(definition, genericInstanceTypeMap);
            }
        }

        private void AddType(TypeReference typeRef, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
        {
            <AddType>c__AnonStorey64 storey = new <AddType>c__AnonStorey64 {
                typeRef = typeRef,
                <>f__this = this
            };
            if (!this.classes_.Any<ClassInfo>(new Func<ClassInfo, bool>(storey.<>m__D8)))
            {
                TypeDefinition definition;
                try
                {
                    definition = storey.typeRef.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    return;
                }
                catch (NotSupportedException)
                {
                    return;
                }
                if (definition != null)
                {
                    if (storey.typeRef.IsGenericInstance)
                    {
                        Collection<TypeReference> genericArguments = ((GenericInstanceType) storey.typeRef).GenericArguments;
                        Collection<GenericParameter> genericParameters = definition.GenericParameters;
                        for (int i = 0; i < genericArguments.Count; i++)
                        {
                            if (genericParameters[i] != genericArguments[i])
                            {
                                genericInstanceTypeMap[genericParameters[i]] = genericArguments[i];
                            }
                        }
                        this.typeResolver.Add((GenericInstanceType) storey.typeRef);
                    }
                    bool flag = false;
                    try
                    {
                        flag = UnitySerializationLogic.ShouldImplementIDeserializable(definition);
                    }
                    catch
                    {
                    }
                    if (!flag)
                    {
                        this.AddNestedTypes(definition, genericInstanceTypeMap);
                    }
                    else
                    {
                        ClassInfo item = new ClassInfo {
                            name = this.GetMonoEmbeddedFullTypeNameFor(storey.typeRef),
                            fields = this.GetFields(definition, storey.typeRef.IsGenericInstance, genericInstanceTypeMap)
                        };
                        this.classes_.Add(item);
                        this.AddNestedTypes(definition, genericInstanceTypeMap);
                        this.AddBaseType(storey.typeRef, genericInstanceTypeMap);
                    }
                    if (storey.typeRef.IsGenericInstance)
                    {
                        this.typeResolver.Remove((GenericInstanceType) storey.typeRef);
                    }
                }
            }
        }

        public ClassInfo[] GatherClassInfo()
        {
            foreach (ModuleDefinition definition in this.assembly_.Modules)
            {
                foreach (TypeDefinition definition2 in definition.Types)
                {
                    if (definition2.Name != "<Module>")
                    {
                        this.AddType(definition2, new Dictionary<TypeReference, TypeReference>());
                    }
                }
            }
            return this.classes_.ToArray();
        }

        private FieldInfo? GetFieldInfo(TypeDefinition type, FieldDefinition field, bool isDeclaringTypeGenericInstance, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
        {
            TypeReference fieldType;
            if (!this.WillSerialize(field))
            {
                return null;
            }
            FieldInfo info = new FieldInfo {
                name = field.Name
            };
            if (isDeclaringTypeGenericInstance)
            {
                fieldType = this.ResolveGenericInstanceType(field.FieldType, genericInstanceTypeMap);
            }
            else
            {
                fieldType = field.FieldType;
            }
            info.type = this.GetMonoEmbeddedFullTypeNameFor(fieldType);
            return new FieldInfo?(info);
        }

        private FieldInfo[] GetFields(TypeDefinition type, bool isGenericInstance, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
        {
            List<FieldInfo> list = new List<FieldInfo>();
            foreach (FieldDefinition definition in type.Fields)
            {
                FieldInfo? nullable = this.GetFieldInfo(type, definition, isGenericInstance, genericInstanceTypeMap);
                if (nullable.HasValue)
                {
                    list.Add(nullable.Value);
                }
            }
            return list.ToArray();
        }

        private string GetMonoEmbeddedFullTypeNameFor(TypeReference type)
        {
            string fullName;
            TypeSpecification specification = type as TypeSpecification;
            if ((specification != null) && specification.IsRequiredModifier)
            {
                fullName = specification.ElementType.FullName;
            }
            else if (type.IsRequiredModifier)
            {
                fullName = type.GetElementType().FullName;
            }
            else
            {
                fullName = type.FullName;
            }
            return fullName.Replace('/', '+').Replace('<', '[').Replace('>', ']');
        }

        private TypeReference MakeGenericInstance(TypeReference genericClass, IEnumerable<TypeReference> arguments, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
        {
            <MakeGenericInstance>c__AnonStorey65 storey = new <MakeGenericInstance>c__AnonStorey65 {
                genericInstanceTypeMap = genericInstanceTypeMap,
                <>f__this = this
            };
            GenericInstanceType type = new GenericInstanceType(genericClass);
            IEnumerator<TypeReference> enumerator = arguments.Select<TypeReference, TypeReference>(new Func<TypeReference, TypeReference>(storey.<>m__D9)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    TypeReference current = enumerator.Current;
                    type.GenericArguments.Add(current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return type;
        }

        private TypeReference ResolveGenericInstanceType(TypeReference typeToResolve, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
        {
            ArrayType type = typeToResolve as ArrayType;
            if (type != null)
            {
                typeToResolve = new ArrayType(this.ResolveGenericInstanceType(type.ElementType, genericInstanceTypeMap), type.Rank);
            }
            while (genericInstanceTypeMap.ContainsKey(typeToResolve))
            {
                typeToResolve = genericInstanceTypeMap[typeToResolve];
            }
            if (typeToResolve.IsGenericInstance)
            {
                GenericInstanceType type2 = (GenericInstanceType) typeToResolve;
                typeToResolve = this.MakeGenericInstance(type2.ElementType, type2.GenericArguments, genericInstanceTypeMap);
            }
            return typeToResolve;
        }

        private bool WillSerialize(FieldDefinition field)
        {
            try
            {
                return UnitySerializationLogic.WillUnitySerialize(field, this.typeResolver);
            }
            catch (Exception exception)
            {
                object[] args = new object[] { field.FullName, field.Module.FullyQualifiedName, exception.Message };
                Debug.LogFormat("Field '{0}' from '{1}', exception {2}", args);
                return false;
            }
        }

        public ClassInfo[] ClassInfoArray
        {
            get
            {
                return this.classes_.ToArray();
            }
        }

        [CompilerGenerated]
        private sealed class <AddType>c__AnonStorey64
        {
            internal AssemblyTypeInfoGenerator <>f__this;
            internal TypeReference typeRef;

            internal bool <>m__D8(AssemblyTypeInfoGenerator.ClassInfo x)
            {
                return (x.name == this.<>f__this.GetMonoEmbeddedFullTypeNameFor(this.typeRef));
            }
        }

        [CompilerGenerated]
        private sealed class <MakeGenericInstance>c__AnonStorey65
        {
            internal AssemblyTypeInfoGenerator <>f__this;
            internal Dictionary<TypeReference, TypeReference> genericInstanceTypeMap;

            internal TypeReference <>m__D9(TypeReference x)
            {
                return this.<>f__this.ResolveGenericInstanceType(x, this.genericInstanceTypeMap);
            }
        }

        private class AssemblyResolver : BaseAssemblyResolver
        {
            private readonly IDictionary m_Assemblies;

            private AssemblyResolver() : this(new Hashtable())
            {
            }

            private AssemblyResolver(IDictionary assemblyCache)
            {
                this.m_Assemblies = assemblyCache;
            }

            public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
            {
                AssemblyDefinition definition = (AssemblyDefinition) this.m_Assemblies[name.Name];
                if (definition == null)
                {
                    definition = base.Resolve(name, parameters);
                    this.m_Assemblies[name.Name] = definition;
                }
                return definition;
            }

            public static IAssemblyResolver WithSearchDirs(params string[] searchDirs)
            {
                AssemblyTypeInfoGenerator.AssemblyResolver resolver = new AssemblyTypeInfoGenerator.AssemblyResolver();
                foreach (string str in searchDirs)
                {
                    resolver.AddSearchDirectory(str);
                }
                return resolver;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ClassInfo
        {
            public string name;
            public AssemblyTypeInfoGenerator.FieldInfo[] fields;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FieldInfo
        {
            public string name;
            public string type;
        }
    }
}

