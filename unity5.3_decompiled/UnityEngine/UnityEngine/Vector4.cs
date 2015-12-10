namespace UnityEngine
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Vector4
    {
        public const float kEpsilon = 1E-05f;
        public float x;
        public float y;
        public float z;
        public float w;
        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 0f;
        }

        public Vector4(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0f;
            this.w = 0f;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;

                    case 1:
                        return this.y;

                    case 2:
                        return this.z;

                    case 3:
                        return this.w;
                }
                throw new IndexOutOfRangeException("Invalid Vector4 index!");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;

                    case 1:
                        this.y = value;
                        break;

                    case 2:
                        this.z = value;
                        break;

                    case 3:
                        this.w = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4 index!");
                }
            }
        }
        public void Set(float new_x, float new_y, float new_z, float new_w)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
            this.w = new_w;
        }

        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector4(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t), a.w + ((b.w - a.w) * t));
        }

        public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t)
        {
            return new Vector4(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t), a.w + ((b.w - a.w) * t));
        }

        public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta)
        {
            Vector4 vector = target - current;
            float magnitude = vector.magnitude;
            if ((magnitude > maxDistanceDelta) && (magnitude != 0f))
            {
                return (current + ((Vector4) ((vector / magnitude) * maxDistanceDelta)));
            }
            return target;
        }

        public static Vector4 Scale(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        public void Scale(Vector4 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
            this.w *= scale.w;
        }

        public override int GetHashCode()
        {
            return (((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2)) ^ (this.w.GetHashCode() >> 1));
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector4))
            {
                return false;
            }
            Vector4 vector = (Vector4) other;
            return (((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z)) && this.w.Equals(vector.w));
        }

        public static Vector4 Normalize(Vector4 a)
        {
            float num = Magnitude(a);
            if (num > 1E-05f)
            {
                return (Vector4) (a / num);
            }
            return zero;
        }

        public void Normalize()
        {
            float num = Magnitude(this);
            if (num > 1E-05f)
            {
                this = (Vector4) (this / num);
            }
            else
            {
                this = zero;
            }
        }

        public Vector4 normalized
        {
            get
            {
                return Normalize(this);
            }
        }
        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y, this.z, this.w };
            return UnityString.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", args);
        }

        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format), this.z.ToString(format), this.w.ToString(format) };
            return UnityString.Format("({0}, {1}, {2}, {3})", args);
        }

        public static float Dot(Vector4 a, Vector4 b)
        {
            return ((((a.x * b.x) + (a.y * b.y)) + (a.z * b.z)) + (a.w * b.w));
        }

        public static Vector4 Project(Vector4 a, Vector4 b)
        {
            return (Vector4) ((b * Dot(a, b)) / Dot(b, b));
        }

        public static float Distance(Vector4 a, Vector4 b)
        {
            return Magnitude(a - b);
        }

        public static float Magnitude(Vector4 a)
        {
            return Mathf.Sqrt(Dot(a, a));
        }

        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(Dot(this, this));
            }
        }
        public static float SqrMagnitude(Vector4 a)
        {
            return Dot(a, a);
        }

        public float SqrMagnitude()
        {
            return Dot(this, this);
        }

        public float sqrMagnitude
        {
            get
            {
                return Dot(this, this);
            }
        }
        public static Vector4 Min(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z), Mathf.Min(lhs.w, rhs.w));
        }

        public static Vector4 Max(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z), Mathf.Max(lhs.w, rhs.w));
        }

        public static Vector4 zero
        {
            get
            {
                return new Vector4(0f, 0f, 0f, 0f);
            }
        }
        public static Vector4 one
        {
            get
            {
                return new Vector4(1f, 1f, 1f, 1f);
            }
        }
        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Vector4 operator -(Vector4 a)
        {
            return new Vector4(-a.x, -a.y, -a.z, -a.w);
        }

        public static Vector4 operator *(Vector4 a, float d)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4 operator *(float d, Vector4 a)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4 operator /(Vector4 a, float d)
        {
            return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
        }

        public static bool operator ==(Vector4 lhs, Vector4 rhs)
        {
            return (SqrMagnitude(lhs - rhs) < 9.999999E-11f);
        }

        public static bool operator !=(Vector4 lhs, Vector4 rhs)
        {
            return (SqrMagnitude(lhs - rhs) >= 9.999999E-11f);
        }

        public static implicit operator Vector4(Vector3 v)
        {
            return new Vector4(v.x, v.y, v.z, 0f);
        }

        public static implicit operator Vector3(Vector4 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static implicit operator Vector4(Vector2 v)
        {
            return new Vector4(v.x, v.y, 0f, 0f);
        }

        public static implicit operator Vector2(Vector4 v)
        {
            return new Vector2(v.x, v.y);
        }
    }
}

