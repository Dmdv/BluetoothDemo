using JetBrains.Annotations;

namespace BluetoothDemo.Security
{
    internal interface ISecureStorageProvider
    {
        void Save([NotNull] string key, [NotNull] byte[] stream);
    }
}