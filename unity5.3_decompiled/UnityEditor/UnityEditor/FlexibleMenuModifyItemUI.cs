namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal abstract class FlexibleMenuModifyItemUI : PopupWindowContent
    {
        protected Action<object> m_AcceptedCallback;
        private bool m_IsInitialized;
        protected MenuType m_MenuType;
        public object m_Object;

        protected FlexibleMenuModifyItemUI()
        {
        }

        public void Accepted()
        {
            if (this.m_AcceptedCallback != null)
            {
                this.m_AcceptedCallback(this.m_Object);
            }
            else
            {
                Debug.LogError("Missing callback. Did you remember to call Init ?");
            }
        }

        public void Init(MenuType menuType, object obj, Action<object> acceptedCallback)
        {
            this.m_MenuType = menuType;
            this.m_Object = obj;
            this.m_AcceptedCallback = acceptedCallback;
            this.m_IsInitialized = true;
        }

        public bool IsShowing()
        {
            return this.m_IsInitialized;
        }

        public override void OnClose()
        {
            this.m_Object = null;
            this.m_AcceptedCallback = null;
            this.m_IsInitialized = false;
            EditorApplication.RequestRepaintAllViews();
        }

        public enum MenuType
        {
            Add,
            Edit
        }
    }
}

