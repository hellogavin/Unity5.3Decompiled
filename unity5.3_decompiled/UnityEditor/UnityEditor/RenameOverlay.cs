namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class RenameOverlay
    {
        private string k_RenameOverlayFocusName = "RenameOverlayField";
        [SerializeField]
        private GUIView m_ClientGUIView;
        [NonSerialized]
        private DelayedCallback m_DelayedCallback;
        [SerializeField]
        private Rect m_EditFieldRect;
        [SerializeField]
        private bool m_IsRenaming;
        [SerializeField]
        private bool m_IsRenamingFilename;
        [SerializeField]
        private bool m_IsWaitingForDelay;
        [NonSerialized]
        private Rect m_LastScreenPosition;
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private EventType m_OriginalEventType = EventType.Ignore;
        [SerializeField]
        private string m_OriginalName;
        private int m_TextFieldControlID;
        [NonSerialized]
        private bool m_UndoRedoWasPerformed;
        [SerializeField]
        private bool m_UserAcceptedRename;
        [SerializeField]
        private int m_UserData;
        private static GUIStyle s_DefaultTextFieldStyle = null;
        private static int s_TextFieldHash = "RenameFieldTextField".GetHashCode();

        public bool BeginRename(string name, int userData, float delay)
        {
            if (this.m_IsRenaming)
            {
                Debug.LogError("BeginRename fail: already renaming");
                return false;
            }
            this.m_Name = name;
            this.m_OriginalName = name;
            this.m_UserData = userData;
            this.m_UserAcceptedRename = false;
            this.m_IsWaitingForDelay = delay > 0f;
            this.m_IsRenaming = true;
            this.m_EditFieldRect = new Rect(0f, 0f, 0f, 0f);
            this.m_ClientGUIView = GUIView.current;
            if (delay > 0f)
            {
                this.m_DelayedCallback = new DelayedCallback(new Action(this.BeginRenameInternalCallback), (double) delay);
            }
            else
            {
                this.BeginRenameInternalCallback();
            }
            return true;
        }

        private void BeginRenameInternalCallback()
        {
            EditorGUI.s_RecycledEditor.text = this.m_Name;
            EditorGUI.s_RecycledEditor.SelectAll();
            this.RepaintClientView();
            this.m_IsWaitingForDelay = false;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoWasPerformed));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoWasPerformed));
        }

        public void Clear()
        {
            this.m_IsRenaming = false;
            this.m_UserAcceptedRename = false;
            this.m_Name = string.Empty;
            this.m_OriginalName = string.Empty;
            this.m_EditFieldRect = new Rect();
            this.m_UserData = 0;
            this.m_IsWaitingForDelay = false;
            this.m_OriginalEventType = EventType.Ignore;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoWasPerformed));
        }

        private string DoTextField(string text, GUIStyle textFieldStyle)
        {
            bool flag;
            if (this.m_TextFieldControlID == 0)
            {
                Debug.LogError("RenameOverlay: Ensure to call OnEvent() as early as possible in the OnGUI of the current EditorWindow!");
            }
            if (s_DefaultTextFieldStyle == null)
            {
                s_DefaultTextFieldStyle = "PR TextField";
            }
            if (this.isRenamingFilename)
            {
                this.EatInvalidChars();
            }
            GUI.changed = false;
            if (GUIUtility.keyboardControl != this.m_TextFieldControlID)
            {
                GUIUtility.keyboardControl = this.m_TextFieldControlID;
            }
            if (textFieldStyle == null)
            {
            }
            return EditorGUI.DoTextField(EditorGUI.s_RecycledEditor, this.m_TextFieldControlID, EditorGUI.IndentedRect(this.m_EditFieldRect), text, s_DefaultTextFieldStyle, null, out flag, false, false, false);
        }

        private void EatInvalidChars()
        {
            if (this.isRenamingFilename)
            {
                Event current = Event.current;
                if ((GUIUtility.keyboardControl == this.m_TextFieldControlID) && (current.GetTypeForControl(this.m_TextFieldControlID) == EventType.KeyDown))
                {
                    string msg = string.Empty;
                    string invalidFilenameChars = EditorUtility.GetInvalidFilenameChars();
                    if (invalidFilenameChars.IndexOf(current.character) > -1)
                    {
                        msg = "A file name can't contain any of the following characters:\t" + invalidFilenameChars;
                    }
                    if (msg != string.Empty)
                    {
                        current.Use();
                        this.ShowMessage(msg);
                    }
                    else
                    {
                        this.RemoveMessage();
                    }
                }
                if (current.type == EventType.Repaint)
                {
                    Rect screenRect = this.GetScreenRect();
                    if (!Mathf.Approximately(this.m_LastScreenPosition.x, screenRect.x) || !Mathf.Approximately(this.m_LastScreenPosition.y, screenRect.y))
                    {
                        this.RemoveMessage();
                    }
                    this.m_LastScreenPosition = screenRect;
                }
            }
        }

        public void EndRename(bool acceptChanges)
        {
            if (this.m_IsRenaming)
            {
                Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoWasPerformed));
                if (this.m_DelayedCallback != null)
                {
                    this.m_DelayedCallback.Clear();
                }
                this.RemoveMessage();
                if (this.isRenamingFilename)
                {
                    this.m_Name = InternalEditorUtility.RemoveInvalidCharsFromFileName(this.m_Name, true);
                }
                this.m_IsRenaming = false;
                this.m_IsWaitingForDelay = false;
                this.m_UserAcceptedRename = acceptChanges;
                this.RepaintClientView();
            }
        }

        private Rect GetScreenRect()
        {
            return GUIUtility.GUIToScreenRect(this.m_EditFieldRect);
        }

        public bool HasKeyboardFocus()
        {
            return (GUI.GetNameOfFocusedControl() == this.k_RenameOverlayFocusName);
        }

        public bool IsRenaming()
        {
            return this.m_IsRenaming;
        }

        public bool OnEvent()
        {
            if (!this.m_IsRenaming)
            {
                return true;
            }
            if (!this.m_IsWaitingForDelay)
            {
                GUIUtility.GetControlID(0x50f6804, FocusType.Passive);
                GUI.SetNextControlName(this.k_RenameOverlayFocusName);
                EditorGUI.FocusTextInControl(this.k_RenameOverlayFocusName);
                this.m_TextFieldControlID = GUIUtility.GetControlID(s_TextFieldHash, FocusType.Keyboard, this.m_EditFieldRect);
            }
            this.m_OriginalEventType = Event.current.type;
            if (!this.m_IsWaitingForDelay || ((this.m_OriginalEventType != EventType.MouseDown) && (this.m_OriginalEventType != EventType.KeyDown)))
            {
                return true;
            }
            this.EndRename(false);
            return false;
        }

        public bool OnGUI()
        {
            return this.OnGUI(null);
        }

        public bool OnGUI(GUIStyle textFieldStyle)
        {
            if (!this.m_IsWaitingForDelay)
            {
                if (!this.m_IsRenaming)
                {
                    return false;
                }
                if (this.m_UndoRedoWasPerformed)
                {
                    this.m_UndoRedoWasPerformed = false;
                    this.EndRename(false);
                    return false;
                }
                if (((this.m_EditFieldRect.width <= 0f) || (this.m_EditFieldRect.height <= 0f)) || (this.m_TextFieldControlID == 0))
                {
                    HandleUtility.Repaint();
                    return true;
                }
                Event current = Event.current;
                if (current.type == EventType.KeyDown)
                {
                    if (current.keyCode == KeyCode.Escape)
                    {
                        current.Use();
                        this.EndRename(false);
                        return false;
                    }
                    if ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter))
                    {
                        current.Use();
                        this.EndRename(true);
                        return false;
                    }
                }
                if ((this.m_OriginalEventType == EventType.MouseDown) && !this.m_EditFieldRect.Contains(Event.current.mousePosition))
                {
                    this.EndRename(true);
                    return false;
                }
                this.m_Name = this.DoTextField(this.m_Name, textFieldStyle);
                if (current.type == EventType.ScrollWheel)
                {
                    current.Use();
                }
            }
            return true;
        }

        private void RemoveMessage()
        {
            TooltipView.Close();
        }

        private void RepaintClientView()
        {
            if (this.m_ClientGUIView != null)
            {
                this.m_ClientGUIView.Repaint();
            }
        }

        private void ShowMessage(string msg)
        {
            TooltipView.Show(msg, this.GetScreenRect());
        }

        private void UndoRedoWasPerformed()
        {
            this.m_UndoRedoWasPerformed = true;
        }

        public Rect editFieldRect
        {
            get
            {
                return this.m_EditFieldRect;
            }
            set
            {
                this.m_EditFieldRect = value;
            }
        }

        public bool isRenamingFilename
        {
            get
            {
                return this.m_IsRenamingFilename;
            }
            set
            {
                this.m_IsRenamingFilename = value;
            }
        }

        public bool isWaitingForDelay
        {
            get
            {
                return this.m_IsWaitingForDelay;
            }
        }

        public string name
        {
            get
            {
                return this.m_Name;
            }
        }

        public string originalName
        {
            get
            {
                return this.m_OriginalName;
            }
        }

        public bool userAcceptedRename
        {
            get
            {
                return this.m_UserAcceptedRename;
            }
        }

        public int userData
        {
            get
            {
                return this.m_UserData;
            }
        }
    }
}

