namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MathUtils
    {
        private const int kMaxDecimals = 15;

        internal static float ClampToFloat(double value)
        {
            if (double.IsPositiveInfinity(value))
            {
                return float.PositiveInfinity;
            }
            if (double.IsNegativeInfinity(value))
            {
                return float.NegativeInfinity;
            }
            if (value < -3.4028234663852886E+38)
            {
                return float.MinValue;
            }
            if (value > 3.4028234663852886E+38)
            {
                return float.MaxValue;
            }
            return (float) value;
        }

        internal static int ClampToInt(long value)
        {
            if (value < -2147483648L)
            {
                return -2147483648;
            }
            if (value > 0x7fffffffL)
            {
                return 0x7fffffff;
            }
            return (int) value;
        }

        public static bool ClosestPtRaySphere(Ray ray, Vector3 sphereOrigin, float sphereRadius, ref float t, ref Vector3 q)
        {
            Vector3 lhs = ray.origin - sphereOrigin;
            float num = Vector3.Dot(lhs, ray.direction);
            float num2 = Vector3.Dot(lhs, lhs) - (sphereRadius * sphereRadius);
            if ((num2 > 0f) && (num > 0f))
            {
                t = 0f;
                q = ray.origin;
                return true;
            }
            float f = (num * num) - num2;
            if (f < 0f)
            {
                f = 0f;
            }
            t = -num - Mathf.Sqrt(f);
            if (t < 0f)
            {
                t = 0f;
            }
            q = ray.origin + (t * ray.direction);
            return true;
        }

        public static Vector3 ClosestPtSegmentRay(Vector3 p1, Vector3 q1, Ray ray, out float squaredDist, out float s, out Vector3 closestRay)
        {
            Vector3 origin = ray.origin;
            Vector3 point = ray.GetPoint(10000f);
            Vector3 lhs = q1 - p1;
            Vector3 vector4 = point - origin;
            Vector3 rhs = p1 - origin;
            float num = Vector3.Dot(lhs, lhs);
            float num2 = Vector3.Dot(vector4, vector4);
            float num3 = Vector3.Dot(vector4, rhs);
            float num4 = 0f;
            if ((num <= Mathf.Epsilon) && (num2 <= Mathf.Epsilon))
            {
                squaredDist = Vector3.Dot(p1 - origin, p1 - origin);
                s = 0f;
                closestRay = origin;
                return p1;
            }
            if (num <= Mathf.Epsilon)
            {
                s = 0f;
                num4 = num3 / num2;
                num4 = Mathf.Clamp(num4, 0f, 1f);
            }
            else
            {
                float num5 = Vector3.Dot(lhs, rhs);
                if (num2 <= Mathf.Epsilon)
                {
                    num4 = 0f;
                    s = Mathf.Clamp((float) (-num5 / num), (float) 0f, (float) 1f);
                }
                else
                {
                    float num6 = Vector3.Dot(lhs, vector4);
                    float num7 = (num * num2) - (num6 * num6);
                    if (num7 != 0f)
                    {
                        s = Mathf.Clamp((float) (((num6 * num3) - (num5 * num2)) / num7), (float) 0f, (float) 1f);
                    }
                    else
                    {
                        s = 0f;
                    }
                    num4 = ((num6 * s) + num3) / num2;
                    if (num4 < 0f)
                    {
                        num4 = 0f;
                        s = Mathf.Clamp((float) (-num5 / num), (float) 0f, (float) 1f);
                    }
                    else if (num4 > 1f)
                    {
                        num4 = 1f;
                        s = Mathf.Clamp((float) ((num6 - num5) / num), (float) 0f, (float) 1f);
                    }
                }
            }
            Vector3 vector6 = p1 + (lhs * s);
            Vector3 vector7 = origin + ((Vector3) (vector4 * num4));
            squaredDist = Vector3.Dot(vector6 - vector7, vector6 - vector7);
            closestRay = vector7;
            return vector6;
        }

        internal static double DiscardLeastSignificantDecimal(double v)
        {
            int digits = Math.Max(0, (int) (5.0 - Math.Log10(Math.Abs(v))));
            try
            {
                return Math.Round(v, digits);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0.0;
            }
        }

        internal static float DiscardLeastSignificantDecimal(float v)
        {
            int digits = Mathf.Clamp((int) (5f - Mathf.Log10(Mathf.Abs(v))), 0, 15);
            return (float) Math.Round((double) v, digits, MidpointRounding.AwayFromZero);
        }

        public static float Ease(float t, float k1, float k2)
        {
            float num2;
            float num = ((((k1 * 2f) / 3.141593f) + k2) - k1) + (((1f - k2) * 2f) / 3.141593f);
            if (t < k1)
            {
                num2 = (k1 * 0.6366197f) * (Mathf.Sin((((t / k1) * 3.141593f) / 2f) - 1.570796f) + 1f);
            }
            else if (t < k2)
            {
                num2 = (((2f * k1) / 3.141593f) + t) - k1;
            }
            else
            {
                num2 = ((((2f * k1) / 3.141593f) + k2) - k1) + (((1f - k2) * 0.6366197f) * Mathf.Sin((((t - k2) / (1f - k2)) * 3.141593f) / 2f));
            }
            return (num2 / num);
        }

        internal static float GetClosestPowerOfTen(float positiveNumber)
        {
            if (positiveNumber <= 0f)
            {
                return 1f;
            }
            return Mathf.Pow(10f, (float) Mathf.RoundToInt(Mathf.Log10(positiveNumber)));
        }

        internal static int GetNumberOfDecimalsForMinimumDifference(double minDifference)
        {
            return (int) Math.Max(0.0, -Math.Floor(Math.Log10(Math.Abs(minDifference))));
        }

        internal static int GetNumberOfDecimalsForMinimumDifference(float minDifference)
        {
            return Mathf.Clamp(-Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(minDifference))), 0, 15);
        }

        public static Quaternion GetQuatConjugate(Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, q.w);
        }

        public static Quaternion GetQuatExp(Quaternion q)
        {
            Quaternion quaternion = q;
            float f = Mathf.Sqrt(((q.x * q.x) + (q.y * q.y)) + (q.z * q.z));
            float num2 = Mathf.Sin(f);
            quaternion.w = Mathf.Cos(f);
            if (Mathf.Abs(num2) > 0.0001)
            {
                float num3 = num2 / f;
                quaternion.x = num3 * q.x;
                quaternion.y = num3 * q.y;
                quaternion.z = num3 * q.z;
            }
            return quaternion;
        }

        public static float GetQuatLength(Quaternion q)
        {
            return Mathf.Sqrt((((q.x * q.x) + (q.y * q.y)) + (q.z * q.z)) + (q.w * q.w));
        }

        public static Quaternion GetQuatLog(Quaternion q)
        {
            Quaternion quaternion = q;
            quaternion.w = 0f;
            if (Mathf.Abs(q.w) < 1f)
            {
                float f = Mathf.Acos(q.w);
                float num2 = Mathf.Sin(f);
                if (Mathf.Abs(num2) > 0.0001)
                {
                    float num3 = f / num2;
                    quaternion.x = q.x * num3;
                    quaternion.y = q.y * num3;
                    quaternion.z = q.z * num3;
                }
            }
            return quaternion;
        }

        public static Quaternion GetQuatSquad(float t, Quaternion q0, Quaternion q1, Quaternion a0, Quaternion a1)
        {
            float num = (2f * t) * (1f - t);
            Quaternion p = Slerp(q0, q1, t);
            Quaternion q = Slerp(a0, a1, t);
            Quaternion quaternion3 = Slerp(p, q, num);
            float num2 = Mathf.Sqrt((((quaternion3.x * quaternion3.x) + (quaternion3.y * quaternion3.y)) + (quaternion3.z * quaternion3.z)) + (quaternion3.w * quaternion3.w));
            quaternion3.x /= num2;
            quaternion3.y /= num2;
            quaternion3.z /= num2;
            quaternion3.w /= num2;
            return quaternion3;
        }

        public static Quaternion GetSquadIntermediate(Quaternion q0, Quaternion q1, Quaternion q2)
        {
            Quaternion quatConjugate = GetQuatConjugate(q1);
            Quaternion quatLog = GetQuatLog(quatConjugate * q0);
            Quaternion quaternion3 = GetQuatLog(quatConjugate * q2);
            Quaternion q = new Quaternion(-0.25f * (quatLog.x + quaternion3.x), -0.25f * (quatLog.y + quaternion3.y), -0.25f * (quatLog.z + quaternion3.z), -0.25f * (quatLog.w + quaternion3.w));
            return (q1 * GetQuatExp(q));
        }

        public static bool IntersectRaySphere(Ray ray, Vector3 sphereOrigin, float sphereRadius, ref float t, ref Vector3 q)
        {
            Vector3 lhs = ray.origin - sphereOrigin;
            float num = Vector3.Dot(lhs, ray.direction);
            float num2 = Vector3.Dot(lhs, lhs) - (sphereRadius * sphereRadius);
            if ((num2 > 0f) && (num > 0f))
            {
                return false;
            }
            float f = (num * num) - num2;
            if (f < 0f)
            {
                return false;
            }
            t = -num - Mathf.Sqrt(f);
            if (t < 0f)
            {
                t = 0f;
            }
            q = ray.origin + (t * ray.direction);
            return true;
        }

        public static object IntersectRayTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2, bool bidirectional)
        {
            Vector3 lhs = v1 - v0;
            Vector3 rhs = v2 - v0;
            Vector3 vector3 = Vector3.Cross(lhs, rhs);
            float num = Vector3.Dot(-ray.direction, vector3);
            if (num <= 0f)
            {
                return null;
            }
            Vector3 vector4 = ray.origin - v0;
            float num2 = Vector3.Dot(vector4, vector3);
            if ((num2 < 0f) && !bidirectional)
            {
                return null;
            }
            Vector3 vector5 = Vector3.Cross(-ray.direction, vector4);
            float y = Vector3.Dot(rhs, vector5);
            if ((y < 0f) || (y > num))
            {
                return null;
            }
            float z = -Vector3.Dot(lhs, vector5);
            if ((z < 0f) || ((y + z) > num))
            {
                return null;
            }
            float num5 = 1f / num;
            num2 *= num5;
            y *= num5;
            z *= num5;
            float x = (1f - y) - z;
            return new RaycastHit { point = ray.origin + ((Vector3) (num2 * ray.direction)), distance = num2, barycentricCoordinate = new Vector3(x, y, z), normal = Vector3.Normalize(vector3) };
        }

        public static Matrix4x4 OrthogonalizeMatrix(Matrix4x4 m)
        {
            Matrix4x4 identity = Matrix4x4.identity;
            Vector3 column = (Vector3) m.GetColumn(0);
            Vector3 lhs = (Vector3) m.GetColumn(1);
            Vector3 normalized = m.GetColumn(2).normalized;
            column = Vector3.Cross(lhs, normalized).normalized;
            lhs = Vector3.Cross(normalized, column).normalized;
            identity.SetColumn(0, column);
            identity.SetColumn(1, lhs);
            identity.SetColumn(2, normalized);
            return identity;
        }

        public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
        {
            Quaternion quaternion;
            quaternion = new Quaternion {
                w = Mathf.Sqrt(Mathf.Max((float) 0f, (float) (((1f + m[0, 0]) + m[1, 1]) + m[2, 2]))) / 2f,
                x = Mathf.Sqrt(Mathf.Max((float) 0f, (float) (((1f + m[0, 0]) - m[1, 1]) - m[2, 2]))) / 2f,
                y = Mathf.Sqrt(Mathf.Max((float) 0f, (float) (((1f - m[0, 0]) + m[1, 1]) - m[2, 2]))) / 2f,
                z = Mathf.Sqrt(Mathf.Max((float) 0f, (float) (((1f - m[0, 0]) - m[1, 1]) + m[2, 2]))) / 2f,
                x = quaternion.x * Mathf.Sign(quaternion.x * (m[2, 1] - m[1, 2])),
                y = quaternion.y * Mathf.Sign(quaternion.y * (m[0, 2] - m[2, 0])),
                z = quaternion.z * Mathf.Sign(quaternion.z * (m[1, 0] - m[0, 1]))
            };
            QuaternionNormalize(ref quaternion);
            return quaternion;
        }

        public static void QuaternionNormalize(ref Quaternion q)
        {
            float num = 1f / Mathf.Sqrt((((q.x * q.x) + (q.y * q.y)) + (q.z * q.z)) + (q.w * q.w));
            q.x *= num;
            q.y *= num;
            q.z *= num;
            q.w *= num;
        }

        internal static double RoundBasedOnMinimumDifference(double valueToRound, double minDifference)
        {
            if (minDifference == 0.0)
            {
                return DiscardLeastSignificantDecimal(valueToRound);
            }
            return Math.Round(valueToRound, GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
        }

        internal static float RoundBasedOnMinimumDifference(float valueToRound, float minDifference)
        {
            if (minDifference == 0f)
            {
                return DiscardLeastSignificantDecimal(valueToRound);
            }
            return (float) Math.Round((double) valueToRound, GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
        }

        internal static float RoundToMultipleOf(float value, float roundingValue)
        {
            if (roundingValue == 0f)
            {
                return value;
            }
            return (Mathf.Round(value / roundingValue) * roundingValue);
        }

        public static Quaternion Slerp(Quaternion p, Quaternion q, float t)
        {
            Quaternion quaternion;
            float f = Quaternion.Dot(p, q);
            if ((1f + f) > 1E-05)
            {
                float num2;
                float num3;
                if ((1f - f) > 1E-05)
                {
                    float num4 = Mathf.Acos(f);
                    float num5 = 1f / Mathf.Sin(num4);
                    num2 = Mathf.Sin((1f - t) * num4) * num5;
                    num3 = Mathf.Sin(t * num4) * num5;
                }
                else
                {
                    num2 = 1f - t;
                    num3 = t;
                }
                quaternion.x = (num2 * p.x) + (num3 * q.x);
                quaternion.y = (num2 * p.y) + (num3 * q.y);
                quaternion.z = (num2 * p.z) + (num3 * q.z);
                quaternion.w = (num2 * p.w) + (num3 * q.w);
                return quaternion;
            }
            float num6 = Mathf.Sin(((1f - t) * 3.141593f) * 0.5f);
            float num7 = Mathf.Sin((t * 3.141593f) * 0.5f);
            quaternion.x = (num6 * p.x) - (num7 * p.y);
            quaternion.y = (num6 * p.y) + (num7 * p.x);
            quaternion.z = (num6 * p.z) - (num7 * p.w);
            quaternion.w = p.z;
            return quaternion;
        }
    }
}

