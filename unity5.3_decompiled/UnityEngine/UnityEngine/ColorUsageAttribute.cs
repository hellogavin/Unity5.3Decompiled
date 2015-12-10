namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
    public sealed class ColorUsageAttribute : PropertyAttribute
    {
        public readonly bool hdr;
        public readonly float maxBrightness;
        public readonly float maxExposureValue;
        public readonly float minBrightness;
        public readonly float minExposureValue;
        public readonly bool showAlpha;

        public ColorUsageAttribute(bool showAlpha)
        {
            this.showAlpha = true;
            this.maxBrightness = 8f;
            this.minExposureValue = 0.125f;
            this.maxExposureValue = 3f;
            this.showAlpha = showAlpha;
        }

        public ColorUsageAttribute(bool showAlpha, bool hdr, float minBrightness, float maxBrightness, float minExposureValue, float maxExposureValue)
        {
            this.showAlpha = true;
            this.maxBrightness = 8f;
            this.minExposureValue = 0.125f;
            this.maxExposureValue = 3f;
            this.showAlpha = showAlpha;
            this.hdr = hdr;
            this.minBrightness = minBrightness;
            this.maxBrightness = maxBrightness;
            this.minExposureValue = minExposureValue;
            this.maxExposureValue = maxExposureValue;
        }
    }
}

