namespace CC.Domain.Dtos;

public class SigningCofigurationDto
{
    public Guid Id { get; set; }
    public string email { get; set; }
    public string? description { get; set; }
    public bool isAdminMail { get; set; }
    public string WorkArea { get; set; }
}