namespace UnityEditor.Web
{
    using System;

    internal class JspmResult
    {
        public long messageID;
        public int status;
        public double version;

        public JspmResult()
        {
            this.version = 1.0;
            this.messageID = -1L;
            this.status = 0;
        }

        public JspmResult(long messageID, int status)
        {
            this.version = 1.0;
            this.messageID = messageID;
            this.status = status;
        }
    }
}

