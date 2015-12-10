namespace UnityEditor
{
    using System;
    using System.IO;
    using UnityEngine;

    internal class ImportRawHeightmap : TerrainWizard
    {
        public ByteOrder m_ByteOrder = ByteOrder.Windows;
        public Depth m_Depth = Depth.Bit16;
        public bool m_FlipVertically;
        public int m_Height = 1;
        private string m_Path;
        public Vector3 m_TerrainSize = new Vector3(2000f, 600f, 2000f);
        public int m_Width = 1;

        internal void InitializeImportRaw(Terrain terrain, string path)
        {
            base.m_Terrain = terrain;
            this.m_Path = path;
            this.PickRawDefaults(this.m_Path);
            base.helpString = "Raw files must use a single channel and be either 8 or 16 bit.";
            this.OnWizardUpdate();
        }

        internal void OnWizardCreate()
        {
            if (base.m_Terrain == null)
            {
                base.isValid = false;
                base.errorString = "Terrain does not exist";
            }
            if ((this.m_Width > 0x1001) || (this.m_Height > 0x1001))
            {
                base.isValid = false;
                base.errorString = "Heightmaps above 4097x4097 in resolution are not supported";
                Debug.LogError(base.errorString);
            }
            if (File.Exists(this.m_Path) && base.isValid)
            {
                Undo.RegisterCompleteObjectUndo(base.terrainData, "Import Raw heightmap");
                base.terrainData.heightmapResolution = Mathf.Max(this.m_Width, this.m_Height);
                base.terrainData.size = this.m_TerrainSize;
                this.ReadRaw(this.m_Path);
                base.FlushHeightmapModification();
            }
        }

        private void PickRawDefaults(string path)
        {
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            int length = (int) stream.Length;
            stream.Close();
            this.m_TerrainSize = base.terrainData.size;
            if ((base.terrainData.heightmapWidth * base.terrainData.heightmapHeight) == length)
            {
                this.m_Width = base.terrainData.heightmapWidth;
                this.m_Height = base.terrainData.heightmapHeight;
                this.m_Depth = Depth.Bit8;
            }
            else if (((base.terrainData.heightmapWidth * base.terrainData.heightmapHeight) * 2) == length)
            {
                this.m_Width = base.terrainData.heightmapWidth;
                this.m_Height = base.terrainData.heightmapHeight;
                this.m_Depth = Depth.Bit16;
            }
            else
            {
                this.m_Depth = Depth.Bit16;
                int num2 = length / this.m_Depth;
                int num3 = Mathf.RoundToInt(Mathf.Sqrt((float) num2));
                int num4 = Mathf.RoundToInt(Mathf.Sqrt((float) num2));
                if (((num3 * num4) * this.m_Depth) == length)
                {
                    this.m_Width = num3;
                    this.m_Height = num4;
                }
                else
                {
                    this.m_Depth = Depth.Bit8;
                    num2 = length / this.m_Depth;
                    num3 = Mathf.RoundToInt(Mathf.Sqrt((float) num2));
                    num4 = Mathf.RoundToInt(Mathf.Sqrt((float) num2));
                    if (((num3 * num4) * this.m_Depth) == length)
                    {
                        this.m_Width = num3;
                        this.m_Height = num4;
                    }
                    else
                    {
                        this.m_Depth = Depth.Bit16;
                    }
                }
            }
        }

        private void ReadRaw(string path)
        {
            byte[] buffer;
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                buffer = reader.ReadBytes((this.m_Width * this.m_Height) * this.m_Depth);
                reader.Close();
            }
            int heightmapWidth = base.terrainData.heightmapWidth;
            int heightmapHeight = base.terrainData.heightmapHeight;
            float[,] heights = new float[heightmapHeight, heightmapWidth];
            if (this.m_Depth == Depth.Bit16)
            {
                float num3 = 1.525879E-05f;
                for (int i = 0; i < heightmapHeight; i++)
                {
                    for (int j = 0; j < heightmapWidth; j++)
                    {
                        int num6 = Mathf.Clamp(j, 0, this.m_Width - 1) + (Mathf.Clamp(i, 0, this.m_Height - 1) * this.m_Width);
                        if ((this.m_ByteOrder == ByteOrder.Mac) == BitConverter.IsLittleEndian)
                        {
                            byte num7 = buffer[num6 * 2];
                            buffer[num6 * 2] = buffer[(num6 * 2) + 1];
                            buffer[(num6 * 2) + 1] = num7;
                        }
                        float num9 = BitConverter.ToUInt16(buffer, num6 * 2) * num3;
                        int num10 = !this.m_FlipVertically ? i : ((heightmapHeight - 1) - i);
                        heights[num10, j] = num9;
                    }
                }
            }
            else
            {
                float num11 = 0.00390625f;
                for (int k = 0; k < heightmapHeight; k++)
                {
                    for (int m = 0; m < heightmapWidth; m++)
                    {
                        int index = Mathf.Clamp(m, 0, this.m_Width - 1) + (Mathf.Clamp(k, 0, this.m_Height - 1) * this.m_Width);
                        byte num15 = buffer[index];
                        float num16 = num15 * num11;
                        int num17 = !this.m_FlipVertically ? k : ((heightmapHeight - 1) - k);
                        heights[num17, m] = num16;
                    }
                }
            }
            base.terrainData.SetHeights(0, 0, heights);
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

