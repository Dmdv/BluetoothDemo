using System;

namespace BluetoothDemo.Bluetooth
{
    public interface IReceiverBluetoothService
    {
        /// <summary>
        /// Gets a value indicating whether was started.
        /// </summary>
        bool WasStarted { get; }

        /// <summary>
        /// Starts the listening from Senders.
        /// </summary>
        void Start(Action<string> reportAction);

        /// <summary>
        /// Stops the listening from Senders.
        /// </summary>
        void Stop();
    }
}