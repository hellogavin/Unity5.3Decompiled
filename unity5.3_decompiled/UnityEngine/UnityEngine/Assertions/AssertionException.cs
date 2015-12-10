namespace UnityEngine.Assertions
{
    using System;

    public class AssertionException : Exception
    {
        private string m_UserMessage;

        public AssertionException(string message, string userMessage) : base(message)
        {
            this.m_UserMessage = userMessage;
        }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (this.m_UserMessage != null)
                {
                    message = message + '\n' + this.m_UserMessage;
                }
                return message;
            }
        }
    }
}

