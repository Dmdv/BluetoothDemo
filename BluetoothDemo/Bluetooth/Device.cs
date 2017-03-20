using InTheHand.Net.Sockets;
using JetBrains.Annotations;
using MugenMvvmToolkit.ViewModels;
using PropertyChanged;
using System;
using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace BluetoothDemo.Bluetooth
{
    [ImplementPropertyChanged]
    public sealed class Device : ViewModelBase, IEqualityComparer<Device>
    {
        public Device([NotNull] BluetoothDeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;
            IsAuthenticated = deviceInfo.Authenticated;
            IsConnected = deviceInfo.Connected;
            DeviceName = deviceInfo.DeviceName;
            LastSeen = deviceInfo.LastSeen;
            LastUsed = deviceInfo.LastUsed;
            Nap = deviceInfo.DeviceAddress.Nap;
            Sap = deviceInfo.DeviceAddress.Sap;
            Remembered = deviceInfo.Remembered;
        }

        public string DeviceName { get; set; }

        public bool IsAuthenticated { get; set; }

        public bool IsConnected { get; set; }

        public ushort Nap { get; set; }

        public uint Sap { get; set; }

        public DateTime LastSeen { get; set; }

        public DateTime LastUsed { get; set; }

        public bool Remembered { get; set; }

        public BluetoothDeviceInfo DeviceInfo { get; set; }

        public bool IsSelected { get; set; }

        public bool Equals(Device x, Device y)
        {
            return x.DeviceName.Equals(y.DeviceName);
        }

        public int GetHashCode(Device obj)
        {
            return obj.DeviceName.GetHashCode();
        }

        public override string ToString()
        {
            return DeviceName;
        }
    }
}