namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SplitterState
    {
        public int currentActiveSplitter;
        private const int defaultSplitSize = 6;
        public int ID;
        public int lastTotalSize;
        public int[] maxSizes;
        public int[] minSizes;
        public int[] realSizes;
        public float[] relativeSizes;
        public int splitSize;
        public int splitterInitialOffset;
        public float xOffset;

        public SplitterState(params float[] relativeSizes)
        {
            this.currentActiveSplitter = -1;
            this.Init(relativeSizes, null, null, 0);
        }

        public SplitterState(int[] realSizes, int[] minSizes, int[] maxSizes)
        {
            this.currentActiveSplitter = -1;
            this.realSizes = realSizes;
            this.minSizes = (minSizes != null) ? minSizes : new int[realSizes.Length];
            this.maxSizes = (maxSizes != null) ? maxSizes : new int[realSizes.Length];
            this.relativeSizes = new float[realSizes.Length];
            this.splitSize = (this.splitSize != 0) ? this.splitSize : 6;
            this.RealToRelativeSizes();
        }

        public SplitterState(float[] relativeSizes, int[] minSizes, int[] maxSizes)
        {
            this.currentActiveSplitter = -1;
            this.Init(relativeSizes, minSizes, maxSizes, 0);
        }

        public SplitterState(float[] relativeSizes, int[] minSizes, int[] maxSizes, int splitSize)
        {
            this.currentActiveSplitter = -1;
            this.Init(relativeSizes, minSizes, maxSizes, splitSize);
        }

        public void DoSplitter(int i1, int i2, int diff)
        {
            int num = this.realSizes[i1];
            int num2 = this.realSizes[i2];
            int num3 = this.minSizes[i1];
            int num4 = this.minSizes[i2];
            int num5 = this.maxSizes[i1];
            int num6 = this.maxSizes[i2];
            bool flag = false;
            if (num3 == 0)
            {
                num3 = 0x10;
            }
            if (num4 == 0)
            {
                num4 = 0x10;
            }
            if ((num + diff) < num3)
            {
                diff -= num3 - num;
                this.realSizes[i2] += this.realSizes[i1] - num3;
                this.realSizes[i1] = num3;
                if (i1 != 0)
                {
                    this.DoSplitter(i1 - 1, i2, diff);
                }
                else
                {
                    this.splitterInitialOffset -= diff;
                }
                flag = true;
            }
            else if ((num2 - diff) < num4)
            {
                diff -= num2 - num4;
                this.realSizes[i1] += this.realSizes[i2] - num4;
                this.realSizes[i2] = num4;
                if (i2 != (this.realSizes.Length - 1))
                {
                    this.DoSplitter(i1, i2 + 1, diff);
                }
                else
                {
                    this.splitterInitialOffset -= diff;
                }
                flag = true;
            }
            if (!flag)
            {
                if ((num5 != 0) && ((num + diff) > num5))
                {
                    diff -= this.realSizes[i1] - num5;
                    this.realSizes[i2] += this.realSizes[i1] - num5;
                    this.realSizes[i1] = num5;
                    if (i1 != 0)
                    {
                        this.DoSplitter(i1 - 1, i2, diff);
                    }
                    else
                    {
                        this.splitterInitialOffset -= diff;
                    }
                    flag = true;
                }
                else if ((num6 != 0) && ((num2 - diff) > num6))
                {
                    diff -= num2 - num6;
                    this.realSizes[i1] += this.realSizes[i2] - num6;
                    this.realSizes[i2] = num6;
                    if (i2 != (this.realSizes.Length - 1))
                    {
                        this.DoSplitter(i1, i2 + 1, diff);
                    }
                    else
                    {
                        this.splitterInitialOffset -= diff;
                    }
                    flag = true;
                }
            }
            if (!flag)
            {
                this.realSizes[i1] += diff;
                this.realSizes[i2] -= diff;
            }
        }

        private void Init(float[] relativeSizes, int[] minSizes, int[] maxSizes, int splitSize)
        {
            this.relativeSizes = relativeSizes;
            this.minSizes = (minSizes != null) ? minSizes : new int[relativeSizes.Length];
            this.maxSizes = (maxSizes != null) ? maxSizes : new int[relativeSizes.Length];
            this.realSizes = new int[relativeSizes.Length];
            this.splitSize = (splitSize != 0) ? splitSize : 6;
            this.NormalizeRelativeSizes();
        }

        public void NormalizeRelativeSizes()
        {
            int num3;
            float num = 1f;
            float num2 = 0f;
            for (num3 = 0; num3 < this.relativeSizes.Length; num3++)
            {
                num2 += this.relativeSizes[num3];
            }
            for (num3 = 0; num3 < this.relativeSizes.Length; num3++)
            {
                this.relativeSizes[num3] /= num2;
                num -= this.relativeSizes[num3];
            }
            this.relativeSizes[this.relativeSizes.Length - 1] += num;
        }

        public void RealToRelativeSizes()
        {
            int num3;
            float num = 1f;
            float num2 = 0f;
            for (num3 = 0; num3 < this.realSizes.Length; num3++)
            {
                num2 += this.realSizes[num3];
            }
            for (num3 = 0; num3 < this.realSizes.Length; num3++)
            {
                this.relativeSizes[num3] = ((float) this.realSizes[num3]) / num2;
                num -= this.relativeSizes[num3];
            }
            if (this.relativeSizes.Length > 0)
            {
                this.relativeSizes[this.relativeSizes.Length - 1] += num;
            }
        }

        public void RelativeToRealSizes(int totalSpace)
        {
            int num2;
            int num = totalSpace;
            for (num2 = 0; num2 < this.relativeSizes.Length; num2++)
            {
                this.realSizes[num2] = (int) Mathf.Round(this.relativeSizes[num2] * totalSpace);
                if (this.realSizes[num2] < this.minSizes[num2])
                {
                    this.realSizes[num2] = this.minSizes[num2];
                }
                num -= this.realSizes[num2];
            }
            if (num < 0)
            {
                for (num2 = 0; num2 < this.relativeSizes.Length; num2++)
                {
                    if (this.realSizes[num2] > this.minSizes[num2])
                    {
                        int num3 = this.realSizes[num2] - this.minSizes[num2];
                        int num4 = (-num >= num3) ? num3 : -num;
                        num += num4;
                        this.realSizes[num2] -= num4;
                        if (num >= 0)
                        {
                            break;
                        }
                    }
                }
            }
            int index = this.realSizes.Length - 1;
            if (index >= 0)
            {
                this.realSizes[index] += num;
                if (this.realSizes[index] < this.minSizes[index])
                {
                    this.realSizes[index] = this.minSizes[index];
                }
            }
        }
    }
}

