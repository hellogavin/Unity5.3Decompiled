namespace UnityEngine
{
    using System;
    using System.Collections.Generic;

    public class TextEditor
    {
        public int controlID;
        public Vector2 graphicalCursorPos;
        public Vector2 graphicalSelectCursorPos;
        public bool hasHorizontalCursorPos;
        public bool isPasswordField;
        public TouchScreenKeyboard keyboardOnScreen;
        private bool m_bJustSelected;
        private GUIContent m_Content = new GUIContent();
        private int m_CursorIndex;
        private int m_DblClickInitPos;
        private DblClickSnapping m_DblClickSnap;
        internal bool m_HasFocus;
        private int m_iAltCursorPos = -1;
        private bool m_MouseDragSelectsWholeWords;
        private Rect m_Position;
        private bool m_RevealCursor;
        private int m_SelectIndex;
        public bool multiline;
        private int oldPos;
        private int oldSelectPos;
        private string oldText;
        private static Dictionary<Event, TextEditOp> s_Keyactions;
        public Vector2 scrollOffset = Vector2.zero;
        public GUIStyle style = GUIStyle.none;

        public bool Backspace()
        {
            if (this.hasSelection)
            {
                this.DeleteSelection();
                return true;
            }
            if (this.cursorIndex > 0)
            {
                this.m_Content.text = this.text.Remove(this.cursorIndex - 1, 1);
                int num = this.cursorIndex - 1;
                this.cursorIndex = num;
                this.selectIndex = num;
                this.ClearCursorPos();
                return true;
            }
            return false;
        }

        public bool CanPaste()
        {
            return (GUIUtility.systemCopyBuffer.Length != 0);
        }

        private void ClampTextIndex(ref int index)
        {
            index = Mathf.Clamp(index, 0, this.text.Length);
        }

        private CharacterType ClassifyChar(char c)
        {
            if (char.IsWhiteSpace(c))
            {
                return CharacterType.WhiteSpace;
            }
            if (!char.IsLetterOrDigit(c) && (c != '\''))
            {
                return CharacterType.Symbol;
            }
            return CharacterType.LetterLike;
        }

        private void ClearCursorPos()
        {
            this.hasHorizontalCursorPos = false;
            this.m_iAltCursorPos = -1;
        }

        public void Copy()
        {
            if ((this.selectIndex != this.cursorIndex) && !this.isPasswordField)
            {
                string str;
                if (this.cursorIndex < this.selectIndex)
                {
                    str = this.text.Substring(this.cursorIndex, this.selectIndex - this.cursorIndex);
                }
                else
                {
                    str = this.text.Substring(this.selectIndex, this.cursorIndex - this.selectIndex);
                }
                GUIUtility.systemCopyBuffer = str;
            }
        }

        public bool Cut()
        {
            if (this.isPasswordField)
            {
                return false;
            }
            this.Copy();
            return this.DeleteSelection();
        }

        public void DblClickSnap(DblClickSnapping snapping)
        {
            this.m_DblClickSnap = snapping;
        }

        public bool Delete()
        {
            if (this.hasSelection)
            {
                this.DeleteSelection();
                return true;
            }
            if (this.cursorIndex < this.text.Length)
            {
                this.m_Content.text = this.text.Remove(this.cursorIndex, 1);
                return true;
            }
            return false;
        }

        public bool DeleteLineBack()
        {
            if (this.hasSelection)
            {
                this.DeleteSelection();
                return true;
            }
            int cursorIndex = this.cursorIndex;
            int num2 = cursorIndex;
            while (num2-- != 0)
            {
                if (this.text[num2] == '\n')
                {
                    cursorIndex = num2 + 1;
                    break;
                }
            }
            if (num2 == -1)
            {
                cursorIndex = 0;
            }
            if (this.cursorIndex != cursorIndex)
            {
                this.m_Content.text = this.text.Remove(cursorIndex, this.cursorIndex - cursorIndex);
                int num3 = cursorIndex;
                this.cursorIndex = num3;
                this.selectIndex = num3;
                return true;
            }
            return false;
        }

        public bool DeleteSelection()
        {
            if (this.cursorIndex == this.selectIndex)
            {
                return false;
            }
            if (this.cursorIndex < this.selectIndex)
            {
                this.m_Content.text = this.text.Substring(0, this.cursorIndex) + this.text.Substring(this.selectIndex, this.text.Length - this.selectIndex);
                this.selectIndex = this.cursorIndex;
            }
            else
            {
                this.m_Content.text = this.text.Substring(0, this.selectIndex) + this.text.Substring(this.cursorIndex, this.text.Length - this.cursorIndex);
                this.cursorIndex = this.selectIndex;
            }
            this.ClearCursorPos();
            return true;
        }

        public bool DeleteWordBack()
        {
            if (this.hasSelection)
            {
                this.DeleteSelection();
                return true;
            }
            int startIndex = this.FindEndOfPreviousWord(this.cursorIndex);
            if (this.cursorIndex != startIndex)
            {
                this.m_Content.text = this.text.Remove(startIndex, this.cursorIndex - startIndex);
                int num2 = startIndex;
                this.cursorIndex = num2;
                this.selectIndex = num2;
                return true;
            }
            return false;
        }

        public bool DeleteWordForward()
        {
            if (this.hasSelection)
            {
                this.DeleteSelection();
                return true;
            }
            int num = this.FindStartOfNextWord(this.cursorIndex);
            if (this.cursorIndex < this.text.Length)
            {
                this.m_Content.text = this.text.Remove(this.cursorIndex, num - this.cursorIndex);
                return true;
            }
            return false;
        }

        public void DetectFocusChange()
        {
            if (this.m_HasFocus && (this.controlID != GUIUtility.keyboardControl))
            {
                this.OnLostFocus();
            }
            if (!this.m_HasFocus && (this.controlID == GUIUtility.keyboardControl))
            {
                this.OnFocus();
            }
        }

        public void DrawCursor(string newText)
        {
            string text = this.text;
            int cursorIndex = this.cursorIndex;
            if (Input.compositionString.Length > 0)
            {
                this.m_Content.text = newText.Substring(0, this.cursorIndex) + Input.compositionString + newText.Substring(this.selectIndex);
                cursorIndex += Input.compositionString.Length;
            }
            else
            {
                this.m_Content.text = newText;
            }
            this.graphicalCursorPos = this.style.GetCursorPixelPosition(new Rect(0f, 0f, this.position.width, this.position.height), this.m_Content, cursorIndex);
            Vector2 contentOffset = this.style.contentOffset;
            this.style.contentOffset -= this.scrollOffset;
            this.style.Internal_clipOffset = this.scrollOffset;
            Input.compositionCursorPos = (this.graphicalCursorPos + new Vector2(this.position.x, this.position.y + this.style.lineHeight)) - this.scrollOffset;
            if (Input.compositionString.Length > 0)
            {
                this.style.DrawWithTextSelection(this.position, this.m_Content, this.controlID, this.cursorIndex, this.cursorIndex + Input.compositionString.Length, true);
            }
            else
            {
                this.style.DrawWithTextSelection(this.position, this.m_Content, this.controlID, this.cursorIndex, this.selectIndex);
            }
            if (this.m_iAltCursorPos != -1)
            {
                this.style.DrawCursor(this.position, this.m_Content, this.controlID, this.m_iAltCursorPos);
            }
            this.style.contentOffset = contentOffset;
            this.style.Internal_clipOffset = Vector2.zero;
            this.m_Content.text = text;
        }

        public void ExpandSelectGraphicalLineEnd()
        {
            this.ClearCursorPos();
            if (this.cursorIndex > this.selectIndex)
            {
                this.cursorIndex = this.GetGraphicalLineEnd(this.cursorIndex);
            }
            else
            {
                int cursorIndex = this.cursorIndex;
                this.cursorIndex = this.GetGraphicalLineEnd(this.selectIndex);
                this.selectIndex = cursorIndex;
            }
        }

        public void ExpandSelectGraphicalLineStart()
        {
            this.ClearCursorPos();
            if (this.cursorIndex < this.selectIndex)
            {
                this.cursorIndex = this.GetGraphicalLineStart(this.cursorIndex);
            }
            else
            {
                int cursorIndex = this.cursorIndex;
                this.cursorIndex = this.GetGraphicalLineStart(this.selectIndex);
                this.selectIndex = cursorIndex;
            }
        }

        private int FindEndOfClassification(int p, int dir)
        {
            int length = this.text.Length;
            if ((p >= length) || (p < 0))
            {
                return p;
            }
            CharacterType type = this.ClassifyChar(this.text[p]);
            do
            {
                p += dir;
                if (p < 0)
                {
                    return 0;
                }
                if (p >= length)
                {
                    return length;
                }
            }
            while (this.ClassifyChar(this.text[p]) == type);
            if (dir == 1)
            {
                return p;
            }
            return (p + 1);
        }

        private int FindEndOfPreviousWord(int p)
        {
            if (p != 0)
            {
                p--;
                while ((p > 0) && (this.text[p] == ' '))
                {
                    p--;
                }
                CharacterType type = this.ClassifyChar(this.text[p]);
                if (type == CharacterType.WhiteSpace)
                {
                    return p;
                }
                while ((p > 0) && (this.ClassifyChar(this.text[p - 1]) == type))
                {
                    p--;
                }
            }
            return p;
        }

        private int FindNextSeperator(int startPos)
        {
            int length = this.text.Length;
            while ((startPos < length) && !isLetterLikeChar(this.text[startPos]))
            {
                startPos++;
            }
            while ((startPos < length) && isLetterLikeChar(this.text[startPos]))
            {
                startPos++;
            }
            return startPos;
        }

        private int FindPrevSeperator(int startPos)
        {
            startPos--;
            while ((startPos > 0) && !isLetterLikeChar(this.text[startPos]))
            {
                startPos--;
            }
            while ((startPos >= 0) && isLetterLikeChar(this.text[startPos]))
            {
                startPos--;
            }
            return (startPos + 1);
        }

        public int FindStartOfNextWord(int p)
        {
            int length = this.text.Length;
            if (p != length)
            {
                char c = this.text[p];
                CharacterType type = this.ClassifyChar(c);
                if (type != CharacterType.WhiteSpace)
                {
                    p++;
                    while ((p < length) && (this.ClassifyChar(this.text[p]) == type))
                    {
                        p++;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '\t':
                        case '\n':
                            return (p + 1);
                    }
                }
                if (p != length)
                {
                    switch (this.text[p])
                    {
                        case ' ':
                            while ((p < length) && char.IsWhiteSpace(this.text[p]))
                            {
                                p++;
                            }
                            return p;
                    }
                }
            }
            return p;
        }

        private int GetGraphicalLineEnd(int p)
        {
            Vector2 cursorPixelPosition = this.style.GetCursorPixelPosition(this.position, this.m_Content, p);
            cursorPixelPosition.x += 5000f;
            return this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPixelPosition);
        }

        private int GetGraphicalLineStart(int p)
        {
            Vector2 cursorPixelPosition = this.style.GetCursorPixelPosition(this.position, this.m_Content, p);
            cursorPixelPosition.x = 0f;
            return this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPixelPosition);
        }

        private void GrabGraphicalCursorPos()
        {
            if (!this.hasHorizontalCursorPos)
            {
                this.graphicalCursorPos = this.style.GetCursorPixelPosition(this.position, this.m_Content, this.cursorIndex);
                this.graphicalSelectCursorPos = this.style.GetCursorPixelPosition(this.position, this.m_Content, this.selectIndex);
                this.hasHorizontalCursorPos = false;
            }
        }

        public bool HandleKeyEvent(Event e)
        {
            this.InitKeyActions();
            EventModifiers modifiers = e.modifiers;
            e.modifiers &= ~EventModifiers.CapsLock;
            if (s_Keyactions.ContainsKey(e))
            {
                TextEditOp operation = s_Keyactions[e];
                this.PerformOperation(operation);
                e.modifiers = modifiers;
                return true;
            }
            e.modifiers = modifiers;
            return false;
        }

        private int IndexOfEndOfLine(int startIndex)
        {
            int index = this.text.IndexOf('\n', startIndex);
            return ((index == -1) ? this.text.Length : index);
        }

        private void InitKeyActions()
        {
            if (s_Keyactions == null)
            {
                s_Keyactions = new Dictionary<Event, TextEditOp>();
                MapKey("left", TextEditOp.MoveLeft);
                MapKey("right", TextEditOp.MoveRight);
                MapKey("up", TextEditOp.MoveUp);
                MapKey("down", TextEditOp.MoveDown);
                MapKey("#left", TextEditOp.SelectLeft);
                MapKey("#right", TextEditOp.SelectRight);
                MapKey("#up", TextEditOp.SelectUp);
                MapKey("#down", TextEditOp.SelectDown);
                MapKey("delete", TextEditOp.Delete);
                MapKey("backspace", TextEditOp.Backspace);
                MapKey("#backspace", TextEditOp.Backspace);
                if (((Application.platform == RuntimePlatform.OSXPlayer) || (Application.platform == RuntimePlatform.OSXWebPlayer)) || (((Application.platform == RuntimePlatform.OSXDashboardPlayer) || (Application.platform == RuntimePlatform.OSXEditor)) || ((Application.platform == RuntimePlatform.WebGLPlayer) && SystemInfo.operatingSystem.StartsWith("Mac"))))
                {
                    MapKey("^left", TextEditOp.MoveGraphicalLineStart);
                    MapKey("^right", TextEditOp.MoveGraphicalLineEnd);
                    MapKey("&left", TextEditOp.MoveWordLeft);
                    MapKey("&right", TextEditOp.MoveWordRight);
                    MapKey("&up", TextEditOp.MoveParagraphBackward);
                    MapKey("&down", TextEditOp.MoveParagraphForward);
                    MapKey("%left", TextEditOp.MoveGraphicalLineStart);
                    MapKey("%right", TextEditOp.MoveGraphicalLineEnd);
                    MapKey("%up", TextEditOp.MoveTextStart);
                    MapKey("%down", TextEditOp.MoveTextEnd);
                    MapKey("#home", TextEditOp.SelectTextStart);
                    MapKey("#end", TextEditOp.SelectTextEnd);
                    MapKey("#^left", TextEditOp.ExpandSelectGraphicalLineStart);
                    MapKey("#^right", TextEditOp.ExpandSelectGraphicalLineEnd);
                    MapKey("#^up", TextEditOp.SelectParagraphBackward);
                    MapKey("#^down", TextEditOp.SelectParagraphForward);
                    MapKey("#&left", TextEditOp.SelectWordLeft);
                    MapKey("#&right", TextEditOp.SelectWordRight);
                    MapKey("#&up", TextEditOp.SelectParagraphBackward);
                    MapKey("#&down", TextEditOp.SelectParagraphForward);
                    MapKey("#%left", TextEditOp.ExpandSelectGraphicalLineStart);
                    MapKey("#%right", TextEditOp.ExpandSelectGraphicalLineEnd);
                    MapKey("#%up", TextEditOp.SelectTextStart);
                    MapKey("#%down", TextEditOp.SelectTextEnd);
                    MapKey("%a", TextEditOp.SelectAll);
                    MapKey("%x", TextEditOp.Cut);
                    MapKey("%c", TextEditOp.Copy);
                    MapKey("%v", TextEditOp.Paste);
                    MapKey("^d", TextEditOp.Delete);
                    MapKey("^h", TextEditOp.Backspace);
                    MapKey("^b", TextEditOp.MoveLeft);
                    MapKey("^f", TextEditOp.MoveRight);
                    MapKey("^a", TextEditOp.MoveLineStart);
                    MapKey("^e", TextEditOp.MoveLineEnd);
                    MapKey("&delete", TextEditOp.DeleteWordForward);
                    MapKey("&backspace", TextEditOp.DeleteWordBack);
                    MapKey("%backspace", TextEditOp.DeleteLineBack);
                }
                else
                {
                    MapKey("home", TextEditOp.MoveGraphicalLineStart);
                    MapKey("end", TextEditOp.MoveGraphicalLineEnd);
                    MapKey("%left", TextEditOp.MoveWordLeft);
                    MapKey("%right", TextEditOp.MoveWordRight);
                    MapKey("%up", TextEditOp.MoveParagraphBackward);
                    MapKey("%down", TextEditOp.MoveParagraphForward);
                    MapKey("^left", TextEditOp.MoveToEndOfPreviousWord);
                    MapKey("^right", TextEditOp.MoveToStartOfNextWord);
                    MapKey("^up", TextEditOp.MoveParagraphBackward);
                    MapKey("^down", TextEditOp.MoveParagraphForward);
                    MapKey("#^left", TextEditOp.SelectToEndOfPreviousWord);
                    MapKey("#^right", TextEditOp.SelectToStartOfNextWord);
                    MapKey("#^up", TextEditOp.SelectParagraphBackward);
                    MapKey("#^down", TextEditOp.SelectParagraphForward);
                    MapKey("#home", TextEditOp.SelectGraphicalLineStart);
                    MapKey("#end", TextEditOp.SelectGraphicalLineEnd);
                    MapKey("^delete", TextEditOp.DeleteWordForward);
                    MapKey("^backspace", TextEditOp.DeleteWordBack);
                    MapKey("%backspace", TextEditOp.DeleteLineBack);
                    MapKey("^a", TextEditOp.SelectAll);
                    MapKey("^x", TextEditOp.Cut);
                    MapKey("^c", TextEditOp.Copy);
                    MapKey("^v", TextEditOp.Paste);
                    MapKey("#delete", TextEditOp.Cut);
                    MapKey("^insert", TextEditOp.Copy);
                    MapKey("#insert", TextEditOp.Paste);
                }
            }
        }

        public void Insert(char c)
        {
            this.ReplaceSelection(c.ToString());
        }

        private static bool isLetterLikeChar(char c)
        {
            return (char.IsLetterOrDigit(c) || (c == '\''));
        }

        public bool IsOverSelection(Vector2 cursorPosition)
        {
            int num = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
            return ((num < Mathf.Max(this.cursorIndex, this.selectIndex)) && (num > Mathf.Min(this.cursorIndex, this.selectIndex)));
        }

        private static void MapKey(string key, TextEditOp action)
        {
            s_Keyactions[Event.KeyboardEvent(key)] = action;
        }

        public void MouseDragSelectsWholeWords(bool on)
        {
            this.m_MouseDragSelectsWholeWords = on;
            this.m_DblClickInitPos = this.cursorIndex;
        }

        public void MoveAltCursorToPosition(Vector2 cursorPosition)
        {
            int b = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
            this.m_iAltCursorPos = Mathf.Min(this.text.Length, b);
            this.DetectFocusChange();
        }

        public void MoveCursorToPosition(Vector2 cursorPosition)
        {
            this.selectIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
            if (!Event.current.shift)
            {
                this.cursorIndex = this.selectIndex;
            }
            this.DetectFocusChange();
        }

        public void MoveDown()
        {
            if (this.selectIndex > this.cursorIndex)
            {
                this.selectIndex = this.cursorIndex;
            }
            else
            {
                this.cursorIndex = this.selectIndex;
            }
            this.GrabGraphicalCursorPos();
            this.graphicalCursorPos.y += this.style.lineHeight + 5f;
            int num = this.style.GetCursorStringIndex(this.position, this.m_Content, this.graphicalCursorPos);
            this.selectIndex = num;
            this.cursorIndex = num;
            if (this.cursorIndex == this.text.Length)
            {
                this.ClearCursorPos();
            }
        }

        public void MoveGraphicalLineEnd()
        {
            int graphicalLineEnd = this.GetGraphicalLineEnd((this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex);
            this.selectIndex = graphicalLineEnd;
            this.cursorIndex = graphicalLineEnd;
        }

        public void MoveGraphicalLineStart()
        {
            int graphicalLineStart = this.GetGraphicalLineStart((this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex);
            this.selectIndex = graphicalLineStart;
            this.cursorIndex = graphicalLineStart;
        }

        public void MoveLeft()
        {
            if (this.selectIndex == this.cursorIndex)
            {
                this.cursorIndex--;
                this.selectIndex = this.cursorIndex;
            }
            else if (this.selectIndex > this.cursorIndex)
            {
                this.selectIndex = this.cursorIndex;
            }
            else
            {
                this.cursorIndex = this.selectIndex;
            }
            this.ClearCursorPos();
        }

        public void MoveLineEnd()
        {
            int num4;
            int num2 = (this.selectIndex <= this.cursorIndex) ? this.cursorIndex : this.selectIndex;
            int length = this.text.Length;
            while (num2 < length)
            {
                if (this.text[num2] == '\n')
                {
                    num4 = num2;
                    this.cursorIndex = num4;
                    this.selectIndex = num4;
                    return;
                }
                num2++;
            }
            num4 = length;
            this.cursorIndex = num4;
            this.selectIndex = num4;
        }

        public void MoveLineStart()
        {
            int num3;
            int num2 = (this.selectIndex >= this.cursorIndex) ? this.cursorIndex : this.selectIndex;
            while (num2-- != 0)
            {
                if (this.text[num2] == '\n')
                {
                    num3 = num2 + 1;
                    this.cursorIndex = num3;
                    this.selectIndex = num3;
                    return;
                }
            }
            num3 = 0;
            this.cursorIndex = num3;
            this.selectIndex = num3;
        }

        public void MoveParagraphBackward()
        {
            int num;
            this.cursorIndex = (this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex;
            if (this.cursorIndex > 1)
            {
                num = this.text.LastIndexOf('\n', this.cursorIndex - 2) + 1;
                this.cursorIndex = num;
                this.selectIndex = num;
            }
            else
            {
                num = 0;
                this.cursorIndex = num;
                this.selectIndex = num;
            }
        }

        public void MoveParagraphForward()
        {
            this.cursorIndex = (this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex;
            if (this.cursorIndex < this.text.Length)
            {
                int num = this.IndexOfEndOfLine(this.cursorIndex + 1);
                this.cursorIndex = num;
                this.selectIndex = num;
            }
        }

        public void MoveRight()
        {
            this.ClearCursorPos();
            if (this.selectIndex == this.cursorIndex)
            {
                this.cursorIndex++;
                this.DetectFocusChange();
                this.selectIndex = this.cursorIndex;
            }
            else if (this.selectIndex > this.cursorIndex)
            {
                this.cursorIndex = this.selectIndex;
            }
            else
            {
                this.selectIndex = this.cursorIndex;
            }
        }

        public void MoveSelectionToAltCursor()
        {
            if (this.m_iAltCursorPos != -1)
            {
                int iAltCursorPos = this.m_iAltCursorPos;
                string selectedText = this.SelectedText;
                this.m_Content.text = this.text.Insert(iAltCursorPos, selectedText);
                if (iAltCursorPos < this.cursorIndex)
                {
                    this.cursorIndex += selectedText.Length;
                    this.selectIndex += selectedText.Length;
                }
                this.DeleteSelection();
                int num2 = iAltCursorPos;
                this.cursorIndex = num2;
                this.selectIndex = num2;
                this.ClearCursorPos();
            }
        }

        public void MoveTextEnd()
        {
            int length = this.text.Length;
            this.cursorIndex = length;
            this.selectIndex = length;
        }

        public void MoveTextStart()
        {
            int num = 0;
            this.cursorIndex = num;
            this.selectIndex = num;
        }

        public void MoveToEndOfPreviousWord()
        {
            this.ClearCursorPos();
            if (this.cursorIndex != this.selectIndex)
            {
                this.MoveLeft();
            }
            else
            {
                int num = this.FindEndOfPreviousWord(this.cursorIndex);
                this.selectIndex = num;
                this.cursorIndex = num;
            }
        }

        public void MoveToStartOfNextWord()
        {
            this.ClearCursorPos();
            if (this.cursorIndex != this.selectIndex)
            {
                this.MoveRight();
            }
            else
            {
                int num = this.FindStartOfNextWord(this.cursorIndex);
                this.selectIndex = num;
                this.cursorIndex = num;
            }
        }

        public void MoveUp()
        {
            if (this.selectIndex < this.cursorIndex)
            {
                this.selectIndex = this.cursorIndex;
            }
            else
            {
                this.cursorIndex = this.selectIndex;
            }
            this.GrabGraphicalCursorPos();
            this.graphicalCursorPos.y--;
            int num = this.style.GetCursorStringIndex(this.position, this.m_Content, this.graphicalCursorPos);
            this.selectIndex = num;
            this.cursorIndex = num;
            if (this.cursorIndex <= 0)
            {
                this.ClearCursorPos();
            }
        }

        public void MoveWordLeft()
        {
            this.cursorIndex = (this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex;
            this.cursorIndex = this.FindPrevSeperator(this.cursorIndex);
            this.selectIndex = this.cursorIndex;
        }

        public void MoveWordRight()
        {
            this.cursorIndex = (this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex;
            int num = this.FindNextSeperator(this.cursorIndex);
            this.selectIndex = num;
            this.cursorIndex = num;
            this.ClearCursorPos();
        }

        public void OnFocus()
        {
            if (this.multiline)
            {
                int num = 0;
                this.selectIndex = num;
                this.cursorIndex = num;
            }
            else
            {
                this.SelectAll();
            }
            this.m_HasFocus = true;
        }

        public void OnLostFocus()
        {
            this.m_HasFocus = false;
            this.scrollOffset = Vector2.zero;
        }

        public bool Paste()
        {
            string systemCopyBuffer = GUIUtility.systemCopyBuffer;
            if (!(systemCopyBuffer != string.Empty))
            {
                return false;
            }
            if (!this.multiline)
            {
                systemCopyBuffer = ReplaceNewlinesWithSpaces(systemCopyBuffer);
            }
            this.ReplaceSelection(systemCopyBuffer);
            return true;
        }

        private bool PerformOperation(TextEditOp operation)
        {
            this.m_RevealCursor = true;
            switch (operation)
            {
                case TextEditOp.MoveLeft:
                    this.MoveLeft();
                    break;

                case TextEditOp.MoveRight:
                    this.MoveRight();
                    break;

                case TextEditOp.MoveUp:
                    this.MoveUp();
                    break;

                case TextEditOp.MoveDown:
                    this.MoveDown();
                    break;

                case TextEditOp.MoveLineStart:
                    this.MoveLineStart();
                    break;

                case TextEditOp.MoveLineEnd:
                    this.MoveLineEnd();
                    break;

                case TextEditOp.MoveTextStart:
                    this.MoveTextStart();
                    break;

                case TextEditOp.MoveTextEnd:
                    this.MoveTextEnd();
                    break;

                case TextEditOp.MoveGraphicalLineStart:
                    this.MoveGraphicalLineStart();
                    break;

                case TextEditOp.MoveGraphicalLineEnd:
                    this.MoveGraphicalLineEnd();
                    break;

                case TextEditOp.MoveWordLeft:
                    this.MoveWordLeft();
                    break;

                case TextEditOp.MoveWordRight:
                    this.MoveWordRight();
                    break;

                case TextEditOp.MoveParagraphForward:
                    this.MoveParagraphForward();
                    break;

                case TextEditOp.MoveParagraphBackward:
                    this.MoveParagraphBackward();
                    break;

                case TextEditOp.MoveToStartOfNextWord:
                    this.MoveToStartOfNextWord();
                    break;

                case TextEditOp.MoveToEndOfPreviousWord:
                    this.MoveToEndOfPreviousWord();
                    break;

                case TextEditOp.SelectLeft:
                    this.SelectLeft();
                    break;

                case TextEditOp.SelectRight:
                    this.SelectRight();
                    break;

                case TextEditOp.SelectUp:
                    this.SelectUp();
                    break;

                case TextEditOp.SelectDown:
                    this.SelectDown();
                    break;

                case TextEditOp.SelectTextStart:
                    this.SelectTextStart();
                    break;

                case TextEditOp.SelectTextEnd:
                    this.SelectTextEnd();
                    break;

                case TextEditOp.ExpandSelectGraphicalLineStart:
                    this.ExpandSelectGraphicalLineStart();
                    break;

                case TextEditOp.ExpandSelectGraphicalLineEnd:
                    this.ExpandSelectGraphicalLineEnd();
                    break;

                case TextEditOp.SelectGraphicalLineStart:
                    this.SelectGraphicalLineStart();
                    break;

                case TextEditOp.SelectGraphicalLineEnd:
                    this.SelectGraphicalLineEnd();
                    break;

                case TextEditOp.SelectWordLeft:
                    this.SelectWordLeft();
                    break;

                case TextEditOp.SelectWordRight:
                    this.SelectWordRight();
                    break;

                case TextEditOp.SelectToEndOfPreviousWord:
                    this.SelectToEndOfPreviousWord();
                    break;

                case TextEditOp.SelectToStartOfNextWord:
                    this.SelectToStartOfNextWord();
                    break;

                case TextEditOp.SelectParagraphBackward:
                    this.SelectParagraphBackward();
                    break;

                case TextEditOp.SelectParagraphForward:
                    this.SelectParagraphForward();
                    break;

                case TextEditOp.Delete:
                    return this.Delete();

                case TextEditOp.Backspace:
                    return this.Backspace();

                case TextEditOp.DeleteWordBack:
                    return this.DeleteWordBack();

                case TextEditOp.DeleteWordForward:
                    return this.DeleteWordForward();

                case TextEditOp.DeleteLineBack:
                    return this.DeleteLineBack();

                case TextEditOp.Cut:
                    return this.Cut();

                case TextEditOp.Copy:
                    this.Copy();
                    break;

                case TextEditOp.Paste:
                    return this.Paste();

                case TextEditOp.SelectAll:
                    this.SelectAll();
                    break;

                case TextEditOp.SelectNone:
                    this.SelectNone();
                    break;

                default:
                    Debug.Log("Unimplemented: " + operation);
                    break;
            }
            return false;
        }

        private static string ReplaceNewlinesWithSpaces(string value)
        {
            value = value.Replace("\r\n", " ");
            value = value.Replace('\n', ' ');
            value = value.Replace('\r', ' ');
            return value;
        }

        public void ReplaceSelection(string replace)
        {
            this.DeleteSelection();
            this.m_Content.text = this.text.Insert(this.cursorIndex, replace);
            this.selectIndex = this.cursorIndex += replace.Length;
            this.ClearCursorPos();
        }

        public void SaveBackup()
        {
            this.oldText = this.text;
            this.oldPos = this.cursorIndex;
            this.oldSelectPos = this.selectIndex;
        }

        public void SelectAll()
        {
            this.cursorIndex = 0;
            this.selectIndex = this.text.Length;
            this.ClearCursorPos();
        }

        public void SelectCurrentParagraph()
        {
            this.ClearCursorPos();
            int length = this.text.Length;
            if (this.cursorIndex < length)
            {
                this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex) + 1;
            }
            if (this.selectIndex != 0)
            {
                this.selectIndex = this.text.LastIndexOf('\n', this.selectIndex - 1) + 1;
            }
        }

        public void SelectCurrentWord()
        {
            this.ClearCursorPos();
            int length = this.text.Length;
            this.selectIndex = this.cursorIndex;
            if (length != 0)
            {
                if (this.cursorIndex >= length)
                {
                    this.cursorIndex = length - 1;
                }
                if (this.selectIndex >= length)
                {
                    this.selectIndex--;
                }
                if (this.cursorIndex < this.selectIndex)
                {
                    this.cursorIndex = this.FindEndOfClassification(this.cursorIndex, -1);
                    this.selectIndex = this.FindEndOfClassification(this.selectIndex, 1);
                }
                else
                {
                    this.cursorIndex = this.FindEndOfClassification(this.cursorIndex, 1);
                    this.selectIndex = this.FindEndOfClassification(this.selectIndex, -1);
                }
                this.m_bJustSelected = true;
            }
        }

        public void SelectDown()
        {
            this.GrabGraphicalCursorPos();
            this.graphicalCursorPos.y += this.style.lineHeight + 5f;
            this.cursorIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, this.graphicalCursorPos);
        }

        public void SelectGraphicalLineEnd()
        {
            this.ClearCursorPos();
            this.cursorIndex = this.GetGraphicalLineEnd(this.cursorIndex);
        }

        public void SelectGraphicalLineStart()
        {
            this.ClearCursorPos();
            this.cursorIndex = this.GetGraphicalLineStart(this.cursorIndex);
        }

        public void SelectLeft()
        {
            if (this.m_bJustSelected && (this.cursorIndex > this.selectIndex))
            {
                int cursorIndex = this.cursorIndex;
                this.cursorIndex = this.selectIndex;
                this.selectIndex = cursorIndex;
            }
            this.m_bJustSelected = false;
            this.cursorIndex--;
        }

        public void SelectNone()
        {
            this.selectIndex = this.cursorIndex;
            this.ClearCursorPos();
        }

        public void SelectParagraphBackward()
        {
            this.ClearCursorPos();
            bool flag = this.cursorIndex > this.selectIndex;
            if (this.cursorIndex > 1)
            {
                this.cursorIndex = this.text.LastIndexOf('\n', this.cursorIndex - 2) + 1;
                if (flag && (this.cursorIndex < this.selectIndex))
                {
                    this.cursorIndex = this.selectIndex;
                }
            }
            else
            {
                int num = 0;
                this.cursorIndex = num;
                this.selectIndex = num;
            }
        }

        public void SelectParagraphForward()
        {
            this.ClearCursorPos();
            bool flag = this.cursorIndex < this.selectIndex;
            if (this.cursorIndex < this.text.Length)
            {
                this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex + 1);
                if (flag && (this.cursorIndex > this.selectIndex))
                {
                    this.cursorIndex = this.selectIndex;
                }
            }
        }

        public void SelectRight()
        {
            if (this.m_bJustSelected && (this.cursorIndex < this.selectIndex))
            {
                int cursorIndex = this.cursorIndex;
                this.cursorIndex = this.selectIndex;
                this.selectIndex = cursorIndex;
            }
            this.m_bJustSelected = false;
            this.cursorIndex++;
        }

        public void SelectTextEnd()
        {
            this.cursorIndex = this.text.Length;
        }

        public void SelectTextStart()
        {
            this.cursorIndex = 0;
        }

        public void SelectToEndOfPreviousWord()
        {
            this.ClearCursorPos();
            this.cursorIndex = this.FindEndOfPreviousWord(this.cursorIndex);
        }

        public void SelectToPosition(Vector2 cursorPosition)
        {
            if (!this.m_MouseDragSelectsWholeWords)
            {
                this.cursorIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
            }
            else
            {
                int p = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
                if (this.m_DblClickSnap == DblClickSnapping.WORDS)
                {
                    if (p < this.m_DblClickInitPos)
                    {
                        this.cursorIndex = this.FindEndOfClassification(p, -1);
                        this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPos, 1);
                    }
                    else
                    {
                        if (p >= this.text.Length)
                        {
                            p = this.text.Length - 1;
                        }
                        this.cursorIndex = this.FindEndOfClassification(p, 1);
                        this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPos - 1, -1);
                    }
                }
                else if (p < this.m_DblClickInitPos)
                {
                    if (p > 0)
                    {
                        this.cursorIndex = this.text.LastIndexOf('\n', Mathf.Max(0, p - 2)) + 1;
                    }
                    else
                    {
                        this.cursorIndex = 0;
                    }
                    this.selectIndex = this.text.LastIndexOf('\n', this.m_DblClickInitPos);
                }
                else
                {
                    if (p < this.text.Length)
                    {
                        this.cursorIndex = this.IndexOfEndOfLine(p);
                    }
                    else
                    {
                        this.cursorIndex = this.text.Length;
                    }
                    this.selectIndex = this.text.LastIndexOf('\n', Mathf.Max(0, this.m_DblClickInitPos - 2)) + 1;
                }
            }
        }

        public void SelectToStartOfNextWord()
        {
            this.ClearCursorPos();
            this.cursorIndex = this.FindStartOfNextWord(this.cursorIndex);
        }

        public void SelectUp()
        {
            this.GrabGraphicalCursorPos();
            this.graphicalCursorPos.y--;
            this.cursorIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, this.graphicalCursorPos);
        }

        public void SelectWordLeft()
        {
            this.ClearCursorPos();
            int selectIndex = this.selectIndex;
            if (this.cursorIndex > this.selectIndex)
            {
                this.selectIndex = this.cursorIndex;
                this.MoveWordLeft();
                this.selectIndex = selectIndex;
                this.cursorIndex = (this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex;
            }
            else
            {
                this.selectIndex = this.cursorIndex;
                this.MoveWordLeft();
                this.selectIndex = selectIndex;
            }
        }

        public void SelectWordRight()
        {
            this.ClearCursorPos();
            int selectIndex = this.selectIndex;
            if (this.cursorIndex < this.selectIndex)
            {
                this.selectIndex = this.cursorIndex;
                this.MoveWordRight();
                this.selectIndex = selectIndex;
                this.cursorIndex = (this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex;
            }
            else
            {
                this.selectIndex = this.cursorIndex;
                this.MoveWordRight();
                this.selectIndex = selectIndex;
            }
        }

        public void Undo()
        {
            this.m_Content.text = this.oldText;
            this.cursorIndex = this.oldPos;
            this.selectIndex = this.oldSelectPos;
        }

        private void UpdateScrollOffset()
        {
            int cursorIndex = this.cursorIndex;
            this.graphicalCursorPos = this.style.GetCursorPixelPosition(new Rect(0f, 0f, this.position.width, this.position.height), this.m_Content, cursorIndex);
            Rect rect = this.style.padding.Remove(this.position);
            Vector2 vector = new Vector2(this.style.CalcSize(this.m_Content).x, this.style.CalcHeight(this.m_Content, this.position.width));
            if (vector.x < this.position.width)
            {
                this.scrollOffset.x = 0f;
            }
            else if (this.m_RevealCursor)
            {
                if ((this.graphicalCursorPos.x + 1f) > (this.scrollOffset.x + rect.width))
                {
                    this.scrollOffset.x = this.graphicalCursorPos.x - rect.width;
                }
                if (this.graphicalCursorPos.x < (this.scrollOffset.x + this.style.padding.left))
                {
                    this.scrollOffset.x = this.graphicalCursorPos.x - this.style.padding.left;
                }
            }
            if (vector.y < rect.height)
            {
                this.scrollOffset.y = 0f;
            }
            else if (this.m_RevealCursor)
            {
                if ((this.graphicalCursorPos.y + this.style.lineHeight) > ((this.scrollOffset.y + rect.height) + this.style.padding.top))
                {
                    this.scrollOffset.y = ((this.graphicalCursorPos.y - rect.height) - this.style.padding.top) + this.style.lineHeight;
                }
                if (this.graphicalCursorPos.y < (this.scrollOffset.y + this.style.padding.top))
                {
                    this.scrollOffset.y = this.graphicalCursorPos.y - this.style.padding.top;
                }
            }
            if ((this.scrollOffset.y > 0f) && ((vector.y - this.scrollOffset.y) < rect.height))
            {
                this.scrollOffset.y = ((vector.y - rect.height) - this.style.padding.top) - this.style.padding.bottom;
            }
            this.scrollOffset.y = (this.scrollOffset.y >= 0f) ? this.scrollOffset.y : 0f;
            this.m_RevealCursor = false;
        }

        public void UpdateScrollOffsetIfNeeded()
        {
            if ((Event.current.type != EventType.Repaint) && (Event.current.type != EventType.Layout))
            {
                this.UpdateScrollOffset();
            }
        }

        [Obsolete("Please use 'text' instead of 'content'", false)]
        public GUIContent content
        {
            get
            {
                return this.m_Content;
            }
            set
            {
                this.m_Content = value;
            }
        }

        public int cursorIndex
        {
            get
            {
                return this.m_CursorIndex;
            }
            set
            {
                int cursorIndex = this.m_CursorIndex;
                this.m_CursorIndex = value;
                this.ClampTextIndex(ref this.m_CursorIndex);
                if (this.m_CursorIndex != cursorIndex)
                {
                    this.m_RevealCursor = true;
                }
            }
        }

        public bool hasSelection
        {
            get
            {
                return (this.cursorIndex != this.selectIndex);
            }
        }

        public Rect position
        {
            get
            {
                return this.m_Position;
            }
            set
            {
                if (this.m_Position != value)
                {
                    this.m_Position = value;
                    this.UpdateScrollOffset();
                }
            }
        }

        public string SelectedText
        {
            get
            {
                if (this.cursorIndex == this.selectIndex)
                {
                    return string.Empty;
                }
                if (this.cursorIndex < this.selectIndex)
                {
                    return this.text.Substring(this.cursorIndex, this.selectIndex - this.cursorIndex);
                }
                return this.text.Substring(this.selectIndex, this.cursorIndex - this.selectIndex);
            }
        }

        public int selectIndex
        {
            get
            {
                return this.m_SelectIndex;
            }
            set
            {
                this.m_SelectIndex = value;
                this.ClampTextIndex(ref this.m_SelectIndex);
            }
        }

        public string text
        {
            get
            {
                return this.m_Content.text;
            }
            set
            {
                this.m_Content.text = value;
                this.ClampTextIndex(ref this.m_CursorIndex);
                this.ClampTextIndex(ref this.m_SelectIndex);
            }
        }

        private enum CharacterType
        {
            LetterLike,
            Symbol,
            Symbol2,
            WhiteSpace
        }

        public enum DblClickSnapping : byte
        {
            PARAGRAPHS = 1,
            WORDS = 0
        }

        private enum TextEditOp
        {
            MoveLeft,
            MoveRight,
            MoveUp,
            MoveDown,
            MoveLineStart,
            MoveLineEnd,
            MoveTextStart,
            MoveTextEnd,
            MovePageUp,
            MovePageDown,
            MoveGraphicalLineStart,
            MoveGraphicalLineEnd,
            MoveWordLeft,
            MoveWordRight,
            MoveParagraphForward,
            MoveParagraphBackward,
            MoveToStartOfNextWord,
            MoveToEndOfPreviousWord,
            SelectLeft,
            SelectRight,
            SelectUp,
            SelectDown,
            SelectTextStart,
            SelectTextEnd,
            SelectPageUp,
            SelectPageDown,
            ExpandSelectGraphicalLineStart,
            ExpandSelectGraphicalLineEnd,
            SelectGraphicalLineStart,
            SelectGraphicalLineEnd,
            SelectWordLeft,
            SelectWordRight,
            SelectToEndOfPreviousWord,
            SelectToStartOfNextWord,
            SelectParagraphBackward,
            SelectParagraphForward,
            Delete,
            Backspace,
            DeleteWordBack,
            DeleteWordForward,
            DeleteLineBack,
            Cut,
            Copy,
            Paste,
            SelectAll,
            SelectNone,
            ScrollStart,
            ScrollEnd,
            ScrollPageUp,
            ScrollPageDown
        }
    }
}

