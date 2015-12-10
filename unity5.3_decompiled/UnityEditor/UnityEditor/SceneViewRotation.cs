namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    internal class SceneViewRotation
    {
        private int currentDir = 7;
        private AnimBool[] dirNameVisible = new AnimBool[] { new AnimBool(), new AnimBool(), new AnimBool(), new AnimBool(), new AnimBool(), new AnimBool(), new AnimBool(), new AnimBool(), new AnimBool() };
        private AnimBool[] dirVisible = new AnimBool[] { new AnimBool(true), new AnimBool(true), new AnimBool(true) };
        private static Quaternion[] kDirectionRotations = new Quaternion[] { Quaternion.LookRotation(new Vector3(-1f, 0f, 0f)), Quaternion.LookRotation(new Vector3(0f, -1f, 0f)), Quaternion.LookRotation(new Vector3(0f, 0f, -1f)), Quaternion.LookRotation(new Vector3(1f, 0f, 0f)), Quaternion.LookRotation(new Vector3(0f, 1f, 0f)), Quaternion.LookRotation(new Vector3(0f, 0f, 1f)) };
        private static string[] kDirNames = new string[] { "Right", "Top", "Front", "Left", "Bottom", "Back", "Iso", "Persp", "2D" };
        private static string[] kMenuDirNames = new string[] { "Free", "Right", "Top", "Front", "Left", "Bottom", "Back", string.Empty, "Perspective" };
        private const int kRotationMenuInset = 0x16;
        private const int kRotationSize = 100;
        private int m_CenterButtonControlID;
        private int[] m_ViewDirectionControlIDs;
        private AnimBool m_Visible = new AnimBool();
        private static Styles s_Styles;

        private void AxisSelectors(SceneView view, Camera cam, float size, float sgn, GUIStyle viewAxisLabelStyle)
        {
            for (int i = kDirectionRotations.Length - 1; i >= 0; i--)
            {
                Quaternion direction = kDirectionRotations[i];
                string[] strArray = new string[] { "x", "y", "z" };
                float faded = this.dirVisible[i % 3].faded;
                Vector3 rhs = (Vector3) (kDirectionRotations[i] * Vector3.forward);
                float num3 = Vector3.Dot(view.camera.transform.forward, rhs);
                if (((num3 > 0.0) || (sgn <= 0f)) && ((num3 <= 0.0) || (sgn >= 0f)))
                {
                    Color xAxisColor;
                    switch (i)
                    {
                        case 0:
                            xAxisColor = Handles.xAxisColor;
                            break;

                        case 1:
                            xAxisColor = Handles.yAxisColor;
                            break;

                        case 2:
                            xAxisColor = Handles.zAxisColor;
                            break;

                        default:
                            xAxisColor = Handles.centerColor;
                            break;
                    }
                    if (view.in2DMode)
                    {
                        xAxisColor = Color.Lerp(xAxisColor, Color.gray, this.faded2Dgray);
                    }
                    xAxisColor.a *= faded * this.m_Visible.faded;
                    Handles.color = xAxisColor;
                    if (xAxisColor.a <= 0.1f)
                    {
                        GUI.enabled = false;
                    }
                    if (((sgn > 0f) && Handles.Button(this.m_ViewDirectionControlIDs[i], (Vector3) (((direction * Vector3.forward) * size) * -1.2f), direction, size, size * 0.7f, new Handles.DrawCapFunction(Handles.ConeCap))) && !view.in2DMode)
                    {
                        this.ViewAxisDirection(view, i);
                    }
                    if (i < 3)
                    {
                        GUI.color = new Color(1f, 1f, 1f, this.dirVisible[i].faded * this.m_Visible.faded);
                        Vector3 vector2 = rhs;
                        vector2 += (Vector3) ((num3 * view.camera.transform.forward) * -0.5f);
                        vector2 = (Vector3) (((vector2 * 0.7f) + (vector2.normalized * 1.5f)) * size);
                        Handles.Label(-vector2, new GUIContent(strArray[i]), styles.viewAxisLabelStyle);
                    }
                    if (((sgn < 0f) && Handles.Button(this.m_ViewDirectionControlIDs[i], (Vector3) (((direction * Vector3.forward) * size) * -1.2f), direction, size, size * 0.7f, new Handles.DrawCapFunction(Handles.ConeCap))) && !view.in2DMode)
                    {
                        this.ViewAxisDirection(view, i);
                    }
                    Handles.color = Color.white;
                    GUI.color = Color.white;
                    GUI.enabled = true;
                }
            }
        }

        private void ContextMenuDelegate(object userData, string[] options, int selected)
        {
            SceneView view = userData as SceneView;
            if (view != null)
            {
                if (selected == 0)
                {
                    this.ViewFromNiceAngle(view, false);
                }
                else if ((selected >= 1) && (selected <= 6))
                {
                    int dir = selected - 1;
                    this.ViewAxisDirection(view, dir);
                }
                else if (selected == 8)
                {
                    this.ViewSetOrtho(view, !view.orthographic);
                }
                else if (selected == 10)
                {
                    view.LookAt(view.pivot, Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f)), view.size, view.orthographic);
                }
                else if (selected == 11)
                {
                    view.LookAt(view.pivot, Quaternion.LookRotation(new Vector3(1f, -0.7f, -1f)), view.size, view.orthographic);
                }
                else if (selected == 12)
                {
                    view.LookAt(view.pivot, Quaternion.LookRotation(new Vector3(1f, -0.7f, 1f)), view.size, view.orthographic);
                }
            }
        }

        private void DisplayContextMenu(Rect buttonOrCursorRect, SceneView view)
        {
            int[] selected = new int[!view.orthographic ? 2 : 1];
            selected[0] = (this.currentDir < 6) ? (this.currentDir + 1) : 0;
            if (!view.orthographic)
            {
                selected[1] = 8;
            }
            EditorUtility.DisplayCustomMenu(buttonOrCursorRect, kMenuDirNames, selected, new EditorUtility.SelectMenuItemFunction(this.ContextMenuDelegate), view);
            GUIUtility.ExitGUI();
        }

        private void DrawIsoStatusSymbol(Vector3 center, SceneView view, float alpha)
        {
            float num = 1f - Mathf.Clamp01((view.m_Ortho.faded * 1.2f) - 0.1f);
            Vector3 vector = (Vector3) (Vector3.up * 3f);
            Vector3 vector2 = (Vector3) (Vector3.right * 10f);
            Vector3 vector3 = center - ((Vector3) (vector2 * 0.5f));
            Handles.color = new Color(1f, 1f, 1f, 0.6f * alpha);
            Vector3[] points = new Vector3[] { vector3 + (vector * (1f - num)), (vector3 + vector2) + (vector * (1f + (num * 0.5f))) };
            Handles.DrawAAPolyLine(points);
            Vector3[] vectorArray2 = new Vector3[] { vector3, vector3 + vector2 };
            Handles.DrawAAPolyLine(vectorArray2);
            Vector3[] vectorArray3 = new Vector3[] { vector3 - ((Vector3) (vector * (1f - num))), (vector3 + vector2) - ((Vector3) (vector * (1f + (num * 0.5f)))) };
            Handles.DrawAAPolyLine(vectorArray3);
        }

        private void DrawLabels(SceneView view)
        {
            Rect position = new Rect((view.position.width - 100f) + 17f, 92f, 66f, 16f);
            if (!view.in2DMode && GUI.Button(position, string.Empty, styles.viewLabelStyleLeftAligned))
            {
                if (Event.current.button == 1)
                {
                    this.DisplayContextMenu(position, view);
                }
                else
                {
                    this.ViewSetOrtho(view, !view.orthographic);
                }
            }
            if (Event.current.type == EventType.Repaint)
            {
                int index = 8;
                Rect rect2 = position;
                float num2 = 0f;
                float num3 = 0f;
                for (int i = 0; i < kDirNames.Length; i++)
                {
                    if (i != index)
                    {
                        num3 += this.dirNameVisible[i].faded;
                        if (this.dirNameVisible[i].faded > 0f)
                        {
                            num2 += styles.viewLabelStyleLeftAligned.CalcSize(EditorGUIUtility.TempContent(kDirNames[i])).x * this.dirNameVisible[i].faded;
                        }
                    }
                }
                if (num3 > 0f)
                {
                    num2 /= num3;
                }
                rect2.x += 37f - (num2 * 0.5f);
                rect2.x = Mathf.RoundToInt(rect2.x);
                for (int j = 0; (j < this.dirNameVisible.Length) && (j < kDirNames.Length); j++)
                {
                    if (j != index)
                    {
                        Color color = Handles.centerColor;
                        color.a *= this.dirNameVisible[j].faded;
                        if (color.a > 0f)
                        {
                            GUI.color = color;
                            GUI.Label(rect2, kDirNames[j], styles.viewLabelStyleLeftAligned);
                        }
                    }
                }
                Color centerColor = Handles.centerColor;
                centerColor.a *= this.faded2Dgray * this.m_Visible.faded;
                if (centerColor.a > 0f)
                {
                    GUI.color = centerColor;
                    GUI.Label(position, kDirNames[index], styles.viewLabelStyleCentered);
                }
                if (this.faded2Dgray < 1f)
                {
                    this.DrawIsoStatusSymbol(new Vector3(rect2.x - 8f, rect2.y + 8.5f, 0f), view, 1f - this.faded2Dgray);
                }
            }
        }

        internal int GetLabelIndexForView(SceneView view, Vector3 direction, bool ortho)
        {
            if (view.in2DMode)
            {
                return 8;
            }
            if (this.IsAxisAligned(direction))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (Vector3.Dot((Vector3) (kDirectionRotations[i] * Vector3.forward), direction) > 0.9f)
                    {
                        return i;
                    }
                }
            }
            return (!ortho ? 7 : 6);
        }

        internal void HandleContextClick(SceneView view)
        {
            if (!view.in2DMode)
            {
                Event current = Event.current;
                if (((current.type == EventType.MouseDown) && (current.button == 1)) && (Mathf.Min(view.position.width, view.position.height) >= 100f))
                {
                    Rect rect = new Rect((view.position.width - 100f) + 22f, 22f, 56f, 56f);
                    if (rect.Contains(current.mousePosition))
                    {
                        this.DisplayContextMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), view);
                        current.Use();
                    }
                }
            }
        }

        private bool IsAxisAligned(Vector3 v)
        {
            return (((Mathf.Abs((float) (v.x * v.y)) < 0.0001f) && (Mathf.Abs((float) (v.y * v.z)) < 0.0001f)) && (Mathf.Abs((float) (v.z * v.x)) < 0.0001f));
        }

        internal void OnGUI(SceneView view)
        {
            if (Mathf.Min(view.position.width, view.position.height) >= 100f)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    Profiler.BeginSample("SceneView.AxisSelector");
                }
                this.HandleContextClick(view);
                Camera camera = view.camera;
                HandleUtility.PushCamera(camera);
                if (camera.orthographic)
                {
                    camera.orthographicSize = 0.5f;
                }
                camera.cullingMask = 0;
                camera.transform.position = (Vector3) (camera.transform.rotation * new Vector3(0f, 0f, -5f));
                camera.clearFlags = CameraClearFlags.Nothing;
                camera.nearClipPlane = 0.1f;
                camera.farClipPlane = 10f;
                camera.fieldOfView = view.m_Ortho.Fade(70f, 0f);
                SceneView.AddCursorRect(new Rect((view.position.width - 100f) + 22f, 22f, 56f, 102f), MouseCursor.Arrow);
                Handles.SetCamera(new Rect(view.position.width - 100f, 0f, 100f, 100f), camera);
                Handles.BeginGUI();
                this.DrawLabels(view);
                Handles.EndGUI();
                for (int i = 0; i < 3; i++)
                {
                    Vector3 rhs = (Vector3) (kDirectionRotations[i] * Vector3.forward);
                    this.dirVisible[i].target = Mathf.Abs(Vector3.Dot(camera.transform.forward, rhs)) < 0.9f;
                }
                float size = HandleUtility.GetHandleSize(Vector3.zero) * 0.2f;
                this.AxisSelectors(view, camera, size, -1f, styles.viewAxisLabelStyle);
                Color color = Color.Lerp(Handles.centerColor, Color.gray, this.faded2Dgray);
                color.a *= this.m_Visible.faded;
                if (color.a <= 0.1f)
                {
                    GUI.enabled = false;
                }
                Handles.color = color;
                if (Handles.Button(this.m_CenterButtonControlID, Vector3.zero, Quaternion.identity, size * 0.8f, size, new Handles.DrawCapFunction(Handles.CubeCap)) && !view.in2DMode)
                {
                    if (Event.current.clickCount == 2)
                    {
                        view.FrameSelected();
                    }
                    else if (Event.current.shift || (Event.current.button == 2))
                    {
                        this.ViewFromNiceAngle(view, true);
                    }
                    else
                    {
                        this.ViewSetOrtho(view, !view.orthographic);
                    }
                }
                this.AxisSelectors(view, camera, size, 1f, styles.viewAxisLabelStyle);
                GUI.enabled = true;
                if (!view.in2DMode && (Event.current.type == EditorGUIUtility.swipeGestureEventType))
                {
                    Vector3 up;
                    Event current = Event.current;
                    if (current.delta.y > 0f)
                    {
                        up = Vector3.up;
                    }
                    else if (current.delta.y < 0f)
                    {
                        up = -Vector3.up;
                    }
                    else if (current.delta.x < 0f)
                    {
                        up = Vector3.right;
                    }
                    else
                    {
                        up = -Vector3.right;
                    }
                    Vector3 direction = -up - ((Vector3) (Vector3.forward * 0.9f));
                    direction = view.camera.transform.TransformDirection(direction);
                    float num4 = 0f;
                    int dir = 0;
                    for (int j = 0; j < 6; j++)
                    {
                        float num7 = Vector3.Dot((Vector3) (kDirectionRotations[j] * -Vector3.forward), direction);
                        if (num7 > num4)
                        {
                            num4 = num7;
                            dir = j;
                        }
                    }
                    this.ViewAxisDirection(view, dir);
                    Event.current.Use();
                }
                HandleUtility.PopCamera(camera);
                Handles.SetCamera(camera);
                if (Event.current.type == EventType.Repaint)
                {
                    Profiler.EndSample();
                }
            }
        }

        public void Register(SceneView view)
        {
            for (int i = 0; i < this.dirVisible.Length; i++)
            {
                this.dirVisible[i].valueChanged.AddListener(new UnityAction(view.Repaint));
            }
            for (int j = 0; j < this.dirNameVisible.Length; j++)
            {
                this.dirNameVisible[j].valueChanged.AddListener(new UnityAction(view.Repaint));
            }
            this.m_Visible.valueChanged.AddListener(new UnityAction(view.Repaint));
            int newVisible = this.GetLabelIndexForView(view, (Vector3) (view.rotation * Vector3.forward), view.orthographic);
            for (int k = 0; k < this.dirNameVisible.Length; k++)
            {
                this.dirNameVisible[k].value = k == newVisible;
            }
            this.m_Visible.value = newVisible != 8;
            this.SwitchDirNameVisible(newVisible);
            if (this.m_ViewDirectionControlIDs == null)
            {
                this.m_ViewDirectionControlIDs = new int[kDirectionRotations.Length];
                for (int m = 0; m < this.m_ViewDirectionControlIDs.Length; m++)
                {
                    this.m_ViewDirectionControlIDs[m] = GUIUtility.GetPermanentControlID();
                }
                this.m_CenterButtonControlID = GUIUtility.GetPermanentControlID();
            }
        }

        private void SwitchDirNameVisible(int newVisible)
        {
            if (newVisible != this.currentDir)
            {
                this.dirNameVisible[this.currentDir].target = false;
                this.currentDir = newVisible;
                this.dirNameVisible[this.currentDir].target = true;
                if (newVisible == 8)
                {
                    this.m_Visible.speed = 0.3f;
                }
                else
                {
                    this.m_Visible.speed = 2f;
                }
                this.m_Visible.target = newVisible != 8;
            }
        }

        internal void UpdateGizmoLabel(SceneView view, Vector3 direction, bool ortho)
        {
            this.SwitchDirNameVisible(this.GetLabelIndexForView(view, direction, ortho));
        }

        private void ViewAxisDirection(SceneView view, int dir)
        {
            bool orthographic = view.orthographic;
            if ((Event.current != null) && (Event.current.shift || (Event.current.button == 2)))
            {
                orthographic = true;
            }
            view.LookAt(view.pivot, kDirectionRotations[dir], view.size, orthographic);
            this.SwitchDirNameVisible(dir);
        }

        private void ViewFromNiceAngle(SceneView view, bool forcePerspective)
        {
            Vector3 forward = (Vector3) (view.rotation * Vector3.forward);
            forward.y = 0f;
            if (forward == Vector3.zero)
            {
                forward = Vector3.forward;
            }
            else
            {
                forward = forward.normalized;
            }
            forward.y = -0.5f;
            bool ortho = !forcePerspective ? view.orthographic : false;
            view.LookAt(view.pivot, Quaternion.LookRotation(forward), view.size, ortho);
            this.SwitchDirNameVisible(!ortho ? 7 : 6);
        }

        private void ViewSetOrtho(SceneView view, bool ortho)
        {
            view.LookAt(view.pivot, view.rotation, view.size, ortho);
        }

        private float faded2Dgray
        {
            get
            {
                return this.dirNameVisible[8].faded;
            }
        }

        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        private class Styles
        {
            public GUIStyle viewAxisLabelStyle;
            public GUIStyle viewLabelStyleCentered = new GUIStyle("SC ViewLabel");
            public GUIStyle viewLabelStyleLeftAligned = new GUIStyle("SC ViewLabel");

            public Styles()
            {
                this.viewLabelStyleLeftAligned.alignment = TextAnchor.MiddleLeft;
                this.viewLabelStyleCentered.alignment = TextAnchor.MiddleCenter;
                this.viewAxisLabelStyle = "SC ViewAxisLabel";
            }
        }
    }
}

