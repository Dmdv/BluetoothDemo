using JetBrains.Annotations;
using System.IO;
using System.Security.Cryptography;

namespace BluetoothDemo.Security
{
    [UsedImplicitly]
    internal class CryptoService : ICryptoService
    {
        private const int IterationCount = 1000;
        private readonly IKeyProvider _keyProvider;
        private readonly ISecureStorageProvider _secureStorage;

        public CryptoService(
            [NotNull] IKeyProvider keyProvider,
            [NotNull] ISecureStorageProvider secureStorage)
        {
            _keyProvider = keyProvider;
            _secureStorage = secureStorage;
        }

        public void Encrypt(string key, byte[] value)
        {
            var salt = Generate256BitsOfRandomEntropy();

            using (var rfc = new Rfc2898DeriveBytes(_keyProvider.GetKey(), salt, IterationCount))
            {
                var aes = new AesManaged
                {
                    KeySize = 256,
                    BlockSize = 128,
                    Mode = CipherMode.CBC
                };

                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (var stream = new MemoryStream())
                {
                    using (aes)
                    {
                        using (var encrypt = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            encrypt.Write(value, 0, value.Length);
                            encrypt.FlushFinalBlock();
                            encrypt.Close();
                        }

                        rfc.Reset();
                    }

                    _secureStorage.Save(key, stream.ToArray());
                }
            }
        }

        internal static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                cryptoServiceProvider.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}