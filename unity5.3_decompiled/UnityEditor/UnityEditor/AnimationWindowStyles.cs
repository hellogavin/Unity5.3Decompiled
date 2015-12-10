namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class AnimationWindowStyles
    {
        public static GUIContent addEventContent = EditorGUIUtility.IconContent("Animation.AddEvent", "|Add Event.");
        public static GUIContent addKeyframeContent = EditorGUIUtility.IconContent("Animation.AddKeyframe", "|Add Keyframe.");
        public static GUIContent animationClip = EditorGUIUtility.TextContent("an Animation Clip");
        public static GUIContent animatorAndAnimationClip = EditorGUIUtility.TextContent("an Animator and an Animation Clip");
        public static GUIContent create = EditorGUIUtility.TextContent("Create");
        public static GUIContent createNewClip = EditorGUIUtility.TextContent("Create New Clip...");
        public static GUIStyle curveEditorBackground = "AnimationCurveEditorBackground";
        public static GUIContent curves = EditorGUIUtility.TextContent("Curves");
        public static GUIContent dopesheet = EditorGUIUtility.TextContent("Dopesheet");
        public static GUIStyle eventBackground = "AnimationEventBackground";
        public static GUIContent formatIsMissing = EditorGUIUtility.TextContent("To begin animating {0}, create {1}.");
        public static GUIStyle keyframeBackground = "AnimationKeyframeBackground";
        public static GUIStyle miniToolbar = new GUIStyle(EditorStyles.toolbar);
        public static GUIStyle miniToolbarButton = new GUIStyle(EditorStyles.toolbarButton);
        public static GUIContent nextKeyContent = EditorGUIUtility.IconContent("Animation.NextKey", "|Go to next key frame.");
        public static GUIContent noAnimatableObjectSelectedText = EditorGUIUtility.TextContent("No animatable object selected.");
        public static GUIContent playContent = EditorGUIUtility.IconContent("Animation.Play", "|Play the animation clip, looping in the shown range.");
        public static Texture2D pointIcon = EditorGUIUtility.LoadIcon("animationkeyframe");
        public static GUIContent prevKeyContent = EditorGUIUtility.IconContent("Animation.PrevKey", "|Go to previous key frame.");
        public static GUIContent recordContent = EditorGUIUtility.IconContent("Animation.Record", "|Are scene and inspector changes recorded into the animation curves?");
        public static GUIStyle rowEven = "AnimationRowOdd";
        public static GUIStyle rowOdd = "AnimationRowEven";
        public static GUIContent samples = EditorGUIUtility.TextContent("Samples");
        public static GUIStyle TimelineTick = "AnimationTimelineTick";
        public static GUIStyle toolbarLabel = new GUIStyle(EditorStyles.toolbarPopup);

        public static void Initialize()
        {
            toolbarLabel.normal.background = null;
            miniToolbarButton.padding.top = 0;
            miniToolbarButton.padding.bottom = 3;
        }
    }
}

