namespace UnityEditor
{
    using System;
    using System.Globalization;
    using UnityEngine;

    internal class MaterialHeaderDecorator : MaterialPropertyDrawer
    {
        private readonly string header;

        public MaterialHeaderDecorator(float headerAsNumber)
        {
            this.header = headerAsNumber.ToString(CultureInfo.InvariantCulture);
        }

        public MaterialHeaderDecorator(string header)
        {
            this.header = header;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 24f;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            position.y += 8f;
            position = EditorGUI.IndentedRect(position);
            GUI.Label(position, this.header, EditorStyles.boldLabel);
        }
    }
}

