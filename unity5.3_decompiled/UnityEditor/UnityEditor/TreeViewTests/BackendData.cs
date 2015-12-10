namespace UnityEditor.TreeViewTests
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class BackendData
    {
        private const int k_MaxChildren = 15;
        private const int k_MaxDepth = 12;
        private const int k_MinChildren = 3;
        private const float k_ProbOfLastDescendent = 0.5f;
        private int m_MaxItems = 0x2710;
        public bool m_RecursiveFindParentsBelow = true;
        private Foo m_Root;

        private void AddChildrenRecursive(Foo foo, int numChildren, bool force)
        {
            if (((this.IDCounter <= this.m_MaxItems) && (foo.depth < 12)) && (force || (Random.value >= 0.5f)))
            {
                if (foo.children == null)
                {
                    foo.children = new List<Foo>(numChildren);
                }
                for (int i = 0; i < numChildren; i++)
                {
                    int num2;
                    this.IDCounter = num2 = this.IDCounter + 1;
                    Foo item = new Foo("Tud" + this.IDCounter, foo.depth + 1, num2) {
                        parent = foo
                    };
                    foo.children.Add(item);
                }
                if (this.IDCounter <= this.m_MaxItems)
                {
                    foreach (Foo foo3 in foo.children)
                    {
                        this.AddChildrenRecursive(foo3, Random.Range(3, 15), false);
                    }
                }
            }
        }

        public static Foo FindNodeRecursive(Foo item, int id)
        {
            if (item != null)
            {
                if (item.id == id)
                {
                    return item;
                }
                if (item.children == null)
                {
                    return null;
                }
                foreach (Foo foo in item.children)
                {
                    Foo foo2 = FindNodeRecursive(foo, id);
                    if (foo2 != null)
                    {
                        return foo2;
                    }
                }
            }
            return null;
        }

        public void GenerateData(int maxNumItems)
        {
            this.m_MaxItems = maxNumItems;
            this.IDCounter = 1;
            this.m_Root = new Foo("Root", 0, 0);
            while (this.IDCounter < this.m_MaxItems)
            {
                this.AddChildrenRecursive(this.m_Root, Random.Range(3, 15), true);
            }
        }

        public HashSet<int> GetParentsBelow(int id)
        {
            Foo searchFromThis = FindNodeRecursive(this.root, id);
            if (searchFromThis == null)
            {
                return new HashSet<int>();
            }
            if (this.m_RecursiveFindParentsBelow)
            {
                return this.GetParentsBelowRecursive(searchFromThis);
            }
            return this.GetParentsBelowStackBased(searchFromThis);
        }

        private HashSet<int> GetParentsBelowRecursive(Foo searchFromThis)
        {
            HashSet<int> parentIDs = new HashSet<int>();
            GetParentsBelowRecursive(searchFromThis, parentIDs);
            return parentIDs;
        }

        private static void GetParentsBelowRecursive(Foo item, HashSet<int> parentIDs)
        {
            if (item.hasChildren)
            {
                parentIDs.Add(item.id);
                foreach (Foo foo in item.children)
                {
                    GetParentsBelowRecursive(foo, parentIDs);
                }
            }
        }

        private HashSet<int> GetParentsBelowStackBased(Foo searchFromThis)
        {
            Stack<Foo> stack = new Stack<Foo>();
            stack.Push(searchFromThis);
            HashSet<int> set = new HashSet<int>();
            while (stack.Count > 0)
            {
                Foo foo = stack.Pop();
                if (foo.hasChildren)
                {
                    set.Add(foo.id);
                    foreach (Foo foo2 in foo.children)
                    {
                        stack.Push(foo2);
                    }
                }
            }
            return set;
        }

        public void ReparentSelection(Foo parentItem, Foo insertAfterItem, List<Foo> draggedItems)
        {
            foreach (Foo foo in draggedItems)
            {
                foo.parent.children.Remove(foo);
                foo.parent = parentItem;
            }
            if (!parentItem.hasChildren)
            {
                parentItem.children = new List<Foo>();
            }
            List<Foo> list = new List<Foo>(parentItem.children);
            int index = 0;
            if (parentItem == insertAfterItem)
            {
                index = 0;
            }
            else
            {
                int num2 = parentItem.children.IndexOf(insertAfterItem);
                if (num2 >= 0)
                {
                    index = num2 + 1;
                }
                else
                {
                    Debug.LogError("Did not find insertAfterItem, should be a child of parentItem!!");
                }
            }
            list.InsertRange(index, draggedItems);
            parentItem.children = list;
        }

        public int IDCounter { get; private set; }

        public Foo root
        {
            get
            {
                return this.m_Root;
            }
        }

        public class Foo
        {
            public Foo(string name, int depth, int id)
            {
                this.name = name;
                this.depth = depth;
                this.id = id;
            }

            public List<BackendData.Foo> children { get; set; }

            public int depth { get; set; }

            public bool hasChildren
            {
                get
                {
                    return ((this.children != null) && (this.children.Count > 0));
                }
            }

            public int id { get; set; }

            public string name { get; set; }

            public BackendData.Foo parent { get; set; }
        }
    }
}

