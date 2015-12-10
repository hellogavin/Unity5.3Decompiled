namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class MaterialKeywordEnumDrawer : MaterialPropertyDrawer
    {
        private readonly string[] keywords;

        public MaterialKeywordEnumDrawer(string kw1) : this(textArray1)
        {
            string[] textArray1 = new string[] { kw1 };
        }

        public MaterialKeywordEnumDrawer(params string[] keywords)
        {
            this.keywords = keywords;
        }

        public MaterialKeywordEnumDrawer(string kw1, string kw2) : this(textArray1)
        {
            string[] textArray1 = new string[] { kw1, kw2 };
        }

        public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3) : this(textArray1)
        {
            string[] textArray1 = new string[] { kw1, kw2, kw3 };
        }

        public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4) : this(textArray1)
        {
            string[] textArray1 = new string[] { kw1, kw2, kw3, kw4 };
        }

        public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5) : this(textArray1)
        {
            string[] textArray1 = new string[] { kw1, kw2, kw3, kw4, kw5 };
        }

        public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6) : this(textArray1)
        {
            string[] textArray1 = new string[] { kw1, kw2, kw3, kw4, kw5, kw6 };
        }

        public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7) : this(textArray1)
        {
            string[] textArray1 = new string[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7 };
        }

        public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8) : this(textArray1)
        {
            string[] textArray1 = new string[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7, kw8 };
        }

        public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8, string kw9) : this(textArray1)
        {
            string[] textArray1 = new string[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7, kw8, kw9 };
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);
            if (IsPropertyTypeSuitable(prop) && !prop.hasMixedValue)
            {
                this.SetKeyword(prop, (int) prop.floatValue);
            }
        }

        private static string GetKeywordName(string propName, string name)
        {
            return (propName + "_" + name).Replace(' ', '_').ToUpperInvariant();
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!IsPropertyTypeSuitable(prop))
            {
                return 40f;
            }
            return base.GetPropertyHeight(prop, label, editor);
        }

        private static bool IsPropertyTypeSuitable(MaterialProperty prop)
        {
            return ((prop.type == MaterialProperty.PropType.Float) || (prop.type == MaterialProperty.PropType.Range));
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!IsPropertyTypeSuitable(prop))
            {
                GUIContent content = EditorGUIUtility.TempContent("KeywordEnum used on a non-float property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
                EditorGUI.LabelField(position, content, EditorStyles.helpBox);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = prop.hasMixedValue;
                int floatValue = (int) prop.floatValue;
                floatValue = EditorGUI.Popup(position, label, floatValue, this.keywords);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    prop.floatValue = floatValue;
                    this.SetKeyword(prop, floatValue);
                }
            }
        }

        private void SetKeyword(MaterialProperty prop, int index)
        {
            for (int i = 0; i < this.keywords.Length; i++)
            {
                string keywordName = GetKeywordName(prop.name, this.keywords[i]);
                foreach (Material material in prop.targets)
                {
                    if (index == i)
                    {
                        material.EnableKeyword(keywordName);
                    }
                    else
                    {
                        material.DisableKeyword(keywordName);
                    }
                }
            }
        }
    }
}

