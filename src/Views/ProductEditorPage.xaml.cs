using JustShop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JustShop.Views
{
    public partial class ProductEditorPage : Page
    {
        private readonly UserSession _session;
        private readonly ProductEditorViewModel _viewModel;
        private static bool _isOpen;

        public ProductEditorPage(int productId, UserSession session)
        {
            InitializeComponent();

            if (_isOpen)
            {
                MessageBox.Show(
                    "Окно редактирования уже открыто. Закройте его, чтобы открыть другое.",
                    "Окно уже открыто",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isOpen = true;
            _session = session;

            _viewModel = new ProductEditorViewModel(productId, session);
            _viewModel.RequestGoToCatalog += (_, _) => NavigateToCatalog();
            DataContext = _viewModel;

            TopBar.Session = session;
            Unloaded += (_, _) => _isOpen = false;
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

        private void TopBar_OrdersRequested(object sender, EventArgs e) =>
            NavigationService.Navigate(new OrdersPage(_session));

        private void Back_Click(object sender, RoutedEventArgs e) => NavigateToCatalog();

        private void NavigateToCatalog()
        {
            NavigationService.Navigate(new CatalogPage(_session));
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }

        private void Int_PreviewTextInput(object sender, TextCompositionEventArgs e) =>
            e.Handled = !int.TryParse(e.Text, out _);

        private void Decimal_PreviewTextInput(object sender, TextCompositionEventArgs e) =>
            e.Handled = !float.TryParse(e.Text, out _) && e.Text != ".";
    }
}