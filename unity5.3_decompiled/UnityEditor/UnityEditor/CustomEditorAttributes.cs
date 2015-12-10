namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class CustomEditorAttributes
    {
        private static readonly List<MonoEditorType> kSCustomEditors = new List<MonoEditorType>();
        private static readonly List<MonoEditorType> kSCustomMultiEditors = new List<MonoEditorType>();
        private static bool s_Initialized;

        internal static Type FindCustomEditorType(Object o, bool multiEdit)
        {
            return FindCustomEditorTypeByType(o.GetType(), multiEdit);
        }

        internal static Type FindCustomEditorTypeByType(Type type, bool multiEdit)
        {
            <FindCustomEditorTypeByType>c__AnonStorey32 storey = new <FindCustomEditorTypeByType>c__AnonStorey32 {
                type = type
            };
            if (!s_Initialized)
            {
                Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
                for (int i = loadedAssemblies.Length - 1; i >= 0; i--)
                {
                    Rebuild(loadedAssemblies[i]);
                }
                s_Initialized = true;
            }
            List<MonoEditorType> source = !multiEdit ? kSCustomEditors : kSCustomMultiEditors;
            <FindCustomEditorTypeByType>c__AnonStorey34 storey2 = new <FindCustomEditorTypeByType>c__AnonStorey34 {
                pass = 0
            };
            while (storey2.pass < 2)
            {
                <FindCustomEditorTypeByType>c__AnonStorey33 storey3 = new <FindCustomEditorTypeByType>c__AnonStorey33 {
                    <>f__ref$50 = storey,
                    <>f__ref$52 = storey2,
                    inspected = storey.type
                };
                while (storey3.inspected != null)
                {
                    MonoEditorType type2 = source.FirstOrDefault<MonoEditorType>(new Func<MonoEditorType, bool>(storey3.<>m__4A));
                    if (type2 != null)
                    {
                        return type2.m_InspectorType;
                    }
                    storey3.inspected = storey3.inspected.BaseType;
                }
                storey2.pass++;
            }
            return null;
        }

        internal static void Rebuild(Assembly assembly)
        {
            foreach (Type type in AssemblyHelper.GetTypesFromAssembly(assembly))
            {
                foreach (CustomEditor editor in type.GetCustomAttributes(typeof(CustomEditor), false))
                {
                    MonoEditorType item = new MonoEditorType();
                    if (editor.m_InspectedType == null)
                    {
                        Debug.Log("Can't load custom inspector " + type.Name + " because the inspected type is null.");
                    }
                    else if (!type.IsSubclassOf(typeof(Editor)))
                    {
                        if (((type.FullName != "TweakMode") || !type.IsEnum) || (editor.m_InspectedType.FullName != "BloomAndFlares"))
                        {
                            Debug.LogWarning(type.Name + " uses the CustomEditor attribute but does not inherit from Editor.\nYou must inherit from Editor. See the Editor class script documentation.");
                        }
                    }
                    else
                    {
                        item.m_InspectedType = editor.m_InspectedType;
                        item.m_InspectorType = type;
                        item.m_EditorForChildClasses = editor.m_EditorForChildClasses;
                        item.m_IsFallback = editor.isFallback;
                        kSCustomEditors.Add(item);
                        if (type.GetCustomAttributes(typeof(CanEditMultipleObjects), false).Length > 0)
                        {
                            kSCustomMultiEditors.Add(item);
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FindCustomEditorTypeByType>c__AnonStorey32
        {
            internal Type type;
        }

        [CompilerGenerated]
        private sealed class <FindCustomEditorTypeByType>c__AnonStorey33
        {
            internal CustomEditorAttributes.<FindCustomEditorTypeByType>c__AnonStorey32 <>f__ref$50;
            internal CustomEditorAttributes.<FindCustomEditorTypeByType>c__AnonStorey34 <>f__ref$52;
            internal Type inspected;

            internal bool <>m__4A(CustomEditorAttributes.MonoEditorType x)
            {
                if ((this.<>f__ref$50.type != this.inspected) && !x.m_EditorForChildClasses)
                {
                    return false;
                }
                if ((this.<>f__ref$52.pass == 1) != x.m_IsFallback)
                {
                    return false;
                }
                return (this.inspected == x.m_InspectedType);
            }
        }

        [CompilerGenerated]
        private sealed class <FindCustomEditorTypeByType>c__AnonStorey34
        {
            internal int pass;
        }

        private class MonoEditorType
        {
            public bool m_EditorForChildClasses;
            public Type m_InspectedType;
            public Type m_InspectorType;
            public bool m_IsFallback;
        }
    }
}

