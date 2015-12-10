namespace UnityEngineInternal
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public delegate MethodInfo GetMethodDelegate(Type classType, string methodName, bool searchBaseTypes, bool instanceMethod, Type[] methodParamTypes);
}

