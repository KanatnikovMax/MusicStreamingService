namespace MusicStreamingService.Service.Controllers.Requests.Users;

public record ChangeUserNameRequest(
    string UserName,
    string Password);