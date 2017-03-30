using JetBrains.Annotations;
using System;
using System.IO;

namespace BluetoothDemo.Security
{
    [UsedImplicitly]
    internal class SecureStorageProvider : ISecureStorageProvider
    {
        public void Save(string key, byte[] stream)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var combine = Path.Combine(folderPath, $"{key}.bin");

            File.WriteAllBytes(combine, stream);
        }

        public byte[] Read(string key)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var combine = Path.Combine(folderPath, $"{key}.bin");

            return File.ReadAllBytes(combine);
        }

        public byte[] ReadPublic()
        {
            return Read("public");
        }

        public byte[] ReadPrivate()
        {
            return Read("private");
        }
    }
}