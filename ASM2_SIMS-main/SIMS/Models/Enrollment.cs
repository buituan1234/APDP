using System;
using System.Collections.Generic;

namespace SIMS.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int? StudentId { get; set; }

    public int? ClassId { get; set; }

    public virtual Class? Class { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual Student? Student { get; set; }
}
