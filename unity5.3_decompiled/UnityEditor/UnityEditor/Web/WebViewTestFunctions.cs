namespace UnityEditor.Web
{
    using System;
    using System.Linq;
    using UnityEngine;

    internal class WebViewTestFunctions
    {
        public void AcceptBool(bool passedBool)
        {
            Debug.Log("A value was passed from JS: " + passedBool);
        }

        public void AcceptBoolArray(bool[] passedArray)
        {
            Debug.Log("An array was passed from the JS. Array elements were:");
            for (int i = 1; i <= passedArray.Length; i++)
            {
                Debug.Log(string.Concat(new object[] { "Element at index ", i, ": ", passedArray[i] }));
            }
        }

        public void AcceptInt(int passedInt)
        {
            Debug.Log("A value was passed from JS: " + passedInt);
        }

        public void AcceptIntArray(int[] passedArray)
        {
            Debug.Log("An array was passed from the JS. Array elements were:");
            for (int i = 0; i <= passedArray.Length; i++)
            {
                Debug.Log(string.Concat(new object[] { "Element at index ", i, ": ", passedArray[i] }));
            }
        }

        public void AcceptString(string passedString)
        {
            Debug.Log("A value was passed from JS: " + passedString);
        }

        public void AcceptStringArray(string[] passedArray)
        {
            Debug.Log("An array was passed from the JS. Array elements were:");
            for (int i = 0; i <= passedArray.Length; i++)
            {
                Debug.Log(string.Concat(new object[] { "Element at index ", i, ": ", passedArray[i] }));
            }
        }

        public void AcceptTestObject(TestObject passedObject)
        {
            Debug.Log("An object was passed from the JS. Properties were:");
            Debug.Log("StringProperty: " + passedObject.StringProperty);
            Debug.Log("NumberProperty: " + passedObject.NumberProperty);
            Debug.Log("BoolProperty: " + passedObject.BoolProperty);
        }

        private string APrivateMethod(string input)
        {
            return "This method is private and not for CEF";
        }

        public string[] ArrayReverse(string[] input)
        {
            return (string[]) input.Reverse<string>();
        }

        public void LogMessage(string message)
        {
            Debug.Log(message);
        }

        public bool ReturnBool()
        {
            return true;
        }

        public bool[] ReturnBoolArray()
        {
            bool[] flagArray1 = new bool[3];
            flagArray1[0] = true;
            flagArray1[2] = true;
            return flagArray1;
        }

        public int ReturnInt()
        {
            return 5;
        }

        public int[] ReturnNumberArray()
        {
            return new int[] { 1, 2, 3 };
        }

        public TestObject ReturnObject()
        {
            return new TestObject { NumberProperty = 5, StringProperty = "Five", BoolProperty = true };
        }

        public string ReturnString()
        {
            return "Five";
        }

        public string[] ReturnStringArray()
        {
            return new string[] { "One", "Two", "Three" };
        }

        public static void RunTestScript(string path)
        {
            string sourcesPath = "file:///" + path;
            WebViewEditorWindow.Create<WebViewTestFunctions>("Test Window", sourcesPath, 0, 0, 0, 0).OnBatchMode();
        }

        public void VoidMethod(string logMessage)
        {
            Debug.Log("A method was called from the CEF: " + logMessage);
        }
    }
}

