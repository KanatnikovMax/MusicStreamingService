namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public record ChangeUserNameModel(
    string UserName,
    string Password);