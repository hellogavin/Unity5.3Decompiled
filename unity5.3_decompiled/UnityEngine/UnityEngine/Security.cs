namespace UnityEngine
{
    using Mono.Security;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Cryptography;
    using UnityEngine.Internal;

    public sealed class Security
    {
        private static List<Assembly> _verifiedAssemblies = new List<Assembly>();
        private static readonly string kSignatureExtension = ".signature";
        private const string publicVerificationKey = "<RSAKeyValue><Modulus>uP7lsvrE6fNoQWhUIdJnQrgKoGXBkgWgs5l1xmS9gfyNkFSXgugIpfmN/0YrtL57PezYFXN0CogAnOpOtcUmpcIrh524VL/7bIh+jDUaOCG292PIx92dtzqCTvbUdCYUmaag9VlrdAw05FxYQJi2iZ/X6EiuO1TnqpVNFCDb6pXPAssoO4Uxn9JXBzL0muNRdcmFGRiLp7JQOL7a2aeU9mF9qjMprnww0k8COa6tHdnNWJqaxdFO+Etk3os0ns/gQ2FWrztKemM1Wfu7lk/B1F+V2g0adwlTiuyNHw6to+5VQXWK775RXB9wAGr8KhsVD5IJvmxrdBT8KVEWve+OXQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        internal static void ClearVerifiedAssemblies()
        {
            _verifiedAssemblies.Clear();
        }

        [SecuritySafeCritical]
        public static string GetChainOfTrustValue(string name)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (!_verifiedAssemblies.Contains(callingAssembly))
            {
                throw new ArgumentException("Calling assembly needs to be verified by Security.LoadAndVerifyAssembly");
            }
            byte[] publicKeyToken = callingAssembly.GetName().GetPublicKeyToken();
            return GetChainOfTrustValueInternal(name, TokenToHex(publicKeyToken));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetChainOfTrustValueInternal(string name, string publicKeyToken);
        private static MethodInfo GetUnityCrossDomainHelperMethod(string methodname)
        {
            Type type = Types.GetType("UnityEngine.UnityCrossDomainHelper", "CrossDomainPolicyParser, Version=1.0.0.0, Culture=neutral");
            if (type == null)
            {
                throw new SecurityException("Cant find type UnityCrossDomainHelper");
            }
            MethodInfo method = type.GetMethod(methodname);
            if (method == null)
            {
                throw new SecurityException("Cant find " + methodname);
            }
            return method;
        }

        [SecuritySafeCritical]
        public static Assembly LoadAndVerifyAssembly(byte[] assemblyData)
        {
            if (Application.GetBuildUnityVersion() >= Application.GetNumericUnityVersion("4.5.0a4"))
            {
                Debug.LogError("Unable to verify assembly data; you must provide an authorization key when loading this assembly.");
                return null;
            }
            return LoadAndVerifyAssemblyInternal(assemblyData);
        }

        [SecuritySafeCritical]
        public static Assembly LoadAndVerifyAssembly(byte[] assemblyData, string authorizationKey)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString("<RSAKeyValue><Modulus>uP7lsvrE6fNoQWhUIdJnQrgKoGXBkgWgs5l1xmS9gfyNkFSXgugIpfmN/0YrtL57PezYFXN0CogAnOpOtcUmpcIrh524VL/7bIh+jDUaOCG292PIx92dtzqCTvbUdCYUmaag9VlrdAw05FxYQJi2iZ/X6EiuO1TnqpVNFCDb6pXPAssoO4Uxn9JXBzL0muNRdcmFGRiLp7JQOL7a2aeU9mF9qjMprnww0k8COa6tHdnNWJqaxdFO+Etk3os0ns/gQ2FWrztKemM1Wfu7lk/B1F+V2g0adwlTiuyNHw6to+5VQXWK775RXB9wAGr8KhsVD5IJvmxrdBT8KVEWve+OXQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            bool flag = false;
            byte[] rgbHash = SHA1.Create().ComputeHash(assemblyData);
            byte[] rgbSignature = Convert.FromBase64String(authorizationKey);
            try
            {
                flag = provider.VerifyHash(rgbHash, CryptoConfig.MapNameToOID("SHA1"), rgbSignature);
            }
            catch (CryptographicException)
            {
                Debug.LogError("Unable to verify that this assembly has been authorized by Unity.  The assembly will not be loaded.");
                flag = false;
            }
            if (!flag)
            {
                return null;
            }
            return LoadAndVerifyAssemblyInternal(assemblyData);
        }

        [SecuritySafeCritical]
        private static Assembly LoadAndVerifyAssemblyInternal(byte[] assemblyData)
        {
            Assembly item = Assembly.Load(assemblyData);
            byte[] publicKey = item.GetName().GetPublicKey();
            if ((publicKey == null) || (publicKey.Length == 0))
            {
                return null;
            }
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.ImportCspBlob(publicKey);
            StrongName name = new StrongName(provider);
            using (MemoryStream stream = new MemoryStream(assemblyData))
            {
                if (name.Verify(stream))
                {
                    _verifiedAssemblies.Add(item);
                    return item;
                }
                return null;
            }
        }

        [ExcludeFromDocs]
        public static bool PrefetchSocketPolicy(string ip, int atPort)
        {
            int timeout = 0xbb8;
            return PrefetchSocketPolicy(ip, atPort, timeout);
        }

        public static bool PrefetchSocketPolicy(string ip, int atPort, [DefaultValue("3000")] int timeout)
        {
            object[] parameters = new object[] { ip, atPort, timeout };
            return (bool) GetUnityCrossDomainHelperMethod("PrefetchSocketPolicy").Invoke(null, parameters);
        }

        internal static string TokenToHex(byte[] token)
        {
            if ((token == null) || (8 > token.Length))
            {
                return string.Empty;
            }
            object[] args = new object[] { token[0], token[1], token[2], token[3], token[4], token[5], token[6], token[7] };
            return string.Format("{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}{6:x2}{7:x2}", args);
        }

        internal static bool VerifySignature(string file, byte[] publicKey)
        {
            try
            {
                string path = file + kSignatureExtension;
                if (!File.Exists(path))
                {
                    return false;
                }
                using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
                {
                    provider.ImportCspBlob(publicKey);
                    using (SHA1CryptoServiceProvider provider2 = new SHA1CryptoServiceProvider())
                    {
                        return provider.VerifyData(File.ReadAllBytes(file), provider2, File.ReadAllBytes(path));
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            return false;
        }
    }
}

