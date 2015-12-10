namespace UnityEngine.Networking.Types
{
    using System;

    public class NetworkAccessToken
    {
        public byte[] array;
        private const int NETWORK_ACCESS_TOKEN_SIZE = 0x40;

        public NetworkAccessToken()
        {
            this.array = new byte[0x40];
        }

        public NetworkAccessToken(byte[] array)
        {
            this.array = array;
        }

        public NetworkAccessToken(string strArray)
        {
            this.array = Convert.FromBase64String(strArray);
        }

        public string GetByteString()
        {
            return Convert.ToBase64String(this.array);
        }

        public bool IsValid()
        {
            if ((this.array == null) || (this.array.Length != 0x40))
            {
                return false;
            }
            byte[] array = this.array;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

