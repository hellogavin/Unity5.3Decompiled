namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PrefKey : IPrefType
    {
        private string m_DefaultShortcut;
        private Event m_event;
        private string m_name;

        public PrefKey()
        {
        }

        public PrefKey(string name, string shortcut)
        {
            this.m_name = name;
            this.m_event = Event.KeyboardEvent(shortcut);
            this.m_DefaultShortcut = shortcut;
            PrefKey key = Settings.Get<PrefKey>(name, this);
            this.m_name = key.Name;
            this.m_event = key.KeyboardEvent;
        }

        public void FromUniqueString(string s)
        {
            int index = s.IndexOf(";");
            if (index < 0)
            {
                Debug.LogError("Malformed string in Keyboard preferences");
            }
            else
            {
                this.m_name = s.Substring(0, index);
                this.m_event = Event.KeyboardEvent(s.Substring(index + 1));
            }
        }

        public static implicit operator Event(PrefKey pkey)
        {
            return pkey.m_event;
        }

        internal void ResetToDefault()
        {
            this.m_event = Event.KeyboardEvent(this.m_DefaultShortcut);
        }

        public string ToUniqueString()
        {
            object[] objArray1 = new object[] { this.m_name, ";", !this.m_event.alt ? string.Empty : "&", !this.m_event.command ? string.Empty : "%", !this.m_event.shift ? string.Empty : "#", !this.m_event.control ? string.Empty : "^", this.m_event.keyCode };
            return string.Concat(objArray1);
        }

        public bool activated
        {
            get
            {
                return (Event.current.Equals(this) && !GUIUtility.textFieldInput);
            }
        }

        public Event KeyboardEvent
        {
            get
            {
                return this.m_event;
            }
            set
            {
                this.m_event = value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }
    }
}

