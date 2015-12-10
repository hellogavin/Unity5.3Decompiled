namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    internal class GameViewSizeGroup
    {
        [NonSerialized]
        private List<GameViewSize> m_Builtin = new List<GameViewSize>();
        [SerializeField]
        private List<GameViewSize> m_Custom = new List<GameViewSize>();

        public void AddBuiltinSize(GameViewSize size)
        {
            this.m_Builtin.Add(size);
            ScriptableSingleton<GameViewSizes>.instance.Changed();
        }

        public void AddBuiltinSizes(params GameViewSize[] sizes)
        {
            for (int i = 0; i < sizes.Length; i++)
            {
                this.AddBuiltinSize(sizes[i]);
            }
        }

        public void AddCustomSize(GameViewSize size)
        {
            this.m_Custom.Add(size);
            ScriptableSingleton<GameViewSizes>.instance.Changed();
        }

        public void AddCustomSizes(params GameViewSize[] sizes)
        {
            for (int i = 0; i < sizes.Length; i++)
            {
                this.AddCustomSize(sizes[i]);
            }
        }

        public int GetBuiltinCount()
        {
            return this.m_Builtin.Count;
        }

        public int GetCustomCount()
        {
            return this.m_Custom.Count;
        }

        public string[] GetDisplayTexts()
        {
            List<string> list = new List<string>();
            foreach (GameViewSize size in this.m_Builtin)
            {
                list.Add(size.displayText);
            }
            foreach (GameViewSize size2 in this.m_Custom)
            {
                list.Add(size2.displayText);
            }
            return list.ToArray();
        }

        public GameViewSize GetGameViewSize(int index)
        {
            if (index < this.m_Builtin.Count)
            {
                return this.m_Builtin[index];
            }
            index -= this.m_Builtin.Count;
            if ((index >= 0) && (index < this.m_Custom.Count))
            {
                return this.m_Custom[index];
            }
            Debug.LogError(string.Concat(new object[] { "Invalid index ", index + this.m_Builtin.Count, " ", this.m_Builtin.Count, " ", this.m_Custom.Count }));
            return new GameViewSize(GameViewSizeType.AspectRatio, 0, 0, string.Empty);
        }

        public int GetTotalCount()
        {
            return (this.m_Builtin.Count + this.m_Custom.Count);
        }

        public int IndexOf(GameViewSize view)
        {
            int index = this.m_Builtin.IndexOf(view);
            if (index >= 0)
            {
                return index;
            }
            return this.m_Custom.IndexOf(view);
        }

        public bool IsCustomSize(int index)
        {
            if (index < this.m_Builtin.Count)
            {
                return false;
            }
            return true;
        }

        public void RemoveCustomSize(int index)
        {
            int num = this.TotalIndexToCustomIndex(index);
            if ((num >= 0) && (num < this.m_Custom.Count))
            {
                this.m_Custom.RemoveAt(num);
                ScriptableSingleton<GameViewSizes>.instance.Changed();
            }
            else
            {
                Debug.LogError(string.Concat(new object[] { "Invalid index ", index, " ", this.m_Builtin.Count, " ", this.m_Custom.Count }));
            }
        }

        public int TotalIndexToCustomIndex(int index)
        {
            return (index - this.m_Builtin.Count);
        }
    }
}

