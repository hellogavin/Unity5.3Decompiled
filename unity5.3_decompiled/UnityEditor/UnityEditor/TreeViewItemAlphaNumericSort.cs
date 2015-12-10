namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    internal class TreeViewItemAlphaNumericSort : IComparer<TreeViewItem>
    {
        public int Compare(TreeViewItem lhs, TreeViewItem rhs)
        {
            if (lhs == rhs)
            {
                return 0;
            }
            if (lhs == null)
            {
                return -1;
            }
            if (rhs == null)
            {
                return 1;
            }
            return EditorUtility.NaturalCompare(lhs.displayName, rhs.displayName);
        }
    }
}

