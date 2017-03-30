using JetBrains.Annotations;

namespace BluetoothDemo.Security
{
    internal interface ISecurityProvider
    {
        void Save([NotNull] string key, [NotNull] byte[] value);

        [CanBeNull]
        byte[] Restore([NotNull] string key);
    }
}