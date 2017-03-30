using Autofac;
using BluetoothDemo.Bluetooth;
using BluetoothDemo.Platform;
using BluetoothDemo.Security;

namespace BluetoothDemo
{
    internal class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // services

            builder.RegisterType<MachineKeyProvider>().As<IKeyProvider>();
            builder.RegisterType<CryptoService>().As<ICryptoService>().SingleInstance();
            builder.RegisterType<SecureStorageProvider>().As<ISecureStorageProvider>();
            builder.RegisterType<DefaultSecurityProvider>().As<ISecurityProvider>();

            // bluetooth

            builder.RegisterType<ReceiverBluetoothService>().As<IReceiverBluetoothService>();
            builder.RegisterType<SenderBluetoothService>().As<ISenderBluetoothService>();

            // logger

            builder.RegisterType<FileLogger>().As<ILog>().SingleInstance();

            // User32

            builder.RegisterType<User32>().As<IUser32>();
        }
    }
}