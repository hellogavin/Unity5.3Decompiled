namespace UnityEditor
{
    using System;

    [CustomEditor(typeof(DefaultAsset), isFallback=true)]
    internal class DefaultAssetInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DefaultAsset target = (DefaultAsset) this.target;
            if (target.message.Length > 0)
            {
                EditorGUILayout.HelpBox(target.message, !target.isWarning ? MessageType.Info : MessageType.Warning);
            }
        }
    }
}

