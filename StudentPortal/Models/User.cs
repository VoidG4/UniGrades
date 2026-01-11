using System;
using System.Collections.Generic;

namespace StudentPortal.Models;

public partial class User
{
    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Professor> Professors { get; set; } = new List<Professor>();

    public virtual ICollection<Secretary> Secretaries { get; set; } = new List<Secretary>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
