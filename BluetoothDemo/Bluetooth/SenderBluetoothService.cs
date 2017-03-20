using InTheHand.Net;
using InTheHand.Net.Sockets;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDemo.Bluetooth
{
    [UsedImplicitly]
    public sealed class SenderBluetoothService : ISenderBluetoothService
    {
        private readonly ILog _log;
        private readonly Guid _serviceClassId;

        public SenderBluetoothService([NotNull] ILog log)
        {
            _log = log;
            _serviceClassId = new Guid("9bde4762-89a6-418e-bacf-fcd82f1e0677");
        }

        public Task<List<Device>> GetDevices()
        {
            return Task.Run(() =>
            {
                using (var bluetoothClient = new BluetoothClient())
                {
                    _log.Log("Bluetooth client created");

                    var deviceInfos = bluetoothClient.DiscoverDevices();

                    _log.Log($"Found: {deviceInfos.Length} devices");

                    return deviceInfos
                        .Select(deviceInfo => new Device(deviceInfo))
                        .ToList();
                }
            });
        }

        /// <summary>
        /// Sends the data to the Receiver.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="content">The content.</param>
        /// <returns>If was sent or not.</returns>
        public Task<bool> Send(Device device, string content)
        {
            Contract.Assert(!string.IsNullOrEmpty(content));

            return Task.Run(() =>
            {
                using (var bluetoothClient = new BluetoothClient())
                {
                    try
                    {
                        var ep = new BluetoothEndPoint(device.DeviceInfo.DeviceAddress, _serviceClassId);

                        bluetoothClient.Connect(ep);

                        // get stream for send the data
                        var bluetoothStream = bluetoothClient.GetStream();

                        // if all is ok to send
                        if (bluetoothClient.Connected && bluetoothStream != null)
                        {
                            // write the data in the stream
                            var buffer = Encoding.UTF8.GetBytes(content);
                            bluetoothStream.Write(buffer, 0, buffer.Length);
                            bluetoothStream.Flush();
                            bluetoothStream.Close();
                            return true;
                        }
                        return false;
                    }
                    catch
                    {
                        // the error will be ignored and the send data will report as not sent
                        // for understood the type of the error, handle the exception
                    }
                }
                return false;
            });
        }
    }
}