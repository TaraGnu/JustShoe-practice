using JustShop.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JustShop.Services
{
    public static class ProductService
    {
        public static List<Category> GetCategories()
        {
            using var context = new ShoesShopContext();
            return context.Categories.AsNoTracking().OrderBy(c => c.Name).ToList();
        }

        public static List<Fabricator> GetFabricators()
        {
            using var context = new ShoesShopContext();
            return context.Fabricators.AsNoTracking().OrderBy(f => f.Name).ToList();
        }

        public static List<Supplier> GetSuppliers()
        {
            using var context = new ShoesShopContext();
            return context.Suppliers.AsNoTracking().OrderBy(s => s.Name).ToList();
        }

        public static List<ShoesType> GetShoesTypes()
        {
            using var context = new ShoesShopContext();
            return context.ShoesTypes.AsNoTracking().OrderBy(s => s.Name).ToList();
        }

        public static Product? GetById(int id)
        {
            using var context = new ShoesShopContext();
            return context.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);
        }

        public static int GetNextId()
        {
            using var context = new ShoesShopContext();
            return (context.Products.Max(p => (int?)p.Id) ?? 0) + 1;
        }

        public static bool IsProductInOrders(int productId)
        {
            using var context = new ShoesShopContext();
            return context.OrderItems.Any(oi => oi.IdProduct == productId);
        }

        public static void Save(Product product)
        {
            using var context = new ShoesShopContext();

            if (context.Products.Any(p => p.Id == product.Id))
                context.Products.Update(product);
            else
                context.Products.Add(product);

            context.SaveChanges();
        }

        public static void Delete(int productId)
        {
            using var context = new ShoesShopContext();
            var product = context.Products.FirstOrDefault(p => p.Id == productId)
                ?? throw new InvalidOperationException("Товар не найден.");

            context.Products.Remove(product);
            context.SaveChanges();
        }

        /// Сохраняет изображение в папку Images, уменьшая до 300×200 (пропорционально).
        /// Возвращает имя файла для записи в БД.

        public static string SaveImage(string sourcePath, int productId)
        {
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException("Файл изображения не найден.", sourcePath);

            var imagesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            Directory.CreateDirectory(imagesDir);

            var fileName = $"product_{productId}_{Guid.NewGuid():N}.png";
            var targetPath = Path.Combine(imagesDir, fileName);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(sourcePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            // Пропорциональное уменьшение до вписывания в 300×200, без апскейла
            double scale = Math.Min(300.0 / bitmap.PixelWidth, 200.0 / bitmap.PixelHeight);
            if (scale > 1) scale = 1;

            var transformed = new TransformedBitmap(bitmap, new ScaleTransform(scale, scale));

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(transformed));

            using (var stream = File.Create(targetPath))
                encoder.Save(stream);

            return fileName;
        }

        public static void DeleteImage(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
            if (File.Exists(path))
            {
                try { File.Delete(path); }
                catch { }
            }
        }
    }
}
