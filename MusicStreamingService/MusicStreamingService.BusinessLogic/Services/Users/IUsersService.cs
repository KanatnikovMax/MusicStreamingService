using MusicStreamingService.BusinessLogic.Services.Users.Models;

namespace MusicStreamingService.BusinessLogic.Services.Users;

public interface IUsersService
{
    Task<TokenResponce> RegisterUserAsync(RegisterUserModel model);
    Task<TokenResponce> LoginUserAsync(LoginUserModel model);
}