namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Sprites;
    using UnityEngine;

    [CustomEditor(typeof(PolygonCollider2D)), CanEditMultipleObjects]
    internal class PolygonCollider2DEditor : Collider2DEditorBase
    {
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Object, PolygonCollider2D> <>f__am$cache3;
        private readonly PolygonEditorUtility m_PolyUtility = new PolygonEditorUtility();
        private bool m_ShowColliderInfo;

        private void HandleDragAndDrop()
        {
            if ((Event.current.type == EventType.DragPerform) || (Event.current.type == EventType.DragUpdated))
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = obj => (obj is Sprite) || (obj is Texture2D);
                }
                IEnumerator<Object> enumerator = DragAndDrop.objectReferences.Where<Object>(<>f__am$cache2).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Object current = enumerator.Current;
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (Event.current.type == EventType.DragPerform)
                        {
                            Sprite sprite = !(current is Sprite) ? SpriteUtility.TextureToSprite(current as Texture2D) : (current as Sprite);
                            if (<>f__am$cache3 == null)
                            {
                                <>f__am$cache3 = target => target as PolygonCollider2D;
                            }
                            IEnumerator<PolygonCollider2D> enumerator2 = base.targets.Select<Object, PolygonCollider2D>(<>f__am$cache3).GetEnumerator();
                            try
                            {
                                while (enumerator2.MoveNext())
                                {
                                    Vector2[][] vectorArray;
                                    PolygonCollider2D colliderd = enumerator2.Current;
                                    SpriteUtility.GenerateOutlineFromSprite(sprite, 0.25f, 200, true, out vectorArray);
                                    colliderd.pathCount = vectorArray.Length;
                                    for (int i = 0; i < vectorArray.Length; i++)
                                    {
                                        colliderd.SetPath(i, vectorArray[i]);
                                    }
                                    this.m_PolyUtility.StopEditing();
                                    DragAndDrop.AcceptDrag();
                                }
                            }
                            finally
                            {
                                if (enumerator2 == null)
                                {
                                }
                                enumerator2.Dispose();
                            }
                        }
                        return;
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }

        protected override void OnEditEnd()
        {
            this.m_PolyUtility.StopEditing();
        }

        protected override void OnEditStart()
        {
            if (this.target != null)
            {
                this.m_PolyUtility.StartEditing(this.target as Collider2D);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            this.HandleDragAndDrop();
            base.BeginColliderInspector();
            base.OnInspectorGUI();
            base.EndColliderInspector();
        }

        public void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                this.m_PolyUtility.OnSceneGUI();
            }
        }
    }
}

