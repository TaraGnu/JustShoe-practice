using JustShop.Models;
using JustShop.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace JustShop.Services
{
    public static class OrderService
    {
        public static List<OrderRowViewModel> GetOrders()
        {
            using var context = new ShoesShopContext();
            return context.Orders
                .Include(o => o.IdUserNavigation)
                .Include(o => o.IdStatusNavigation)
                .Include(o => o.IdPickUpPointNavigation)
                .AsNoTracking()
                .OrderByDescending(o => o.DateOrder)
                .Select(o => new OrderRowViewModel
                {
                    Id = o.Id,
                    DateOrder = o.DateOrder,
                    UserFullName = o.IdUserNavigation.FirstName + " " +
                                   o.IdUserNavigation.Name + " " +
                                   (o.IdUserNavigation.FatherName ?? ""),
                    StatusName = o.IdStatusNavigation.Name,
                    PickUpPoint = $"{o.IdPickUpPointNavigation.City}, " +
                                  $"{o.IdPickUpPointNavigation.Street}, " +
                                  $"{o.IdPickUpPointNavigation.Home}",
                    TotalPrice = o.OrderItems.Sum(oi => oi.Quantity * oi.IdProductNavigation.Price),
                    Code = o.Code
                })
                .ToList();
        }
    }
}