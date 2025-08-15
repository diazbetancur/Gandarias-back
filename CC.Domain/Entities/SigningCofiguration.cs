namespace CC.Domain.Entities;

public class SigningCofiguration : EntityBase<Guid>
{
    public string email { get; set; }
    public string? description { get; set; }
    public bool isAdminMail { get; set; }
    public string WorkArea { get; set; }
}