namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;

    internal class SimpleProfiler
    {
        private static Dictionary<string, int> m_Calls = new Dictionary<string, int>();
        private static Stack<string> m_Names = new Stack<string>();
        private static Stack<float> m_StartTime = new Stack<float>();
        private static Dictionary<string, float> m_Timers = new Dictionary<string, float>();

        [Conditional("SIMPLE_PROFILER")]
        public static void Begin(string label)
        {
            m_Names.Push(label);
            m_StartTime.Push(Time.realtimeSinceStartup);
        }

        [Conditional("SIMPLE_PROFILER")]
        public static void End()
        {
            string str2;
            string key = m_Names.Pop();
            float num = Time.realtimeSinceStartup - m_StartTime.Pop();
            if (m_Timers.ContainsKey(key))
            {
                Dictionary<string, float> dictionary;
                float num2 = dictionary[str2];
                (dictionary = m_Timers)[str2 = key] = num2 + num;
            }
            else
            {
                m_Timers[key] = num;
            }
            if (m_Calls.ContainsKey(key))
            {
                Dictionary<string, int> dictionary2;
                int num3 = dictionary2[str2];
                (dictionary2 = m_Calls)[str2 = key] = num3 + 1;
            }
            else
            {
                m_Calls[key] = 1;
            }
        }

        [Conditional("SIMPLE_PROFILER")]
        public static void PrintTimes()
        {
            string message = "Measured execution times:\n----------------------------\n";
            foreach (KeyValuePair<string, float> pair in m_Timers)
            {
                message = message + string.Format("{0,6:0.0} ms: {1} in {2} calls\n", pair.Value * 1000f, pair.Key, m_Calls[pair.Key]);
            }
            Debug.Log(message);
            m_Names.Clear();
            m_StartTime.Clear();
            m_Timers.Clear();
            m_Calls.Clear();
        }
    }
}

