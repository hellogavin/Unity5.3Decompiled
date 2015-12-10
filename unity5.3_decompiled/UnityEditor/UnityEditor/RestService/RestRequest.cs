namespace UnityEditor.RestService
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    internal class RestRequest
    {
        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            WebResponse response = ((WebRequest) asynchronousResult.AsyncState).EndGetResponse(asynchronousResult);
            try
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                reader.ReadToEnd();
                reader.Close();
                responseStream.Close();
            }
            finally
            {
                response.Close();
            }
        }

        public static bool Send(string endpoint, string payload, int timeout)
        {
            if (ScriptEditorSettings.ServerURL == null)
            {
                return false;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            WebRequest state = WebRequest.Create(ScriptEditorSettings.ServerURL + endpoint);
            state.Timeout = timeout;
            state.Method = "POST";
            state.ContentType = "application/json";
            state.ContentLength = bytes.Length;
            try
            {
                Stream requestStream = state.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return false;
            }
            try
            {
                state.BeginGetResponse(new AsyncCallback(RestRequest.GetResponseCallback), state);
            }
            catch (Exception exception2)
            {
                Logger.Log(exception2);
                return false;
            }
            return true;
        }
    }
}

