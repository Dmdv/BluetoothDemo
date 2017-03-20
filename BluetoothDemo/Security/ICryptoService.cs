using JetBrains.Annotations;

namespace BluetoothDemo.Security
{
    public interface ICryptoService
    {
        void Encrypt([NotNull] string key, [NotNull] byte[] value);
    }
}