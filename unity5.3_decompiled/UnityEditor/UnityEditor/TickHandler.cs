namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    internal class TickHandler
    {
        [SerializeField]
        private int m_BiggestTick = -1;
        [SerializeField]
        private float m_MaxValue = 1f;
        [SerializeField]
        private float m_MinValue;
        [SerializeField]
        private float m_PixelRange = 1f;
        [SerializeField]
        private int m_SmallestTick;
        [SerializeField]
        private float[] m_TickModulos = new float[0];
        [SerializeField]
        private float[] m_TickStrengths = new float[0];

        public int GetLevelWithMinSeparation(float pixelSeparation)
        {
            for (int i = 0; i < this.m_TickModulos.Length; i++)
            {
                float num2 = (this.m_TickModulos[i] * this.m_PixelRange) / (this.m_MaxValue - this.m_MinValue);
                if (num2 >= pixelSeparation)
                {
                    return (i - this.m_SmallestTick);
                }
            }
            return -1;
        }

        public float GetPeriodOfLevel(int level)
        {
            return this.m_TickModulos[Mathf.Clamp(this.m_SmallestTick + level, 0, this.m_TickModulos.Length - 1)];
        }

        public float GetStrengthOfLevel(int level)
        {
            return this.m_TickStrengths[this.m_SmallestTick + level];
        }

        public float[] GetTicksAtLevel(int level, bool excludeTicksFromHigherlevels)
        {
            int index = Mathf.Clamp(this.m_SmallestTick + level, 0, this.m_TickModulos.Length - 1);
            List<float> list = new List<float>();
            int num2 = Mathf.FloorToInt(this.m_MinValue / this.m_TickModulos[index]);
            int num3 = Mathf.CeilToInt(this.m_MaxValue / this.m_TickModulos[index]);
            for (int i = num2; i <= num3; i++)
            {
                if ((!excludeTicksFromHigherlevels || (index >= this.m_BiggestTick)) || ((i % Mathf.RoundToInt(this.m_TickModulos[index + 1] / this.m_TickModulos[index])) != 0))
                {
                    list.Add(i * this.m_TickModulos[index]);
                }
            }
            return list.ToArray();
        }

        public void SetRanges(float minValue, float maxValue, float minPixel, float maxPixel)
        {
            this.m_MinValue = minValue;
            this.m_MaxValue = maxValue;
            this.m_PixelRange = maxPixel - minPixel;
        }

        public void SetTickModulos(float[] tickModulos)
        {
            this.m_TickModulos = tickModulos;
        }

        public void SetTickModulosForFrameRate(float frameRate)
        {
            if (frameRate != Mathf.Round(frameRate))
            {
                float[] tickModulos = new float[] { 1f / frameRate, 5f / frameRate, 10f / frameRate, 50f / frameRate, 100f / frameRate, 500f / frameRate, 1000f / frameRate, 5000f / frameRate, 10000f / frameRate, 50000f / frameRate, 100000f / frameRate, 500000f / frameRate };
                this.SetTickModulos(tickModulos);
            }
            else
            {
                List<int> list = new List<int>();
                int item = 1;
                while (item < frameRate)
                {
                    if (item == frameRate)
                    {
                        break;
                    }
                    int num2 = Mathf.RoundToInt(frameRate / ((float) item));
                    if ((num2 % 60) == 0)
                    {
                        item *= 2;
                        list.Add(item);
                    }
                    else
                    {
                        if ((num2 % 30) == 0)
                        {
                            item *= 3;
                            list.Add(item);
                            continue;
                        }
                        if ((num2 % 20) == 0)
                        {
                            item *= 2;
                            list.Add(item);
                            continue;
                        }
                        if ((num2 % 10) == 0)
                        {
                            item *= 2;
                            list.Add(item);
                            continue;
                        }
                        if ((num2 % 5) == 0)
                        {
                            item *= 5;
                            list.Add(item);
                            continue;
                        }
                        if ((num2 % 2) == 0)
                        {
                            item *= 2;
                            list.Add(item);
                            continue;
                        }
                        if ((num2 % 3) == 0)
                        {
                            item *= 3;
                            list.Add(item);
                            continue;
                        }
                        item = Mathf.RoundToInt(frameRate);
                    }
                }
                float[] numArray = new float[9 + list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    numArray[i] = 1f / ((float) list[(list.Count - i) - 1]);
                }
                numArray[numArray.Length - 1] = 3600f;
                numArray[numArray.Length - 2] = 1800f;
                numArray[numArray.Length - 3] = 600f;
                numArray[numArray.Length - 4] = 300f;
                numArray[numArray.Length - 5] = 60f;
                numArray[numArray.Length - 6] = 30f;
                numArray[numArray.Length - 7] = 10f;
                numArray[numArray.Length - 8] = 5f;
                numArray[numArray.Length - 9] = 1f;
                this.SetTickModulos(numArray);
            }
        }

        public void SetTickStrengths(float tickMinSpacing, float tickMaxSpacing, bool sqrt)
        {
            this.m_TickStrengths = new float[this.m_TickModulos.Length];
            this.m_SmallestTick = 0;
            this.m_BiggestTick = this.m_TickModulos.Length - 1;
            for (int i = this.m_TickModulos.Length - 1; i >= 0; i--)
            {
                float num2 = (this.m_TickModulos[i] * this.m_PixelRange) / (this.m_MaxValue - this.m_MinValue);
                this.m_TickStrengths[i] = (num2 - tickMinSpacing) / (tickMaxSpacing - tickMinSpacing);
                if (this.m_TickStrengths[i] >= 1f)
                {
                    this.m_BiggestTick = i;
                }
                if (num2 <= tickMinSpacing)
                {
                    this.m_SmallestTick = i;
                    break;
                }
            }
            for (int j = this.m_SmallestTick; j <= this.m_BiggestTick; j++)
            {
                this.m_TickStrengths[j] = Mathf.Clamp01(this.m_TickStrengths[j]);
                if (sqrt)
                {
                    this.m_TickStrengths[j] = Mathf.Sqrt(this.m_TickStrengths[j]);
                }
            }
        }

        public int tickLevels
        {
            get
            {
                return ((this.m_BiggestTick - this.m_SmallestTick) + 1);
            }
        }
    }
}

