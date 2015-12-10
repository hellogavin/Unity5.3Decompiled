namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class AnimationWindowClipPopup
    {
        [SerializeField]
        private int selectedIndex;
        [SerializeField]
        public AnimationWindowState state;

        private int ClipToIndex(AnimationClip clip)
        {
            if (this.state.activeRootGameObject != null)
            {
                int num = 0;
                foreach (AnimationClip clip2 in AnimationUtility.GetAnimationClips(this.state.activeRootGameObject))
                {
                    if (clip == clip2)
                    {
                        return num;
                    }
                    num++;
                }
            }
            return 0;
        }

        private string[] GetClipMenuContent()
        {
            List<string> list = new List<string>();
            list.AddRange(this.GetClipNames());
            if (this.creatingNewClipAllowed)
            {
                list.Add(string.Empty);
                list.Add(AnimationWindowStyles.createNewClip.text);
            }
            return list.ToArray();
        }

        private string[] GetClipNames()
        {
            AnimationClip[] animationClips = new AnimationClip[0];
            if (this.state.clipOnlyMode)
            {
                animationClips = new AnimationClip[] { this.state.activeAnimationClip };
            }
            else if (this.state.activeRootGameObject != null)
            {
                animationClips = AnimationUtility.GetAnimationClips(this.state.activeRootGameObject);
            }
            string[] strArray = new string[animationClips.Length];
            for (int i = 0; i < animationClips.Length; i++)
            {
                strArray[i] = CurveUtility.GetClipName(animationClips[i]);
            }
            return strArray;
        }

        private AnimationClip IndexToClip(int index)
        {
            if (this.state.activeRootGameObject != null)
            {
                AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.state.activeRootGameObject);
                if ((index >= 0) && (index < animationClips.Length))
                {
                    return AnimationUtility.GetAnimationClips(this.state.activeRootGameObject)[index];
                }
            }
            return null;
        }

        public void OnGUI()
        {
            string[] clipMenuContent = this.GetClipMenuContent();
            EditorGUI.BeginChangeCheck();
            this.selectedIndex = EditorGUILayout.Popup(this.ClipToIndex(this.state.activeAnimationClip), clipMenuContent, EditorStyles.toolbarPopup, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                if (clipMenuContent[this.selectedIndex] == AnimationWindowStyles.createNewClip.text)
                {
                    AnimationClip newClip = AnimationWindowUtility.CreateNewClip(this.state.activeRootGameObject.name);
                    if (newClip != null)
                    {
                        AnimationWindowUtility.AddClipToAnimationPlayerComponent(this.state.activeAnimationPlayer, newClip);
                        this.state.activeAnimationClip = newClip;
                    }
                }
                else
                {
                    this.state.activeAnimationClip = this.IndexToClip(this.selectedIndex);
                }
            }
        }

        public bool creatingNewClipAllowed
        {
            get
            {
                return ((this.state.activeRootGameObject != null) && this.state.animationIsEditable);
            }
        }
    }
}

