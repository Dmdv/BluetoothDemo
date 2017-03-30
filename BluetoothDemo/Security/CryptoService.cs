using BluetoothDemo.Extensions;
using JetBrains.Annotations;
using System;
using System.IO;
using System.Security.Cryptography;

namespace BluetoothDemo.Security
{
    [UsedImplicitly]
    internal class CryptoService : ICryptoService
    {
        private const int IterationCount = 1000;
        private readonly IKeyProvider _keyProvider;
        private readonly ILog _logger;
        private readonly ISecureStorageProvider _secureStorage;

        public CryptoService(
            [NotNull] IKeyProvider keyProvider,
            [NotNull] ISecureStorageProvider secureStorage,
            [NotNull] ILog logger)
        {
            _keyProvider = keyProvider;
            _secureStorage = secureStorage;
            _logger = logger;

            var rsa = new RSACryptoServiceProvider(2048);

            var publicKey = rsa.ExportParameters(false);
            var privateKey = rsa.ExportParameters(true);

            // TODO: check if keys exist
            _secureStorage.Save("public", publicKey.ToBinary());
            _secureStorage.Save("private", privateKey.ToBinary());
        }

        public void Encrypt(string key, byte[] value)
        {
            var salt = Generate256BitsOfRandomEntropy();
            var generatedKey = _keyProvider.GetKey();

            using (var rfc = new Rfc2898DeriveBytes(generatedKey, salt, IterationCount))
            {
                var aes = new AesManaged
                {
                    KeySize = 256,
                    BlockSize = 128,
                    Mode = CipherMode.CBC
                };

                var keyBytes = rfc.GetBytes(aes.KeySize/8);
                var ivBytes = rfc.GetBytes(aes.BlockSize/8);

                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (var stream = new MemoryStream())
                {
                    using (aes)
                    {
                        using (var cryptoStream = new CryptoStream(
                            stream,
                            aes.CreateEncryptor(),
                            CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(value, 0, value.Length);
                            cryptoStream.FlushFinalBlock();
                            cryptoStream.Close();
                        }

                        rfc.Reset();
                    }

                    try
                    {
                        var rsa = new RSACryptoServiceProvider(2048);
                        rsa.ImportParameters(_secureStorage.ReadPublic().FromBinary());

                        var storage = new Storage
                        {
                            Data = stream.ToArray(),
                            Salt = salt,
                            Key = generatedKey,
                            EncryptedKey = rsa.Encrypt(keyBytes, RSAEncryptionPadding.Pkcs1),
                            EncryptedIv = rsa.Encrypt(ivBytes, RSAEncryptionPadding.Pkcs1)
                        };

                        _secureStorage.Save(key, storage.ToBinary());
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(ex.ToString());
                    }
                }
            }
        }

        public byte[] Decrypt(string key)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider(2048);
                rsa.ImportParameters(_secureStorage.ReadPrivate().FromBinary());

                var storage = _secureStorage.Read(key).FromBinary<Storage>();

                var decryptedKey = rsa.Decrypt(storage.EncryptedKey, RSAEncryptionPadding.Pkcs1);
                var decryptedIv = rsa.Decrypt(storage.EncryptedIv, RSAEncryptionPadding.Pkcs1);

                var aes = new AesManaged
                {
                    KeySize = 256,
                    BlockSize = 128,
                    Mode = CipherMode.CBC,
                    Key = decryptedKey,
                    IV = decryptedIv
                };

                using (var stream = new MemoryStream())
                {
                    using (aes)
                    {
                        using (var cryptoStream = new CryptoStream(
                            stream,
                            aes.CreateDecryptor(),
                            CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(storage.Data, 0, storage.Data.Length);
                            cryptoStream.FlushFinalBlock();
                            cryptoStream.Close();
                        }
                    }


                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
            }

            return null;
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