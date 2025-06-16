using System;
using System.Collections.Generic;

namespace SIMS.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
