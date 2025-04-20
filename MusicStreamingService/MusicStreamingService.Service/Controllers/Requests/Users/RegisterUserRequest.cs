using System.ComponentModel.DataAnnotations;

namespace MusicStreamingService.Service.Controllers.Requests.Users;

public record RegisterUserRequest(
    string Email,
    string UserName,
    [DataType(DataType.Password)]
    string Password);