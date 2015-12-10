namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class MaterialPowerSliderDrawer : MaterialPropertyDrawer
    {
        private readonly float power;

        public MaterialPowerSliderDrawer(float power)
        {
            this.power = power;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (prop.type != MaterialProperty.PropType.Range)
            {
                return 40f;
            }
            return base.GetPropertyHeight(prop, label, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (prop.type != MaterialProperty.PropType.Range)
            {
                GUIContent content = EditorGUIUtility.TempContent("PowerSlider used on a non-range property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
                EditorGUI.LabelField(position, content, EditorStyles.helpBox);
            }
            else
            {
                MaterialEditor.DoPowerRangeProperty(position, prop, label, this.power);
            }
        }
    }
}

