namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class TickStyle
    {
        public bool centerLabel;
        public Color color = new Color(0f, 0f, 0f, 0.2f);
        public int distFull = 80;
        public int distLabel = 50;
        public int distMin = 10;
        public Color labelColor = new Color(0f, 0f, 0f, 1f);
        public bool stubs;
        public string unit = string.Empty;

        public TickStyle()
        {
            if (EditorGUIUtility.isProSkin)
            {
                this.color = new Color(0.45f, 0.45f, 0.45f, 0.2f);
                this.labelColor = new Color(0.8f, 0.8f, 0.8f, 0.32f);
            }
            else
            {
                this.color = new Color(0f, 0f, 0f, 0.2f);
                this.labelColor = new Color(0f, 0f, 0f, 0.32f);
            }
        }
    }
}

