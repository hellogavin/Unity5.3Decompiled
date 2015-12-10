namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class Handles
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map6;
        internal static float backfaceAlphaMultiplier = 0.2f;
        private const float k_BoneThickness = 0.08f;
        private const int kMaxDottedLineVertices = 0x3e8;
        private static Color lineTransparency = new Color(1f, 1f, 1f, 0.75f);
        internal static Color s_BoundingBoxHandleColor = ((Color) (new Color(255f, 255f, 255f, 150f) / 255f));
        internal static int s_ButtonHash = "ButtonHash".GetHashCode();
        internal static PrefColor s_CenterColor = new PrefColor("Scene/Center Axis", 0.8f, 0.8f, 0.8f, 0.93f);
        internal static Color s_ColliderHandleColor = ((Color) (new Color(145f, 244f, 139f, 210f) / 255f));
        internal static Color s_ColliderHandleColorDisabled = ((Color) (new Color(84f, 200f, 77f, 140f) / 255f));
        private static Color s_Color;
        internal static Mesh s_ConeMesh;
        internal static Mesh s_CubeMesh;
        internal static Mesh s_CylinderMesh;
        internal static int s_DiscHash = "DiscHash".GetHashCode();
        internal static int s_FreeMoveHandleHash = "FreeMoveHandleHash".GetHashCode();
        private static bool s_FreeMoveMode = false;
        internal static int s_FreeRotateHandleHash = "FreeRotateHandleHash".GetHashCode();
        internal static Matrix4x4 s_InverseMatrix = Matrix4x4.identity;
        private static bool s_Lighting = true;
        internal static Matrix4x4 s_Matrix = Matrix4x4.identity;
        private static Vector3 s_PlanarHandlesOctant = Vector3.one;
        internal static Mesh s_QuadMesh;
        internal static int s_RadiusHandleHash = "RadiusHandleHash".GetHashCode();
        private static Vector3[] s_RectangleCapPointsCache = new Vector3[5];
        internal static int s_ScaleSliderHash = "ScaleSliderHash".GetHashCode();
        internal static int s_ScaleValueHandleHash = "ScaleValueHandleHash".GetHashCode();
        internal static PrefColor s_SecondaryColor = new PrefColor("Scene/Guide Line", 0.5f, 0.5f, 0.5f, 0.2f);
        internal static PrefColor s_SelectedColor = new PrefColor("Scene/Selected Axis", 0.9647059f, 0.9490196f, 0.1960784f, 0.89f);
        internal static int s_Slider2DHash = "Slider2DHash".GetHashCode();
        internal static int s_SliderHash = "SliderHash".GetHashCode();
        internal static Mesh s_SphereMesh;
        internal static PrefColor s_XAxisColor = new PrefColor("Scene/X Axis", 0.8588235f, 0.2431373f, 0.1137255f, 0.93f);
        internal static int s_xAxisMoveHandleHash = "xAxisFreeMoveHandleHash".GetHashCode();
        internal static int s_xyAxisMoveHandleHash = "xyAxisFreeMoveHandleHash".GetHashCode();
        internal static int s_xzAxisMoveHandleHash = "xzAxisFreeMoveHandleHash".GetHashCode();
        internal static PrefColor s_YAxisColor = new PrefColor("Scene/Y Axis", 0.6039216f, 0.9529412f, 0.282353f, 0.93f);
        internal static int s_yAxisMoveHandleHash = "yAxisFreeMoveHandleHash".GetHashCode();
        internal static int s_yzAxisMoveHandleHash = "yzAxisFreeMoveHandleHash".GetHashCode();
        internal static PrefColor s_ZAxisColor = new PrefColor("Scene/Z Axis", 0.227451f, 0.4784314f, 0.972549f, 0.93f);
        internal static int s_zAxisMoveHandleHash = "xAxisFreeMoveHandleHash".GetHashCode();
        internal static float staticBlend = 0.6f;
        internal static Color staticColor = new Color(0.5f, 0.5f, 0.5f, 0f);
        private static Vector3[] verts = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

        public static void ArrowCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3 forward = (Vector3) (rotation * Vector3.forward);
                ConeCap(controlID, position + ((Vector3) (forward * size)), Quaternion.LookRotation(forward), size * 0.2f);
                DrawLine(position, position + ((Vector3) ((forward * size) * 0.9f)));
            }
        }

        public static void BeginGUI()
        {
            if ((Camera.current != null) && (Event.current.type == EventType.Repaint))
            {
                GUIClip.Reapply();
            }
        }

        [Obsolete("Please use BeginGUI() with GUILayout.BeginArea(position) / GUILayout.EndArea()")]
        public static void BeginGUI(Rect position)
        {
            GUILayout.BeginArea(position);
        }

        private static bool BeginLineDrawing(Matrix4x4 matrix, bool dottedLines)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return false;
            }
            Color c = s_Color * lineTransparency;
            if (dottedLines)
            {
                HandleUtility.ApplyDottedWireMaterial();
            }
            else
            {
                HandleUtility.ApplyWireMaterial();
            }
            GL.PushMatrix();
            GL.MultMatrix(matrix);
            GL.Begin(1);
            GL.Color(c);
            return true;
        }

        public static bool Button(Vector3 position, Quaternion direction, float size, float pickSize, DrawCapFunction capFunc)
        {
            return UnityEditorInternal.Button.Do(GUIUtility.GetControlID(s_ButtonHash, FocusType.Passive), position, direction, size, pickSize, capFunc);
        }

        internal static bool Button(int controlID, Vector3 position, Quaternion direction, float size, float pickSize, DrawCapFunction capFunc)
        {
            return UnityEditorInternal.Button.Do(controlID, position, direction, size, pickSize, capFunc);
        }

        public static void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                StartCapDraw(position, rotation, size);
                Vector3 normal = (Vector3) (rotation * new Vector3(0f, 0f, 1f));
                DrawWireDisc(position, normal, size);
            }
        }

        public static void ClearCamera(Rect position, Camera camera)
        {
            Event current = Event.current;
            if (camera.targetTexture == null)
            {
                Rect rect = EditorGUIUtility.PointsToPixels(GUIClip.Unclip(position));
                Rect rect2 = new Rect(rect.xMin, Screen.height - rect.yMax, rect.width, rect.height);
                camera.pixelRect = rect2;
            }
            else
            {
                camera.rect = new Rect(0f, 0f, 1f, 1f);
            }
            if (current.type == EventType.Repaint)
            {
                Internal_ClearCamera(camera);
            }
            else
            {
                Internal_SetCurrentCamera(camera);
            }
        }

        public static void ConeCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Graphics.DrawMeshNow(s_ConeMesh, StartCapDraw(position, rotation, size));
            }
        }

        internal static Vector3 ConeFrustrumHandle(Quaternion rotation, Vector3 position, Vector3 radiusAngleRange)
        {
            return DoConeFrustrumHandle(rotation, position, radiusAngleRange);
        }

        internal static Vector2 ConeHandle(Quaternion rotation, Vector3 position, Vector2 angleAndRange, float angleScale, float rangeScale, bool handlesOnly)
        {
            return DoConeHandle(rotation, position, angleAndRange, angleScale, rangeScale, handlesOnly);
        }

        public static void CubeCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Graphics.DrawMeshNow(s_CubeMesh, StartCapDraw(position, rotation, size));
            }
        }

        public static void CylinderCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Graphics.DrawMeshNow(s_CylinderMesh, StartCapDraw(position, rotation, size));
            }
        }

        public static Quaternion Disc(Quaternion rotation, Vector3 position, Vector3 axis, float size, bool cutoffPlane, float snap)
        {
            return UnityEditorInternal.Disc.Do(GUIUtility.GetControlID(s_DiscHash, FocusType.Keyboard), rotation, position, axis, size, cutoffPlane, snap);
        }

        internal static float DistanceToPolygone(Vector3[] vertices)
        {
            return HandleUtility.DistanceToPolyLine(vertices);
        }

        internal static void DoBoneHandle(Transform target)
        {
            DoBoneHandle(target, null);
        }

        internal static void DoBoneHandle(Transform target, Dictionary<Transform, bool> validBones)
        {
            Vector3 vector;
            int hashCode = target.name.GetHashCode();
            Event current = Event.current;
            bool flag = false;
            if (validBones != null)
            {
                IEnumerator enumerator = target.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform key = (Transform) enumerator.Current;
                        if (validBones.ContainsKey(key))
                        {
                            flag = true;
                            goto Label_006E;
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        Label_006E:
            vector = target.position;
            List<Vector3> list = new List<Vector3>();
            if (!flag && (target.parent != null))
            {
                list.Add(target.position + ((Vector3) ((target.position - target.parent.position) * 0.4f)));
            }
            else
            {
                IEnumerator enumerator2 = target.GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        Transform transform2 = (Transform) enumerator2.Current;
                        if ((validBones == null) || validBones.ContainsKey(transform2))
                        {
                            list.Add(transform2.position);
                        }
                    }
                }
                finally
                {
                    IDisposable disposable2 = enumerator2 as IDisposable;
                    if (disposable2 == null)
                    {
                    }
                    disposable2.Dispose();
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                float num6;
                Vector3 endPoint = list[i];
                switch (current.GetTypeForControl(hashCode))
                {
                    case EventType.MouseDown:
                    {
                        if (!current.alt && (((HandleUtility.nearestControl == hashCode) && (current.button == 0)) || ((GUIUtility.keyboardControl == hashCode) && (current.button == 2))))
                        {
                            int num7 = hashCode;
                            GUIUtility.keyboardControl = num7;
                            GUIUtility.hotControl = num7;
                            if (current.shift)
                            {
                                Object[] objects = Selection.objects;
                                if (!ArrayUtility.Contains<Object>(objects, target))
                                {
                                    ArrayUtility.Add<Object>(ref objects, target);
                                    Selection.objects = objects;
                                }
                            }
                            else
                            {
                                Selection.activeObject = target;
                            }
                            EditorGUIUtility.PingObject(target);
                            current.Use();
                        }
                        continue;
                    }
                    case EventType.MouseUp:
                    {
                        if ((GUIUtility.hotControl == hashCode) && ((current.button == 0) || (current.button == 2)))
                        {
                            GUIUtility.hotControl = 0;
                            current.Use();
                        }
                        continue;
                    }
                    case EventType.MouseMove:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                    case EventType.ScrollWheel:
                    {
                        continue;
                    }
                    case EventType.MouseDrag:
                    {
                        if (!current.alt && (GUIUtility.hotControl == hashCode))
                        {
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.objectReferences = new Object[] { target };
                            DragAndDrop.StartDrag(ObjectNames.GetDragAndDropTitle(target));
                            current.Use();
                        }
                        continue;
                    }
                    case EventType.Repaint:
                    {
                        float num5 = Vector3.Magnitude(endPoint - vector);
                        if (num5 > 0f)
                        {
                            num6 = num5 * 0.08f;
                            if (!flag)
                            {
                                break;
                            }
                            DrawBone(endPoint, vector, num6);
                        }
                        continue;
                    }
                    case EventType.Layout:
                    {
                        float radius = Vector3.Magnitude(endPoint - vector) * 0.08f;
                        Vector3[] vertices = GetBoneVertices(endPoint, vector, radius);
                        HandleUtility.AddControl(hashCode, DistanceToPolygone(vertices));
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                SphereCap(hashCode, vector, target.rotation, num6 * 5f);
            }
        }

        internal static Vector3 DoConeFrustrumHandle(Quaternion rotation, Vector3 position, Vector3 radiusAngleRange)
        {
            Vector3 d = (Vector3) (rotation * Vector3.forward);
            Vector3 vector2 = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            float x = radiusAngleRange.x;
            float y = radiusAngleRange.y;
            float z = radiusAngleRange.z;
            y = Mathf.Max(0f, y);
            bool changed = GUI.changed;
            z = SizeSlider(position, d, z);
            GUI.changed |= changed;
            changed = GUI.changed;
            GUI.changed = false;
            x = SizeSlider(position, vector2, x);
            x = SizeSlider(position, -vector2, x);
            x = SizeSlider(position, vector3, x);
            x = SizeSlider(position, -vector3, x);
            if (GUI.changed)
            {
                x = Mathf.Max(0f, x);
            }
            GUI.changed |= changed;
            changed = GUI.changed;
            GUI.changed = false;
            float r = Mathf.Min((float) 1000f, (float) (Mathf.Abs((float) (z * Mathf.Tan(0.01745329f * y))) + x));
            r = SizeSlider(position + ((Vector3) (d * z)), vector2, r);
            r = SizeSlider(position + ((Vector3) (d * z)), -vector2, r);
            r = SizeSlider(position + ((Vector3) (d * z)), vector3, r);
            r = SizeSlider(position + ((Vector3) (d * z)), -vector3, r);
            if (GUI.changed)
            {
                y = Mathf.Clamp((float) (57.29578f * Mathf.Atan((r - x) / Mathf.Abs(z))), (float) 0f, (float) 90f);
            }
            GUI.changed |= changed;
            if (x > 0f)
            {
                DrawWireDisc(position, d, x);
            }
            if (r > 0f)
            {
                DrawWireDisc(position + ((Vector3) (z * d)), d, r);
            }
            DrawLine(position + ((Vector3) (vector2 * x)), (Vector3) ((position + (d * z)) + (vector2 * r)));
            DrawLine(position - ((Vector3) (vector2 * x)), (Vector3) ((position + (d * z)) - (vector2 * r)));
            DrawLine(position + ((Vector3) (vector3 * x)), (Vector3) ((position + (d * z)) + (vector3 * r)));
            DrawLine(position - ((Vector3) (vector3 * x)), (Vector3) ((position + (d * z)) - (vector3 * r)));
            return new Vector3(x, y, z);
        }

        internal static Vector2 DoConeHandle(Quaternion rotation, Vector3 position, Vector2 angleAndRange, float angleScale, float rangeScale, bool handlesOnly)
        {
            float x = angleAndRange.x;
            float y = angleAndRange.y;
            float r = y * rangeScale;
            Vector3 d = (Vector3) (rotation * Vector3.forward);
            Vector3 vector2 = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            bool changed = GUI.changed;
            GUI.changed = false;
            r = SizeSlider(position, d, r);
            if (GUI.changed)
            {
                y = Mathf.Max((float) 0f, (float) (r / rangeScale));
            }
            GUI.changed |= changed;
            changed = GUI.changed;
            GUI.changed = false;
            float num4 = (r * Mathf.Tan((0.01745329f * x) / 2f)) * angleScale;
            num4 = SizeSlider(position + ((Vector3) (d * r)), vector2, num4);
            num4 = SizeSlider(position + ((Vector3) (d * r)), -vector2, num4);
            num4 = SizeSlider(position + ((Vector3) (d * r)), vector3, num4);
            num4 = SizeSlider(position + ((Vector3) (d * r)), -vector3, num4);
            if (GUI.changed)
            {
                x = Mathf.Clamp((float) ((57.29578f * Mathf.Atan(num4 / (r * angleScale))) * 2f), (float) 0f, (float) 179f);
            }
            GUI.changed |= changed;
            if (!handlesOnly)
            {
                DrawLine(position, (Vector3) ((position + (d * r)) + (vector2 * num4)));
                DrawLine(position, (Vector3) ((position + (d * r)) - (vector2 * num4)));
                DrawLine(position, (Vector3) ((position + (d * r)) + (vector3 * num4)));
                DrawLine(position, (Vector3) ((position + (d * r)) - (vector3 * num4)));
                DrawWireDisc(position + ((Vector3) (r * d)), d, num4);
            }
            return new Vector2(x, y);
        }

        private static void DoDrawAAConvexPolygon(Vector3[] points, int actualNumberOfPoints, float alpha)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                Color defaultColor = new Color(1f, 1f, 1f, alpha) * s_Color;
                Internal_DrawAAConvexPolygon(points, defaultColor, actualNumberOfPoints, matrix);
            }
        }

        private static void DoDrawAAPolyLine(Color[] colors, Vector3[] points, int actualNumberOfPoints, Texture2D lineTex, float width, float alpha)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                Color defaultColor = new Color(1f, 1f, 1f, alpha);
                if (colors != null)
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] *= defaultColor;
                    }
                }
                else
                {
                    defaultColor *= s_Color;
                }
                Internal_DrawAAPolyLine(colors, points, defaultColor, actualNumberOfPoints, lineTex, width, matrix);
            }
        }

        private static Vector3 DoPlanarHandle(PlaneHandle planeID, Vector3 position, Quaternion rotation, float handleSize)
        {
            Vector3 normalized;
            int num = 0;
            int num2 = 0;
            int hint = 0;
            bool flag = (!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects);
            switch (planeID)
            {
                case PlaneHandle.xzPlane:
                    num = 0;
                    num2 = 2;
                    Handles.color = !flag ? yAxisColor : staticColor;
                    hint = s_xzAxisMoveHandleHash;
                    break;

                case PlaneHandle.xyPlane:
                    num = 0;
                    num2 = 1;
                    Handles.color = !flag ? zAxisColor : staticColor;
                    hint = s_xyAxisMoveHandleHash;
                    break;

                case PlaneHandle.yzPlane:
                    num = 1;
                    num2 = 2;
                    Handles.color = !flag ? xAxisColor : staticColor;
                    hint = s_yzAxisMoveHandleHash;
                    break;
            }
            int num4 = (3 - num2) - num;
            Color color = Handles.color;
            Matrix4x4 matrixx = Matrix4x4.TRS(position, rotation, Vector3.one);
            if (Camera.current.orthographic)
            {
                normalized = matrixx.inverse.MultiplyVector((Vector3) (SceneView.currentDrawingSceneView.cameraTargetRotation * -Vector3.forward)).normalized;
            }
            else
            {
                normalized = matrixx.inverse.MultiplyPoint(SceneView.currentDrawingSceneView.camera.transform.position).normalized;
            }
            int controlID = GUIUtility.GetControlID(hint, FocusType.Keyboard);
            if ((Mathf.Abs(normalized[num4]) < 0.05f) && (GUIUtility.hotControl != controlID))
            {
                Handles.color = color;
                return position;
            }
            if (!currentlyDragging)
            {
                s_PlanarHandlesOctant[num] = (normalized[num] >= -0.01f) ? ((float) 1) : ((float) (-1));
                s_PlanarHandlesOctant[num2] = (normalized[num2] >= -0.01f) ? ((float) 1) : ((float) (-1));
            }
            Vector3 offset = s_PlanarHandlesOctant;
            offset[num4] = 0f;
            offset = (Vector3) (rotation * ((offset * handleSize) * 0.5f));
            Vector3 zero = Vector3.zero;
            Vector3 vector4 = Vector3.zero;
            Vector3 handleDir = Vector3.zero;
            zero[num] = 1f;
            vector4[num2] = 1f;
            handleDir[num4] = 1f;
            zero = (Vector3) (rotation * zero);
            vector4 = (Vector3) (rotation * vector4);
            handleDir = (Vector3) (rotation * handleDir);
            verts[0] = (position + offset) + ((Vector3) (((zero + vector4) * handleSize) * 0.5f));
            verts[1] = (position + offset) + ((Vector3) (((-zero + vector4) * handleSize) * 0.5f));
            verts[2] = (position + offset) + ((Vector3) (((-zero - vector4) * handleSize) * 0.5f));
            verts[3] = (position + offset) + ((Vector3) (((zero - vector4) * handleSize) * 0.5f));
            DrawSolidRectangleWithOutline(verts, new Color(Handles.color.r, Handles.color.g, Handles.color.b, 0.1f), new Color(0f, 0f, 0f, 0f));
            position = Slider2D(controlID, position, offset, handleDir, zero, vector4, handleSize * 0.5f, new DrawCapFunction(Handles.RectangleCap), new Vector2(SnapSettings.move[num], SnapSettings.move[num2]));
            Handles.color = color;
            return position;
        }

        public static Vector3 DoPositionHandle(Vector3 position, Quaternion rotation)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.KeyDown:
                    if ((current.keyCode == KeyCode.V) && !currentlyDragging)
                    {
                        s_FreeMoveMode = true;
                    }
                    break;

                case EventType.KeyUp:
                    position = DoPositionHandle_Internal(position, rotation);
                    if (((current.keyCode == KeyCode.V) && !current.shift) && !currentlyDragging)
                    {
                        s_FreeMoveMode = false;
                    }
                    return position;

                case EventType.Layout:
                    if (!currentlyDragging && !Tools.vertexDragging)
                    {
                        s_FreeMoveMode = current.shift;
                    }
                    break;
            }
            return DoPositionHandle_Internal(position, rotation);
        }

        private static Vector3 DoPositionHandle_Internal(Vector3 position, Quaternion rotation)
        {
            float handleSize = HandleUtility.GetHandleSize(position);
            Color color = Handles.color;
            bool flag = (!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects);
            Handles.color = !flag ? xAxisColor : Color.Lerp(xAxisColor, staticColor, staticBlend);
            GUI.SetNextControlName("xAxis");
            position = Slider(position, (Vector3) (rotation * Vector3.right), handleSize, new DrawCapFunction(Handles.ArrowCap), SnapSettings.move.x);
            Handles.color = !flag ? yAxisColor : Color.Lerp(yAxisColor, staticColor, staticBlend);
            GUI.SetNextControlName("yAxis");
            position = Slider(position, (Vector3) (rotation * Vector3.up), handleSize, new DrawCapFunction(Handles.ArrowCap), SnapSettings.move.y);
            Handles.color = !flag ? zAxisColor : Color.Lerp(zAxisColor, staticColor, staticBlend);
            GUI.SetNextControlName("zAxis");
            position = Slider(position, (Vector3) (rotation * Vector3.forward), handleSize, new DrawCapFunction(Handles.ArrowCap), SnapSettings.move.z);
            if (s_FreeMoveMode)
            {
                Handles.color = centerColor;
                GUI.SetNextControlName("FreeMoveAxis");
                position = FreeMoveHandle(position, rotation, handleSize * 0.15f, SnapSettings.move, new DrawCapFunction(Handles.RectangleCap));
            }
            else
            {
                position = DoPlanarHandle(PlaneHandle.xzPlane, position, rotation, handleSize * 0.25f);
                position = DoPlanarHandle(PlaneHandle.xyPlane, position, rotation, handleSize * 0.25f);
                position = DoPlanarHandle(PlaneHandle.yzPlane, position, rotation, handleSize * 0.25f);
            }
            Handles.color = color;
            return position;
        }

        internal static float DoRadiusHandle(Quaternion rotation, Vector3 position, float radius, bool handlesOnly)
        {
            Vector3 forward;
            float num = 90f;
            Vector3[] vectorArray = new Vector3[] { rotation * Vector3.right, rotation * Vector3.up, rotation * Vector3.forward, rotation * -Vector3.right, rotation * -Vector3.up, rotation * -Vector3.forward };
            if (Camera.current.orthographic)
            {
                forward = Camera.current.transform.forward;
                if (!handlesOnly)
                {
                    DrawWireDisc(position, forward, radius);
                    for (int j = 0; j < 3; j++)
                    {
                        Vector3 normalized = Vector3.Cross(vectorArray[j], forward).normalized;
                        DrawTwoShadedWireDisc(position, vectorArray[j], normalized, 180f, radius);
                    }
                }
            }
            else
            {
                forward = position - Camera.current.transform.position;
                float sqrMagnitude = forward.sqrMagnitude;
                float num4 = radius * radius;
                float f = (num4 * num4) / sqrMagnitude;
                float num6 = f / num4;
                if (num6 < 1f)
                {
                    float y = Mathf.Sqrt(num4 - f);
                    num = Mathf.Atan2(y, Mathf.Sqrt(f)) * 57.29578f;
                    if (!handlesOnly)
                    {
                        DrawWireDisc(position - ((Vector3) ((num4 * forward) / sqrMagnitude)), forward, y);
                    }
                }
                else
                {
                    num = -1000f;
                }
                if (!handlesOnly)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (num6 < 1f)
                        {
                            float a = Vector3.Angle(forward, vectorArray[k]);
                            a = 90f - Mathf.Min(a, 180f - a);
                            float num10 = Mathf.Tan(a * 0.01745329f);
                            float num11 = Mathf.Sqrt(f + ((num10 * num10) * f)) / radius;
                            if (num11 < 1f)
                            {
                                float angle = Mathf.Asin(num11) * 57.29578f;
                                Vector3 from = Vector3.Cross(vectorArray[k], forward).normalized;
                                from = (Vector3) (Quaternion.AngleAxis(angle, vectorArray[k]) * from);
                                DrawTwoShadedWireDisc(position, vectorArray[k], from, (90f - angle) * 2f, radius);
                            }
                            else
                            {
                                DrawTwoShadedWireDisc(position, vectorArray[k], radius);
                            }
                        }
                        else
                        {
                            DrawTwoShadedWireDisc(position, vectorArray[k], radius);
                        }
                    }
                }
            }
            Color color = Handles.color;
            for (int i = 0; i < 6; i++)
            {
                int controlID = GUIUtility.GetControlID(s_RadiusHandleHash, FocusType.Keyboard);
                float num15 = Vector3.Angle(vectorArray[i], -forward);
                if (((num15 > 5f) && (num15 < 175f)) || (GUIUtility.hotControl == controlID))
                {
                    Color color2 = color;
                    if (num15 > (num + 5f))
                    {
                        color2.a = Mathf.Clamp01((backfaceAlphaMultiplier * color.a) * 2f);
                    }
                    else
                    {
                        color2.a = Mathf.Clamp01(color.a * 2f);
                    }
                    Handles.color = color2;
                    Vector3 vector4 = position + ((Vector3) (radius * vectorArray[i]));
                    bool changed = GUI.changed;
                    GUI.changed = false;
                    vector4 = Slider1D.Do(controlID, vector4, vectorArray[i], HandleUtility.GetHandleSize(vector4) * 0.03f, new DrawCapFunction(Handles.DotCap), 0f);
                    if (GUI.changed)
                    {
                        radius = Vector3.Distance(vector4, position);
                    }
                    GUI.changed |= changed;
                }
            }
            Handles.color = color;
            return radius;
        }

        internal static Vector2 DoRectHandles(Quaternion rotation, Vector3 position, Vector2 size)
        {
            Vector3 vector = (Vector3) (rotation * Vector3.forward);
            Vector3 d = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            float r = 0.5f * size.x;
            float num2 = 0.5f * size.y;
            Vector3 vector4 = (Vector3) ((position + (d * num2)) + (vector3 * r));
            Vector3 vector5 = (Vector3) ((position - (d * num2)) + (vector3 * r));
            Vector3 vector6 = (Vector3) ((position - (d * num2)) - (vector3 * r));
            Vector3 vector7 = (Vector3) ((position + (d * num2)) - (vector3 * r));
            DrawLine(vector4, vector5);
            DrawLine(vector5, vector6);
            DrawLine(vector6, vector7);
            DrawLine(vector7, vector4);
            Color color = Handles.color;
            color.a = Mathf.Clamp01(color.a * 2f);
            Handles.color = color;
            num2 = SizeSlider(position, d, num2);
            num2 = SizeSlider(position, -d, num2);
            r = SizeSlider(position, vector3, r);
            r = SizeSlider(position, -vector3, r);
            if (((Tools.current != Tool.Move) && (Tools.current != Tool.Scale)) || (Tools.pivotRotation != PivotRotation.Local))
            {
                DrawLine(position, position + vector);
            }
            size.x = 2f * r;
            size.y = 2f * num2;
            return size;
        }

        public static Quaternion DoRotationHandle(Quaternion rotation, Vector3 position)
        {
            float handleSize = HandleUtility.GetHandleSize(position);
            Color color = Handles.color;
            bool flag = (!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects);
            Handles.color = !flag ? xAxisColor : Color.Lerp(xAxisColor, staticColor, staticBlend);
            rotation = Disc(rotation, position, (Vector3) (rotation * Vector3.right), handleSize, true, SnapSettings.rotation);
            Handles.color = !flag ? yAxisColor : Color.Lerp(yAxisColor, staticColor, staticBlend);
            rotation = Disc(rotation, position, (Vector3) (rotation * Vector3.up), handleSize, true, SnapSettings.rotation);
            Handles.color = !flag ? zAxisColor : Color.Lerp(zAxisColor, staticColor, staticBlend);
            rotation = Disc(rotation, position, (Vector3) (rotation * Vector3.forward), handleSize, true, SnapSettings.rotation);
            if (!flag)
            {
                Handles.color = centerColor;
                rotation = Disc(rotation, position, Camera.current.transform.forward, handleSize * 1.1f, false, 0f);
                rotation = FreeRotateHandle(rotation, position, handleSize);
            }
            Handles.color = color;
            return rotation;
        }

        public static Vector3 DoScaleHandle(Vector3 scale, Vector3 position, Quaternion rotation, float size)
        {
            bool flag = (!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects);
            color = !flag ? xAxisColor : Color.Lerp(xAxisColor, staticColor, staticBlend);
            scale.x = ScaleSlider(scale.x, position, (Vector3) (rotation * Vector3.right), rotation, size, SnapSettings.scale);
            color = !flag ? yAxisColor : Color.Lerp(yAxisColor, staticColor, staticBlend);
            scale.y = ScaleSlider(scale.y, position, (Vector3) (rotation * Vector3.up), rotation, size, SnapSettings.scale);
            color = !flag ? zAxisColor : Color.Lerp(zAxisColor, staticColor, staticBlend);
            scale.z = ScaleSlider(scale.z, position, (Vector3) (rotation * Vector3.forward), rotation, size, SnapSettings.scale);
            color = centerColor;
            EditorGUI.BeginChangeCheck();
            float num = ScaleValueHandle(scale.x, position, rotation, size, new DrawCapFunction(Handles.CubeCap), SnapSettings.scale);
            if (EditorGUI.EndChangeCheck())
            {
                float num2 = num / scale.x;
                scale.x = num;
                scale.y *= num2;
                scale.z *= num2;
            }
            return scale;
        }

        internal static float DoSimpleEdgeHandle(Quaternion rotation, Vector3 position, float radius)
        {
            Vector3 d = (Vector3) (rotation * Vector3.right);
            EditorGUI.BeginChangeCheck();
            radius = SizeSlider(position, d, radius);
            radius = SizeSlider(position, -d, radius);
            if (EditorGUI.EndChangeCheck())
            {
                radius = Mathf.Max(0f, radius);
            }
            if (radius > 0f)
            {
                DrawLine(position - ((Vector3) (d * radius)), position + ((Vector3) (d * radius)));
            }
            return radius;
        }

        internal static void DoSimpleRadiusArcHandleXY(Quaternion rotation, Vector3 position, ref float radius, ref float arc)
        {
            Vector3 normal = (Vector3) (rotation * Vector3.forward);
            Vector3 d = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            Vector3 vector4 = (Vector3) (Quaternion.Euler(0f, 0f, arc) * vector3);
            EditorGUI.BeginChangeCheck();
            if (arc < 315f)
            {
                radius = SizeSlider(position, vector3, radius);
            }
            if (arc > 135f)
            {
                radius = SizeSlider(position, d, radius);
            }
            if (arc > 225f)
            {
                radius = SizeSlider(position, -vector3, radius);
            }
            if (arc > 315f)
            {
                radius = SizeSlider(position, -d, radius);
            }
            if (EditorGUI.EndChangeCheck())
            {
                radius = Mathf.Max(0f, radius);
            }
            if (radius > 0f)
            {
                DrawWireArc(position, normal, vector3, arc, radius);
                if (arc < 360f)
                {
                    DrawLine(position, vector3 * radius);
                    DrawLine(position, vector4 * radius);
                }
                else
                {
                    DrawDottedLine(position, vector3 * radius, 5f);
                }
                Vector3 vector5 = vector4 * radius;
                float handleSize = HandleUtility.GetHandleSize(vector5);
                EditorGUI.BeginChangeCheck();
                Vector3 rhs = FreeMoveHandle(vector5, Quaternion.identity, handleSize * 0.03f, SnapSettings.move, new DrawCapFunction(Handles.CircleCap));
                if (EditorGUI.EndChangeCheck())
                {
                    arc += Mathf.Atan2(Vector3.Dot(normal, Vector3.Cross(vector5, rhs)), Vector3.Dot(vector5, rhs)) * 57.29578f;
                }
            }
        }

        internal static float DoSimpleRadiusHandle(Quaternion rotation, Vector3 position, float radius, bool hemisphere)
        {
            Vector3 d = (Vector3) (rotation * Vector3.forward);
            Vector3 vector2 = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            bool changed = GUI.changed;
            GUI.changed = false;
            radius = SizeSlider(position, d, radius);
            if (!hemisphere)
            {
                radius = SizeSlider(position, -d, radius);
            }
            if (GUI.changed)
            {
                radius = Mathf.Max(0f, radius);
            }
            GUI.changed |= changed;
            changed = GUI.changed;
            GUI.changed = false;
            radius = SizeSlider(position, vector2, radius);
            radius = SizeSlider(position, -vector2, radius);
            radius = SizeSlider(position, vector3, radius);
            radius = SizeSlider(position, -vector3, radius);
            if (GUI.changed)
            {
                radius = Mathf.Max(0f, radius);
            }
            GUI.changed |= changed;
            if (radius > 0f)
            {
                DrawWireDisc(position, d, radius);
                DrawWireArc(position, vector2, -vector3, !hemisphere ? ((float) 360) : ((float) 180), radius);
                DrawWireArc(position, vector3, vector2, !hemisphere ? ((float) 360) : ((float) 180), radius);
            }
            return radius;
        }

        public static void DotCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                position = matrix.MultiplyPoint(position);
                Vector3 vector = (Vector3) (Camera.current.transform.right * size);
                Vector3 vector2 = (Vector3) (Camera.current.transform.up * size);
                Color c = s_Color * new Color(1f, 1f, 1f, 0.99f);
                HandleUtility.ApplyWireMaterial();
                GL.Begin(7);
                GL.Color(c);
                GL.Vertex((position + vector) + vector2);
                GL.Vertex((position + vector) - vector2);
                GL.Vertex((position - vector) - vector2);
                GL.Vertex((position - vector) + vector2);
                GL.End();
            }
        }

        public static void DrawAAConvexPolygon(params Vector3[] points)
        {
            DoDrawAAConvexPolygon(points, -1, 1f);
        }

        public static void DrawAAPolyLine(params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, -1, null, 2f, 0.75f);
        }

        internal static void DrawAAPolyLine(Color[] colors, Vector3[] points)
        {
            DoDrawAAPolyLine(colors, points, -1, null, 2f, 0.75f);
        }

        public static void DrawAAPolyLine(float width, params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, -1, null, width, 0.75f);
        }

        public static void DrawAAPolyLine(Texture2D lineTex, params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, -1, lineTex, (float) (lineTex.height / 2), 0.99f);
        }

        internal static void DrawAAPolyLine(float width, Color[] colors, Vector3[] points)
        {
            DoDrawAAPolyLine(colors, points, -1, null, width, 0.75f);
        }

        public static void DrawAAPolyLine(float width, int actualNumberOfPoints, params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, actualNumberOfPoints, null, width, 0.75f);
        }

        public static void DrawAAPolyLine(Texture2D lineTex, float width, params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, -1, lineTex, width, 0.99f);
        }

        [Obsolete("DrawArrow has been renamed to ArrowCap.")]
        public static void DrawArrow(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            ArrowCap(controlID, position, rotation, size);
        }

        public static void DrawBezier(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, Color color, Texture2D texture, float width)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                Internal_DrawBezier(startPosition, endPosition, startTangent, endTangent, color, texture, width, matrix);
            }
        }

        internal static void DrawBone(Vector3 endPoint, Vector3 basePoint, float size)
        {
            Vector3[] vectorArray = GetBoneVertices(endPoint, basePoint, size);
            HandleUtility.ApplyWireMaterial();
            GL.Begin(4);
            GL.Color(s_Color);
            for (int i = 0; i < 3; i++)
            {
                GL.Vertex(vectorArray[i * 6]);
                GL.Vertex(vectorArray[(i * 6) + 1]);
                GL.Vertex(vectorArray[(i * 6) + 2]);
                GL.Vertex(vectorArray[(i * 6) + 3]);
                GL.Vertex(vectorArray[(i * 6) + 4]);
                GL.Vertex(vectorArray[(i * 6) + 5]);
            }
            GL.End();
            GL.Begin(1);
            GL.Color((s_Color * new Color(1f, 1f, 1f, 0f)) + new Color(0f, 0f, 0f, 1f));
            for (int j = 0; j < 3; j++)
            {
                GL.Vertex(vectorArray[j * 6]);
                GL.Vertex(vectorArray[(j * 6) + 1]);
                GL.Vertex(vectorArray[(j * 6) + 1]);
                GL.Vertex(vectorArray[(j * 6) + 2]);
            }
            GL.End();
        }

        [ExcludeFromDocs]
        public static void DrawCamera(Rect position, Camera camera)
        {
            DrawCameraMode normal = DrawCameraMode.Normal;
            DrawCamera(position, camera, normal);
        }

        public static void DrawCamera(Rect position, Camera camera, [DefaultValue("DrawCameraMode.Normal")] DrawCameraMode drawMode)
        {
            DrawGridParameters gridParam = new DrawGridParameters();
            DrawCameraImpl(position, camera, drawMode, false, gridParam, true);
        }

        internal static void DrawCamera(Rect position, Camera camera, DrawCameraMode drawMode, DrawGridParameters gridParam)
        {
            DrawCameraImpl(position, camera, drawMode, true, gridParam, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void DrawCameraFade(Camera camera, float fade);
        internal static void DrawCameraImpl(Rect position, Camera camera, DrawCameraMode drawMode, bool drawGrid, DrawGridParameters gridParam, bool finish)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (camera.targetTexture == null)
                {
                    Rect rect = EditorGUIUtility.PointsToPixels(GUIClip.Unclip(position));
                    camera.pixelRect = new Rect(rect.xMin, Screen.height - rect.yMax, rect.width, rect.height);
                }
                else
                {
                    camera.rect = new Rect(0f, 0f, 1f, 1f);
                }
                if (drawMode == DrawCameraMode.Normal)
                {
                    RenderTexture targetTexture = camera.targetTexture;
                    camera.targetTexture = RenderTexture.active;
                    camera.Render();
                    camera.targetTexture = targetTexture;
                }
                else
                {
                    if (drawGrid)
                    {
                        Internal_DrawCameraWithGrid(camera, (int) drawMode, ref gridParam);
                    }
                    else
                    {
                        Internal_DrawCamera(camera, (int) drawMode);
                    }
                    if (finish)
                    {
                        Internal_FinishDrawingCamera(camera);
                    }
                }
            }
            else
            {
                Internal_SetCurrentCamera(camera);
            }
        }

        internal static void DrawCameraStep1(Rect position, Camera camera, DrawCameraMode drawMode, DrawGridParameters gridParam)
        {
            DrawCameraImpl(position, camera, drawMode, true, gridParam, false);
        }

        internal static void DrawCameraStep2(Camera camera, DrawCameraMode drawMode)
        {
            if ((Event.current.type == EventType.Repaint) && (drawMode != DrawCameraMode.Normal))
            {
                Internal_FinishDrawingCamera(camera);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool DrawCameraTonemap(Camera camera, RenderTexture srcRT, RenderTexture dstRT);
        [Obsolete("DrawCone has been renamed to ConeCap.")]
        public static void DrawCone(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            ConeCap(controlID, position, rotation, size);
        }

        [Obsolete("DrawCube has been renamed to CubeCap.")]
        public static void DrawCube(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            CubeCap(controlID, position, rotation, size);
        }

        [Obsolete("DrawCylinder has been renamed to CylinderCap.")]
        public static void DrawCylinder(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            CylinderCap(controlID, position, rotation, size);
        }

        public static void DrawDottedLine(Vector3 p1, Vector3 p2, float screenSpaceSize)
        {
            if (BeginLineDrawing(matrix, true))
            {
                float x = screenSpaceSize * EditorGUIUtility.pixelsPerPoint;
                GL.MultiTexCoord(1, p1);
                GL.MultiTexCoord2(2, x, 0f);
                GL.Vertex(p1);
                GL.MultiTexCoord(1, p1);
                GL.MultiTexCoord2(2, x, 0f);
                GL.Vertex(p2);
                EndLineDrawing();
            }
        }

        public static void DrawDottedLines(Vector3[] lineSegments, float screenSpaceSize)
        {
            if (BeginLineDrawing(matrix, true))
            {
                float x = screenSpaceSize * EditorGUIUtility.pixelsPerPoint;
                for (int i = 0; i < lineSegments.Length; i += 2)
                {
                    Vector3 v = lineSegments[i];
                    Vector3 vector2 = lineSegments[i + 1];
                    GL.MultiTexCoord(1, v);
                    GL.MultiTexCoord2(2, x, 0f);
                    GL.Vertex(v);
                    GL.MultiTexCoord(1, v);
                    GL.MultiTexCoord2(2, x, 0f);
                    GL.Vertex(vector2);
                }
                EndLineDrawing();
            }
        }

        public static void DrawDottedLines(Vector3[] points, int[] segmentIndices, float screenSpaceSize)
        {
            if (BeginLineDrawing(matrix, true))
            {
                float x = screenSpaceSize * EditorGUIUtility.pixelsPerPoint;
                for (int i = 0; i < segmentIndices.Length; i += 2)
                {
                    Vector3 v = points[segmentIndices[i]];
                    Vector3 vector2 = points[segmentIndices[i + 1]];
                    GL.MultiTexCoord(1, v);
                    GL.MultiTexCoord2(2, x, 0f);
                    GL.Vertex(v);
                    GL.MultiTexCoord(1, v);
                    GL.MultiTexCoord2(2, x, 0f);
                    GL.Vertex(vector2);
                }
                EndLineDrawing();
            }
        }

        public static void DrawLine(Vector3 p1, Vector3 p2)
        {
            if (BeginLineDrawing(matrix, false))
            {
                GL.Vertex(p1);
                GL.Vertex(p2);
                EndLineDrawing();
            }
        }

        public static void DrawLines(Vector3[] lineSegments)
        {
            if (BeginLineDrawing(matrix, false))
            {
                for (int i = 0; i < lineSegments.Length; i += 2)
                {
                    Vector3 v = lineSegments[i];
                    Vector3 vector2 = lineSegments[i + 1];
                    GL.Vertex(v);
                    GL.Vertex(vector2);
                }
                EndLineDrawing();
            }
        }

        public static void DrawLines(Vector3[] points, int[] segmentIndices)
        {
            if (BeginLineDrawing(matrix, false))
            {
                for (int i = 0; i < segmentIndices.Length; i += 2)
                {
                    Vector3 v = points[segmentIndices[i]];
                    Vector3 vector2 = points[segmentIndices[i + 1]];
                    GL.Vertex(v);
                    GL.Vertex(vector2);
                }
                EndLineDrawing();
            }
        }

        public static void DrawPolyLine(params Vector3[] points)
        {
            if (BeginLineDrawing(matrix, false))
            {
                for (int i = 1; i < points.Length; i++)
                {
                    GL.Vertex(points[i]);
                    GL.Vertex(points[i - 1]);
                }
                EndLineDrawing();
            }
        }

        [Obsolete("DrawRectangle has been renamed to RectangleCap.")]
        public static void DrawRectangle(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            RectangleCap(controlID, position, rotation, size);
        }

        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3[] dest = new Vector3[60];
                SetDiscSectionPoints(dest, 60, center, normal, from, angle, radius);
                Shader.SetGlobalColor("_HandleColor", color * new Color(1f, 1f, 1f, 0.5f));
                Shader.SetGlobalFloat("_HandleSize", 1f);
                HandleUtility.ApplyWireMaterial();
                GL.PushMatrix();
                GL.MultMatrix(matrix);
                GL.Begin(4);
                for (int i = 1; i < dest.Length; i++)
                {
                    GL.Color(color);
                    GL.Vertex(center);
                    GL.Vertex(dest[i - 1]);
                    GL.Vertex(dest[i]);
                    GL.Vertex(center);
                    GL.Vertex(dest[i]);
                    GL.Vertex(dest[i - 1]);
                }
                GL.End();
                GL.PopMatrix();
            }
        }

        public static void DrawSolidDisc(Vector3 center, Vector3 normal, float radius)
        {
            Vector3 from = Vector3.Cross(normal, Vector3.up);
            if (from.sqrMagnitude < 0.001f)
            {
                from = Vector3.Cross(normal, Vector3.right);
            }
            DrawSolidArc(center, normal, from, 360f, radius);
        }

        public static void DrawSolidRectangleWithOutline(Rect rectangle, Color faceColor, Color outlineColor)
        {
            Vector3[] verts = new Vector3[] { new Vector3(rectangle.xMin, rectangle.yMin, 0f), new Vector3(rectangle.xMax, rectangle.yMin, 0f), new Vector3(rectangle.xMax, rectangle.yMax, 0f), new Vector3(rectangle.xMin, rectangle.yMax, 0f) };
            DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);
        }

        public static void DrawSolidRectangleWithOutline(Vector3[] verts, Color faceColor, Color outlineColor)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                GL.PushMatrix();
                GL.MultMatrix(matrix);
                if (faceColor.a > 0f)
                {
                    Color c = faceColor * color;
                    GL.Begin(4);
                    for (int i = 0; i < 2; i++)
                    {
                        GL.Color(c);
                        GL.Vertex(verts[i * 2]);
                        GL.Vertex(verts[(i * 2) + 1]);
                        GL.Vertex(verts[((i * 2) + 2) % 4]);
                        GL.Vertex(verts[i * 2]);
                        GL.Vertex(verts[((i * 2) + 2) % 4]);
                        GL.Vertex(verts[(i * 2) + 1]);
                    }
                    GL.End();
                }
                if (outlineColor.a > 0f)
                {
                    Color color2 = outlineColor * color;
                    GL.Begin(1);
                    GL.Color(color2);
                    for (int j = 0; j < 4; j++)
                    {
                        GL.Vertex(verts[j]);
                        GL.Vertex(verts[(j + 1) % 4]);
                    }
                    GL.End();
                }
                GL.PopMatrix();
            }
        }

        [Obsolete("DrawSphere has been renamed to SphereCap.")]
        public static void DrawSphere(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            SphereCap(controlID, position, rotation, size);
        }

        internal static void DrawTwoShadedWireDisc(Vector3 position, Vector3 axis, float radius)
        {
            Color color = Handles.color;
            Color color2 = color;
            color.a *= backfaceAlphaMultiplier;
            Handles.color = color;
            DrawWireDisc(position, axis, radius);
            Handles.color = color2;
        }

        internal static void DrawTwoShadedWireDisc(Vector3 position, Vector3 axis, Vector3 from, float degrees, float radius)
        {
            DrawWireArc(position, axis, from, degrees, radius);
            Color color = Handles.color;
            Color color2 = color;
            color.a *= backfaceAlphaMultiplier;
            Handles.color = color;
            DrawWireArc(position, axis, from, degrees - 360f, radius);
            Handles.color = color2;
        }

        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            Vector3[] dest = new Vector3[60];
            SetDiscSectionPoints(dest, 60, center, normal, from, angle, radius);
            DrawPolyLine(dest);
        }

        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius)
        {
            Vector3 from = Vector3.Cross(normal, Vector3.up);
            if (from.sqrMagnitude < 0.001f)
            {
                from = Vector3.Cross(normal, Vector3.right);
            }
            DrawWireArc(center, normal, from, 360f, radius);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EmitGUIGeometryForCamera(Camera source, Camera dest);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EnableCameraFlares(Camera cam, bool flares);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EnableCameraFx(Camera cam, bool fx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EnableCameraSkybox(Camera cam, bool skybox);
        public static void EndGUI()
        {
            Camera current = Camera.current;
            if ((current != null) && (Event.current.type == EventType.Repaint))
            {
                Internal_SetupCamera(current);
            }
        }

        private static void EndLineDrawing()
        {
            GL.End();
            GL.PopMatrix();
        }

        public static Vector3 FreeMoveHandle(Vector3 position, Quaternion rotation, float size, Vector3 snap, DrawCapFunction capFunc)
        {
            return FreeMove.Do(GUIUtility.GetControlID(s_FreeMoveHandleHash, FocusType.Keyboard), position, rotation, size, snap, capFunc);
        }

        public static Quaternion FreeRotateHandle(Quaternion rotation, Vector3 position, float size)
        {
            return FreeRotate.Do(GUIUtility.GetControlID(s_FreeRotateHandleHash, FocusType.Keyboard), rotation, position, size);
        }

        internal static Vector3[] GetBoneVertices(Vector3 endPoint, Vector3 basePoint, float radius)
        {
            Vector3 lhs = Vector3.Normalize(endPoint - basePoint);
            Vector3 a = Vector3.Cross(lhs, Vector3.up);
            if (Vector3.SqrMagnitude(a) < 0.1f)
            {
                a = Vector3.Cross(lhs, Vector3.right);
            }
            a.Normalize();
            Vector3 vector3 = Vector3.Cross(lhs, a);
            Vector3[] vectorArray = new Vector3[0x12];
            float f = 0f;
            for (int i = 0; i < 3; i++)
            {
                float num5 = Mathf.Cos(f);
                float num6 = Mathf.Sin(f);
                float num7 = Mathf.Cos(f + 2.094395f);
                float num8 = Mathf.Sin(f + 2.094395f);
                Vector3 vector4 = (Vector3) ((basePoint + (a * (num5 * radius))) + (vector3 * (num6 * radius)));
                Vector3 vector5 = (Vector3) ((basePoint + (a * (num7 * radius))) + (vector3 * (num8 * radius)));
                vectorArray[i * 6] = endPoint;
                vectorArray[(i * 6) + 1] = vector4;
                vectorArray[(i * 6) + 2] = vector5;
                vectorArray[(i * 6) + 3] = basePoint;
                vectorArray[(i * 6) + 4] = vector5;
                vectorArray[(i * 6) + 5] = vector4;
                f += 2.094395f;
            }
            return vectorArray;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern FilterMode GetCameraFilterMode(Camera camera);
        internal static Rect GetCameraRect(Rect position)
        {
            Rect rect = GUIClip.Unclip(position);
            return new Rect(rect.xMin, Screen.height - rect.yMax, rect.width, rect.height);
        }

        public static Vector2 GetMainGameViewSize()
        {
            return GameView.GetSizeOfMainGameView();
        }

        internal static void Init()
        {
            if (s_CubeMesh == null)
            {
                GameObject obj2 = (GameObject) EditorGUIUtility.Load("SceneView/HandlesGO.fbx");
                if (obj2 == null)
                {
                    Debug.Log("ARGH - We couldn't find SceneView/HandlesGO.fbx");
                }
                obj2.SetActive(false);
                IEnumerator enumerator = obj2.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        MeshFilter component = current.GetComponent<MeshFilter>();
                        string name = current.name;
                        if (name != null)
                        {
                            int num;
                            if (<>f__switch$map6 == null)
                            {
                                Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
                                dictionary.Add("Cube", 0);
                                dictionary.Add("Sphere", 1);
                                dictionary.Add("Cone", 2);
                                dictionary.Add("Cylinder", 3);
                                dictionary.Add("Quad", 4);
                                <>f__switch$map6 = dictionary;
                            }
                            if (<>f__switch$map6.TryGetValue(name, out num))
                            {
                                switch (num)
                                {
                                    case 0:
                                        s_CubeMesh = component.sharedMesh;
                                        break;

                                    case 1:
                                        s_SphereMesh = component.sharedMesh;
                                        break;

                                    case 2:
                                        s_ConeMesh = component.sharedMesh;
                                        break;

                                    case 3:
                                        s_CylinderMesh = component.sharedMesh;
                                        break;

                                    case 4:
                                        s_QuadMesh = component.sharedMesh;
                                        break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande.ttf"));
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Bold.ttf"));
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Small.ttf"));
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Small Bold.ttf"));
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Big.ttf"));
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_DrawAAConvexPolygon(Vector3[] points, ref Color defaultColor, int actualNumberOfPoints, ref Matrix4x4 toWorld);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_DrawAAPolyLine(Color[] colors, Vector3[] points, ref Color defaultColor, int actualNumberOfPoints, Texture2D texture, float width, ref Matrix4x4 toWorld);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_DrawBezier(ref Vector3 startPosition, ref Vector3 endPosition, ref Vector3 startTangent, ref Vector3 endTangent, ref Color color, Texture2D texture, float width, ref Matrix4x4 toWorld);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Vector3[] INTERNAL_CALL_Internal_MakeBezierPoints(ref Vector3 startPosition, ref Vector3 endPosition, ref Vector3 startTangent, ref Vector3 endTangent, int division);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetSceneViewColors(ref Color wire, ref Color wireOverlay, ref Color active, ref Color selected);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_ClearCamera(Camera cam);
        private static void Internal_DrawAAConvexPolygon(Vector3[] points, Color defaultColor, int actualNumberOfPoints, Matrix4x4 toWorld)
        {
            INTERNAL_CALL_Internal_DrawAAConvexPolygon(points, ref defaultColor, actualNumberOfPoints, ref toWorld);
        }

        private static void Internal_DrawAAPolyLine(Color[] colors, Vector3[] points, Color defaultColor, int actualNumberOfPoints, Texture2D texture, float width, Matrix4x4 toWorld)
        {
            INTERNAL_CALL_Internal_DrawAAPolyLine(colors, points, ref defaultColor, actualNumberOfPoints, texture, width, ref toWorld);
        }

        private static void Internal_DrawBezier(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, Color color, Texture2D texture, float width, Matrix4x4 toWorld)
        {
            INTERNAL_CALL_Internal_DrawBezier(ref startPosition, ref endPosition, ref startTangent, ref endTangent, ref color, texture, width, ref toWorld);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_DrawCamera(Camera cam, int renderMode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_DrawCameraWithGrid(Camera cam, int renderMode, ref DrawGridParameters gridParam);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_FinishDrawingCamera(Camera cam);
        private static Vector3[] Internal_MakeBezierPoints(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int division)
        {
            return INTERNAL_CALL_Internal_MakeBezierPoints(ref startPosition, ref endPosition, ref startTangent, ref endTangent, division);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void Internal_SetCurrentCamera(Camera cam);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetupCamera(Camera cam);
        public static void Label(Vector3 position, string text)
        {
            Label(position, EditorGUIUtility.TempContent(text), GUI.skin.label);
        }

        public static void Label(Vector3 position, GUIContent content)
        {
            Label(position, content, GUI.skin.label);
        }

        public static void Label(Vector3 position, Texture image)
        {
            Label(position, EditorGUIUtility.TempContent(image), GUI.skin.label);
        }

        public static void Label(Vector3 position, string text, GUIStyle style)
        {
            Label(position, EditorGUIUtility.TempContent(text), style);
        }

        public static void Label(Vector3 position, GUIContent content, GUIStyle style)
        {
            BeginGUI();
            GUI.Label(HandleUtility.WorldPointToSizedRect(position, content, style), content, style);
            EndGUI();
        }

        public static Vector3[] MakeBezierPoints(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int division)
        {
            return Internal_MakeBezierPoints(startPosition, endPosition, startTangent, endTangent, division);
        }

        public static Vector3 PositionHandle(Vector3 position, Quaternion rotation)
        {
            return DoPositionHandle(position, rotation);
        }

        public static float RadiusHandle(Quaternion rotation, Vector3 position, float radius)
        {
            return DoRadiusHandle(rotation, position, radius, false);
        }

        public static float RadiusHandle(Quaternion rotation, Vector3 position, float radius, bool handlesOnly)
        {
            return DoRadiusHandle(rotation, position, radius, handlesOnly);
        }

        public static void RectangleCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            RectangleCap(controlID, position, rotation, new Vector2(size, size));
        }

        internal static void RectangleCap(int controlID, Vector3 position, Quaternion rotation, Vector2 size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3 vector = (Vector3) (rotation * new Vector3(size.x, 0f, 0f));
                Vector3 vector2 = (Vector3) (rotation * new Vector3(0f, size.y, 0f));
                s_RectangleCapPointsCache[0] = (position + vector) + vector2;
                s_RectangleCapPointsCache[1] = (position + vector) - vector2;
                s_RectangleCapPointsCache[2] = (position - vector) - vector2;
                s_RectangleCapPointsCache[3] = (position - vector) + vector2;
                s_RectangleCapPointsCache[4] = (position + vector) + vector2;
                DrawPolyLine(s_RectangleCapPointsCache);
            }
        }

        private static void ReplaceFontForWindows(Font font)
        {
            if (font.name.Contains("Bold"))
            {
                font.fontNames = new string[] { "Verdana Bold", "Tahoma Bold" };
            }
            else
            {
                font.fontNames = new string[] { "Verdana", "Tahoma" };
            }
            font.hideFlags = HideFlags.HideAndDontSave;
        }

        public static Quaternion RotationHandle(Quaternion rotation, Vector3 position)
        {
            return DoRotationHandle(rotation, position);
        }

        public static Vector3 ScaleHandle(Vector3 scale, Vector3 position, Quaternion rotation, float size)
        {
            return DoScaleHandle(scale, position, rotation, size);
        }

        public static float ScaleSlider(float scale, Vector3 position, Vector3 direction, Quaternion rotation, float size, float snap)
        {
            return SliderScale.DoAxis(GUIUtility.GetControlID(s_ScaleSliderHash, FocusType.Keyboard), scale, position, direction, rotation, size, snap);
        }

        public static float ScaleValueHandle(float value, Vector3 position, Quaternion rotation, float size, DrawCapFunction capFunc, float snap)
        {
            return SliderScale.DoCenter(GUIUtility.GetControlID(s_ScaleValueHandleHash, FocusType.Keyboard), value, position, rotation, size, capFunc, snap);
        }

        public static void SelectionFrame(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                StartCapDraw(position, rotation, size);
                Vector3 vector = (Vector3) (rotation * new Vector3(size, 0f, 0f));
                Vector3 vector2 = (Vector3) (rotation * new Vector3(0f, size, 0f));
                Vector3 vector3 = (position - vector) + vector2;
                Vector3 vector4 = (position + vector) + vector2;
                Vector3 vector5 = (position + vector) - vector2;
                Vector3 vector6 = (position - vector) - vector2;
                DrawLine(vector3, vector4);
                DrawLine(vector4, vector5);
                DrawLine(vector5, vector6);
                DrawLine(vector6, vector3);
            }
        }

        public static void SetCamera(Camera camera)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Internal_SetupCamera(camera);
            }
            else
            {
                Internal_SetCurrentCamera(camera);
            }
        }

        public static void SetCamera(Rect position, Camera camera)
        {
            Rect rect = EditorGUIUtility.PointsToPixels(GUIClip.Unclip(position));
            Rect rect2 = new Rect(rect.xMin, Screen.height - rect.yMax, rect.width, rect.height);
            camera.pixelRect = rect2;
            if (Event.current.type == EventType.Repaint)
            {
                Internal_SetupCamera(camera);
            }
            else
            {
                Internal_SetCurrentCamera(camera);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetCameraFilterMode(Camera camera, FilterMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetCameraOnlyDrawMesh(Camera cam);
        internal static void SetDiscSectionPoints(Vector3[] dest, int count, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            from.Normalize();
            Quaternion quaternion = Quaternion.AngleAxis(angle / ((float) (count - 1)), normal);
            Vector3 vector = (Vector3) (from * radius);
            for (int i = 0; i < count; i++)
            {
                dest[i] = center + vector;
                vector = (Vector3) (quaternion * vector);
            }
        }

        internal static void SetSceneViewColors(Color wire, Color wireOverlay, Color active, Color selected)
        {
            INTERNAL_CALL_SetSceneViewColors(ref wire, ref wireOverlay, ref active, ref selected);
        }

        internal static void SetupIgnoreRaySnapObjects()
        {
            HandleUtility.ignoreRaySnapObjects = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.Deep);
        }

        internal static void ShowStaticLabelIfNeeded(Vector3 pos)
        {
            if ((!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects))
            {
                color = Color.white;
                GUIStyle style = "SC ViewAxisLabel";
                style.alignment = TextAnchor.MiddleLeft;
                style.fixedWidth = 0f;
                BeginGUI();
                Rect position = HandleUtility.WorldPointToSizedRect(pos, EditorGUIUtility.TempContent("Static"), style);
                position.x += 10f;
                position.y += 10f;
                GUI.Label(position, EditorGUIUtility.TempContent("Static"), style);
                EndGUI();
            }
        }

        private static float SizeSlider(Vector3 p, Vector3 d, float r)
        {
            Vector3 position = p + ((Vector3) (d * r));
            float handleSize = HandleUtility.GetHandleSize(position);
            bool changed = GUI.changed;
            GUI.changed = false;
            position = Slider(position, d, handleSize * 0.03f, new DrawCapFunction(Handles.DotCap), 0f);
            if (GUI.changed)
            {
                r = Vector3.Dot(position - p, d);
            }
            GUI.changed |= changed;
            return r;
        }

        public static Vector3 Slider(Vector3 position, Vector3 direction)
        {
            return Slider(position, direction, HandleUtility.GetHandleSize(position), new DrawCapFunction(Handles.ArrowCap), -1f);
        }

        public static Vector3 Slider(Vector3 position, Vector3 direction, float size, DrawCapFunction drawFunc, float snap)
        {
            return Slider1D.Do(GUIUtility.GetControlID(s_SliderHash, FocusType.Keyboard), position, direction, size, drawFunc, snap);
        }

        [ExcludeFromDocs]
        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, float snap)
        {
            bool drawHelper = false;
            return Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        [ExcludeFromDocs]
        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap)
        {
            bool drawHelper = false;
            return Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        [ExcludeFromDocs]
        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap)
        {
            bool drawHelper = false;
            return Slider2D(id, handlePos, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, float snap, [DefaultValue("false")] bool drawHelper)
        {
            return Slider2D(GUIUtility.GetControlID(s_Slider2DHash, FocusType.Keyboard), handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);
        }

        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap, [DefaultValue("false")] bool drawHelper)
        {
            return UnityEditorInternal.Slider2D.Do(GUIUtility.GetControlID(s_Slider2DHash, FocusType.Keyboard), handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap, [DefaultValue("false")] bool drawHelper)
        {
            return UnityEditorInternal.Slider2D.Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        [ExcludeFromDocs]
        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap)
        {
            bool drawHelper = false;
            return Slider2D(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap, [DefaultValue("false")] bool drawHelper)
        {
            return UnityEditorInternal.Slider2D.Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        public static float SnapValue(float val, float snap)
        {
            if (EditorGUI.actionKey && (snap > 0f))
            {
                return (Mathf.Round(val / snap) * snap);
            }
            return val;
        }

        public static void SphereCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Graphics.DrawMeshNow(s_SphereMesh, StartCapDraw(position, rotation, size));
            }
        }

        internal static Matrix4x4 StartCapDraw(Vector3 position, Quaternion rotation, float size)
        {
            Shader.SetGlobalColor("_HandleColor", realHandleColor);
            Shader.SetGlobalFloat("_HandleSize", size);
            Matrix4x4 mat = matrix * Matrix4x4.TRS(position, rotation, Vector3.one);
            Shader.SetGlobalMatrix("_ObjectToWorld", mat);
            HandleUtility.handleMaterial.SetPass(0);
            return mat;
        }

        public static Color centerColor
        {
            get
            {
                return (Color) s_CenterColor;
            }
        }

        public static Color color
        {
            get
            {
                return s_Color;
            }
            set
            {
                s_Color = value;
            }
        }

        public Camera currentCamera
        {
            get
            {
                return Camera.current;
            }
            set
            {
                Internal_SetCurrentCamera(value);
            }
        }

        private static bool currentlyDragging
        {
            get
            {
                return (GUIUtility.hotControl != 0);
            }
        }

        public static Matrix4x4 inverseMatrix
        {
            get
            {
                return s_InverseMatrix;
            }
        }

        public static bool lighting
        {
            get
            {
                return s_Lighting;
            }
            set
            {
                s_Lighting = value;
            }
        }

        public static Matrix4x4 matrix
        {
            get
            {
                return s_Matrix;
            }
            set
            {
                s_Matrix = value;
                s_InverseMatrix = value.inverse;
            }
        }

        internal static Color realHandleColor
        {
            get
            {
                return ((s_Color * new Color(1f, 1f, 1f, 0.5f)) + (!s_Lighting ? new Color(0f, 0f, 0f, 0f) : new Color(0f, 0f, 0f, 0.5f)));
            }
        }

        public static Color secondaryColor
        {
            get
            {
                return (Color) s_SecondaryColor;
            }
        }

        public static Color selectedColor
        {
            get
            {
                return (Color) s_SelectedColor;
            }
        }

        public static Color xAxisColor
        {
            get
            {
                return (Color) s_XAxisColor;
            }
        }

        public static Color yAxisColor
        {
            get
            {
                return (Color) s_YAxisColor;
            }
        }

        public static Color zAxisColor
        {
            get
            {
                return (Color) s_ZAxisColor;
            }
        }

        public delegate void DrawCapFunction(int controlID, Vector3 position, Quaternion rotation, float size);

        internal enum FilterMode
        {
            Off,
            ShowFiltered,
            ShowRest
        }

        private enum PlaneHandle
        {
            xzPlane,
            xyPlane,
            yzPlane
        }
    }
}

