using System;
using System.Collections.Generic;

namespace SIMS.Models;

public partial class Score
{
    public int ScoreId { get; set; }

    public int? EnrollmentId { get; set; }

    public decimal? Score1 { get; set; }

    public virtual Enrollment? Enrollment { get; set; }
}
