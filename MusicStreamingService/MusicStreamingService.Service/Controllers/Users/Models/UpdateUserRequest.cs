namespace MusicStreamingService.Service.Controllers.Users.Models;

public record UpdateUserRequest(
    string? Email,
    string? UserName,
    string? Password);