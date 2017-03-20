using BluetoothDemo.Bluetooth;
using BluetoothDemo.Platform;
using JetBrains.Annotations;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BluetoothDemo.ViewModel
{
    [UsedImplicitly]
    [ImplementPropertyChanged]
    internal class MainViewModel : CloseableViewModel
    {
        private const int TimeInterval = 10;
        private readonly ILog _log;
        private readonly ISenderBluetoothService _service;
        private readonly IUser32 _user32;
        // ReSharper disable once NotAccessedField.Local
        private Task _initializeDevices;
        private bool _isWarningShown;

        public MainViewModel(
            [NotNull] ISenderBluetoothService service,
            [NotNull] IUser32 user32,
            [NotNull] ILog log)
        {
            _service = service;
            _user32 = user32;
            _log = log;

            WatchCommand = new RelayCommand(ExecuteWatch);
            RemoveWatchCommand = new RelayCommand(ExecuteRemoveWatch);

            StartDiscoverTask();
            StartScanning();
        }

        public bool ControlsEnabled { get; set; }

        public RelayCommand WatchCommand { get; private set; }
        public RelayCommand RemoveWatchCommand { get; private set; }

        [AlsoNotifyFor("Status")]
        public ObservableCollection<Device> Devices { get; set; } = new ObservableCollection<Device>();

        public string Status => $"List of available devices. Found: {Count}";

        [AlsoNotifyFor("Status")]
        private int Count { get; set; }

        public ObservableCollection<Device> WatchedDevices { get; private set; } = new ObservableCollection<Device>();

        private IEnumerable<Device> SelectedDevices
        {
            get { return Devices.Where(x => x.IsSelected); }
        }

        public string Footer { get; private set; }

        private void ExecuteRemoveWatch()
        {
            WatchedDevices.Clear();
        }

        private void ExecuteWatch()
        {
            WatchedDevices = new ObservableCollection<Device>(SelectedDevices.ToArray());
        }

        private void StartScanning()
        {
            Observable
                .Interval(TimeSpan.FromSeconds(TimeInterval))
                .Subscribe(x => StartDiscoverTask());
        }

        private void StartDiscoverTask()
        {
            _initializeDevices = InitializeDevices();
        }

        private async Task InitializeDevices()
        {
            try
            {
                var msg = "Start scan...";
                _log.Log(msg);
                Footer = msg;

                var devices = await _service.GetDevices();

                var message = $"Initialize {devices.Count} devices";
                Footer = message;
                _log.Log(message);

                foreach (var device in devices)
                {
                    msg = $"{device.DeviceName} found";
                    Footer = msg;
                    _log.Log(msg);
                }

                ControlsEnabled = true;
                Devices = new ObservableCollection<Device>(devices);

                Count = Devices.Count;

                message = $"Watched devices count: {WatchedDevices.Count}";
                _log.Log(message);
                Footer = message;

                var outOfRange = WatchedDevices.Except(Devices).ToList();

                message = $"Watched devices count after filtering: {outOfRange.Count}";
                _log.Log(message);
                Footer = message;

                foreach (var device in outOfRange)
                {
                    _log.Log($"Device {device.DeviceName} is out of range");
                }

                if (outOfRange.Count > 0)
                {
                    _log.Log("Lock station");
                    _user32.LockWorkStation();
                }
            }
            catch (PlatformNotSupportedException ex)
            {
                Footer = "Bluetooth is not supported";
                ControlsEnabled = false;

                if (!_isWarningShown)
                {
                    MessageBox.Show(ex.Message, "Error");
                    _isWarningShown = true;
                }
            }
        }
    }
}