using System.ComponentModel.DataAnnotations;

namespace CC.Domain.Entities;

public class EntityBase<T>
{
    /// <summary>
    /// Id model
    /// </summary>
    [Key]
    public required T Id { get; set; }
    
    /// <summary>
    /// Date created
    /// </summary>
    public DateTime DateCreated { get; set; }
}