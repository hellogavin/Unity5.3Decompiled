namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SnapSettings : EditorWindow
    {
        private static Styles ms_Styles;
        private static bool s_Initialized;
        private static float s_MoveSnapX;
        private static float s_MoveSnapY;
        private static float s_MoveSnapZ;
        private static float s_RotationSnap;
        private static float s_ScaleSnap;

        private static void Initialize()
        {
            if (!s_Initialized)
            {
                s_MoveSnapX = EditorPrefs.GetFloat("MoveSnapX", 1f);
                s_MoveSnapY = EditorPrefs.GetFloat("MoveSnapY", 1f);
                s_MoveSnapZ = EditorPrefs.GetFloat("MoveSnapZ", 1f);
                s_ScaleSnap = EditorPrefs.GetFloat("ScaleSnap", 0.1f);
                s_RotationSnap = EditorPrefs.GetFloat("RotationSnap", 15f);
                s_Initialized = true;
            }
        }

        private void OnGUI()
        {
            if (ms_Styles == null)
            {
                ms_Styles = new Styles();
            }
            GUILayout.Space(5f);
            EditorGUI.BeginChangeCheck();
            Vector3 move = SnapSettings.move;
            move.x = EditorGUILayout.FloatField(ms_Styles.moveX, move.x, new GUILayoutOption[0]);
            move.y = EditorGUILayout.FloatField(ms_Styles.moveY, move.y, new GUILayoutOption[0]);
            move.z = EditorGUILayout.FloatField(ms_Styles.moveZ, move.z, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                if (move.x <= 0f)
                {
                    move.x = SnapSettings.move.x;
                }
                if (move.y <= 0f)
                {
                    move.y = SnapSettings.move.y;
                }
                if (move.z <= 0f)
                {
                    move.z = SnapSettings.move.z;
                }
                SnapSettings.move = move;
            }
            scale = EditorGUILayout.FloatField(ms_Styles.scale, scale, new GUILayoutOption[0]);
            rotation = EditorGUILayout.FloatField(ms_Styles.rotation, rotation, new GUILayoutOption[0]);
            GUILayout.Space(5f);
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button(ms_Styles.snapAllAxes, ms_Styles.buttonLeft, new GUILayoutOption[0]))
            {
                flag = true;
                flag2 = true;
                flag3 = true;
            }
            if (GUILayout.Button(ms_Styles.snapX, ms_Styles.buttonMid, new GUILayoutOption[0]))
            {
                flag = true;
            }
            if (GUILayout.Button(ms_Styles.snapY, ms_Styles.buttonMid, new GUILayoutOption[0]))
            {
                flag2 = true;
            }
            if (GUILayout.Button(ms_Styles.snapZ, ms_Styles.buttonRight, new GUILayoutOption[0]))
            {
                flag3 = true;
            }
            GUILayout.EndHorizontal();
            if ((flag | flag2) | flag3)
            {
                Vector3 vector2 = new Vector3(1f / SnapSettings.move.x, 1f / SnapSettings.move.y, 1f / SnapSettings.move.z);
                Undo.RecordObjects(Selection.transforms, "Snap " + ((Selection.transforms.Length != 1) ? " selection" : Selection.activeGameObject.name) + " to grid");
                foreach (Transform transform in Selection.transforms)
                {
                    Vector3 position = transform.position;
                    if (flag)
                    {
                        float introduced15 = Mathf.Round(position.x * vector2.x);
                        position.x = introduced15 / vector2.x;
                    }
                    if (flag2)
                    {
                        float introduced16 = Mathf.Round(position.y * vector2.y);
                        position.y = introduced16 / vector2.y;
                    }
                    if (flag3)
                    {
                        float introduced17 = Mathf.Round(position.z * vector2.z);
                        position.z = introduced17 / vector2.z;
                    }
                    transform.position = position;
                }
            }
        }

        [MenuItem("Edit/Snap Settings...")]
        private static void ShowSnapSettings()
        {
            EditorWindow.GetWindowWithRect<SnapSettings>(new Rect(100f, 100f, 230f, 130f), true, "Snap settings");
        }

        public static Vector3 move
        {
            get
            {
                Initialize();
                return new Vector3(s_MoveSnapX, s_MoveSnapY, s_MoveSnapZ);
            }
            set
            {
                EditorPrefs.SetFloat("MoveSnapX", value.x);
                s_MoveSnapX = value.x;
                EditorPrefs.SetFloat("MoveSnapY", value.y);
                s_MoveSnapY = value.y;
                EditorPrefs.SetFloat("MoveSnapZ", value.z);
                s_MoveSnapZ = value.z;
            }
        }

        public static float rotation
        {
            get
            {
                Initialize();
                return s_RotationSnap;
            }
            set
            {
                EditorPrefs.SetFloat("RotationSnap", value);
                s_RotationSnap = value;
            }
        }

        public static float scale
        {
            get
            {
                Initialize();
                return s_ScaleSnap;
            }
            set
            {
                EditorPrefs.SetFloat("ScaleSnap", value);
                s_ScaleSnap = value;
            }
        }

        private class Styles
        {
            public GUIStyle buttonLeft = "ButtonLeft";
            public GUIStyle buttonMid = "ButtonMid";
            public GUIStyle buttonRight = "ButtonRight";
            public GUIContent moveX = EditorGUIUtility.TextContent("Move X|Grid spacing X");
            public GUIContent moveY = EditorGUIUtility.TextContent("Move Y|Grid spacing Y");
            public GUIContent moveZ = EditorGUIUtility.TextContent("Move Z|Grid spacing Z");
            public GUIContent rotation = EditorGUIUtility.TextContent("Rotation|Grid spacing for rotation in degrees");
            public GUIContent scale = EditorGUIUtility.TextContent("Scale|Grid spacing for scaling");
            public GUIContent snapAllAxes = EditorGUIUtility.TextContent("Snap All Axes|Snaps selected objects to the grid");
            public GUIContent snapX = EditorGUIUtility.TextContent("X|Snaps selected objects to the grid on the x axis");
            public GUIContent snapY = EditorGUIUtility.TextContent("Y|Snaps selected objects to the grid on the y axis");
            public GUIContent snapZ = EditorGUIUtility.TextContent("Z|Snaps selected objects to the grid on the z axis");
        }
    }
}

