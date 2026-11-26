using JustShop.Models;
using JustShop.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace JustShop.Services
{
    public static class CatalogService
    {
        public static List<ShoeCardViewModel> GetCatalog()
        {
            using var context = new ShoesShopContext();

            var products = context.Products
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdFabricatorNavigation)
                .Include(p => p.IdSupplierNavigation)
                .Include(p => p.IdShoesTypeNavigation)
                .AsNoTracking()
                .ToList();

            return products.Select(p => new ShoeCardViewModel(p)).ToList();
        }
    }
}
