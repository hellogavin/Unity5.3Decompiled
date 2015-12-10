namespace UnityEngine.Assertions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using UnityEngine;
    using UnityEngine.Assertions.Comparers;

    [DebuggerStepThrough]
    public static class Assert
    {
        public static bool raiseExceptions;
        internal const string UNITY_ASSERTIONS = "UNITY_ASSERTIONS";

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual)
        {
            AreEqual<float>(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual, float tolerance)
        {
            AreApproximatelyEqual(expected, actual, tolerance, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual, string message)
        {
            AreEqual<float>(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual, float tolerance, string message)
        {
            AreEqual<float>(expected, actual, message, new FloatComparer(tolerance));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual<T>(T expected, T actual)
        {
            AreEqual<T>(expected, actual, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            AreEqual<T>(expected, actual, message, EqualityComparer<T>.Default);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
        {
            if (!comparer.Equals(actual, expected))
            {
                Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, true), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotApproximatelyEqual(float expected, float actual)
        {
            AreNotEqual<float>(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance)
        {
            AreNotApproximatelyEqual(expected, actual, tolerance, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotApproximatelyEqual(float expected, float actual, string message)
        {
            AreNotEqual<float>(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance, string message)
        {
            AreNotEqual<float>(expected, actual, message, new FloatComparer(tolerance));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual<T>(T expected, T actual)
        {
            AreNotEqual<T>(expected, actual, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual<T>(T expected, T actual, string message)
        {
            AreNotEqual<T>(expected, actual, message, EqualityComparer<T>.Default);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
        {
            if (comparer.Equals(actual, expected))
            {
                Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, false), message);
            }
        }

        [Obsolete("Assert.Equals should not be used for Assertions", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static bool Equals(object obj1, object obj2)
        {
            throw new InvalidOperationException("Assert.Equals should not be used for Assertions");
        }

        private static void Fail(string message, string userMessage)
        {
            if (Debugger.IsAttached)
            {
                throw new AssertionException(message, userMessage);
            }
            if (raiseExceptions)
            {
                throw new AssertionException(message, userMessage);
            }
            if (message == null)
            {
                message = "Assertion has failed\n";
            }
            if (userMessage != null)
            {
                message = userMessage + '\n' + message;
            }
            Debug.LogAssertion(message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse(bool condition)
        {
            IsFalse(condition, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse(bool condition, string message)
        {
            if (condition)
            {
                Fail(AssertionMessageUtil.BooleanFailureMessage(false), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T value) where T: class
        {
            IsNotNull<T>(value, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T value, string message) where T: class
        {
            if (value == null)
            {
                Fail(AssertionMessageUtil.NullFailureMessage(value, false), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T value) where T: class
        {
            IsNull<T>(value, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T value, string message) where T: class
        {
            if (value != null)
            {
                Fail(AssertionMessageUtil.NullFailureMessage(value, true), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue(bool condition)
        {
            IsTrue(condition, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                Fail(AssertionMessageUtil.BooleanFailureMessage(true), message);
            }
        }

        [Obsolete("Assert.ReferenceEquals should not be used for Assertions", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static bool ReferenceEquals(object obj1, object obj2)
        {
            throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions");
        }
    }
}

