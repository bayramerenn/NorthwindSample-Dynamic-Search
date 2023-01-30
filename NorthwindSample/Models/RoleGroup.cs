using System;
using System.Collections.Generic;

namespace NorthwindSample.Models;

public partial class RoleGroup
{
    public int Id { get; set; }

    public string GroupName { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Role> Roles { get; } = new List<Role>();

    public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
}
