using System.ComponentModel.DataAnnotations;

namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public record LoginUserModel(
    string UserName,
    [DataType(DataType.Password)]
    string Password);