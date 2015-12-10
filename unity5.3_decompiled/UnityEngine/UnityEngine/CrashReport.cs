namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class CrashReport
    {
        private readonly string id;
        private static List<CrashReport> internalReports;
        private static object reportsLock = new object();
        public readonly string text;
        public readonly DateTime time;

        private CrashReport(string id, DateTime time, string text)
        {
            this.id = id;
            this.time = time;
            this.text = text;
        }

        private static int Compare(CrashReport c1, CrashReport c2)
        {
            long ticks = c1.time.Ticks;
            long num2 = c2.time.Ticks;
            if (ticks > num2)
            {
                return 1;
            }
            if (ticks < num2)
            {
                return -1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void GetReportData(string id, out double secondsSinceUnixEpoch, out string text);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string[] GetReports();
        private static void PopulateReports()
        {
            object reportsLock = CrashReport.reportsLock;
            lock (reportsLock)
            {
                if (internalReports == null)
                {
                    string[] reports = GetReports();
                    internalReports = new List<CrashReport>(reports.Length);
                    foreach (string str in reports)
                    {
                        double num2;
                        string str2;
                        GetReportData(str, out num2, out str2);
                        DateTime time = new DateTime(0x7b2, 1, 1).AddSeconds(num2);
                        internalReports.Add(new CrashReport(str, time, str2));
                    }
                    internalReports.Sort(new Comparison<CrashReport>(CrashReport.Compare));
                }
            }
        }

        public void Remove()
        {
            if (RemoveReport(this.id))
            {
                object reportsLock = CrashReport.reportsLock;
                lock (reportsLock)
                {
                    internalReports.Remove(this);
                }
            }
        }

        public static void RemoveAll()
        {
            foreach (CrashReport report in reports)
            {
                report.Remove();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool RemoveReport(string id);

        public static CrashReport lastReport
        {
            get
            {
                PopulateReports();
                object reportsLock = CrashReport.reportsLock;
                lock (reportsLock)
                {
                    if (internalReports.Count > 0)
                    {
                        return internalReports[internalReports.Count - 1];
                    }
                }
                return null;
            }
        }

        public static CrashReport[] reports
        {
            get
            {
                PopulateReports();
                object reportsLock = CrashReport.reportsLock;
                lock (reportsLock)
                {
                    return internalReports.ToArray();
                }
            }
        }
    }
}

