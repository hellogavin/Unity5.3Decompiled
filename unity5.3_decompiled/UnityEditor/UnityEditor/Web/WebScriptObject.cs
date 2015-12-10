namespace UnityEditor.Web
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class WebScriptObject : ScriptableObject
    {
        private WebView m_WebView = null;

        private WebScriptObject()
        {
        }

        public bool processMessage(string jsonRequest, WebViewV8CallbackCSharp callback)
        {
            return this.ProcessMessage(jsonRequest, callback);
        }

        public bool ProcessMessage(string jsonRequest, WebViewV8CallbackCSharp callback)
        {
            <ProcessMessage>c__AnonStorey9B storeyb = new <ProcessMessage>c__AnonStorey9B {
                callback = callback
            };
            return ((this.m_WebView != null) && JSProxyMgr.GetInstance().DoMessage(jsonRequest, new JSProxyMgr.ExecCallback(storeyb.<>m__1C9), this.m_WebView));
        }

        public WebView webView
        {
            get
            {
                return this.m_WebView;
            }
            set
            {
                this.m_WebView = value;
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessMessage>c__AnonStorey9B
        {
            internal WebViewV8CallbackCSharp callback;

            internal void <>m__1C9(object result)
            {
                string str = JSProxyMgr.GetInstance().Stringify(result);
                this.callback.Callback(str);
            }
        }
    }
}

