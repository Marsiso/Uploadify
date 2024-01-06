﻿using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Data.Contracts;

namespace Uploadify.Server.Domain.Infrastructure.Data.Models;

public class ChangeTrackingBaseEntity : BaseEntity, IChangeTrackingBaseEntity
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }

    public User? UserCreatedBy { get; set; }
    public User? UserUpdatedBy { get; set; }
}
