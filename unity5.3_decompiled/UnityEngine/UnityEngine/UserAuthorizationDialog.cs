namespace UnityEngine
{
    using System;

    [AddComponentMenu("")]
    internal class UserAuthorizationDialog : MonoBehaviour
    {
        private const int height = 0x9b;
        private Texture warningIcon;
        private const int width = 0x181;
        private Rect windowRect;

        private void DoUserAuthorizationDialog(int windowID)
        {
            UserAuthorization userAuthorizationRequestMode = Application.GetUserAuthorizationRequestMode();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.7f);
            GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.Label(this.warningIcon, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            switch (userAuthorizationRequestMode)
            {
                case (UserAuthorization.Microphone | UserAuthorization.WebCam):
                    GUILayout.Label("The content on this site would like to use your\ncomputer's web camera and microphone.\nThese images and sounds may be recorded.", new GUILayoutOption[0]);
                    break;

                case UserAuthorization.WebCam:
                    GUILayout.Label("The content on this site would like to use\nyour computer's web camera. The images\nfrom your web camera may be recorded.", new GUILayoutOption[0]);
                    break;

                case UserAuthorization.Microphone:
                    GUILayout.Label("The content on this site would like to use\nyour computer's microphone. The sounds\nfrom your microphone may be recorded.", new GUILayoutOption[0]);
                    break;

                default:
                    return;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("Deny", new GUILayoutOption[0]))
            {
                Application.ReplyToUserAuthorizationRequest(false);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Always Allow for this Site", new GUILayoutOption[0]))
            {
                Application.ReplyToUserAuthorizationRequest(true, true);
            }
            GUILayout.Space(5f);
            if (GUILayout.Button("Allow", new GUILayoutOption[0]))
            {
                Application.ReplyToUserAuthorizationRequest(true);
            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        private void OnGUI()
        {
            GUISkin skin = GUI.skin;
            GUISkin skin2 = ScriptableObject.CreateInstance("GUISkin") as GUISkin;
            skin2.box.normal.background = (Texture2D) Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/box.png");
            skin2.box.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            skin2.box.padding.left = 6;
            skin2.box.padding.right = 6;
            skin2.box.padding.top = 4;
            skin2.box.padding.bottom = 4;
            skin2.box.border.left = 6;
            skin2.box.border.right = 6;
            skin2.box.border.top = 6;
            skin2.box.border.bottom = 6;
            skin2.box.margin.left = 4;
            skin2.box.margin.right = 4;
            skin2.box.margin.top = 4;
            skin2.box.margin.bottom = 4;
            skin2.button.normal.background = (Texture2D) Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/button.png");
            skin2.button.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            skin2.button.hover.background = (Texture2D) Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/button hover.png");
            skin2.button.hover.textColor = Color.white;
            skin2.button.active.background = (Texture2D) Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/button active.png");
            skin2.button.active.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            skin2.button.border.left = 6;
            skin2.button.border.right = 6;
            skin2.button.border.top = 6;
            skin2.button.border.bottom = 6;
            skin2.button.padding.left = 8;
            skin2.button.padding.right = 8;
            skin2.button.padding.top = 4;
            skin2.button.padding.bottom = 4;
            skin2.button.margin.left = 4;
            skin2.button.margin.right = 4;
            skin2.button.margin.top = 4;
            skin2.button.margin.bottom = 4;
            skin2.label.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            skin2.label.padding.left = 6;
            skin2.label.padding.right = 6;
            skin2.label.padding.top = 4;
            skin2.label.padding.bottom = 4;
            skin2.label.margin.left = 4;
            skin2.label.margin.right = 4;
            skin2.label.margin.top = 4;
            skin2.label.margin.bottom = 4;
            skin2.label.alignment = TextAnchor.UpperLeft;
            skin2.window.normal.background = (Texture2D) Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/window.png");
            skin2.window.normal.textColor = Color.white;
            skin2.window.border.left = 8;
            skin2.window.border.right = 8;
            skin2.window.border.top = 0x12;
            skin2.window.border.bottom = 8;
            skin2.window.padding.left = 8;
            skin2.window.padding.right = 8;
            skin2.window.padding.top = 20;
            skin2.window.padding.bottom = 5;
            skin2.window.alignment = TextAnchor.UpperCenter;
            skin2.window.contentOffset = new Vector2(0f, -18f);
            GUI.skin = skin2;
            this.windowRect = GUI.Window(0, this.windowRect, new GUI.WindowFunction(this.DoUserAuthorizationDialog), "Unity Web Player Authorization Request");
            GUI.skin = skin;
        }

        private void Start()
        {
            this.warningIcon = Resources.GetBuiltinResource(typeof(Texture2D), "WarningSign.psd") as Texture2D;
            if ((Screen.width < 0x181) || (Screen.height < 0x9b))
            {
                Debug.LogError("Screen is to small to display authorization dialog. Authorization denied.");
                Application.ReplyToUserAuthorizationRequest(false);
            }
            this.windowRect = new Rect((float) ((Screen.width / 2) - 0xc0), (float) ((Screen.height / 2) - 0x4d), 385f, 155f);
        }
    }
}

