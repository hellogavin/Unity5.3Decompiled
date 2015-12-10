namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class MaterialToggleDrawer : MaterialPropertyDrawer
    {
        private readonly string keyword;

        public MaterialToggleDrawer()
        {
        }

        public MaterialToggleDrawer(string keyword)
        {
            this.keyword = keyword;
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);
            if (IsPropertyTypeSuitable(prop) && !prop.hasMixedValue)
            {
                this.SetKeyword(prop, Math.Abs(prop.floatValue) > 0.001f);
            }
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
                GUIContent content = EditorGUIUtility.TempContent("Toggle used on a non-float property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
                EditorGUI.LabelField(position, content, EditorStyles.helpBox);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                bool flag = Math.Abs(prop.floatValue) > 0.001f;
                EditorGUI.showMixedValue = prop.hasMixedValue;
                flag = EditorGUI.Toggle(position, label, flag);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    prop.floatValue = !flag ? 0f : 1f;
                    this.SetKeyword(prop, flag);
                }
            }
        }

        private void SetKeyword(MaterialProperty prop, bool on)
        {
            string keyword = !string.IsNullOrEmpty(this.keyword) ? this.keyword : (prop.name.ToUpperInvariant() + "_ON");
            foreach (Material material in prop.targets)
            {
                if (on)
                {
                    material.EnableKeyword(keyword);
                }
                else
                {
                    material.DisableKeyword(keyword);
                }
            }
        }
    }
}

