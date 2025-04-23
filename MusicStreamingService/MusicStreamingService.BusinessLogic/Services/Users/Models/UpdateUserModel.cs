namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public record UpdateUserModel(
    string? Email,
    string? UserName,
    string? Password);