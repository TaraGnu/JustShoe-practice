using JustShop.ViewModels;
using JustShop.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JustShop
{
    public partial class MainWindow : Window
    {
        private readonly UserSession _session = new();

        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigated += (_, e) =>
            {
                if (e.Content is Page page)
                    Title = $"{page.Title} — Магазин обуви";
            };

            try
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/Icon.ico"));
            }
            catch
            {
            }

            MainFrame.Navigate(new LoginPage(_session));
        }
    }
}