using System;
using System.Collections.Generic;

namespace JustShop.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Articul { get; set; } = null!;

    public int IdShoesType { get; set; }

    public float Price { get; set; }

    public int IdSupplier { get; set; }

    public int IdFabricator { get; set; }

    public int IdCategory { get; set; }

    public string Units { get; set; } = null!;

    public float? Discount { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual Fabricator IdFabricatorNavigation { get; set; } = null!;

    public virtual ShoesType IdShoesTypeNavigation { get; set; } = null!;

    public virtual Supplier IdSupplierNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
