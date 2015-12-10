namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class FilteredHierarchyProperty : IHierarchyProperty
    {
        private FilteredHierarchy m_Hierarchy;
        private int m_Position = -1;

        public FilteredHierarchyProperty(FilteredHierarchy filter)
        {
            this.m_Hierarchy = filter;
        }

        public int CountRemaining(int[] expanded)
        {
            return ((this.m_Hierarchy.results.Length - this.m_Position) - 1);
        }

        public static IHierarchyProperty CreateHierarchyPropertyForFilter(FilteredHierarchy filteredHierarchy)
        {
            if (filteredHierarchy.searchFilter.GetState() != SearchFilter.State.EmptySearchFilter)
            {
                return new FilteredHierarchyProperty(filteredHierarchy);
            }
            return new HierarchyProperty(filteredHierarchy.hierarchyType);
        }

        public bool Find(int _instanceID, int[] expanded)
        {
            this.Reset();
            while (this.Next(expanded))
            {
                if (this.instanceID == _instanceID)
                {
                    return true;
                }
            }
            return false;
        }

        public int[] FindAllAncestors(int[] instanceIDs)
        {
            return new int[0];
        }

        public bool IsExpanded(int[] expanded)
        {
            return false;
        }

        public bool Next(int[] expanded)
        {
            this.m_Position++;
            return (this.m_Position < this.m_Hierarchy.results.Length);
        }

        public bool NextWithDepthCheck(int[] expanded, int minDepth)
        {
            return this.Next(expanded);
        }

        public bool Parent()
        {
            return false;
        }

        public bool Previous(int[] expanded)
        {
            this.m_Position--;
            return (this.m_Position >= 0);
        }

        public void Reset()
        {
            this.m_Position = -1;
        }

        public bool Skip(int count, int[] expanded)
        {
            this.m_Position += count;
            return (this.m_Position < this.m_Hierarchy.results.Length);
        }

        public int[] ancestors
        {
            get
            {
                return new int[0];
            }
        }

        public int colorCode
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].colorCode;
            }
        }

        public int depth
        {
            get
            {
                return 0;
            }
        }

        public string guid
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].guid;
            }
        }

        public bool hasChildren
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].hasChildren;
            }
        }

        public bool hasFullPreviewImage
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].hasFullPreviewImage;
            }
        }

        public Texture2D icon
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].icon;
            }
        }

        public IconDrawStyle iconDrawStyle
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].iconDrawStyle;
            }
        }

        public int instanceID
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].instanceID;
            }
        }

        public bool isFolder
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].isFolder;
            }
        }

        public bool isMainRepresentation
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].isMainRepresentation;
            }
        }

        public bool isValid
        {
            get
            {
                return (((this.m_Hierarchy.results != null) && (this.m_Position < this.m_Hierarchy.results.Length)) && (this.m_Position >= 0));
            }
        }

        public string name
        {
            get
            {
                return this.m_Hierarchy.results[this.m_Position].name;
            }
        }

        public Object pptrValue
        {
            get
            {
                return EditorUtility.InstanceIDToObject(this.instanceID);
            }
        }

        public int row
        {
            get
            {
                return this.m_Position;
            }
        }
    }
}

