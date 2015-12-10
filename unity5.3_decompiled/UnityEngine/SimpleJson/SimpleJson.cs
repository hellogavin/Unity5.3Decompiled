namespace SimpleJson
{
    using SimpleJson.Reflection;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;

    [GeneratedCode("simple-json", "1.0.0")]
    internal static class SimpleJson
    {
        private static IJsonSerializerStrategy _currentJsonSerializerStrategy;
        private static SimpleJson.PocoJsonSerializerStrategy _pocoJsonSerializerStrategy;
        private const int BUILDER_CAPACITY = 0x7d0;
        private const int TOKEN_COLON = 5;
        private const int TOKEN_COMMA = 6;
        private const int TOKEN_CURLY_CLOSE = 2;
        private const int TOKEN_CURLY_OPEN = 1;
        private const int TOKEN_FALSE = 10;
        private const int TOKEN_NONE = 0;
        private const int TOKEN_NULL = 11;
        private const int TOKEN_NUMBER = 8;
        private const int TOKEN_SQUARED_CLOSE = 4;
        private const int TOKEN_SQUARED_OPEN = 3;
        private const int TOKEN_STRING = 7;
        private const int TOKEN_TRUE = 9;

        private static string ConvertFromUtf32(int utf32)
        {
            if ((utf32 < 0) || (utf32 > 0x10ffff))
            {
                throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
            }
            if ((0xd800 <= utf32) && (utf32 <= 0xdfff))
            {
                throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
            }
            if (utf32 < 0x10000)
            {
                return new string((char) utf32, 1);
            }
            utf32 -= 0x10000;
            return new string(new char[] { (char) ((utf32 >> 10) + 0xd800), (char) ((utf32 % 0x400) + 0xdc00) });
        }

        public static object DeserializeObject(string json)
        {
            object obj2;
            if (!TryDeserializeObject(json, out obj2))
            {
                throw new SerializationException("Invalid JSON string");
            }
            return obj2;
        }

        public static T DeserializeObject<T>(string json)
        {
            return (T) DeserializeObject(json, typeof(T), null);
        }

        public static T DeserializeObject<T>(string json, IJsonSerializerStrategy jsonSerializerStrategy)
        {
            return (T) DeserializeObject(json, typeof(T), jsonSerializerStrategy);
        }

        public static object DeserializeObject(string json, Type type)
        {
            return DeserializeObject(json, type, null);
        }

        public static object DeserializeObject(string json, Type type, IJsonSerializerStrategy jsonSerializerStrategy)
        {
            object obj2 = DeserializeObject(json);
            if (jsonSerializerStrategy == null)
            {
            }
            return (((type != null) && ((obj2 == null) || !ReflectionUtils.IsAssignableFrom(obj2.GetType(), type))) ? CurrentJsonSerializerStrategy.DeserializeObject(obj2, type) : obj2);
        }

        private static void EatWhitespace(char[] json, ref int index)
        {
            while (index < json.Length)
            {
                if (" \t\n\r\b\f".IndexOf(json[index]) == -1)
                {
                    break;
                }
                index++;
            }
        }

        public static string EscapeToJavascriptString(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return jsonString;
            }
            StringBuilder builder = new StringBuilder();
            int num = 0;
            while (num < jsonString.Length)
            {
                char ch = jsonString[num++];
                if (ch == '\\')
                {
                    int num2 = jsonString.Length - num;
                    if (num2 >= 2)
                    {
                        char ch2 = jsonString[num];
                        switch (ch2)
                        {
                            case '\\':
                            {
                                builder.Append('\\');
                                num++;
                                continue;
                            }
                            case '"':
                            {
                                builder.Append("\"");
                                num++;
                                continue;
                            }
                            case 't':
                            {
                                builder.Append('\t');
                                num++;
                                continue;
                            }
                            case 'b':
                            {
                                builder.Append('\b');
                                num++;
                                continue;
                            }
                            case 'n':
                            {
                                builder.Append('\n');
                                num++;
                                continue;
                            }
                        }
                        if (ch2 == 'r')
                        {
                            builder.Append('\r');
                            num++;
                        }
                    }
                }
                else
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }

        private static int GetLastIndexOfNumber(char[] json, int index)
        {
            int num = index;
            while (num < json.Length)
            {
                if ("0123456789+-.eE".IndexOf(json[num]) == -1)
                {
                    break;
                }
                num++;
            }
            return (num - 1);
        }

        private static bool IsNumeric(object value)
        {
            return ((value is sbyte) || ((value is byte) || ((value is short) || ((value is ushort) || ((value is int) || ((value is uint) || ((value is long) || ((value is ulong) || ((value is float) || ((value is double) || (value is decimal)))))))))));
        }

        private static int LookAhead(char[] json, int index)
        {
            int num = index;
            return NextToken(json, ref num);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);
            if (index != json.Length)
            {
                char ch = json[index];
                index++;
                switch (ch)
                {
                    case '"':
                        return 7;

                    case ',':
                        return 6;

                    case '-':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        return 8;

                    case ':':
                        return 5;

                    case '[':
                        return 3;

                    case ']':
                        return 4;

                    case '{':
                        return 1;

                    case '}':
                        return 2;
                }
                index--;
                int num = json.Length - index;
                if ((((num >= 5) && (json[index] == 'f')) && ((json[index + 1] == 'a') && (json[index + 2] == 'l'))) && ((json[index + 3] == 's') && (json[index + 4] == 'e')))
                {
                    index += 5;
                    return 10;
                }
                if ((((num >= 4) && (json[index] == 't')) && ((json[index + 1] == 'r') && (json[index + 2] == 'u'))) && (json[index + 3] == 'e'))
                {
                    index += 4;
                    return 9;
                }
                if ((((num >= 4) && (json[index] == 'n')) && ((json[index + 1] == 'u') && (json[index + 2] == 'l'))) && (json[index + 3] == 'l'))
                {
                    index += 4;
                    return 11;
                }
            }
            return 0;
        }

        private static JsonArray ParseArray(char[] json, ref int index, ref bool success)
        {
            JsonArray array = new JsonArray();
            NextToken(json, ref index);
            bool flag = false;
            while (!flag)
            {
                int num = LookAhead(json, index);
                if (num == 0)
                {
                    success = false;
                    return null;
                }
                if (num == 6)
                {
                    NextToken(json, ref index);
                }
                else
                {
                    if (num == 4)
                    {
                        NextToken(json, ref index);
                        return array;
                    }
                    object item = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        return null;
                    }
                    array.Add(item);
                }
            }
            return array;
        }

        private static object ParseNumber(char[] json, ref int index, ref bool success)
        {
            object obj2;
            EatWhitespace(json, ref index);
            int lastIndexOfNumber = GetLastIndexOfNumber(json, index);
            int length = (lastIndexOfNumber - index) + 1;
            string str = new string(json, index, length);
            if ((str.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1) || (str.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1))
            {
                double num3;
                success = double.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out num3);
                obj2 = num3;
            }
            else
            {
                long num4;
                success = long.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out num4);
                obj2 = num4;
            }
            index = lastIndexOfNumber + 1;
            return obj2;
        }

        private static IDictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
        {
            IDictionary<string, object> dictionary = new JsonObject();
            NextToken(json, ref index);
            bool flag = false;
            while (!flag)
            {
                switch (LookAhead(json, index))
                {
                    case 0:
                        success = false;
                        return null;

                    case 6:
                    {
                        NextToken(json, ref index);
                        continue;
                    }
                    case 2:
                        NextToken(json, ref index);
                        return dictionary;
                }
                string str = ParseString(json, ref index, ref success);
                if (!success)
                {
                    success = false;
                    return null;
                }
                if (NextToken(json, ref index) != 5)
                {
                    success = false;
                    return null;
                }
                object obj2 = ParseValue(json, ref index, ref success);
                if (!success)
                {
                    success = false;
                    return null;
                }
                dictionary[str] = obj2;
            }
            return dictionary;
        }

        private static string ParseString(char[] json, ref int index, ref bool success)
        {
            StringBuilder builder = new StringBuilder(0x7d0);
            EatWhitespace(json, ref index);
            char ch = json[index++];
            bool flag = false;
            while (!flag)
            {
                if (index == json.Length)
                {
                    break;
                }
                ch = json[index++];
                if (ch == '"')
                {
                    flag = true;
                    break;
                }
                if (ch == '\\')
                {
                    if (index == json.Length)
                    {
                        break;
                    }
                    ch = json[index++];
                    if (ch == '"')
                    {
                        builder.Append('"');
                    }
                    else
                    {
                        if (ch == '\\')
                        {
                            builder.Append('\\');
                            continue;
                        }
                        if (ch == '/')
                        {
                            builder.Append('/');
                            continue;
                        }
                        if (ch == 'b')
                        {
                            builder.Append('\b');
                            continue;
                        }
                        if (ch == 'f')
                        {
                            builder.Append('\f');
                            continue;
                        }
                        if (ch == 'n')
                        {
                            builder.Append('\n');
                            continue;
                        }
                        if (ch == 'r')
                        {
                            builder.Append('\r');
                            continue;
                        }
                        if (ch == 't')
                        {
                            builder.Append('\t');
                        }
                        else if (ch == 'u')
                        {
                            uint num2;
                            int num = json.Length - index;
                            if (num < 4)
                            {
                                break;
                            }
                            if (!(success = uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2)))
                            {
                                return string.Empty;
                            }
                            if ((0xd800 <= num2) && (num2 <= 0xdbff))
                            {
                                uint num3;
                                index += 4;
                                num = json.Length - index;
                                if ((((num < 6) || (new string(json, index, 2) != @"\u")) || (!uint.TryParse(new string(json, index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num3) || (0xdc00 > num3))) || (num3 > 0xdfff))
                                {
                                    success = false;
                                    return string.Empty;
                                }
                                builder.Append((char) num2);
                                builder.Append((char) num3);
                                index += 6;
                            }
                            else
                            {
                                builder.Append(ConvertFromUtf32((int) num2));
                                index += 4;
                            }
                        }
                    }
                }
                else
                {
                    builder.Append(ch);
                }
            }
            if (!flag)
            {
                success = false;
                return null;
            }
            return builder.ToString();
        }

        private static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case 1:
                    return ParseObject(json, ref index, ref success);

                case 3:
                    return ParseArray(json, ref index, ref success);

                case 7:
                    return ParseString(json, ref index, ref success);

                case 8:
                    return ParseNumber(json, ref index, ref success);

                case 9:
                    NextToken(json, ref index);
                    return true;

                case 10:
                    NextToken(json, ref index);
                    return false;

                case 11:
                    NextToken(json, ref index);
                    return null;
            }
            success = false;
            return null;
        }

        private static bool SerializeArray(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable anArray, StringBuilder builder)
        {
            builder.Append("[");
            bool flag = true;
            IEnumerator enumerator = anArray.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    if (!flag)
                    {
                        builder.Append(",");
                    }
                    if (!SerializeValue(jsonSerializerStrategy, current, builder))
                    {
                        return false;
                    }
                    flag = false;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            builder.Append("]");
            return true;
        }

        private static bool SerializeNumber(object number, StringBuilder builder)
        {
            if (number is long)
            {
                builder.Append(((long) number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is ulong)
            {
                builder.Append(((ulong) number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is int)
            {
                builder.Append(((int) number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is uint)
            {
                builder.Append(((uint) number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is decimal)
            {
                builder.Append(((decimal) number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is float)
            {
                builder.Append(((float) number).ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                builder.Append(Convert.ToDouble(number, CultureInfo.InvariantCulture).ToString("r", CultureInfo.InvariantCulture));
            }
            return true;
        }

        public static string SerializeObject(object json)
        {
            return SerializeObject(json, CurrentJsonSerializerStrategy);
        }

        public static string SerializeObject(object json, IJsonSerializerStrategy jsonSerializerStrategy)
        {
            StringBuilder builder = new StringBuilder(0x7d0);
            return (!SerializeValue(jsonSerializerStrategy, json, builder) ? null : builder.ToString());
        }

        private static bool SerializeObject(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable keys, IEnumerable values, StringBuilder builder)
        {
            builder.Append("{");
            IEnumerator enumerator = keys.GetEnumerator();
            IEnumerator enumerator2 = values.GetEnumerator();
            for (bool flag = true; enumerator.MoveNext() && enumerator2.MoveNext(); flag = false)
            {
                object current = enumerator.Current;
                object obj3 = enumerator2.Current;
                if (!flag)
                {
                    builder.Append(",");
                }
                string aString = current as string;
                if (aString != null)
                {
                    SerializeString(aString, builder);
                }
                else if (!SerializeValue(jsonSerializerStrategy, obj3, builder))
                {
                    return false;
                }
                builder.Append(":");
                if (!SerializeValue(jsonSerializerStrategy, obj3, builder))
                {
                    return false;
                }
            }
            builder.Append("}");
            return true;
        }

        private static bool SerializeString(string aString, StringBuilder builder)
        {
            builder.Append("\"");
            foreach (char ch in aString.ToCharArray())
            {
                switch (ch)
                {
                    case '"':
                        builder.Append("\\\"");
                        break;

                    case '\\':
                        builder.Append(@"\\");
                        break;

                    case '\b':
                        builder.Append(@"\b");
                        break;

                    case '\f':
                        builder.Append(@"\f");
                        break;

                    case '\n':
                        builder.Append(@"\n");
                        break;

                    case '\r':
                        builder.Append(@"\r");
                        break;

                    case '\t':
                        builder.Append(@"\t");
                        break;

                    default:
                        builder.Append(ch);
                        break;
                }
            }
            builder.Append("\"");
            return true;
        }

        private static bool SerializeValue(IJsonSerializerStrategy jsonSerializerStrategy, object value, StringBuilder builder)
        {
            object obj2;
            bool flag = true;
            string aString = value as string;
            if (aString != null)
            {
                return SerializeString(aString, builder);
            }
            IDictionary<string, object> dictionary = value as IDictionary<string, object>;
            if (dictionary != null)
            {
                return SerializeObject(jsonSerializerStrategy, dictionary.Keys, dictionary.Values, builder);
            }
            IDictionary<string, string> dictionary2 = value as IDictionary<string, string>;
            if (dictionary2 != null)
            {
                return SerializeObject(jsonSerializerStrategy, dictionary2.Keys, dictionary2.Values, builder);
            }
            IEnumerable anArray = value as IEnumerable;
            if (anArray != null)
            {
                return SerializeArray(jsonSerializerStrategy, anArray, builder);
            }
            if (IsNumeric(value))
            {
                return SerializeNumber(value, builder);
            }
            if (value is bool)
            {
                builder.Append(!((bool) value) ? "false" : "true");
                return flag;
            }
            if (value == null)
            {
                builder.Append("null");
                return flag;
            }
            flag = jsonSerializerStrategy.TrySerializeNonPrimitiveObject(value, out obj2);
            if (flag)
            {
                SerializeValue(jsonSerializerStrategy, obj2, builder);
            }
            return flag;
        }

        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification="Need to support .NET 2")]
        public static bool TryDeserializeObject(string json, out object obj)
        {
            bool success = true;
            if (json != null)
            {
                char[] chArray = json.ToCharArray();
                int index = 0;
                obj = ParseValue(chArray, ref index, ref success);
                return success;
            }
            obj = null;
            return success;
        }

        public static IJsonSerializerStrategy CurrentJsonSerializerStrategy
        {
            get
            {
                if (_currentJsonSerializerStrategy == null)
                {
                }
                return (_currentJsonSerializerStrategy = PocoJsonSerializerStrategy);
            }
            set
            {
                _currentJsonSerializerStrategy = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static SimpleJson.PocoJsonSerializerStrategy PocoJsonSerializerStrategy
        {
            get
            {
                if (_pocoJsonSerializerStrategy == null)
                {
                }
                return (_pocoJsonSerializerStrategy = new SimpleJson.PocoJsonSerializerStrategy());
            }
        }
    }
}

