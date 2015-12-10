namespace UnityEditorInternal
{
    using System;

    internal class AudioProfilerInfoWrapper
    {
        public bool addToRoot;
        public string assetName;
        public AudioProfilerInfo info;
        public string objectName;

        public AudioProfilerInfoWrapper(AudioProfilerInfo info, string assetName, string objectName, bool addToRoot)
        {
            this.info = info;
            this.assetName = assetName;
            this.objectName = objectName;
            this.addToRoot = addToRoot;
        }
    }
}

