namespace UnityEditor
{
    using System;
    using UnityEditor.Audio;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AudioMixerExposedParameterView
    {
        private AudioMixerController m_Controller;
        private SerializedObject m_ControllerSerialized;
        private ReorderableListWithRenameAndScrollView m_ReorderableListWithRenameAndScrollView;
        private ReorderableListWithRenameAndScrollView.State m_State;

        public AudioMixerExposedParameterView(ReorderableListWithRenameAndScrollView.State state)
        {
            this.m_State = state;
        }

        public Vector2 CalcSize()
        {
            float x = 0f;
            for (int i = 0; i < this.m_ReorderableListWithRenameAndScrollView.list.count; i++)
            {
                float num3 = this.WidthOfRow(i, this.m_ReorderableListWithRenameAndScrollView.elementStyle, this.m_ReorderableListWithRenameAndScrollView.elementStyleRightAligned);
                if (num3 > x)
                {
                    x = num3;
                }
            }
            return new Vector2(x, this.height);
        }

        private void Delete(int index)
        {
            if (this.m_Controller != null)
            {
                Undo.RecordObject(this.m_Controller, "Unexpose Mixer Parameter");
                ExposedAudioParameter parameter = this.m_Controller.exposedParameters[index];
                this.m_Controller.RemoveExposedParameter(parameter.guid);
            }
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            Event current = Event.current;
            if ((current.type == EventType.ContextClick) && rect.Contains(current.mousePosition))
            {
                this.OnContextClick(index);
                current.Use();
            }
            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.BeginDisabledGroup(true);
                this.m_ReorderableListWithRenameAndScrollView.elementStyleRightAligned.Draw(rect, this.GetInfoString(index), false, false, false, false);
                EditorGUI.EndDisabledGroup();
            }
        }

        public void EndDragChild(ReorderableList list)
        {
            this.m_ControllerSerialized.ApplyModifiedProperties();
        }

        private string GetInfoString(int index)
        {
            ExposedAudioParameter parameter = this.m_Controller.exposedParameters[index];
            return this.m_Controller.ResolveExposedParameterPath(parameter.guid, false);
        }

        private string GetNameOfElement(int index)
        {
            ExposedAudioParameter parameter = this.m_Controller.exposedParameters[index];
            return parameter.name;
        }

        public void NameChanged(int index, string newName)
        {
            if (newName.Length > 0x40)
            {
                newName = newName.Substring(0, 0x40);
                Debug.LogWarning(string.Concat(new object[] { "Maximum name length of an exposed parameter is ", 0x40, " characters. Name truncated to '", newName, "'" }));
            }
            ExposedAudioParameter[] exposedParameters = this.m_Controller.exposedParameters;
            exposedParameters[index].name = newName;
            this.m_Controller.exposedParameters = exposedParameters;
        }

        public void OnContextClick(int itemIndex)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Unexpose"), false, data => this.Delete((int) data), itemIndex);
            menu.AddItem(new GUIContent("Rename"), false, data => this.m_ReorderableListWithRenameAndScrollView.BeginRename((int) data, 0f), itemIndex);
            menu.ShowAsContext();
        }

        public void OnEvent()
        {
            this.m_ReorderableListWithRenameAndScrollView.OnEvent();
        }

        public void OnGUI(Rect rect)
        {
            if (this.m_Controller != null)
            {
                this.m_ReorderableListWithRenameAndScrollView.OnGUI(rect);
            }
        }

        public void OnMixerControllerChanged(AudioMixerController controller)
        {
            this.m_Controller = controller;
            if (this.m_Controller != null)
            {
                this.m_Controller.ChangedExposedParameter += new ChangedExposedParameterHandler(this.RecreateListControl);
            }
            this.RecreateListControl();
        }

        public void RecreateListControl()
        {
            if (this.m_Controller != null)
            {
                ReorderableList list;
                this.m_ControllerSerialized = new SerializedObject(this.m_Controller);
                SerializedProperty elements = this.m_ControllerSerialized.FindProperty("m_ExposedParameters");
                list = new ReorderableList(this.m_ControllerSerialized, elements, false, false, false, false) {
                    onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild),
                    drawElementCallback = (ReorderableList.ElementCallbackDelegate) Delegate.Combine(list.drawElementCallback, new ReorderableList.ElementCallbackDelegate(this.DrawElement)),
                    elementHeight = 16f,
                    headerHeight = 0f,
                    footerHeight = 0f,
                    showDefaultBackground = false
                };
                this.m_ReorderableListWithRenameAndScrollView = new ReorderableListWithRenameAndScrollView(list, this.m_State);
                this.m_ReorderableListWithRenameAndScrollView.onNameChangedAtIndex = (Action<int, string>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onNameChangedAtIndex, new Action<int, string>(this.NameChanged));
                this.m_ReorderableListWithRenameAndScrollView.onDeleteItemAtIndex = (Action<int>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onDeleteItemAtIndex, new Action<int>(this.Delete));
                this.m_ReorderableListWithRenameAndScrollView.onGetNameAtIndex = (Func<int, string>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onGetNameAtIndex, new Func<int, string>(this.GetNameOfElement));
            }
        }

        private float WidthOfRow(int index, GUIStyle leftStyle, GUIStyle rightStyle)
        {
            string infoString = this.GetInfoString(index);
            Vector2 vector = rightStyle.CalcSize(GUIContent.Temp(infoString));
            return ((leftStyle.CalcSize(GUIContent.Temp(this.GetNameOfElement(index))).x + vector.x) + 25f);
        }

        private float height
        {
            get
            {
                return this.m_ReorderableListWithRenameAndScrollView.list.GetHeight();
            }
        }
    }
}

