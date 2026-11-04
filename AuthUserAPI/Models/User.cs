using System;
using System.Collections.Generic;

namespace AuthUserAPI.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? RegTime { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
