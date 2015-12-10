namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal static class Json
    {
        public static object Deserialize(string json)
        {
            if (json == null)
            {
                return null;
            }
            return Parser.Parse(json);
        }

        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        private sealed class Parser : IDisposable
        {
            [CompilerGenerated]
            private static Dictionary<string, int> <>f__switch$map21;
            private StringReader json;
            private const string WORD_BREAK = "{}[],:\"";

            private Parser(string jsonString)
            {
                this.json = new StringReader(jsonString);
            }

            public void Dispose()
            {
                this.json.Dispose();
                this.json = null;
            }

            private void EatWhitespace()
            {
                while (char.IsWhiteSpace(this.PeekChar))
                {
                    this.json.Read();
                    if (this.json.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            public static bool IsWordBreak(char c)
            {
                return (char.IsWhiteSpace(c) || ("{}[],:\"".IndexOf(c) != -1));
            }

            public static object Parse(string jsonString)
            {
                using (Json.Parser parser = new Json.Parser(jsonString))
                {
                    return parser.ParseValue();
                }
            }

            private List<object> ParseArray()
            {
                List<object> list = new List<object>();
                this.json.Read();
                bool flag = true;
                while (flag)
                {
                    TOKEN nextToken = this.NextToken;
                    switch (nextToken)
                    {
                        case TOKEN.SQUARED_CLOSE:
                        {
                            flag = false;
                            continue;
                        }
                        case TOKEN.COMMA:
                        {
                            continue;
                        }
                        case TOKEN.NONE:
                            return null;
                    }
                    object item = this.ParseByToken(nextToken);
                    list.Add(item);
                }
                return list;
            }

            private object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.CURLY_OPEN:
                        return this.ParseObject();

                    case TOKEN.SQUARED_OPEN:
                        return this.ParseArray();

                    case TOKEN.STRING:
                        return this.ParseString();

                    case TOKEN.NUMBER:
                        return this.ParseNumber();

                    case TOKEN.TRUE:
                        return true;

                    case TOKEN.FALSE:
                        return false;

                    case TOKEN.NULL:
                        return null;
                }
                return null;
            }

            private object ParseNumber()
            {
                double num2;
                string nextWord = this.NextWord;
                if (nextWord.IndexOf('.') == -1)
                {
                    long num;
                    long.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num);
                    return num;
                }
                double.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
                return num2;
            }

            private Dictionary<string, object> ParseObject()
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                this.json.Read();
                while (true)
                {
                    TOKEN nextToken = this.NextToken;
                    switch (nextToken)
                    {
                        case TOKEN.NONE:
                            return null;

                        case TOKEN.CURLY_CLOSE:
                            return dictionary;
                    }
                    if (nextToken != TOKEN.COMMA)
                    {
                        string str = this.ParseString();
                        if (str == null)
                        {
                            return null;
                        }
                        if (this.NextToken != TOKEN.COLON)
                        {
                            return null;
                        }
                        this.json.Read();
                        dictionary[str] = this.ParseValue();
                    }
                }
            }

            private string ParseString()
            {
                StringBuilder builder = new StringBuilder();
                this.json.Read();
                bool flag = true;
                while (flag)
                {
                    char[] chArray;
                    int num;
                    if (this.json.Peek() == -1)
                    {
                        flag = false;
                        break;
                    }
                    char nextChar = this.NextChar;
                    char ch2 = nextChar;
                    if (ch2 == '"')
                    {
                        flag = false;
                        continue;
                    }
                    if (ch2 != '\\')
                    {
                        goto Label_016F;
                    }
                    if (this.json.Peek() == -1)
                    {
                        flag = false;
                        continue;
                    }
                    nextChar = this.NextChar;
                    char ch3 = nextChar;
                    switch (ch3)
                    {
                        case 'n':
                        {
                            builder.Append('\n');
                            continue;
                        }
                        case 'r':
                        {
                            builder.Append('\r');
                            continue;
                        }
                        case 't':
                        {
                            builder.Append('\t');
                            continue;
                        }
                        case 'u':
                            chArray = new char[4];
                            num = 0;
                            goto Label_0148;

                        default:
                        {
                            if (((ch3 != '"') && (ch3 != '/')) && (ch3 != '\\'))
                            {
                                if (ch3 == 'b')
                                {
                                    break;
                                }
                                if (ch3 == 'f')
                                {
                                    goto Label_00F1;
                                }
                            }
                            else
                            {
                                builder.Append(nextChar);
                            }
                            continue;
                        }
                    }
                    builder.Append('\b');
                    continue;
                Label_00F1:
                    builder.Append('\f');
                    continue;
                Label_0138:
                    chArray[num] = this.NextChar;
                    num++;
                Label_0148:
                    if (num < 4)
                    {
                        goto Label_0138;
                    }
                    builder.Append((char) Convert.ToInt32(new string(chArray), 0x10));
                    continue;
                Label_016F:
                    builder.Append(nextChar);
                }
                return builder.ToString();
            }

            private object ParseValue()
            {
                TOKEN nextToken = this.NextToken;
                return this.ParseByToken(nextToken);
            }

            private char NextChar
            {
                get
                {
                    return Convert.ToChar(this.json.Read());
                }
            }

            private TOKEN NextToken
            {
                get
                {
                    this.EatWhitespace();
                    if (this.json.Peek() != -1)
                    {
                        switch (this.PeekChar)
                        {
                            case '"':
                                return TOKEN.STRING;

                            case ',':
                                this.json.Read();
                                return TOKEN.COMMA;

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
                                return TOKEN.NUMBER;

                            case ':':
                                return TOKEN.COLON;

                            case '[':
                                return TOKEN.SQUARED_OPEN;

                            case ']':
                                this.json.Read();
                                return TOKEN.SQUARED_CLOSE;

                            case '{':
                                return TOKEN.CURLY_OPEN;

                            case '}':
                                this.json.Read();
                                return TOKEN.CURLY_CLOSE;
                        }
                        string nextWord = this.NextWord;
                        if (nextWord != null)
                        {
                            int num;
                            if (<>f__switch$map21 == null)
                            {
                                Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
                                dictionary.Add("false", 0);
                                dictionary.Add("true", 1);
                                dictionary.Add("null", 2);
                                <>f__switch$map21 = dictionary;
                            }
                            if (<>f__switch$map21.TryGetValue(nextWord, out num))
                            {
                                switch (num)
                                {
                                    case 0:
                                        return TOKEN.FALSE;

                                    case 1:
                                        return TOKEN.TRUE;

                                    case 2:
                                        return TOKEN.NULL;
                                }
                            }
                        }
                    }
                    return TOKEN.NONE;
                }
            }

            private string NextWord
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    while (!IsWordBreak(this.PeekChar))
                    {
                        builder.Append(this.NextChar);
                        if (this.json.Peek() == -1)
                        {
                            break;
                        }
                    }
                    return builder.ToString();
                }
            }

            private char PeekChar
            {
                get
                {
                    return Convert.ToChar(this.json.Peek());
                }
            }

            private enum TOKEN
            {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL
            }
        }

        private sealed class Serializer
        {
            private StringBuilder builder = new StringBuilder();

            private Serializer()
            {
            }

            public static string Serialize(object obj)
            {
                Json.Serializer serializer = new Json.Serializer();
                serializer.SerializeValue(obj);
                return serializer.builder.ToString();
            }

            private void SerializeArray(IList anArray)
            {
                this.builder.Append('[');
                bool flag = true;
                IEnumerator enumerator = anArray.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        if (!flag)
                        {
                            this.builder.Append(',');
                        }
                        this.SerializeValue(current);
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
                this.builder.Append(']');
            }

            private void SerializeObject(IDictionary obj)
            {
                bool flag = true;
                this.builder.Append('{');
                IEnumerator enumerator = obj.Keys.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        if (!flag)
                        {
                            this.builder.Append(',');
                        }
                        this.SerializeString(current.ToString());
                        this.builder.Append(':');
                        this.SerializeValue(obj[current]);
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
                this.builder.Append('}');
            }

            private void SerializeOther(object value)
            {
                if (value is float)
                {
                    this.builder.Append(((float) value).ToString("R", CultureInfo.InvariantCulture));
                }
                else if ((((value is int) || (value is uint)) || ((value is long) || (value is sbyte))) || (((value is byte) || (value is short)) || ((value is ushort) || (value is ulong))))
                {
                    this.builder.Append(value);
                }
                else if ((value is double) || (value is decimal))
                {
                    this.builder.Append(Convert.ToDouble(value).ToString("R", CultureInfo.InvariantCulture));
                }
                else
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    foreach (FieldInfo info in value.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList<FieldInfo>())
                    {
                        dictionary.Add(info.Name, info.GetValue(value));
                    }
                    foreach (PropertyInfo info2 in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList<PropertyInfo>())
                    {
                        dictionary.Add(info2.Name, info2.GetValue(value, null));
                    }
                    this.SerializeObject(dictionary);
                }
            }

            private void SerializeString(string str)
            {
                this.builder.Append('"');
                foreach (char ch in str.ToCharArray())
                {
                    int num2;
                    char ch2 = ch;
                    switch (ch2)
                    {
                        case '\b':
                        {
                            this.builder.Append(@"\b");
                            continue;
                        }
                        case '\t':
                        {
                            this.builder.Append(@"\t");
                            continue;
                        }
                        case '\n':
                        {
                            this.builder.Append(@"\n");
                            continue;
                        }
                        case '\f':
                        {
                            this.builder.Append(@"\f");
                            continue;
                        }
                        case '\r':
                        {
                            this.builder.Append(@"\r");
                            continue;
                        }
                        default:
                        {
                            if (ch2 != '"')
                            {
                                if (ch2 == '\\')
                                {
                                    break;
                                }
                                goto Label_00F7;
                            }
                            this.builder.Append("\\\"");
                            continue;
                        }
                    }
                    this.builder.Append(@"\\");
                    continue;
                Label_00F7:
                    num2 = Convert.ToInt32(ch);
                    if ((num2 >= 0x20) && (num2 <= 0x7e))
                    {
                        this.builder.Append(ch);
                    }
                    else
                    {
                        this.builder.Append(@"\u");
                        this.builder.Append(num2.ToString("x4"));
                    }
                }
                this.builder.Append('"');
            }

            private void SerializeValue(object value)
            {
                if (value == null)
                {
                    this.builder.Append("null");
                }
                else
                {
                    string str = value as string;
                    if (str != null)
                    {
                        this.SerializeString(str);
                    }
                    else if (value is bool)
                    {
                        this.builder.Append(!((bool) value) ? "false" : "true");
                    }
                    else
                    {
                        IList anArray = value as IList;
                        if (anArray != null)
                        {
                            this.SerializeArray(anArray);
                        }
                        else
                        {
                            IDictionary dictionary = value as IDictionary;
                            if (dictionary != null)
                            {
                                this.SerializeObject(dictionary);
                            }
                            else if (value is char)
                            {
                                this.SerializeString(new string((char) value, 1));
                            }
                            else
                            {
                                this.SerializeOther(value);
                            }
                        }
                    }
                }
            }
        }
    }
}

