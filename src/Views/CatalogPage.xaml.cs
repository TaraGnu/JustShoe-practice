using JustShop.ViewModels;
using JustShop.Views.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JustShop.Views
{
    public partial class CatalogPage : Page
    {
        private readonly UserSession _session;

        public CatalogPage(UserSession session)
        {
            InitializeComponent();
            _session = session;
            DataContext = new CatalogViewModel(_session);
            TopBar.Session = session;
        }

        private void TopBar_LoginRequested(object sender, EventArgs e) =>
            NavigationService.Navigate(new LoginPage(_session));

        private void TopBar_LogoutRequested(object sender, EventArgs e)
        {
            _session.SignOut();
            NavigationService.Navigate(new CatalogPage(_session));
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }

        private void TopBar_OrdersRequested(object sender, EventArgs e) =>
            NavigationService.Navigate(new OrdersPage(_session));

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!_session.IsAdmin)
            {
                MessageBox.Show("Добавлять товары может только администратор.",
                    "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NavigationService.Navigate(new ProductEditorPage(0, _session));
        }

        private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_session.IsAdmin)
                return;

            if (sender is ShoeCardControl { DataContext: ShoeCardViewModel card })
                NavigationService.Navigate(new ProductEditorPage(card.Id, _session));
        }
    }
}