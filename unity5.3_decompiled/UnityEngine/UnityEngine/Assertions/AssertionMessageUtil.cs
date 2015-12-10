namespace UnityEngine.Assertions
{
    using System;
    using UnityEngine;

    internal class AssertionMessageUtil
    {
        private const string k_AssertionFailed = "Assertion failed.";
        private const string k_Expected = "Expected:";

        public static string BooleanFailureMessage(bool expected)
        {
            return GetMessage("Value was " + !expected, expected.ToString());
        }

        public static string GetEqualityMessage(object actual, object expected, bool expectEqual)
        {
            object[] args = new object[] { !expectEqual ? string.Empty : "not " };
            object[] objArray2 = new object[] { actual, expected, !expectEqual ? "!=" : "==" };
            return GetMessage(UnityString.Format("Values are {0}equal.", args), UnityString.Format("{0} {2} {1}", objArray2));
        }

        public static string GetMessage(string failureMessage)
        {
            object[] args = new object[] { "Assertion failed.", failureMessage };
            return UnityString.Format("{0} {1}", args);
        }

        public static string GetMessage(string failureMessage, string expected)
        {
            object[] args = new object[] { failureMessage, Environment.NewLine, "Expected:", expected };
            return GetMessage(UnityString.Format("{0}{1}{2} {3}", args));
        }

        public static string NullFailureMessage(object value, bool expectNull)
        {
            object[] args = new object[] { !expectNull ? string.Empty : "not " };
            object[] objArray2 = new object[] { !expectNull ? "not " : string.Empty };
            return GetMessage(UnityString.Format("Value was {0}Null", args), UnityString.Format("Value was {0}Null", objArray2));
        }
    }
}

