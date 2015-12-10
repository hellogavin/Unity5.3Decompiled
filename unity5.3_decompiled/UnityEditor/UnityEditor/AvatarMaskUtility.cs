namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Animations;

    internal class AvatarMaskUtility
    {
        private static string sBoneName = "m_BoneName";
        private static string sHuman = "m_HumanDescription.m_Human";

        public static string[] GetAvatarHumanTransform(SerializedObject so, string[] refTransformsPath)
        {
            SerializedProperty property = so.FindProperty(sHuman);
            if ((property == null) || !property.isArray)
            {
                return null;
            }
            string[] array = new string[0];
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty property2 = property.GetArrayElementAtIndex(i).FindPropertyRelative(sBoneName);
                ArrayUtility.Add<string>(ref array, property2.stringValue);
            }
            return TokeniseHumanTransformsPath(refTransformsPath, array);
        }

        public static void SetActiveHumanTransforms(AvatarMask mask, string[] humanTransforms)
        {
            for (int i = 0; i < mask.transformCount; i++)
            {
                <SetActiveHumanTransforms>c__AnonStorey83 storey = new <SetActiveHumanTransforms>c__AnonStorey83 {
                    path = mask.GetTransformPath(i)
                };
                if (ArrayUtility.FindIndex<string>(humanTransforms, new Predicate<string>(storey.<>m__140)) != -1)
                {
                    mask.SetTransformActive(i, true);
                }
            }
        }

        private static string[] TokeniseHumanTransformsPath(string[] refTransformsPath, string[] humanTransforms)
        {
            <TokeniseHumanTransformsPath>c__AnonStorey84 storey = new <TokeniseHumanTransformsPath>c__AnonStorey84 {
                humanTransforms = humanTransforms
            };
            if (storey.humanTransforms == null)
            {
                return null;
            }
            string[] array = new string[] { string.Empty };
            <TokeniseHumanTransformsPath>c__AnonStorey85 storey2 = new <TokeniseHumanTransformsPath>c__AnonStorey85 {
                <>f__ref$132 = storey,
                i = 0
            };
            while (storey2.i < storey.humanTransforms.Length)
            {
                int index = ArrayUtility.FindIndex<string>(refTransformsPath, new Predicate<string>(storey2.<>m__141));
                if (index != -1)
                {
                    <TokeniseHumanTransformsPath>c__AnonStorey86 storey3 = new <TokeniseHumanTransformsPath>c__AnonStorey86();
                    int length = array.Length;
                    storey3.path = refTransformsPath[index];
                    while (storey3.path.Length > 0)
                    {
                        if (ArrayUtility.FindIndex<string>(array, new Predicate<string>(storey3.<>m__142)) == -1)
                        {
                            ArrayUtility.Insert<string>(ref array, length, storey3.path);
                        }
                        int num4 = storey3.path.LastIndexOf('/');
                        storey3.path = storey3.path.Substring(0, (num4 == -1) ? 0 : num4);
                    }
                }
                storey2.i++;
            }
            return array;
        }

        public static void UpdateTransformMask(AvatarMask mask, string[] refTransformsPath, string[] humanTransforms)
        {
            <UpdateTransformMask>c__AnonStorey7F storeyf = new <UpdateTransformMask>c__AnonStorey7F {
                refTransformsPath = refTransformsPath
            };
            mask.transformCount = storeyf.refTransformsPath.Length;
            <UpdateTransformMask>c__AnonStorey80 storey = new <UpdateTransformMask>c__AnonStorey80 {
                <>f__ref$127 = storeyf,
                i = 0
            };
            while (storey.i < storeyf.refTransformsPath.Length)
            {
                mask.SetTransformPath(storey.i, storeyf.refTransformsPath[storey.i]);
                bool flag = (humanTransforms != null) ? (ArrayUtility.FindIndex<string>(humanTransforms, new Predicate<string>(storey.<>m__13E)) != -1) : true;
                mask.SetTransformActive(storey.i, flag);
                storey.i++;
            }
        }

        public static void UpdateTransformMask(SerializedProperty transformMask, string[] refTransformsPath, string[] humanTransforms)
        {
            <UpdateTransformMask>c__AnonStorey81 storey = new <UpdateTransformMask>c__AnonStorey81 {
                refTransformsPath = refTransformsPath
            };
            transformMask.ClearArray();
            <UpdateTransformMask>c__AnonStorey82 storey2 = new <UpdateTransformMask>c__AnonStorey82 {
                <>f__ref$129 = storey,
                i = 0
            };
            while (storey2.i < storey.refTransformsPath.Length)
            {
                transformMask.InsertArrayElementAtIndex(storey2.i);
                transformMask.GetArrayElementAtIndex(storey2.i).FindPropertyRelative("m_Path").stringValue = storey.refTransformsPath[storey2.i];
                bool flag = (humanTransforms != null) ? (ArrayUtility.FindIndex<string>(humanTransforms, new Predicate<string>(storey2.<>m__13F)) != -1) : true;
                transformMask.GetArrayElementAtIndex(storey2.i).FindPropertyRelative("m_Weight").floatValue = !flag ? 0f : 1f;
                storey2.i++;
            }
        }

        [CompilerGenerated]
        private sealed class <SetActiveHumanTransforms>c__AnonStorey83
        {
            internal string path;

            internal bool <>m__140(string s)
            {
                return (this.path == s);
            }
        }

        [CompilerGenerated]
        private sealed class <TokeniseHumanTransformsPath>c__AnonStorey84
        {
            internal string[] humanTransforms;
        }

        [CompilerGenerated]
        private sealed class <TokeniseHumanTransformsPath>c__AnonStorey85
        {
            internal AvatarMaskUtility.<TokeniseHumanTransformsPath>c__AnonStorey84 <>f__ref$132;
            internal int i;

            internal bool <>m__141(string s)
            {
                return (this.<>f__ref$132.humanTransforms[this.i] == FileUtil.GetLastPathNameComponent(s));
            }
        }

        [CompilerGenerated]
        private sealed class <TokeniseHumanTransformsPath>c__AnonStorey86
        {
            internal string path;

            internal bool <>m__142(string s)
            {
                return (this.path == s);
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey7F
        {
            internal string[] refTransformsPath;
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey80
        {
            internal AvatarMaskUtility.<UpdateTransformMask>c__AnonStorey7F <>f__ref$127;
            internal int i;

            internal bool <>m__13E(string s)
            {
                return (this.<>f__ref$127.refTransformsPath[this.i] == s);
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey81
        {
            internal string[] refTransformsPath;
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey82
        {
            internal AvatarMaskUtility.<UpdateTransformMask>c__AnonStorey81 <>f__ref$129;
            internal int i;

            internal bool <>m__13F(string s)
            {
                return (this.<>f__ref$129.refTransformsPath[this.i] == s);
            }
        }
    }
}

