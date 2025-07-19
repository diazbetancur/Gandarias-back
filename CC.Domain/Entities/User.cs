using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

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
    public int HiredHours { get; set; } = 0;
    public bool ComplementHours { get; set; } = true;
    public bool LawApply { get; set; } = false;
    public bool ExtraHours { get; set; } = false;
    public int CantPartTimeSchedule { get; set; } = 0;
    public string? PasswordResetToken { get; set; } = null;
    public DateTime? PasswordResetTokenExpiration { get; set; } = null;
    public virtual ICollection<UserWorkstation> UserWorkstations { get; set; } = new List<UserWorkstation>();

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();
}