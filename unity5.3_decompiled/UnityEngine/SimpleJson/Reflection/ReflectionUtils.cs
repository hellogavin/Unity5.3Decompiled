namespace SimpleJson.Reflection
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [GeneratedCode("reflection-utils", "1.0.0")]
    internal class ReflectionUtils
    {
        private static readonly object[] EmptyObjects = new object[0];

        public static Attribute GetAttribute(MemberInfo info, Type type)
        {
            if (((info != null) && (type != null)) && Attribute.IsDefined(info, type))
            {
                return Attribute.GetCustomAttribute(info, type);
            }
            return null;
        }

        public static Attribute GetAttribute(Type objectType, Type attributeType)
        {
            if (((objectType != null) && (attributeType != null)) && Attribute.IsDefined(objectType, attributeType))
            {
                return Attribute.GetCustomAttribute(objectType, attributeType);
            }
            return null;
        }

        public static ConstructorDelegate GetConstructorByReflection(ConstructorInfo constructorInfo)
        {
            <GetConstructorByReflection>c__AnonStorey2 storey = new <GetConstructorByReflection>c__AnonStorey2 {
                constructorInfo = constructorInfo
            };
            return new ConstructorDelegate(storey.<>m__6);
        }

        public static ConstructorDelegate GetConstructorByReflection(Type type, params Type[] argsType)
        {
            ConstructorInfo constructorInfo = GetConstructorInfo(type, argsType);
            return ((constructorInfo != null) ? GetConstructorByReflection(constructorInfo) : null);
        }

        public static ConstructorInfo GetConstructorInfo(Type type, params Type[] argsType)
        {
            IEnumerator<ConstructorInfo> enumerator = GetConstructors(type).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ConstructorInfo current = enumerator.Current;
                    ParameterInfo[] parameters = current.GetParameters();
                    if (argsType.Length == parameters.Length)
                    {
                        int index = 0;
                        bool flag = true;
                        foreach (ParameterInfo info2 in current.GetParameters())
                        {
                            if (info2.ParameterType != argsType[index])
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            return current;
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return null;
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
        {
            return type.GetConstructors();
        }

        public static ConstructorDelegate GetContructor(ConstructorInfo constructorInfo)
        {
            return GetConstructorByReflection(constructorInfo);
        }

        public static ConstructorDelegate GetContructor(Type type, params Type[] argsType)
        {
            return GetConstructorByReflection(type, argsType);
        }

        public static IEnumerable<FieldInfo> GetFields(Type type)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public static Type[] GetGenericTypeArguments(Type type)
        {
            return type.GetGenericArguments();
        }

        public static GetDelegate GetGetMethod(FieldInfo fieldInfo)
        {
            return GetGetMethodByReflection(fieldInfo);
        }

        public static GetDelegate GetGetMethod(PropertyInfo propertyInfo)
        {
            return GetGetMethodByReflection(propertyInfo);
        }

        public static GetDelegate GetGetMethodByReflection(FieldInfo fieldInfo)
        {
            <GetGetMethodByReflection>c__AnonStorey4 storey = new <GetGetMethodByReflection>c__AnonStorey4 {
                fieldInfo = fieldInfo
            };
            return new GetDelegate(storey.<>m__8);
        }

        public static GetDelegate GetGetMethodByReflection(PropertyInfo propertyInfo)
        {
            <GetGetMethodByReflection>c__AnonStorey3 storey = new <GetGetMethodByReflection>c__AnonStorey3 {
                methodInfo = GetGetterMethodInfo(propertyInfo)
            };
            return new GetDelegate(storey.<>m__7);
        }

        public static MethodInfo GetGetterMethodInfo(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetGetMethod(true);
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public static SetDelegate GetSetMethod(FieldInfo fieldInfo)
        {
            return GetSetMethodByReflection(fieldInfo);
        }

        public static SetDelegate GetSetMethod(PropertyInfo propertyInfo)
        {
            return GetSetMethodByReflection(propertyInfo);
        }

        public static SetDelegate GetSetMethodByReflection(FieldInfo fieldInfo)
        {
            <GetSetMethodByReflection>c__AnonStorey6 storey = new <GetSetMethodByReflection>c__AnonStorey6 {
                fieldInfo = fieldInfo
            };
            return new SetDelegate(storey.<>m__A);
        }

        public static SetDelegate GetSetMethodByReflection(PropertyInfo propertyInfo)
        {
            <GetSetMethodByReflection>c__AnonStorey5 storey = new <GetSetMethodByReflection>c__AnonStorey5 {
                methodInfo = GetSetterMethodInfo(propertyInfo)
            };
            return new SetDelegate(storey.<>m__9);
        }

        public static MethodInfo GetSetterMethodInfo(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetSetMethod(true);
        }

        public static bool IsAssignableFrom(Type type1, Type type2)
        {
            return type1.IsAssignableFrom(type2);
        }

        public static bool IsNullableType(Type type)
        {
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static bool IsTypeDictionary(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return true;
            }
            if (!type.IsGenericType)
            {
                return false;
            }
            return (type.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        public static bool IsTypeGenericeCollectionInterface(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            return (((genericTypeDefinition == typeof(IList<>)) || (genericTypeDefinition == typeof(ICollection<>))) || (genericTypeDefinition == typeof(IEnumerable<>)));
        }

        public static bool IsValueType(Type type)
        {
            return type.IsValueType;
        }

        public static object ToNullableType(object obj, Type nullableType)
        {
            return ((obj != null) ? Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture) : null);
        }

        [CompilerGenerated]
        private sealed class <GetConstructorByReflection>c__AnonStorey2
        {
            internal ConstructorInfo constructorInfo;

            internal object <>m__6(object[] args)
            {
                return this.constructorInfo.Invoke(args);
            }
        }

        [CompilerGenerated]
        private sealed class <GetGetMethodByReflection>c__AnonStorey3
        {
            internal MethodInfo methodInfo;

            internal object <>m__7(object source)
            {
                return this.methodInfo.Invoke(source, ReflectionUtils.EmptyObjects);
            }
        }

        [CompilerGenerated]
        private sealed class <GetGetMethodByReflection>c__AnonStorey4
        {
            internal FieldInfo fieldInfo;

            internal object <>m__8(object source)
            {
                return this.fieldInfo.GetValue(source);
            }
        }

        [CompilerGenerated]
        private sealed class <GetSetMethodByReflection>c__AnonStorey5
        {
            internal MethodInfo methodInfo;

            internal void <>m__9(object source, object value)
            {
                object[] parameters = new object[] { value };
                this.methodInfo.Invoke(source, parameters);
            }
        }

        [CompilerGenerated]
        private sealed class <GetSetMethodByReflection>c__AnonStorey6
        {
            internal FieldInfo fieldInfo;

            internal void <>m__A(object source, object value)
            {
                this.fieldInfo.SetValue(source, value);
            }
        }

        public delegate object ConstructorDelegate(params object[] args);

        public delegate object GetDelegate(object source);

        public delegate void SetDelegate(object source, object value);

        public sealed class ThreadSafeDictionary<TKey, TValue> : IEnumerable, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
        {
            private Dictionary<TKey, TValue> _dictionary;
            private readonly object _lock;
            private readonly ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> _valueFactory;

            public ThreadSafeDictionary(ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> valueFactory)
            {
                this._lock = new object();
                this._valueFactory = valueFactory;
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public void Add(TKey key, TValue value)
            {
                throw new NotImplementedException();
            }

            private TValue AddValue(TKey key)
            {
                TValue local = this._valueFactory(key);
                object obj2 = this._lock;
                lock (obj2)
                {
                    TValue local2;
                    if (this._dictionary == null)
                    {
                        this._dictionary = new Dictionary<TKey, TValue>();
                        this._dictionary[key] = local;
                        return local;
                    }
                    if (this._dictionary.TryGetValue(key, out local2))
                    {
                        return local2;
                    }
                    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._dictionary);
                    dictionary[key] = local;
                    this._dictionary = dictionary;
                }
                return local;
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(TKey key)
            {
                return this._dictionary.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            private TValue Get(TKey key)
            {
                TValue local;
                if (this._dictionary == null)
                {
                    return this.AddValue(key);
                }
                if (!this._dictionary.TryGetValue(key, out local))
                {
                    return this.AddValue(key);
                }
                return local;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return this._dictionary.GetEnumerator();
            }

            public bool Remove(TKey key)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this._dictionary.GetEnumerator();
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                value = this[key];
                return true;
            }

            public int Count
            {
                get
                {
                    return this._dictionary.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public TValue this[TKey key]
            {
                get
                {
                    return this.Get(key);
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<TKey> Keys
            {
                get
                {
                    return this._dictionary.Keys;
                }
            }

            public ICollection<TValue> Values
            {
                get
                {
                    return this._dictionary.Values;
                }
            }
        }

        public delegate TValue ThreadSafeDictionaryValueFactory<TKey, TValue>(TKey key);
    }
}

