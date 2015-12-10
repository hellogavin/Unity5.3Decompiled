namespace UnityEditor
{
    using System;
    using System.Globalization;
    using UnityEngine;

    internal class PrefColor : IPrefType
    {
        private UnityEngine.Color m_color;
        private UnityEngine.Color m_DefaultColor;
        private string m_name;

        public PrefColor()
        {
        }

        public PrefColor(string name, float defaultRed, float defaultGreen, float defaultBlue, float defaultAlpha)
        {
            this.m_name = name;
            this.m_color = this.m_DefaultColor = new UnityEngine.Color(defaultRed, defaultGreen, defaultBlue, defaultAlpha);
            PrefColor color = Settings.Get<PrefColor>(name, this);
            this.m_name = color.Name;
            this.m_color = color.Color;
        }

        public void FromUniqueString(string s)
        {
            char[] separator = new char[] { ';' };
            string[] strArray = s.Split(separator);
            if (strArray.Length != 5)
            {
                Debug.LogError("Parsing PrefColor failed");
            }
            else
            {
                float num;
                float num2;
                float num3;
                float num4;
                this.m_name = strArray[0];
                strArray[1] = strArray[1].Replace(',', '.');
                strArray[2] = strArray[2].Replace(',', '.');
                strArray[3] = strArray[3].Replace(',', '.');
                strArray[4] = strArray[4].Replace(',', '.');
                bool flag = float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num) & float.TryParse(strArray[2], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num2);
                flag &= float.TryParse(strArray[3], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num3);
                if (flag & float.TryParse(strArray[4], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num4))
                {
                    this.m_color = new UnityEngine.Color(num, num2, num3, num4);
                }
                else
                {
                    Debug.LogError("Parsing PrefColor failed");
                }
            }
        }

        public static implicit operator UnityEngine.Color(PrefColor pcolor)
        {
            return pcolor.Color;
        }

        internal void ResetToDefault()
        {
            this.m_color = this.m_DefaultColor;
        }

        public string ToUniqueString()
        {
            object[] args = new object[] { this.m_name, this.Color.r, this.Color.g, this.Color.b, this.Color.a };
            return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4}", args);
        }

        public UnityEngine.Color Color
        {
            get
            {
                return this.m_color;
            }
            set
            {
                this.m_color = value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }
    }
}

