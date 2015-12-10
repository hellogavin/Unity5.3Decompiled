namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor.Audio;
    using UnityEngine;

    internal static class AudioMixerColorCodes
    {
        private static string[] colorNames = new string[] { "No Color", "Yellow", "Orange", "Red", "Magenta", "Violet", "Blue", "Cyan", "Green" };
        private static Color[] darkSkinColors = new Color[] { new Color(0.5f, 0.5f, 0.5f, 0.2f), new Color(1f, 0.8156863f, 0f), new Color(0.9607843f, 0.6117647f, 0.01568628f), new Color(1f, 0.2941177f, 0.227451f), new Color(1f, 0.3803922f, 0.6117647f), new Color(0.6588235f, 0.4470588f, 0.7176471f), new Color(0.05098039f, 0.6117647f, 0.8235294f), new Color(0f, 0.7450981f, 0.7843137f), new Color(0.5411765f, 0.7529412f, 0.003921569f) };
        private static Color[] lightSkinColors = new Color[] { new Color(0.5f, 0.5f, 0.5f, 0.2f), new Color(1f, 0.8392157f, 0.08627451f), new Color(0.9686275f, 0.5764706f, 0f), new Color(1f, 0.2941177f, 0.227451f), new Color(1f, 0.3803922f, 0.6117647f), new Color(0.6588235f, 0.4470588f, 0.7176471f), new Color(0.05098039f, 0.6117647f, 0.8235294f), new Color(0f, 0.7098039f, 0.7254902f), new Color(0.4470588f, 0.6627451f, 0.09411765f) };

        public static void AddColorItemsToGenericMenu(GenericMenu menu, AudioMixerGroupController[] groups)
        {
            Color[] colors = GetColors();
            string[] colorNames = GetColorNames();
            for (int i = 0; i < colors.Length; i++)
            {
                bool on = (groups.Length == 1) && (i == groups[0].userColorIndex);
                ItemData userData = new ItemData {
                    groups = groups,
                    index = i
                };
                menu.AddItem(new GUIContent(colorNames[i]), on, new GenericMenu.MenuFunction2(AudioMixerColorCodes.ItemCallback), userData);
            }
        }

        public static Color GetColor(int index)
        {
            Color[] colors = GetColors();
            if ((index >= 0) && (index < colors.Length))
            {
                return colors[index];
            }
            Debug.LogError("Invalid color code index: " + index);
            return Color.white;
        }

        private static string[] GetColorNames()
        {
            return colorNames;
        }

        private static Color[] GetColors()
        {
            if (EditorGUIUtility.isProSkin)
            {
                return darkSkinColors;
            }
            return lightSkinColors;
        }

        private static void ItemCallback(object data)
        {
            ItemData data2 = (ItemData) data;
            Undo.RecordObjects(data2.groups, "Change Group(s) Color");
            foreach (AudioMixerGroupController controller in data2.groups)
            {
                controller.userColorIndex = data2.index;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ItemData
        {
            public AudioMixerGroupController[] groups;
            public int index;
        }
    }
}

