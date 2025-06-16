using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    [Required(ErrorMessage = "Username is require")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Password is require")]
    public string? Password { get; set; }

    public int? RoleId { get; set; }

    public virtual Role? Role { get; set; }
}
