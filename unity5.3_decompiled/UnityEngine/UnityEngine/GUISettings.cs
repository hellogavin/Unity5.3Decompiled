namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public sealed class GUISettings
    {
        [SerializeField]
        private Color m_CursorColor = Color.white;
        [SerializeField]
        private float m_CursorFlashSpeed = -1f;
        [SerializeField]
        private bool m_DoubleClickSelectsWord = true;
        [SerializeField]
        private Color m_SelectionColor = new Color(0.5f, 0.5f, 1f);
        [SerializeField]
        private bool m_TripleClickSelectsLine = true;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float Internal_GetCursorFlashSpeed();

        public Color cursorColor
        {
            get
            {
                return this.m_CursorColor;
            }
            set
            {
                this.m_CursorColor = value;
            }
        }

        public float cursorFlashSpeed
        {
            get
            {
                if (this.m_CursorFlashSpeed >= 0f)
                {
                    return this.m_CursorFlashSpeed;
                }
                return Internal_GetCursorFlashSpeed();
            }
            set
            {
                this.m_CursorFlashSpeed = value;
            }
        }

        public bool doubleClickSelectsWord
        {
            get
            {
                return this.m_DoubleClickSelectsWord;
            }
            set
            {
                this.m_DoubleClickSelectsWord = value;
            }
        }

        public Color selectionColor
        {
            get
            {
                return this.m_SelectionColor;
            }
            set
            {
                this.m_SelectionColor = value;
            }
        }

        public bool tripleClickSelectsLine
        {
            get
            {
                return this.m_TripleClickSelectsLine;
            }
            set
            {
                this.m_TripleClickSelectsLine = value;
            }
        }
    }
}

