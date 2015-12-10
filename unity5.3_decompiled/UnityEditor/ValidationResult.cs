using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct ValidationResult
{
    public bool Success;
    public IValidationRule Rule;
    public IEnumerable<CompilerMessage> CompilerMessages;
}

