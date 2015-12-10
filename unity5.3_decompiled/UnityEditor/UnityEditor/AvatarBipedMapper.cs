namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class AvatarBipedMapper
    {
        private static string[] kBipedHumanBoneNames = new string[] { 
            "Pelvis", "L Thigh", "R Thigh", "L Calf", "R Calf", "L Foot", "R Foot", "Spine", "Spine1", "Neck", "Head", "L Clavicle", "R Clavicle", "L UpperArm", "R UpperArm", "L Forearm", 
            "R Forearm", "L Hand", "R Hand", "L Toe0", "R Toe0", string.Empty, string.Empty, string.Empty, "L Finger0", "L Finger01", "L Finger02", "L Finger1", "L Finger11", "L Finger12", "L Finger2", "L Finger21", 
            "L Finger22", "L Finger3", "L Finger31", "L Finger32", "L Finger4", "L Finger41", "L Finger42", "R Finger0", "R Finger01", "R Finger02", "R Finger1", "R Finger11", "R Finger12", "R Finger2", "R Finger21", "R Finger22", 
            "R Finger3", "R Finger31", "R Finger32", "R Finger4", "R Finger41", "R Finger42"
         };

        public static void BipedPose(GameObject go)
        {
            BipedPose(go.transform);
        }

        private static void BipedPose(Transform t)
        {
            if (t.name.EndsWith("Pelvis"))
            {
                t.localRotation = Quaternion.Euler(270f, 90f, 0f);
                t.parent.localRotation = Quaternion.Euler(270f, 90f, 0f);
            }
            else if (t.name.EndsWith("Thigh"))
            {
                t.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (t.name.EndsWith("Toe0"))
            {
                t.localRotation = Quaternion.Euler(0f, 0f, 270f);
            }
            else if (t.name.EndsWith("L Clavicle"))
            {
                t.localRotation = Quaternion.Euler(0f, 270f, 180f);
            }
            else if (t.name.EndsWith("R Clavicle"))
            {
                t.localRotation = Quaternion.Euler(0f, 90f, 180f);
            }
            else if (t.name.EndsWith("L Hand"))
            {
                t.localRotation = Quaternion.Euler(270f, 0f, 0f);
            }
            else if (t.name.EndsWith("R Hand"))
            {
                t.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }
            else if (t.name.EndsWith("L Finger0"))
            {
                t.localRotation = Quaternion.Euler(0f, 315f, 0f);
            }
            else if (t.name.EndsWith("R Finger0"))
            {
                t.localRotation = Quaternion.Euler(0f, 45f, 0f);
            }
            else
            {
                t.localRotation = Quaternion.identity;
            }
            IEnumerator enumerator = t.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    BipedPose(current);
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

        public static bool IsBiped(Transform root, List<string> report)
        {
            if (report != null)
            {
                report.Clear();
            }
            Transform[] humanToTransform = new Transform[HumanTrait.BoneCount];
            return MapBipedBones(root, ref humanToTransform, report);
        }

        private static Transform MapBipedBone(int boneIndex, Transform transform, Transform parentTransform, List<string> report)
        {
            Transform child = null;
            if (transform != null)
            {
                int childCount = transform.childCount;
                for (int i = 0; (child == null) && (i < childCount); i++)
                {
                    if (transform.GetChild(i).name.EndsWith(kBipedHumanBoneNames[boneIndex]))
                    {
                        child = transform.GetChild(i);
                        if (((child != null) && (report != null)) && ((boneIndex != 0) && (transform != parentTransform)))
                        {
                            string[] textArray1 = new string[] { "- Invalid parent for ", child.name, ".Expected ", parentTransform.name, ", but found ", transform.name, "." };
                            string item = string.Concat(textArray1);
                            if ((boneIndex == 1) || (boneIndex == 2))
                            {
                                item = item + " Disable Triangle Pelvis";
                            }
                            else if ((boneIndex == 11) || (boneIndex == 12))
                            {
                                item = item + " Enable Triangle Neck";
                            }
                            else if (boneIndex == 9)
                            {
                                item = item + " Preferred is two Spine Links";
                            }
                            else if (boneIndex == 10)
                            {
                                item = item + " Preferred is one Neck Links";
                            }
                            item = item + "\n";
                            report.Add(item);
                        }
                    }
                }
                for (int j = 0; (child == null) && (j < childCount); j++)
                {
                    child = MapBipedBone(boneIndex, transform.GetChild(j), parentTransform, report);
                }
            }
            return child;
        }

        private static bool MapBipedBones(Transform root, ref Transform[] humanToTransform, List<string> report)
        {
            for (int i = 0; i < HumanTrait.BoneCount; i++)
            {
                string str = kBipedHumanBoneNames[i];
                int parentBone = HumanTrait.GetParentBone(i);
                bool flag = HumanTrait.RequiredBone(i);
                bool flag2 = (parentBone == -1) || HumanTrait.RequiredBone(parentBone);
                Transform transform = (parentBone == -1) ? root : humanToTransform[parentBone];
                if ((transform == null) && !flag2)
                {
                    parentBone = HumanTrait.GetParentBone(parentBone);
                    transform = (parentBone == -1) ? null : humanToTransform[parentBone];
                }
                if (str != string.Empty)
                {
                    humanToTransform[i] = MapBipedBone(i, transform, transform, report);
                    if ((humanToTransform[i] == null) && flag)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static Dictionary<int, Transform> MapBones(Transform root)
        {
            Dictionary<int, Transform> dictionary = new Dictionary<int, Transform>();
            Transform[] humanToTransform = new Transform[HumanTrait.BoneCount];
            if (MapBipedBones(root, ref humanToTransform, null))
            {
                for (int i = 0; i < HumanTrait.BoneCount; i++)
                {
                    if (humanToTransform[i] != null)
                    {
                        dictionary.Add(i, humanToTransform[i]);
                    }
                }
            }
            return dictionary;
        }
    }
}

