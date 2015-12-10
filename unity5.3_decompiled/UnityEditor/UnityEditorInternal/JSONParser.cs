namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using UnityEngine;

    internal class JSONParser
    {
        private char cur;
        private static char[] endcodes = new char[] { '\\', '"' };
        private int idx;
        private string json;
        private int len;
        private int line;
        private int linechar;
        private int pctParsed;

        public JSONParser(string jsondata)
        {
            this.json = jsondata + "    ";
            this.line = 1;
            this.linechar = 1;
            this.len = this.json.Length;
            this.idx = 0;
            this.pctParsed = 0;
        }

        private char Next()
        {
            if (this.cur == '\n')
            {
                this.line++;
                this.linechar = 0;
            }
            this.idx++;
            if (this.idx >= this.len)
            {
                throw new JSONParseException("End of json while parsing at " + this.PosMsg());
            }
            this.linechar++;
            int num = (int) ((this.idx * 100f) / ((float) this.len));
            if (num != this.pctParsed)
            {
                this.pctParsed = num;
            }
            this.cur = this.json[this.idx];
            return this.cur;
        }

        public JSONValue Parse()
        {
            this.cur = this.json[this.idx];
            return this.ParseValue();
        }

        private JSONValue ParseArray()
        {
            this.Next();
            this.SkipWs();
            List<JSONValue> o = new List<JSONValue>();
            while (this.cur != ']')
            {
                o.Add(this.ParseValue());
                this.SkipWs();
                if (this.cur == ',')
                {
                    this.Next();
                    this.SkipWs();
                }
            }
            this.Next();
            return new JSONValue(o);
        }

        private JSONValue ParseConstant()
        {
            string str = string.Empty;
            object[] objArray1 = new object[] { string.Empty, this.cur, this.Next(), this.Next(), this.Next() };
            str = string.Concat(objArray1);
            this.Next();
            switch (str)
            {
                case "true":
                    return new JSONValue(true);

                case "fals":
                    if (this.cur == 'e')
                    {
                        this.Next();
                        return new JSONValue(false);
                    }
                    break;

                case "null":
                    return new JSONValue(null);
            }
            throw new JSONParseException("Invalid token at " + this.PosMsg());
        }

        private JSONValue ParseDict()
        {
            this.Next();
            this.SkipWs();
            Dictionary<string, JSONValue> o = new Dictionary<string, JSONValue>();
            while (this.cur != '}')
            {
                JSONValue value2 = this.ParseValue();
                if (!value2.IsString())
                {
                    throw new JSONParseException("Key not string type at " + this.PosMsg());
                }
                this.SkipWs();
                if (this.cur != ':')
                {
                    throw new JSONParseException("Missing dict entry delimiter ':' at " + this.PosMsg());
                }
                this.Next();
                o.Add(value2.AsString(), this.ParseValue());
                this.SkipWs();
                if (this.cur == ',')
                {
                    this.Next();
                    this.SkipWs();
                }
            }
            this.Next();
            return new JSONValue(o);
        }

        private JSONValue ParseNumber()
        {
            JSONValue value2;
            string str = string.Empty;
            if (this.cur == '-')
            {
                str = "-";
                this.Next();
            }
            while ((this.cur >= '0') && (this.cur <= '9'))
            {
                str = str + this.cur;
                this.Next();
            }
            if (this.cur == '.')
            {
                this.Next();
                str = str + '.';
                while ((this.cur >= '0') && (this.cur <= '9'))
                {
                    str = str + this.cur;
                    this.Next();
                }
            }
            if ((this.cur == 'e') || (this.cur == 'E'))
            {
                str = str + "e";
                this.Next();
                if ((this.cur != '-') && (this.cur != '+'))
                {
                    str = str + this.cur;
                    this.Next();
                }
                while ((this.cur >= '0') && (this.cur <= '9'))
                {
                    str = str + this.cur;
                    this.Next();
                }
            }
            try
            {
                value2 = new JSONValue(Convert.ToSingle(str));
            }
            catch (Exception)
            {
                throw new JSONParseException("Cannot convert string to float : '" + str + "' at " + this.PosMsg());
            }
            return value2;
        }

        private JSONValue ParseString()
        {
            string o = string.Empty;
            this.Next();
            while (this.idx < this.len)
            {
                object[] objArray1;
                string str2;
                int num = this.json.IndexOfAny(endcodes, this.idx);
                if (num < 0)
                {
                    throw new JSONParseException("missing '\"' to end string at " + this.PosMsg());
                }
                o = o + this.json.Substring(this.idx, num - this.idx);
                if (this.json[num] == '"')
                {
                    this.cur = this.json[num];
                    this.idx = num;
                    break;
                }
                num++;
                if (num >= this.len)
                {
                    throw new JSONParseException("End of json while parsing while parsing string at " + this.PosMsg());
                }
                char ch = this.json[num];
                char ch2 = ch;
                switch (ch2)
                {
                    case 'n':
                        o = o + '\n';
                        goto Label_02AE;

                    case 'r':
                        o = o + '\r';
                        goto Label_02AE;

                    case 't':
                        o = o + '\t';
                        goto Label_02AE;

                    case 'u':
                        str2 = string.Empty;
                        if ((num + 4) >= this.len)
                        {
                            throw new JSONParseException("End of json while parsing while parsing unicode char near " + this.PosMsg());
                        }
                        break;

                    default:
                        if (((ch2 == '"') || (ch2 == '/')) || (ch2 == '\\'))
                        {
                            o = o + ch;
                        }
                        else if (ch2 == 'b')
                        {
                            o = o + '\b';
                        }
                        else
                        {
                            if (ch2 != 'f')
                            {
                                goto Label_027B;
                            }
                            o = o + '\f';
                        }
                        goto Label_02AE;
                }
                str2 = (str2 + this.json[num + 1] + this.json[num + 2]) + this.json[num + 3] + this.json[num + 4];
                try
                {
                    int num2 = int.Parse(str2, NumberStyles.AllowHexSpecifier);
                    o = o + ((char) num2);
                }
                catch (FormatException)
                {
                    throw new JSONParseException("Invalid unicode escape char near " + this.PosMsg());
                }
                num += 4;
                goto Label_02AE;
            Label_027B:
                objArray1 = new object[] { "Invalid escape char '", ch, "' near ", this.PosMsg() };
                throw new JSONParseException(string.Concat(objArray1));
            Label_02AE:
                this.idx = num + 1;
            }
            if (this.idx >= this.len)
            {
                throw new JSONParseException("End of json while parsing while parsing string near " + this.PosMsg());
            }
            this.cur = this.json[this.idx];
            this.Next();
            return new JSONValue(o);
        }

        private JSONValue ParseValue()
        {
            this.SkipWs();
            char cur = this.cur;
            switch (cur)
            {
                case '"':
                    return this.ParseString();

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
                    return this.ParseNumber();

                case '[':
                    return this.ParseArray();

                case 'f':
                case 'n':
                case 't':
                    return this.ParseConstant();
            }
            if (cur != '{')
            {
                throw new JSONParseException("Cannot parse json value starting with '" + this.json.Substring(this.idx, 5) + "' at " + this.PosMsg());
            }
            return this.ParseDict();
        }

        private string PosMsg()
        {
            return ("line " + this.line.ToString() + ", column " + this.linechar.ToString());
        }

        public static JSONValue SimpleParse(string jsondata)
        {
            JSONParser parser = new JSONParser(jsondata);
            try
            {
                return parser.Parse();
            }
            catch (JSONParseException exception)
            {
                Debug.LogError(exception.Message);
            }
            return new JSONValue(null);
        }

        private void SkipWs()
        {
            string str = " \n\t\r";
            while (str.IndexOf(this.cur) != -1)
            {
                this.Next();
            }
        }
    }
}

