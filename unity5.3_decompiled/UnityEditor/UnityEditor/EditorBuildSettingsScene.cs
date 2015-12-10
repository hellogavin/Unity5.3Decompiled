namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class EditorBuildSettingsScene : IComparable
    {
        private int m_Enabled;
        private string m_Path;
        public EditorBuildSettingsScene(string path, bool enable)
        {
            this.m_Path = path.Replace(@"\", "/");
            this.enabled = enable;
        }

        public EditorBuildSettingsScene()
        {
        }

        public int CompareTo(object obj)
        {
            if (!(obj is EditorBuildSettingsScene))
            {
                throw new ArgumentException("object is not a EditorBuildSettingsScene");
            }
            EditorBuildSettingsScene scene = (EditorBuildSettingsScene) obj;
            return scene.m_Path.CompareTo(this.m_Path);
        }

        public bool enabled
        {
            get
            {
                return (this.m_Enabled != 0);
            }
            set
            {
                this.m_Enabled = !value ? 0 : 1;
            }
        }
        public string path
        {
            get
            {
                return this.m_Path;
            }
            set
            {
                this.m_Path = value.Replace(@"\", "/");
            }
        }
    }
}

