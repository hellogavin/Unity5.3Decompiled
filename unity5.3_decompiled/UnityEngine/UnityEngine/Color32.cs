namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), IL2CPPStructAlignment(Align=4), UsedByNativeCode]
    public struct Color32
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
        public Color32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.r, this.g, this.b, this.a };
            return UnityString.Format("RGBA({0}, {1}, {2}, {3})", args);
        }

        public string ToString(string format)
        {
            object[] args = new object[] { this.r.ToString(format), this.g.ToString(format), this.b.ToString(format), this.a.ToString(format) };
            return UnityString.Format("RGBA({0}, {1}, {2}, {3})", args);
        }

        public static Color32 Lerp(Color32 a, Color32 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Color32((byte) (a.r + ((b.r - a.r) * t)), (byte) (a.g + ((b.g - a.g) * t)), (byte) (a.b + ((b.b - a.b) * t)), (byte) (a.a + ((b.a - a.a) * t)));
        }

        public static Color32 LerpUnclamped(Color32 a, Color32 b, float t)
        {
            return new Color32((byte) (a.r + ((b.r - a.r) * t)), (byte) (a.g + ((b.g - a.g) * t)), (byte) (a.b + ((b.b - a.b) * t)), (byte) (a.a + ((b.a - a.a) * t)));
        }

        public static implicit operator Color32(Color c)
        {
            return new Color32((byte) (Mathf.Clamp01(c.r) * 255f), (byte) (Mathf.Clamp01(c.g) * 255f), (byte) (Mathf.Clamp01(c.b) * 255f), (byte) (Mathf.Clamp01(c.a) * 255f));
        }

        public static implicit operator Color(Color32 c)
        {
            return new Color(((float) c.r) / 255f, ((float) c.g) / 255f, ((float) c.b) / 255f, ((float) c.a) / 255f);
        }
    }
}

