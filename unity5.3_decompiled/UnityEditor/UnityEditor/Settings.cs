namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class Settings
    {
        private static SortedList<string, object> m_Prefs = new SortedList<string, object>();

        internal static T Get<T>(string name, T defaultValue) where T: IPrefType, new()
        {
            if (defaultValue == null)
            {
                throw new ArgumentException("default can not be null", "defaultValue");
            }
            if (m_Prefs.ContainsKey(name))
            {
                return (T) m_Prefs[name];
            }
            string sstr = EditorPrefs.GetString(name, string.Empty);
            if (sstr == string.Empty)
            {
                Set<T>(name, defaultValue);
                return defaultValue;
            }
            defaultValue.FromUniqueString(sstr);
            Set<T>(name, defaultValue);
            return defaultValue;
        }

        [DebuggerHidden]
        internal static IEnumerable<KeyValuePair<string, T>> Prefs<T>() where T: IPrefType
        {
            return new <Prefs>c__Iterator4<T> { $PC = -2 };
        }

        internal static void Set<T>(string name, T value) where T: IPrefType
        {
            EditorPrefs.SetString(name, value.ToUniqueString());
            m_Prefs[name] = value;
        }

        [CompilerGenerated]
        private sealed class <Prefs>c__Iterator4<T> : IDisposable, IEnumerator, IEnumerable, IEnumerable<KeyValuePair<string, T>>, IEnumerator<KeyValuePair<string, T>> where T: IPrefType
        {
            internal KeyValuePair<string, T> $current;
            internal int $PC;
            internal IEnumerator<KeyValuePair<string, object>> <$s_445>__0;
            internal KeyValuePair<string, object> <kvp>__1;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            if (this.<$s_445>__0 == null)
                            {
                            }
                            this.<$s_445>__0.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.<$s_445>__0 = Settings.m_Prefs.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00D6;
                }
                try
                {
                    while (this.<$s_445>__0.MoveNext())
                    {
                        this.<kvp>__1 = this.<$s_445>__0.Current;
                        if (this.<kvp>__1.Value is T)
                        {
                            this.$current = new KeyValuePair<string, T>(this.<kvp>__1.Key, (T) this.<kvp>__1.Value);
                            this.$PC = 1;
                            flag = true;
                            return true;
                        }
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.<$s_445>__0 == null)
                    {
                    }
                    this.<$s_445>__0.Dispose();
                }
                this.$PC = -1;
            Label_00D6:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new Settings.<Prefs>c__Iterator4<T>();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string,T>>.GetEnumerator();
            }

            KeyValuePair<string, T> IEnumerator<KeyValuePair<string, T>>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

