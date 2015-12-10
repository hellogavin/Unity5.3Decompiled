namespace UnityEditor.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditor;

    [InitializeOnLoad]
    internal class JSProxyMgr
    {
        public const int kErrInvalidMessageFormat = -1000;
        public const int kErrInvocationFailed = -1003;
        public const int kErrNone = 0;
        public const int kErrUnknownEvent = -1005;
        public const int kErrUnknownMethod = -1002;
        public const int kErrUnknownObject = -1001;
        public const int kErrUnsupportedProtocol = -1004;
        public const long kInvalidMessageID = -1L;
        public const double kProtocolVersion = 1.0;
        public const string kTypeGetStubInfo = "GETSTUBINFO";
        public const string kTypeInvoke = "INVOKE";
        public const string kTypeOnEvent = "ONEVENT";
        private Dictionary<string, object> m_GlobalObjects = new Dictionary<string, object>();
        private Queue<TaskCallback> m_TaskList = new Queue<TaskCallback>();
        private static string[] s_IgnoredMethods = new string[] { "Equals", "GetHashCode", "GetType", "ToString" };
        private static JSProxyMgr s_Instance = null;

        static JSProxyMgr()
        {
            WebView.OnDomainReload();
        }

        protected JSProxyMgr()
        {
        }

        public void AddGlobalObject(string referenceName, object obj)
        {
            if (this.m_GlobalObjects == null)
            {
                this.m_GlobalObjects = new Dictionary<string, object>();
            }
            this.RemoveGlobalObject(referenceName);
            this.m_GlobalObjects.Add(referenceName, obj);
        }

        private void AddTask(TaskCallback task)
        {
            if (this.m_TaskList == null)
            {
                this.m_TaskList = new Queue<TaskCallback>();
            }
            this.m_TaskList.Enqueue(task);
        }

        private bool DoGetStubInfoMessage(long messageID, ExecCallback callback, Dictionary<string, object> jsonData)
        {
            if (!jsonData.ContainsKey("reference"))
            {
                callback(FormatError(messageID, -1001, "errUnknownObject", "object reference missing"));
                return false;
            }
            string reference = (string) jsonData["reference"];
            object destinationObject = this.GetDestinationObject(reference);
            if (destinationObject == null)
            {
                callback(FormatError(messageID, -1001, "errUnknownObject", "cannot find object with reference <" + reference + ">"));
                return false;
            }
            List<MethodInfo> list = destinationObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList<MethodInfo>();
            list.AddRange(destinationObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.Static).ToList<MethodInfo>());
            ArrayList list2 = new ArrayList();
            foreach (MethodInfo info in list)
            {
                if ((Array.IndexOf<string>(s_IgnoredMethods, info.Name) < 0) && (!info.IsSpecialName || (!info.Name.StartsWith("set_") && !info.Name.StartsWith("get_"))))
                {
                    ParameterInfo[] parameters = info.GetParameters();
                    ArrayList list3 = new ArrayList();
                    foreach (ParameterInfo info2 in parameters)
                    {
                        list3.Add(info2.Name);
                    }
                    JspmMethodInfo info3 = new JspmMethodInfo(info.Name, (string[]) list3.ToArray(typeof(string)));
                    list2.Add(info3);
                }
            }
            List<PropertyInfo> list4 = destinationObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList<PropertyInfo>();
            ArrayList list5 = new ArrayList();
            foreach (PropertyInfo info4 in list4)
            {
                list5.Add(new JspmPropertyInfo(info4.Name, info4.GetValue(destinationObject, null)));
            }
            foreach (FieldInfo info5 in destinationObject.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList<FieldInfo>())
            {
                list5.Add(new JspmPropertyInfo(info5.Name, info5.GetValue(destinationObject)));
            }
            List<EventInfo> list7 = destinationObject.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance).ToList<EventInfo>();
            ArrayList list8 = new ArrayList();
            foreach (EventInfo info6 in list7)
            {
                list8.Add(info6.Name);
            }
            callback(new JspmStubInfoSuccess(messageID, reference, (JspmPropertyInfo[]) list5.ToArray(typeof(JspmPropertyInfo)), (JspmMethodInfo[]) list2.ToArray(typeof(JspmMethodInfo)), (string[]) list8.ToArray(typeof(string))));
            return true;
        }

        private bool DoInvokeMessage(long messageID, ExecCallback callback, Dictionary<string, object> jsonData)
        {
            <DoInvokeMessage>c__AnonStorey9A storeya = new <DoInvokeMessage>c__AnonStorey9A {
                callback = callback,
                messageID = messageID
            };
            if ((!jsonData.ContainsKey("destination") || !jsonData.ContainsKey("method")) || !jsonData.ContainsKey("params"))
            {
                storeya.callback(FormatError(storeya.messageID, -1001, "errUnknownObject", "object reference, method name or parameters missing"));
                return false;
            }
            string reference = (string) jsonData["destination"];
            string str2 = (string) jsonData["method"];
            List<object> data = (List<object>) jsonData["params"];
            storeya.destObject = this.GetDestinationObject(reference);
            if (storeya.destObject == null)
            {
                storeya.callback(FormatError(storeya.messageID, -1001, "errUnknownObject", "cannot find object with reference <" + reference + ">"));
                return false;
            }
            MethodInfo[] methods = storeya.destObject.GetType().GetMethods();
            storeya.foundMethod = null;
            storeya.parameters = null;
            string message = string.Empty;
            foreach (MethodInfo info in methods)
            {
                if (info.Name == str2)
                {
                    try
                    {
                        storeya.parameters = this.ParseParams(info, data);
                        storeya.foundMethod = info;
                        break;
                    }
                    catch (Exception exception)
                    {
                        message = exception.Message;
                    }
                }
            }
            if (storeya.foundMethod == null)
            {
                string[] textArray1 = new string[] { "cannot find method <", str2, "> for object <", reference, ">, reason:", message };
                storeya.callback(FormatError(storeya.messageID, -1002, "errUnknownMethod", string.Concat(textArray1)));
                return false;
            }
            this.AddTask(new TaskCallback(storeya.<>m__1C8));
            return true;
        }

        public bool DoMessage(string jsonRequest, ExecCallback callback, WebView webView)
        {
            long messageID = -1L;
            try
            {
                Dictionary<string, object> jsonData = Json.Deserialize(jsonRequest) as Dictionary<string, object>;
                if (((jsonData == null) || !jsonData.ContainsKey("messageID")) || (!jsonData.ContainsKey("version") || !jsonData.ContainsKey("type")))
                {
                    callback(FormatError(messageID, -1000, "errInvalidMessageFormat", jsonRequest));
                    return false;
                }
                messageID = (long) jsonData["messageID"];
                double num2 = double.Parse((string) jsonData["version"]);
                string str = (string) jsonData["type"];
                if (num2 > 1.0)
                {
                    callback(FormatError(messageID, -1004, "errUnsupportedProtocol", "The protocol version <" + num2 + "> is not supported by this verison of the code"));
                    return false;
                }
                switch (str)
                {
                    case "INVOKE":
                        return this.DoInvokeMessage(messageID, callback, jsonData);

                    case "GETSTUBINFO":
                        return this.DoGetStubInfoMessage(messageID, callback, jsonData);

                    case "ONEVENT":
                        return this.DoOnEventMessage(messageID, callback, jsonData, webView);
                }
            }
            catch (Exception exception)
            {
                callback(FormatError(messageID, -1000, "errInvalidMessageFormat", exception.Message));
            }
            return false;
        }

        private bool DoOnEventMessage(long messageID, ExecCallback callback, Dictionary<string, object> jsonData, WebView webView)
        {
            callback(FormatError(messageID, -1002, "errUnknownMethod", "method DoOnEventMessage is depracated"));
            return false;
        }

        public static void DoTasks()
        {
            GetInstance().ProcessTasks();
        }

        ~JSProxyMgr()
        {
            this.m_GlobalObjects.Clear();
            this.m_GlobalObjects = null;
        }

        public static JspmError FormatError(long messageID, int status, string errorClass, string message)
        {
            return new JspmError(messageID, status, errorClass, message);
        }

        public static JspmSuccess FormatSuccess(long messageID, object result)
        {
            return new JspmSuccess(messageID, result, "INVOKE");
        }

        public object GetDestinationObject(string reference)
        {
            object obj2 = null;
            this.m_GlobalObjects.TryGetValue(reference, out obj2);
            return obj2;
        }

        public static JSProxyMgr GetInstance()
        {
            if (s_Instance == null)
            {
                s_Instance = new JSProxyMgr();
            }
            return s_Instance;
        }

        protected object GetMemberValue(MemberInfo member, object target)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) member).GetValue(target);

                case MemberTypes.Property:
                    try
                    {
                        return ((PropertyInfo) member).GetValue(target, null);
                    }
                    catch (TargetParameterCountException exception)
                    {
                        throw new ArgumentException("MemberInfo has index parameters", "member", exception);
                    }
                    break;
            }
            throw new ArgumentException("MemberInfo is not of type FieldInfo or PropertyInfo", "member");
        }

        private object InternalParseParam(Type type, object data)
        {
            if (data == null)
            {
                return null;
            }
            IList list = data as IList;
            if (list != null)
            {
                if (!type.IsArray)
                {
                    throw new InvalidOperationException("Not an array " + type.FullName);
                }
                Type elementType = type.GetElementType();
                ArrayList list2 = new ArrayList();
                for (int i = 0; i < list.Count; i++)
                {
                    object obj2 = this.InternalParseParam(elementType, list[i]);
                    list2.Add(obj2);
                }
                return list2.ToArray(elementType);
            }
            IDictionary dictionary = data as IDictionary;
            if (dictionary != null)
            {
                if (!type.IsClass)
                {
                    throw new InvalidOperationException("Not a class " + type.FullName);
                }
                ConstructorInfo info = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[0], null);
                if (info == null)
                {
                    throw new InvalidOperationException("Cannot find a default constructor for " + type.FullName);
                }
                object obj3 = info.Invoke(new object[0]);
                foreach (FieldInfo info2 in type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList<FieldInfo>())
                {
                    try
                    {
                        object obj4 = this.InternalParseParam(info2.FieldType, dictionary[info2.Name]);
                        info2.SetValue(obj3, obj4);
                    }
                    catch (KeyNotFoundException)
                    {
                    }
                }
                foreach (PropertyInfo info3 in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList<PropertyInfo>())
                {
                    try
                    {
                        object obj5 = this.InternalParseParam(info3.PropertyType, dictionary[info3.Name]);
                        MethodInfo setMethod = info3.GetSetMethod();
                        if (setMethod != null)
                        {
                            object[] parameters = new object[] { obj5 };
                            setMethod.Invoke(obj3, parameters);
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                    }
                    catch (TargetInvocationException)
                    {
                    }
                }
                return Convert.ChangeType(obj3, type);
            }
            string str = data as string;
            if (str != null)
            {
                return str;
            }
            if (data is bool)
            {
                return (bool) data;
            }
            if (data is double)
            {
                return (double) data;
            }
            if ((!(data is int) && !(data is short)) && ((!(data is int) && !(data is long)) && !(data is long)))
            {
                throw new InvalidOperationException("Cannot parse " + Json.Serialize(data));
            }
            return Convert.ChangeType(data, type);
        }

        public object[] ParseParams(MethodInfo method, List<object> data)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != data.Count)
            {
                return null;
            }
            List<object> list = new List<object>(data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                object item = this.InternalParseParam(parameters[i].ParameterType, data[i]);
                list.Add(item);
            }
            return list.ToArray();
        }

        private void ProcessTasks()
        {
            if ((this.m_TaskList != null) && (this.m_TaskList.Count != 0))
            {
                for (int i = 10; (this.m_TaskList.Count > 0) && (i > 0); i--)
                {
                    this.m_TaskList.Dequeue()();
                }
            }
        }

        public void RemoveGlobalObject(string referenceName)
        {
            if ((this.m_GlobalObjects != null) && this.m_GlobalObjects.ContainsKey(referenceName))
            {
                this.m_GlobalObjects.Remove(referenceName);
            }
        }

        public string Stringify(object target)
        {
            return Json.Serialize(target);
        }

        [CompilerGenerated]
        private sealed class <DoInvokeMessage>c__AnonStorey9A
        {
            internal JSProxyMgr.ExecCallback callback;
            internal object destObject;
            internal MethodInfo foundMethod;
            internal long messageID;
            internal object[] parameters;

            internal void <>m__1C8()
            {
                try
                {
                    object result = this.foundMethod.Invoke(this.destObject, this.parameters);
                    this.callback(JSProxyMgr.FormatSuccess(this.messageID, result));
                }
                catch (TargetInvocationException exception)
                {
                    if (exception.InnerException != null)
                    {
                        this.callback(JSProxyMgr.FormatError(this.messageID, -1003, exception.InnerException.GetType().Name, exception.InnerException.Message));
                    }
                    else
                    {
                        this.callback(JSProxyMgr.FormatError(this.messageID, -1003, exception.GetType().Name, exception.Message));
                    }
                }
                catch (Exception exception2)
                {
                    this.callback(JSProxyMgr.FormatError(this.messageID, -1003, exception2.GetType().Name, exception2.Message));
                }
            }
        }

        public delegate void ExecCallback(object result);

        protected delegate void TaskCallback();
    }
}

