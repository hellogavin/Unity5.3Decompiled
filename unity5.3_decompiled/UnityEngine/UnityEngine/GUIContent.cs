namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public class GUIContent
    {
        [SerializeField]
        private string m_Text;
        [SerializeField]
        private Texture m_Image;
        [SerializeField]
        private string m_Tooltip;
        private static readonly GUIContent s_Text = new GUIContent();
        private static readonly GUIContent s_Image = new GUIContent();
        private static readonly GUIContent s_TextImage = new GUIContent();
        public static GUIContent none = new GUIContent(string.Empty);
        public GUIContent()
        {
            this.m_Text = string.Empty;
            this.m_Tooltip = string.Empty;
        }

        public GUIContent(string text)
        {
            this.m_Text = string.Empty;
            this.m_Tooltip = string.Empty;
            this.m_Text = text;
        }

        public GUIContent(Texture image)
        {
            this.m_Text = string.Empty;
            this.m_Tooltip = string.Empty;
            this.m_Image = image;
        }

        public GUIContent(string text, Texture image)
        {
            this.m_Text = string.Empty;
            this.m_Tooltip = string.Empty;
            this.m_Text = text;
            this.m_Image = image;
        }

        public GUIContent(string text, string tooltip)
        {
            this.m_Text = string.Empty;
            this.m_Tooltip = string.Empty;
            this.m_Text = text;
            this.m_Tooltip = tooltip;
        }

        public GUIContent(Texture image, string tooltip)
        {
            this.m_Text = string.Empty;
            this.m_Tooltip = string.Empty;
            this.m_Image = image;
            this.m_Tooltip = tooltip;
        }

        public GUIContent(string text, Texture image, string tooltip)
        {
            this.m_Text = string.Empty;
            this.m_Tooltip = string.Empty;
            this.m_Text = text;
            this.m_Image = image;
            this.m_Tooltip = tooltip;
        }

        public GUIContent(GUIContent src)
        {
            this.m_Text = string.Empty;
            this.m_Tooltip = string.Empty;
            this.m_Text = src.m_Text;
            this.m_Image = src.m_Image;
            this.m_Tooltip = src.m_Tooltip;
        }

        public string text
        {
            get
            {
                return this.m_Text;
            }
            set
            {
                this.m_Text = value;
            }
        }
        public Texture image
        {
            get
            {
                return this.m_Image;
            }
            set
            {
                this.m_Image = value;
            }
        }
        public string tooltip
        {
            get
            {
                return this.m_Tooltip;
            }
            set
            {
                this.m_Tooltip = value;
            }
        }
        internal int hash
        {
            get
            {
                int num = 0;
                if (!string.IsNullOrEmpty(this.m_Text))
                {
                    num = this.m_Text.GetHashCode() * 0x25;
                }
                return num;
            }
        }
        internal static GUIContent Temp(string t)
        {
            s_Text.m_Text = t;
            s_Text.m_Tooltip = string.Empty;
            return s_Text;
        }

        internal static GUIContent Temp(string t, string tooltip)
        {
            s_Text.m_Text = t;
            s_Text.m_Tooltip = tooltip;
            return s_Text;
        }

        internal static GUIContent Temp(Texture i)
        {
            s_Image.m_Image = i;
            s_Image.m_Tooltip = string.Empty;
            return s_Image;
        }

        internal static GUIContent Temp(Texture i, string tooltip)
        {
            s_Image.m_Image = i;
            s_Image.m_Tooltip = tooltip;
            return s_Image;
        }

        internal static GUIContent Temp(string t, Texture i)
        {
            s_TextImage.m_Text = t;
            s_TextImage.m_Image = i;
            return s_TextImage;
        }

        internal static void ClearStaticCache()
        {
            s_Text.m_Text = null;
            s_Text.m_Tooltip = string.Empty;
            s_Image.m_Image = null;
            s_Image.m_Tooltip = string.Empty;
            s_TextImage.m_Text = null;
            s_TextImage.m_Image = null;
        }

        internal static GUIContent[] Temp(string[] texts)
        {
            GUIContent[] contentArray = new GUIContent[texts.Length];
            for (int i = 0; i < texts.Length; i++)
            {
                contentArray[i] = new GUIContent(texts[i]);
            }
            return contentArray;
        }

        internal static GUIContent[] Temp(Texture[] images)
        {
            GUIContent[] contentArray = new GUIContent[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                contentArray[i] = new GUIContent(images[i]);
            }
            return contentArray;
        }
    }
}

