namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SearchUtility
    {
        internal static bool CheckForKeyWords(string searchString, SearchFilter filter, int quote1, int quote2)
        {
            bool flag = false;
            int index = searchString.IndexOf("t:");
            if (index == 0)
            {
                string str = searchString.Substring(index + 2);
                filter.classNames = new List<string>(filter.classNames) { str }.ToArray();
                flag = true;
            }
            index = searchString.IndexOf("l:");
            if (index == 0)
            {
                string str2 = searchString.Substring(index + 2);
                filter.assetLabels = new List<string>(filter.assetLabels) { str2 }.ToArray();
                flag = true;
            }
            index = searchString.IndexOf("b:");
            if (index == 0)
            {
                string str3 = searchString.Substring(index + 2);
                filter.assetBundleNames = new List<string>(filter.assetBundleNames) { str3 }.ToArray();
                flag = true;
            }
            index = searchString.IndexOf("ref:");
            if (index != 0)
            {
                return flag;
            }
            int instanceID = 0;
            int num3 = index + 3;
            int num4 = searchString.IndexOf(':', num3 + 1);
            if (num4 >= 0)
            {
                int num5;
                if (int.TryParse(searchString.Substring(num3 + 1, (num4 - num3) - 1), out num5))
                {
                    instanceID = num5;
                }
            }
            else
            {
                string str5;
                if ((quote1 != -1) && (quote2 != -1))
                {
                    int startIndex = quote1 + 1;
                    int length = (quote2 - quote1) - 1;
                    if ((length < 0) || (quote2 == -1))
                    {
                        length = searchString.Length - startIndex;
                    }
                    str5 = "Assets/" + searchString.Substring(startIndex, length);
                }
                else
                {
                    str5 = "Assets/" + searchString.Substring(num3 + 1);
                }
                Object obj2 = AssetDatabase.LoadMainAssetAtPath(str5);
                if (obj2 != null)
                {
                    instanceID = obj2.GetInstanceID();
                }
            }
            filter.referencingInstanceIDs = new int[] { instanceID };
            return true;
        }

        private static int FindFirstPositionNotOf(string source, string chars)
        {
            if (source != null)
            {
                if (chars == null)
                {
                    return 0;
                }
                if (source.Length == 0)
                {
                    return -1;
                }
                if (chars.Length == 0)
                {
                    return 0;
                }
                for (int i = 0; i < source.Length; i++)
                {
                    if (chars.IndexOf(source[i]) == -1)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        internal static bool ParseSearchString(string searchText, SearchFilter filter)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return false;
            }
            filter.ClearSearch();
            string searchString = string.Copy(searchText);
            RemoveUnwantedWhitespaces(ref searchString);
            bool flag = false;
            int startIndex = FindFirstPositionNotOf(searchString, " \t,");
            if (startIndex == -1)
            {
                startIndex = 0;
            }
            while (startIndex < searchString.Length)
            {
                int length = searchString.IndexOfAny(" \t,".ToCharArray(), startIndex);
                int index = searchString.IndexOf('"', startIndex);
                int num4 = -1;
                if (index != -1)
                {
                    num4 = searchString.IndexOf('"', index + 1);
                    if (num4 != -1)
                    {
                        length = searchString.IndexOfAny(" \t,".ToCharArray(), num4);
                    }
                    else
                    {
                        length = -1;
                    }
                }
                if (length == -1)
                {
                    length = searchString.Length;
                }
                if (length > startIndex)
                {
                    string str3 = searchString.Substring(startIndex, length - startIndex);
                    if (CheckForKeyWords(str3, filter, index, num4))
                    {
                        flag = true;
                    }
                    else
                    {
                        filter.nameFilter = filter.nameFilter + (!string.IsNullOrEmpty(filter.nameFilter) ? " " : string.Empty) + str3;
                    }
                }
                startIndex = length + 1;
            }
            return flag;
        }

        private static void RemoveUnwantedWhitespaces(ref string searchString)
        {
            searchString = searchString.Replace(": ", ":");
        }
    }
}

