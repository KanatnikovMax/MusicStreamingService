namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public class TokenResponce
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}