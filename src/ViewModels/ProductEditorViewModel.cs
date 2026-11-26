using JustShop.Commands;
using JustShop.Models;
using JustShop.Services;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace JustShop.ViewModels
{
    public class ProductEditorViewModel : ObservableObject
    {
        private readonly UserSession _session;
        private readonly int _productId;
        private Product? _original;

        private string _imageFileName = string.Empty;
        private string _pendingImageSource = string.Empty;

        public ProductEditorViewModel(int productId, UserSession session)
        {
            _session = session;
            _productId = productId;

            Categories = ProductService.GetCategories();
            Fabricators = ProductService.GetFabricators();
            Suppliers = ProductService.GetSuppliers();
            ShoesTypes = ProductService.GetShoesTypes();

            SaveCommand = new RelayCommand(_ => Save());
            DeleteCommand = new RelayCommand(_ => Delete(), _ => IsEditMode && IsAdmin);
            PickImageCommand = new RelayCommand(_ => PickImage());

            if (IsEditMode)
                LoadProduct();
            else
                PrepareNew();
        }

        public bool IsAdmin => _session.IsAdmin;
        public bool IsEditMode => _productId > 0;
        public string HeaderText => IsEditMode
            ? $"Редактирование товара №{_productId}"
            : "Добавление товара";

        public List<Category> Categories { get; }
        public List<Fabricator> Fabricators { get; }
        public List<Supplier> Suppliers { get; }
        public List<ShoesType> ShoesTypes { get; }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand PickImageCommand { get; }

        // ---- Свойства формы ----

        private int _id;
        public int Id
        {
            get => _id;
            private set => SetProperty(ref _id, value);
        }

        private string _articul = string.Empty;
        public string Articul
        {
            get => _articul;
            set => SetProperty(ref _articul, value);
        }

        private Category? _selectedCategory;
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private ShoesType? _selectedShoesType;
        public ShoesType? SelectedShoesType
        {
            get => _selectedShoesType;
            set => SetProperty(ref _selectedShoesType, value);
        }

        private Fabricator? _selectedFabricator;
        public Fabricator? SelectedFabricator
        {
            get => _selectedFabricator;
            set => SetProperty(ref _selectedFabricator, value);
        }

        private Supplier? _selectedSupplier;
        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _units = "шт";
        public string Units
        {
            get => _units;
            set => SetProperty(ref _units, value);
        }

        private string _priceText = "0";
        public string PriceText
        {
            get => _priceText;
            set => SetProperty(ref _priceText, value);
        }

        private string _quantityText = "0";
        public string QuantityText
        {
            get => _quantityText;
            set => SetProperty(ref _quantityText, value);
        }

        private string _discountText = "0";
        public string DiscountText
        {
            get => _discountText;
            set => SetProperty(ref _discountText, value);
        }

        // Отображаемый путь к фото (текущий из БД или только что выбранный)
        public string ImagePath
        {
            get
            {
                var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                if (!string.IsNullOrWhiteSpace(_pendingImageSource) && File.Exists(_pendingImageSource))
                    return _pendingImageSource;

                if (!string.IsNullOrWhiteSpace(_imageFileName))
                {
                    var full = Path.Combine(baseDir, _imageFileName);
                    if (File.Exists(full))
                        return full;
                }

                return Path.Combine(baseDir, "picture.png");
            }
        }

        public bool IsImageSelected => !string.IsNullOrWhiteSpace(_pendingImageSource);

        // ---- Загрузка / подготовка ----

        private void LoadProduct()
        {
            _original = ProductService.GetById(_productId);
            if (_original is null)
            {
                MessageBox.Show("Товар не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Id = _original.Id;
            Articul = _original.Articul;
            Description = _original.Description ?? string.Empty;
            Units = _original.Units;
            PriceText = _original.Price.ToString("0.##");
            QuantityText = _original.Quantity.ToString();
            DiscountText = (_original.Discount ?? 0).ToString("0.##");
            _imageFileName = _original.Image ?? string.Empty;

            SelectedCategory = Categories.FirstOrDefault(c => c.Id == _original.IdCategory);
            SelectedShoesType = ShoesTypes.FirstOrDefault(t => t.Id == _original.IdShoesType);
            SelectedFabricator = Fabricators.FirstOrDefault(f => f.Id == _original.IdFabricator);
            SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == _original.IdSupplier);

            OnPropertyChanged(nameof(ImagePath));
        }

        private void PrepareNew()
        {
            Id = ProductService.GetNextId();
            SelectedCategory = Categories.FirstOrDefault();
            SelectedShoesType = ShoesTypes.FirstOrDefault();
            SelectedFabricator = Fabricators.FirstOrDefault();
            SelectedSupplier = Suppliers.FirstOrDefault();
        }

        // ---- Фото ----

        private void PickImage()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp",
                Title = "Выберите изображение товара"
            };

            if (dialog.ShowDialog() != true)
                return;

            // Проверка размера файла (не более 5 МБ)
            var info = new FileInfo(dialog.FileName);
            if (info.Length > 5 * 1024 * 1024)
            {
                MessageBox.Show("Файл слишком большой. Максимум 5 МБ.",
                    "Слишком большой файл", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _pendingImageSource = dialog.FileName;
            OnPropertyChanged(nameof(ImagePath));
            OnPropertyChanged(nameof(IsImageSelected));
        }

        // ---- Сохранение ----

        private void Save()
        {
            if (!IsAdmin)
            {
                MessageBox.Show("Добавлять и редактировать товары может только администратор.",
                    "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Validate(out float price, out int quantity, out float discount))
                return;

            try
            {
                // Работа с фото
                string? newFileName = _imageFileName;

                if (!string.IsNullOrWhiteSpace(_pendingImageSource))
                {
                    // Удаляем старое фото, если было
                    if (!string.IsNullOrWhiteSpace(_imageFileName))
                        ProductService.DeleteImage(_imageFileName);

                    newFileName = ProductService.SaveImage(_pendingImageSource, Id);
                }

                var product = new Product
                {
                    Id = Id,
                    Articul = Articul.Trim(),
                    IdCategory = SelectedCategory!.Id,
                    IdShoesType = SelectedShoesType!.Id,
                    IdFabricator = SelectedFabricator!.Id,
                    IdSupplier = SelectedSupplier!.Id,
                    Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    Units = Units.Trim(),
                    Price = price,
                    Quantity = quantity,
                    Discount = discount,
                    Image = newFileName
                };

                ProductService.Save(product);

                MessageBox.Show("Товар сохранён.", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Возвращаемся в каталог
                ReturnToCatalog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось сохранить товар.\n\n{ex.Message}",
                    "Ошибка сохранения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Validate(out float price, out int quantity, out float discount)
        {
            price = 0; quantity = 0; discount = 0;

            if (!IsAdmin)
            {
                MessageBox.Show("Редактирование доступно только администратору.",
                    "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Articul))
            {
                MessageBox.Show("Поле «Артикул (наименование)» обязательно для заполнения.",
                    "Не заполнено поле", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedCategory is null || SelectedShoesType is null ||
                SelectedFabricator is null || SelectedSupplier is null)
            {
                MessageBox.Show("Заполните категорию, вид обуви, производителя и поставщика.",
                    "Не заполнены поля", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Units))
            {
                MessageBox.Show("Укажите единицу измерения.",
                    "Не заполнено поле", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!float.TryParse(PriceText, out price) || price < 0)
            {
                MessageBox.Show("Цена должна быть неотрицательным числом. Допустимы сотые доли.",
                    "Некорректная цена", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(QuantityText, out quantity) || quantity < 0)
            {
                MessageBox.Show("Количество на складе не может быть отрицательным.",
                    "Некорректное количество", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!float.TryParse(DiscountText, out discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Скидка должна быть числом от 0 до 100 (в процентах).",
                    "Некорректная скидка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // ---- Удаление ----

        private void Delete()
        {
            if (!IsAdmin || !IsEditMode)
                return;

            if (ProductService.IsProductInOrders(Id))
            {
                MessageBox.Show(
                    "Товар присутствует в заказе и не может быть удалён.\n" +
                    "Сначала удалите все позиции заказов с этим товаром.",
                    "Удаление невозможно", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить товар «{Articul}»?\nЭто действие необратимо.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                // Удаляем фото товара из папки
                if (_original is not null)
                    ProductService.DeleteImage(_original.Image);

                ProductService.Delete(Id);

                MessageBox.Show("Товар удалён.", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                ReturnToCatalog();
            }
            catch (Exception ex)
            {

                MessageBox.Show(
                    $"Не удалось удалить товар.\n\n{ex.Message}",
                    "Ошибка удаления",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Возврат в каталог: подписываемся в Page
        public event EventHandler? RequestGoToCatalog;

        private void ReturnToCatalog() => RequestGoToCatalog?.Invoke(this, EventArgs.Empty);
    }
}