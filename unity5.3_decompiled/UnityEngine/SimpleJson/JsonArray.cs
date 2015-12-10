namespace SimpleJson
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;

    [GeneratedCode("simple-json", "1.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
    internal class JsonArray : List<object>
    {
        public JsonArray()
        {
        }

        public JsonArray(int capacity) : base(capacity)
        {
        }

        public override string ToString()
        {
            string text1 = SimpleJson.SimpleJson.SerializeObject(this);
            if (text1 != null)
            {
                return text1;
            }
            return string.Empty;
        }
    }
}

