namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class GameViewSizesMenuItemProvider : IFlexibleMenuItemProvider
    {
        private readonly GameViewSizeGroup m_GameViewSizeGroup;

        public GameViewSizesMenuItemProvider(GameViewSizeGroupType gameViewSizeGroupType)
        {
            this.m_GameViewSizeGroup = ScriptableSingleton<GameViewSizes>.instance.GetGroup(gameViewSizeGroupType);
        }

        public int Add(object obj)
        {
            GameViewSize size = CastToGameViewSize(obj);
            if (size == null)
            {
                return -1;
            }
            this.m_GameViewSizeGroup.AddCustomSize(size);
            ScriptableSingleton<GameViewSizes>.instance.SaveToHDD();
            return (this.Count() - 1);
        }

        private static GameViewSize CastToGameViewSize(object obj)
        {
            GameViewSize size = obj as GameViewSize;
            if (obj == null)
            {
                Debug.LogError("Incorrect input");
                return null;
            }
            return size;
        }

        public int Count()
        {
            return this.m_GameViewSizeGroup.GetTotalCount();
        }

        public object Create()
        {
            return new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, string.Empty);
        }

        public object GetItem(int index)
        {
            return this.m_GameViewSizeGroup.GetGameViewSize(index);
        }

        public string GetName(int index)
        {
            GameViewSize gameViewSize = this.m_GameViewSizeGroup.GetGameViewSize(index);
            if (gameViewSize != null)
            {
                return gameViewSize.displayText;
            }
            return string.Empty;
        }

        public int[] GetSeperatorIndices()
        {
            return new int[] { (this.m_GameViewSizeGroup.GetBuiltinCount() - 1) };
        }

        public bool IsModificationAllowed(int index)
        {
            return this.m_GameViewSizeGroup.IsCustomSize(index);
        }

        public void Move(int index, int destIndex, bool insertAfterDestIndex)
        {
            Debug.LogError("Missing impl");
        }

        public void Remove(int index)
        {
            if (index < this.m_GameViewSizeGroup.GetBuiltinCount())
            {
                Debug.LogError("Only custom game view sizes can be changed");
            }
            else
            {
                this.m_GameViewSizeGroup.RemoveCustomSize(index);
                ScriptableSingleton<GameViewSizes>.instance.SaveToHDD();
            }
        }

        public void Replace(int index, object obj)
        {
            GameViewSize other = CastToGameViewSize(obj);
            if (other != null)
            {
                if (index < this.m_GameViewSizeGroup.GetBuiltinCount())
                {
                    Debug.LogError("Only custom game view sizes can be changed");
                }
                else
                {
                    GameViewSize gameViewSize = this.m_GameViewSizeGroup.GetGameViewSize(index);
                    if (gameViewSize != null)
                    {
                        gameViewSize.Set(other);
                        ScriptableSingleton<GameViewSizes>.instance.SaveToHDD();
                    }
                }
            }
        }
    }
}

