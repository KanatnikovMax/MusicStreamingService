namespace MusicStreamingService.Service.Controllers.Requests.Users;

public record ChangeEmailRequest(
    string Email,
    string Password);