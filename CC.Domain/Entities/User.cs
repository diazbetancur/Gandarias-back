using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public string JobTitle { get; set; }
    public DateTime HireDate { get; set; }
    public Guid? HireTypeId { get; set; }
    public virtual HireType? HireType { get; set; }
}