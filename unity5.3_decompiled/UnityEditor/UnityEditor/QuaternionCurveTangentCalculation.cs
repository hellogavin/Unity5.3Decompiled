namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct QuaternionCurveTangentCalculation
    {
        private AnimationCurve eulerX;
        private AnimationCurve eulerY;
        private AnimationCurve eulerZ;
        public AnimationCurve GetCurve(int index)
        {
            if (index == 0)
            {
                return this.eulerX;
            }
            if (index == 1)
            {
                return this.eulerY;
            }
            return this.eulerZ;
        }

        public void SetCurve(int index, AnimationCurve curve)
        {
            if (index == 0)
            {
                this.eulerX = curve;
            }
            else if (index == 1)
            {
                this.eulerY = curve;
            }
            else
            {
                this.eulerZ = curve;
            }
        }

        private Vector3 EvaluateEulerCurvesDirectly(float time)
        {
            return new Vector3(this.eulerX.Evaluate(time), this.eulerY.Evaluate(time), this.eulerZ.Evaluate(time));
        }

        public float CalculateLinearTangent(int fromIndex, int toIndex, int componentIndex)
        {
            AnimationCurve curve = this.GetCurve(componentIndex);
            return this.CalculateLinearTangent(curve[fromIndex], curve[toIndex], componentIndex);
        }

        public float CalculateLinearTangent(Keyframe from, Keyframe to, int component)
        {
            float t = 0.01f;
            Vector3 euler = this.EvaluateEulerCurvesDirectly(to.time);
            Vector3 vector2 = this.EvaluateEulerCurvesDirectly(from.time);
            Quaternion a = Quaternion.Euler(euler);
            Quaternion b = Quaternion.Euler(vector2);
            Vector3 eulerFromQuaternion = GetEulerFromQuaternion(Quaternion.Slerp(a, b, t), euler);
            switch (component)
            {
                case 0:
                    return (((eulerFromQuaternion.x - euler.x) / t) / -(to.time - from.time));

                case 1:
                    return (((eulerFromQuaternion.y - euler.y) / t) / -(to.time - from.time));

                case 2:
                    return (((eulerFromQuaternion.z - euler.z) / t) / -(to.time - from.time));
            }
            return 0f;
        }

        public float CalculateSmoothTangent(int index, int component)
        {
            AnimationCurve curve = this.GetCurve(component);
            if (curve.length < 2)
            {
                return 0f;
            }
            if (index <= 0)
            {
                return this.CalculateLinearTangent(curve[0], curve[1], component);
            }
            if (index >= (curve.length - 1))
            {
                return this.CalculateLinearTangent(curve[curve.length - 1], curve[curve.length - 2], component);
            }
            Keyframe keyframe = curve[index - 1];
            float time = keyframe.time;
            Keyframe keyframe2 = curve[index];
            float num2 = keyframe2.time;
            Keyframe keyframe3 = curve[index + 1];
            float num3 = keyframe3.time;
            Vector3 euler = this.EvaluateEulerCurvesDirectly(time);
            Vector3 vector2 = this.EvaluateEulerCurvesDirectly(num2);
            Vector3 vector3 = this.EvaluateEulerCurvesDirectly(num3);
            Quaternion quaternion = Quaternion.Euler(euler);
            Quaternion quaternion2 = Quaternion.Euler(vector2);
            Quaternion quaternion3 = Quaternion.Euler(vector3);
            if (((((quaternion.x * quaternion2.x) + (quaternion.y * quaternion2.y)) + (quaternion.z * quaternion2.z)) + (quaternion.w * quaternion2.w)) < 0f)
            {
                quaternion = new Quaternion(-quaternion.x, -quaternion.y, -quaternion.z, -quaternion.w);
            }
            if (((((quaternion3.x * quaternion2.x) + (quaternion3.y * quaternion2.y)) + (quaternion3.z * quaternion2.z)) + (quaternion3.w * quaternion2.w)) < 0f)
            {
                quaternion3 = new Quaternion(-quaternion3.x, -quaternion3.y, -quaternion3.z, -quaternion3.w);
            }
            Quaternion quaternion4 = new Quaternion();
            float dx = num2 - time;
            float num5 = num3 - num2;
            for (int i = 0; i < 4; i++)
            {
                float dy = quaternion2[i] - quaternion[i];
                float num8 = quaternion3[i] - quaternion2[i];
                float num9 = SafeDeltaDivide(dy, dx);
                float num10 = SafeDeltaDivide(num8, num5);
                quaternion4[i] = (0.5f * num9) + (0.5f * num10);
            }
            float num11 = Mathf.Abs((float) (num3 - time)) * 0.01f;
            Quaternion q = new Quaternion(quaternion2.x - (quaternion4.x * num11), quaternion2.y - (quaternion4.y * num11), quaternion2.z - (quaternion4.z * num11), quaternion2.w - (quaternion4.w * num11));
            Quaternion quaternion6 = new Quaternion(quaternion2.x + (quaternion4.x * num11), quaternion2.y + (quaternion4.y * num11), quaternion2.z + (quaternion4.z * num11), quaternion2.w + (quaternion4.w * num11));
            Vector3 eulerFromQuaternion = GetEulerFromQuaternion(q, vector2);
            Vector3 vector6 = (Vector3) ((GetEulerFromQuaternion(quaternion6, vector2) - eulerFromQuaternion) / (num11 * 2f));
            return vector6[component];
        }

        public static Vector3[] GetEquivalentEulerAngles(Quaternion quat)
        {
            Vector3 eulerAngles = quat.eulerAngles;
            return new Vector3[] { eulerAngles, new Vector3(180f - eulerAngles.x, eulerAngles.y + 180f, eulerAngles.z + 180f) };
        }

        public static Vector3 GetEulerFromQuaternion(Quaternion q, Vector3 refEuler)
        {
            Vector3[] equivalentEulerAngles = GetEquivalentEulerAngles(q);
            for (int i = 0; i < equivalentEulerAngles.Length; i++)
            {
                equivalentEulerAngles[i] = new Vector3((Mathf.Repeat((equivalentEulerAngles[i].x - refEuler.x) + 180f, 360f) + refEuler.x) - 180f, (Mathf.Repeat((equivalentEulerAngles[i].y - refEuler.y) + 180f, 360f) + refEuler.y) - 180f, (Mathf.Repeat((equivalentEulerAngles[i].z - refEuler.z) + 180f, 360f) + refEuler.z) - 180f);
                float num2 = Mathf.Repeat(equivalentEulerAngles[i].x, 360f);
                if (Mathf.Abs((float) (num2 - 90f)) < 1f)
                {
                    float num3 = equivalentEulerAngles[i].z - equivalentEulerAngles[i].y;
                    float num4 = refEuler.z - refEuler.y;
                    float num5 = num3 - num4;
                    equivalentEulerAngles[i].z = refEuler.z + (num5 * 0.5f);
                    equivalentEulerAngles[i].y = refEuler.y - (num5 * 0.5f);
                }
                if (Mathf.Abs((float) (num2 - 270f)) < 1f)
                {
                    float num6 = equivalentEulerAngles[i].z + equivalentEulerAngles[i].y;
                    float num7 = refEuler.z + refEuler.y;
                    float num8 = num6 - num7;
                    equivalentEulerAngles[i].z = refEuler.z + (num8 * 0.5f);
                    equivalentEulerAngles[i].y = refEuler.y + (num8 * 0.5f);
                }
            }
            Vector3 vector = equivalentEulerAngles[0];
            Vector3 vector2 = equivalentEulerAngles[0] - refEuler;
            float sqrMagnitude = vector2.sqrMagnitude;
            for (int j = 1; j < equivalentEulerAngles.Length; j++)
            {
                Vector3 vector3 = equivalentEulerAngles[j] - refEuler;
                float num11 = vector3.sqrMagnitude;
                if (num11 < sqrMagnitude)
                {
                    sqrMagnitude = num11;
                    vector = equivalentEulerAngles[j];
                }
            }
            return vector;
        }

        public static float SafeDeltaDivide(float dy, float dx)
        {
            if (dx == 0f)
            {
                return 0f;
            }
            return (dy / dx);
        }

        public void UpdateTangentsFromMode(int componentIndex)
        {
            AnimationCurve curve = this.GetCurve(componentIndex);
            for (int i = 0; i < curve.length; i++)
            {
                this.UpdateTangentsFromMode(i, componentIndex);
            }
        }

        public void UpdateTangentsFromMode(int index, int componentIndex)
        {
            AnimationCurve curve = this.GetCurve(componentIndex);
            if ((index >= 0) && (index < curve.length))
            {
                Keyframe key = curve[index];
                if ((CurveUtility.GetKeyTangentMode(key, 0) == TangentMode.Linear) && (index >= 1))
                {
                    key.inTangent = this.CalculateLinearTangent(index, index - 1, componentIndex);
                    curve.MoveKey(index, key);
                }
                if ((CurveUtility.GetKeyTangentMode(key, 1) == TangentMode.Linear) && ((index + 1) < curve.length))
                {
                    key.outTangent = this.CalculateLinearTangent(index, index + 1, componentIndex);
                    curve.MoveKey(index, key);
                }
                if ((CurveUtility.GetKeyTangentMode(key, 0) == TangentMode.Smooth) || (CurveUtility.GetKeyTangentMode(key, 1) == TangentMode.Smooth))
                {
                    float num = this.CalculateSmoothTangent(index, componentIndex);
                    key.outTangent = num;
                    key.inTangent = num;
                    curve.MoveKey(index, key);
                }
            }
        }

        public static void UpdateTangentsFromMode(AnimationCurve curve, AnimationClip clip, EditorCurveBinding curveBinding)
        {
            CurveUtility.UpdateTangentsFromMode(curve);
        }
    }
}

