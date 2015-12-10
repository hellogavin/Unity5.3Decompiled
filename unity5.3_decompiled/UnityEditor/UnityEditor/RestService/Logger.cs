namespace UnityEditor.RestService
{
    using System;
    using UnityEngine;

    internal class Logger
    {
        public static void Log(Exception an_exception)
        {
            Debug.Log(an_exception.ToString());
        }

        public static void Log(string a_message)
        {
            Debug.Log(a_message);
        }
    }
}

