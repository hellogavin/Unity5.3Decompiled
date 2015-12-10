namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SplitView : View, ICleanuppable, IDropArea
    {
        public int controlID;
        internal const float kGrabDist = 5f;
        private static float[] s_DragPos;
        private static float[] s_StartDragPos;
        private SplitterState splitState;
        public bool vertical;

        public override void AddChild(View child, int idx)
        {
            base.AddChild(child, idx);
            this.ChildrenMinMaxChanged();
            this.splitState = null;
        }

        private void CalcRoomForRect(Rect[] sources, Rect r)
        {
            float num16;
            float num = !this.vertical ? r.x : r.y;
            float num2 = num + (!this.vertical ? r.width : r.height);
            float num3 = (num + num2) * 0.5f;
            int index = 0;
            while (index < sources.Length)
            {
                float num5 = !this.vertical ? (sources[index].x + (sources[index].width * 0.5f)) : (sources[index].y + (sources[index].height * 0.5f));
                if (num5 > num3)
                {
                    break;
                }
                index++;
            }
            float num6 = num;
            for (int i = index - 1; i >= 0; i--)
            {
                if (this.vertical)
                {
                    sources[i].yMax = num6;
                    if (sources[i].height >= base.children[i].minSize.y)
                    {
                        break;
                    }
                    num16 = sources[i].yMax - base.children[i].minSize.y;
                    sources[i].yMin = num16;
                    num6 = num16;
                }
                else
                {
                    sources[i].xMax = num6;
                    if (sources[i].width >= base.children[i].minSize.x)
                    {
                        break;
                    }
                    num16 = sources[i].xMax - base.children[i].minSize.x;
                    sources[i].xMin = num16;
                    num6 = num16;
                }
            }
            if (num6 < 0f)
            {
                float num8 = -num6;
                for (int k = 0; k < (index - 1); k++)
                {
                    if (this.vertical)
                    {
                        sources[k].y += num8;
                    }
                    else
                    {
                        sources[k].x += num8;
                    }
                }
                num2 += num8;
            }
            num6 = num2;
            for (int j = index; j < sources.Length; j++)
            {
                if (this.vertical)
                {
                    float yMax = sources[j].yMax;
                    sources[j].yMin = num6;
                    sources[j].yMax = yMax;
                    if (sources[j].height >= base.children[j].minSize.y)
                    {
                        break;
                    }
                    num16 = sources[j].yMin + base.children[j].minSize.y;
                    sources[j].yMax = num16;
                    num6 = num16;
                }
                else
                {
                    float xMax = sources[j].xMax;
                    sources[j].xMin = num6;
                    sources[j].xMax = xMax;
                    if (sources[j].width >= base.children[j].minSize.x)
                    {
                        break;
                    }
                    num16 = sources[j].xMin + base.children[j].minSize.x;
                    sources[j].xMax = num16;
                    num6 = num16;
                }
            }
            float num13 = !this.vertical ? base.position.width : base.position.height;
            if (num6 > num13)
            {
                float num14 = num13 - num6;
                for (int m = 0; m < (index - 1); m++)
                {
                    if (this.vertical)
                    {
                        sources[m].y += num14;
                    }
                    else
                    {
                        sources[m].x += num14;
                    }
                }
                num2 += num14;
            }
        }

        private DropInfo CheckRootWindowDropZones(Vector2 mouseScreenPosition)
        {
            DropInfo info = null;
            if ((base.parent is SplitView) || ((base.children.Length == 1) && (DockArea.s_IgnoreDockingForView == base.children[0])))
            {
                return info;
            }
            Rect screenPosition = base.screenPosition;
            if (base.parent is MainWindow)
            {
                info = this.DoDropZone(-1, mouseScreenPosition, new Rect(screenPosition.x, screenPosition.yMax, screenPosition.width, 100f), new Rect(screenPosition.x, screenPosition.yMax - 200f, screenPosition.width, 200f));
            }
            else
            {
                info = this.DoDropZone(-1, mouseScreenPosition, new Rect(screenPosition.x, screenPosition.yMax - 20f, screenPosition.width, 100f), new Rect(screenPosition.x, screenPosition.yMax - 50f, screenPosition.width, 200f));
            }
            if (info != null)
            {
                return info;
            }
            info = this.DoDropZone(-2, mouseScreenPosition, new Rect(screenPosition.x - 30f, screenPosition.y, 50f, screenPosition.height), new Rect(screenPosition.x - 50f, screenPosition.y, 100f, screenPosition.height));
            if (info != null)
            {
                return info;
            }
            return this.DoDropZone(-3, mouseScreenPosition, new Rect(screenPosition.xMax - 20f, screenPosition.y, 50f, screenPosition.height), new Rect(screenPosition.xMax - 50f, screenPosition.y, 100f, screenPosition.height));
        }

        protected override void ChildrenMinMaxChanged()
        {
            Vector2 zero = Vector2.zero;
            Vector2 max = Vector2.zero;
            if (this.vertical)
            {
                foreach (View view in base.children)
                {
                    zero.x = Mathf.Max(view.minSize.x, zero.x);
                    max.x = Mathf.Max(view.maxSize.x, max.x);
                    zero.y += view.minSize.y;
                    max.y += view.maxSize.y;
                }
            }
            else
            {
                foreach (View view2 in base.children)
                {
                    zero.x += view2.minSize.x;
                    max.x += view2.maxSize.x;
                    zero.y = Mathf.Max(view2.minSize.y, zero.y);
                    max.y = Mathf.Max(view2.maxSize.y, max.y);
                }
            }
            this.splitState = null;
            base.SetMinMaxSizes(zero, max);
        }

        public void Cleanup()
        {
            SplitView parent = base.parent as SplitView;
            if ((base.children.Length == 1) && (parent != null))
            {
                View child = base.children[0];
                child.position = base.position;
                if (base.parent != null)
                {
                    base.parent.AddChild(child, base.parent.IndexOfChild(this));
                    base.parent.RemoveChild(this);
                    if (parent != null)
                    {
                        parent.Cleanup();
                    }
                    child.position = base.position;
                    if (!Unsupported.IsDestroyScriptableObject(this))
                    {
                        Object.DestroyImmediate(this);
                    }
                    return;
                }
                if (child is SplitView)
                {
                    this.RemoveChild(child);
                    base.window.mainView = child;
                    child.position = new Rect(0f, 0f, child.window.position.width, base.window.position.height);
                    child.Reflow();
                    if (!Unsupported.IsDestroyScriptableObject(this))
                    {
                        Object.DestroyImmediate(this);
                    }
                    return;
                }
            }
            if (parent != null)
            {
                parent.Cleanup();
                parent = base.parent as SplitView;
                if ((parent != null) && (parent.vertical == this.vertical))
                {
                    int index = new List<View>(base.parent.children).IndexOf(this);
                    foreach (View view3 in base.children)
                    {
                        parent.AddChild(view3, index++);
                        view3.position = new Rect(base.position.x + view3.position.x, base.position.y + view3.position.y, view3.position.width, view3.position.height);
                    }
                }
            }
            if (base.children.Length == 0)
            {
                if ((base.parent == null) && (base.window != null))
                {
                    base.window.Close();
                }
                else
                {
                    ICleanuppable cleanuppable = base.parent as ICleanuppable;
                    if (base.parent is SplitView)
                    {
                        ((SplitView) base.parent).RemoveChildNice(this);
                        if (!Unsupported.IsDestroyScriptableObject(this))
                        {
                            Object.DestroyImmediate(this, true);
                        }
                    }
                    cleanuppable.Cleanup();
                }
            }
            else
            {
                this.splitState = null;
                this.Reflow();
            }
        }

        private DropInfo DoDropZone(int idx, Vector2 mousePos, Rect sourceRect, Rect previewRect)
        {
            if (!sourceRect.Contains(mousePos))
            {
                return null;
            }
            return new DropInfo(this) { type = DropInfo.Type.Pane, userData = idx, rect = previewRect };
        }

        public DropInfo DragOver(EditorWindow w, Vector2 mouseScreenPosition)
        {
            DropInfo info = this.CheckRootWindowDropZones(mouseScreenPosition);
            if (info != null)
            {
                return info;
            }
            for (int i = 0; i < base.children.Length; i++)
            {
                View view = base.children[i];
                if ((view != DockArea.s_IgnoreDockingForView) && !(view is SplitView))
                {
                    Rect screenPosition = view.screenPosition;
                    int num2 = 0;
                    float width = Mathf.Round(Mathf.Min((float) (screenPosition.width / 3f), (float) 300f));
                    float height = Mathf.Round(Mathf.Min((float) (screenPosition.height / 3f), (float) 300f));
                    Rect rect2 = new Rect(screenPosition.x, screenPosition.y + 39f, width, screenPosition.height - 39f);
                    if (rect2.Contains(mouseScreenPosition))
                    {
                        num2 |= 1;
                    }
                    Rect rect3 = new Rect(screenPosition.x, screenPosition.yMax - height, screenPosition.width, height);
                    if (rect3.Contains(mouseScreenPosition))
                    {
                        num2 |= 2;
                    }
                    Rect rect4 = new Rect(screenPosition.xMax - width, screenPosition.y + 39f, width, screenPosition.height - 39f);
                    if (rect4.Contains(mouseScreenPosition))
                    {
                        num2 |= 4;
                    }
                    if (num2 == 3)
                    {
                        Vector2 vector = new Vector2(screenPosition.x, screenPosition.yMax) - mouseScreenPosition;
                        Vector2 vector2 = new Vector2(width, -height);
                        if (((vector.x * vector2.y) - (vector.y * vector2.x)) < 0f)
                        {
                            num2 = 1;
                        }
                        else
                        {
                            num2 = 2;
                        }
                    }
                    else if (num2 == 6)
                    {
                        Vector2 vector3 = new Vector2(screenPosition.xMax, screenPosition.yMax) - mouseScreenPosition;
                        Vector2 vector4 = new Vector2(-width, -height);
                        if (((vector3.x * vector4.y) - (vector3.y * vector4.x)) < 0f)
                        {
                            num2 = 2;
                        }
                        else
                        {
                            num2 = 4;
                        }
                    }
                    float num5 = Mathf.Round(Mathf.Max((float) (screenPosition.width / 3f), (float) 100f));
                    float num6 = Mathf.Round(Mathf.Max((float) (screenPosition.height / 3f), (float) 100f));
                    if (this.vertical)
                    {
                        switch (num2)
                        {
                            case 1:
                                return new DropInfo(this) { userData = i + 0x3e8, type = DropInfo.Type.Pane, rect = new Rect(screenPosition.x, screenPosition.y, num5, screenPosition.height) };

                            case 2:
                                return new DropInfo(this) { userData = i + 1, type = DropInfo.Type.Pane, rect = new Rect(screenPosition.x, screenPosition.yMax - num6, screenPosition.width, num6) };

                            case 4:
                                return new DropInfo(this) { userData = i + 0x7d0, type = DropInfo.Type.Pane, rect = new Rect(screenPosition.xMax - num5, screenPosition.y, num5, screenPosition.height) };
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 1:
                                return new DropInfo(this) { userData = i, type = DropInfo.Type.Pane, rect = new Rect(screenPosition.x, screenPosition.y, num5, screenPosition.height) };

                            case 2:
                                return new DropInfo(this) { userData = i + 0x7d0, type = DropInfo.Type.Pane, rect = new Rect(screenPosition.x, screenPosition.yMax - num6, screenPosition.width, num6) };

                            case 4:
                                return new DropInfo(this) { userData = i + 1, type = DropInfo.Type.Pane, rect = new Rect(screenPosition.xMax - num5, screenPosition.y, num5, screenPosition.height) };
                        }
                    }
                }
            }
            if (base.screenPosition.Contains(mouseScreenPosition) && !(base.parent is SplitView))
            {
                return new DropInfo(null);
            }
            return null;
        }

        private void MakeRoomForRect(Rect r)
        {
            Rect[] sources = new Rect[base.children.Length];
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i] = base.children[i].position;
            }
            this.CalcRoomForRect(sources, r);
            for (int j = 0; j < sources.Length; j++)
            {
                base.children[j].position = sources[j];
            }
        }

        public bool PerformDrop(EditorWindow w, DropInfo di, Vector2 screenPos)
        {
            int userData = (int) di.userData;
            DockArea child = ScriptableObject.CreateInstance<DockArea>();
            Rect r = di.rect;
            switch (userData)
            {
                case -1:
                case -2:
                case -3:
                {
                    bool flag = userData == -2;
                    bool flag2 = userData == -1;
                    this.splitState = null;
                    if ((this.vertical == flag2) || (base.children.Length < 2))
                    {
                        this.vertical = flag2;
                        r.x -= base.screenPosition.x;
                        r.y -= base.screenPosition.y;
                        this.MakeRoomForRect(r);
                        this.AddChild(child, !flag ? base.children.Length : 0);
                        child.position = r;
                    }
                    else
                    {
                        SplitView view = ScriptableObject.CreateInstance<SplitView>();
                        Rect position = base.position;
                        view.vertical = flag2;
                        view.position = new Rect(position.x, position.y, position.width, position.height);
                        if (base.window.mainView == this)
                        {
                            base.window.mainView = view;
                        }
                        else
                        {
                            base.parent.AddChild(view, base.parent.IndexOfChild(this));
                        }
                        view.AddChild(this);
                        base.position = new Rect(0f, 0f, position.width, position.height);
                        Rect rect3 = r;
                        rect3.x -= base.screenPosition.x;
                        rect3.y -= base.screenPosition.y;
                        view.MakeRoomForRect(rect3);
                        child.position = rect3;
                        view.AddChild(child, !flag ? 1 : 0);
                    }
                    break;
                }
                default:
                    if (userData < 0x3e8)
                    {
                        Rect rect4 = r;
                        rect4.x -= base.screenPosition.x;
                        rect4.y -= base.screenPosition.y;
                        this.MakeRoomForRect(rect4);
                        this.AddChild(child, userData);
                        child.position = rect4;
                    }
                    else
                    {
                        int index = userData % 0x3e8;
                        if (base.children.Length != 1)
                        {
                            SplitView view2 = ScriptableObject.CreateInstance<SplitView>();
                            view2.vertical = !this.vertical;
                            Rect rect5 = base.children[index].position;
                            view2.AddChild(base.children[index]);
                            this.AddChild(view2, index);
                            view2.position = rect5;
                            float num3 = 0f;
                            rect5.y = num3;
                            rect5.x = num3;
                            view2.children[0].position = rect5;
                            Rect rect6 = r;
                            rect6.x -= view2.screenPosition.x;
                            rect6.y -= view2.screenPosition.y;
                            view2.MakeRoomForRect(rect6);
                            view2.AddChild(child, (userData >= 0x7d0) ? 1 : 0);
                            child.position = rect6;
                        }
                        else
                        {
                            this.vertical = !this.vertical;
                            Rect rect7 = r;
                            rect7.x -= base.screenPosition.x;
                            rect7.y -= base.screenPosition.y;
                            this.MakeRoomForRect(rect7);
                            this.AddChild(child, (userData != 0x3e8) ? 1 : 0);
                            child.position = rect7;
                        }
                    }
                    break;
            }
            DockArea.s_OriginalDragSource.RemoveTab(w);
            w.m_Parent = child;
            child.AddTab(w);
            this.Reflow();
            RecalcMinMaxAndReflowAll(this);
            child.MakeVistaDWMHappyDance();
            return true;
        }

        private void PlaceView(int i, float pos, float size)
        {
            float y = Mathf.Round(pos);
            if (this.vertical)
            {
                base.children[i].position = new Rect(0f, y, base.position.width, Mathf.Round(pos + size) - y);
            }
            else
            {
                base.children[i].position = new Rect(y, 0f, Mathf.Round(pos + size) - y, base.position.height);
            }
        }

        private static string PosVals(float[] posVals)
        {
            string str = "[";
            foreach (float num in posVals)
            {
                string str2 = str;
                object[] objArray1 = new object[] { str2, string.Empty, num, ", " };
                str = string.Concat(objArray1);
            }
            return (str + "]");
        }

        private static void RecalcMinMaxAndReflowAll(SplitView start)
        {
            SplitView node = start;
            SplitView parent = start;
            do
            {
                node = parent;
                parent = node.parent as SplitView;
            }
            while (parent != null);
            RecalcMinMaxRecurse(node);
            ReflowRecurse(node);
        }

        private static void RecalcMinMaxRecurse(SplitView node)
        {
            foreach (View view in node.children)
            {
                SplitView view2 = view as SplitView;
                if (view2 != null)
                {
                    RecalcMinMaxRecurse(view2);
                }
            }
            node.ChildrenMinMaxChanged();
        }

        internal override void Reflow()
        {
            this.SetupSplitter();
            for (int i = 0; i < (base.children.Length - 1); i++)
            {
                this.splitState.DoSplitter(i, i + 1, 0);
            }
            this.splitState.RelativeToRealSizes(!this.vertical ? ((int) base.position.width) : ((int) base.position.height));
            this.SetupRectsFromSplitter();
        }

        private static void ReflowRecurse(SplitView node)
        {
            node.Reflow();
            foreach (View view in node.children)
            {
                SplitView view2 = view as SplitView;
                if (view2 != null)
                {
                    RecalcMinMaxRecurse(view2);
                }
            }
        }

        public override void RemoveChild(View child)
        {
            this.splitState = null;
            base.RemoveChild(child);
        }

        public void RemoveChildNice(View child)
        {
            if (base.children.Length != 1)
            {
                int num = base.IndexOfChild(child);
                float t = 0f;
                if (num == 0)
                {
                    t = 0f;
                }
                else if (num == (base.children.Length - 1))
                {
                    t = 1f;
                }
                else
                {
                    t = 0.5f;
                }
                t = !this.vertical ? Mathf.Lerp(child.position.xMin, child.position.xMax, t) : Mathf.Lerp(child.position.yMin, child.position.yMax, t);
                if (num > 0)
                {
                    View view = base.children[num - 1];
                    Rect position = view.position;
                    if (this.vertical)
                    {
                        position.yMax = t;
                    }
                    else
                    {
                        position.xMax = t;
                    }
                    view.position = position;
                    if (view is SplitView)
                    {
                        ((SplitView) view).Reflow();
                    }
                }
                if (num < (base.children.Length - 1))
                {
                    View view2 = base.children[num + 1];
                    Rect rect2 = view2.position;
                    if (this.vertical)
                    {
                        view2.position = new Rect(rect2.x, t, rect2.width, rect2.yMax - t);
                    }
                    else
                    {
                        view2.position = new Rect(t, rect2.y, rect2.xMax - t, rect2.height);
                    }
                    if (view2 is SplitView)
                    {
                        ((SplitView) view2).Reflow();
                    }
                }
            }
            this.RemoveChild(child);
        }

        protected override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            this.Reflow();
        }

        private void SetupRectsFromSplitter()
        {
            if (base.children.Length != 0)
            {
                int num = 0;
                int num2 = 0;
                foreach (int num3 in this.splitState.realSizes)
                {
                    num2 += num3;
                }
                float num5 = 1f;
                if (num2 > (!this.vertical ? base.position.width : base.position.height))
                {
                    num5 = (!this.vertical ? base.position.width : base.position.height) / ((float) num2);
                }
                for (int i = 0; i < base.children.Length; i++)
                {
                    int num7 = (int) Mathf.Round(this.splitState.realSizes[i] * num5);
                    if (this.vertical)
                    {
                        base.children[i].position = new Rect(0f, (float) num, base.position.width, (float) num7);
                    }
                    else
                    {
                        base.children[i].position = new Rect((float) num, 0f, (float) num7, base.position.height);
                    }
                    num += num7;
                }
            }
        }

        private void SetupSplitter()
        {
            int[] realSizes = new int[base.children.Length];
            int[] minSizes = new int[base.children.Length];
            for (int i = 0; i < base.children.Length; i++)
            {
                View view = base.children[i];
                realSizes[i] = !this.vertical ? ((int) view.position.width) : ((int) view.position.height);
                minSizes[i] = !this.vertical ? ((int) view.minSize.x) : ((int) view.minSize.y);
            }
            this.splitState = new SplitterState(realSizes, minSizes, null);
            this.splitState.splitSize = 10;
        }

        public void SplitGUI(Event evt)
        {
            if (this.splitState == null)
            {
                this.SetupSplitter();
            }
            SplitView parent = base.parent as SplitView;
            if (parent != null)
            {
                Event event2;
                event2 = new Event(evt) {
                    mousePosition = event2.mousePosition + new Vector2(base.position.x, base.position.y)
                };
                parent.SplitGUI(event2);
                if (event2.type == EventType.Used)
                {
                    evt.Use();
                }
            }
            float num = !this.vertical ? evt.mousePosition.x : evt.mousePosition.y;
            int controlID = GUIUtility.GetControlID(0x857b3, FocusType.Passive);
            this.controlID = controlID;
            switch (evt.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (base.children.Length != 1)
                    {
                        int num3 = !this.vertical ? ((int) base.children[0].position.x) : ((int) base.children[0].position.y);
                        for (int i = 0; i < (base.children.Length - 1); i++)
                        {
                            if (i >= this.splitState.realSizes.Length)
                            {
                                DockArea current = GUIView.current as DockArea;
                                string str = "Non-dock area " + GUIView.current.GetType();
                                if (((current != null) && (current.m_Selected < current.m_Panes.Count)) && (current.m_Panes[current.m_Selected] != null))
                                {
                                    str = current.m_Panes[current.m_Selected].GetType().ToString();
                                }
                                if (Unsupported.IsDeveloperBuild())
                                {
                                    Debug.LogError(string.Concat(new object[] { "Real sizes out of bounds for: ", str, " index: ", i, " RealSizes: ", this.splitState.realSizes.Length }));
                                }
                                this.SetupSplitter();
                            }
                            Rect rect = !this.vertical ? new Rect((float) ((num3 + this.splitState.realSizes[i]) - (this.splitState.splitSize / 2)), base.children[0].position.y, (float) this.splitState.splitSize, base.children[0].position.height) : new Rect(base.children[0].position.x, (float) ((num3 + this.splitState.realSizes[i]) - (this.splitState.splitSize / 2)), base.children[0].position.width, (float) this.splitState.splitSize);
                            if (rect.Contains(evt.mousePosition))
                            {
                                this.splitState.splitterInitialOffset = (int) num;
                                this.splitState.currentActiveSplitter = i;
                                GUIUtility.hotControl = controlID;
                                evt.Use();
                                break;
                            }
                            num3 += this.splitState.realSizes[i];
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    break;

                case EventType.MouseDrag:
                    if (((base.children.Length > 1) && (GUIUtility.hotControl == controlID)) && (this.splitState.currentActiveSplitter >= 0))
                    {
                        int diff = ((int) num) - this.splitState.splitterInitialOffset;
                        if (diff != 0)
                        {
                            this.splitState.splitterInitialOffset = (int) num;
                            this.splitState.DoSplitter(this.splitState.currentActiveSplitter, this.splitState.currentActiveSplitter + 1, diff);
                        }
                        this.SetupRectsFromSplitter();
                        evt.Use();
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return (!this.vertical ? "SplitView (horiz)" : "SplitView (vert)");
        }

        private class ExtraDropInfo
        {
            public Rect dropRect;
            public int idx;

            public ExtraDropInfo(Rect _dropRect, int _idx)
            {
                this.dropRect = _dropRect;
                this.idx = _idx;
            }
        }
    }
}

