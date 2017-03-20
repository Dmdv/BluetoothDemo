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

            File.WriteAllBytes(Path.Combine(folderPath, $"{key}.bin"), stream);
        }
    }
}