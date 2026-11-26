using JustShop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace JustShop.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private readonly UserSession _session;
        private readonly LoginViewModel _viewModel;

        public LoginPage(UserSession session)
        {
            InitializeComponent();
            _session = session;
            _viewModel = new LoginViewModel(session);
            DataContext = _viewModel;
            TopBar.Session = session;
        }

        private void TopBar_CatalogRequested(object sender, EventArgs e) =>
            NavigationService.Navigate(new CatalogPage(_session));

        private void LoginButton_Click(object sender, RoutedEventArgs e) => TryLogin();

        private void LoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordBox.Focus();
                e.Handled = true;
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryLogin();
                e.Handled = true;
            }
        }

        private void TryLogin()
        {
            if (!_viewModel.Login(PasswordBox.Password))
                return;

            NavigationService.Navigate(new CatalogPage(_session));
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            _session.SignOut();
            NavigationService.Navigate(new CatalogPage(_session));
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }
    }
}
