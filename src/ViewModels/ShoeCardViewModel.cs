using JustShop.Commands;
using JustShop.Models;
using System.IO;

namespace JustShop.ViewModels
{
    public class ShoeCardViewModel : ObservableObject
    {
        private readonly Product _product;

        public ShoeCardViewModel(Product product)
        {
            _product = product ?? throw new ArgumentNullException(nameof(product));
        }

        public int Id => _product.Id;

        public string Name => _product.Articul;
        public string Articul => _product.Articul;

        public string CategoryName => _product.IdCategoryNavigation?.Name ?? "—";
        public string ShoesTypeName => _product.IdShoesTypeNavigation?.Name ?? "—";

        public string Description =>
            string.IsNullOrWhiteSpace(_product.Description) ? "—" : _product.Description;

        public string FabricatorName => _product.IdFabricatorNavigation?.Name ?? "—";
        public string SupplierName => _product.IdSupplierNavigation?.Name ?? "—";

        public float Price => _product.Price;
        public string Units => _product.Units;
        public int Quantity => _product.Quantity;

        // Discount в процентах (0..100). Если в БД доля (0.15) — поправь ниже.
        public double Discount => _product.Discount ?? 0;
        public bool HasDiscount => Discount > 0;
        public bool IsHighDiscount => Discount > 15;
        public bool IsOutOfStock => Quantity == 0;
        public float PriceWithDiscount =>
            (float)(_product.Price * (1 - Discount / 100.0));

        public string ImagePath
        {
            get
            {
                var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/");
                if (!string.IsNullOrWhiteSpace(_product.Image))
                    if (File.Exists($"{baseDirectory}{_product.Image}"))
                        return Path.Combine(baseDirectory, _product.Image);
                return Path.Combine(baseDirectory, "picture.png");
            }
        }

        // Единая строка для поиска по всем текстовым полям
        public string SearchBlob =>
            string.Join(" ",
                Articul,
                CategoryName,
                ShoesTypeName,
                Description,
                FabricatorName,
                SupplierName,
                Units);
    }
}
