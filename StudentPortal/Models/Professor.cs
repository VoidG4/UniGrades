using System;
using System.Collections.Generic;

namespace StudentPortal.Models;

public partial class Professor
{
    public int Afm { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Department { get; set; }

    public string UsersUsername { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual User UsersUsernameNavigation { get; set; } = null!;
}
