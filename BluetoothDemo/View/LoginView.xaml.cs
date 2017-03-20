using BluetoothDemo.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace BluetoothDemo.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var loginViewModel = DataContext as LoginViewModel;

            if (loginViewModel != null)
            {
                loginViewModel.Token = (sender as PasswordBox)?.SecurePassword;
            }
        }
    }
}