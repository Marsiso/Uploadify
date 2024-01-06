﻿using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Domain.Infrastructure.Data.Contracts;

namespace Uploadify.Server.Domain.Application.Models;

public class UserClaim : IdentityUserClaim<string>, IBaseEntity
{
    public bool IsActive { get; set; }
}