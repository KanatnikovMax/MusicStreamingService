using System.ComponentModel.DataAnnotations;

namespace MusicStreamingService.DataAccess.Entities;

public abstract class BaseEntity
{
    [Key]
    public virtual Guid Id { get; set; }
}