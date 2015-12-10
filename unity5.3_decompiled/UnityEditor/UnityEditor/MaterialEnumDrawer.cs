namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class MaterialEnumDrawer : MaterialPropertyDrawer
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<Type>> <>f__am$cache2;
        private readonly string[] names;
        private readonly int[] values;

        public MaterialEnumDrawer(string enumName)
        {
            <MaterialEnumDrawer>c__AnonStorey8F storeyf = new <MaterialEnumDrawer>c__AnonStorey8F {
                enumName = enumName
            };
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<Assembly, IEnumerable<Type>>(MaterialEnumDrawer.<MaterialEnumDrawer>m__175);
            }
            Type[] source = AppDomain.CurrentDomain.GetAssemblies().SelectMany<Assembly, Type>(<>f__am$cache2).ToArray<Type>();
            try
            {
                Type enumType = source.FirstOrDefault<Type>(new Func<Type, bool>(storeyf.<>m__176));
                this.names = Enum.GetNames(enumType);
                Array values = Enum.GetValues(enumType);
                this.values = new int[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    this.values[i] = (int) values.GetValue(i);
                }
            }
            catch (Exception)
            {
                object[] args = new object[] { storeyf.enumName };
                Debug.LogWarningFormat("Failed to create MaterialEnum, enum {0} not found", args);
                throw;
            }
        }

        public MaterialEnumDrawer(string n1, float v1) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1 };
            float[] singleArray1 = new float[] { v1 };
        }

        public MaterialEnumDrawer(string[] names, float[] vals)
        {
            this.names = names;
            this.values = new int[vals.Length];
            for (int i = 0; i < vals.Length; i++)
            {
                this.values[i] = (int) vals[i];
            }
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2 };
            float[] singleArray1 = new float[] { v1, v2 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3 };
            float[] singleArray1 = new float[] { v1, v2, v3 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3, n4 };
            float[] singleArray1 = new float[] { v1, v2, v3, v4 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3, n4, n5 };
            float[] singleArray1 = new float[] { v1, v2, v3, v4, v5 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3, n4, n5, n6 };
            float[] singleArray1 = new float[] { v1, v2, v3, v4, v5, v6 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6, string n7, float v7) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3, n4, n5, n6, n7 };
            float[] singleArray1 = new float[] { v1, v2, v3, v4, v5, v6, v7 };
        }

        [CompilerGenerated]
        private static IEnumerable<Type> <MaterialEnumDrawer>m__175(Assembly x)
        {
            return AssemblyHelper.GetTypesFromAssembly(x);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if ((prop.type != MaterialProperty.PropType.Float) && (prop.type != MaterialProperty.PropType.Range))
            {
                return 40f;
            }
            return base.GetPropertyHeight(prop, label, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            if ((prop.type != MaterialProperty.PropType.Float) && (prop.type != MaterialProperty.PropType.Range))
            {
                GUIContent content = EditorGUIUtility.TempContent("Enum used on a non-float property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
                EditorGUI.LabelField(position, content, EditorStyles.helpBox);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = prop.hasMixedValue;
                int floatValue = (int) prop.floatValue;
                floatValue = EditorGUI.IntPopup(position, label, floatValue, this.names, this.values);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    prop.floatValue = floatValue;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <MaterialEnumDrawer>c__AnonStorey8F
        {
            internal string enumName;

            internal bool <>m__176(Type x)
            {
                return (x.IsSubclassOf(typeof(Enum)) && ((x.Name == this.enumName) || (x.FullName == this.enumName)));
            }
        }
    }
}

