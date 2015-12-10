namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal static class AvatarSetupTool
    {
        private static BonePoseData[] sBonePoses;
        private static string sHasTranslationDoF = "m_HumanDescription.m_HasTranslationDoF";
        private static string sHuman = "m_HumanDescription.m_Human";
        internal static int[] sHumanParent = new int[] { 
            0, 0, 0, 1, 2, 3, 4, 0, 7, 8, 9, 8, 8, 11, 12, 13, 
            14, 15, 0x10, 5, 6, 10, 10, 10, 0x11, 0x18, 0x19, 0x11, 0x1b, 0x1c, 0x11, 30, 
            0x1f, 0x11, 0x21, 0x22, 0x11, 0x24, 0x25, 0x12, 0x27, 40, 0x12, 0x2a, 0x2b, 0x12, 0x2d, 0x2e, 
            0x12, 0x30, 0x31, 0x12, 0x33, 0x34
         };
        internal static string sName = "m_Name";
        internal static string sPosition = "m_Position";
        internal static string sRotation = "m_Rotation";
        internal static string sScale = "m_Scale";
        internal static string sSkeleton = "m_HumanDescription.m_Skeleton";
        internal static string sTransformModified = "m_TransformModified";

        static AvatarSetupTool()
        {
            BonePoseData[] dataArray1 = new BonePoseData[0x36];
            dataArray1[0] = new BonePoseData(Vector3.up, true, 15f);
            dataArray1[1] = new BonePoseData(new Vector3(-0.05f, -1f, 0f), true, 15f);
            dataArray1[2] = new BonePoseData(new Vector3(0.05f, -1f, 0f), true, 15f);
            dataArray1[3] = new BonePoseData(new Vector3(-0.05f, -1f, -0.15f), true, 20f);
            dataArray1[4] = new BonePoseData(new Vector3(0.05f, -1f, -0.15f), true, 20f);
            dataArray1[5] = new BonePoseData(new Vector3(-0.05f, 0f, 1f), true, 20f, Vector3.up, null);
            dataArray1[6] = new BonePoseData(new Vector3(0.05f, 0f, 1f), true, 20f, Vector3.up, null);
            int[] children = new int[] { 8, 9, 10 };
            dataArray1[7] = new BonePoseData(Vector3.up, true, 30f, children);
            int[] numArray2 = new int[] { 9, 10 };
            dataArray1[8] = new BonePoseData(Vector3.up, true, 30f, numArray2);
            dataArray1[9] = new BonePoseData(Vector3.up, true, 30f);
            dataArray1[11] = new BonePoseData(-Vector3.right, true, 20f);
            dataArray1[12] = new BonePoseData(Vector3.right, true, 20f);
            dataArray1[13] = new BonePoseData(-Vector3.right, true, 5f);
            dataArray1[14] = new BonePoseData(Vector3.right, true, 5f);
            dataArray1[15] = new BonePoseData(-Vector3.right, true, 5f);
            dataArray1[0x10] = new BonePoseData(Vector3.right, true, 5f);
            int[] numArray3 = new int[] { 30 };
            dataArray1[0x11] = new BonePoseData(-Vector3.right, false, 10f, Vector3.forward, numArray3);
            int[] numArray4 = new int[] { 0x2d };
            dataArray1[0x12] = new BonePoseData(Vector3.right, false, 10f, Vector3.forward, numArray4);
            dataArray1[0x18] = new BonePoseData(new Vector3(-1f, 0f, 1f), false, 10f);
            dataArray1[0x19] = new BonePoseData(new Vector3(-1f, 0f, 1f), false, 5f);
            dataArray1[0x1a] = new BonePoseData(new Vector3(-1f, 0f, 1f), false, 5f);
            dataArray1[0x1b] = new BonePoseData(-Vector3.right, false, 10f);
            dataArray1[0x1c] = new BonePoseData(-Vector3.right, false, 5f);
            dataArray1[0x1d] = new BonePoseData(-Vector3.right, false, 5f);
            dataArray1[30] = new BonePoseData(-Vector3.right, false, 10f);
            dataArray1[0x1f] = new BonePoseData(-Vector3.right, false, 5f);
            dataArray1[0x20] = new BonePoseData(-Vector3.right, false, 5f);
            dataArray1[0x21] = new BonePoseData(-Vector3.right, false, 10f);
            dataArray1[0x22] = new BonePoseData(-Vector3.right, false, 5f);
            dataArray1[0x23] = new BonePoseData(-Vector3.right, false, 5f);
            dataArray1[0x24] = new BonePoseData(-Vector3.right, false, 10f);
            dataArray1[0x25] = new BonePoseData(-Vector3.right, false, 5f);
            dataArray1[0x26] = new BonePoseData(-Vector3.right, false, 5f);
            dataArray1[0x27] = new BonePoseData(new Vector3(1f, 0f, 1f), false, 10f);
            dataArray1[40] = new BonePoseData(new Vector3(1f, 0f, 1f), false, 5f);
            dataArray1[0x29] = new BonePoseData(new Vector3(1f, 0f, 1f), false, 5f);
            dataArray1[0x2a] = new BonePoseData(Vector3.right, false, 10f);
            dataArray1[0x2b] = new BonePoseData(Vector3.right, false, 5f);
            dataArray1[0x2c] = new BonePoseData(Vector3.right, false, 5f);
            dataArray1[0x2d] = new BonePoseData(Vector3.right, false, 10f);
            dataArray1[0x2e] = new BonePoseData(Vector3.right, false, 5f);
            dataArray1[0x2f] = new BonePoseData(Vector3.right, false, 5f);
            dataArray1[0x30] = new BonePoseData(Vector3.right, false, 10f);
            dataArray1[0x31] = new BonePoseData(Vector3.right, false, 5f);
            dataArray1[50] = new BonePoseData(Vector3.right, false, 5f);
            dataArray1[0x33] = new BonePoseData(Vector3.right, false, 10f);
            dataArray1[0x34] = new BonePoseData(Vector3.right, false, 5f);
            dataArray1[0x35] = new BonePoseData(Vector3.right, false, 5f);
            sBonePoses = dataArray1;
        }

        public static void AutoSetup(GameObject modelPrefab, GameObject modelInstance, SerializedObject modelImporterSerializedObject)
        {
            SerializedProperty property = modelImporterSerializedObject.FindProperty(sHuman);
            SerializedProperty property2 = modelImporterSerializedObject.FindProperty(sHasTranslationDoF);
            if ((property != null) && property.isArray)
            {
                ClearHumanBoneArray(modelImporterSerializedObject);
                bool flag = AvatarBipedMapper.IsBiped(modelInstance.transform, null);
                SampleBindPose(modelInstance);
                Dictionary<Transform, bool> validBones = GetModelBones(modelInstance.transform, false, null);
                Dictionary<int, Transform> dictionary2 = null;
                if (flag)
                {
                    dictionary2 = AvatarBipedMapper.MapBones(modelInstance.transform);
                    AvatarBipedMapper.BipedPose(modelInstance);
                }
                else
                {
                    dictionary2 = AvatarAutoMapper.MapBones(modelInstance.transform, validBones);
                }
                BoneWrapper[] humanBones = GetHumanBones(modelImporterSerializedObject, validBones);
                for (int i = 0; i < humanBones.Length; i++)
                {
                    BoneWrapper wrapper = humanBones[i];
                    if (dictionary2.ContainsKey(i))
                    {
                        wrapper.bone = dictionary2[i];
                    }
                    else
                    {
                        wrapper.bone = null;
                    }
                    wrapper.Serialize(modelImporterSerializedObject);
                }
                if (!flag)
                {
                    float poseError = GetPoseError(humanBones);
                    CopyPose(modelInstance, modelPrefab);
                    float num3 = GetPoseError(humanBones);
                    if (poseError < num3)
                    {
                        SampleBindPose(modelInstance);
                    }
                    MakePoseValid(humanBones);
                }
                else
                {
                    property2.boolValue = true;
                }
                TransferPoseToDescription(modelImporterSerializedObject, modelInstance.transform);
            }
        }

        public static void AutoSetupOnInstance(GameObject modelPrefab, SerializedObject modelImporterSerializedObject)
        {
            GameObject modelInstance = Object.Instantiate<GameObject>(modelPrefab);
            modelInstance.hideFlags = HideFlags.HideAndDontSave;
            AutoSetup(modelPrefab, modelInstance, modelImporterSerializedObject);
            Object.DestroyImmediate(modelInstance);
        }

        public static Quaternion AvatarComputeOrientation(BoneWrapper[] bones)
        {
            Transform bone = bones[1].bone;
            Transform transform2 = bones[2].bone;
            Transform transform3 = bones[13].bone;
            Transform transform4 = bones[14].bone;
            if (((bone != null) && (transform2 != null)) && ((transform3 != null) && (transform4 != null)))
            {
                return AvatarComputeOrientation(bone.position, transform2.position, transform3.position, transform4.position);
            }
            return Quaternion.identity;
        }

        public static Quaternion AvatarComputeOrientation(Vector3 leftUpLeg, Vector3 rightUpLeg, Vector3 leftArm, Vector3 rightArm)
        {
            Vector3 vector = Vector3.Normalize(rightUpLeg - leftUpLeg);
            Vector3 vector2 = Vector3.Normalize(rightArm - leftArm);
            Vector3 lhs = Vector3.Normalize(vector + vector2);
            bool flag = ((Mathf.Abs((float) (lhs.x * lhs.y)) < 0.05f) && (Mathf.Abs((float) (lhs.y * lhs.z)) < 0.05f)) && (Mathf.Abs((float) (lhs.z * lhs.x)) < 0.05f);
            Vector3 vector4 = (Vector3) ((leftUpLeg + rightUpLeg) * 0.5f);
            Vector3 vector5 = (Vector3) ((leftArm + rightArm) * 0.5f);
            Vector3 rhs = Vector3.Normalize(vector5 - vector4);
            if (flag)
            {
                int num = 0;
                for (int i = 1; i < 3; i++)
                {
                    float introduced11 = Mathf.Abs(rhs[i]);
                    if (introduced11 > Mathf.Abs(rhs[num]))
                    {
                        num = i;
                    }
                }
                float num3 = Mathf.Sign(rhs[num]);
                rhs = Vector3.zero;
                rhs[num] = num3;
            }
            Vector3 forward = Vector3.Cross(lhs, rhs);
            if (!(forward == Vector3.zero) && !(rhs == Vector3.zero))
            {
                return Quaternion.LookRotation(forward, rhs);
            }
            return Quaternion.identity;
        }

        public static void ClearAll(SerializedObject serializedObject)
        {
            ClearHumanBoneArray(serializedObject);
            ClearSkeletonBoneArray(serializedObject);
        }

        public static void ClearHumanBoneArray(SerializedObject serializedObject)
        {
            SerializedProperty property = serializedObject.FindProperty(sHuman);
            if ((property != null) && property.isArray)
            {
                property.ClearArray();
            }
        }

        public static void ClearSkeletonBoneArray(SerializedObject serializedObject)
        {
            SerializedProperty property = serializedObject.FindProperty(sSkeleton);
            if ((property != null) && property.isArray)
            {
                property.ClearArray();
            }
        }

        public static void CopyPose(GameObject go, GameObject source)
        {
            GameObject obj2 = Object.Instantiate<GameObject>(source);
            obj2.hideFlags = HideFlags.HideAndDontSave;
            AnimatorUtility.DeoptimizeTransformHierarchy(obj2);
            CopyPose(go.transform, obj2.transform);
            Object.DestroyImmediate(obj2);
        }

        private static void CopyPose(Transform t, Transform source)
        {
            t.localPosition = source.localPosition;
            t.localRotation = source.localRotation;
            t.localScale = source.localScale;
            IEnumerator enumerator = t.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    Transform transform2 = source.FindChild(current.name);
                    if (transform2 != null)
                    {
                        CopyPose(current, transform2);
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        public static void DebugTransformTree(Transform tr, Dictionary<Transform, bool> bones, int level)
        {
            string str = "  ";
            if (bones.ContainsKey(tr))
            {
                if (bones[tr])
                {
                    str = "* ";
                }
                else
                {
                    str = ". ";
                }
            }
            Debug.Log("                                             ".Substring(0, level * 2) + str + tr.name + "\n\n");
            IEnumerator enumerator = tr.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    DebugTransformTree(current, bones, level + 1);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        private static bool DetermineIsActualBone(Transform tr, Dictionary<Transform, bool> bones, List<Transform> skinnedBones, bool includeAll, BoneWrapper[] humanBones)
        {
            bool flag = includeAll;
            bool flag2 = false;
            bool flag3 = false;
            int num = 0;
            IEnumerator enumerator = tr.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (DetermineIsActualBone(current, bones, skinnedBones, includeAll, humanBones))
                    {
                        num++;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            if (num > 0)
            {
                flag2 = true;
            }
            if (num > 1)
            {
                flag = true;
            }
            if (!flag && skinnedBones.Contains(tr))
            {
                flag = true;
            }
            if (!flag)
            {
                Component[] components = tr.GetComponents<Component>();
                if (components.Length > 1)
                {
                    foreach (Component component in components)
                    {
                        if ((component is Renderer) && !(component is SkinnedMeshRenderer))
                        {
                            Bounds bounds = (component as Renderer).bounds;
                            bounds.extents = bounds.size;
                            if (((tr.childCount == 0) && (tr.parent != null)) && bounds.Contains(tr.parent.position))
                            {
                                if (tr.parent.GetComponent<Renderer>() != null)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    flag3 = true;
                                }
                            }
                            else if (bounds.Contains(tr.position))
                            {
                                flag = true;
                            }
                        }
                    }
                }
            }
            if (!flag && (humanBones != null))
            {
                foreach (BoneWrapper wrapper in humanBones)
                {
                    if (tr == wrapper.bone)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                bones[tr] = true;
            }
            else if (flag2)
            {
                if (!bones.ContainsKey(tr))
                {
                    bones[tr] = false;
                }
            }
            else if (flag3)
            {
                bones[tr.parent] = true;
            }
            return bones.ContainsKey(tr);
        }

        public static SerializedProperty FindSkeletonBone(SerializedObject serializedObject, Transform t, bool createMissing, bool isRoot)
        {
            SerializedProperty skeletonBoneArray = serializedObject.FindProperty(sSkeleton);
            if ((skeletonBoneArray != null) && skeletonBoneArray.isArray)
            {
                return FindSkeletonBone(skeletonBoneArray, t, createMissing, isRoot);
            }
            return null;
        }

        public static SerializedProperty FindSkeletonBone(SerializedProperty skeletonBoneArray, Transform t, bool createMissing, bool isRoot)
        {
            if (isRoot && (skeletonBoneArray.arraySize > 0))
            {
                SerializedProperty arrayElementAtIndex = skeletonBoneArray.GetArrayElementAtIndex(0);
                if (arrayElementAtIndex.FindPropertyRelative(sName).stringValue == t.name)
                {
                    return arrayElementAtIndex;
                }
            }
            else
            {
                for (int i = 1; i < skeletonBoneArray.arraySize; i++)
                {
                    SerializedProperty property3 = skeletonBoneArray.GetArrayElementAtIndex(i);
                    if (property3.FindPropertyRelative(sName).stringValue == t.name)
                    {
                        return property3;
                    }
                }
            }
            if (createMissing)
            {
                skeletonBoneArray.arraySize++;
                SerializedProperty property5 = skeletonBoneArray.GetArrayElementAtIndex(skeletonBoneArray.arraySize - 1);
                if (property5 != null)
                {
                    property5.FindPropertyRelative(sName).stringValue = t.name;
                    property5.FindPropertyRelative(sPosition).vector3Value = t.localPosition;
                    property5.FindPropertyRelative(sRotation).quaternionValue = t.localRotation;
                    property5.FindPropertyRelative(sScale).vector3Value = t.localScale;
                    property5.FindPropertyRelative(sTransformModified).boolValue = true;
                    return property5;
                }
            }
            return null;
        }

        public static void GetBindPoseBonePositionRotation(Matrix4x4 skinMatrix, Matrix4x4 boneMatrix, Transform bone, out Vector3 position, out Quaternion rotation)
        {
            Matrix4x4 matrixx = skinMatrix * boneMatrix.inverse;
            Vector3 lhs = new Vector3(matrixx.m00, matrixx.m10, matrixx.m20);
            Vector3 rhs = new Vector3(matrixx.m01, matrixx.m11, matrixx.m21);
            Vector3 vector3 = new Vector3(matrixx.m02, matrixx.m12, matrixx.m22);
            Vector3 vector4 = new Vector3(matrixx.m03, matrixx.m13, matrixx.m23);
            float magnitude = vector3.magnitude;
            float num2 = Mathf.Abs(bone.lossyScale.z);
            position = (Vector3) (vector4 * (num2 / magnitude));
            if (Vector3.Dot(Vector3.Cross(lhs, rhs), vector3) >= 0f)
            {
                rotation = Quaternion.LookRotation(vector3, rhs);
            }
            else
            {
                rotation = Quaternion.LookRotation(-vector3, -rhs);
            }
        }

        private static Vector3 GetBoneAlignmentDirection(BoneWrapper[] bones, Quaternion avatarOrientation, int boneIndex)
        {
            Vector3 zero;
            if (sBonePoses[boneIndex] == null)
            {
                return Vector3.zero;
            }
            BoneWrapper wrapper = bones[boneIndex];
            BonePoseData data = sBonePoses[boneIndex];
            int index = -1;
            if (data.childIndices != null)
            {
                foreach (int num2 in data.childIndices)
                {
                    if (bones[num2].bone != null)
                    {
                        index = num2;
                        break;
                    }
                }
            }
            else
            {
                index = GetHumanBoneChild(bones, boneIndex);
            }
            if (((index >= 0) && (bones[index] != null)) && (bones[index].bone != null))
            {
                BoneWrapper wrapper2 = bones[index];
                zero = wrapper2.bone.position - wrapper.bone.position;
            }
            else
            {
                if (wrapper.bone.childCount != 1)
                {
                    return Vector3.zero;
                }
                zero = Vector3.zero;
                IEnumerator enumerator = wrapper.bone.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        zero = current.position - wrapper.bone.position;
                        goto Label_0146;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        Label_0146:
            return zero.normalized;
        }

        public static float GetBoneAlignmentError(BoneWrapper[] bones, Quaternion avatarOrientation, int boneIndex)
        {
            if ((boneIndex < 0) || (boneIndex >= sBonePoses.Length))
            {
                return 0f;
            }
            BoneWrapper wrapper = bones[boneIndex];
            BonePoseData data = sBonePoses[boneIndex];
            if ((wrapper.bone == null) || (data == null))
            {
                return 0f;
            }
            if (boneIndex == 0)
            {
                float num = Vector3.Angle((Vector3) (avatarOrientation * Vector3.right), Vector3.right);
                float num2 = Vector3.Angle((Vector3) (avatarOrientation * Vector3.up), Vector3.up);
                float num3 = Vector3.Angle((Vector3) (avatarOrientation * Vector3.forward), Vector3.forward);
                float[] values = new float[] { num, num2, num3 };
                return Mathf.Max((float) 0f, (float) (Mathf.Max(values) - data.maxAngle));
            }
            Vector3 vector = GetBoneAlignmentDirection(bones, avatarOrientation, boneIndex);
            if (vector == Vector3.zero)
            {
                return 0f;
            }
            Quaternion quaternion = GetRotationSpace(bones, avatarOrientation, boneIndex);
            Vector3 to = (Vector3) (quaternion * data.direction);
            if (data.planeNormal != Vector3.zero)
            {
                vector = Vector3.ProjectOnPlane(vector, (Vector3) (quaternion * data.planeNormal));
            }
            return Mathf.Max((float) 0f, (float) (Vector3.Angle(vector, to) - data.maxAngle));
        }

        private static Vector3 GetCharacterPositionAdjustVector(BoneWrapper[] bones, out float error)
        {
            error = 0f;
            Transform bone = bones[1].bone;
            Transform transform2 = bones[2].bone;
            if ((bone == null) || (transform2 == null))
            {
                return Vector3.zero;
            }
            Vector3 vector = (Vector3) ((bone.position + transform2.position) * 0.5f);
            bool flag = true;
            Transform transform3 = bones[0x13].bone;
            Transform transform4 = bones[20].bone;
            if ((transform3 == null) || (transform4 == null))
            {
                flag = false;
                transform3 = bones[5].bone;
                transform4 = bones[6].bone;
            }
            if ((transform3 == null) || (transform4 == null))
            {
                return Vector3.zero;
            }
            Vector3 vector2 = (Vector3) ((transform3.position + transform4.position) * 0.5f);
            float num = vector.y - vector2.y;
            if (num <= 0f)
            {
                return Vector3.zero;
            }
            Vector3 zero = Vector3.zero;
            if ((vector2.y < 0f) || (vector2.y > (num * (!flag ? 0.3f : 0.1f))))
            {
                float num2 = vector.y - (num * (!flag ? 1.13f : 1.03f));
                zero.y = -num2;
            }
            if (Mathf.Abs(vector.x) > (0.01f * num))
            {
                zero.x = -vector.x;
            }
            if (Mathf.Abs(vector.z) > (0.2f * num))
            {
                zero.z = -vector.z;
            }
            error = (zero.magnitude * 100f) / num;
            return zero;
        }

        private static float GetCharacterPositionError(BoneWrapper[] bones)
        {
            float num;
            GetCharacterPositionAdjustVector(bones, out num);
            return num;
        }

        public static int GetFirstHumanBoneAncestor(BoneWrapper[] bones, int boneIndex)
        {
            boneIndex = sHumanParent[boneIndex];
            while ((bones[boneIndex].bone == null) && (boneIndex != 0))
            {
                boneIndex = sHumanParent[boneIndex];
            }
            return boneIndex;
        }

        public static int GetHumanBoneChild(BoneWrapper[] bones, int boneIndex)
        {
            for (int i = 0; i < sHumanParent.Length; i++)
            {
                if (sHumanParent[i] == boneIndex)
                {
                    return i;
                }
            }
            return -1;
        }

        public static BoneWrapper[] GetHumanBones(SerializedObject serializedObject, Dictionary<Transform, bool> actualBones)
        {
            string[] boneName = HumanTrait.BoneName;
            BoneWrapper[] wrapperArray = new BoneWrapper[boneName.Length];
            for (int i = 0; i < boneName.Length; i++)
            {
                wrapperArray[i] = new BoneWrapper(boneName[i], serializedObject, actualBones);
            }
            return wrapperArray;
        }

        public static Dictionary<Transform, bool> GetModelBones(Transform root, bool includeAll, BoneWrapper[] humanBones)
        {
            if (root == null)
            {
                return null;
            }
            Dictionary<Transform, bool> bones = new Dictionary<Transform, bool>();
            List<Transform> skinnedBones = new List<Transform>();
            if (!includeAll)
            {
                foreach (SkinnedMeshRenderer renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    Transform[] transformArray = renderer.bones;
                    bool[] flagArray = new bool[transformArray.Length];
                    foreach (BoneWeight weight in renderer.sharedMesh.boneWeights)
                    {
                        if (weight.weight0 != 0f)
                        {
                            flagArray[weight.boneIndex0] = true;
                        }
                        if (weight.weight1 != 0f)
                        {
                            flagArray[weight.boneIndex1] = true;
                        }
                        if (weight.weight2 != 0f)
                        {
                            flagArray[weight.boneIndex2] = true;
                        }
                        if (weight.weight3 != 0f)
                        {
                            flagArray[weight.boneIndex3] = true;
                        }
                    }
                    for (int i = 0; i < transformArray.Length; i++)
                    {
                        if (flagArray[i] && !skinnedBones.Contains(transformArray[i]))
                        {
                            skinnedBones.Add(transformArray[i]);
                        }
                    }
                }
                DetermineIsActualBone(root, bones, skinnedBones, false, humanBones);
            }
            if (bones.Count < HumanTrait.RequiredBoneCount)
            {
                bones.Clear();
                skinnedBones.Clear();
                DetermineIsActualBone(root, bones, skinnedBones, true, humanBones);
            }
            return bones;
        }

        public static float GetPoseError(BoneWrapper[] bones)
        {
            Quaternion avatarOrientation = AvatarComputeOrientation(bones);
            float num = 0f;
            for (int i = 0; i < sBonePoses.Length; i++)
            {
                num += GetBoneAlignmentError(bones, avatarOrientation, i);
            }
            return (num + GetCharacterPositionError(bones));
        }

        private static Quaternion GetRotationSpace(BoneWrapper[] bones, Quaternion avatarOrientation, int boneIndex)
        {
            Quaternion identity = Quaternion.identity;
            BonePoseData data = sBonePoses[boneIndex];
            if (!data.compareInGlobalSpace)
            {
                int index = sHumanParent[boneIndex];
                BonePoseData data2 = sBonePoses[index];
                if ((bones[index].bone != null) && (data2 != null))
                {
                    Vector3 toDirection = GetBoneAlignmentDirection(bones, avatarOrientation, index);
                    if (toDirection != Vector3.zero)
                    {
                        Vector3 fromDirection = (Vector3) (avatarOrientation * data2.direction);
                        identity = Quaternion.FromToRotation(fromDirection, toDirection);
                    }
                }
            }
            return (identity * avatarOrientation);
        }

        public static bool IsPoseValid(BoneWrapper[] bones)
        {
            return (GetPoseError(bones) == 0f);
        }

        public static bool IsPoseValidOnInstance(GameObject modelPrefab, SerializedObject modelImporterSerializedObject)
        {
            GameObject obj2 = Object.Instantiate<GameObject>(modelPrefab);
            obj2.hideFlags = HideFlags.HideAndDontSave;
            Dictionary<Transform, bool> actualBones = GetModelBones(obj2.transform, false, null);
            BoneWrapper[] humanBones = GetHumanBones(modelImporterSerializedObject, actualBones);
            TransferDescriptionToPose(modelImporterSerializedObject, obj2.transform);
            bool flag = IsPoseValid(humanBones);
            Object.DestroyImmediate(obj2);
            return flag;
        }

        public static void MakeBoneAlignmentValid(BoneWrapper[] bones, Quaternion avatarOrientation, int boneIndex)
        {
            if (((boneIndex >= 0) && (boneIndex < sBonePoses.Length)) && (boneIndex < bones.Length))
            {
                BoneWrapper wrapper = bones[boneIndex];
                BonePoseData data = sBonePoses[boneIndex];
                if ((wrapper.bone != null) && (data != null))
                {
                    if (boneIndex == 0)
                    {
                        float num = Vector3.Angle((Vector3) (avatarOrientation * Vector3.right), Vector3.right);
                        float num2 = Vector3.Angle((Vector3) (avatarOrientation * Vector3.up), Vector3.up);
                        float num3 = Vector3.Angle((Vector3) (avatarOrientation * Vector3.forward), Vector3.forward);
                        if (((num > data.maxAngle) || (num2 > data.maxAngle)) || (num3 > data.maxAngle))
                        {
                            wrapper.bone.rotation = Quaternion.Inverse(avatarOrientation) * wrapper.bone.rotation;
                        }
                    }
                    else
                    {
                        Vector3 vector = GetBoneAlignmentDirection(bones, avatarOrientation, boneIndex);
                        if (vector != Vector3.zero)
                        {
                            Quaternion quaternion = GetRotationSpace(bones, avatarOrientation, boneIndex);
                            Vector3 to = (Vector3) (quaternion * data.direction);
                            if (data.planeNormal != Vector3.zero)
                            {
                                vector = Vector3.ProjectOnPlane(vector, (Vector3) (quaternion * data.planeNormal));
                            }
                            float num4 = Vector3.Angle(vector, to);
                            if (num4 > (data.maxAngle * 0.99f))
                            {
                                Quaternion b = Quaternion.FromToRotation(vector, to);
                                Transform bone = null;
                                Quaternion identity = Quaternion.identity;
                                if ((boneIndex == 1) || (boneIndex == 3))
                                {
                                    bone = bones[5].bone;
                                }
                                if ((boneIndex == 2) || (boneIndex == 4))
                                {
                                    bone = bones[6].bone;
                                }
                                if (bone != null)
                                {
                                    identity = bone.rotation;
                                }
                                float t = Mathf.Clamp01(1.05f - (data.maxAngle / num4));
                                b = Quaternion.Slerp(Quaternion.identity, b, t);
                                wrapper.bone.rotation = b * wrapper.bone.rotation;
                                if (bone != null)
                                {
                                    bone.rotation = identity;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void MakeCharacterPositionValid(BoneWrapper[] bones)
        {
            float num;
            Vector3 characterPositionAdjustVector = GetCharacterPositionAdjustVector(bones, out num);
            if (characterPositionAdjustVector != Vector3.zero)
            {
                bones[0].bone.position += characterPositionAdjustVector;
            }
        }

        public static void MakePoseValid(BoneWrapper[] bones)
        {
            Quaternion avatarOrientation = AvatarComputeOrientation(bones);
            for (int i = 0; i < sBonePoses.Length; i++)
            {
                MakeBoneAlignmentValid(bones, avatarOrientation, i);
                if (i == 0)
                {
                    avatarOrientation = AvatarComputeOrientation(bones);
                }
            }
            MakeCharacterPositionValid(bones);
        }

        public static void SampleBindPose(GameObject go)
        {
            List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>(go.GetComponentsInChildren<SkinnedMeshRenderer>());
            list.Sort(new SkinTransformHierarchySorter());
            foreach (SkinnedMeshRenderer renderer in list)
            {
                Matrix4x4 localToWorldMatrix = renderer.transform.localToWorldMatrix;
                List<Transform> list2 = new List<Transform>(renderer.bones);
                Vector3[] vectorArray = new Vector3[list2.Count];
                for (int i = 0; i < list2.Count; i++)
                {
                    vectorArray[i] = list2[i].localPosition;
                }
                Dictionary<Transform, Transform> dictionary = new Dictionary<Transform, Transform>();
                foreach (Transform transform in list2)
                {
                    dictionary[transform] = transform.parent;
                    transform.parent = null;
                }
                for (int j = 0; j < list2.Count; j++)
                {
                    Vector3 vector;
                    Quaternion quaternion;
                    GetBindPoseBonePositionRotation(localToWorldMatrix, renderer.sharedMesh.bindposes[j], list2[j], out vector, out quaternion);
                    list2[j].position = vector;
                    list2[j].rotation = quaternion;
                }
                foreach (Transform transform2 in list2)
                {
                    transform2.parent = dictionary[transform2];
                }
                for (int k = 0; k < list2.Count; k++)
                {
                    list2[k].localPosition = vectorArray[k];
                }
            }
        }

        public static void ShowBindPose(SkinnedMeshRenderer skin)
        {
            Matrix4x4 localToWorldMatrix = skin.transform.localToWorldMatrix;
            for (int i = 0; i < skin.bones.Length; i++)
            {
                Vector3 vector;
                Quaternion quaternion;
                GetBindPoseBonePositionRotation(localToWorldMatrix, skin.sharedMesh.bindposes[i], skin.bones[i], out vector, out quaternion);
                float handleSize = HandleUtility.GetHandleSize(vector);
                Handles.color = Handles.xAxisColor;
                Handles.DrawLine(vector, vector + ((Vector3) (((quaternion * Vector3.right) * 0.3f) * handleSize)));
                Handles.color = Handles.yAxisColor;
                Handles.DrawLine(vector, vector + ((Vector3) (((quaternion * Vector3.up) * 0.3f) * handleSize)));
                Handles.color = Handles.zAxisColor;
                Handles.DrawLine(vector, vector + ((Vector3) (((quaternion * Vector3.forward) * 0.3f) * handleSize)));
            }
        }

        public static bool TestAndValidateAutoSetup(GameObject modelAsset)
        {
            if (modelAsset == null)
            {
                Debug.LogError("GameObject is null");
                return false;
            }
            if (PrefabUtility.GetPrefabType(modelAsset) != PrefabType.ModelPrefab)
            {
                Debug.LogError(modelAsset.name + ": GameObject is not a ModelPrefab", modelAsset);
                return false;
            }
            if (modelAsset.transform.parent != null)
            {
                Debug.LogError(modelAsset.name + ": GameObject is not the root", modelAsset);
                return false;
            }
            string assetPath = AssetDatabase.GetAssetPath(modelAsset);
            ModelImporter atPath = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (atPath == null)
            {
                Debug.LogError(modelAsset.name + ": Could not load ModelImporter (path:" + assetPath + ")", modelAsset);
                return false;
            }
            SerializedObject serializedObject = new SerializedObject(atPath);
            SerializedProperty property = serializedObject.FindProperty("m_AnimationType");
            if (property == null)
            {
                Debug.LogError(modelAsset.name + ": Could not find property m_AnimationType on ModelImporter", modelAsset);
                return false;
            }
            property.intValue = 2;
            ClearAll(serializedObject);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.ImportAsset(assetPath);
            property.intValue = 3;
            AutoSetupOnInstance(modelAsset, serializedObject);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.ImportAsset(assetPath);
            Avatar avatar = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Avatar)) as Avatar;
            if (avatar == null)
            {
                Debug.LogError(modelAsset.name + ": Did not find Avatar after reimport with CreateAvatar enabled", modelAsset);
                return false;
            }
            if (!avatar.isHuman)
            {
                Debug.LogError(modelAsset.name + ": Avatar is not valid after reimport", modelAsset);
                return false;
            }
            if (!IsPoseValidOnInstance(modelAsset, serializedObject))
            {
                Debug.LogError(modelAsset.name + ": Avatar has invalid pose after reimport", modelAsset);
                return false;
            }
            string str2 = assetPath.Substring(0, assetPath.Length - Path.GetExtension(assetPath).Length);
            string str3 = str2 + ".ht";
            HumanTemplate template = AssetDatabase.LoadMainAssetAtPath(str3) as HumanTemplate;
            if (template == null)
            {
                Debug.LogWarning(modelAsset.name + ": Didn't find template at path " + str3);
            }
            else
            {
                List<string> list = null;
                string path = str2 + ".ignore";
                if (File.Exists(path))
                {
                    list = new List<string>(File.ReadAllLines(path));
                }
                GameObject obj3 = Object.Instantiate<GameObject>(modelAsset);
                obj3.hideFlags = HideFlags.HideAndDontSave;
                Dictionary<Transform, bool> actualBones = GetModelBones(obj3.transform, false, null);
                BoneWrapper[] humanBones = GetHumanBones(serializedObject, actualBones);
                bool flag = false;
                for (int i = 0; i < humanBones.Length; i++)
                {
                    if ((list == null) || !list.Contains(humanBones[i].humanBoneName))
                    {
                        string boneName = template.Find(humanBones[i].humanBoneName);
                        string transformName = (humanBones[i].bone != null) ? humanBones[i].bone.name : string.Empty;
                        if (!AvatarMappingEditor.MatchName(transformName, boneName))
                        {
                            flag = true;
                            Debug.LogError(modelAsset.name + ": Avatar has bone " + humanBones[i].humanBoneName + " mapped to \"" + transformName + "\" but expected \"" + boneName + "\"", modelAsset);
                        }
                    }
                }
                Object.DestroyImmediate(obj3);
                if (flag)
                {
                    return false;
                }
            }
            return true;
        }

        public static void TransferDescriptionToPose(SerializedObject serializedObject, Transform root)
        {
            if (root != null)
            {
                TransferDescriptionToPose(serializedObject, root, true);
            }
        }

        private static void TransferDescriptionToPose(SerializedObject serializedObject, Transform transform, bool isRoot)
        {
            SerializedProperty property = FindSkeletonBone(serializedObject, transform, false, isRoot);
            if ((property != null) && property.FindPropertyRelative(sTransformModified).boolValue)
            {
                SerializedProperty property3 = property.FindPropertyRelative(sPosition);
                SerializedProperty property4 = property.FindPropertyRelative(sRotation);
                SerializedProperty property5 = property.FindPropertyRelative(sScale);
                transform.localPosition = property3.vector3Value;
                transform.localRotation = property4.quaternionValue;
                transform.localScale = property5.vector3Value;
            }
            IEnumerator enumerator = transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    TransferDescriptionToPose(serializedObject, current, false);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        public static void TransferPoseToDescription(SerializedObject serializedObject, Transform root)
        {
            SkeletonBone[] skeletonBones = new SkeletonBone[0];
            if (root != null)
            {
                TransferPoseToDescription(serializedObject, root, true, ref skeletonBones);
            }
            SerializedProperty serializedProperty = serializedObject.FindProperty(sSkeleton);
            ModelImporter.UpdateSkeletonPose(skeletonBones, serializedProperty);
        }

        private static void TransferPoseToDescription(SerializedObject serializedObject, Transform transform, bool isRoot, ref SkeletonBone[] skeletonBones)
        {
            SkeletonBone item = new SkeletonBone {
                name = transform.name,
                position = transform.localPosition,
                rotation = transform.localRotation,
                scale = transform.localScale,
                transformModified = 1
            };
            ArrayUtility.Add<SkeletonBone>(ref skeletonBones, item);
            IEnumerator enumerator = transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    TransferPoseToDescription(serializedObject, current, false, ref skeletonBones);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        private class BonePoseData
        {
            public int[] childIndices;
            public bool compareInGlobalSpace;
            public Vector3 direction;
            public float maxAngle;
            public Vector3 planeNormal;

            public BonePoseData(Vector3 dir, bool globalSpace, float maxAngleDiff)
            {
                this.direction = Vector3.zero;
                this.planeNormal = Vector3.zero;
                this.direction = !(dir == Vector3.zero) ? dir.normalized : dir;
                this.compareInGlobalSpace = globalSpace;
                this.maxAngle = maxAngleDiff;
            }

            public BonePoseData(Vector3 dir, bool globalSpace, float maxAngleDiff, int[] children) : this(dir, globalSpace, maxAngleDiff)
            {
                this.childIndices = children;
            }

            public BonePoseData(Vector3 dir, bool globalSpace, float maxAngleDiff, Vector3 planeNormal, int[] children) : this(dir, globalSpace, maxAngleDiff, children)
            {
                this.planeNormal = planeNormal;
            }
        }

        [Serializable]
        internal class BoneWrapper
        {
            public Transform bone;
            public string error = string.Empty;
            private static Color kBoneDrop = new Color(0.1f, 0.7f, 1f, 1f);
            private static Color kBoneInactive = Color.gray;
            private static Color kBoneInvalid = new Color(1f, 0.3f, 0.25f, 1f);
            private static Color kBoneSelected = new Color(0.4f, 0.7f, 1f, 1f);
            private static Color kBoneValid = new Color(0f, 0.75f, 0f, 1f);
            public const int kIconSize = 0x13;
            private string m_HumanBoneName;
            private static string sBoneName = "m_BoneName";
            private static string sHumanName = "m_HumanName";
            public BoneState state;

            public BoneWrapper(string humanBoneName, SerializedObject serializedObject, Dictionary<Transform, bool> bones)
            {
                this.m_HumanBoneName = humanBoneName;
                this.Reset(serializedObject, bones);
            }

            public void BoneDotGUI(Rect rect, int boneIndex, bool doClickSelect, bool doDragDrop, SerializedObject serializedObject, AvatarMappingEditor editor)
            {
                Texture image;
                int controlID = GUIUtility.GetControlID(FocusType.Passive, rect);
                if (doClickSelect)
                {
                    this.HandleClickSelection(rect, boneIndex);
                }
                if (doDragDrop)
                {
                    this.HandleDragDrop(rect, boneIndex, controlID, serializedObject, editor);
                }
                Color color = GUI.color;
                if (AvatarMappingEditor.s_SelectedBoneIndex == boneIndex)
                {
                    GUI.color = kBoneSelected;
                    GUI.DrawTexture(rect, AvatarMappingEditor.styles.dotSelection.image);
                }
                if (DragAndDrop.activeControlID == controlID)
                {
                    GUI.color = kBoneDrop;
                }
                else if (this.state == BoneState.Valid)
                {
                    GUI.color = kBoneValid;
                }
                else if (this.state == BoneState.None)
                {
                    GUI.color = kBoneInactive;
                }
                else
                {
                    GUI.color = kBoneInvalid;
                }
                if (HumanTrait.RequiredBone(boneIndex))
                {
                    image = AvatarMappingEditor.styles.dotFrame.image;
                }
                else
                {
                    image = AvatarMappingEditor.styles.dotFrameDotted.image;
                }
                GUI.DrawTexture(rect, image);
                if ((this.bone != null) || (DragAndDrop.activeControlID == controlID))
                {
                    GUI.DrawTexture(rect, AvatarMappingEditor.styles.dotFill.image);
                }
                GUI.color = color;
            }

            protected void DeleteSerializedProperty(SerializedObject serializedObject)
            {
                SerializedProperty property = serializedObject.FindProperty(AvatarSetupTool.sHuman);
                if ((property != null) && property.isArray)
                {
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        if (property.GetArrayElementAtIndex(i).FindPropertyRelative(sHumanName).stringValue == this.humanBoneName)
                        {
                            property.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                }
            }

            public SerializedProperty GetSerializedProperty(SerializedObject serializedObject, bool createIfMissing)
            {
                SerializedProperty property = serializedObject.FindProperty(AvatarSetupTool.sHuman);
                if ((property != null) && property.isArray)
                {
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        if (property.GetArrayElementAtIndex(i).FindPropertyRelative(sHumanName).stringValue == this.humanBoneName)
                        {
                            return property.GetArrayElementAtIndex(i);
                        }
                    }
                    if (createIfMissing)
                    {
                        property.arraySize++;
                        SerializedProperty arrayElementAtIndex = property.GetArrayElementAtIndex(property.arraySize - 1);
                        if (arrayElementAtIndex != null)
                        {
                            arrayElementAtIndex.FindPropertyRelative(sHumanName).stringValue = this.humanBoneName;
                            return arrayElementAtIndex;
                        }
                    }
                }
                return null;
            }

            public void HandleClickSelection(Rect selectRect, int boneIndex)
            {
                Event current = Event.current;
                if ((current.type == EventType.MouseDown) && selectRect.Contains(current.mousePosition))
                {
                    AvatarMappingEditor.s_SelectedBoneIndex = boneIndex;
                    AvatarMappingEditor.s_DirtySelection = true;
                    Selection.activeTransform = this.bone;
                    if (this.bone != null)
                    {
                        EditorGUIUtility.PingObject(this.bone);
                    }
                    current.Use();
                }
            }

            private void HandleDragDrop(Rect dropRect, int boneIndex, int id, SerializedObject serializedObject, AvatarMappingEditor editor)
            {
                EventType type = Event.current.type;
                EventType type2 = type;
                switch (type2)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (dropRect.Contains(Event.current.mousePosition) && GUI.enabled)
                        {
                            Object[] objectReferences = DragAndDrop.objectReferences;
                            Object target = (objectReferences.Length != 1) ? null : objectReferences[0];
                            if ((target != null) && ((!(target is Transform) && !(target is GameObject)) || EditorUtility.IsPersistent(target)))
                            {
                                target = null;
                            }
                            if (target != null)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                                if (type == EventType.DragPerform)
                                {
                                    Undo.RegisterCompleteObjectUndo(editor, "Avatar mapping modified");
                                    if (target is GameObject)
                                    {
                                        this.bone = (target as GameObject).transform;
                                    }
                                    else
                                    {
                                        this.bone = target as Transform;
                                    }
                                    this.Serialize(serializedObject);
                                    GUI.changed = true;
                                    DragAndDrop.AcceptDrag();
                                    DragAndDrop.activeControlID = 0;
                                }
                                else
                                {
                                    DragAndDrop.activeControlID = id;
                                }
                                Event.current.Use();
                            }
                        }
                        break;

                    default:
                        if ((type2 == EventType.DragExited) && GUI.enabled)
                        {
                            HandleUtility.Repaint();
                        }
                        break;
                }
            }

            public void Reset(SerializedObject serializedObject, Dictionary<Transform, bool> bones)
            {
                this.bone = null;
                SerializedProperty serializedProperty = this.GetSerializedProperty(serializedObject, false);
                if (serializedProperty != null)
                {
                    <Reset>c__AnonStorey97 storey = new <Reset>c__AnonStorey97 {
                        boneName = serializedProperty.FindPropertyRelative(sBoneName).stringValue
                    };
                    this.bone = bones.Keys.FirstOrDefault<Transform>(new Func<Transform, bool>(storey.<>m__1C1));
                }
                this.state = BoneState.Valid;
            }

            public void Serialize(SerializedObject serializedObject)
            {
                if (this.bone == null)
                {
                    this.DeleteSerializedProperty(serializedObject);
                }
                else
                {
                    SerializedProperty serializedProperty = this.GetSerializedProperty(serializedObject, true);
                    if (serializedProperty != null)
                    {
                        serializedProperty.FindPropertyRelative(sBoneName).stringValue = this.bone.name;
                    }
                }
            }

            public string humanBoneName
            {
                get
                {
                    return this.m_HumanBoneName;
                }
            }

            public string messageName
            {
                get
                {
                    return (ObjectNames.NicifyVariableName(this.m_HumanBoneName) + " Transform '" + ((this.bone == null) ? "None" : this.bone.name) + "'");
                }
            }

            [CompilerGenerated]
            private sealed class <Reset>c__AnonStorey97
            {
                internal string boneName;

                internal bool <>m__1C1(Transform b)
                {
                    return ((b != null) && (b.name == this.boneName));
                }
            }
        }

        private class SkinTransformHierarchySorter : IComparer<SkinnedMeshRenderer>
        {
            public int Compare(SkinnedMeshRenderer skinA, SkinnedMeshRenderer skinB)
            {
                Transform parent = skinA.transform;
                Transform transform2 = skinB.transform;
                while (true)
                {
                    if (parent == null)
                    {
                        if (transform2 == null)
                        {
                            return 0;
                        }
                        return -1;
                    }
                    if (transform2 == null)
                    {
                        return 1;
                    }
                    parent = parent.parent;
                    transform2 = transform2.parent;
                }
            }
        }

        private class TransformHierarchySorter : IComparer<Transform>
        {
            public int Compare(Transform a, Transform b)
            {
                while (true)
                {
                    if (a == null)
                    {
                        if (b == null)
                        {
                            return 0;
                        }
                        return -1;
                    }
                    if (b == null)
                    {
                        return 1;
                    }
                    a = a.parent;
                    b = b.parent;
                }
            }
        }
    }
}

