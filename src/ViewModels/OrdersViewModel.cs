using JustShop.Commands;
using JustShop.Services;
using System.Collections.ObjectModel;

namespace JustShop.ViewModels
{
    public class OrdersViewModel : ObservableObject
    {
        private readonly UserSession _session;

        public OrdersViewModel(UserSession session)
        {
            _session = session;
            Reload();
        }

        public ObservableCollection<OrderRowViewModel> Orders { get; } = new();
        public bool IsStaff => _session.IsStaff;

        public void Reload()
        {
            Orders.Clear();
            foreach (var order in OrderService.GetOrders())
                Orders.Add(order);
        }
    }
}