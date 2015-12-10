namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(TextAsset)), CanEditMultipleObjects]
    internal class TextAssetInspector : Editor
    {
        private const int kMaxChars = 0x1b58;
        [NonSerialized]
        private GUIStyle m_TextStyle;

        public override void OnInspectorGUI()
        {
            if (this.m_TextStyle == null)
            {
                this.m_TextStyle = "ScriptText";
            }
            bool enabled = GUI.enabled;
            GUI.enabled = true;
            TextAsset target = this.target as TextAsset;
            if (target != null)
            {
                string targetTitle;
                if (base.targets.Length > 1)
                {
                    targetTitle = this.targetTitle;
                }
                else
                {
                    targetTitle = target.ToString();
                    if (targetTitle.Length > 0x1b58)
                    {
                        targetTitle = targetTitle.Substring(0, 0x1b58) + "...\n\n<...etc...>";
                    }
                }
                Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.TempContent(targetTitle), this.m_TextStyle);
                position.x = 0f;
                position.y -= 3f;
                position.width = GUIClip.visibleRect.width + 1f;
                GUI.Box(position, targetTitle, this.m_TextStyle);
            }
            GUI.enabled = enabled;
        }
    }
}

