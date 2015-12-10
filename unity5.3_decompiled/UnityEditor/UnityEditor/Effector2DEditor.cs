namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(Effector2D), true), CanEditMultipleObjects]
    internal class Effector2DEditor : Editor
    {
        [CompilerGenerated]
        private static Func<Collider2D, bool> <>f__am$cache3;
        private SerializedProperty m_ColliderMask;
        private readonly AnimBool m_ShowColliderMask = new AnimBool();
        private SerializedProperty m_UseColliderMask;

        public static void CheckEffectorWarnings(Collider2D collider)
        {
            if (collider.usedByEffector)
            {
                Effector2D component = collider.GetComponent<Effector2D>();
                if ((component == null) || !component.enabled)
                {
                    EditorGUILayout.HelpBox("This collider will not function with an effector until there is at least one enabled 2D effector on this GameObject.", MessageType.Warning);
                    if (component == null)
                    {
                        return;
                    }
                }
                if (component.designedForNonTrigger && collider.isTrigger)
                {
                    EditorGUILayout.HelpBox("This collider has 'Is Trigger' checked but this should be unchecked when used with the '" + component.GetType().Name + "' component which is designed to work with collisions.", MessageType.Warning);
                }
                else if (component.designedForTrigger && !collider.isTrigger)
                {
                    EditorGUILayout.HelpBox("This collider has 'Is Trigger' unchecked but this should be checked when used with the '" + component.GetType().Name + "' component which is designed to work with triggers.", MessageType.Warning);
                }
            }
        }

        public void OnDisable()
        {
            this.m_ShowColliderMask.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public void OnEnable()
        {
            this.m_UseColliderMask = base.serializedObject.FindProperty("m_UseColliderMask");
            this.m_ColliderMask = base.serializedObject.FindProperty("m_ColliderMask");
            this.m_ShowColliderMask.value = (this.target as Effector2D).useColliderMask;
            this.m_ShowColliderMask.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            Effector2D target = this.target as Effector2D;
            this.m_ShowColliderMask.target = target.useColliderMask;
            EditorGUILayout.PropertyField(this.m_UseColliderMask, new GUILayoutOption[0]);
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowColliderMask.faded))
            {
                EditorGUILayout.PropertyField(this.m_ColliderMask, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            base.serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = collider => collider.enabled && collider.usedByEffector;
            }
            if (!target.GetComponents<Collider2D>().Any<Collider2D>(<>f__am$cache3))
            {
                if (target.requiresCollider)
                {
                    EditorGUILayout.HelpBox("This effector will not function until there is at least one enabled 2D collider with 'Used by Effector' checked on this GameObject.", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("This effector can optionally work without a 2D collider.", MessageType.Info);
                }
            }
        }
    }
}

