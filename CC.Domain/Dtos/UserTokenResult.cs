namespace CC.Domain.Dtos;

public class UserTokenResult
{
    public Guid UserId { get; set; }
    public string EncryptedToken { get; set; }
}