namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SliderHandler
    {
        private readonly Rect position;
        private readonly float currentValue;
        private readonly float size;
        private readonly float start;
        private readonly float end;
        private readonly GUIStyle slider;
        private readonly GUIStyle thumb;
        private readonly bool horiz;
        private readonly int id;
        public SliderHandler(Rect position, float currentValue, float size, float start, float end, GUIStyle slider, GUIStyle thumb, bool horiz, int id)
        {
            this.position = position;
            this.currentValue = currentValue;
            this.size = size;
            this.start = start;
            this.end = end;
            this.slider = slider;
            this.thumb = thumb;
            this.horiz = horiz;
            this.id = id;
        }

        public float Handle()
        {
            if ((this.slider != null) && (this.thumb != null))
            {
                switch (this.CurrentEventType())
                {
                    case EventType.MouseDown:
                        return this.OnMouseDown();

                    case EventType.MouseUp:
                        return this.OnMouseUp();

                    case EventType.MouseDrag:
                        return this.OnMouseDrag();

                    case EventType.Repaint:
                        return this.OnRepaint();
                }
            }
            return this.currentValue;
        }

        private float OnMouseDown()
        {
            if (!this.position.Contains(this.CurrentEvent().mousePosition) || this.IsEmptySlider())
            {
                return this.currentValue;
            }
            GUI.scrollTroughSide = 0;
            GUIUtility.hotControl = this.id;
            this.CurrentEvent().Use();
            if (this.ThumbSelectionRect().Contains(this.CurrentEvent().mousePosition))
            {
                this.StartDraggingWithValue(this.ClampedCurrentValue());
                return this.currentValue;
            }
            GUI.changed = true;
            if (this.SupportsPageMovements())
            {
                this.SliderState().isDragging = false;
                GUI.nextScrollStepTime = SystemClock.now.AddMilliseconds(250.0);
                GUI.scrollTroughSide = this.CurrentScrollTroughSide();
                return this.PageMovementValue();
            }
            float dragStartValue = this.ValueForCurrentMousePosition();
            this.StartDraggingWithValue(dragStartValue);
            return this.Clamp(dragStartValue);
        }

        private float OnMouseDrag()
        {
            if (GUIUtility.hotControl != this.id)
            {
                return this.currentValue;
            }
            UnityEngine.SliderState state = this.SliderState();
            if (!state.isDragging)
            {
                return this.currentValue;
            }
            GUI.changed = true;
            this.CurrentEvent().Use();
            float num = this.MousePosition() - state.dragStartPos;
            float num2 = state.dragStartValue + (num / this.ValuesPerPixel());
            return this.Clamp(num2);
        }

        private float OnMouseUp()
        {
            if (GUIUtility.hotControl == this.id)
            {
                this.CurrentEvent().Use();
                GUIUtility.hotControl = 0;
            }
            return this.currentValue;
        }

        private float OnRepaint()
        {
            this.slider.Draw(this.position, GUIContent.none, this.id);
            if (!this.IsEmptySlider())
            {
                this.thumb.Draw(this.ThumbRect(), GUIContent.none, this.id);
            }
            if (((GUIUtility.hotControl != this.id) || !this.position.Contains(this.CurrentEvent().mousePosition)) || this.IsEmptySlider())
            {
                return this.currentValue;
            }
            if (this.ThumbRect().Contains(this.CurrentEvent().mousePosition))
            {
                if (GUI.scrollTroughSide != 0)
                {
                    GUIUtility.hotControl = 0;
                }
                return this.currentValue;
            }
            GUI.InternalRepaintEditorWindow();
            if (SystemClock.now < GUI.nextScrollStepTime)
            {
                return this.currentValue;
            }
            if (this.CurrentScrollTroughSide() != GUI.scrollTroughSide)
            {
                return this.currentValue;
            }
            GUI.nextScrollStepTime = SystemClock.now.AddMilliseconds(30.0);
            if (this.SupportsPageMovements())
            {
                this.SliderState().isDragging = false;
                GUI.changed = true;
                return this.PageMovementValue();
            }
            return this.ClampedCurrentValue();
        }

        private EventType CurrentEventType()
        {
            return this.CurrentEvent().GetTypeForControl(this.id);
        }

        private int CurrentScrollTroughSide()
        {
            float num = !this.horiz ? this.CurrentEvent().mousePosition.y : this.CurrentEvent().mousePosition.x;
            float num2 = !this.horiz ? this.ThumbRect().y : this.ThumbRect().x;
            return ((num <= num2) ? -1 : 1);
        }

        private bool IsEmptySlider()
        {
            return (this.start == this.end);
        }

        private bool SupportsPageMovements()
        {
            return ((this.size != 0f) && GUI.usePageScrollbars);
        }

        private float PageMovementValue()
        {
            float currentValue = this.currentValue;
            int num2 = (this.start <= this.end) ? 1 : -1;
            if (this.MousePosition() > this.PageUpMovementBound())
            {
                currentValue += (this.size * num2) * 0.9f;
            }
            else
            {
                currentValue -= (this.size * num2) * 0.9f;
            }
            return this.Clamp(currentValue);
        }

        private float PageUpMovementBound()
        {
            if (this.horiz)
            {
                return (this.ThumbRect().xMax - this.position.x);
            }
            return (this.ThumbRect().yMax - this.position.y);
        }

        private Event CurrentEvent()
        {
            return Event.current;
        }

        private float ValueForCurrentMousePosition()
        {
            if (this.horiz)
            {
                return ((((this.MousePosition() - (this.ThumbRect().width * 0.5f)) / this.ValuesPerPixel()) + this.start) - (this.size * 0.5f));
            }
            return ((((this.MousePosition() - (this.ThumbRect().height * 0.5f)) / this.ValuesPerPixel()) + this.start) - (this.size * 0.5f));
        }

        private float Clamp(float value)
        {
            return Mathf.Clamp(value, this.MinValue(), this.MaxValue());
        }

        private Rect ThumbSelectionRect()
        {
            return this.ThumbRect();
        }

        private void StartDraggingWithValue(float dragStartValue)
        {
            UnityEngine.SliderState state = this.SliderState();
            state.dragStartPos = this.MousePosition();
            state.dragStartValue = dragStartValue;
            state.isDragging = true;
        }

        private UnityEngine.SliderState SliderState()
        {
            return (UnityEngine.SliderState) GUIUtility.GetStateObject(typeof(UnityEngine.SliderState), this.id);
        }

        private Rect ThumbRect()
        {
            return (!this.horiz ? this.VerticalThumbRect() : this.HorizontalThumbRect());
        }

        private Rect VerticalThumbRect()
        {
            float num = this.ValuesPerPixel();
            if (this.start < this.end)
            {
                return new Rect(this.position.x + this.slider.padding.left, (((this.ClampedCurrentValue() - this.start) * num) + this.position.y) + this.slider.padding.top, this.position.width - this.slider.padding.horizontal, (this.size * num) + this.ThumbSize());
            }
            return new Rect(this.position.x + this.slider.padding.left, ((((this.ClampedCurrentValue() + this.size) - this.start) * num) + this.position.y) + this.slider.padding.top, this.position.width - this.slider.padding.horizontal, (this.size * -num) + this.ThumbSize());
        }

        private Rect HorizontalThumbRect()
        {
            float num = this.ValuesPerPixel();
            if (this.start < this.end)
            {
                return new Rect((((this.ClampedCurrentValue() - this.start) * num) + this.position.x) + this.slider.padding.left, this.position.y + this.slider.padding.top, (this.size * num) + this.ThumbSize(), this.position.height - this.slider.padding.vertical);
            }
            return new Rect(((((this.ClampedCurrentValue() + this.size) - this.start) * num) + this.position.x) + this.slider.padding.left, this.position.y, (this.size * -num) + this.ThumbSize(), this.position.height);
        }

        private float ClampedCurrentValue()
        {
            return this.Clamp(this.currentValue);
        }

        private float MousePosition()
        {
            if (this.horiz)
            {
                return (this.CurrentEvent().mousePosition.x - this.position.x);
            }
            return (this.CurrentEvent().mousePosition.y - this.position.y);
        }

        private float ValuesPerPixel()
        {
            if (this.horiz)
            {
                return (((this.position.width - this.slider.padding.horizontal) - this.ThumbSize()) / (this.end - this.start));
            }
            return (((this.position.height - this.slider.padding.vertical) - this.ThumbSize()) / (this.end - this.start));
        }

        private float ThumbSize()
        {
            if (this.horiz)
            {
                return ((this.thumb.fixedWidth == 0f) ? ((float) this.thumb.padding.horizontal) : this.thumb.fixedWidth);
            }
            return ((this.thumb.fixedHeight == 0f) ? ((float) this.thumb.padding.vertical) : this.thumb.fixedHeight);
        }

        private float MaxValue()
        {
            return (Mathf.Max(this.start, this.end) - this.size);
        }

        private float MinValue()
        {
            return Mathf.Min(this.start, this.end);
        }
    }
}

