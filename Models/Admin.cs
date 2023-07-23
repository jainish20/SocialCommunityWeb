using System;
using System.Collections.Generic;

namespace communityWeb.Models;

public partial class Admin
{
    public int Id { get; set; }

    public string? UName { get; set; }

    public string? Name { get; set; }

    public string? EmailId { get; set; }

    public string? Password { get; set; }

    public bool IsActive { get; set; }
}
