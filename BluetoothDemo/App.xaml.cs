using Autofac;
using BluetoothDemo.Monads;
using BluetoothDemo.ViewModel;
using MugenMvvmToolkit;
using MugenMvvmToolkit.WPF.Infrastructure;
using System.Windows;

namespace BluetoothDemo
{
    public partial class App
    {
        private readonly AutofacContainer _container;

        public App()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<Module>();

            _container = new AutofacContainer(containerBuilder);

            // ReSharper disable once UnusedVariable
            var bootstrapper = new Bootstrapper<LoginViewModel>(this, _container);
            
            //bootstrapper.ShutdownOnMainViewModelClose = false;
            //Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //var mainViewModel = _container.Get<LoginViewModel>();
            //mainViewModel.ShowAsync();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _container.If(container => container != null && !container.IsDisposed).Dispose();
        }
    }
}