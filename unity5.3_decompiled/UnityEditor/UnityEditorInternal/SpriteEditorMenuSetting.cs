namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SpriteEditorMenuSetting : ScriptableObject
    {
        [SerializeField]
        public int autoSlicingMethod;
        [SerializeField]
        public Vector2 gridCellCount = new Vector2(1f, 1f);
        [SerializeField]
        public Vector2 gridSpriteOffset = new Vector2(0f, 0f);
        [SerializeField]
        public Vector2 gridSpritePadding = new Vector2(0f, 0f);
        [SerializeField]
        public Vector2 gridSpriteSize = new Vector2(64f, 64f);
        [SerializeField]
        public Vector2 pivot = Vector2.zero;
        [SerializeField]
        public SlicingType slicingType;
        [SerializeField]
        public int spriteAlignment;

        public enum SlicingType
        {
            Automatic,
            GridByCellSize,
            GridByCellCount
        }
    }
}

