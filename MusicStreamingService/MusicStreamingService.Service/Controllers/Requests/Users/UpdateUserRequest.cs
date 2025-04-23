namespace MusicStreamingService.Service.Controllers.Requests.Users;

public record UpdateUserRequest(
    string? Email,
    string? UserName,
    string? Password);