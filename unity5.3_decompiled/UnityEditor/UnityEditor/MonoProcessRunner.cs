namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;

    internal class MonoProcessRunner
    {
        public StringBuilder Error = new StringBuilder(string.Empty);
        public StringBuilder Output = new StringBuilder(string.Empty);

        private void ReadErrors(object process)
        {
            Process process2 = process as Process;
            try
            {
                using (StreamReader reader = process2.StandardError)
                {
                    this.Error.Append(reader.ReadToEnd());
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void ReadOutput(object process)
        {
            Process process2 = process as Process;
            try
            {
                using (StreamReader reader = process2.StandardOutput)
                {
                    this.Output.Append(reader.ReadToEnd());
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        public bool Run(Process process)
        {
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            Thread thread = new Thread(new ParameterizedThreadStart(this.ReadOutput));
            Thread thread2 = new Thread(new ParameterizedThreadStart(this.ReadErrors));
            process.Start();
            thread.Start(process);
            thread2.Start(process);
            bool flag = process.WaitForExit(0x927c0);
            DateTime now = DateTime.Now;
        Label_006E:
            if (thread.IsAlive || thread2.IsAlive)
            {
                TimeSpan span = (TimeSpan) (DateTime.Now - now);
                if (span.TotalMilliseconds < 5.0)
                {
                    Thread.Sleep(0);
                    goto Label_006E;
                }
            }
            if (thread.IsAlive)
            {
                thread.Abort();
            }
            if (thread2.IsAlive)
            {
                thread2.Abort();
            }
            thread.Join();
            thread2.Join();
            return flag;
        }
    }
}

