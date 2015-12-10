namespace UnityEditor
{
    using System;
    using System.IO;
    using UnityEngine;

    internal class ExportRawHeightmap : TerrainWizard
    {
        public ByteOrder m_ByteOrder = ByteOrder.Windows;
        public Depth m_Depth = Depth.Bit16;
        public bool m_FlipVertically;

        private void InitializeDefaults(Terrain terrain)
        {
            base.m_Terrain = terrain;
            object[] objArray1 = new object[] { "Width ", terrain.terrainData.heightmapWidth, " Height ", terrain.terrainData.heightmapHeight };
            base.helpString = string.Concat(objArray1);
            this.OnWizardUpdate();
        }

        public void OnEnable()
        {
            base.minSize = new Vector2(400f, 200f);
        }

        internal void OnWizardCreate()
        {
            if (base.m_Terrain == null)
            {
                base.isValid = false;
                base.errorString = "Terrain does not exist";
            }
            string path = EditorUtility.SaveFilePanel("Save Raw Heightmap", string.Empty, "terrain", "raw");
            if (path != string.Empty)
            {
                this.WriteRaw(path);
            }
        }

        internal override void OnWizardUpdate()
        {
            base.OnWizardUpdate();
            if (base.terrainData != null)
            {
                object[] objArray1 = new object[] { "Width ", base.terrainData.heightmapWidth, "\nHeight ", base.terrainData.heightmapHeight };
                base.helpString = string.Concat(objArray1);
            }
        }

        private void WriteRaw(string path)
        {
            int heightmapWidth = base.terrainData.heightmapWidth;
            int heightmapHeight = base.terrainData.heightmapHeight;
            float[,] numArray = base.terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
            byte[] array = new byte[(heightmapWidth * heightmapHeight) * this.m_Depth];
            if (this.m_Depth == Depth.Bit16)
            {
                float num3 = 65536f;
                for (int i = 0; i < heightmapHeight; i++)
                {
                    for (int j = 0; j < heightmapWidth; j++)
                    {
                        int num6 = j + (i * heightmapWidth);
                        int num7 = !this.m_FlipVertically ? i : ((heightmapHeight - 1) - i);
                        ushort num9 = (ushort) Mathf.Clamp(Mathf.RoundToInt(numArray[num7, j] * num3), 0, 0xffff);
                        byte[] bytes = BitConverter.GetBytes(num9);
                        if ((this.m_ByteOrder == ByteOrder.Mac) == BitConverter.IsLittleEndian)
                        {
                            array[num6 * 2] = bytes[1];
                            array[(num6 * 2) + 1] = bytes[0];
                        }
                        else
                        {
                            array[num6 * 2] = bytes[0];
                            array[(num6 * 2) + 1] = bytes[1];
                        }
                    }
                }
            }
            else
            {
                float num10 = 256f;
                for (int k = 0; k < heightmapHeight; k++)
                {
                    for (int m = 0; m < heightmapWidth; m++)
                    {
                        int index = m + (k * heightmapWidth);
                        int num14 = !this.m_FlipVertically ? k : ((heightmapHeight - 1) - k);
                        array[index] = (byte) Mathf.Clamp(Mathf.RoundToInt(numArray[num14, m] * num10), 0, 0xff);
                    }
                }
            }
            FileStream stream = new FileStream(path, FileMode.Create);
            stream.Write(array, 0, array.Length);
            stream.Close();
        }

        internal enum ByteOrder
        {
            Mac = 1,
            Windows = 2
        }

        internal enum Depth
        {
            Bit16 = 2,
            Bit8 = 1
        }
    }
}

