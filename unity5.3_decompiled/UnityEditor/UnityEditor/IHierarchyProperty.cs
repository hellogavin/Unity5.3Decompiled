namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal interface IHierarchyProperty
    {
        int CountRemaining(int[] expanded);
        bool Find(int instanceID, int[] expanded);
        int[] FindAllAncestors(int[] instanceIDs);
        bool IsExpanded(int[] expanded);
        bool Next(int[] expanded);
        bool NextWithDepthCheck(int[] expanded, int minDepth);
        bool Parent();
        bool Previous(int[] expanded);
        void Reset();
        bool Skip(int count, int[] expanded);

        int[] ancestors { get; }

        int colorCode { get; }

        int depth { get; }

        string guid { get; }

        bool hasChildren { get; }

        bool hasFullPreviewImage { get; }

        Texture2D icon { get; }

        IconDrawStyle iconDrawStyle { get; }

        int instanceID { get; }

        bool isFolder { get; }

        bool isMainRepresentation { get; }

        bool isValid { get; }

        string name { get; }

        Object pptrValue { get; }

        int row { get; }
    }
}

