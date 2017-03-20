using JetBrains.Annotations;

namespace BluetoothDemo.Security
{
    public interface IKeyProvider
    {
        [NotNull]
        byte[] GetKey();
    }
}