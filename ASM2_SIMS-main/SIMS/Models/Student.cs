using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Models;

public partial class Student
{
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Username is require")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Password is require")]
    public string? Password { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Role? Role { get; set; }
}
