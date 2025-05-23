﻿using System.ComponentModel.DataAnnotations;

namespace MusicStreamingService.DataAccess.Postgres.Entities;

public abstract class BaseEntity
{
    [Key]
    public virtual Guid Id { get; set; }
    public DateTime CreatedAt{ get; set; }
}