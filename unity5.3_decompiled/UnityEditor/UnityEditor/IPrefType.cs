namespace UnityEditor
{
    using System;

    internal interface IPrefType
    {
        void FromUniqueString(string sstr);
        string ToUniqueString();
    }
}

