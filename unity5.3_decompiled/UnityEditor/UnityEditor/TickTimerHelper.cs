namespace UnityEditor
{
    using System;

    internal class TickTimerHelper
    {
        private double m_Interval;
        private double m_NextTick;

        public TickTimerHelper(double intervalBetweenTicksInSeconds)
        {
            this.m_Interval = intervalBetweenTicksInSeconds;
        }

        public bool DoTick()
        {
            if (EditorApplication.timeSinceStartup > this.m_NextTick)
            {
                this.m_NextTick = EditorApplication.timeSinceStartup + this.m_Interval;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.m_NextTick = 0.0;
        }
    }
}

