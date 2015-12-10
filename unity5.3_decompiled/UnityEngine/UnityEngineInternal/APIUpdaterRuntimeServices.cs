namespace UnityEngineInternal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class APIUpdaterRuntimeServices
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<Type>> <>f__am$cache1;
        private static IList<Type> ComponentsFromUnityEngine;

        static APIUpdaterRuntimeServices()
        {
            Type type = typeof(Component);
            ComponentsFromUnityEngine = type.Assembly.GetTypes().Where<Type>(new Func<Type, bool>(type.IsAssignableFrom)).ToList<Type>();
        }

        [Obsolete("AddComponent(string) has been deprecated. Use GameObject.AddComponent<T>() / GameObject.AddComponent(Type) instead.\nAPI Updater could not automatically update the original call to AddComponent(string name), because it was unable to resolve the type specified in parameter 'name'.\nInstead, this call has been replaced with a call to APIUpdaterRuntimeServices.AddComponent() so you can try to test your game in the editor.\nIn order to be able to build the game, replace this call (APIUpdaterRuntimeServices.AddComponent()) with a call to GameObject.AddComponent<T>() / GameObject.AddComponent(Type).")]
        public static Component AddComponent(GameObject go, string sourceInfo, string name)
        {
            object[] args = new object[] { name };
            Debug.LogWarningFormat("Performing a potentially slow search for component {0}.", args);
            Type componentType = ResolveType(name, Assembly.GetCallingAssembly(), sourceInfo);
            return ((componentType != null) ? go.AddComponent(componentType) : null);
        }

        private static bool IsMarkedAsObsolete(Type t)
        {
            return t.GetCustomAttributes(typeof(ObsoleteAttribute), false).Any<object>();
        }

        private static Type ResolveType(string name, Assembly callingAssembly, string sourceInfo)
        {
            <ResolveType>c__AnonStorey7 storey = new <ResolveType>c__AnonStorey7 {
                name = name
            };
            Type type = ComponentsFromUnityEngine.FirstOrDefault<Type>(new Func<Type, bool>(storey.<>m__D));
            if (type != null)
            {
                object[] objArray1 = new object[] { storey.name, sourceInfo };
                Debug.LogWarningFormat("[{1}] Type '{0}' found in UnityEngine, consider replacing with go.AddComponent<{0}>();", objArray1);
                return type;
            }
            Type type2 = callingAssembly.GetType(storey.name);
            if (type2 != null)
            {
                object[] objArray2 = new object[] { type2.FullName, sourceInfo };
                Debug.LogWarningFormat("[{1}] Component type '{0}' found on caller assembly. Consider replacing the call method call with: AddComponent<{0}>()", objArray2);
                return type2;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = (Func<Assembly, IEnumerable<Type>>) (a => a.GetTypes());
            }
            type2 = AppDomain.CurrentDomain.GetAssemblies().SelectMany<Assembly, Type>(<>f__am$cache1).SingleOrDefault<Type>(new Func<Type, bool>(storey.<>m__F));
            if (type2 != null)
            {
                object[] objArray3 = new object[] { type2.FullName, type2.Assembly.Location, sourceInfo };
                Debug.LogWarningFormat("[{2}] Component type '{0}' found on assembly {1}. Consider replacing the call method with: AddComponent<{0}>()", objArray3);
                return type2;
            }
            object[] args = new object[] { storey.name, sourceInfo };
            Debug.LogErrorFormat("[{1}] Component Type '{0}' not found.", args);
            return null;
        }

        [CompilerGenerated]
        private sealed class <ResolveType>c__AnonStorey7
        {
            internal string name;

            internal bool <>m__D(Type t)
            {
                return (((t.Name == this.name) || (t.FullName == this.name)) && !APIUpdaterRuntimeServices.IsMarkedAsObsolete(t));
            }

            internal bool <>m__F(Type t)
            {
                return ((t.Name == this.name) && typeof(Component).IsAssignableFrom(t));
            }
        }
    }
}

