namespace UnityEditor.RestService
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditorInternal;

    internal class PlayModeRestHandler : Handler
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$mapA;

        internal string CurrentState()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return "stopped";
            }
            return (!EditorApplication.isPaused ? "playing" : "paused");
        }

        protected override JSONValue HandleGet(Request request, JSONValue payload)
        {
            JSONValue value2 = new JSONValue();
            value2["state"] = this.CurrentState();
            return value2;
        }

        protected override JSONValue HandlePost(Request request, JSONValue payload)
        {
            JSONValue value2;
            string str = payload.Get("action").AsString();
            string str2 = this.CurrentState();
            string key = str;
            if (key != null)
            {
                int num;
                if (<>f__switch$mapA == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
                    dictionary.Add("play", 0);
                    dictionary.Add("pause", 1);
                    dictionary.Add("stop", 2);
                    <>f__switch$mapA = dictionary;
                }
                if (<>f__switch$mapA.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            EditorApplication.isPlaying = true;
                            EditorApplication.isPaused = false;
                            goto Label_00E0;

                        case 1:
                            EditorApplication.isPaused = true;
                            goto Label_00E0;

                        case 2:
                            EditorApplication.isPlaying = false;
                            goto Label_00E0;
                    }
                }
            }
            RestRequestException exception = new RestRequestException {
                HttpStatusCode = HttpStatusCode.BadRequest,
                RestErrorString = "Invalid action: " + str
            };
            throw exception;
        Label_00E0:
            value2 = new JSONValue();
            value2["oldstate"] = str2;
            value2["newstate"] = this.CurrentState();
            return value2;
        }

        internal static void Register()
        {
            Router.RegisterHandler("/unity/playmode", new PlayModeRestHandler());
        }
    }
}

