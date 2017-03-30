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

            OpenWindowCommand = new AsyncRelayCommand(ExecuteOpenWindow);
            SaveCredsCommand = new RelayCommand(ExecuteSaveCreds);
            RestoreCredsCommand = new RelayCommand(ExecuteRestoreCreds);
        }

        public RelayCommand OpenWindowCommand { get; private set; }

        public RelayCommand SaveCredsCommand { get; private set; }

        public RelayCommand RestoreCredsCommand { get; set; }

        [DoNotNotify]
        public SecureString Token { get; set; }

        public string LoginName { get; set; }

        public string ErrorText { get; private set; }

        public bool IsError { get; private set; }

        public string RestoredLogin { get; set; }

        public string RestoredPassword { get; set; }

        private void ExecuteRestoreCreds()
        {
            RestoreCreds();
        }

        private void ExecuteSaveCreds()
        {
            if (string.IsNullOrEmpty(LoginName) || (Token?.Length).GetValueOrDefault() == 0)
            {
                ErrorText = "Please, enter login and name";
                IsError = true;
                return;
            }

            IsError = false;

            SaveCredentials();
        }

        private async Task ExecuteOpenWindow()
        {
            GetViewModel<MainViewModel>()
                .ShowAsync(NavigationConstants.IsDialog.ToValue(false));

            await CloseAsync();
        }

        private void SaveCredentials()
        {
            var buffer = new UTF8Encoding(false).GetBytes(LoginName);

            _securityProvider.Save("login", buffer);

            using (var wrapper = new SecureStringWrapper(Token, Encoding.UTF8))
            {
                _securityProvider.Save("token", wrapper.ToByteArray());
            }

            _log.Log("Credential are saved");
        }

        private void RestoreCreds()
        {
            var restore = _securityProvider.Restore("token");
            if (restore != null)
            {
                RestoredPassword = Encoding.UTF8.GetString(restore);
            }

            var restore2 = _securityProvider.Restore("login");
            if (restore2 != null)
            {
                RestoredLogin = Encoding.UTF8.GetString(restore2);
            }
        }
    }
}