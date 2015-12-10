namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    internal class SceneViewGrid
    {
        private static PrefColor kViewGridColor = new PrefColor("Scene/Grid", 0.5f, 0.5f, 0.5f, 0.4f);
        [SerializeField]
        private AnimBool xGrid = new AnimBool();
        [SerializeField]
        private AnimBool yGrid = new AnimBool();
        [SerializeField]
        private AnimBool zGrid = new AnimBool();

        public DrawGridParameters PrepareGridRender(Camera camera, Vector3 pivot, Quaternion rotation, float size, bool orthoMode, bool gridVisible)
        {
            DrawGridParameters parameters;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            if (gridVisible)
            {
                if (orthoMode)
                {
                    Vector3 vector = (Vector3) (rotation * Vector3.forward);
                    if (Mathf.Abs(vector.y) > 0.2f)
                    {
                        flag2 = true;
                    }
                    else if ((vector == Vector3.left) || (vector == Vector3.right))
                    {
                        flag = true;
                    }
                    else if ((vector == Vector3.forward) || (vector == Vector3.back))
                    {
                        flag3 = true;
                    }
                }
                else
                {
                    flag2 = true;
                }
            }
            this.xGrid.target = flag;
            this.yGrid.target = flag2;
            this.zGrid.target = flag3;
            parameters.pivot = pivot;
            parameters.color = (Color) kViewGridColor;
            parameters.size = size;
            parameters.alphaX = this.xGrid.faded;
            parameters.alphaY = this.yGrid.faded;
            parameters.alphaZ = this.zGrid.faded;
            return parameters;
        }

        public void Register(SceneView source)
        {
            this.xGrid.valueChanged.AddListener(new UnityAction(source.Repaint));
            this.yGrid.valueChanged.AddListener(new UnityAction(source.Repaint));
            this.zGrid.valueChanged.AddListener(new UnityAction(source.Repaint));
        }
    }
}

