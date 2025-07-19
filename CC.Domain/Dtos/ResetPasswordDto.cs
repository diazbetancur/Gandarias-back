namespace CC.Domain.Dtos;

public class ResetPasswordDto
{
    public string username { get; set; }
    public string token { get; set; }
    public string newPassword { get; set; }
}