namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class Event
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        private static Event s_Current;
        private static Event s_MasterEvent;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;
        public Event()
        {
            this.Init(0);
        }

        public Event(int displayIndex)
        {
            this.Init(displayIndex);
        }

        public Event(Event other)
        {
            if (other == null)
            {
                throw new ArgumentException("Event to copy from is null.");
            }
            this.InitCopy(other);
        }

        private Event(IntPtr ptr)
        {
            this.InitPtr(ptr);
        }

        ~Event()
        {
            this.Cleanup();
        }

        public Vector2 mousePosition
        {
            get
            {
                Vector2 vector;
                this.Internal_GetMousePosition(out vector);
                return vector;
            }
            set
            {
                this.Internal_SetMousePosition(value);
            }
        }
        public Vector2 delta
        {
            get
            {
                Vector2 vector;
                this.Internal_GetMouseDelta(out vector);
                return vector;
            }
            set
            {
                this.Internal_SetMouseDelta(value);
            }
        }
        [Obsolete("Use HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);", true)]
        public Ray mouseRay
        {
            get
            {
                return new Ray(Vector3.up, Vector3.up);
            }
            set
            {
            }
        }
        public bool shift
        {
            get
            {
                return ((this.modifiers & EventModifiers.Shift) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Shift;
                }
                else
                {
                    this.modifiers |= EventModifiers.Shift;
                }
            }
        }
        public bool control
        {
            get
            {
                return ((this.modifiers & EventModifiers.Control) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Control;
                }
                else
                {
                    this.modifiers |= EventModifiers.Control;
                }
            }
        }
        public bool alt
        {
            get
            {
                return ((this.modifiers & EventModifiers.Alt) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Alt;
                }
                else
                {
                    this.modifiers |= EventModifiers.Alt;
                }
            }
        }
        public bool command
        {
            get
            {
                return ((this.modifiers & EventModifiers.Command) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Command;
                }
                else
                {
                    this.modifiers |= EventModifiers.Command;
                }
            }
        }
        public bool capsLock
        {
            get
            {
                return ((this.modifiers & EventModifiers.CapsLock) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.CapsLock;
                }
                else
                {
                    this.modifiers |= EventModifiers.CapsLock;
                }
            }
        }
        public bool numeric
        {
            get
            {
                return ((this.modifiers & EventModifiers.Numeric) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Shift;
                }
                else
                {
                    this.modifiers |= EventModifiers.Shift;
                }
            }
        }
        public bool functionKey
        {
            get
            {
                return ((this.modifiers & EventModifiers.FunctionKey) != EventModifiers.None);
            }
        }
        public static Event current
        {
            get
            {
                if (GUIUtility.Internal_GetGUIDepth() > 0)
                {
                    return s_Current;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    s_Current = value;
                }
                else
                {
                    s_Current = s_MasterEvent;
                }
                Internal_SetNativeEvent(s_Current.m_Ptr);
            }
        }
        [RequiredByNativeCode]
        private static void Internal_MakeMasterEventCurrent(int displayIndex)
        {
            if (s_MasterEvent == null)
            {
                s_MasterEvent = new Event(displayIndex);
            }
            s_MasterEvent.displayIndex = displayIndex;
            s_Current = s_MasterEvent;
            Internal_SetNativeEvent(s_MasterEvent.m_Ptr);
        }

        public bool isKey
        {
            get
            {
                EventType type = this.type;
                return ((type == EventType.KeyDown) || (type == EventType.KeyUp));
            }
        }
        public bool isMouse
        {
            get
            {
                EventType type = this.type;
                return ((((type == EventType.MouseMove) || (type == EventType.MouseDown)) || (type == EventType.MouseUp)) || (type == EventType.MouseDrag));
            }
        }
        public static Event KeyboardEvent(string key)
        {
            Event event2 = new Event(0) {
                type = EventType.KeyDown
            };
            if (!string.IsNullOrEmpty(key))
            {
                int startIndex = 0;
                bool flag = false;
                do
                {
                    flag = true;
                    if (startIndex >= key.Length)
                    {
                        flag = false;
                        break;
                    }
                    char ch = key[startIndex];
                    switch (ch)
                    {
                        case '#':
                            event2.modifiers |= EventModifiers.Shift;
                            startIndex++;
                            break;

                        case '%':
                            event2.modifiers |= EventModifiers.Command;
                            startIndex++;
                            break;

                        case '&':
                            event2.modifiers |= EventModifiers.Alt;
                            startIndex++;
                            break;

                        default:
                            if (ch == '^')
                            {
                                event2.modifiers |= EventModifiers.Control;
                                startIndex++;
                            }
                            else
                            {
                                flag = false;
                            }
                            break;
                    }
                }
                while (flag);
                string str = key.Substring(startIndex, key.Length - startIndex).ToLower();
                string str2 = str;
                if (str2 != null)
                {
                    int num2;
                    if (<>f__switch$map0 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(0x31);
                        dictionary.Add("[0]", 0);
                        dictionary.Add("[1]", 1);
                        dictionary.Add("[2]", 2);
                        dictionary.Add("[3]", 3);
                        dictionary.Add("[4]", 4);
                        dictionary.Add("[5]", 5);
                        dictionary.Add("[6]", 6);
                        dictionary.Add("[7]", 7);
                        dictionary.Add("[8]", 8);
                        dictionary.Add("[9]", 9);
                        dictionary.Add("[.]", 10);
                        dictionary.Add("[/]", 11);
                        dictionary.Add("[-]", 12);
                        dictionary.Add("[+]", 13);
                        dictionary.Add("[=]", 14);
                        dictionary.Add("[equals]", 15);
                        dictionary.Add("[enter]", 0x10);
                        dictionary.Add("up", 0x11);
                        dictionary.Add("down", 0x12);
                        dictionary.Add("left", 0x13);
                        dictionary.Add("right", 20);
                        dictionary.Add("insert", 0x15);
                        dictionary.Add("home", 0x16);
                        dictionary.Add("end", 0x17);
                        dictionary.Add("pgup", 0x18);
                        dictionary.Add("page up", 0x19);
                        dictionary.Add("pgdown", 0x1a);
                        dictionary.Add("page down", 0x1b);
                        dictionary.Add("backspace", 0x1c);
                        dictionary.Add("delete", 0x1d);
                        dictionary.Add("tab", 30);
                        dictionary.Add("f1", 0x1f);
                        dictionary.Add("f2", 0x20);
                        dictionary.Add("f3", 0x21);
                        dictionary.Add("f4", 0x22);
                        dictionary.Add("f5", 0x23);
                        dictionary.Add("f6", 0x24);
                        dictionary.Add("f7", 0x25);
                        dictionary.Add("f8", 0x26);
                        dictionary.Add("f9", 0x27);
                        dictionary.Add("f10", 40);
                        dictionary.Add("f11", 0x29);
                        dictionary.Add("f12", 0x2a);
                        dictionary.Add("f13", 0x2b);
                        dictionary.Add("f14", 0x2c);
                        dictionary.Add("f15", 0x2d);
                        dictionary.Add("[esc]", 0x2e);
                        dictionary.Add("return", 0x2f);
                        dictionary.Add("space", 0x30);
                        <>f__switch$map0 = dictionary;
                    }
                    if (<>f__switch$map0.TryGetValue(str2, out num2))
                    {
                        switch (num2)
                        {
                            case 0:
                                event2.character = '0';
                                event2.keyCode = KeyCode.Keypad0;
                                return event2;

                            case 1:
                                event2.character = '1';
                                event2.keyCode = KeyCode.Keypad1;
                                return event2;

                            case 2:
                                event2.character = '2';
                                event2.keyCode = KeyCode.Keypad2;
                                return event2;

                            case 3:
                                event2.character = '3';
                                event2.keyCode = KeyCode.Keypad3;
                                return event2;

                            case 4:
                                event2.character = '4';
                                event2.keyCode = KeyCode.Keypad4;
                                return event2;

                            case 5:
                                event2.character = '5';
                                event2.keyCode = KeyCode.Keypad5;
                                return event2;

                            case 6:
                                event2.character = '6';
                                event2.keyCode = KeyCode.Keypad6;
                                return event2;

                            case 7:
                                event2.character = '7';
                                event2.keyCode = KeyCode.Keypad7;
                                return event2;

                            case 8:
                                event2.character = '8';
                                event2.keyCode = KeyCode.Keypad8;
                                return event2;

                            case 9:
                                event2.character = '9';
                                event2.keyCode = KeyCode.Keypad9;
                                return event2;

                            case 10:
                                event2.character = '.';
                                event2.keyCode = KeyCode.KeypadPeriod;
                                return event2;

                            case 11:
                                event2.character = '/';
                                event2.keyCode = KeyCode.KeypadDivide;
                                return event2;

                            case 12:
                                event2.character = '-';
                                event2.keyCode = KeyCode.KeypadMinus;
                                return event2;

                            case 13:
                                event2.character = '+';
                                event2.keyCode = KeyCode.KeypadPlus;
                                return event2;

                            case 14:
                                event2.character = '=';
                                event2.keyCode = KeyCode.KeypadEquals;
                                return event2;

                            case 15:
                                event2.character = '=';
                                event2.keyCode = KeyCode.KeypadEquals;
                                return event2;

                            case 0x10:
                                event2.character = '\n';
                                event2.keyCode = KeyCode.KeypadEnter;
                                return event2;

                            case 0x11:
                                event2.keyCode = KeyCode.UpArrow;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x12:
                                event2.keyCode = KeyCode.DownArrow;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x13:
                                event2.keyCode = KeyCode.LeftArrow;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 20:
                                event2.keyCode = KeyCode.RightArrow;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x15:
                                event2.keyCode = KeyCode.Insert;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x16:
                                event2.keyCode = KeyCode.Home;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x17:
                                event2.keyCode = KeyCode.End;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x18:
                                event2.keyCode = KeyCode.PageDown;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x19:
                                event2.keyCode = KeyCode.PageUp;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x1a:
                                event2.keyCode = KeyCode.PageUp;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x1b:
                                event2.keyCode = KeyCode.PageDown;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x1c:
                                event2.keyCode = KeyCode.Backspace;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x1d:
                                event2.keyCode = KeyCode.Delete;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 30:
                                event2.keyCode = KeyCode.Tab;
                                return event2;

                            case 0x1f:
                                event2.keyCode = KeyCode.F1;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x20:
                                event2.keyCode = KeyCode.F2;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x21:
                                event2.keyCode = KeyCode.F3;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x22:
                                event2.keyCode = KeyCode.F4;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x23:
                                event2.keyCode = KeyCode.F5;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x24:
                                event2.keyCode = KeyCode.F6;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x25:
                                event2.keyCode = KeyCode.F7;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x26:
                                event2.keyCode = KeyCode.F8;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x27:
                                event2.keyCode = KeyCode.F9;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 40:
                                event2.keyCode = KeyCode.F10;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x29:
                                event2.keyCode = KeyCode.F11;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2a:
                                event2.keyCode = KeyCode.F12;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2b:
                                event2.keyCode = KeyCode.F13;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2c:
                                event2.keyCode = KeyCode.F14;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2d:
                                event2.keyCode = KeyCode.F15;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2e:
                                event2.keyCode = KeyCode.Escape;
                                return event2;

                            case 0x2f:
                                event2.character = '\n';
                                event2.keyCode = KeyCode.Return;
                                event2.modifiers &= ~EventModifiers.FunctionKey;
                                return event2;

                            case 0x30:
                                event2.keyCode = KeyCode.Space;
                                event2.character = ' ';
                                event2.modifiers &= ~EventModifiers.FunctionKey;
                                return event2;
                        }
                    }
                }
                if (str.Length != 1)
                {
                    try
                    {
                        event2.keyCode = (KeyCode) ((int) Enum.Parse(typeof(KeyCode), str, true));
                    }
                    catch (ArgumentException)
                    {
                        object[] args = new object[] { str };
                        Debug.LogError(UnityString.Format("Unable to find key name that matches '{0}'", args));
                    }
                    return event2;
                }
                event2.character = str.ToLower()[0];
                event2.keyCode = (KeyCode) event2.character;
                if (event2.modifiers != EventModifiers.None)
                {
                    event2.character = '\0';
                }
            }
            return event2;
        }

        public override int GetHashCode()
        {
            int keyCode = 1;
            if (this.isKey)
            {
                keyCode = (ushort) this.keyCode;
            }
            if (this.isMouse)
            {
                keyCode = this.mousePosition.GetHashCode();
            }
            return ((keyCode * 0x25) | this.modifiers);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != base.GetType())
            {
                return false;
            }
            Event event2 = (Event) obj;
            if ((this.type != event2.type) || ((this.modifiers & ~EventModifiers.CapsLock) != (event2.modifiers & ~EventModifiers.CapsLock)))
            {
                return false;
            }
            if (this.isKey)
            {
                return (this.keyCode == event2.keyCode);
            }
            return (this.isMouse && (this.mousePosition == event2.mousePosition));
        }

        public override string ToString()
        {
            if (this.isKey)
            {
                if (this.character == '\0')
                {
                    object[] objArray1 = new object[] { this.type, this.modifiers, this.keyCode };
                    return UnityString.Format(@"Event:{0}   Character:\0   Modifiers:{1}   KeyCode:{2}", objArray1);
                }
                object[] objArray2 = new object[] { "Event:", this.type, "   Character:", (int) this.character, "   Modifiers:", this.modifiers, "   KeyCode:", this.keyCode };
                return string.Concat(objArray2);
            }
            if (this.isMouse)
            {
                object[] objArray3 = new object[] { this.type, this.mousePosition, this.modifiers };
                return UnityString.Format("Event: {0}   Position: {1} Modifiers: {2}", objArray3);
            }
            if ((this.type != EventType.ExecuteCommand) && (this.type != EventType.ValidateCommand))
            {
                return (string.Empty + this.type);
            }
            object[] args = new object[] { this.type, this.commandName };
            return UnityString.Format("Event: {0}  \"{1}\"", args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Init(int displayIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Cleanup();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InitCopy(Event other);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InitPtr(IntPtr ptr);
        public EventType rawType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public EventType type { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern EventType GetTypeForControl(int controlID);
        private void Internal_SetMousePosition(Vector2 value)
        {
            INTERNAL_CALL_Internal_SetMousePosition(this, ref value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_SetMousePosition(Event self, ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_GetMousePosition(out Vector2 value);
        private void Internal_SetMouseDelta(Vector2 value)
        {
            INTERNAL_CALL_Internal_SetMouseDelta(this, ref value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_SetMouseDelta(Event self, ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_GetMouseDelta(out Vector2 value);
        public int button { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public EventModifiers modifiers { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public float pressure { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public int clickCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public char character { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public string commandName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public KeyCode keyCode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetNativeEvent(IntPtr ptr);
        public int displayIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Use();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool PopEvent(Event outEvent);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetEventCount();
    }
}

