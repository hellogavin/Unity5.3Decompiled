namespace UnityEditor
{
    using System;
    using System.Collections;
    using UnityEngine;

    internal class CurveRendererCache
    {
        private static Hashtable m_CombiRenderers = new Hashtable();
        private static Hashtable m_NormalRenderers = new Hashtable();

        public static void ClearCurveRendererCache()
        {
            m_CombiRenderers = new Hashtable();
            m_NormalRenderers = new Hashtable();
        }

        public static CurveRenderer GetCurveRenderer(AnimationClip clip, EditorCurveBinding curveBinding)
        {
            if ((curveBinding.type == typeof(Transform)) && curveBinding.propertyName.StartsWith("localEulerAngles."))
            {
                int curveIndexFromName = RotationCurveInterpolation.GetCurveIndexFromName(curveBinding.propertyName);
                string str = CurveUtility.GetCurveGroupID(clip, curveBinding).ToString();
                EulerCurveCombinedRenderer renderer = (EulerCurveCombinedRenderer) m_CombiRenderers[str];
                if (renderer == null)
                {
                    renderer = new EulerCurveCombinedRenderer(AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "m_LocalRotation.x")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "m_LocalRotation.y")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "m_LocalRotation.z")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "m_LocalRotation.w")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "localEulerAngles.x")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "localEulerAngles.y")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "localEulerAngles.z")));
                    m_CombiRenderers.Add(str, renderer);
                }
                return new EulerCurveRenderer(curveIndexFromName, renderer);
            }
            string key = CurveUtility.GetCurveID(clip, curveBinding).ToString();
            NormalCurveRenderer renderer2 = (NormalCurveRenderer) m_NormalRenderers[key];
            if (renderer2 == null)
            {
                renderer2 = new NormalCurveRenderer(AnimationUtility.GetEditorCurve(clip, curveBinding));
                m_NormalRenderers.Add(key, renderer2);
            }
            return renderer2;
        }
    }
}

