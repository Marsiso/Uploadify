﻿using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Domain.Data.Contracts;

namespace Uploadify.Server.Domain.Application.Models;

public class RoleClaim : IdentityRoleClaim<string>, IBaseEntity
{
    public bool IsActive { get; set; }
}
