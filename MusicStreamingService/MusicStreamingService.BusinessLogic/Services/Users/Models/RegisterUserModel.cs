using System.ComponentModel.DataAnnotations;

namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public record RegisterUserModel(
    string Email,
    string UserName,
    [DataType(DataType.Password)]
    string Password);