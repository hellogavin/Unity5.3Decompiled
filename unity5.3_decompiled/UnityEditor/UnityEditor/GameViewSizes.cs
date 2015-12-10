namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    [FilePath("GameViewSizes.asset", FilePathAttribute.Location.PreferencesFolder)]
    internal class GameViewSizes : ScriptableSingleton<GameViewSizes>
    {
        [CompilerGenerated]
        private static Action <>f__am$cache11;
        [SerializeField]
        private GameViewSizeGroup m_Android = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_BB10 = new GameViewSizeGroup();
        [NonSerialized]
        private int m_ChangeID;
        [SerializeField]
        private GameViewSizeGroup m_iOS = new GameViewSizeGroup();
        [NonSerialized]
        private Vector2 m_LastRemoteScreenSize = new Vector2(-1f, -1f);
        [NonSerialized]
        private Vector2 m_LastStandaloneScreenSize = new Vector2(-1f, -1f);
        [NonSerialized]
        private Vector2 m_LastWebPlayerScreenSize = new Vector2(-1f, -1f);
        [SerializeField]
        private GameViewSizeGroup m_Nintendo3DS = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_PS3 = new GameViewSizeGroup();
        [NonSerialized]
        private GameViewSize m_Remote;
        [SerializeField]
        private GameViewSizeGroup m_Standalone = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_Tizen = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_WebPlayer = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_WiiU = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_WP8 = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_XBox360 = new GameViewSizeGroup();
        [NonSerialized]
        private static GameViewSizeGroupType s_GameViewSizeGroupType;

        static GameViewSizes()
        {
            RefreshGameViewSizeGroupType();
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = new Action(GameViewSizes.<GameViewSizes>m__FF);
            }
            EditorUserBuildSettings.activeBuildTargetChanged = (Action) Delegate.Combine(EditorUserBuildSettings.activeBuildTargetChanged, <>f__am$cache11);
        }

        [CompilerGenerated]
        private static void <GameViewSizes>m__FF()
        {
            RefreshGameViewSizeGroupType();
        }

        public static GameViewSizeGroupType BuildTargetGroupToGameViewSizeGroup(BuildTargetGroup buildTargetGroup)
        {
            switch (buildTargetGroup)
            {
                case BuildTargetGroup.Standalone:
                    return GameViewSizeGroupType.Standalone;

                case BuildTargetGroup.WebPlayer:
                    return GameViewSizeGroupType.WebPlayer;

                case BuildTargetGroup.iPhone:
                    return GameViewSizeGroupType.iOS;

                case BuildTargetGroup.PS3:
                    return GameViewSizeGroupType.PS3;

                case BuildTargetGroup.XBOX360:
                    return GameViewSizeGroupType.Xbox360;

                case BuildTargetGroup.Android:
                    return GameViewSizeGroupType.Android;

                case BuildTargetGroup.WP8:
                    return GameViewSizeGroupType.WP8;

                case BuildTargetGroup.BlackBerry:
                    return GameViewSizeGroupType.BB10;

                case BuildTargetGroup.Tizen:
                    return GameViewSizeGroupType.Tizen;

                case BuildTargetGroup.Nintendo3DS:
                    return GameViewSizeGroupType.Nintendo3DS;

                case BuildTargetGroup.WiiU:
                    return GameViewSizeGroupType.WiiU;
            }
            return GameViewSizeGroupType.Standalone;
        }

        public void Changed()
        {
            this.m_ChangeID++;
        }

        public int GetChangeID()
        {
            return this.m_ChangeID;
        }

        public static Rect GetConstrainedRect(Rect startRect, GameViewSizeGroupType groupType, int gameViewSizeIndex, out bool fitsInsideRect)
        {
            bool flag;
            fitsInsideRect = true;
            Rect rect = startRect;
            GameViewSize gameViewSize = ScriptableSingleton<GameViewSizes>.instance.GetGroup(groupType).GetGameViewSize(gameViewSizeIndex);
            RefreshDerivedGameViewSize(groupType, gameViewSizeIndex, gameViewSize);
            if (gameViewSize.isFreeAspectRatio)
            {
                return startRect;
            }
            float aspectRatio = 0f;
            GameViewSizeType sizeType = gameViewSize.sizeType;
            if (sizeType != GameViewSizeType.AspectRatio)
            {
                if (sizeType != GameViewSizeType.FixedResolution)
                {
                    Debug.LogError("Unhandled enum");
                    return startRect;
                }
                if ((gameViewSize.height > startRect.height) || (gameViewSize.width > startRect.width))
                {
                    aspectRatio = gameViewSize.aspectRatio;
                    flag = true;
                    fitsInsideRect = false;
                }
                else
                {
                    rect.height = gameViewSize.height;
                    rect.width = gameViewSize.width;
                    flag = false;
                }
            }
            else
            {
                aspectRatio = gameViewSize.aspectRatio;
                flag = true;
            }
            if (flag)
            {
                rect.height = ((rect.width / aspectRatio) <= startRect.height) ? (rect.width / aspectRatio) : startRect.height;
                rect.width = rect.height * aspectRatio;
            }
            rect.height = Mathf.Clamp(rect.height, 0f, startRect.height);
            rect.width = Mathf.Clamp(rect.width, 0f, startRect.width);
            rect.y = ((startRect.height * 0.5f) - (rect.height * 0.5f)) + startRect.y;
            rect.x = ((startRect.width * 0.5f) - (rect.width * 0.5f)) + startRect.x;
            rect.width = Mathf.Floor(rect.width + 0.5f);
            rect.height = Mathf.Floor(rect.height + 0.5f);
            rect.x = Mathf.Floor(rect.x + 0.5f);
            rect.y = Mathf.Floor(rect.y + 0.5f);
            return rect;
        }

        public int GetDefaultStandaloneIndex()
        {
            return (this.m_Standalone.GetBuiltinCount() - 1);
        }

        public int GetDefaultWebPlayerIndex()
        {
            return (this.m_WebPlayer.GetBuiltinCount() - 1);
        }

        public GameViewSizeGroup GetGroup(GameViewSizeGroupType gameViewSizeGroupType)
        {
            this.InitBuiltinGroups();
            switch (gameViewSizeGroupType)
            {
                case GameViewSizeGroupType.Standalone:
                    return this.m_Standalone;

                case GameViewSizeGroupType.WebPlayer:
                    return this.m_WebPlayer;

                case GameViewSizeGroupType.iOS:
                    return this.m_iOS;

                case GameViewSizeGroupType.Android:
                    return this.m_Android;

                case GameViewSizeGroupType.PS3:
                    return this.m_PS3;

                case GameViewSizeGroupType.Xbox360:
                    return this.m_XBox360;

                case GameViewSizeGroupType.BB10:
                    return this.m_BB10;

                case GameViewSizeGroupType.WiiU:
                    return this.m_WiiU;

                case GameViewSizeGroupType.Tizen:
                    return this.m_Tizen;

                case GameViewSizeGroupType.WP8:
                    return this.m_WP8;

                case GameViewSizeGroupType.Nintendo3DS:
                    return this.m_Nintendo3DS;
            }
            Debug.LogError("Unhandled group enum! " + gameViewSizeGroupType);
            return this.m_Standalone;
        }

        private void InitBuiltinGroups()
        {
            if (this.m_Standalone.GetBuiltinCount() <= 0)
            {
                this.m_Remote = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Remote (Not Connected)");
                GameViewSize size = new GameViewSize(GameViewSizeType.AspectRatio, 0, 0, "Free Aspect");
                GameViewSize size2 = new GameViewSize(GameViewSizeType.AspectRatio, 5, 4, string.Empty);
                GameViewSize size3 = new GameViewSize(GameViewSizeType.AspectRatio, 4, 3, string.Empty);
                GameViewSize size4 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, string.Empty);
                GameViewSize size5 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 10, string.Empty);
                GameViewSize size6 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 9, string.Empty);
                GameViewSize size7 = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Standalone");
                GameViewSize size8 = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Web");
                GameViewSize size9 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 480, "iPhone Tall");
                GameViewSize size10 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 320, "iPhone Wide");
                GameViewSize size11 = new GameViewSize(GameViewSizeType.FixedResolution, 640, 960, "iPhone 4 Tall");
                GameViewSize size12 = new GameViewSize(GameViewSizeType.FixedResolution, 960, 640, "iPhone 4 Wide");
                GameViewSize size13 = new GameViewSize(GameViewSizeType.FixedResolution, 0x300, 0x400, "iPad Tall");
                GameViewSize size14 = new GameViewSize(GameViewSizeType.FixedResolution, 0x400, 0x300, "iPad Wide");
                GameViewSize size15 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 0x10, "iPhone 5 Tall");
                GameViewSize size16 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 9, "iPhone 5 Wide");
                GameViewSize size17 = new GameViewSize(GameViewSizeType.AspectRatio, 2, 3, "iPhone Tall");
                GameViewSize size18 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "iPhone Wide");
                GameViewSize size19 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 4, "iPad Tall");
                GameViewSize size20 = new GameViewSize(GameViewSizeType.AspectRatio, 4, 3, "iPad Wide");
                GameViewSize size21 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 480, "HVGA Portrait");
                GameViewSize size22 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 320, "HVGA Landscape");
                GameViewSize size23 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 800, "WVGA Portrait");
                GameViewSize size24 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 480, "WVGA Landscape");
                GameViewSize size25 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 0x356, "FWVGA Portrait");
                GameViewSize size26 = new GameViewSize(GameViewSizeType.FixedResolution, 0x356, 480, "FWVGA Landscape");
                GameViewSize size27 = new GameViewSize(GameViewSizeType.FixedResolution, 600, 0x400, "WSVGA Portrait");
                GameViewSize size28 = new GameViewSize(GameViewSizeType.FixedResolution, 0x400, 600, "WSVGA Landscape");
                GameViewSize size29 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 0x500, "WXGA Portrait");
                GameViewSize size30 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 800, "WXGA Landscape");
                GameViewSize size31 = new GameViewSize(GameViewSizeType.AspectRatio, 2, 3, "3:2 Portrait");
                GameViewSize size32 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "3:2 Landscape");
                GameViewSize size33 = new GameViewSize(GameViewSizeType.AspectRatio, 10, 0x10, "16:10 Portrait");
                GameViewSize size34 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 10, "16:10 Landscape");
                GameViewSize size35 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 720, "720p (16:9)");
                GameViewSize size36 = new GameViewSize(GameViewSizeType.FixedResolution, 0x780, 0x438, "1080p (16:9)");
                GameViewSize size37 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 720, "720p (16:9)");
                GameViewSize size38 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 0x240, "576p (4:3)");
                GameViewSize size39 = new GameViewSize(GameViewSizeType.FixedResolution, 0x400, 0x240, "576p (16:9)");
                GameViewSize size40 = new GameViewSize(GameViewSizeType.FixedResolution, 640, 480, "480p (4:3)");
                GameViewSize size41 = new GameViewSize(GameViewSizeType.FixedResolution, 0x355, 480, "480p (16:9)");
                GameViewSize size42 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 0x500, "Touch Phone Portrait");
                GameViewSize size43 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 720, "Touch Phone Landscape");
                GameViewSize size44 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 720, "Keyboard Phone");
                GameViewSize size45 = new GameViewSize(GameViewSizeType.FixedResolution, 600, 0x400, "Playbook Portrait");
                GameViewSize size46 = new GameViewSize(GameViewSizeType.FixedResolution, 0x400, 600, "Playbook Landscape");
                GameViewSize size47 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 0x10, "9:16 Portrait");
                GameViewSize size48 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 9, "16:9 Landscape");
                GameViewSize size49 = new GameViewSize(GameViewSizeType.AspectRatio, 1, 1, "1:1");
                GameViewSize size50 = new GameViewSize(GameViewSizeType.FixedResolution, 0x780, 0x438, "1080p (16:9)");
                GameViewSize size51 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 720, "720p (16:9)");
                GameViewSize size52 = new GameViewSize(GameViewSizeType.FixedResolution, 0x356, 480, "GamePad 480p (16:9)");
                GameViewSize size53 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 720, "16:9 Landscape");
                GameViewSize size54 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 0x500, "9:16 Portrait");
                GameViewSize size55 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 800, "WVGA Portrait");
                GameViewSize size56 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 480, "WVGA Landscape");
                GameViewSize size57 = new GameViewSize(GameViewSizeType.FixedResolution, 0x300, 0x500, "WXGA Portrait");
                GameViewSize size58 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 0x300, "WXGA Landscape");
                GameViewSize size59 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 0x500, "720p Portrait");
                GameViewSize size60 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 720, "720p Landscape");
                GameViewSize size61 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 15, "WVGA Portrait");
                GameViewSize size62 = new GameViewSize(GameViewSizeType.AspectRatio, 15, 9, "WVGA Landscape");
                GameViewSize size63 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 15, "WXGA Portrait");
                GameViewSize size64 = new GameViewSize(GameViewSizeType.AspectRatio, 15, 9, "WXGA Landscape");
                GameViewSize size65 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 0x10, "720p Portrait");
                GameViewSize size66 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 9, "720p Landscape");
                GameViewSize size67 = new GameViewSize(GameViewSizeType.FixedResolution, 400, 240, "Top Screen");
                GameViewSize size68 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 240, "Bottom Screen");
                GameViewSize[] sizes = new GameViewSize[] { size, size55, size61, size56, size62, size57, size63, size58, size64, size59, size65, size60, size66 };
                this.m_WP8.AddBuiltinSizes(sizes);
                GameViewSize[] sizeArray2 = new GameViewSize[] { size, size2, size3, size4, size5, size6, size7 };
                this.m_Standalone.AddBuiltinSizes(sizeArray2);
                GameViewSize[] sizeArray3 = new GameViewSize[] { size, size2, size3, size4, size5, size6, size8 };
                this.m_WebPlayer.AddBuiltinSizes(sizeArray3);
                GameViewSize[] sizeArray4 = new GameViewSize[] { size, size3, size6, size5, size36, size37, size38, size39, size40, size41 };
                this.m_PS3.AddBuiltinSizes(sizeArray4);
                GameViewSize[] sizeArray5 = new GameViewSize[] { size, size3, size6, size5, size35 };
                this.m_XBox360.AddBuiltinSizes(sizeArray5);
                GameViewSize[] sizeArray6 = new GameViewSize[] { size, size3, size6, size50, size51, size52 };
                this.m_WiiU.AddBuiltinSizes(sizeArray6);
                GameViewSize[] sizeArray7 = new GameViewSize[] { size, size9, size10, size11, size12, size13, size14, size15, size16, size17, size18, size19, size20 };
                this.m_iOS.AddBuiltinSizes(sizeArray7);
                GameViewSize[] sizeArray8 = new GameViewSize[] { size, this.m_Remote, size21, size22, size23, size24, size25, size26, size27, size28, size29, size30, size31, size32, size33, size34 };
                this.m_Android.AddBuiltinSizes(sizeArray8);
                GameViewSize[] sizeArray9 = new GameViewSize[] { size, size42, size43, size44, size45, size46, size47, size48, size49 };
                this.m_BB10.AddBuiltinSizes(sizeArray9);
                GameViewSize[] sizeArray10 = new GameViewSize[] { size, size53, size54 };
                this.m_Tizen.AddBuiltinSizes(sizeArray10);
                GameViewSize[] sizeArray11 = new GameViewSize[] { size, size67, size68 };
                this.m_Nintendo3DS.AddBuiltinSizes(sizeArray11);
            }
        }

        public bool IsDefaultStandaloneScreenSize(GameViewSizeGroupType gameViewSizeGroupType, int index)
        {
            return ((gameViewSizeGroupType == GameViewSizeGroupType.Standalone) && (this.GetDefaultStandaloneIndex() == index));
        }

        public bool IsDefaultWebPlayerScreenSize(GameViewSizeGroupType gameViewSizeGroupType, int index)
        {
            return ((gameViewSizeGroupType == GameViewSizeGroupType.WebPlayer) && (this.GetDefaultWebPlayerIndex() == index));
        }

        public bool IsRemoteScreenSize(GameViewSizeGroupType gameViewSizeGroupType, int index)
        {
            return (this.GetGroup(gameViewSizeGroupType).IndexOf(this.m_Remote) == index);
        }

        private static void RefreshDerivedGameViewSize(GameViewSizeGroupType groupType, int gameViewSizeIndex, GameViewSize gameViewSize)
        {
            if (ScriptableSingleton<GameViewSizes>.instance.IsDefaultStandaloneScreenSize(groupType, gameViewSizeIndex))
            {
                gameViewSize.width = (int) InternalEditorUtility.defaultScreenWidth;
                gameViewSize.height = (int) InternalEditorUtility.defaultScreenHeight;
            }
            else if (ScriptableSingleton<GameViewSizes>.instance.IsDefaultWebPlayerScreenSize(groupType, gameViewSizeIndex))
            {
                gameViewSize.width = (int) InternalEditorUtility.defaultWebScreenWidth;
                gameViewSize.height = (int) InternalEditorUtility.defaultWebScreenHeight;
            }
            else if (ScriptableSingleton<GameViewSizes>.instance.IsRemoteScreenSize(groupType, gameViewSizeIndex))
            {
                if ((InternalEditorUtility.remoteScreenWidth <= 0f) || (InternalEditorUtility.remoteScreenHeight <= 0f))
                {
                    gameViewSize.sizeType = GameViewSizeType.AspectRatio;
                    int num = 0;
                    gameViewSize.height = num;
                    gameViewSize.width = num;
                }
                else
                {
                    gameViewSize.sizeType = GameViewSizeType.FixedResolution;
                    gameViewSize.width = (int) InternalEditorUtility.remoteScreenWidth;
                    gameViewSize.height = (int) InternalEditorUtility.remoteScreenHeight;
                }
            }
        }

        private static void RefreshGameViewSizeGroupType()
        {
            s_GameViewSizeGroupType = BuildTargetGroupToGameViewSizeGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
        }

        public void RefreshRemoteScreenSize(int width, int height)
        {
            this.m_Remote.width = width;
            this.m_Remote.height = height;
            if ((width > 0) && (height > 0))
            {
                this.m_Remote.baseText = "Remote";
            }
            else
            {
                this.m_Remote.baseText = "Remote (Not Connected)";
            }
            this.Changed();
        }

        public void RefreshStandaloneAndWebplayerDefaultSizes()
        {
            if ((InternalEditorUtility.defaultScreenWidth != this.m_LastStandaloneScreenSize.x) || (InternalEditorUtility.defaultScreenHeight != this.m_LastStandaloneScreenSize.y))
            {
                this.m_LastStandaloneScreenSize = new Vector2(InternalEditorUtility.defaultScreenWidth, InternalEditorUtility.defaultScreenHeight);
                this.RefreshStandaloneDefaultScreenSize((int) this.m_LastStandaloneScreenSize.x, (int) this.m_LastStandaloneScreenSize.y);
            }
            if ((InternalEditorUtility.defaultWebScreenWidth != this.m_LastWebPlayerScreenSize.x) || (InternalEditorUtility.defaultWebScreenHeight != this.m_LastWebPlayerScreenSize.y))
            {
                this.m_LastWebPlayerScreenSize = new Vector2(InternalEditorUtility.defaultWebScreenWidth, InternalEditorUtility.defaultWebScreenHeight);
                this.RefreshWebPlayerDefaultScreenSize((int) this.m_LastWebPlayerScreenSize.x, (int) this.m_LastWebPlayerScreenSize.y);
            }
            if ((InternalEditorUtility.remoteScreenWidth != this.m_LastRemoteScreenSize.x) || (InternalEditorUtility.remoteScreenHeight != this.m_LastRemoteScreenSize.y))
            {
                this.m_LastRemoteScreenSize = new Vector2(InternalEditorUtility.remoteScreenWidth, InternalEditorUtility.remoteScreenHeight);
                this.RefreshRemoteScreenSize((int) this.m_LastRemoteScreenSize.x, (int) this.m_LastRemoteScreenSize.y);
            }
        }

        public void RefreshStandaloneDefaultScreenSize(int width, int height)
        {
            GameViewSize gameViewSize = this.m_Standalone.GetGameViewSize(this.GetDefaultStandaloneIndex());
            gameViewSize.height = height;
            gameViewSize.width = width;
            this.Changed();
        }

        public void RefreshWebPlayerDefaultScreenSize(int width, int height)
        {
            GameViewSize gameViewSize = this.m_WebPlayer.GetGameViewSize(this.GetDefaultWebPlayerIndex());
            gameViewSize.height = height;
            gameViewSize.width = width;
            this.Changed();
        }

        public void SaveToHDD()
        {
            bool saveAsText = true;
            this.Save(saveAsText);
        }

        public GameViewSizeGroup currentGroup
        {
            get
            {
                return this.GetGroup(s_GameViewSizeGroupType);
            }
        }

        public GameViewSizeGroupType currentGroupType
        {
            get
            {
                return s_GameViewSizeGroupType;
            }
        }
    }
}

