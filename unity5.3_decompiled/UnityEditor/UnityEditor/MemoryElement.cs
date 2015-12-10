namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    internal class MemoryElement
    {
        public List<MemoryElement> children;
        public string description;
        public bool expanded;
        public ObjectInfo memoryInfo;
        public string name;
        public MemoryElement parent;
        public int totalChildCount;
        public int totalMemory;

        public MemoryElement()
        {
            this.children = new List<MemoryElement>();
        }

        public MemoryElement(string n)
        {
            this.expanded = false;
            this.name = n;
            this.children = new List<MemoryElement>();
            this.description = string.Empty;
        }

        public MemoryElement(string n, List<MemoryElement> groups)
        {
            this.name = n;
            this.expanded = false;
            this.description = string.Empty;
            this.totalMemory = 0;
            this.totalChildCount = 0;
            this.children = new List<MemoryElement>();
            foreach (MemoryElement element in groups)
            {
                this.AddChild(element);
            }
        }

        public MemoryElement(ObjectInfo memInfo, bool finalize)
        {
            this.expanded = false;
            this.memoryInfo = memInfo;
            this.name = this.memoryInfo.name;
            this.totalMemory = (memInfo == null) ? 0 : memInfo.memorySize;
            this.totalChildCount = 1;
            if (finalize)
            {
                this.children = new List<MemoryElement>();
            }
        }

        public int AccumulatedChildCount()
        {
            return this.totalChildCount;
        }

        public void AddChild(MemoryElement node)
        {
            if (node == this)
            {
                throw new Exception("Should not AddChild to itself");
            }
            this.children.Add(node);
            node.parent = this;
            this.totalMemory += node.totalMemory;
            this.totalChildCount += node.totalChildCount;
        }

        public int ChildCount()
        {
            if (this.children != null)
            {
                return this.children.Count;
            }
            return this.ReferenceCount();
        }

        public void ExpandChildren()
        {
            if (this.children == null)
            {
                this.children = new List<MemoryElement>();
                for (int i = 0; i < this.ReferenceCount(); i++)
                {
                    this.AddChild(new MemoryElement(this.memoryInfo.referencedBy[i], false));
                }
            }
        }

        public MemoryElement FirstChild()
        {
            return this.children[0];
        }

        public int GetChildIndexInList()
        {
            for (int i = 0; i < this.parent.children.Count; i++)
            {
                if (this.parent.children[i] == this)
                {
                    return i;
                }
            }
            return this.parent.children.Count;
        }

        public MemoryElement GetNextNode()
        {
            if (this.expanded && (this.children.Count > 0))
            {
                return this.children[0];
            }
            int num = this.GetChildIndexInList() + 1;
            if (num < this.parent.children.Count)
            {
                return this.parent.children[num];
            }
            for (MemoryElement element = this.parent; element.parent != null; element = element.parent)
            {
                int num2 = element.GetChildIndexInList() + 1;
                if (num2 < element.parent.children.Count)
                {
                    return element.parent.children[num2];
                }
            }
            return null;
        }

        public MemoryElement GetPrevNode()
        {
            int num = this.GetChildIndexInList() - 1;
            if (num < 0)
            {
                return this.parent;
            }
            MemoryElement element = this.parent.children[num];
            while (element.expanded)
            {
                element = element.children[element.children.Count - 1];
            }
            return element;
        }

        public MemoryElement GetRoot()
        {
            if (this.parent != null)
            {
                return this.parent.GetRoot();
            }
            return this;
        }

        public MemoryElement LastChild()
        {
            if (!this.expanded)
            {
                return this;
            }
            return this.children[this.children.Count - 1].LastChild();
        }

        public int ReferenceCount()
        {
            return (((this.memoryInfo == null) || (this.memoryInfo.referencedBy == null)) ? 0 : this.memoryInfo.referencedBy.Count);
        }
    }
}

