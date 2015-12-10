namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ColorPickerHDRConfig
    {
        [SerializeField]
        public float maxBrightness;
        [SerializeField]
        public float maxExposureValue;
        [SerializeField]
        public float minBrightness;
        [SerializeField]
        public float minExposureValue;
        private static readonly ColorPickerHDRConfig s_Temp = new ColorPickerHDRConfig(0f, 0f, 0f, 0f);

        internal ColorPickerHDRConfig(ColorPickerHDRConfig other)
        {
            this.minBrightness = other.minBrightness;
            this.maxBrightness = other.maxBrightness;
            this.minExposureValue = other.minExposureValue;
            this.maxExposureValue = other.maxExposureValue;
        }

        public ColorPickerHDRConfig(float minBrightness, float maxBrightness, float minExposureValue, float maxExposureValue)
        {
            this.minBrightness = minBrightness;
            this.maxBrightness = maxBrightness;
            this.minExposureValue = minExposureValue;
            this.maxExposureValue = maxExposureValue;
        }

        internal static ColorPickerHDRConfig Temp(float minBrightness, float maxBrightness, float minExposure, float maxExposure)
        {
            s_Temp.minBrightness = minBrightness;
            s_Temp.maxBrightness = maxBrightness;
            s_Temp.minExposureValue = minExposure;
            s_Temp.maxExposureValue = maxExposure;
            return s_Temp;
        }
    }
}

