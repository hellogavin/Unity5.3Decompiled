namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class GameObjectTreeViewItem : TreeViewItem
    {
        private int m_ColorCode;
        private Object m_ObjectPPTR;
        private bool m_ShouldDisplay;
        private Scene m_UnityScene;

        public GameObjectTreeViewItem(int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
        {
        }

        public virtual int colorCode
        {
            get
            {
                return this.m_ColorCode;
            }
            set
            {
                this.m_ColorCode = value;
            }
        }

        public override string displayName
        {
            get
            {
                if (string.IsNullOrEmpty(base.displayName))
                {
                    if (this.m_ObjectPPTR != null)
                    {
                        this.displayName = this.objectPPTR.name;
                    }
                    else
                    {
                        this.displayName = "deleted gameobject";
                    }
                }
                return base.displayName;
            }
            set
            {
                base.displayName = value;
            }
        }

        public bool isSceneHeader { get; set; }

        public virtual Object objectPPTR
        {
            get
            {
                return this.m_ObjectPPTR;
            }
            set
            {
                this.m_ObjectPPTR = value;
            }
        }

        public Scene scene
        {
            get
            {
                return this.m_UnityScene;
            }
            set
            {
                this.m_UnityScene = value;
            }
        }

        public virtual bool shouldDisplay
        {
            get
            {
                return this.m_ShouldDisplay;
            }
            set
            {
                this.m_ShouldDisplay = value;
            }
        }
    }
}

