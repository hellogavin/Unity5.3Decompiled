namespace UnityEditor.ProjectWindowCallback
{
    using System;
    using UnityEditor;
    using UnityEditor.Audio;
    using UnityEditorInternal;

    internal class DoCreateAudioMixer : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            int num;
            AudioMixerController o = AudioMixerController.CreateMixerControllerAtPath(pathName);
            if (!string.IsNullOrEmpty(resourceFile) && int.TryParse(resourceFile, out num))
            {
                AudioMixerGroupController objectFromInstanceID = InternalEditorUtility.GetObjectFromInstanceID(num) as AudioMixerGroupController;
                if (objectFromInstanceID != null)
                {
                    o.outputAudioMixerGroup = objectFromInstanceID;
                }
            }
            ProjectWindowUtil.ShowCreatedAsset(o);
        }
    }
}

