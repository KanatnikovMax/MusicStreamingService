using System.ComponentModel.DataAnnotations;

namespace MusicStreamingService.Service.Controllers.Users.Models;

public record RegisterUserRequest(
    string Email,
    string UserName,
    [DataType(DataType.Password)]
    string Password);