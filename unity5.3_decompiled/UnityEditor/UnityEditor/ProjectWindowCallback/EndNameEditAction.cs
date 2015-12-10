namespace UnityEditor.ProjectWindowCallback
{
    using System;
    using UnityEngine;

    public abstract class EndNameEditAction : ScriptableObject
    {
        protected EndNameEditAction()
        {
        }

        public abstract void Action(int instanceId, string pathName, string resourceFile);
        public virtual void CleanUp()
        {
            Object.DestroyImmediate(this);
        }

        public virtual void OnEnable()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}

