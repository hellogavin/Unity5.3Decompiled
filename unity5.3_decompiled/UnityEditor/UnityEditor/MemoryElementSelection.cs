namespace UnityEditor
{
    using System;

    [Serializable]
    internal class MemoryElementSelection
    {
        private MemoryElement m_Selected;

        public void ClearSelection()
        {
            this.m_Selected = null;
        }

        public bool isSelected(MemoryElement node)
        {
            return (this.m_Selected == node);
        }

        public void MoveDown()
        {
            if ((this.m_Selected != null) && (this.m_Selected.parent != null))
            {
                MemoryElement nextNode = this.m_Selected.GetNextNode();
                if (nextNode != null)
                {
                    this.SetSelection(nextNode);
                }
            }
        }

        public void MoveFirst()
        {
            if ((this.m_Selected != null) && (this.m_Selected.parent != null))
            {
                this.SetSelection(this.m_Selected.GetRoot().FirstChild());
            }
        }

        public void MoveLast()
        {
            if ((this.m_Selected != null) && (this.m_Selected.parent != null))
            {
                this.SetSelection(this.m_Selected.GetRoot().LastChild());
            }
        }

        public void MoveParent()
        {
            if (((this.m_Selected != null) && (this.m_Selected.parent != null)) && (this.m_Selected.parent.parent != null))
            {
                this.SetSelection(this.m_Selected.parent);
            }
        }

        public void MoveUp()
        {
            if ((this.m_Selected != null) && (this.m_Selected.parent != null))
            {
                MemoryElement prevNode = this.m_Selected.GetPrevNode();
                if (prevNode.parent != null)
                {
                    this.SetSelection(prevNode);
                }
                else
                {
                    this.SetSelection(prevNode.FirstChild());
                }
            }
        }

        public void SetSelection(MemoryElement node)
        {
            this.m_Selected = node;
            for (MemoryElement element = node.parent; element != null; element = element.parent)
            {
                element.expanded = true;
            }
        }

        public MemoryElement Selected
        {
            get
            {
                return this.m_Selected;
            }
        }
    }
}

