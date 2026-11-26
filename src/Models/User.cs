using System;
using System.Collections.Generic;

namespace JustShop.Models;

public partial class User
{
    public int Id { get; set; }

    public int IdRole { get; set; }

    public string FirstName { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? FatherName { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public override string ToString()
    {
        return string.Join(" ", new[] { FirstName, Name, FatherName }
            .Where(p => !string.IsNullOrWhiteSpace(p)));
    }
}
