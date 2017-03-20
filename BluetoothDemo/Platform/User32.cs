using BluetoothDemo.ViewModel;
using JetBrains.Annotations;
using System.Runtime.InteropServices;

namespace BluetoothDemo.Platform
{
    [UsedImplicitly]
    public class User32 : IUser32
    {
        void IUser32.LockWorkStation()
        {
            LockWorkStation();
        }

        [DllImport("user32")]
        private static extern void LockWorkStation();
    }
}