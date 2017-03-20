using JetBrains.Annotations;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BluetoothDemo.Security
{
    public sealed class SecureStringWrapper : IDisposable
    {
        private readonly Encoding _encoding;
        private readonly SecureString _secureString;
        private byte[] _bytes;

        private bool _disposed;

        public SecureStringWrapper([NotNull] SecureString secureString)
            : this(secureString, Encoding.UTF8)
        {
        }

        public SecureStringWrapper([NotNull] SecureString secureString, [NotNull] Encoding encoding)
        {
            _encoding = encoding;
            _secureString = secureString;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Destroy();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        public unsafe byte[] ToByteArray()
        {
            var maxLength = _encoding.GetMaxByteCount(_secureString.Length);

            var bytes = IntPtr.Zero;
            var str = IntPtr.Zero;

            try
            {
                bytes = Marshal.AllocHGlobal(maxLength);
                str = Marshal.SecureStringToBSTR(_secureString);

                var chars = (char*) str.ToPointer();
                var bptr = (byte*) bytes.ToPointer();
                var len = _encoding.GetBytes(chars, _secureString.Length, bptr, maxLength);

                _bytes = new byte[len];
                for (var i = 0; i < len; ++i)
                {
                    _bytes[i] = *bptr;
                    bptr++;
                }

                return _bytes;
            }
            finally
            {
                if (bytes != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(bytes);
                }
                if (str != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(str);
                }
            }
        }

        private void Destroy()
        {
            if (_bytes == null) { return; }

            for (var i = 0; i < _bytes.Length; i++)
            {
                _bytes[i] = 0;
            }
            _bytes = null;
        }

        ~SecureStringWrapper()
        {
            Dispose();
        }
    }
}