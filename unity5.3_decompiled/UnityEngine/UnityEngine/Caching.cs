namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class Caching
    {
        [Obsolete("Size is now specified as a long")]
        public static bool Authorize(string name, string domain, int size, string signature)
        {
            return Authorize(name, domain, (long) size, signature);
        }

        public static bool Authorize(string name, string domain, long size, string signature)
        {
            return Authorize(name, domain, size, -1, signature);
        }

        [Obsolete("Size is now specified as a long")]
        public static bool Authorize(string name, string domain, int size, int expiration, string signature)
        {
            return Authorize(name, domain, (long) size, expiration, signature);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Authorize(string name, string domain, long size, int expiration, string signature);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CleanCache();
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("this API is not for public use."), WrapperlessIcall]
        public static extern bool CleanNamedCache(string name);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("This function is obsolete and has no effect."), WrapperlessIcall]
        public static extern bool DeleteFromCache(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("This function is obsolete and will always return -1. Use IsVersionCached instead.")]
        public static extern int GetVersionFromCache(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_IsVersionCached(string url, ref Hash128 hash);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_MarkAsUsed(string url, ref Hash128 hash);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ResetNoBackupFlag(string url, ref Hash128 hash);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetNoBackupFlag(string url, ref Hash128 hash);
        public static bool IsVersionCached(string url, int version)
        {
            Hash128 hash = new Hash128(0, 0, 0, (uint) version);
            return IsVersionCached(url, hash);
        }

        public static bool IsVersionCached(string url, Hash128 hash)
        {
            return INTERNAL_CALL_IsVersionCached(url, ref hash);
        }

        public static bool MarkAsUsed(string url, int version)
        {
            Hash128 hash = new Hash128(0, 0, 0, (uint) version);
            return MarkAsUsed(url, hash);
        }

        public static bool MarkAsUsed(string url, Hash128 hash)
        {
            return INTERNAL_CALL_MarkAsUsed(url, ref hash);
        }

        public static void ResetNoBackupFlag(string url, int version)
        {
        }

        public static void ResetNoBackupFlag(string url, Hash128 hash)
        {
            INTERNAL_CALL_ResetNoBackupFlag(url, ref hash);
        }

        public static void SetNoBackupFlag(string url, int version)
        {
        }

        public static void SetNoBackupFlag(string url, Hash128 hash)
        {
            INTERNAL_CALL_SetNoBackupFlag(url, ref hash);
        }

        public static bool compressionEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int expirationDelay { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("this API is not for public use.")]
        public static CacheIndex[] index { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static long maximumAvailableDiskSpace { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool ready { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Please use Caching.spaceFree instead")]
        public static int spaceAvailable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static long spaceFree { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static long spaceOccupied { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Please use Caching.spaceOccupied instead")]
        public static int spaceUsed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

