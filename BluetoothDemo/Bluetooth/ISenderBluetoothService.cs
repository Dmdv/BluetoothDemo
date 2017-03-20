using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BluetoothDemo.Bluetooth
{
    public interface ISenderBluetoothService
    {
        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <returns>The list of the devices.</returns>
        Task<List<Device>> GetDevices();

        /// <summary>
        /// Sends the data to the Receiver.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="content">The content.</param>
        /// <returns>If was sent or not.</returns>
        Task<bool> Send([NotNull] Device device, [NotNull] string content);
    }
}