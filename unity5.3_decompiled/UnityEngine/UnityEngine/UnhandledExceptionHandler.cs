namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    internal sealed class UnhandledExceptionHandler
    {
        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception exceptionObject = args.ExceptionObject as Exception;
            if (exceptionObject != null)
            {
                PrintException("Unhandled Exception: ", exceptionObject);
            }
            NativeUnhandledExceptionHandler();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void NativeUnhandledExceptionHandler();
        private static void PrintException(string title, Exception e)
        {
            Debug.LogError(title + e.ToString());
            if (e.InnerException != null)
            {
                PrintException("Inner Exception: ", e.InnerException);
            }
        }

        [RequiredByNativeCode]
        private static void RegisterUECatcher()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler.HandleUnhandledException);
        }
    }
}

