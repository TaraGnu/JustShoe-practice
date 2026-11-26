using System;
using System.Collections.Generic;

namespace JustShop.Models;

public partial class PickUpPoint
{
    public int Id { get; set; }

    public int Index { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public int Home { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
