namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;

    internal class AttributeHelper
    {
        [DebuggerHidden]
        internal static IEnumerable<T> CallMethodsWithAttribute<T>(Type attributeType, params object[] arguments)
        {
            return new <CallMethodsWithAttribute>c__Iterator3<T> { attributeType = attributeType, arguments = arguments, <$>attributeType = attributeType, <$>arguments = arguments, $PC = -2 };
        }

        private static MonoMenuItem[] ExtractContextMenu(Type klass)
        {
            Dictionary<string, MonoMenuItem> dictionary = new Dictionary<string, MonoMenuItem>();
            MethodInfo[] methods = klass.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < methods.GetLength(0); i++)
            {
                MethodInfo info = methods[i];
                foreach (ContextMenu menu in info.GetCustomAttributes(typeof(ContextMenu), false))
                {
                    MonoMenuItem item = !dictionary.ContainsKey(menu.menuItem) ? new MonoMenuItem() : dictionary[menu.menuItem];
                    item.menuItem = menu.menuItem;
                    item.type = klass;
                    item.execute = info.Name;
                    dictionary[menu.menuItem] = item;
                }
            }
            return dictionary.Values.ToArray<MonoMenuItem>();
        }

        private static MonoCreateAssetItem[] ExtractCreateAssetMenuItems(Assembly assembly)
        {
            List<MonoCreateAssetItem> list = new List<MonoCreateAssetItem>();
            foreach (Type type in AssemblyHelper.GetTypesFromAssembly(assembly))
            {
                CreateAssetMenuAttribute customAttribute = (CreateAssetMenuAttribute) Attribute.GetCustomAttribute(type, typeof(CreateAssetMenuAttribute));
                if (customAttribute != null)
                {
                    if (!type.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        object[] args = new object[] { type.FullName };
                        Debug.LogWarningFormat("CreateAssetMenu attribute on {0} will be ignored as {0} is not derived from ScriptableObject.", args);
                    }
                    else
                    {
                        string str = !string.IsNullOrEmpty(customAttribute.menuName) ? customAttribute.menuName : ObjectNames.NicifyVariableName(type.Name);
                        string path = !string.IsNullOrEmpty(customAttribute.fileName) ? customAttribute.fileName : ("New " + ObjectNames.NicifyVariableName(type.Name) + ".asset");
                        if (!Path.HasExtension(path))
                        {
                            path = path + ".asset";
                        }
                        MonoCreateAssetItem item = new MonoCreateAssetItem {
                            menuItem = str,
                            fileName = path,
                            order = customAttribute.order,
                            type = type
                        };
                        list.Add(item);
                    }
                }
            }
            return list.ToArray();
        }

        private static MonoGizmoMethod[] ExtractGizmos(Assembly assembly)
        {
            List<MonoGizmoMethod> list = new List<MonoGizmoMethod>();
            foreach (Type type in AssemblyHelper.GetTypesFromAssembly(assembly))
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < methods.GetLength(0); i++)
                {
                    MethodInfo info = methods[i];
                    foreach (DrawGizmo gizmo in info.GetCustomAttributes(typeof(DrawGizmo), false))
                    {
                        ParameterInfo[] parameters = info.GetParameters();
                        if (parameters.Length != 2)
                        {
                            Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but does not take parameters (ComponentType, GizmoType) so will be ignored.", info.DeclaringType.FullName, info.Name));
                        }
                        else
                        {
                            MonoGizmoMethod item = new MonoGizmoMethod();
                            if (gizmo.drawnType == null)
                            {
                                item.drawnType = parameters[0].ParameterType;
                            }
                            else if (parameters[0].ParameterType.IsAssignableFrom(gizmo.drawnType))
                            {
                                item.drawnType = gizmo.drawnType;
                            }
                            else
                            {
                                Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but the component type it applies to could not be determined.", info.DeclaringType.FullName, info.Name));
                                goto Label_018E;
                            }
                            if ((parameters[1].ParameterType != typeof(GizmoType)) && (parameters[1].ParameterType != typeof(int)))
                            {
                                Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but does not take a second parameter of type GizmoType so will be ignored.", info.DeclaringType.FullName, info.Name));
                            }
                            else
                            {
                                item.drawGizmo = info;
                                item.options = (int) gizmo.drawOptions;
                                list.Add(item);
                            }
                        Label_018E:;
                        }
                    }
                }
            }
            return list.ToArray();
        }

        private static MonoMenuItem[] ExtractMenuCommands(Assembly assembly)
        {
            bool @bool = EditorPrefs.GetBool("InternalMode", false);
            Dictionary<string, MonoMenuItem> dictionary = new Dictionary<string, MonoMenuItem>();
            foreach (Type type in AssemblyHelper.GetTypesFromAssembly(assembly))
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < methods.GetLength(0); i++)
                {
                    MethodInfo info = methods[i];
                    foreach (MenuItem item in info.GetCustomAttributes(typeof(MenuItem), false))
                    {
                        MonoMenuItem item2 = !dictionary.ContainsKey(item.menuItem) ? new MonoMenuItem() : dictionary[item.menuItem];
                        if (item.menuItem.StartsWith("internal:", StringComparison.Ordinal))
                        {
                            if (!@bool)
                            {
                                continue;
                            }
                            item2.menuItem = item.menuItem.Substring(9);
                        }
                        else
                        {
                            item2.menuItem = item.menuItem;
                        }
                        item2.type = type;
                        if (item.validate)
                        {
                            item2.validate = info.Name;
                        }
                        else
                        {
                            item2.execute = info.Name;
                            item2.index = i;
                            item2.priority = item.priority;
                        }
                        dictionary[item.menuItem] = item2;
                    }
                }
            }
            MonoMenuItem[] array = dictionary.Values.ToArray<MonoMenuItem>();
            Array.Sort(array, new CompareMenuIndex());
            return array;
        }

        internal static ArrayList FindEditorClassesWithAttribute(Type attrib)
        {
            ArrayList list = new ArrayList();
            IEnumerator<Type> enumerator = EditorAssemblies.loadedTypes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Type current = enumerator.Current;
                    if (current.GetCustomAttributes(attrib, false).Length != 0)
                    {
                        list.Add(current);
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
            return list;
        }

        internal static bool GameObjectContainsAttribute(GameObject go, Type attributeType)
        {
            foreach (Component component in go.GetComponents(typeof(Component)))
            {
                if ((component != null) && (component.GetType().GetCustomAttributes(attributeType, true).Length > 0))
                {
                    return true;
                }
            }
            return false;
        }

        private static string GetComponentMenuName(Type klass)
        {
            object[] customAttributes = klass.GetCustomAttributes(typeof(AddComponentMenu), false);
            if (customAttributes.Length > 0)
            {
                AddComponentMenu menu = (AddComponentMenu) customAttributes[0];
                return menu.componentMenu;
            }
            return null;
        }

        private static int GetComponentMenuOrdering(Type klass)
        {
            object[] customAttributes = klass.GetCustomAttributes(typeof(AddComponentMenu), false);
            if (customAttributes.Length > 0)
            {
                AddComponentMenu menu = (AddComponentMenu) customAttributes[0];
                return menu.componentOrder;
            }
            return 0;
        }

        internal static string GetHelpURLFromAttribute(Type objectType)
        {
            HelpURLAttribute customAttribute = (HelpURLAttribute) Attribute.GetCustomAttribute(objectType, typeof(HelpURLAttribute));
            return ((customAttribute == null) ? null : customAttribute.URL);
        }

        internal static object InvokeMemberIfAvailable(object target, string methodName, object[] args)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
            {
                return method.Invoke(target, args);
            }
            return null;
        }

        [CompilerGenerated]
        private sealed class <CallMethodsWithAttribute>c__Iterator3<T> : IDisposable, IEnumerator, IEnumerable, IEnumerable<T>, IEnumerator<T>
        {
            internal T $current;
            internal int $PC;
            internal object[] <$>arguments;
            internal Type <$>attributeType;
            internal Assembly[] <$s_291>__0;
            internal int <$s_292>__1;
            internal Type[] <$s_293>__3;
            internal int <$s_294>__4;
            internal MethodInfo[] <$s_295>__6;
            internal int <$s_296>__7;
            internal Assembly <assembly>__2;
            internal MethodInfo <method>__8;
            internal Type <type>__5;
            internal object[] arguments;
            internal Type attributeType;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<$s_291>__0 = EditorAssemblies.loadedAssemblies;
                        this.<$s_292>__1 = 0;
                        while (this.<$s_292>__1 < this.<$s_291>__0.Length)
                        {
                            this.<assembly>__2 = this.<$s_291>__0[this.<$s_292>__1];
                            this.<$s_293>__3 = this.<assembly>__2.GetTypes();
                            this.<$s_294>__4 = 0;
                            while (this.<$s_294>__4 < this.<$s_293>__3.Length)
                            {
                                this.<type>__5 = this.<$s_293>__3[this.<$s_294>__4];
                                this.<$s_295>__6 = this.<type>__5.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                                this.<$s_296>__7 = 0;
                                while (this.<$s_296>__7 < this.<$s_295>__6.Length)
                                {
                                    this.<method>__8 = this.<$s_295>__6[this.<$s_296>__7];
                                    if (this.<method>__8.GetCustomAttributes(this.attributeType, false).Length > 0)
                                    {
                                        this.$current = (T) this.<method>__8.Invoke(null, this.arguments);
                                        this.$PC = 1;
                                        return true;
                                    }
                                Label_00F0:
                                    this.<$s_296>__7++;
                                }
                                this.<$s_294>__4++;
                            }
                            this.<$s_292>__1++;
                        }
                        this.$PC = -1;
                        break;

                    case 1:
                        goto Label_00F0;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AttributeHelper.<CallMethodsWithAttribute>c__Iterator3<T> { attributeType = this.<$>attributeType, arguments = this.<$>arguments };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();
            }

            T IEnumerator<T>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        internal class CompareMenuIndex : IComparer
        {
            int IComparer.Compare(object xo, object yo)
            {
                AttributeHelper.MonoMenuItem item = (AttributeHelper.MonoMenuItem) xo;
                AttributeHelper.MonoMenuItem item2 = (AttributeHelper.MonoMenuItem) yo;
                if (item.priority != item2.priority)
                {
                    return item.priority.CompareTo(item2.priority);
                }
                return item.index.CompareTo(item2.index);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoCreateAssetItem
        {
            public string menuItem;
            public string fileName;
            public int order;
            public Type type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoGizmoMethod
        {
            public MethodInfo drawGizmo;
            public Type drawnType;
            public int options;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoMenuItem
        {
            public string menuItem;
            public string execute;
            public string validate;
            public int priority;
            public int index;
            public Type type;
        }
    }
}

