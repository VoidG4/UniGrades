using System;
using System.Collections.Generic;

namespace StudentPortal.Models;

public partial class Secretary
{
    public int Phonenumber { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Department { get; set; }

    public string UsersUsername { get; set; } = null!;

    public virtual User UsersUsernameNavigation { get; set; } = null!;
}
