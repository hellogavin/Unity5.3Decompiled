namespace SimpleJson
{
    using SimpleJson.Reflection;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [GeneratedCode("simple-json", "1.0.0")]
    internal class PocoJsonSerializerStrategy : IJsonSerializerStrategy
    {
        internal static readonly Type[] ArrayConstructorParameterTypes = new Type[] { typeof(int) };
        internal IDictionary<Type, ReflectionUtils.ConstructorDelegate> ConstructorCache;
        internal static readonly Type[] EmptyTypes = new Type[0];
        internal IDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>> GetCache;
        private static readonly string[] Iso8601Format = new string[] { @"yyyy-MM-dd\THH:mm:ss.FFFFFFF\Z", @"yyyy-MM-dd\THH:mm:ss\Z", @"yyyy-MM-dd\THH:mm:ssK" };
        internal IDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>> SetCache;

        public PocoJsonSerializerStrategy()
        {
            this.ConstructorCache = new ReflectionUtils.ThreadSafeDictionary<Type, ReflectionUtils.ConstructorDelegate>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, ReflectionUtils.ConstructorDelegate>(this.ContructorDelegateFactory));
            this.GetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(this.GetterValueFactory));
            this.SetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(this.SetterValueFactory));
        }

        internal virtual ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
        {
            return ReflectionUtils.GetContructor(key, !key.IsArray ? EmptyTypes : ArrayConstructorParameterTypes);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public virtual object DeserializeObject(object value, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            string str = value as string;
            if ((type == typeof(Guid)) && string.IsNullOrEmpty(str))
            {
                return new Guid();
            }
            if (value == null)
            {
                return null;
            }
            object source = null;
            if (str != null)
            {
                if (str.Length != 0)
                {
                    if ((type == typeof(DateTime)) || (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(DateTime))))
                    {
                        return DateTime.ParseExact(str, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    }
                    if ((type == typeof(DateTimeOffset)) || (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(DateTimeOffset))))
                    {
                        return DateTimeOffset.ParseExact(str, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    }
                    if ((type == typeof(Guid)) || (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(Guid))))
                    {
                        return new Guid(str);
                    }
                    return str;
                }
                if (type == typeof(Guid))
                {
                    source = new Guid();
                }
                else if (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(Guid)))
                {
                    source = null;
                }
                else
                {
                    source = str;
                }
                if (!ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(Guid)))
                {
                    return str;
                }
            }
            else if (value is bool)
            {
                return value;
            }
            bool flag = value is long;
            bool flag2 = value is double;
            if ((flag && (type == typeof(long))) || (flag2 && (type == typeof(double))))
            {
                return value;
            }
            if ((flag2 && (type != typeof(double))) || (flag && (type != typeof(long))))
            {
                source = !typeof(IConvertible).IsAssignableFrom(type) ? value : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            else
            {
                IDictionary<string, object> dictionary = value as IDictionary<string, object>;
                if (dictionary != null)
                {
                    IDictionary<string, object> dictionary2 = dictionary;
                    if (ReflectionUtils.IsTypeDictionary(type))
                    {
                        Type[] genericTypeArguments = ReflectionUtils.GetGenericTypeArguments(type);
                        Type type2 = genericTypeArguments[0];
                        Type type3 = genericTypeArguments[1];
                        Type[] typeArguments = new Type[] { type2, type3 };
                        Type type4 = typeof(Dictionary<,>).MakeGenericType(typeArguments);
                        IDictionary dictionary3 = (IDictionary) this.ConstructorCache[type4](null);
                        IEnumerator<KeyValuePair<string, object>> enumerator = dictionary2.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                KeyValuePair<string, object> current = enumerator.Current;
                                dictionary3.Add(current.Key, this.DeserializeObject(current.Value, type3));
                            }
                        }
                        finally
                        {
                            if (enumerator == null)
                            {
                            }
                            enumerator.Dispose();
                        }
                        return dictionary3;
                    }
                    if (type == typeof(object))
                    {
                        return value;
                    }
                    source = this.ConstructorCache[type](null);
                    IEnumerator<KeyValuePair<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>> enumerator2 = this.SetCache[type].GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            object obj3;
                            KeyValuePair<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> pair2 = enumerator2.Current;
                            if (dictionary2.TryGetValue(pair2.Key, out obj3))
                            {
                                obj3 = this.DeserializeObject(obj3, pair2.Value.Key);
                                pair2.Value.Value(source, obj3);
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator2 == null)
                        {
                        }
                        enumerator2.Dispose();
                    }
                    return source;
                }
                IList<object> list = value as IList<object>;
                if (list == null)
                {
                    return source;
                }
                IList<object> list2 = list;
                IList list3 = null;
                if (type.IsArray)
                {
                    object[] args = new object[] { list2.Count };
                    list3 = (IList) this.ConstructorCache[type](args);
                    int num = 0;
                    IEnumerator<object> enumerator3 = list2.GetEnumerator();
                    try
                    {
                        while (enumerator3.MoveNext())
                        {
                            object obj4 = enumerator3.Current;
                            list3[num++] = this.DeserializeObject(obj4, type.GetElementType());
                        }
                    }
                    finally
                    {
                        if (enumerator3 == null)
                        {
                        }
                        enumerator3.Dispose();
                    }
                    return list3;
                }
                if (ReflectionUtils.IsTypeGenericeCollectionInterface(type) || ReflectionUtils.IsAssignableFrom(typeof(IList), type))
                {
                    Type type5 = ReflectionUtils.GetGenericTypeArguments(type)[0];
                    Type[] typeArray2 = new Type[] { type5 };
                    Type type6 = typeof(List<>).MakeGenericType(typeArray2);
                    object[] objArray2 = new object[] { list2.Count };
                    list3 = (IList) this.ConstructorCache[type6](objArray2);
                    IEnumerator<object> enumerator4 = list2.GetEnumerator();
                    try
                    {
                        while (enumerator4.MoveNext())
                        {
                            object obj5 = enumerator4.Current;
                            list3.Add(this.DeserializeObject(obj5, type5));
                        }
                    }
                    finally
                    {
                        if (enumerator4 == null)
                        {
                        }
                        enumerator4.Dispose();
                    }
                }
                return list3;
            }
            if (ReflectionUtils.IsNullableType(type))
            {
                return ReflectionUtils.ToNullableType(source, type);
            }
            return source;
        }

        internal virtual IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
        {
            IDictionary<string, ReflectionUtils.GetDelegate> dictionary = new Dictionary<string, ReflectionUtils.GetDelegate>();
            IEnumerator<PropertyInfo> enumerator = ReflectionUtils.GetProperties(type).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    PropertyInfo current = enumerator.Current;
                    if (current.CanRead)
                    {
                        MethodInfo getterMethodInfo = ReflectionUtils.GetGetterMethodInfo(current);
                        if (!getterMethodInfo.IsStatic && getterMethodInfo.IsPublic)
                        {
                            dictionary[this.MapClrMemberNameToJsonFieldName(current.Name)] = ReflectionUtils.GetGetMethod(current);
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
            IEnumerator<FieldInfo> enumerator2 = ReflectionUtils.GetFields(type).GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    FieldInfo fieldInfo = enumerator2.Current;
                    if (!fieldInfo.IsStatic && fieldInfo.IsPublic)
                    {
                        dictionary[this.MapClrMemberNameToJsonFieldName(fieldInfo.Name)] = ReflectionUtils.GetGetMethod(fieldInfo);
                    }
                }
            }
            finally
            {
                if (enumerator2 == null)
                {
                }
                enumerator2.Dispose();
            }
            return dictionary;
        }

        protected virtual string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            return clrPropertyName;
        }

        protected virtual object SerializeEnum(Enum p)
        {
            return Convert.ToDouble(p, CultureInfo.InvariantCulture);
        }

        internal virtual IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
        {
            IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> dictionary = new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
            IEnumerator<PropertyInfo> enumerator = ReflectionUtils.GetProperties(type).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    PropertyInfo current = enumerator.Current;
                    if (current.CanWrite)
                    {
                        MethodInfo setterMethodInfo = ReflectionUtils.GetSetterMethodInfo(current);
                        if (!setterMethodInfo.IsStatic && setterMethodInfo.IsPublic)
                        {
                            dictionary[this.MapClrMemberNameToJsonFieldName(current.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(current.PropertyType, ReflectionUtils.GetSetMethod(current));
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
            IEnumerator<FieldInfo> enumerator2 = ReflectionUtils.GetFields(type).GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    FieldInfo fieldInfo = enumerator2.Current;
                    if ((!fieldInfo.IsInitOnly && !fieldInfo.IsStatic) && fieldInfo.IsPublic)
                    {
                        dictionary[this.MapClrMemberNameToJsonFieldName(fieldInfo.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(fieldInfo.FieldType, ReflectionUtils.GetSetMethod(fieldInfo));
                    }
                }
            }
            finally
            {
                if (enumerator2 == null)
                {
                }
                enumerator2.Dispose();
            }
            return dictionary;
        }

        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification="Need to support .NET 2")]
        protected virtual bool TrySerializeKnownTypes(object input, out object output)
        {
            bool flag = true;
            if (input is DateTime)
            {
                DateTime time = (DateTime) input;
                output = time.ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
                return flag;
            }
            if (input is DateTimeOffset)
            {
                DateTimeOffset offset = (DateTimeOffset) input;
                output = offset.ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
                return flag;
            }
            if (input is Guid)
            {
                output = ((Guid) input).ToString("D");
                return flag;
            }
            if (input is Uri)
            {
                output = input.ToString();
                return flag;
            }
            Enum p = input as Enum;
            if (p != 0)
            {
                output = this.SerializeEnum(p);
                return flag;
            }
            flag = false;
            output = null;
            return flag;
        }

        public virtual bool TrySerializeNonPrimitiveObject(object input, out object output)
        {
            return (this.TrySerializeKnownTypes(input, out output) || this.TrySerializeUnknownTypes(input, out output));
        }

        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification="Need to support .NET 2")]
        protected virtual bool TrySerializeUnknownTypes(object input, out object output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            output = null;
            Type type = input.GetType();
            if (type.FullName == null)
            {
                return false;
            }
            IDictionary<string, object> dictionary = new JsonObject();
            IEnumerator<KeyValuePair<string, ReflectionUtils.GetDelegate>> enumerator = this.GetCache[type].GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, ReflectionUtils.GetDelegate> current = enumerator.Current;
                    if (current.Value != null)
                    {
                        string key = this.MapClrMemberNameToJsonFieldName(current.Key);
                        dictionary.Add(key, current.Value(input));
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
            output = dictionary;
            return true;
        }
    }
}

