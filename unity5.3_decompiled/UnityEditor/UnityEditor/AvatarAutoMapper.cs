namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UnityEngine;

    internal class AvatarAutoMapper
    {
        private static bool kDebug = false;
        private static string[] kEyeKeywords = new string[] { "eye", "ball", "!brow", "!lid", "!pony", "!braid", "!end", "!top", "!nub" };
        private static string[] kFootKeywords = new string[] { "foot", "ankle" };
        private static string[] kHandKeywords = new string[] { "hand", "wrist" };
        private static string[] kHeadKeywords = new string[] { "head" };
        private static string[] kIndexFingerKeywords = new string[] { "ind", "point", "!palm", "!wrist", "!end", "!top", "!nub" };
        private static string[] kJawKeywords = new string[] { "jaw", "open", "!teeth", "!tongue", "!pony", "!braid", "!end", "!top", "!nub" };
        private const string kLeftMatch = @"(^|.*[ \.:_-])[lL]($|[ \.:_-].*)";
        private static string[] kLittleFingerKeywords = new string[] { "lit", "pin", "!palm", "!wrist", "!end", "!top", "!nub" };
        private static string[] kLowerArmKeywords = new string[] { "lo", "fore", "elbow" };
        private static string[] kLowerLegKeywords = new string[] { "lo", "calf", "knee", "shin" };
        private static string[] kMiddleFingerKeywords = new string[] { "mid", "long", "!palm", "!wrist", "!end", "!top", "!nub" };
        private static string[] kNeckKeywords = new string[] { "neck" };
        private const string kRightMatch = @"(^|.*[ \.:_-])[rR]($|[ \.:_-].*)";
        private static string[] kRingFingerKeywords = new string[] { "rin", "!palm", "!wrist", "!end", "!top", "!nub" };
        private static string[] kShoulderKeywords = new string[] { "shoulder", "collar", "clavicle" };
        private static string[] kThumbKeywords = new string[] { "thu", "!palm", "!wrist", "!end", "!top", "!nub" };
        private static string[] kToeKeywords = new string[] { "toe", "!end", "!top", "!nub" };
        private static string[] kUpperArmKeywords = new string[] { "up" };
        private static string[] kUpperLegKeywords = new string[] { "up", "thigh" };
        private Dictionary<string, int> m_BoneHasBadKeywordDict = new Dictionary<string, int>();
        private Dictionary<string, int> m_BoneHasKeywordDict = new Dictionary<string, int>();
        private Dictionary<int, BoneMatch> m_BoneMatchDict = new Dictionary<int, BoneMatch>();
        private BoneMappingItem[] m_MappingData;
        private int m_MappingIndexOffset;
        private Quaternion m_Orientation;
        private bool m_TreatDummyBonesAsReal;
        private Dictionary<Transform, bool> m_ValidBones;
        private static bool s_DidPerformInit = false;
        private static BoneMappingItem[] s_LeftMappingDataHand = new BoneMappingItem[] { new BoneMappingItem(-2, -1, 1, 2, 0f, Side.None, new string[0]), new BoneMappingItem(-1, 0x18, 1, 3, 0f, new Vector3(2f, 0f, 1f), Side.None, kThumbKeywords), new BoneMappingItem(-1, 0x1b, 1, 3, 0f, new Vector3(4f, 0f, 1f), Side.None, kIndexFingerKeywords), new BoneMappingItem(-1, 30, 1, 3, 0f, new Vector3(4f, 0f, 0f), Side.None, kMiddleFingerKeywords), new BoneMappingItem(-1, 0x21, 1, 3, 0f, new Vector3(4f, 0f, -1f), Side.None, kRingFingerKeywords), new BoneMappingItem(-1, 0x24, 1, 3, 0f, new Vector3(4f, 0f, -2f), Side.None, kLittleFingerKeywords), new BoneMappingItem(0x18, 0x19, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x1b, 0x1c, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(30, 0x1f, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x21, 0x22, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x24, 0x25, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x19, 0x1a, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x1c, 0x1d, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x1f, 0x20, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x22, 0x23, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x25, 0x26, 1, 1, 0f, Side.None, false, true, new string[0]) };
        private static BoneMappingItem[] s_MappingDataBody = new BoneMappingItem[] { 
            new BoneMappingItem(-1, 0, 1, 3, 0f, Side.None, new string[0]), new BoneMappingItem(0, 2, 1, 2, 0f, Vector3.right, Side.Right, kUpperLegKeywords), new BoneMappingItem(2, 4, 1, 2, 3f, -Vector3.up, Side.Right, kLowerLegKeywords), new BoneMappingItem(4, 6, 1, 2, 1f, -Vector3.up, Side.Right, kFootKeywords), new BoneMappingItem(6, 20, 1, 2, 0.5f, Vector3.forward, Side.Right, true, true, kToeKeywords), new BoneMappingItem(0, 7, 1, 3, 0f, Vector3.up, Side.None, new string[0]), new BoneMappingItem(7, 8, 0, 3, 1.4f, Vector3.up, Side.None, true, false, new string[0]), new BoneMappingItem(8, 12, 1, 3, 0f, Vector3.right, Side.Right, true, false, kShoulderKeywords), new BoneMappingItem(12, 14, 0, 2, 0.5f, Vector3.right, Side.Right, kUpperArmKeywords), new BoneMappingItem(14, 0x10, 1, 2, 2f, Vector3.right, Side.Right, kLowerArmKeywords), new BoneMappingItem(0x10, 0x12, 1, 2, 1f, Vector3.right, Side.Right, kHandKeywords), new BoneMappingItem(8, 9, 1, 3, 1.8f, Vector3.up, Side.None, true, false, kNeckKeywords), new BoneMappingItem(9, 10, 0, 2, 0.3f, Vector3.up, Side.None, kHeadKeywords), new BoneMappingItem(10, 0x17, 1, 2, 0f, Vector3.forward, Side.None, true, false, kJawKeywords), new BoneMappingItem(10, 0x16, 1, 2, 0f, new Vector3(1f, 1f, 1f), Side.Right, true, false, kEyeKeywords), new BoneMappingItem(0x12, -2, 1, 2, 0f, new Vector3(1f, -1f, 2f), Side.Right, true, false, kThumbKeywords), 
            new BoneMappingItem(0x12, -3, 1, 2, 0f, new Vector3(3f, 0f, 1f), Side.Right, true, false, kIndexFingerKeywords)
         };
        private static BoneMappingItem[] s_RightMappingDataHand = new BoneMappingItem[] { new BoneMappingItem(-2, -1, 1, 2, 0f, Side.None, new string[0]), new BoneMappingItem(-1, 0x27, 1, 3, 0f, new Vector3(2f, 0f, 1f), Side.None, kThumbKeywords), new BoneMappingItem(-1, 0x2a, 1, 3, 0f, new Vector3(4f, 0f, 1f), Side.None, kIndexFingerKeywords), new BoneMappingItem(-1, 0x2d, 1, 3, 0f, new Vector3(4f, 0f, 0f), Side.None, kMiddleFingerKeywords), new BoneMappingItem(-1, 0x30, 1, 3, 0f, new Vector3(4f, 0f, -1f), Side.None, kRingFingerKeywords), new BoneMappingItem(-1, 0x33, 1, 3, 0f, new Vector3(4f, 0f, -2f), Side.None, kLittleFingerKeywords), new BoneMappingItem(0x27, 40, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x2a, 0x2b, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x2d, 0x2e, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x30, 0x31, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x33, 0x34, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(40, 0x29, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x2b, 0x2c, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x2e, 0x2f, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x31, 50, 1, 1, 0f, Side.None, false, true, new string[0]), new BoneMappingItem(0x34, 0x35, 1, 1, 0f, Side.None, false, true, new string[0]) };

        public AvatarAutoMapper(Dictionary<Transform, bool> validBones)
        {
            this.m_ValidBones = validBones;
        }

        private void ApplyMapping(BoneMatch match, Dictionary<int, Transform> mapping)
        {
            if (match.doMap)
            {
                mapping[match.item.bone] = match.bone;
            }
            foreach (BoneMatch match2 in match.children)
            {
                this.ApplyMapping(match2, mapping);
            }
        }

        private int BoneHasBadKeyword(Transform bone, params string[] keywords)
        {
            string key = bone.GetInstanceID() + ":" + string.Concat(keywords);
            if (this.m_BoneHasBadKeywordDict.ContainsKey(key))
            {
                return this.m_BoneHasBadKeywordDict[key];
            }
            int num = 0;
            Transform parent = bone.parent;
            while (((parent.parent != null) && this.m_ValidBones.ContainsKey(parent)) && !this.m_ValidBones[parent])
            {
                parent = parent.parent;
            }
            string str2 = this.GetStrippedAndNiceBoneName(parent).ToLower();
            foreach (string str3 in keywords)
            {
                if ((str3[0] != '!') && str2.Contains(str3))
                {
                    num = -20;
                    this.m_BoneHasBadKeywordDict[key] = num;
                    return num;
                }
            }
            str2 = this.GetStrippedAndNiceBoneName(bone).ToLower();
            foreach (string str4 in keywords)
            {
                if ((str4[0] == '!') && str2.Contains(str4.Substring(1)))
                {
                    num = -1000;
                    this.m_BoneHasBadKeywordDict[key] = num;
                    return num;
                }
            }
            this.m_BoneHasBadKeywordDict[key] = num;
            return num;
        }

        private int BoneHasKeyword(Transform bone, params string[] keywords)
        {
            string key = bone.GetInstanceID() + ":" + string.Concat(keywords);
            if (this.m_BoneHasKeywordDict.ContainsKey(key))
            {
                return this.m_BoneHasKeywordDict[key];
            }
            int num = 0;
            string str2 = this.GetStrippedAndNiceBoneName(bone).ToLower();
            foreach (string str3 in keywords)
            {
                if ((str3[0] != '!') && str2.Contains(str3))
                {
                    num = 20;
                    this.m_BoneHasKeywordDict[key] = num;
                    return num;
                }
            }
            this.m_BoneHasKeywordDict[key] = num;
            return num;
        }

        private void DebugMatchChoice(List<BoneMatch> matches)
        {
            string str3;
            string message = this.GetNameOfBone(matches[0].item.bone) + " preferred order: ";
            for (int i = 0; i < matches.Count; i++)
            {
                str3 = message;
                string[] textArray1 = new string[] { str3, matches[i].bone.name, " (", matches[i].score.ToString("0.0"), " / ", matches[i].totalSiblingScore.ToString("0.0"), "), " };
                message = string.Concat(textArray1);
            }
            foreach (BoneMatch match in matches)
            {
                str3 = message;
                string[] textArray2 = new string[] { str3, "\n   Match ", match.bone.name, " (", match.score.ToString("0.0"), " / ", match.totalSiblingScore.ToString("0.0"), "):" };
                message = string.Concat(textArray2);
                foreach (string str2 in match.debugTracker)
                {
                    message = message + "\n    - " + str2;
                }
            }
            Debug.Log(message);
        }

        private void EvaluateBoneMatch(BoneMatch match, bool confirmedChoice)
        {
            match.score = 0f;
            match.siblingScore = 0f;
            List<List<BoneMatch>> childMatchesLists = new List<List<BoneMatch>>();
            int num = 0;
            foreach (int num2 in match.item.GetChildren(this.m_MappingData))
            {
                BoneMappingItem goalItem = this.m_MappingData[num2];
                if (goalItem.parent == match.item.bone)
                {
                    num++;
                    List<BoneMatch> item = this.RecursiveFindPotentialBoneMatches(match, goalItem, confirmedChoice);
                    if ((item != null) && (item.Count != 0))
                    {
                        childMatchesLists.Add(item);
                    }
                }
            }
            bool flag = match.bone == match.humanBoneParent.bone;
            int num4 = 0;
            if (childMatchesLists.Count > 0)
            {
                match.children = this.GetBestChildMatches(match, childMatchesLists);
                foreach (BoneMatch match2 in match.children)
                {
                    if (kDebug && confirmedChoice)
                    {
                        this.EvaluateBoneMatch(match2, confirmedChoice);
                    }
                    num4++;
                    match.score += match2.score;
                    if (kDebug)
                    {
                        match.debugTracker.AddRange(match2.debugTracker);
                    }
                    if ((match2.bone == match.bone) && (match2.item.bone >= 0))
                    {
                        flag = true;
                    }
                }
            }
            if (!match.item.optional || !flag)
            {
                this.ScoreBoneMatch(match);
            }
            if (match.item.dir != Vector3.zero)
            {
                Vector3 dir = match.item.dir;
                if ((this.m_MappingIndexOffset >= 0x18) && (this.m_MappingIndexOffset < 0x27))
                {
                    dir.x *= -1f;
                }
                Vector3 vector3 = match.bone.position - match.humanBoneParent.bone.position;
                Vector3 normalized = vector3.normalized;
                normalized = (Vector3) (Quaternion.Inverse(this.m_Orientation) * normalized);
                float num5 = Vector3.Dot(normalized, dir) * (!match.item.optional ? ((float) 10) : ((float) 5));
                match.siblingScore += num5;
                if (kDebug)
                {
                    object[] objArray1 = new object[9];
                    objArray1[0] = "* ";
                    objArray1[1] = num5;
                    objArray1[2] = ": ";
                    objArray1[3] = this.GetMatchString(match);
                    objArray1[4] = " matched dir (";
                    Vector3 vector4 = match.bone.position - match.humanBoneParent.bone.position;
                    objArray1[5] = vector4.normalized;
                    objArray1[6] = " , ";
                    objArray1[7] = dir;
                    objArray1[8] = ")";
                    match.debugTracker.Add(string.Concat(objArray1));
                }
                if (num5 > 0f)
                {
                    match.score += 10f;
                    if (kDebug)
                    {
                        object[] objArray2 = new object[8];
                        objArray2[0] = 10;
                        objArray2[1] = ": ";
                        objArray2[2] = this.GetMatchString(match);
                        objArray2[3] = " matched dir (";
                        Vector3 vector5 = match.bone.position - match.humanBoneParent.bone.position;
                        objArray2[4] = vector5.normalized;
                        objArray2[5] = " , ";
                        objArray2[6] = dir;
                        objArray2[7] = ")";
                        match.debugTracker.Add(string.Concat(objArray2));
                    }
                }
            }
            if (this.m_MappingIndexOffset == 0)
            {
                int boneSideMatchPoints = this.GetBoneSideMatchPoints(match);
                if ((match.parent.item.side == Side.None) || (boneSideMatchPoints < 0))
                {
                    match.siblingScore += boneSideMatchPoints;
                    if (kDebug)
                    {
                        match.debugTracker.Add(string.Concat(new object[] { "* ", boneSideMatchPoints, ": ", this.GetMatchString(match), " matched side" }));
                    }
                }
            }
            if (match.score > 0f)
            {
                if (match.item.optional && !flag)
                {
                    match.score += 5f;
                    if (kDebug)
                    {
                        match.debugTracker.Add(string.Concat(new object[] { 5, ": ", this.GetMatchString(match), " optional bone is included" }));
                    }
                }
                if ((num == 0) && (match.bone.childCount > 0))
                {
                    match.score++;
                    if (kDebug)
                    {
                        match.debugTracker.Add(string.Concat(new object[] { 1, ": ", this.GetMatchString(match), " has dummy child bone" }));
                    }
                }
                if (match.item.lengthRatio != 0f)
                {
                    float num7 = Vector3.Distance(match.bone.position, match.humanBoneParent.bone.position);
                    if ((num7 == 0f) && (match.bone != match.humanBoneParent.bone))
                    {
                        match.score -= 1000f;
                        if (kDebug)
                        {
                            match.debugTracker.Add(string.Concat(new object[] { -1000, ": ", this.GetMatchString(match.humanBoneParent), " has zero length" }));
                        }
                    }
                    float num8 = Vector3.Distance(match.humanBoneParent.bone.position, match.humanBoneParent.humanBoneParent.bone.position);
                    if (num8 > 0f)
                    {
                        float num9 = Mathf.Log(num7 / num8, 2f);
                        float num10 = Mathf.Log(match.item.lengthRatio, 2f);
                        float num11 = 10f * Mathf.Clamp((float) (1f - (0.6f * Mathf.Abs((float) (num9 - num10)))), (float) 0f, (float) 1f);
                        match.score += num11;
                        if (kDebug)
                        {
                            match.debugTracker.Add(string.Concat(new object[] { num11, ": parent ", this.GetMatchString(match.humanBoneParent), " matched lengthRatio - ", num7, " / ", num8, " = ", num7 / num8, " (", num9, ") goal: ", match.item.lengthRatio, " (", num10, ")" }));
                        }
                    }
                }
            }
            if ((match.item.bone >= 0) && (!match.item.optional || !flag))
            {
                match.doMap = true;
            }
        }

        private int[] GetBestChildMatchChoices(BoneMatch parentMatch, List<List<BoneMatch>> childMatchesLists, int[] choices, out float score)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < choices.Length; i++)
            {
                if (choices[i] >= 0)
                {
                    list.Clear();
                    list.Add(i);
                    for (int k = i + 1; k < choices.Length; k++)
                    {
                        if ((choices[k] >= 0) && this.ShareTransformPath(parentMatch.bone, childMatchesLists[i][choices[i]].bone, childMatchesLists[k][choices[k]].bone))
                        {
                            list.Add(k);
                        }
                    }
                    if (list.Count > 1)
                    {
                        break;
                    }
                }
            }
            if (list.Count <= 1)
            {
                score = 0f;
                for (int m = 0; m < choices.Length; m++)
                {
                    if (choices[m] >= 0)
                    {
                        score += childMatchesLists[m][choices[m]].totalSiblingScore;
                    }
                }
                return choices;
            }
            float num4 = 0f;
            int[] numArray = choices;
            for (int j = 0; j < list.Count; j++)
            {
                float num7;
                int[] destinationArray = new int[choices.Length];
                Array.Copy(choices, destinationArray, choices.Length);
                for (int n = 0; n < list.Count; n++)
                {
                    if (j != n)
                    {
                        if (list[n] >= destinationArray.Length)
                        {
                            Debug.LogError(string.Concat(new object[] { "sharedIndices[j] (", list[n], ") >= altChoices.Length (", destinationArray.Length, ")" }));
                        }
                        if (list[n] >= childMatchesLists.Count)
                        {
                            Debug.LogError(string.Concat(new object[] { "sharedIndices[j] (", list[n], ") >= childMatchesLists.Count (", childMatchesLists.Count, ")" }));
                        }
                        if (destinationArray[list[n]] < (childMatchesLists[list[n]].Count - 1))
                        {
                            destinationArray[list[n]]++;
                        }
                        else
                        {
                            destinationArray[list[n]] = -1;
                        }
                    }
                }
                destinationArray = this.GetBestChildMatchChoices(parentMatch, childMatchesLists, destinationArray, out num7);
                if (num7 > num4)
                {
                    num4 = num7;
                    numArray = destinationArray;
                }
            }
            score = num4;
            return numArray;
        }

        private List<BoneMatch> GetBestChildMatches(BoneMatch parentMatch, List<List<BoneMatch>> childMatchesLists)
        {
            float num;
            List<BoneMatch> list = new List<BoneMatch>();
            if (childMatchesLists.Count == 1)
            {
                list.Add(childMatchesLists[0][0]);
                return list;
            }
            int[] choices = new int[childMatchesLists.Count];
            choices = this.GetBestChildMatchChoices(parentMatch, childMatchesLists, choices, out num);
            for (int i = 0; i < choices.Length; i++)
            {
                if (choices[i] >= 0)
                {
                    list.Add(childMatchesLists[i][choices[i]]);
                }
            }
            return list;
        }

        private BoneMappingItem GetBoneMappingItem(int bone)
        {
            foreach (BoneMappingItem item in this.m_MappingData)
            {
                if (item.bone == bone)
                {
                    return item;
                }
            }
            return new BoneMappingItem();
        }

        private int GetBoneSideMatchPoints(BoneMatch match)
        {
            string name = match.bone.name;
            if ((match.item.side == Side.None) && (this.MatchesSideKeywords(name, false) || this.MatchesSideKeywords(name, true)))
            {
                return -1000;
            }
            bool left = match.item.side == Side.Left;
            if (this.MatchesSideKeywords(name, left))
            {
                return 15;
            }
            if (this.MatchesSideKeywords(name, !left))
            {
                return -1000;
            }
            return 0;
        }

        private static int GetLeftBoneIndexFromRight(int rightIndex)
        {
            if (rightIndex < 0)
            {
                return rightIndex;
            }
            if (rightIndex < 0x36)
            {
                string str = Enum.GetName(typeof(HumanBodyBones), rightIndex).Replace("Right", "Left");
                return (int) Enum.Parse(typeof(HumanBodyBones), str);
            }
            return ((rightIndex + 0x18) - 0x27);
        }

        private int GetMatchKey(BoneMatch parentMatch, Transform t, BoneMappingItem goalItem)
        {
            int num = goalItem.bone + (t.GetInstanceID() * 0x3e8);
            if (parentMatch != null)
            {
                num += parentMatch.bone.GetInstanceID() * 0xf4240;
                if (parentMatch.parent != null)
                {
                    num += parentMatch.parent.bone.GetInstanceID() * 0x3b9aca00;
                }
            }
            return num;
        }

        private string GetMatchString(BoneMatch match)
        {
            return (this.GetNameOfBone(match.item.bone) + ":" + ((match.bone != null) ? match.bone.name : "null"));
        }

        private string GetNameOfBone(int boneIndex)
        {
            if (boneIndex < 0)
            {
                return (string.Empty + boneIndex);
            }
            return (string.Empty + ((HumanBodyBones) boneIndex));
        }

        private string GetStrippedAndNiceBoneName(Transform bone)
        {
            char[] separator = new char[] { ':' };
            string[] strArray = bone.name.Split(separator);
            return ObjectNames.NicifyVariableName(strArray[strArray.Length - 1]);
        }

        public static void InitGlobalMappingData()
        {
            if (!s_DidPerformInit)
            {
                List<BoneMappingItem> list = new List<BoneMappingItem>(s_MappingDataBody);
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    BoneMappingItem item = list[i];
                    if (item.side == Side.Right)
                    {
                        int leftBoneIndexFromRight = GetLeftBoneIndexFromRight(item.bone);
                        int parent = GetLeftBoneIndexFromRight(item.parent);
                        list.Add(new BoneMappingItem(parent, leftBoneIndexFromRight, item.minStep, item.maxStep, item.lengthRatio, new Vector3(-item.dir.x, item.dir.y, item.dir.z), Side.Left, item.optional, item.alwaysInclude, item.keywords));
                    }
                }
                s_MappingDataBody = list.ToArray();
                for (int j = 0; j < s_MappingDataBody.Length; j++)
                {
                    s_MappingDataBody[j].GetChildren(s_MappingDataBody);
                }
                for (int k = 0; k < s_LeftMappingDataHand.Length; k++)
                {
                    s_LeftMappingDataHand[k].GetChildren(s_LeftMappingDataHand);
                }
                for (int m = 0; m < s_RightMappingDataHand.Length; m++)
                {
                    s_RightMappingDataHand[m].GetChildren(s_RightMappingDataHand);
                }
                s_DidPerformInit = true;
            }
        }

        private bool IsParentOfOther(Transform knownCommonParent, Transform potentialParent, Transform potentialChild)
        {
            for (Transform transform = potentialChild; transform != knownCommonParent; transform = transform.parent)
            {
                if (transform == potentialParent)
                {
                    return true;
                }
                if (transform == knownCommonParent)
                {
                    return false;
                }
            }
            return false;
        }

        public Dictionary<int, Transform> MapBones(Transform root)
        {
            InitGlobalMappingData();
            Dictionary<int, Transform> mapping = new Dictionary<int, Transform>();
            this.m_Orientation = Quaternion.identity;
            this.m_MappingData = s_MappingDataBody;
            this.m_MappingIndexOffset = 0;
            this.m_BoneMatchDict.Clear();
            BoneMatch rootMatch = new BoneMatch(null, root, this.m_MappingData[0]);
            this.m_TreatDummyBonesAsReal = false;
            this.MapBonesFromRootDown(rootMatch, mapping);
            if (mapping.Count < 15)
            {
                this.m_TreatDummyBonesAsReal = true;
                this.MapBonesFromRootDown(rootMatch, mapping);
            }
            if ((mapping.ContainsKey(1) && mapping.ContainsKey(2)) && (mapping.ContainsKey(13) && mapping.ContainsKey(14)))
            {
                this.m_Orientation = AvatarSetupTool.AvatarComputeOrientation(mapping[1].position, mapping[2].position, mapping[13].position, mapping[14].position);
                if ((Vector3.Angle((Vector3) (this.m_Orientation * Vector3.up), Vector3.up) > 20f) || (Vector3.Angle((Vector3) (this.m_Orientation * Vector3.forward), Vector3.forward) > 20f))
                {
                    if (kDebug)
                    {
                        Debug.Log("*** Mapping with new computed orientation");
                    }
                    mapping.Clear();
                    this.m_BoneMatchDict.Clear();
                    this.MapBonesFromRootDown(rootMatch, mapping);
                }
            }
            if ((!(!this.m_ValidBones.ContainsKey(root) ? false : this.m_ValidBones[root]) && (mapping.Count > 0)) && mapping.ContainsKey(0))
            {
                while (true)
                {
                    Transform parent = mapping[0].parent;
                    if (((parent == null) || (parent == rootMatch.bone)) || (!this.m_ValidBones.ContainsKey(parent) || !this.m_ValidBones[parent]))
                    {
                        break;
                    }
                    mapping[0] = parent;
                }
            }
            int num = 3;
            Quaternion orientation = this.m_Orientation;
            if (mapping.ContainsKey(0x11))
            {
                Transform bone = mapping[15];
                Transform transform3 = mapping[0x11];
                this.m_Orientation = Quaternion.FromToRotation((Vector3) (orientation * -Vector3.right), transform3.position - bone.position) * orientation;
                this.m_MappingData = s_LeftMappingDataHand;
                this.m_MappingIndexOffset = 0x18;
                this.m_BoneMatchDict.Clear();
                BoneMatch match2 = new BoneMatch(null, bone, this.m_MappingData[0]);
                this.m_TreatDummyBonesAsReal = true;
                int count = mapping.Count;
                this.MapBonesFromRootDown(match2, mapping);
                if (mapping.Count < (count + num))
                {
                    for (int i = 0x18; i <= 0x26; i++)
                    {
                        mapping.Remove(i);
                    }
                }
            }
            if (mapping.ContainsKey(0x12))
            {
                Transform transform4 = mapping[0x10];
                Transform transform5 = mapping[0x12];
                this.m_Orientation = Quaternion.FromToRotation((Vector3) (orientation * Vector3.right), transform5.position - transform4.position) * orientation;
                this.m_MappingData = s_RightMappingDataHand;
                this.m_MappingIndexOffset = 0x27;
                this.m_BoneMatchDict.Clear();
                BoneMatch match3 = new BoneMatch(null, transform4, this.m_MappingData[0]);
                this.m_TreatDummyBonesAsReal = true;
                int num4 = mapping.Count;
                this.MapBonesFromRootDown(match3, mapping);
                if (mapping.Count >= (num4 + num))
                {
                    return mapping;
                }
                for (int j = 0x27; j <= 0x35; j++)
                {
                    mapping.Remove(j);
                }
            }
            return mapping;
        }

        public static Dictionary<int, Transform> MapBones(Transform root, Dictionary<Transform, bool> validBones)
        {
            AvatarAutoMapper mapper = new AvatarAutoMapper(validBones);
            return mapper.MapBones(root);
        }

        private void MapBonesFromRootDown(BoneMatch rootMatch, Dictionary<int, Transform> mapping)
        {
            List<BoneMatch> list = this.RecursiveFindPotentialBoneMatches(rootMatch, this.m_MappingData[0], true);
            if ((list != null) && (list.Count > 0))
            {
                if (kDebug)
                {
                    this.EvaluateBoneMatch(list[0], true);
                }
                this.ApplyMapping(list[0], mapping);
            }
        }

        private bool MatchesSideKeywords(string boneName, bool left)
        {
            return (boneName.ToLower().Contains(!left ? "right" : "left") || (Regex.Match(boneName, !left ? @"(^|.*[ \.:_-])[rR]($|[ \.:_-].*)" : @"(^|.*[ \.:_-])[lL]($|[ \.:_-].*)").Length > 0));
        }

        private List<BoneMatch> RecursiveFindPotentialBoneMatches(BoneMatch parentMatch, BoneMappingItem goalItem, bool confirmedChoice)
        {
            List<BoneMatch> matches = new List<BoneMatch>();
            Queue<QueuedBone> queue = new Queue<QueuedBone>();
            queue.Enqueue(new QueuedBone(parentMatch.bone, 0));
            while (queue.Count > 0)
            {
                QueuedBone bone = queue.Dequeue();
                Transform key = bone.bone;
                if ((bone.level >= goalItem.minStep) && ((this.m_TreatDummyBonesAsReal || (this.m_ValidBones == null)) || (this.m_ValidBones.ContainsKey(key) && this.m_ValidBones[key])))
                {
                    BoneMatch match;
                    int num = this.GetMatchKey(parentMatch, key, goalItem);
                    if (this.m_BoneMatchDict.ContainsKey(num))
                    {
                        match = this.m_BoneMatchDict[num];
                    }
                    else
                    {
                        match = new BoneMatch(parentMatch, key, goalItem);
                        this.EvaluateBoneMatch(match, false);
                        this.m_BoneMatchDict[num] = match;
                    }
                    if ((match.score > 0f) || kDebug)
                    {
                        matches.Add(match);
                    }
                }
                if (bone.level < goalItem.maxStep)
                {
                    IEnumerator enumerator = key.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            Transform current = (Transform) enumerator.Current;
                            if ((this.m_ValidBones == null) || this.m_ValidBones.ContainsKey(current))
                            {
                                if ((!this.m_TreatDummyBonesAsReal && (this.m_ValidBones != null)) && !this.m_ValidBones[current])
                                {
                                    queue.Enqueue(new QueuedBone(current, bone.level));
                                }
                                else
                                {
                                    queue.Enqueue(new QueuedBone(current, bone.level + 1));
                                }
                            }
                        }
                        continue;
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
            }
            if (matches.Count == 0)
            {
                return null;
            }
            matches.Sort();
            if (matches[0].score <= 0f)
            {
                return null;
            }
            if (kDebug && confirmedChoice)
            {
                this.DebugMatchChoice(matches);
            }
            while (matches.Count > 3)
            {
                matches.RemoveAt(matches.Count - 1);
            }
            matches.TrimExcess();
            return matches;
        }

        private void ScoreBoneMatch(BoneMatch match)
        {
            int num = this.BoneHasBadKeyword(match.bone, match.item.keywords);
            match.score += num;
            if (kDebug && (num != 0))
            {
                match.debugTracker.Add(string.Concat(new object[] { num, ": ", this.GetMatchString(match), " matched bad keywords" }));
            }
            if (num >= 0)
            {
                int num2 = this.BoneHasKeyword(match.bone, match.item.keywords);
                match.score += num2;
                if (kDebug && (num2 != 0))
                {
                    match.debugTracker.Add(string.Concat(new object[] { num2, ": ", this.GetMatchString(match), " matched keywords" }));
                }
                if ((match.item.keywords.Length == 0) && match.item.alwaysInclude)
                {
                    match.score++;
                    if (kDebug)
                    {
                        match.debugTracker.Add(string.Concat(new object[] { 1, ": ", this.GetMatchString(match), " always-include point" }));
                    }
                }
            }
        }

        private bool ShareTransformPath(Transform commonParent, Transform childA, Transform childB)
        {
            return (this.IsParentOfOther(commonParent, childA, childB) || this.IsParentOfOther(commonParent, childB, childA));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BoneMappingItem
        {
            public int parent;
            public int bone;
            public int minStep;
            public int maxStep;
            public float lengthRatio;
            public Vector3 dir;
            public AvatarAutoMapper.Side side;
            public bool optional;
            public bool alwaysInclude;
            public string[] keywords;
            private int[] children;
            public BoneMappingItem(int parent, int bone, int minStep, int maxStep, float lengthRatio, Vector3 dir, AvatarAutoMapper.Side side, bool optional, bool alwaysInclude, params string[] keywords)
            {
                this.parent = parent;
                this.bone = bone;
                this.minStep = minStep;
                this.maxStep = maxStep;
                this.lengthRatio = lengthRatio;
                this.dir = dir;
                this.side = side;
                this.optional = optional;
                this.alwaysInclude = alwaysInclude;
                this.keywords = keywords;
                this.children = null;
            }

            public BoneMappingItem(int parent, int bone, int minStep, int maxStep, float lengthRatio, AvatarAutoMapper.Side side, bool optional, bool alwaysInclude, params string[] keywords) : this(parent, bone, minStep, maxStep, lengthRatio, Vector3.zero, side, optional, alwaysInclude, keywords)
            {
            }

            public BoneMappingItem(int parent, int bone, int minStep, int maxStep, float lengthRatio, Vector3 dir, AvatarAutoMapper.Side side, params string[] keywords) : this(parent, bone, minStep, maxStep, lengthRatio, dir, side, false, false, keywords)
            {
            }

            public BoneMappingItem(int parent, int bone, int minStep, int maxStep, float lengthRatio, AvatarAutoMapper.Side side, params string[] keywords) : this(parent, bone, minStep, maxStep, lengthRatio, Vector3.zero, side, false, false, keywords)
            {
            }

            public int[] GetChildren(AvatarAutoMapper.BoneMappingItem[] mappingData)
            {
                if (this.children == null)
                {
                    List<int> list = new List<int>();
                    for (int i = 0; i < mappingData.Length; i++)
                    {
                        if (mappingData[i].parent == this.bone)
                        {
                            list.Add(i);
                        }
                    }
                    this.children = list.ToArray();
                }
                return this.children;
            }
        }

        private class BoneMatch : IComparable<AvatarAutoMapper.BoneMatch>
        {
            public Transform bone;
            public List<AvatarAutoMapper.BoneMatch> children = new List<AvatarAutoMapper.BoneMatch>();
            public List<string> debugTracker = new List<string>();
            public bool doMap;
            public AvatarAutoMapper.BoneMappingItem item;
            public AvatarAutoMapper.BoneMatch parent;
            public float score;
            public float siblingScore;

            public BoneMatch(AvatarAutoMapper.BoneMatch parent, Transform bone, AvatarAutoMapper.BoneMappingItem item)
            {
                this.parent = parent;
                this.bone = bone;
                this.item = item;
            }

            public int CompareTo(AvatarAutoMapper.BoneMatch other)
            {
                if (other == null)
                {
                    return 1;
                }
                return other.totalSiblingScore.CompareTo(this.totalSiblingScore);
            }

            public AvatarAutoMapper.BoneMatch humanBoneParent
            {
                get
                {
                    AvatarAutoMapper.BoneMatch parent = this.parent;
                    while ((parent.parent != null) && (parent.item.bone < 0))
                    {
                        parent = parent.parent;
                    }
                    return parent;
                }
            }

            public float totalSiblingScore
            {
                get
                {
                    return (this.score + this.siblingScore);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct QueuedBone
        {
            public Transform bone;
            public int level;
            public QueuedBone(Transform bone, int level)
            {
                this.bone = bone;
                this.level = level;
            }
        }

        private enum Side
        {
            None,
            Left,
            Right
        }
    }
}

