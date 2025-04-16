using Microsoft.AspNetCore.Identity;

namespace CC.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; }
    public string NickName { get; set; }
    public DateTime HireDate { get; set; }
    public Guid? HireTypeId { get; set; }
    public virtual HireType? HireType { get; set; }
    public bool IsDelete { get; set; }
}