namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Hardware;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AttachProfilerUI
    {
        [CompilerGenerated]
        private static Func<bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<ProfilerChoise, string> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<ProfilerChoise, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Predicate<ProfilerChoise> <>f__am$cache7;
        private static string kEnterIPText = "<Enter IP>";
        private GUIContent m_CurrentProfiler;
        private static GUIContent ms_NotificationMessage;
        private const int PLAYER_DIRECT_IP_CONNECT_GUID = 0xfeed;
        private const int PLAYER_DIRECT_URL_CONNECT_GUID = 0xfeee;

        private static void AddDeviceProfilers(List<ProfilerChoise> profilers)
        {
            foreach (DevDevice device in DevDeviceList.GetDevices())
            {
                <AddDeviceProfilers>c__AnonStoreyA1 ya = new <AddDeviceProfilers>c__AnonStoreyA1();
                bool flag = (device.features & DevDeviceFeatures.PlayerConnection) != DevDeviceFeatures.None;
                if (device.isConnected && flag)
                {
                    ya.url = "device://" + device.id;
                    ProfilerChoise item = new ProfilerChoise {
                        Name = device.name,
                        Enabled = true,
                        IsSelected = new Func<bool>(ya.<>m__1E2),
                        ConnectTo = new Action(ya.<>m__1E3)
                    };
                    profilers.Add(item);
                }
            }
        }

        private void AddEnterIPProfiler(List<ProfilerChoise> profilers, Rect buttonScreenRect)
        {
            <AddEnterIPProfiler>c__AnonStoreyA2 ya = new <AddEnterIPProfiler>c__AnonStoreyA2 {
                buttonScreenRect = buttonScreenRect
            };
            ProfilerChoise item = new ProfilerChoise {
                Name = kEnterIPText,
                Enabled = true
            };
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = () => false;
            }
            item.IsSelected = <>f__am$cache4;
            item.ConnectTo = new Action(ya.<>m__1E5);
            profilers.Add(item);
        }

        private static void AddLastIPProfiler(List<ProfilerChoise> profilers)
        {
            <AddLastIPProfiler>c__AnonStorey9F storeyf = new <AddLastIPProfiler>c__AnonStorey9F {
                lastIP = ProfilerIPWindow.GetLastIPString()
            };
            if (!string.IsNullOrEmpty(storeyf.lastIP))
            {
                ProfilerChoise item = new ProfilerChoise {
                    Name = storeyf.lastIP,
                    Enabled = true
                };
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = () => ProfilerDriver.connectedProfiler == 0xfeed;
                }
                item.IsSelected = <>f__am$cache3;
                item.ConnectTo = new Action(storeyf.<>m__1DF);
                profilers.Add(item);
            }
        }

        private static void AddPlayerProfilers(List<ProfilerChoise> profilers)
        {
            int[] availableProfilers = ProfilerDriver.GetAvailableProfilers();
            for (int i = 0; i < availableProfilers.Length; i++)
            {
                <AddPlayerProfilers>c__AnonStoreyA0 ya = new <AddPlayerProfilers>c__AnonStoreyA0 {
                    guid = availableProfilers[i]
                };
                string connectionIdentifier = ProfilerDriver.GetConnectionIdentifier(ya.guid);
                bool flag = ProfilerDriver.IsIdentifierOnLocalhost(ya.guid) && connectionIdentifier.Contains("MetroPlayerX");
                bool flag2 = !flag && ProfilerDriver.IsIdentifierConnectable(ya.guid);
                if (!flag2)
                {
                    if (flag)
                    {
                        connectionIdentifier = connectionIdentifier + " (Localhost prohibited)";
                    }
                    else
                    {
                        connectionIdentifier = connectionIdentifier + " (Version mismatch)";
                    }
                }
                ProfilerChoise item = new ProfilerChoise {
                    Name = connectionIdentifier,
                    Enabled = flag2,
                    IsSelected = new Func<bool>(ya.<>m__1E0),
                    ConnectTo = new Action(ya.<>m__1E1)
                };
                profilers.Add(item);
            }
        }

        public static void DirectIPConnect(string ip)
        {
            ConsoleWindow.ShowConsoleWindow(true);
            ms_NotificationMessage = new GUIContent("Connecting to player...(this can take a while)");
            ProfilerDriver.DirectIPConnect(ip);
            ms_NotificationMessage = null;
        }

        public static void DirectURLConnect(string url)
        {
            ConsoleWindow.ShowConsoleWindow(true);
            ms_NotificationMessage = new GUIContent("Connecting to player...(this can take a while)");
            ProfilerDriver.DirectURLConnect(url);
            ms_NotificationMessage = null;
        }

        public string GetConnectedProfiler()
        {
            return ProfilerDriver.GetConnectionIdentifier(ProfilerDriver.connectedProfiler);
        }

        public bool IsEditor()
        {
            return ProfilerDriver.IsConnectionEditor();
        }

        public void OnGUI(Rect connectRect, GUIContent profilerLabel)
        {
            if (EditorGUI.ButtonMouseDown(connectRect, profilerLabel, FocusType.Native, EditorStyles.toolbarDropDown))
            {
                int[] numArray;
                List<ProfilerChoise> profilers = new List<ProfilerChoise>();
                profilers.Clear();
                AddPlayerProfilers(profilers);
                AddDeviceProfilers(profilers);
                AddLastIPProfiler(profilers);
                this.AddEnterIPProfiler(profilers, GUIUtility.GUIToScreenRect(connectRect));
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = p => p.Name;
                }
                string[] options = profilers.Select<ProfilerChoise, string>(<>f__am$cache5).ToArray<string>();
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = p => p.Enabled;
                }
                bool[] enabled = profilers.Select<ProfilerChoise, bool>(<>f__am$cache6).ToArray<bool>();
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = p => p.IsSelected();
                }
                int num = profilers.FindIndex(<>f__am$cache7);
                if (num == -1)
                {
                    numArray = new int[0];
                }
                else
                {
                    numArray = new int[] { num };
                }
                EditorUtility.DisplayCustomMenu(connectRect, options, enabled, numArray, new EditorUtility.SelectMenuItemFunction(this.SelectProfilerClick), profilers);
            }
        }

        public void OnGUILayout(EditorWindow window)
        {
            if (this.m_CurrentProfiler == null)
            {
                this.m_CurrentProfiler = EditorGUIUtility.TextContent("Active Profiler|Select connected player to profile");
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(100f) };
            Rect connectRect = GUILayoutUtility.GetRect(this.m_CurrentProfiler, EditorStyles.toolbarDropDown, options);
            this.OnGUI(connectRect, this.m_CurrentProfiler);
            if (ms_NotificationMessage != null)
            {
                window.ShowNotification(ms_NotificationMessage);
            }
            else
            {
                window.RemoveNotification();
            }
        }

        private void SelectProfilerClick(object userData, string[] options, int selected)
        {
            List<ProfilerChoise> source = (List<ProfilerChoise>) userData;
            if (selected < source.Count<ProfilerChoise>())
            {
                ProfilerChoise choise = source[selected];
                choise.ConnectTo();
            }
        }

        [CompilerGenerated]
        private sealed class <AddDeviceProfilers>c__AnonStoreyA1
        {
            internal string url;

            internal bool <>m__1E2()
            {
                return ((ProfilerDriver.connectedProfiler == 0xfeee) && (ProfilerDriver.directConnectionUrl == this.url));
            }

            internal void <>m__1E3()
            {
                AttachProfilerUI.DirectURLConnect(this.url);
            }
        }

        [CompilerGenerated]
        private sealed class <AddEnterIPProfiler>c__AnonStoreyA2
        {
            internal Rect buttonScreenRect;

            internal void <>m__1E5()
            {
                ProfilerIPWindow.Show(this.buttonScreenRect);
            }
        }

        [CompilerGenerated]
        private sealed class <AddLastIPProfiler>c__AnonStorey9F
        {
            internal string lastIP;

            internal void <>m__1DF()
            {
                AttachProfilerUI.DirectIPConnect(this.lastIP);
            }
        }

        [CompilerGenerated]
        private sealed class <AddPlayerProfilers>c__AnonStoreyA0
        {
            internal int guid;

            internal bool <>m__1E0()
            {
                return (ProfilerDriver.connectedProfiler == this.guid);
            }

            internal void <>m__1E1()
            {
                ProfilerDriver.connectedProfiler = this.guid;
            }
        }
    }
}

