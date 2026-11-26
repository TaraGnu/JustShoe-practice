using System;
using System.Collections.Generic;

namespace JustShop.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime DateOrder { get; set; }

    public DateTime DateEnd { get; set; }

    public int IdPickUpPoint { get; set; }

    public int IdUser { get; set; }

    public int Code { get; set; }

    public int IdStatus { get; set; }

    public virtual PickUpPoint IdPickUpPointNavigation { get; set; } = null!;

    public virtual Status IdStatusNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
