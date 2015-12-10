namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal class ClipboardAccess
    {
        static ClipboardAccess()
        {
            JSProxyMgr.GetInstance().AddGlobalObject("unity/ClipboardAccess", new ClipboardAccess());
        }

        private ClipboardAccess()
        {
        }

        public void CopyToClipboard(string value)
        {
            TextEditor editor = new TextEditor {
                text = value
            };
            editor.SelectAll();
            editor.Copy();
        }

        public string PasteFromClipboard()
        {
            TextEditor editor = new TextEditor();
            editor.Paste();
            return editor.text;
        }
    }
}

