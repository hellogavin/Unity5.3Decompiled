namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class UnityEventQueueSystem
    {
        public static string GenerateEventIdForPayload(string eventPayloadName)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            object[] args = new object[] { 
                buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6], buffer[7], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15], 
                eventPayloadName
             };
            return string.Format("REGISTER_EVENT_ID(0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}ULL,0x{8:X2}{9:X2}{10:X2}{11:X2}{12:X2}{13:X2}{14:X2}{15:X2}ULL,{16})", args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetGlobalEventQueue();
    }
}

