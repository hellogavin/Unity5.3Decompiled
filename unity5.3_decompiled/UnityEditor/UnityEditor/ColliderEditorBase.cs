namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ColliderEditorBase : Editor
    {
        protected void ForceQuitEditMode()
        {
            EditMode.QuitEditMode();
        }

        private static Bounds GetColliderBounds(Object collider)
        {
            if (collider is Collider2D)
            {
                return (collider as Collider2D).bounds;
            }
            if (collider is Collider)
            {
                return (collider as Collider).bounds;
            }
            return new Bounds();
        }

        protected void InspectorEditButtonGUI()
        {
            EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Collider", EditorGUIUtility.IconContent("EditCollider"), GetColliderBounds(this.target), this);
        }

        public virtual void OnDisable()
        {
            EditMode.onEditModeEndDelegate = (EditMode.OnEditModeStopFunc) Delegate.Remove(EditMode.onEditModeEndDelegate, new EditMode.OnEditModeStopFunc(this.OnEditModeEnd));
        }

        protected virtual void OnEditEnd()
        {
        }

        protected void OnEditModeEnd(Editor editor)
        {
            if (editor == this)
            {
                this.OnEditEnd();
            }
        }

        protected void OnEditModeStart(Editor editor, EditMode.SceneViewEditMode mode)
        {
            if ((mode == EditMode.SceneViewEditMode.Collider) && (editor == this))
            {
                this.OnEditStart();
            }
        }

        protected virtual void OnEditStart()
        {
        }

        public virtual void OnEnable()
        {
            EditMode.onEditModeStartDelegate = (EditMode.OnEditModeStartFunc) Delegate.Combine(EditMode.onEditModeStartDelegate, new EditMode.OnEditModeStartFunc(this.OnEditModeStart));
        }

        public bool editingCollider
        {
            get
            {
                return ((EditMode.editMode == EditMode.SceneViewEditMode.Collider) && EditMode.IsOwner(this));
            }
        }
    }
}

