using System;
using System.Collections.Generic;

namespace StudentPortal.Models;

public partial class Student
{
    public int RegistrationNumber { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Department { get; set; }

    public string UsersUsername { get; set; } = null!;
    public int Semester { get; set; }

    public virtual ICollection<CourseHasStudent> CourseHasStudents { get; set; } = new List<CourseHasStudent>();

    public virtual User UsersUsernameNavigation { get; set; } = null!;
}
