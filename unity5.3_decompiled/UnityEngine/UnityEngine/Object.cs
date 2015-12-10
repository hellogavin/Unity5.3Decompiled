namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class Object
    {
        private int m_InstanceID;
        private IntPtr m_CachedPtr;
        private string m_UnityRuntimeErrorString;
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Object Internal_CloneSingle(Object data);
        private static Object Internal_InstantiateSingle(Object data, Vector3 pos, Quaternion rot)
        {
            return INTERNAL_CALL_Internal_InstantiateSingle(data, ref pos, ref rot);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Object INTERNAL_CALL_Internal_InstantiateSingle(Object data, ref Vector3 pos, ref Quaternion rot);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Destroy(Object obj, [DefaultValue("0.0F")] float t);
        [ExcludeFromDocs]
        public static void Destroy(Object obj)
        {
            float t = 0f;
            Destroy(obj, t);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DestroyImmediate(Object obj, [DefaultValue("false")] bool allowDestroyingAssets);
        [ExcludeFromDocs]
        public static void DestroyImmediate(Object obj)
        {
            bool allowDestroyingAssets = false;
            DestroyImmediate(obj, allowDestroyingAssets);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
        public static extern Object[] FindObjectsOfType(Type type);
        public string name { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DontDestroyOnLoad(Object target);
        public HideFlags hideFlags { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DestroyObject(Object obj, [DefaultValue("0.0F")] float t);
        [ExcludeFromDocs]
        public static void DestroyObject(Object obj)
        {
            float t = 0f;
            DestroyObject(obj, t);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("use Object.FindObjectsOfType instead.")]
        public static extern Object[] FindSceneObjectsOfType(Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
        public static extern Object[] FindObjectsOfTypeIncludingAssets(Type type);
        [Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
        public static Object[] FindObjectsOfTypeAll(Type type)
        {
            return Resources.FindObjectsOfTypeAll(type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public override extern string ToString();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool DoesObjectWithInstanceIDExist(int instanceID);
        public override bool Equals(object o)
        {
            return CompareBaseObjects(this, o as Object);
        }

        public override int GetHashCode()
        {
            return this.GetInstanceID();
        }

        private static bool CompareBaseObjects(Object lhs, Object rhs)
        {
            bool flag = lhs == null;
            bool flag2 = rhs == null;
            if (flag2 && flag)
            {
                return true;
            }
            if (flag2)
            {
                return !IsNativeObjectAlive(lhs);
            }
            if (flag)
            {
                return !IsNativeObjectAlive(rhs);
            }
            return (lhs.m_InstanceID == rhs.m_InstanceID);
        }

        private static bool IsNativeObjectAlive(Object o)
        {
            return ((o.GetCachedPtr() != IntPtr.Zero) || ((!(o is MonoBehaviour) && !(o is ScriptableObject)) && DoesObjectWithInstanceIDExist(o.GetInstanceID())));
        }

        public int GetInstanceID()
        {
            return this.m_InstanceID;
        }

        private IntPtr GetCachedPtr()
        {
            return this.m_CachedPtr;
        }

        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static Object Instantiate(Object original, Vector3 position, Quaternion rotation)
        {
            CheckNullArgument(original, "The thing you want to instantiate is null.");
            return Internal_InstantiateSingle(original, position, rotation);
        }

        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static Object Instantiate(Object original)
        {
            CheckNullArgument(original, "The thing you want to instantiate is null.");
            return Internal_CloneSingle(original);
        }

        public static T Instantiate<T>(T original) where T: Object
        {
            CheckNullArgument(original, "The thing you want to instantiate is null.");
            return (T) Internal_CloneSingle(original);
        }

        private static void CheckNullArgument(object arg, string message)
        {
            if (arg == null)
            {
                throw new ArgumentException(message);
            }
        }

        public static T[] FindObjectsOfType<T>() where T: Object
        {
            return Resources.ConvertObjects<T>(FindObjectsOfType(typeof(T)));
        }

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static Object FindObjectOfType(Type type)
        {
            Object[] objArray = FindObjectsOfType(type);
            if (objArray.Length > 0)
            {
                return objArray[0];
            }
            return null;
        }

        public static T FindObjectOfType<T>() where T: Object
        {
            return (T) FindObjectOfType(typeof(T));
        }

        public static implicit operator bool(Object exists)
        {
            return !CompareBaseObjects(exists, null);
        }

        public static bool operator ==(Object x, Object y)
        {
            return CompareBaseObjects(x, y);
        }

        public static bool operator !=(Object x, Object y)
        {
            return !CompareBaseObjects(x, y);
        }
    }
}

