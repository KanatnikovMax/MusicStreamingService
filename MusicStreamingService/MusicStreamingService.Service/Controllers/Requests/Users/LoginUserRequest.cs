using System.ComponentModel.DataAnnotations;

namespace MusicStreamingService.Service.Controllers.Requests.Users;

public record LoginUserRequest(
    string UserName,
    [DataType(DataType.Password)]
    string Password);