using System;

namespace BluetoothDemo.Security
{
    [Serializable]
    public class Storage
    {
        public byte[] Key { get; set; }

        public byte[] Salt { get; set; }

        public byte[] Data { get; set; }

        public byte[] EncryptedKey { get; set; }

        public byte[] EncryptedIv { get; set; }
    }
}