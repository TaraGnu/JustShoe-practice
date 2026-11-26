namespace JustShop.ViewModels
{
    public class OrderRowViewModel
    {
        public int Id { get; init; }
        public DateTime DateOrder { get; init; }
        public string UserFullName { get; init; } = "";
        public string StatusName { get; init; } = "";
        public string PickUpPoint { get; init; } = "";
        public double TotalPrice { get; init; }
        public int Code { get; init; }
    }
}