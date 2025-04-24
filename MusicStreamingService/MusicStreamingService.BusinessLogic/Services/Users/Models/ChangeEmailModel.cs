namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public record ChangeEmailModel(
    string Email,
    string Password);