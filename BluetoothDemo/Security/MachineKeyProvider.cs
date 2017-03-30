using JetBrains.Annotations;

namespace BluetoothDemo.Security
{
    /// <summary>
    /// Let's pretend this service lives on dedicated server and provides a key to hash user password.
    /// </summary>
    [UsedImplicitly]
    public class MachineKeyProvider : IKeyProvider
    {
        public byte[] GetKey()
        {
            return CryptoService.Generate256BitsOfRandomEntropy();
        }

        public byte[] GetKey(int byteCount)
        {
            throw new System.NotImplementedException();
        }
    }
}