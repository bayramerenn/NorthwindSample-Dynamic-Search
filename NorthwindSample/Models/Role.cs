﻿using System;
using System.Collections.Generic;

namespace NorthwindSample.Models;

public partial class Role
{
    public int Id { get; set; }

    public long? RoleId { get; set; }

    public string RoleName { get; set; }

    public int? GroupId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual RoleGroup Group { get; set; }
}
