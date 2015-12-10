namespace SimpleJson
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [GeneratedCode("simple-json", "1.0.0")]
    internal interface IJsonSerializerStrategy
    {
        object DeserializeObject(object value, Type type);
        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification="Need to support .NET 2")]
        bool TrySerializeNonPrimitiveObject(object input, out object output);
    }
}

