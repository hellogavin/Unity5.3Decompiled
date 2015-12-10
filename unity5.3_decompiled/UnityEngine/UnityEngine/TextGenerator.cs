namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public sealed class TextGenerator : IDisposable
    {
        internal IntPtr m_Ptr;
        private string m_LastString;
        private TextGenerationSettings m_LastSettings;
        private bool m_HasGenerated;
        private bool m_LastValid;
        private readonly List<UIVertex> m_Verts;
        private readonly List<UICharInfo> m_Characters;
        private readonly List<UILineInfo> m_Lines;
        private bool m_CachedVerts;
        private bool m_CachedCharacters;
        private bool m_CachedLines;
        private static int s_NextId = 0;
        private int m_Id;
        private static readonly Dictionary<int, WeakReference> s_Instances = new Dictionary<int, WeakReference>();
        public TextGenerator() : this(50)
        {
        }

        public TextGenerator(int initialCapacity)
        {
            this.m_Verts = new List<UIVertex>((initialCapacity + 1) * 4);
            this.m_Characters = new List<UICharInfo>(initialCapacity + 1);
            this.m_Lines = new List<UILineInfo>(20);
            this.Init();
            Dictionary<int, WeakReference> dictionary = s_Instances;
            lock (dictionary)
            {
                this.m_Id = s_NextId++;
                s_Instances.Add(this.m_Id, new WeakReference(this));
            }
        }

        void IDisposable.Dispose()
        {
            Dictionary<int, WeakReference> dictionary = s_Instances;
            lock (dictionary)
            {
                s_Instances.Remove(this.m_Id);
            }
            this.Dispose_cpp();
        }

        ~TextGenerator()
        {
            ((IDisposable) this).Dispose();
        }

        [RequiredByNativeCode]
        internal static void InvalidateAll()
        {
            Dictionary<int, WeakReference> dictionary = s_Instances;
            lock (dictionary)
            {
                foreach (KeyValuePair<int, WeakReference> pair in s_Instances)
                {
                    WeakReference reference = pair.Value;
                    if (reference.IsAlive)
                    {
                        (reference.Target as TextGenerator).Invalidate();
                    }
                }
            }
        }

        private TextGenerationSettings ValidatedSettings(TextGenerationSettings settings)
        {
            if ((settings.font == null) || !settings.font.dynamic)
            {
                if ((settings.fontSize != 0) || (settings.fontStyle != FontStyle.Normal))
                {
                    Debug.LogWarning("Font size and style overrides are only supported for dynamic fonts.");
                    settings.fontSize = 0;
                    settings.fontStyle = FontStyle.Normal;
                }
                if (settings.resizeTextForBestFit)
                {
                    Debug.LogWarning("BestFit is only supported for dynamic fonts.");
                    settings.resizeTextForBestFit = false;
                }
            }
            return settings;
        }

        public void Invalidate()
        {
            this.m_HasGenerated = false;
        }

        public void GetCharacters(List<UICharInfo> characters)
        {
            this.GetCharactersInternal(characters);
        }

        public void GetLines(List<UILineInfo> lines)
        {
            this.GetLinesInternal(lines);
        }

        public void GetVertices(List<UIVertex> vertices)
        {
            this.GetVerticesInternal(vertices);
        }

        public float GetPreferredWidth(string str, TextGenerationSettings settings)
        {
            settings.horizontalOverflow = HorizontalWrapMode.Overflow;
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            settings.updateBounds = true;
            this.Populate(str, settings);
            return this.rectExtents.width;
        }

        public float GetPreferredHeight(string str, TextGenerationSettings settings)
        {
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            settings.updateBounds = true;
            this.Populate(str, settings);
            return this.rectExtents.height;
        }

        public bool Populate(string str, TextGenerationSettings settings)
        {
            if ((this.m_HasGenerated && (str == this.m_LastString)) && settings.Equals(this.m_LastSettings))
            {
                return this.m_LastValid;
            }
            return this.PopulateAlways(str, settings);
        }

        private bool PopulateAlways(string str, TextGenerationSettings settings)
        {
            this.m_LastString = str;
            this.m_HasGenerated = true;
            this.m_CachedVerts = false;
            this.m_CachedCharacters = false;
            this.m_CachedLines = false;
            this.m_LastSettings = settings;
            TextGenerationSettings settings2 = this.ValidatedSettings(settings);
            this.m_LastValid = this.Populate_Internal(str, settings2.font, settings2.color, settings2.fontSize, settings2.scaleFactor, settings2.lineSpacing, settings2.fontStyle, settings2.richText, settings2.resizeTextForBestFit, settings2.resizeTextMinSize, settings2.resizeTextMaxSize, settings2.verticalOverflow, settings2.horizontalOverflow, settings2.updateBounds, settings2.textAnchor, settings2.generationExtents, settings2.pivot, settings2.generateOutOfBounds, settings2.alignByGeometry);
            return this.m_LastValid;
        }

        public IList<UIVertex> verts
        {
            get
            {
                if (!this.m_CachedVerts)
                {
                    this.GetVertices(this.m_Verts);
                    this.m_CachedVerts = true;
                }
                return this.m_Verts;
            }
        }
        public IList<UICharInfo> characters
        {
            get
            {
                if (!this.m_CachedCharacters)
                {
                    this.GetCharacters(this.m_Characters);
                    this.m_CachedCharacters = true;
                }
                return this.m_Characters;
            }
        }
        public IList<UILineInfo> lines
        {
            get
            {
                if (!this.m_CachedLines)
                {
                    this.GetLines(this.m_Lines);
                    this.m_CachedLines = true;
                }
                return this.m_Lines;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Init();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Dispose_cpp();
        internal bool Populate_Internal(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, VerticalWrapMode verticalOverFlow, HorizontalWrapMode horizontalOverflow, bool updateBounds, TextAnchor anchor, Vector2 extents, Vector2 pivot, bool generateOutOfBounds, bool alignByGeometry)
        {
            return this.Populate_Internal_cpp(str, font, color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, (int) verticalOverFlow, (int) horizontalOverflow, updateBounds, anchor, extents.x, extents.y, pivot.x, pivot.y, generateOutOfBounds, alignByGeometry);
        }

        internal bool Populate_Internal_cpp(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry)
        {
            return INTERNAL_CALL_Populate_Internal_cpp(this, str, font, ref color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, verticalOverFlow, horizontalOverflow, updateBounds, anchor, extentsX, extentsY, pivotX, pivotY, generateOutOfBounds, alignByGeometry);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Populate_Internal_cpp(TextGenerator self, string str, Font font, ref Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry);
        public Rect rectExtents
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_rectExtents(out rect);
                return rect;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_rectExtents(out Rect value);
        public int vertexCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetVerticesInternal(object vertices);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern UIVertex[] GetVerticesArray();
        public int characterCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public int characterCountVisible
        {
            get
            {
                return (!string.IsNullOrEmpty(this.m_LastString) ? Mathf.Min(this.m_LastString.Length, Mathf.Max(0, (this.vertexCount - 4) / 4)) : 0);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetCharactersInternal(object characters);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern UICharInfo[] GetCharactersArray();
        public int lineCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetLinesInternal(object lines);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern UILineInfo[] GetLinesArray();
        public int fontSizeUsedForBestFit { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

