using JustShop.Commands;
using JustShop.Services;
using System.Collections.ObjectModel;

namespace JustShop.ViewModels
{
    public class CatalogViewModel : ObservableObject
    {
        private const string AllSuppliers = "Все поставщики";
        private const string AllCategories = "Все категории";
        private const string AllShoesTypes = "Все виды обуви";

        private readonly List<ShoeCardViewModel> _allCards = new();
        private readonly UserSession _session;

        private string _searchText = string.Empty;
        private string _selectedSupplier = AllSuppliers;
        private string _selectedCategory = AllCategories;
        private string _selectedShoesType = AllShoesTypes;
        private string _selectedSort = "По умолчанию";

        public bool IsAdmin => _session.IsAdmin;
        public bool CanFilter => _session.IsStaff;
        public CatalogViewModel(UserSession session)
        {
            _session = session;
            _allCards.AddRange(CatalogService.GetCatalog());

            Suppliers = _allCards.Select(c => c.SupplierName)
                                 .Distinct().OrderBy(s => s).ToList();
            Suppliers.Insert(0, AllSuppliers);

            Categories = _allCards.Select(c => c.CategoryName)
                                  .Distinct().OrderBy(s => s).ToList();
            Categories.Insert(0, AllCategories);

            ShoesTypes = _allCards.Select(c => c.ShoesTypeName)
                                  .Distinct().OrderBy(s => s).ToList();
            ShoesTypes.Insert(0, AllShoesTypes);

            ApplyFilters();
        }

        public ObservableCollection<ShoeCardViewModel> Cards { get; } = new();

        public List<string> Suppliers { get; }
        public List<string> Categories { get; }
        public List<string> ShoesTypes { get; }

        public List<string> SortOptions { get; } = new()
        {
            "По умолчанию",
            "Количество: по возрастанию",
            "Количество: по убыванию"
        };

        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) ApplyFilters(); }
        }

        public string SelectedSupplier
        {
            get => _selectedSupplier;
            set { if (SetProperty(ref _selectedSupplier, value)) ApplyFilters(); }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set { if (SetProperty(ref _selectedCategory, value)) ApplyFilters(); }
        }

        public string SelectedShoesType
        {
            get => _selectedShoesType;
            set { if (SetProperty(ref _selectedShoesType, value)) ApplyFilters(); }
        }

        public string SelectedSort
        {
            get => _selectedSort;
            set { if (SetProperty(ref _selectedSort, value)) ApplyFilters(); }
        }

        private void ApplyFilters()
        {
            IEnumerable<ShoeCardViewModel> query = _allCards;

            // Фильтр по поставщику. «Все поставщики» = сброс фильтра
            if (SelectedSupplier != AllSuppliers)
                query = query.Where(c => c.SupplierName == SelectedSupplier);

            if (SelectedCategory != AllCategories)
                query = query.Where(c => c.CategoryName == SelectedCategory);

            if (SelectedShoesType != AllShoesTypes)
                query = query.Where(c => c.ShoesTypeName == SelectedShoesType);

            // Поиск по всем текстовым полям
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim();
                query = query.Where(c =>
                    c.SearchBlob.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            // Сортировка сохраняется и после фильтра/поиска
            query = SelectedSort switch
            {
                "Количество: по возрастанию" => query.OrderBy(c => c.Quantity),
                "Количество: по убыванию" => query.OrderByDescending(c => c.Quantity),
                _ => query
            };

            Cards.Clear();
            foreach (var card in query)
                Cards.Add(card);
        }
    }
}
