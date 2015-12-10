namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Hardware;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(EditorSettings))]
    internal class EditorSettingsInspector : Editor
    {
        private PopupElement[] behaviorPopupList = new PopupElement[] { new PopupElement("3D"), new PopupElement("2D") };
        private string[] logLevelPopupList = new string[] { "Verbose", "Info", "Notice", "Fatal" };
        private PopupElement[] remoteCompressionList = new PopupElement[] { new PopupElement("JPEG"), new PopupElement("PNG") };
        private DevDevice[] remoteDeviceList;
        private PopupElement[] remoteDevicePopupList;
        private PopupElement[] remoteResolutionList = new PopupElement[] { new PopupElement("Normal"), new PopupElement("Downsize") };
        private string[] semanticMergePopupList = new string[] { "Off", "Premerge", "Ask" };
        private PopupElement[] serializationPopupList = new PopupElement[] { new PopupElement("Mixed"), new PopupElement("Force Binary"), new PopupElement("Force Text") };
        private PopupElement[] spritePackerPaddingPowerPopupList = new PopupElement[] { new PopupElement("1"), new PopupElement("2"), new PopupElement("3") };
        private PopupElement[] spritePackerPopupList = new PopupElement[] { new PopupElement("Disabled"), new PopupElement("Enabled For Builds"), new PopupElement("Always Enabled") };
        public readonly GUIContent spritePaddingPower = EditorGUIUtility.TextContent("Padding Power|Set Padding Power if Atlas is enabled.");
        private PopupElement[] vcDefaultPopupList = new PopupElement[] { new PopupElement(ExternalVersionControl.Disabled), new PopupElement(ExternalVersionControl.Generic), new PopupElement(ExternalVersionControl.AssetServer, true) };
        private PopupElement[] vcPopupList;

        private void BuildRemoteDeviceList()
        {
            List<DevDevice> list = new List<DevDevice>();
            List<PopupElement> list2 = new List<PopupElement> {
                DevDevice.none,
                new PopupElement("None"),
                new DevDevice("Any Android Device", "Any Android Device", "Android", "Android", DevDeviceState.Connected, DevDeviceFeatures.RemoteConnection),
                new PopupElement("Any Android Device")
            };
            foreach (DevDevice device in DevDeviceList.GetDevices())
            {
                bool flag = (device.features & DevDeviceFeatures.RemoteConnection) != DevDeviceFeatures.None;
                if (device.isConnected && flag)
                {
                    list.Add(device);
                    list2.Add(new PopupElement(device.name));
                }
            }
            this.remoteDeviceList = list.ToArray();
            this.remoteDevicePopupList = list2.ToArray();
        }

        private void DoPopup(Rect popupRect, PopupElement[] elements, int selectedIndex, GenericMenu.MenuFunction2 func)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < elements.Length; i++)
            {
                PopupElement element = elements[i];
                if (element.Enabled)
                {
                    menu.AddItem(element.content, i == selectedIndex, func, i);
                }
                else
                {
                    menu.AddDisabledItem(element.content);
                }
            }
            menu.DropDown(popupRect);
        }

        private void DoProjectGenerationSettings()
        {
            GUILayout.Space(10f);
            GUILayout.Label("C# Project Generation", EditorStyles.boldLabel, new GUILayoutOption[0]);
            string text = EditorSettings.Internal_ProjectGenerationUserExtensions;
            string str2 = EditorGUILayout.TextField("Additional extensions to include", text, new GUILayoutOption[0]);
            if (str2 != text)
            {
                EditorSettings.Internal_ProjectGenerationUserExtensions = str2;
            }
            text = EditorSettings.projectGenerationRootNamespace;
            str2 = EditorGUILayout.TextField("Root namespace", text, new GUILayoutOption[0]);
            if (str2 != text)
            {
                EditorSettings.projectGenerationRootNamespace = str2;
            }
        }

        private void DrawOverlayDescription(Asset.States state)
        {
            Rect atlasRectForState = Provider.GetAtlasRectForState((int) state);
            if (atlasRectForState.width != 0f)
            {
                Texture2D overlayAtlas = Provider.overlayAtlas;
                if (overlayAtlas != null)
                {
                    GUILayout.Label("    " + Asset.StateToString(state), EditorStyles.miniLabel, new GUILayoutOption[0]);
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    lastRect.width = 16f;
                    GUI.DrawTextureWithTexCoords(lastRect, overlayAtlas, atlasRectForState);
                }
            }
        }

        private void DrawOverlayDescriptions()
        {
            if (Provider.overlayAtlas != null)
            {
                GUILayout.Space(10f);
                GUILayout.Label("Overlay legends", EditorStyles.boldLabel, new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                this.DrawOverlayDescription(Asset.States.Local);
                this.DrawOverlayDescription(Asset.States.OutOfSync);
                this.DrawOverlayDescription(Asset.States.CheckedOutLocal);
                this.DrawOverlayDescription(Asset.States.CheckedOutRemote);
                this.DrawOverlayDescription(Asset.States.DeletedLocal);
                this.DrawOverlayDescription(Asset.States.DeletedRemote);
                GUILayout.EndVertical();
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                this.DrawOverlayDescription(Asset.States.AddedLocal);
                this.DrawOverlayDescription(Asset.States.AddedRemote);
                this.DrawOverlayDescription(Asset.States.Conflicted);
                this.DrawOverlayDescription(Asset.States.LockedLocal);
                this.DrawOverlayDescription(Asset.States.LockedRemote);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        private static int GetIndexById(PopupElement[] elements, string id, int defaultIndex)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].id == id)
                {
                    return i;
                }
            }
            return defaultIndex;
        }

        private static int GetIndexById(DevDevice[] elements, string id, int defaultIndex)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].id == id)
                {
                    return i;
                }
            }
            return defaultIndex;
        }

        private void OnDeviceListChanged()
        {
            this.BuildRemoteDeviceList();
        }

        public void OnDisable()
        {
            DevDeviceList.Changed -= new DevDeviceList.OnChangedHandler(this.OnDeviceListChanged);
        }

        public void OnEnable()
        {
            Plugin[] availablePlugins = Plugin.availablePlugins;
            this.vcPopupList = new PopupElement[availablePlugins.Length + this.vcDefaultPopupList.Length];
            Array.Copy(this.vcDefaultPopupList, this.vcPopupList, this.vcDefaultPopupList.Length);
            int index = 0;
            int length = this.vcDefaultPopupList.Length;
            while (length < this.vcPopupList.Length)
            {
                this.vcPopupList[length] = new PopupElement(availablePlugins[index].name, true);
                length++;
                index++;
            }
            DevDeviceList.Changed += new DevDeviceList.OnChangedHandler(this.OnDeviceListChanged);
            this.BuildRemoteDeviceList();
        }

        public override void OnInspectorGUI()
        {
            <OnInspectorGUI>c__AnonStorey87 storey = new <OnInspectorGUI>c__AnonStorey87();
            bool enabled = GUI.enabled;
            this.ShowUnityRemoteGUI(enabled);
            GUILayout.Space(10f);
            GUI.enabled = true;
            GUILayout.Label("Version Control", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUI.enabled = enabled;
            storey.selvc = EditorSettings.externalVersionControl;
            int index = Array.FindIndex<PopupElement>(this.vcPopupList, new Predicate<PopupElement>(storey.<>m__153));
            if (index < 0)
            {
                index = 0;
            }
            GUIContent content = new GUIContent(this.vcPopupList[index].content);
            Rect position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Mode"));
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.vcPopupList, index, new GenericMenu.MenuFunction2(this.SetVersionControlSystem));
            }
            if (this.VersionControlSystemHasGUI())
            {
                <OnInspectorGUI>c__AnonStorey88 storey2 = new <OnInspectorGUI>c__AnonStorey88();
                GUI.enabled = true;
                bool flag2 = false;
                if (EditorSettings.externalVersionControl == ExternalVersionControl.AssetServer)
                {
                    EditorUserSettings.SetConfigValue("vcUsername", EditorGUILayout.TextField("User", EditorUserSettings.GetConfigValue("vcUsername"), new GUILayoutOption[0]));
                    EditorUserSettings.SetConfigValue("vcPassword", EditorGUILayout.PasswordField("Password", EditorUserSettings.GetConfigValue("vcPassword"), new GUILayoutOption[0]));
                }
                else if ((EditorSettings.externalVersionControl != ExternalVersionControl.Generic) && (EditorSettings.externalVersionControl != ExternalVersionControl.Disabled))
                {
                    ConfigField[] activeConfigFields = Provider.GetActiveConfigFields();
                    flag2 = true;
                    foreach (ConfigField field in activeConfigFields)
                    {
                        string configValue = EditorUserSettings.GetConfigValue(field.name);
                        string str2 = configValue;
                        if (field.isPassword)
                        {
                            str2 = EditorGUILayout.PasswordField(field.label, configValue, new GUILayoutOption[0]);
                        }
                        else
                        {
                            str2 = EditorGUILayout.TextField(field.label, configValue, new GUILayoutOption[0]);
                        }
                        if (str2 != configValue)
                        {
                            EditorUserSettings.SetConfigValue(field.name, str2);
                        }
                        if (field.isRequired && string.IsNullOrEmpty(str2))
                        {
                            flag2 = false;
                        }
                    }
                }
                storey2.logLevel = EditorUserSettings.GetConfigValue("vcSharedLogLevel");
                int num3 = Array.FindIndex<string>(this.logLevelPopupList, new Predicate<string>(storey2.<>m__154));
                if (num3 == -1)
                {
                    storey2.logLevel = "info";
                }
                int num4 = EditorGUILayout.Popup("Log Level", Math.Abs(num3), this.logLevelPopupList, new GUILayoutOption[0]);
                if (num4 != num3)
                {
                    EditorUserSettings.SetConfigValue("vcSharedLogLevel", this.logLevelPopupList[num4].ToLower());
                }
                GUI.enabled = enabled;
                string str3 = "Connected";
                if (Provider.onlineState == OnlineState.Updating)
                {
                    str3 = "Connecting...";
                }
                else if (Provider.onlineState == OnlineState.Offline)
                {
                    str3 = "Disconnected";
                }
                EditorGUILayout.LabelField("Status", str3, new GUILayoutOption[0]);
                if ((Provider.onlineState != OnlineState.Online) && !string.IsNullOrEmpty(Provider.offlineReason))
                {
                    GUI.enabled = false;
                    GUILayout.TextArea(Provider.offlineReason, new GUILayoutOption[0]);
                    GUI.enabled = enabled;
                }
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUI.enabled = flag2;
                if (GUILayout.Button("Connect", EditorStyles.miniButton, new GUILayoutOption[0]))
                {
                    Provider.UpdateSettings();
                }
                GUILayout.EndHorizontal();
                EditorUserSettings.AutomaticAdd = EditorGUILayout.Toggle("Automatic add", EditorUserSettings.AutomaticAdd, new GUILayoutOption[0]);
                if (Provider.requiresNetwork)
                {
                    bool flag3 = EditorGUILayout.Toggle("Work Offline", EditorUserSettings.WorkOffline, new GUILayoutOption[0]);
                    if (flag3 != EditorUserSettings.WorkOffline)
                    {
                        if (flag3 && !EditorUtility.DisplayDialog("Confirm working offline", "Working offline and making changes to your assets means that you will have to manually integrate changes back into version control using your standard version control client before you stop working offline in Unity. Make sure you know what you are doing.", "Work offline", "Cancel"))
                        {
                            flag3 = false;
                        }
                        EditorUserSettings.WorkOffline = flag3;
                        EditorApplication.RequestRepaintAllViews();
                    }
                }
                if (Provider.hasCheckoutSupport)
                {
                    EditorUserSettings.showFailedCheckout = EditorGUILayout.Toggle("Show failed checkouts", EditorUserSettings.showFailedCheckout, new GUILayoutOption[0]);
                }
                GUI.enabled = enabled;
                EditorUserSettings.semanticMergeMode = (SemanticMergeMode) EditorGUILayout.Popup("Smart merge", (int) EditorUserSettings.semanticMergeMode, this.semanticMergePopupList, new GUILayoutOption[0]);
                this.DrawOverlayDescriptions();
            }
            GUILayout.Space(10f);
            GUILayout.Label("WWW Security Emulation", EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorSettings.webSecurityEmulationEnabled = EditorGUILayout.Toggle("Enable Webplayer Security Emulation", EditorSettings.webSecurityEmulationEnabled, new GUILayoutOption[0]);
            string str4 = EditorGUILayout.TextField("Host URL", EditorSettings.webSecurityEmulationHostUrl, new GUILayoutOption[0]);
            if (str4 != EditorSettings.webSecurityEmulationHostUrl)
            {
                EditorSettings.webSecurityEmulationHostUrl = str4;
            }
            GUILayout.Space(10f);
            GUI.enabled = true;
            GUILayout.Label("Asset Serialization", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUI.enabled = enabled;
            content = new GUIContent(this.serializationPopupList[(int) EditorSettings.serializationMode].content);
            position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Mode"));
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.serializationPopupList, (int) EditorSettings.serializationMode, new GenericMenu.MenuFunction2(this.SetAssetSerializationMode));
            }
            GUILayout.Space(10f);
            GUI.enabled = true;
            GUILayout.Label("Default Behavior Mode", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUI.enabled = enabled;
            int num5 = Mathf.Clamp((int) EditorSettings.defaultBehaviorMode, 0, this.behaviorPopupList.Length - 1);
            content = new GUIContent(this.behaviorPopupList[num5].content);
            position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Mode"));
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.behaviorPopupList, num5, new GenericMenu.MenuFunction2(this.SetDefaultBehaviorMode));
            }
            GUILayout.Space(10f);
            GUI.enabled = true;
            GUILayout.Label("Sprite Packer", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUI.enabled = enabled;
            num5 = Mathf.Clamp((int) EditorSettings.spritePackerMode, 0, this.spritePackerPopupList.Length - 1);
            content = new GUIContent(this.spritePackerPopupList[num5].content);
            position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Mode"));
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.spritePackerPopupList, num5, new GenericMenu.MenuFunction2(this.SetSpritePackerMode));
            }
            num5 = Mathf.Clamp(EditorSettings.spritePackerPaddingPower - 1, 0, 2);
            content = new GUIContent(this.spritePackerPaddingPowerPopupList[num5].content);
            position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Padding Power"));
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.spritePackerPaddingPowerPopupList, num5, new GenericMenu.MenuFunction2(this.SetSpritePackerPaddingPower));
            }
            this.DoProjectGenerationSettings();
        }

        private void SetAssetSerializationMode(object data)
        {
            int num = (int) data;
            EditorSettings.serializationMode = (SerializationMode) num;
        }

        private void SetDefaultBehaviorMode(object data)
        {
            int num = (int) data;
            EditorSettings.defaultBehaviorMode = (EditorBehaviorMode) num;
        }

        private void SetSpritePackerMode(object data)
        {
            int num = (int) data;
            EditorSettings.spritePackerMode = (SpritePackerMode) num;
        }

        private void SetSpritePackerPaddingPower(object data)
        {
            int num = (int) data;
            EditorSettings.spritePackerPaddingPower = num + 1;
        }

        private void SetUnityRemoteCompression(object data)
        {
            EditorSettings.unityRemoteCompression = this.remoteCompressionList[(int) data].id;
        }

        private void SetUnityRemoteDevice(object data)
        {
            EditorSettings.unityRemoteDevice = this.remoteDeviceList[(int) data].id;
        }

        private void SetUnityRemoteResolution(object data)
        {
            EditorSettings.unityRemoteResolution = this.remoteResolutionList[(int) data].id;
        }

        private void SetVersionControlSystem(object data)
        {
            int index = (int) data;
            if ((index >= 0) || (index < this.vcPopupList.Length))
            {
                PopupElement element = this.vcPopupList[index];
                string externalVersionControl = EditorSettings.externalVersionControl;
                EditorSettings.externalVersionControl = element.content.text;
                Provider.UpdateSettings();
                AssetDatabase.Refresh();
                if (externalVersionControl != element.content.text)
                {
                    if (((element.content.text == ExternalVersionControl.AssetServer) || (element.content.text == ExternalVersionControl.Disabled)) || (element.content.text == ExternalVersionControl.Generic))
                    {
                        WindowPending.CloseAllWindows();
                    }
                    else
                    {
                        ASMainWindow[] windowArray = Resources.FindObjectsOfTypeAll(typeof(ASMainWindow)) as ASMainWindow[];
                        ASMainWindow window = (windowArray.Length <= 0) ? null : windowArray[0];
                        if (window != null)
                        {
                            window.Close();
                        }
                    }
                }
            }
        }

        private void ShowUnityRemoteGUI(bool editorEnabled)
        {
            GUI.enabled = true;
            GUILayout.Label("Unity Remote", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUI.enabled = editorEnabled;
            string unityRemoteDevice = EditorSettings.unityRemoteDevice;
            int index = GetIndexById(this.remoteDeviceList, unityRemoteDevice, 0);
            GUIContent content = new GUIContent(this.remoteDevicePopupList[index].content);
            Rect position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Device"));
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.remoteDevicePopupList, index, new GenericMenu.MenuFunction2(this.SetUnityRemoteDevice));
            }
            int num2 = GetIndexById(this.remoteCompressionList, EditorSettings.unityRemoteCompression, 0);
            content = new GUIContent(this.remoteCompressionList[num2].content);
            position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Compression"));
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.remoteCompressionList, num2, new GenericMenu.MenuFunction2(this.SetUnityRemoteCompression));
            }
            int num3 = GetIndexById(this.remoteResolutionList, EditorSettings.unityRemoteResolution, 0);
            content = new GUIContent(this.remoteResolutionList[num3].content);
            position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Resolution"));
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.remoteResolutionList, num3, new GenericMenu.MenuFunction2(this.SetUnityRemoteResolution));
            }
        }

        private bool VersionControlSystemHasGUI()
        {
            ExternalVersionControl externalVersionControl = EditorSettings.externalVersionControl;
            return ((((externalVersionControl != ExternalVersionControl.Disabled) && (externalVersionControl != ExternalVersionControl.AutoDetect)) && (externalVersionControl != ExternalVersionControl.AssetServer)) && (externalVersionControl != ExternalVersionControl.Generic));
        }

        [CompilerGenerated]
        private sealed class <OnInspectorGUI>c__AnonStorey87
        {
            internal ExternalVersionControl selvc;

            internal bool <>m__153(EditorSettingsInspector.PopupElement cand)
            {
                return (cand.content.text == this.selvc);
            }
        }

        [CompilerGenerated]
        private sealed class <OnInspectorGUI>c__AnonStorey88
        {
            internal string logLevel;

            internal bool <>m__154(string item)
            {
                return (item.ToLower() == this.logLevel);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PopupElement
        {
            public readonly string id;
            public readonly bool requiresTeamLicense;
            public readonly GUIContent content;
            public PopupElement(string content) : this(content, false)
            {
            }

            public PopupElement(string content, bool requiresTeamLicense)
            {
                this.id = content;
                this.content = new GUIContent(content);
                this.requiresTeamLicense = requiresTeamLicense;
            }

            public bool Enabled
            {
                get
                {
                    return (!this.requiresTeamLicense || InternalEditorUtility.HasTeamLicense());
                }
            }
        }
    }
}

