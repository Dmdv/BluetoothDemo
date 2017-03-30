using JetBrains.Annotations;

namespace BluetoothDemo.Security
{
    [UsedImplicitly]
    internal class DefaultSecurityProvider : ISecurityProvider
    {
        private readonly ICryptoService _service;

        public DefaultSecurityProvider(ICryptoService service)
        {
            _service = service;
        }

        public void Save(string key, byte[] value)
        {
            _service.Encrypt(key, value);
        }

        public byte[] Restore(string key)
        {
            return _service.Decrypt(key);
        }
    }
}