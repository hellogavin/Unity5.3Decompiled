namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class BaseHierarchySort : IComparer<GameObject>
    {
        protected BaseHierarchySort()
        {
        }

        public virtual int Compare(GameObject lhs, GameObject rhs)
        {
            return 0;
        }

        public virtual GUIContent content
        {
            get
            {
                return null;
            }
        }
    }
}

