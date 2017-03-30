using System.Security.Cryptography;

namespace BluetoothDemo.Security
{
    public class RsaKeyProvider : IKeyProvider
    {
        RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider(2048);

        public byte[] GetKey()
        {
            throw new System.NotImplementedException();
        }

        public byte[] GetKey(int byteCount)
        {
            throw new System.NotImplementedException();
        }
    }
}