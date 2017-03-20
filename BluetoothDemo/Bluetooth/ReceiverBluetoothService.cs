using InTheHand.Net.Sockets;
using JetBrains.Annotations;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BluetoothDemo.Bluetooth
{
    /// <summary>
    /// The Receiver bluetooth service.
    /// </summary>
    [UsedImplicitly]
    public sealed class ReceiverBluetoothService : IDisposable, IReceiverBluetoothService
    {
        private readonly Guid _serviceClassId;
        private Action<string> _responseAction;
        private BluetoothListener _listener;
        private CancellationTokenSource _cancelSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverBluetoothService" /> class.
        /// </summary>
        public ReceiverBluetoothService()
        {
            _serviceClassId = new Guid("9bde4762-89a6-418e-bacf-fcd82f1e0677");
        }

        /// <summary>
        /// Gets or sets a value indicating whether was started.
        /// </summary>
        public bool WasStarted { get; set; }

        /// <summary>
        /// Starts the listening from Senders.
        /// </summary>
        public void Start(Action<string> reportAction)
        {
            WasStarted = true;
            _responseAction = reportAction;
            if (_cancelSource != null && _listener != null)
            {
                Dispose(true);
            }
            _listener = new BluetoothListener(_serviceClassId)
            {
                ServiceName = "MyService"
            };
            _listener.Start();

            _cancelSource = new CancellationTokenSource();

            Task.Run(() => Listener(_cancelSource));
        }

        /// <summary>
        /// Stops the listening from Senders.
        /// </summary>
        public void Stop()
        {
            WasStarted = false;
            _cancelSource.Cancel();
        }

        /// <summary>
        /// Listeners the accept bluetooth client.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        private void Listener(CancellationTokenSource token)
        {
            try
            {
                while (true)
                {
                    using (var client = _listener.AcceptBluetoothClient())
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        using (var streamReader = new StreamReader(client.GetStream()))
                        {
                            try
                            {
                                var content = streamReader.ReadToEnd();
                                if (!string.IsNullOrEmpty(content))
                                {
                                    _responseAction(content);
                                }
                            }
                            catch (IOException)
                            {
                                client.Close();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // todo handle the exception
                // for the sample it will be ignored
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cancelSource != null)
                {
                    _listener.Stop();
                    _listener = null;
                    _cancelSource.Dispose();
                    _cancelSource = null;
                }
            }
        }
    }
}
