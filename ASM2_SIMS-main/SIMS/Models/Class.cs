using System;
using System.Collections.Generic;

namespace SIMS.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string? ClassName { get; set; }

    public int? CourseId { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
