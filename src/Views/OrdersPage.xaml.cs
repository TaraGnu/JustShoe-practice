using JustShop.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace JustShop.Views
{
    public partial class OrdersPage : Page
    {
        private readonly UserSession _session;

        public OrdersPage(UserSession session)
        {
            InitializeComponent();
            _session = session;
            DataContext = new OrdersViewModel(session);
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

        private void TopBar_CatalogRequested(object sender, EventArgs e) =>
            NavigationService.Navigate(new CatalogPage(_session));

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new CatalogPage(_session));
        }
    }
}