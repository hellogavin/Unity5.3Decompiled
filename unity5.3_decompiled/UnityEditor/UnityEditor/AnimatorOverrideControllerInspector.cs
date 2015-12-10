namespace UnityEditor
{
    using System;
    using UnityEditor.Animations;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(AnimatorOverrideController)), CanEditMultipleObjects]
    internal class AnimatorOverrideControllerInspector : Editor
    {
        private ReorderableList m_ClipList;
        private AnimationClipPair[] m_Clips;
        private SerializedProperty m_Controller;

        private void DrawClipElement(Rect rect, int index, bool selected, bool focused)
        {
            AnimationClip originalClip = this.m_Clips[index].originalClip;
            AnimationClip overrideClip = this.m_Clips[index].overrideClip;
            rect.xMax /= 2f;
            GUI.Label(rect, originalClip.name, EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax *= 2f;
            EditorGUI.BeginChangeCheck();
            overrideClip = EditorGUI.ObjectField(rect, string.Empty, overrideClip, typeof(AnimationClip), false) as AnimationClip;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Clips[index].overrideClip = overrideClip;
            }
        }

        private void DrawClipHeader(Rect rect)
        {
            rect.xMax /= 2f;
            GUI.Label(rect, "Original", EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax *= 2f;
            GUI.Label(rect, "Override", EditorStyles.label);
        }

        private void OnDisable()
        {
            AnimatorOverrideController target = this.target as AnimatorOverrideController;
            target.OnOverrideControllerDirty = (AnimatorOverrideController.OnOverrideControllerDirtyCallback) Delegate.Remove(target.OnOverrideControllerDirty, new AnimatorOverrideController.OnOverrideControllerDirtyCallback(this.Repaint));
        }

        private void OnEnable()
        {
            AnimatorOverrideController target = this.target as AnimatorOverrideController;
            this.m_Controller = base.serializedObject.FindProperty("m_Controller");
            if (this.m_ClipList == null)
            {
                this.m_ClipList = new ReorderableList(target.clips, typeof(AnimationClipPair), false, true, false, false);
                this.m_ClipList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawClipElement);
                this.m_ClipList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawClipHeader);
                this.m_ClipList.elementHeight = 16f;
            }
            target.OnOverrideControllerDirty = (AnimatorOverrideController.OnOverrideControllerDirtyCallback) Delegate.Combine(target.OnOverrideControllerDirty, new AnimatorOverrideController.OnOverrideControllerDirtyCallback(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            bool flag = base.targets.Length > 1;
            bool flag2 = false;
            base.serializedObject.UpdateIfDirtyOrScript();
            AnimatorOverrideController target = this.target as AnimatorOverrideController;
            RuntimeAnimatorController controller2 = !this.m_Controller.hasMultipleDifferentValues ? target.runtimeAnimatorController : null;
            EditorGUI.BeginChangeCheck();
            controller2 = EditorGUILayout.ObjectField("Controller", controller2, typeof(AnimatorController), false, new GUILayoutOption[0]) as RuntimeAnimatorController;
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < base.targets.Length; i++)
                {
                    AnimatorOverrideController controller3 = base.targets[i] as AnimatorOverrideController;
                    controller3.runtimeAnimatorController = controller2;
                }
                flag2 = true;
            }
            EditorGUI.BeginDisabledGroup(((this.m_Controller == null) || (flag && this.m_Controller.hasMultipleDifferentValues)) || (controller2 == null));
            EditorGUI.BeginChangeCheck();
            this.m_Clips = target.clips;
            this.m_ClipList.list = this.m_Clips;
            this.m_ClipList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                for (int j = 0; j < base.targets.Length; j++)
                {
                    AnimatorOverrideController controller4 = base.targets[j] as AnimatorOverrideController;
                    controller4.clips = this.m_Clips;
                }
                flag2 = true;
            }
            EditorGUI.EndDisabledGroup();
            if (flag2)
            {
                target.PerformOverrideClipListCleanup();
            }
        }
    }
}

