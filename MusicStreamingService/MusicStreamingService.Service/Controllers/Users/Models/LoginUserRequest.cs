using System.ComponentModel.DataAnnotations;

namespace MusicStreamingService.Service.Controllers.Users.Models;

public record LoginUserRequest(
    string UserName,
    [DataType(DataType.Password)]
    string Password);