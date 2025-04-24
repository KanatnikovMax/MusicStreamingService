namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public record ChangePasswordModel(
    string CurrentPassword,
    string NewPassword);