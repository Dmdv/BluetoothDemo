using BluetoothDemo.Security;
using JetBrains.Annotations;
using MugenMvvmToolkit.DataConstants;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using PropertyChanged;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDemo.ViewModel
{
    [UsedImplicitly]
    [ImplementPropertyChanged]
    internal class LoginViewModel : CloseableViewModel
    {
        private readonly ILog _log;
        private readonly ISecurityProvider _securityProvider;

        public LoginViewModel(
            [NotNull] ISecurityProvider securityProvider,
            [NotNull] ILog log)
        {
            _securityProvider = securityProvider;
            _log = log;

            OpenWindow = new AsyncRelayCommand(ExecuteOpenWindow);
        }

        public RelayCommand OpenWindow { get; private set; }

        [DoNotNotify]
        public SecureString Token { get; set; }

        public string LoginName { get; set; }

        public string ErrorText { get; private set; }

        public bool IsError { get; private set; }

        private async Task ExecuteOpenWindow()
        {
            if (string.IsNullOrEmpty(LoginName) || (Token?.Length).GetValueOrDefault() == 0)
            {
                ErrorText = "Please, enter login and name";
                IsError = true;
                return;
            }

            IsError = false;

            SaveCredentials();

            GetViewModel<MainViewModel>()
                .ShowAsync(NavigationConstants.IsDialog.ToValue(false));

            await CloseAsync();
        }

        private void SaveCredentials()
        {
            var buffer = new UTF8Encoding(false).GetBytes(LoginName);

            _securityProvider.Save("login", buffer);

            using (var wrapper = new SecureStringWrapper(Token))
            {
                _securityProvider.Save("token", wrapper.ToByteArray());
            }

            _log.Log("Credential are saved");
        }
    }
}