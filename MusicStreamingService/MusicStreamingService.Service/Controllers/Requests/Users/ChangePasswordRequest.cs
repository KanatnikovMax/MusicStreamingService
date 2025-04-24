namespace MusicStreamingService.Service.Controllers.Requests.Users;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);