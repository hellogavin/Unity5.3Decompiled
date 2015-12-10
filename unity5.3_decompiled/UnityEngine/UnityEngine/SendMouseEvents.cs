namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    internal class SendMouseEvents
    {
        private static Camera[] m_Cameras;
        private static readonly HitInfo[] m_CurrentHit = new HitInfo[] { new HitInfo(), new HitInfo(), new HitInfo() };
        private const int m_HitIndexGUI = 0;
        private const int m_HitIndexPhysics2D = 2;
        private const int m_HitIndexPhysics3D = 1;
        private static readonly HitInfo[] m_LastHit = new HitInfo[] { new HitInfo(), new HitInfo(), new HitInfo() };
        private static readonly HitInfo[] m_MouseDownHit = new HitInfo[] { new HitInfo(), new HitInfo(), new HitInfo() };
        private static bool s_MouseUsed = false;

        [RequiredByNativeCode]
        private static void DoSendMouseEvents(int skipRTCameras)
        {
            Vector3 mousePosition = Input.mousePosition;
            int allCamerasCount = Camera.allCamerasCount;
            if ((m_Cameras == null) || (m_Cameras.Length != allCamerasCount))
            {
                m_Cameras = new Camera[allCamerasCount];
            }
            Camera.GetAllCameras(m_Cameras);
            for (int i = 0; i < m_CurrentHit.Length; i++)
            {
                m_CurrentHit[i] = new HitInfo();
            }
            if (!s_MouseUsed)
            {
                foreach (Camera camera in m_Cameras)
                {
                    if (((camera != null) && ((skipRTCameras == 0) || (camera.targetTexture == null))) && camera.pixelRect.Contains(mousePosition))
                    {
                        GUILayer component = camera.GetComponent<GUILayer>();
                        if (component != null)
                        {
                            GUIElement element = component.HitTest(mousePosition);
                            if (element != null)
                            {
                                m_CurrentHit[0].target = element.gameObject;
                                m_CurrentHit[0].camera = camera;
                            }
                            else
                            {
                                m_CurrentHit[0].target = null;
                                m_CurrentHit[0].camera = null;
                            }
                        }
                        if (camera.eventMask != 0)
                        {
                            Ray ray = camera.ScreenPointToRay(mousePosition);
                            float z = ray.direction.z;
                            float distance = !Mathf.Approximately(0f, z) ? Mathf.Abs((float) ((camera.farClipPlane - camera.nearClipPlane) / z)) : float.PositiveInfinity;
                            GameObject obj2 = camera.RaycastTry(ray, distance, camera.cullingMask & camera.eventMask);
                            if (obj2 != null)
                            {
                                m_CurrentHit[1].target = obj2;
                                m_CurrentHit[1].camera = camera;
                            }
                            else if ((camera.clearFlags == CameraClearFlags.Skybox) || (camera.clearFlags == CameraClearFlags.Color))
                            {
                                m_CurrentHit[1].target = null;
                                m_CurrentHit[1].camera = null;
                            }
                            GameObject obj3 = camera.RaycastTry2D(ray, distance, camera.cullingMask & camera.eventMask);
                            if (obj3 != null)
                            {
                                m_CurrentHit[2].target = obj3;
                                m_CurrentHit[2].camera = camera;
                            }
                            else if ((camera.clearFlags == CameraClearFlags.Skybox) || (camera.clearFlags == CameraClearFlags.Color))
                            {
                                m_CurrentHit[2].target = null;
                                m_CurrentHit[2].camera = null;
                            }
                        }
                    }
                }
            }
            for (int j = 0; j < m_CurrentHit.Length; j++)
            {
                SendEvents(j, m_CurrentHit[j]);
            }
            s_MouseUsed = false;
        }

        private static void SendEvents(int i, HitInfo hit)
        {
            bool mouseButtonDown = Input.GetMouseButtonDown(0);
            bool mouseButton = Input.GetMouseButton(0);
            if (mouseButtonDown)
            {
                if (hit != 0)
                {
                    m_MouseDownHit[i] = hit;
                    m_MouseDownHit[i].SendMessage("OnMouseDown");
                }
            }
            else if (!mouseButton)
            {
                if (m_MouseDownHit[i] != 0)
                {
                    if (HitInfo.Compare(hit, m_MouseDownHit[i]))
                    {
                        m_MouseDownHit[i].SendMessage("OnMouseUpAsButton");
                    }
                    m_MouseDownHit[i].SendMessage("OnMouseUp");
                    m_MouseDownHit[i] = new HitInfo();
                }
            }
            else if (m_MouseDownHit[i] != 0)
            {
                m_MouseDownHit[i].SendMessage("OnMouseDrag");
            }
            if (HitInfo.Compare(hit, m_LastHit[i]))
            {
                if (hit != 0)
                {
                    hit.SendMessage("OnMouseOver");
                }
            }
            else
            {
                if (m_LastHit[i] != 0)
                {
                    m_LastHit[i].SendMessage("OnMouseExit");
                }
                if (hit != 0)
                {
                    hit.SendMessage("OnMouseEnter");
                    hit.SendMessage("OnMouseOver");
                }
            }
            m_LastHit[i] = hit;
        }

        [RequiredByNativeCode]
        private static void SetMouseMoved()
        {
            s_MouseUsed = true;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HitInfo
        {
            public GameObject target;
            public Camera camera;
            public void SendMessage(string name)
            {
                this.target.SendMessage(name, null, SendMessageOptions.DontRequireReceiver);
            }

            public static bool Compare(SendMouseEvents.HitInfo lhs, SendMouseEvents.HitInfo rhs)
            {
                return ((lhs.target == rhs.target) && (lhs.camera == rhs.camera));
            }

            public static implicit operator bool(SendMouseEvents.HitInfo exists)
            {
                return ((exists.target != null) && (exists.camera != null));
            }
        }
    }
}

